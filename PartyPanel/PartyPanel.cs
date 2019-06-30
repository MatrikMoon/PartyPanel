using SongCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;

namespace PartyPanel
{
    public partial class PartyPanel : Form
    {
        public Label g_searchLabel;
        public TextBox g_searchBox;
        public ListView g_songList;
        public CheckBox g_artCheckBox;
        public List<IPreviewBeatmapLevel> masterLevelList;

        private IBeatmapLevel currentSelection;

        public PartyPanel()
        {
            InitializeComponent();

            songListView.LargeImageList = new ImageList();
            songListView.SmallImageList = new ImageList();
            g_songList = songListView;
            g_songList.LargeImageList.ImageSize = new System.Drawing.Size(64, 64);
            g_searchLabel = searchLabel;
            g_searchBox = searchBox;
            g_artCheckBox = artBox;
        }

        private async void songListView_SelectedIndexChangedAsync(object sender, EventArgs e)
        {
            if (songListView.SelectedItems.Count >= 1)
            {
                var levelID = songListView.SelectedItems[0].ImageKey;
                IPreviewBeatmapLevel level = masterLevelList.Where(x => x.levelID == levelID).First();

                //Clear out old info
                characteristicDropdown.Items.Clear();
                difficultyDropdown.Items.Clear();

                //Callback for when the IBeatmapLevel is loaded
                Action<IBeatmapLevel> SongLoaded = (loadedLevel) =>
                {
                    currentSelection = loadedLevel;

                    loadedLevel.beatmapCharacteristics.ToList().ForEach(x => characteristicDropdown.Items.Add(x.serializedName));
                    characteristicDropdown.SelectedIndex = 0;

                    //We'll assume we've currently selected the 0th checkbox, since, well, we have
                    loadedLevel
                        .beatmapLevelData.difficultyBeatmapSets.First(x => x.beatmapCharacteristic == loadedLevel.beatmapCharacteristics.First())
                        .difficultyBeatmaps.ToList().ForEach(x => difficultyDropdown.Items.Add(x.difficulty));
                    difficultyDropdown.SelectedIndex = difficultyDropdown.Items.Count - 1;
                };

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
                        SongLoaded(result?.beatmapLevel);
                    }
                }
                else if (level is BeatmapLevelSO)
                {
                    SongLoaded(level as IBeatmapLevel);
                }
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
                modifiers.disappearingArrows = disappearingArrowsCheckbox.Checked && !ghostNotesCheckbox.Checked;
                modifiers.ghostNotes = ghostNotesCheckbox.Checked;

                var characteristic = currentSelection.beatmapCharacteristics.First(x => x.serializedName == characteristicDropdown.SelectedItem as string);
                SaberUtilities.PlaySong(currentSelection, characteristic, (BeatmapDifficulty)difficultyDropdown.SelectedItem, modifiers, playerSettings);
            }
        }

        private void returnToMenuButton_Click(object sender, EventArgs e)
        {
            Resources.FindObjectsOfTypeAll<StandardLevelScenesTransitionSetupDataSO>().FirstOrDefault()?.PopScenes(0.35f);
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
                        item.ImageKey = x.levelID; //LevelID used for launching game. AKA `key` in the Items.Add parameters. `Name` is the key we passed in on creation. Weird naming scheme.
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
                        item.ImageKey = x.levelID; //LevelID used for launching game. AKA `key` in the Items.Add parameters
                        return item;
                    })
                    .ToArray()
                );
            }
        }

        private void artBox_CheckedChanged(object sender, EventArgs e)
        {
            songListView.SmallImageList.Images.Clear();
            songListView.LargeImageList.Images.Clear();
        }

        private void CharacteristicDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            difficultyDropdown.Items.Clear();

            currentSelection.beatmapLevelData.difficultyBeatmapSets.First(
                x => x.beatmapCharacteristic == currentSelection.beatmapCharacteristics.First(y => y.serializedName == characteristicDropdown.SelectedItem as string)
            ).difficultyBeatmaps.ToList().ForEach(x => difficultyDropdown.Items.Add(x.difficulty));

            difficultyDropdown.SelectedIndex = difficultyDropdown.Items.Count - 1;
        }
    }
}
