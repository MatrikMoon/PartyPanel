using IPA;
using SongCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = PartyPanelShared.Logger;

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

        private AlwaysOwnedContentSO _alwaysOwnedContent;
        private BeatmapLevelCollectionSO _primaryLevelCollection;
        private BeatmapLevelCollectionSO _secondaryLevelCollection;
        private BeatmapLevelCollectionSO _tertiaryLevelCollection;
        private BeatmapLevelCollectionSO _extrasLevelCollection;

        public static List<IPreviewBeatmapLevel> masterLevelList;

        private Client client;

        public void OnApplicationStart()
        {
            client = new Client();
            client.Start();

            Loader.SongsLoadedEvent += (Loader _, Dictionary<string, CustomPreviewBeatmapLevel> __) =>
            {
                if (_alwaysOwnedContent == null) _alwaysOwnedContent = Resources.FindObjectsOfTypeAll<AlwaysOwnedContentSO>().First();
                if (_primaryLevelCollection == null) _primaryLevelCollection = _alwaysOwnedContent.alwaysOwnedPacks.First(x => x.packID == OstHelper.packs[0].PackID).beatmapLevelCollection as BeatmapLevelCollectionSO;
                if (_secondaryLevelCollection == null) _secondaryLevelCollection = _alwaysOwnedContent.alwaysOwnedPacks.First(x => x.packID == OstHelper.packs[1].PackID).beatmapLevelCollection as BeatmapLevelCollectionSO;
                if (_tertiaryLevelCollection == null) _tertiaryLevelCollection = _alwaysOwnedContent.alwaysOwnedPacks.First(x => x.packID == OstHelper.packs[2].PackID).beatmapLevelCollection as BeatmapLevelCollectionSO;
                if (_extrasLevelCollection == null) _extrasLevelCollection = _alwaysOwnedContent.alwaysOwnedPacks.First(x => x.packID == OstHelper.packs[3].PackID).beatmapLevelCollection as BeatmapLevelCollectionSO;

                masterLevelList = new List<IPreviewBeatmapLevel>();
                masterLevelList.AddRange(_primaryLevelCollection.beatmapLevels);
                masterLevelList.AddRange(_secondaryLevelCollection.beatmapLevels);
                masterLevelList.AddRange(_tertiaryLevelCollection.beatmapLevels);
                masterLevelList.AddRange(_extrasLevelCollection.beatmapLevels);
                masterLevelList.AddRange(Loader.CustomLevelsCollection.beatmapLevels);

                client.SendSongList(masterLevelList);
            };
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
