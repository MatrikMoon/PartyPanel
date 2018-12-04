using SongLoaderPlugin;
using SongLoaderPlugin.OverrideClasses;
using System;
using System.Linq;
using UnityEngine;

namespace PartyPanel
{
    class SaberUtilities
    {
        public static void PlaySong(string levelID, BeatmapDifficulty difficulty, GameplayModifiers gameplayModifiers = null, PlayerSpecificSettings playerSettings = null)
        {
            IBeatmapLevel level = SongLoader.CustomLevels.Where(x => x.levelID == levelID).FirstOrDefault();

            //There really is no safety check for this. Oh well.
            if (level == null) level = Resources.FindObjectsOfTypeAll<LevelCollectionSO>().First().levels.First(x => x.levelID == levelID);

            if (level != null)
            {
                //Our callback for when the audio is loaded
                Action<IBeatmapLevel> songLoadedCallback = (IBeatmapLevel loadedLevel) =>
                {
                    MenuSceneSetupDataSO _menuSceneSetupData = Resources.FindObjectsOfTypeAll<MenuSceneSetupDataSO>().FirstOrDefault();
                    _menuSceneSetupData.StartStandardLevel(
                        loadedLevel.GetDifficultyBeatmap(difficulty),
                        gameplayModifiers ?? new GameplayModifiers(),
                        playerSettings ?? new PlayerSpecificSettings(),
                        null,
                        null,
                        null
                    );
                };

                //Load audio if it's custom
                if (level is CustomLevel)
                {
                    SongLoader.Instance.LoadAudioClipForLevel((CustomLevel)level, songLoadedCallback);
                }
                else
                {
                    songLoadedCallback(level);
                }
            }
        }

        //Returns the closest difficulty to the one provided, preferring lower difficulties first if any exist
        private static IDifficultyBeatmap GetClosestDifficultyPreferLower(IBeatmapLevel level, BeatmapDifficulty difficulty)
        {
            IDifficultyBeatmap ret = level.GetDifficultyBeatmap(difficulty);
            if (ret == null)
            {
                ret = GetLowerDifficulty(level, difficulty);
            }
            if (ret == null)
            {
                ret = GetHigherDifficulty(level, difficulty);
            }
            return ret;
        }

        //Returns the next-lowest difficulty to the one provided
        private static IDifficultyBeatmap GetLowerDifficulty(IBeatmapLevel level, BeatmapDifficulty difficulty)
        {
            IDifficultyBeatmap[] availableMaps = level.difficultyBeatmaps.OrderBy(x => x.difficulty).ToArray();
            return availableMaps.TakeWhile(x => x.difficulty < difficulty).LastOrDefault();
        }

        //Returns the next-highest difficulty to the one provided
        private static IDifficultyBeatmap GetHigherDifficulty(IBeatmapLevel level, BeatmapDifficulty difficulty)
        {
            IDifficultyBeatmap[] availableMaps = level.difficultyBeatmaps.OrderBy(x => x.difficulty).ToArray();
            return availableMaps.SkipWhile(x => x.difficulty < difficulty).FirstOrDefault();
        }
    }
}
