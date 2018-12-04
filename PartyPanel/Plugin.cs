using IllusionPlugin;
using SongLoaderPlugin;
using SongLoaderPlugin.OverrideClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    public class Plugin : IPlugin
    {
        public string Name => "PartyPanel";
        public string Version => "0.0.1";
        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            PartyPanel panel = new PartyPanel();
            panel.Show();

            SongLoader.SongsLoadedEvent += (SongLoader sender, List<CustomLevel> loadedSongs) =>
            {
                //No safety checks here
                var levels = new List<IBeatmapLevel>();
                levels.AddRange(Resources.FindObjectsOfTypeAll<LevelCollectionSO>().First().levels);
                SharedCoroutineStarter.instance.StartCoroutine(PopulatePartyPanel(panel, levels));
            };
        }

        //Load song list into the control panel without freezing the game
        private IEnumerator PopulatePartyPanel(PartyPanel panel, List<IBeatmapLevel> loadedSongs)
        {
            panel.g_songList.Enabled = false;
            foreach (IBeatmapLevel x in loadedSongs)
            {
                if (x is CustomLevel)
                {
                    try
                    {
                        var songIcon = Image.FromFile($"{((CustomLevel)x).customSongInfo.path}/{((CustomLevel)x).customSongInfo.coverImagePath}");
                        panel.g_songList.SmallImageList.Images.Add(x.levelID, songIcon);
                        panel.g_songList.LargeImageList.Images.Add(x.levelID, songIcon);
                    }
                    catch { }
                }

                if (x.levelID.Contains("OneSaber") || x.levelID.Contains("NoArrows"))
                {
                    yield return panel.g_songList.Items.Add(x.levelID, $"{x.songName}({x.levelID})", x.levelID);
                }
                else yield return panel.g_songList.Items.Add(x.levelID, x.songName, x.levelID);
            }
            panel.masterLevelList = loadedSongs;
            panel.g_songList.Enabled = true;
            panel.g_searchBox.Visible = true;
            panel.g_searchLabel.Text = "Search:";
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
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
