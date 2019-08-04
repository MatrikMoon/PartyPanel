﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace PartyPanelShared
{
    public enum PacketType
    {
        SongList,
        Command,
        LoadSong,
        LoadedSong,
        PlaySong
    }

    public class Packet
    {
        //Size of the header, the info we need to parse the specific packet
        // 4x byte - "moon"
        // int - packet type
        // int - packet size
        public const int packetHeaderSize = (sizeof(int) * 2) + (sizeof(byte) * 4);

        public int Size { get; set; }
        public PacketType Type { get; set; }

        public object SpecificPacket { get; set; }

        public Packet(object specificPacket)
        {
            //Assign type based on parameter type
            Type = (PacketType)Enum.Parse(typeof(PacketType), specificPacket.GetType().Name);

            SpecificPacket = specificPacket;
        }

        public byte[] ToBytes()
        {
            byte[] specificPacketBytes = null;

            using (var memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Binder = new CustomSerializationBinder();
                binaryFormatter.Serialize(memoryStream, SpecificPacket);
                specificPacketBytes = memoryStream.ToArray();
            }

            var magicFlag = Encoding.UTF8.GetBytes("moon");
            var typeBytes = BitConverter.GetBytes((int)Type);
            var sizeBytes = BitConverter.GetBytes(specificPacketBytes.Length);

            return Combine(magicFlag, typeBytes, sizeBytes, specificPacketBytes);
        }

        public string ToBase64() => Convert.ToBase64String(ToBytes());

        public static Packet FromBytes(byte[] bytes) => FromStream(new MemoryStream(bytes));

        public static Packet FromStream(MemoryStream stream)
        {
            var typeBytes = new byte[sizeof(int)];
            var sizeBytes = new byte[sizeof(int)];

            //Verify that this is indeed a Packet
            if (!StreamIsAtPacket(stream, false))
            {
                stream.Seek(-(sizeof(byte) * 4), SeekOrigin.Current); //Return to original position in stream
                return null;
            }

            stream.Read(typeBytes, 0, sizeof(int));
            stream.Read(sizeBytes, 0, sizeof(int));

            var specificPacketSize = BitConverter.ToInt32(sizeBytes, 0);
            object specificPacket = null;

            //There needn't mecessarily be a specific packet for every packet (acks)
            if (specificPacketSize > 0)
            {
                var specificPacketBytes = new byte[specificPacketSize];

                stream.Read(specificPacketBytes, 0, specificPacketBytes.Length);

                using (var memStream = new MemoryStream())
                {
                    memStream.Write(specificPacketBytes, 0, specificPacketBytes.Length);
                    memStream.Seek(0, SeekOrigin.Begin);

                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Binder = new CustomSerializationBinder();
                    specificPacket = binaryFormatter.Deserialize(memStream);
                }
            }

            return new Packet(specificPacket)
            {
                Size = specificPacketSize,
                Type = (PacketType)BitConverter.ToInt32(typeBytes, 0)
            };
        }

        public static bool StreamIsAtPacket(byte[] bytes, bool resetStreamPos = true) => StreamIsAtPacket(new MemoryStream(bytes), resetStreamPos);
        public static bool StreamIsAtPacket(MemoryStream stream, bool resetStreamPos = true)
        {
            var magicFlagBytes = new byte[sizeof(byte) * 4];

            //Verify that this is indeed a Packet
            stream.Read(magicFlagBytes, 0, sizeof(byte) * 4);

            if (resetStreamPos) stream.Seek(-(sizeof(byte) * 4), SeekOrigin.Current); //Return to original position in stream

            return Encoding.UTF8.GetString(magicFlagBytes) == "moon";
        }

        public static bool PotentiallyValidPacket(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);

            var typeBytes = new byte[sizeof(int)];
            var sizeBytes = new byte[sizeof(int)];

            //Verify that this is indeed a Packet
            if (!StreamIsAtPacket(stream, false))
            {
                stream.Seek(-(sizeof(byte) * 4), SeekOrigin.Current); //Return to original position in stream
                return false;
            }

            stream.Read(typeBytes, 0, sizeof(int));
            stream.Read(sizeBytes, 0, sizeof(int));

            stream.Seek(-(sizeof(byte) * 4 + sizeof(int) * 2), SeekOrigin.Current); //Return to original position in stream

            return (BitConverter.ToInt32(sizeBytes, 0) + packetHeaderSize) <= bytes.Length;
        }

        private static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}
