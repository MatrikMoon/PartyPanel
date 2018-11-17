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
        public List<CustomLevel> masterLevelList;

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
                    var level = SongLoader.CustomLevels.Where(x => x.levelID == levelID).FirstOrDefault();

                    if (level != null)
                    {
                        difficultyDropdown.Items.Clear();
                        level.difficultyBeatmaps.ToList().ForEach(x => difficultyDropdown.Items.Add(x.difficulty));
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
                var gameOptions = new GameplayOptions();
                gameOptions.mirror = mirrorCheckbox.Checked;
                gameOptions.noEnergy = noFailCheckbox.Checked;
                gameOptions.staticLights = staticLightsCheckbox.Checked;
                SaberUtilities.PlaySong(songListView.SelectedItems[0].Name, (LevelDifficulty)difficultyDropdown.SelectedItem, gameOptions); //`Name` is the key we passed in on creation. Weird naming scheme.
            }
        }

        private void returnToMenuButton_Click(object sender, EventArgs e)
        {
            Resources.FindObjectsOfTypeAll<MenuSceneSetupData>().First().TransitionToScene(0.35f);
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
