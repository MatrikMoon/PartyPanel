using PartyPanelShared;
using PartyPanelShared.Models;
using PartyPanelUI.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyPanelUI
{
    public class Server
    {
        private PartyPanel panel;
        private Network.Server server;

        public Server(PartyPanel panel)
        {
            this.panel = panel;
        }

        public void Start()
        {
            server = new Network.Server(10155);
            panel.SetServer(server);
            server.PacketRecieved += Server_PacketRecieved;
            server.PlayerConnected += Server_PlayerConnected;
            server.PlayerDisconnected += Server_PlayerDisconnected;
            server.Start();
        }

        private void Server_PlayerDisconnected(NetworkPlayer obj)
        {
            Logger.Debug("Player Disconnected!");
            panel.DisableSongList();
        }

        private void Server_PlayerConnected(NetworkPlayer obj)
        {
            Logger.Debug("Player Connected!");
        }

        private void Server_PacketRecieved(NetworkPlayer player, Packet packet)
        {
            if (packet.Type == PacketType.SongList)
            {
                SongList masterLevelCollection = packet.SpecificPacket as SongList;
                panel.PopulatePartyPanel(masterLevelCollection.Levels.ToList());
            }
            else if (packet.Type == PacketType.LoadedSong)
            {
                LoadedSong loadedSong = packet.SpecificPacket as LoadedSong;
                panel.SongLoaded(loadedSong.level);
            }
        }
    }
}
