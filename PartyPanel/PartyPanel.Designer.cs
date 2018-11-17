namespace PartyPanel
{
    partial class PartyPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.songListView = new System.Windows.Forms.ListView();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.returnToMenuButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.difficultyDropdown = new System.Windows.Forms.ComboBox();
            this.staticLightsCheckbox = new System.Windows.Forms.CheckBox();
            this.mirrorCheckbox = new System.Windows.Forms.CheckBox();
            this.noFailCheckbox = new System.Windows.Forms.CheckBox();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchLabel = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // songListView
            // 
            this.songListView.HideSelection = false;
            this.songListView.Location = new System.Drawing.Point(12, 38);
            this.songListView.MultiSelect = false;
            this.songListView.Name = "songListView";
            this.songListView.Size = new System.Drawing.Size(550, 400);
            this.songListView.TabIndex = 0;
            this.songListView.UseCompatibleStateImageBehavior = false;
            this.songListView.SelectedIndexChanged += new System.EventHandler(this.songListView_SelectedIndexChanged);
            this.songListView.DoubleClick += new System.EventHandler(this.playButton_Click);
            // 
            // groupBox
            // 
            this.groupBox.AutoSize = true;
            this.groupBox.Controls.Add(this.returnToMenuButton);
            this.groupBox.Controls.Add(this.playButton);
            this.groupBox.Controls.Add(this.difficultyDropdown);
            this.groupBox.Controls.Add(this.staticLightsCheckbox);
            this.groupBox.Controls.Add(this.mirrorCheckbox);
            this.groupBox.Controls.Add(this.noFailCheckbox);
            this.groupBox.Location = new System.Drawing.Point(568, 12);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(186, 426);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Options";
            // 
            // returnToMenuButton
            // 
            this.returnToMenuButton.Location = new System.Drawing.Point(6, 146);
            this.returnToMenuButton.Name = "returnToMenuButton";
            this.returnToMenuButton.Size = new System.Drawing.Size(174, 23);
            this.returnToMenuButton.TabIndex = 5;
            this.returnToMenuButton.Text = "Return to Menu";
            this.returnToMenuButton.UseVisualStyleBackColor = true;
            this.returnToMenuButton.Click += new System.EventHandler(this.returnToMenuButton_Click);
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(6, 117);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(174, 23);
            this.playButton.TabIndex = 4;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // difficultyDropdown
            // 
            this.difficultyDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.difficultyDropdown.FormattingEnabled = true;
            this.difficultyDropdown.Location = new System.Drawing.Point(6, 90);
            this.difficultyDropdown.Name = "difficultyDropdown";
            this.difficultyDropdown.Size = new System.Drawing.Size(174, 21);
            this.difficultyDropdown.TabIndex = 3;
            // 
            // staticLightsCheckbox
            // 
            this.staticLightsCheckbox.AutoSize = true;
            this.staticLightsCheckbox.Location = new System.Drawing.Point(6, 67);
            this.staticLightsCheckbox.Name = "staticLightsCheckbox";
            this.staticLightsCheckbox.Size = new System.Drawing.Size(84, 17);
            this.staticLightsCheckbox.TabIndex = 2;
            this.staticLightsCheckbox.Text = "Static Lights";
            this.staticLightsCheckbox.UseVisualStyleBackColor = true;
            // 
            // mirrorCheckbox
            // 
            this.mirrorCheckbox.AutoSize = true;
            this.mirrorCheckbox.Location = new System.Drawing.Point(6, 43);
            this.mirrorCheckbox.Name = "mirrorCheckbox";
            this.mirrorCheckbox.Size = new System.Drawing.Size(52, 17);
            this.mirrorCheckbox.TabIndex = 1;
            this.mirrorCheckbox.Text = "Mirror";
            this.mirrorCheckbox.UseVisualStyleBackColor = true;
            // 
            // noFailCheckbox
            // 
            this.noFailCheckbox.AutoSize = true;
            this.noFailCheckbox.Location = new System.Drawing.Point(6, 19);
            this.noFailCheckbox.Name = "noFailCheckbox";
            this.noFailCheckbox.Size = new System.Drawing.Size(59, 17);
            this.noFailCheckbox.TabIndex = 0;
            this.noFailCheckbox.Text = "No Fail";
            this.noFailCheckbox.UseVisualStyleBackColor = true;
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(62, 12);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(500, 20);
            this.searchBox.TabIndex = 2;
            this.searchBox.Visible = false;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(12, 15);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(133, 13);
            this.searchLabel.TabIndex = 3;
            this.searchLabel.Text = "Waiting for songs to load...";
            // 
            // PartyPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 450);
            this.Controls.Add(this.searchLabel);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.songListView);
            this.Name = "PartyPanel";
            this.Text = "PartyPanel";
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView songListView;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.ComboBox difficultyDropdown;
        private System.Windows.Forms.CheckBox staticLightsCheckbox;
        private System.Windows.Forms.CheckBox mirrorCheckbox;
        private System.Windows.Forms.CheckBox noFailCheckbox;
        private System.Windows.Forms.Button returnToMenuButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label searchLabel;
    }
}