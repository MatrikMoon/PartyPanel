using SongLoaderPlugin;
using SongLoaderPlugin.OverrideClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PartyPanel
{
    class SaberUtilities
    {
        public static void PlaySong(string levelID, LevelDifficulty? difficulty = null, GameplayOptions options = null)
        {
            var level = SongLoader.CustomLevels.Where(x => x.levelID == levelID).FirstOrDefault();

            if (level)
            {
                //Our callback for when the audio is loaded
                Action<IStandardLevel> songLoadedCallback = (IStandardLevel loadedLevel) =>
                {
                    MainGameSceneSetupData mainGameSceneSetupData = Resources.FindObjectsOfTypeAll<MainGameSceneSetupData>().FirstOrDefault();
                    mainGameSceneSetupData.Init(
                        GetClosestDifficultyPreferLower(level, difficulty == null ? LevelDifficulty.Easy : (LevelDifficulty)difficulty),
                        options,
                        GameplayMode.PartyStandard,
                        0f);
                    mainGameSceneSetupData.didFinishEvent -= SongFinished;
                    mainGameSceneSetupData.didFinishEvent += SongFinished;
                    mainGameSceneSetupData.TransitionToScene(0.7f);
                };

                //Load audio if it's custom
                if (level is CustomLevel)
                {
                    SongLoader.Instance.LoadAudioClipForLevel(level, songLoadedCallback);
                }
                else
                {
                    songLoadedCallback(level);
                }
            }
        }

        private static void SongFinished(MainGameSceneSetupData mainGameSceneSetupData, LevelCompletionResults results)
        {
            mainGameSceneSetupData.didFinishEvent -= SongFinished;
            Resources.FindObjectsOfTypeAll<MenuSceneSetupData>().First().TransitionToScene((results == null) ? 0.35f : 1.3f);
        }

        //Returns the closest difficulty to the one provided, preferring lower difficulties first if any exist
        private static IStandardLevelDifficultyBeatmap GetClosestDifficultyPreferLower(IStandardLevel level, LevelDifficulty difficulty)
        {
            IStandardLevelDifficultyBeatmap ret = level.GetDifficultyLevel(difficulty);
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
        private static IStandardLevelDifficultyBeatmap GetLowerDifficulty(IStandardLevel level, LevelDifficulty difficulty)
        {
            IStandardLevelDifficultyBeatmap[] availableMaps = level.difficultyBeatmaps.OrderBy(x => x.difficulty).ToArray();
            return availableMaps.TakeWhile(x => x.difficulty < difficulty).LastOrDefault();
        }

        //Returns the next-highest difficulty to the one provided
        private static IStandardLevelDifficultyBeatmap GetHigherDifficulty(IStandardLevel level, LevelDifficulty difficulty)
        {
            IStandardLevelDifficultyBeatmap[] availableMaps = level.difficultyBeatmaps.OrderBy(x => x.difficulty).ToArray();
            return availableMaps.SkipWhile(x => x.difficulty < difficulty).FirstOrDefault();
        }
    }
}
