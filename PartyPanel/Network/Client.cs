﻿using PartyPanelShared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PartyPanel.Network
{
    public class ClientPlayer
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public List<byte> accumulatedBytes = new List<byte>();
    }

    public class Client
    {
        public event Action<Packet> PacketRecieved;
        public event Action ServerDisconnected;

        // The port number for the remote device.  
        private int port;
        private ClientPlayer player;

        private static ManualResetEvent connectDone = new ManualResetEvent(false);

        public Client(int port)
        {
            this.port = port;
        }

        public void Start()
        {
            IPAddress ipAddress = IPAddress.Loopback;
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                // Create the player object.
                player = new ClientPlayer();
                player.workSocket = client;

                //Signal to continue after connect
                connectDone.Set();

                // Begin receiving the data from the remote device.  
                client.BeginReceive(player.buffer, 0, ClientPlayer.BufferSize, 0, new AsyncCallback(ReadCallback), player);
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString());
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                ClientPlayer player = (ClientPlayer)ar.AsyncState;
                Socket client = player.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    var currentBytes = new byte[bytesRead];
                    Buffer.BlockCopy(player.buffer, 0, currentBytes, 0, bytesRead);

                    player.accumulatedBytes.AddRange(currentBytes);
                    if (player.accumulatedBytes.Count >= Packet.packetHeaderSize)
                    {
                        //If we're not at the start of a packet, increment our position until we are, or we run out of bytes
                        var accumulatedBytes = player.accumulatedBytes.ToArray();
                        while (!Packet.StreamIsAtPacket(accumulatedBytes) && accumulatedBytes.Length >= Packet.packetHeaderSize)
                        {
                            player.accumulatedBytes.RemoveAt(0);
                            accumulatedBytes = player.accumulatedBytes.ToArray();
                        }

                        if (Packet.PotentiallyValidPacket(accumulatedBytes))
                        {
                            PacketRecieved?.Invoke(Packet.FromBytes(accumulatedBytes));
                            player.accumulatedBytes.Clear();
                        }
                    }

                    // Get the rest of the data.  
                    client.BeginReceive(player.buffer, 0, ClientPlayer.BufferSize, 0, new AsyncCallback(ReadCallback), player);
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString());
                ServerDisconnected_Internal();
            }
        }

        public void Send(byte[] data)
        {
            player.workSocket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), player.workSocket);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString());
                ServerDisconnected_Internal();
            }
        }

        private void ServerDisconnected_Internal()
        {
            Shutdown();
            ServerDisconnected?.Invoke();
        }

        public void Shutdown()
        {
            if (player.workSocket.Connected) player.workSocket.Shutdown(SocketShutdown.Both);
            player.workSocket.Close();
        }
    }
}
