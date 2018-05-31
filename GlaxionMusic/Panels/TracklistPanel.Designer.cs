namespace Glaxion.Music
{
    partial class TracklistPanel
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
            this.closeButton = new System.Windows.Forms.Button();
            this.updateAndCloseButton = new System.Windows.Forms.Button();
            this.saveAndCloseButton = new System.Windows.Forms.Button();
            this.close_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.close_update_toopTip = new System.Windows.Forms.ToolTip(this.components);
            this.close_save_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.update_music_tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.updateMusicPlayerButton = new System.Windows.Forms.Button();
            this.playlistNameLabel = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tracklistView = new Glaxion.Music.TracklistView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.DarkRed;
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.ForeColor = System.Drawing.Color.White;
            this.closeButton.Location = new System.Drawing.Point(279, 3);
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
            this.updateAndCloseButton.FlatAppearance.BorderSize = 0;
            this.updateAndCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateAndCloseButton.ForeColor = System.Drawing.Color.White;
            this.updateAndCloseButton.Location = new System.Drawing.Point(239, 3);
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
            this.saveAndCloseButton.FlatAppearance.BorderSize = 0;
            this.saveAndCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveAndCloseButton.ForeColor = System.Drawing.Color.White;
            this.saveAndCloseButton.Location = new System.Drawing.Point(196, 3);
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
            // updateMusicPlayerButton
            // 
            this.updateMusicPlayerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateMusicPlayerButton.BackColor = System.Drawing.Color.Blue;
            this.updateMusicPlayerButton.Enabled = false;
            this.updateMusicPlayerButton.FlatAppearance.BorderSize = 0;
            this.updateMusicPlayerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateMusicPlayerButton.ForeColor = System.Drawing.Color.White;
            this.updateMusicPlayerButton.Location = new System.Drawing.Point(155, 3);
            this.updateMusicPlayerButton.Name = "updateMusicPlayerButton";
            this.updateMusicPlayerButton.Size = new System.Drawing.Size(25, 23);
            this.updateMusicPlayerButton.TabIndex = 1;
            this.updateMusicPlayerButton.Text = "U";
            this.update_music_tooltip.SetToolTip(this.updateMusicPlayerButton, "Update Music Player");
            this.updateMusicPlayerButton.UseVisualStyleBackColor = false;
            this.updateMusicPlayerButton.Visible = false;
            this.updateMusicPlayerButton.Click += new System.EventHandler(this.updateMusicPlayerButton_Click);
            // 
            // playlistNameLabel
            // 
            this.playlistNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.playlistNameLabel.BackColor = System.Drawing.Color.Gainsboro;
            this.playlistNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playlistNameLabel.Location = new System.Drawing.Point(3, 26);
            this.playlistNameLabel.Name = "playlistNameLabel";
            this.playlistNameLabel.Size = new System.Drawing.Size(304, 28);
            this.playlistNameLabel.TabIndex = 4;
            this.playlistNameLabel.Text = "Playlist Name";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(0, 57);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(310, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.closeButton);
            this.splitContainer1.Panel1.Controls.Add(this.updateAndCloseButton);
            this.splitContainer1.Panel1.Controls.Add(this.saveAndCloseButton);
            this.splitContainer1.Panel1.Controls.Add(this.updateMusicPlayerButton);
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.playlistNameLabel);
            this.splitContainer1.Panel1MinSize = 55;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tracklistView);
            this.splitContainer1.Panel2MinSize = 250;
            this.splitContainer1.Size = new System.Drawing.Size(310, 337);
            this.splitContainer1.SplitterDistance = 75;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 6;
            this.splitContainer1.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(this.splitContainer1_SplitterMoving);
            // 
            // tracklistView
            // 
            this.tracklistView.AllowColumnReorder = true;
            this.tracklistView.AllowDrop = true;
            this.tracklistView.BackColor = System.Drawing.Color.LemonChiffon;
            this.tracklistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tracklistView.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tracklistView.ForeColor = System.Drawing.Color.Black;
            this.tracklistView.FullRowSelect = true;
            this.tracklistView.Location = new System.Drawing.Point(0, 0);
            this.tracklistView.Name = "tracklistView";
            this.tracklistView.OwnerDraw = true;
            this.tracklistView.Size = new System.Drawing.Size(310, 254);
            this.tracklistView.TabIndex = 3;
            this.tracklistView.UseCompatibleStateImageBehavior = false;
            this.tracklistView.View = System.Windows.Forms.View.Details;
            this.tracklistView.MouseEnter += new System.EventHandler(this.trackManager_MouseEnter);
            // 
            // TracklistPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.splitContainer1);
            this.Name = "TracklistPanel";
            this.Size = new System.Drawing.Size(310, 337);
            this.Load += new System.EventHandler(this.TrackUserControl_Load);
            this.MouseEnter += new System.EventHandler(this.TrackUserControl_MouseEnter);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button closeButton;
        public Glaxion.Music.TracklistView tracklistView;
        private System.Windows.Forms.Button updateAndCloseButton;
        private System.Windows.Forms.Button saveAndCloseButton;
        private System.Windows.Forms.ToolTip close_toolTip;
        private System.Windows.Forms.ToolTip close_update_toopTip;
        private System.Windows.Forms.ToolTip close_save_toolTip;
        private System.Windows.Forms.ToolTip update_music_tooltip;
        private System.Windows.Forms.Button updateMusicPlayerButton;
        private System.Windows.Forms.Label playlistNameLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
