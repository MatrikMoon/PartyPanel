using SongLoaderPlugin;
using SongLoaderPlugin.OverrideClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

namespace PartyPanel
{
    public partial class PartyPanel : Form
    {
        public Label g_searchLabel;
        public TextBox g_searchBox;
        public ListView g_songList;
        public List<BeatmapLevelSO> masterLevelList;

        public PartyPanel()
        {
            InitializeComponent();

            songListView.LargeImageList = new ImageList();
            songListView.SmallImageList = new ImageList();
            g_songList = songListView;
            g_songList.LargeImageList.ImageSize = new System.Drawing.Size(64, 64);
            g_searchLabel = searchLabel;
            g_searchBox = searchBox;
        }

        private void songListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (songListView.SelectedItems.Count >= 1)
                {
                    var levelID = songListView.SelectedItems[0].Name;
                    BeatmapLevelSO level = SongLoader.CustomLevels.Where(x => x.levelID == levelID).FirstOrDefault();
                    
                    //There really is no safety check for this. Again.
                    if (level == null) level = masterLevelList.First(x => x.levelID == levelID);

                    if (level != null)
                    {
                        var desiredCharacteristic = level.beatmapCharacteristics.FirstOrDefault(x => x.serializedName == "Standard") ?? level.beatmapCharacteristics.First();

                        difficultyDropdown.Items.Clear();
                        level.difficultyBeatmapSets
                            .ToList().First(x => x.beatmapCharacteristic == desiredCharacteristic).difficultyBeatmaps
                            .ToList().ForEach(x => difficultyDropdown.Items.Add(x.difficulty));
                        difficultyDropdown.SelectedIndex = difficultyDropdown.Items.Count - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (songListView.SelectedItems.Count > 0 && difficultyDropdown.SelectedIndex >= 0)
            {
                var playerSettings = new PlayerSpecificSettings();
                playerSettings.leftHanded = mirrorCheckbox.Checked;
                playerSettings.staticLights = staticLightsCheckbox.Checked;
                playerSettings.noTextsAndHuds = noHudCheckbox.Checked;
                playerSettings.advancedHud= advancedHudCheckbox.Checked;
                playerSettings.reduceDebris = reduceDebrisCheckbox.Checked;

                var modifiers = new GameplayModifiers();
                modifiers.noFail = noFailCheckbox.Checked;
                modifiers.noBombs = noBombsCheckbox.Checked;
                modifiers.noObstacles = noWallsCheckbox.Checked;
                modifiers.instaFail = instaFailCheckbox.Checked && !modifiers.noFail;
                modifiers.failOnSaberClash = failOnClashCheckbox.Checked;
                modifiers.batteryEnergy = batteryEnergyCheckbox.Checked && !modifiers.noFail && !modifiers.instaFail;
                modifiers.fastNotes = fastNotesCheckbox.Checked;
                modifiers.songSpeed = fastSongCheckbox.Checked ?
                    GameplayModifiers.SongSpeed.Faster :
                    slowSongCheckbox.Checked ? 
                        GameplayModifiers.SongSpeed.Slower :
                        GameplayModifiers.SongSpeed.Normal;
                modifiers.disappearingArrows = disappearingArrowsCheckbox.Checked;
                modifiers.ghostNotes = ghostNotesCheckbox.Checked;

                var level = masterLevelList.First(x => x.levelID == songListView.SelectedItems[0].Name);
                var desiredCharacteristic = level.beatmapCharacteristics.FirstOrDefault(x => x.serializedName == "Standard") ?? level.beatmapCharacteristics.First();

                SaberUtilities.PlaySong(level.difficultyBeatmapSets.First(x => x.beatmapCharacteristic == desiredCharacteristic).difficultyBeatmaps.First(x => x.difficulty == (BeatmapDifficulty)difficultyDropdown.SelectedItem), modifiers, playerSettings); //`Name` is the key we passed in on creation. Weird naming scheme.
            }
        }

        private void returnToMenuButton_Click(object sender, EventArgs e)
        {
            Resources.FindObjectsOfTypeAll<ScenesTransitionSetupDataSO>().FirstOrDefault()?.PopScenes(0.35f);
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            songListView.Clear();
            if (searchBox.Text.Length <= 0)
            {
                songListView.Items.AddRange(
                masterLevelList
                    .Select(x => {
                        var item = new ListViewItem(x.songName, x.levelID);
                        item.Name = x.levelID; //LevelID used for launching game. AKA `key` in the Items.Add parameters
                        return item;
                    })
                    .ToArray()
                );
            }
            else
            {
                songListView.Items.AddRange(
                masterLevelList
                    .Where(x => x.songName.ToLower().Contains(searchBox.Text.ToLower()))
                    .Select(x => {
                        var item = new ListViewItem(x.songName, x.levelID);
                        item.Name = x.levelID; //LevelID used for launching game. AKA `key` in the Items.Add parameters
                        return item;
                    })
                    .ToArray()
                );
            }
        }
    }
}
