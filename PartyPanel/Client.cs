using PartyPanelShared;
using PartyPanelShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace PartyPanel
{
    class Client
    {
        private Network.Client client;
        private Timer heartbeatTimer = new Timer();

        public void Start()
        {
            heartbeatTimer.Interval = 10000;
            heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
            heartbeatTimer.Start();
        }

        private void HeartbeatTimer_Elapsed(object _, ElapsedEventArgs __)
        {
            try
            {
                var command = new Command();
                command.commandType = Command.CommandType.Heartbeat;
                client.Send(new Packet(command).ToBytes());
            }
            catch (Exception e)
            {
                Logger.Debug("HEARTBEAT FAILED");
                Logger.Debug(e.ToString());

                ConnectToServer();
            }
        }

        private void ConnectToServer()
        {
            try
            {
                client = new Network.Client(10155);
                client.PacketRecieved += Client_PacketRecieved;
                client.ServerDisconnected += Client_ServerDisconnected;
                client.Start();

                //Send the server the master list if we can
                if (Plugin.masterLevelList != null)
                {
                    var subpacketList = new List<PreviewBeatmapLevel>();
                    subpacketList.AddRange(
                        Plugin.masterLevelList.Select(x => {
                            var level = new PreviewBeatmapLevel();
                            level.Name = x.songName;
                            level.LevelId = x.levelID;
                            return level;
                        })
                    );

                    var songList = new SongList();
                    songList.Levels = subpacketList.ToArray();

                    client.Send(new Packet(songList).ToBytes());
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString());
            }
        }

        private void Client_ServerDisconnected()
        {
            Logger.Debug("Server disconnected!");
        }

        public void SendSongList(List<IPreviewBeatmapLevel> levels)
        {
            if (client != null)
            {
                var subpacketList = new List<PreviewBeatmapLevel>();
                subpacketList.AddRange(
                    levels.Select(x =>
                    {
                        var level = new PreviewBeatmapLevel();
                        level.LevelId = x.levelID;
                        level.Name = x.songName;
                        return level;
                    })
                );

                var songList = new SongList();
                songList.Levels = subpacketList.ToArray();

                client.Send(new Packet(songList).ToBytes());
            }
            else Logger.Debug("Skipped sending songs because there is no server connected");
        }

        private void Client_PacketRecieved(Packet packet)
        {
            if (packet.Type == PacketType.PlaySong)
            {
                PlaySong playSong = packet.SpecificPacket as PlaySong;

                var desiredLevel = Plugin.masterLevelList.First(x => x.levelID == playSong.levelId);
                var desiredCharacteristic = desiredLevel.beatmapCharacteristics.First(x => x.serializedName == playSong.characteristic.SerializedName);
                var desiredDifficulty = (BeatmapDifficulty)playSong.difficulty;

                var playerSpecificSettings = new PlayerSpecificSettings();
                playerSpecificSettings.advancedHud = playSong.playerSettings.advancedHud;
                playerSpecificSettings.leftHanded = playSong.playerSettings.leftHanded;
                playerSpecificSettings.noTextsAndHuds = playSong.playerSettings.noTextsAndHuds;
                playerSpecificSettings.reduceDebris = playSong.playerSettings.reduceDebris;
                playerSpecificSettings.staticLights = playSong.playerSettings.staticLights;

                var gameplayModifiers = new GameplayModifiers();
                gameplayModifiers.batteryEnergy = playSong.gameplayModifiers.batteryEnergy;
                gameplayModifiers.disappearingArrows = playSong.gameplayModifiers.disappearingArrows;
                gameplayModifiers.failOnSaberClash = playSong.gameplayModifiers.failOnSaberClash;
                gameplayModifiers.fastNotes = playSong.gameplayModifiers.fastNotes;
                gameplayModifiers.ghostNotes = playSong.gameplayModifiers.ghostNotes;
                gameplayModifiers.instaFail = playSong.gameplayModifiers.instaFail;
                gameplayModifiers.noBombs = playSong.gameplayModifiers.noBombs;
                gameplayModifiers.noFail = playSong.gameplayModifiers.noFail;
                gameplayModifiers.noObstacles = playSong.gameplayModifiers.noObstacles;
                gameplayModifiers.songSpeed = (GameplayModifiers.SongSpeed)playSong.gameplayModifiers.songSpeed;

                SaberUtilities.PlaySong(desiredLevel, desiredCharacteristic, desiredDifficulty, gameplayModifiers, playerSpecificSettings);
            }
            else if (packet.Type == PacketType.LoadSong)
            {
                LoadSong loadSong = packet.SpecificPacket as LoadSong;

                Action<IBeatmapLevel> SongLoaded = (loadedLevel) =>
                {
                    var loadedSong = new LoadedSong();
                    var beatmapLevel = new PreviewBeatmapLevel();
                    beatmapLevel.Characteristics = loadedLevel.beatmapCharacteristics.ToList().Select(x => {
                        var characteristic = new Characteristic();
                        characteristic.SerializedName = x.serializedName;
                        characteristic.difficulties =
                            loadedLevel.beatmapLevelData.difficultyBeatmapSets
                                .First(y => y.beatmapCharacteristic.serializedName == x.serializedName)
                                .difficultyBeatmaps.Select(y => (Characteristic.BeatmapDifficulty)y.difficulty).ToArray();

                        return characteristic;
                    }).ToArray();

                    beatmapLevel.LevelId = loadedLevel.levelID;
                    beatmapLevel.Name = loadedLevel.songName;
                    beatmapLevel.Loaded = true;

                    loadedSong.level = beatmapLevel;

                    client.Send(new Packet(loadedSong).ToBytes());
                };

                LoadSong(loadSong.levelId, SongLoaded);
            }
            else if (packet.Type == PacketType.Command)
            {
                Command command = packet.SpecificPacket as Command;
                if (command.commandType == Command.CommandType.ReturnToMenu)
                {
                    SaberUtilities.ReturnToMenu();
                }
            }
        }

        private async void LoadSong(string levelId, Action<IBeatmapLevel> loadedCallback)
        {
            IPreviewBeatmapLevel level = Plugin.masterLevelList.Where(x => x.levelID == levelId).First();

            //Load IBeatmapLevel
            if (level is PreviewBeatmapLevelSO || level is CustomPreviewBeatmapLevel)
            {
                if (level is PreviewBeatmapLevelSO)
                {
                    if (!await SaberUtilities.HasDLCLevel(level.levelID)) return; //In the case of unowned DLC, just bail out and do nothing
                }

                var map = ((CustomPreviewBeatmapLevel)level).standardLevelInfoSaveData.difficultyBeatmapSets.First().difficultyBeatmaps.First();

                var result = await SaberUtilities.GetLevelFromPreview(level);
                if (result != null && !(result?.isError == true))
                {
                    loadedCallback(result?.beatmapLevel);
                }
            }
            else if (level is BeatmapLevelSO)
            {
                loadedCallback(level as IBeatmapLevel);
            }
        }
    }
}
