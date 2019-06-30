using IPA;
using SongCore;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Created by Moon on 11/12/2018
 * 
 * This plugin is designed to provide a user interface to launch songs
 * without being in the game
 */

namespace PartyPanel
{
    public class Plugin : IBeatSaberPlugin
    {
        public string Name => "PartyPanel";
        public string Version => "0.0.1";

        private AlwaysOwnedContentModelSO _alwaysOwnedContentModel;
        private BeatmapLevelCollectionSO _primaryLevelCollection;
        private BeatmapLevelCollectionSO _secondaryLevelCollection;
        private BeatmapLevelCollectionSO _extrasLevelCollection;

        public void OnApplicationStart()
        {
            PartyPanel panel = new PartyPanel();
            panel.Show();

            Loader.SongsLoadedEvent += (Loader _, Dictionary<string, CustomPreviewBeatmapLevel> __) =>
            {
                if (_alwaysOwnedContentModel == null) _alwaysOwnedContentModel = Resources.FindObjectsOfTypeAll<AlwaysOwnedContentModelSO>().First();
                if (_primaryLevelCollection == null) _primaryLevelCollection = _alwaysOwnedContentModel.alwaysOwnedPacks.First(x => x.packID == OstHelper.packs[0].PackID).beatmapLevelCollection as BeatmapLevelCollectionSO;
                if (_secondaryLevelCollection == null) _secondaryLevelCollection = _alwaysOwnedContentModel.alwaysOwnedPacks.First(x => x.packID == OstHelper.packs[1].PackID).beatmapLevelCollection as BeatmapLevelCollectionSO;
                if (_extrasLevelCollection == null) _extrasLevelCollection = _alwaysOwnedContentModel.alwaysOwnedPacks.First(x => x.packID == OstHelper.packs[2].PackID).beatmapLevelCollection as BeatmapLevelCollectionSO;

                var levels = new List<IPreviewBeatmapLevel>();
                levels.AddRange(_primaryLevelCollection.beatmapLevels);
                levels.AddRange(_secondaryLevelCollection.beatmapLevels);
                levels.AddRange(_extrasLevelCollection.beatmapLevels);
                levels.AddRange(Loader.CustomLevelsCollection.beatmapLevels);
                PopulatePartyPanel(panel, levels);
            };
        }

        //Load song list into the control panel without freezing the game
        private void PopulatePartyPanel(PartyPanel panel, List<IPreviewBeatmapLevel> loadedSongs)
        {
            panel.g_songList.Enabled = false;
            panel.g_songList.Items.Clear();

            /*
            foreach (IPreviewBeatmapLevel x in loadedSongs)
            {
                if (panel.g_artCheckBox.Checked && x is CustomPreviewBeatmapLevel)
                {
                    try
                    {
                        var songPath = ((CustomPreviewBeatmapLevel)x).customLevelPath;
                        var coverArtFileName = Directory.EnumerateFiles(songPath).First(y => y.StartsWith("cover."));
                        var songIcon = Image.FromFile($"{songPath}/{coverArtFileName}");
                        panel.g_songList.SmallImageList.Images.Add(x.levelID, songIcon);
                        panel.g_songList.LargeImageList.Images.Add(x.levelID, songIcon);
                    }
                    catch { }
                }

                yield return panel.g_songList.Items.Add(x.levelID, x.songName, x.levelID);
            }
            */

            panel.g_songList.Items.AddRange(loadedSongs.Select(x => new System.Windows.Forms.ListViewItem(x.songName, x.levelID)).ToArray());

            panel.masterLevelList = loadedSongs;
            panel.g_songList.Enabled = true;
            panel.g_searchBox.Visible = true;
            panel.g_searchLabel.Text = "Search:";
        }

        public void OnApplicationQuit()
        {
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
        }

        public void OnSceneUnloaded(Scene scene)
        {
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }
    }
}
