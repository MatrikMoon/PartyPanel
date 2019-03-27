using SongLoaderPlugin;
using SongLoaderPlugin.OverrideClasses;
using System;
using System.Linq;
using UnityEngine;

namespace PartyPanel
{
    class SaberUtilities
    {
        public static void PlaySong(IDifficultyBeatmap map, GameplayModifiers gameplayModifiers = null, PlayerSpecificSettings playerSettings = null)
        {
            //Our callback for when the audio is loaded
            Action<IBeatmapLevel> songLoadedCallback = (_) =>
            {
                MenuTransitionsHelperSO menuTransitionHelper = Resources.FindObjectsOfTypeAll<MenuTransitionsHelperSO>().FirstOrDefault();
                menuTransitionHelper.StartStandardLevel(
                    map,
                    gameplayModifiers ?? new GameplayModifiers(),
                    playerSettings ?? new PlayerSpecificSettings(),
                    null,
                    null,
                    null
                );
            };

            //Load audio if it's custom
            if (map.level is CustomLevel)
            {
                SongLoader.Instance.LoadAudioClipForLevel((CustomLevel)map.level, songLoadedCallback);
            }
            else
            {
                songLoadedCallback(map.level);
            }
        }
    }
}
