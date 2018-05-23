namespace Glaxion.Music
{
    partial class PlaylistPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.trackMenuStrip = new System.Windows.Forms.MenuStrip();
            this.textLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.closeButton = new System.Windows.Forms.Button();
            this.updateAndCloseButton = new System.Windows.Forms.Button();
            this.saveAndCloseButton = new System.Windows.Forms.Button();
            this.close_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.close_update_toopTip = new System.Windows.Forms.ToolTip(this.components);
            this.close_save_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.update_music_tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.tracklistView = new Glaxion.Music.TracklistView();
            this.updateMusicPlayerButton = new System.Windows.Forms.Button();
            this.trackMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // trackMenuStrip
            // 
            this.trackMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textLabel});
            this.trackMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.trackMenuStrip.Name = "trackMenuStrip";
            this.trackMenuStrip.ShowItemToolTips = true;
            this.trackMenuStrip.Size = new System.Drawing.Size(310, 24);
            this.trackMenuStrip.TabIndex = 0;
            this.trackMenuStrip.Text = "menuStrip1";
            // 
            // textLabel
            // 
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(91, 20);
            this.textLabel.Text = "Playlist Name";
            this.textLabel.ToolTipText = "Update the music player with the most recent playlist changes";
            this.textLabel.Click += new System.EventHandler(this.textLabel_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.DarkRed;
            this.closeButton.ForeColor = System.Drawing.Color.White;
            this.closeButton.Location = new System.Drawing.Point(282, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(25, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "C";
            this.close_toolTip.SetToolTip(this.closeButton, "Close this panel and discard whatever changes you\'ve made");
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // updateAndCloseButton
            // 
            this.updateAndCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateAndCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.updateAndCloseButton.ForeColor = System.Drawing.Color.White;
            this.updateAndCloseButton.Location = new System.Drawing.Point(251, 0);
            this.updateAndCloseButton.Name = "updateAndCloseButton";
            this.updateAndCloseButton.Size = new System.Drawing.Size(25, 23);
            this.updateAndCloseButton.TabIndex = 1;
            this.updateAndCloseButton.Text = "R";
            this.close_update_toopTip.SetToolTip(this.updateAndCloseButton, "Close this panel and remember the changes you\'ve made to this playlist without sa" +
        "ving the file. The last closed panel will be stored in the playlist.");
            this.updateAndCloseButton.UseVisualStyleBackColor = false;
            this.updateAndCloseButton.Click += new System.EventHandler(this.UpdateAndCloseButton_Click);
            // 
            // saveAndCloseButton
            // 
            this.saveAndCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveAndCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.saveAndCloseButton.ForeColor = System.Drawing.Color.White;
            this.saveAndCloseButton.Location = new System.Drawing.Point(220, 0);
            this.saveAndCloseButton.Name = "saveAndCloseButton";
            this.saveAndCloseButton.Size = new System.Drawing.Size(25, 23);
            this.saveAndCloseButton.TabIndex = 1;
            this.saveAndCloseButton.Text = "S";
            this.close_save_toolTip.SetToolTip(this.saveAndCloseButton, "Save the current list to file and close.  ");
            this.saveAndCloseButton.UseVisualStyleBackColor = false;
            this.saveAndCloseButton.Click += new System.EventHandler(this.saveAndCloseButton_Click);
            // 
            // close_toolTip
            // 
            this.close_toolTip.ToolTipTitle = "Close";
            // 
            // close_update_toopTip
            // 
            this.close_update_toopTip.ToolTipTitle = "Remember and Close";
            // 
            // close_save_toolTip
            // 
            this.close_save_toolTip.ToolTipTitle = "Save and Close";
            // 
            // update_music_tooltip
            // 
            this.update_music_tooltip.ToolTipTitle = "Update Music Player";
            // 
            // playlistView
            // 
            this.tracklistView.AllowDrop = true;
            this.tracklistView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tracklistView.BackColor = System.Drawing.Color.LemonChiffon;
            this.tracklistView.ForeColor = System.Drawing.Color.Black;
            this.tracklistView.FullRowSelect = true;
            this.tracklistView.Location = new System.Drawing.Point(0, 29);
            this.tracklistView.Name = "playlistView";
            this.tracklistView.OwnerDraw = true;
            this.tracklistView.Size = new System.Drawing.Size(310, 308);
            this.tracklistView.TabIndex = 3;
            this.tracklistView.UseCompatibleStateImageBehavior = false;
            this.tracklistView.View = System.Windows.Forms.View.Details;
            this.tracklistView.MouseEnter += new System.EventHandler(this.trackManager_MouseEnter);
            // 
            // updateMusicPlayerButton
            // 
            this.updateMusicPlayerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateMusicPlayerButton.BackColor = System.Drawing.Color.Blue;
            this.updateMusicPlayerButton.Enabled = false;
            this.updateMusicPlayerButton.ForeColor = System.Drawing.Color.White;
            this.updateMusicPlayerButton.Location = new System.Drawing.Point(189, 0);
            this.updateMusicPlayerButton.Name = "updateMusicPlayerButton";
            this.updateMusicPlayerButton.Size = new System.Drawing.Size(25, 23);
            this.updateMusicPlayerButton.TabIndex = 1;
            this.updateMusicPlayerButton.Text = "U";
            this.update_music_tooltip.SetToolTip(this.updateMusicPlayerButton, "Update Music Player");
            this.updateMusicPlayerButton.UseVisualStyleBackColor = false;
            this.updateMusicPlayerButton.Visible = false;
            this.updateMusicPlayerButton.Click += new System.EventHandler(this.saveAndCloseButton_Click);
            // 
            // PlaylistPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tracklistView);
            this.Controls.Add(this.updateMusicPlayerButton);
            this.Controls.Add(this.saveAndCloseButton);
            this.Controls.Add(this.updateAndCloseButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.trackMenuStrip);
            this.Name = "PlaylistPanel";
            this.Size = new System.Drawing.Size(310, 337);
            this.Load += new System.EventHandler(this.TrackUserControl_Load);
            this.MouseEnter += new System.EventHandler(this.TrackUserControl_MouseEnter);
            this.trackMenuStrip.ResumeLayout(false);
            this.trackMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip trackMenuStrip;
        private System.Windows.Forms.Button closeButton;
        public Glaxion.Music.TracklistView tracklistView;
        private System.Windows.Forms.Button updateAndCloseButton;
        private System.Windows.Forms.Button saveAndCloseButton;
        public System.Windows.Forms.ToolStripMenuItem textLabel;
        private System.Windows.Forms.ToolTip close_toolTip;
        private System.Windows.Forms.ToolTip close_update_toopTip;
        private System.Windows.Forms.ToolTip close_save_toolTip;
        private System.Windows.Forms.ToolTip update_music_tooltip;
        private System.Windows.Forms.Button updateMusicPlayerButton;
    }
}
