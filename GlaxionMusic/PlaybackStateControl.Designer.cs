namespace Glaxion.Music
{
    partial class PlaybackStateControl
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
            this.nameLabel = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.fileButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.prevButton = new System.Windows.Forms.Button();
            this.resumeButton = new System.Windows.Forms.Button();
            this.loopCheckBox = new System.Windows.Forms.CheckBox();
            this.hideNameButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.Location = new System.Drawing.Point(3, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(375, 39);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Track Name";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.nameLabel.TextChanged += new System.EventHandler(this.nameLabel_TextChanged);
            this.nameLabel.MouseHover += new System.EventHandler(this.nameLabel_MouseHover);
            this.nameLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.nameLabel_MouseUp);
            // 
            // stopButton
            // 
            this.stopButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.stopButton.BackgroundImage = global::Glaxion.Music.Properties.Resources.stop;
            this.stopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.stopButton.Location = new System.Drawing.Point(213, 52);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(30, 30);
            this.stopButton.TabIndex = 6;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // fileButton
            // 
            this.fileButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.fileButton.BackgroundImage = global::Glaxion.Music.Properties.Resources.undo_file_318_30946;
            this.fileButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.fileButton.Location = new System.Drawing.Point(249, 52);
            this.fileButton.Name = "fileButton";
            this.fileButton.Size = new System.Drawing.Size(30, 30);
            this.fileButton.TabIndex = 5;
            this.fileButton.UseVisualStyleBackColor = true;
            this.fileButton.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.nextButton.BackgroundImage = global::Glaxion.Music.Properties.Resources.last_512;
            this.nextButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.nextButton.Location = new System.Drawing.Point(141, 52);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(30, 30);
            this.nextButton.TabIndex = 4;
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // prevButton
            // 
            this.prevButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.prevButton.BackgroundImage = global::Glaxion.Music.Properties.Resources.first_256;
            this.prevButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.prevButton.Location = new System.Drawing.Point(177, 52);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(30, 30);
            this.prevButton.TabIndex = 3;
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
            // 
            // resumeButton
            // 
            this.resumeButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.resumeButton.BackgroundImage = global::Glaxion.Music.Properties.Resources.media_play_pause_resume;
            this.resumeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.resumeButton.Location = new System.Drawing.Point(105, 52);
            this.resumeButton.Name = "resumeButton";
            this.resumeButton.Size = new System.Drawing.Size(30, 30);
            this.resumeButton.TabIndex = 1;
            this.resumeButton.UseVisualStyleBackColor = true;
            this.resumeButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // loopCheckBox
            // 
            this.loopCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.loopCheckBox.AutoSize = true;
            this.loopCheckBox.Location = new System.Drawing.Point(285, 52);
            this.loopCheckBox.Name = "loopCheckBox";
            this.loopCheckBox.Size = new System.Drawing.Size(50, 17);
            this.loopCheckBox.TabIndex = 8;
            this.loopCheckBox.Text = "Loop";
            this.loopCheckBox.UseVisualStyleBackColor = true;
            this.loopCheckBox.CheckedChanged += new System.EventHandler(this.loopCheckBox_CheckedChanged);
            // 
            // hideNameButton
            // 
            this.hideNameButton.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.hideNameButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.hideNameButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.hideNameButton.FlatAppearance.BorderSize = 0;
            this.hideNameButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hideNameButton.Location = new System.Drawing.Point(0, 39);
            this.hideNameButton.Margin = new System.Windows.Forms.Padding(0);
            this.hideNameButton.Name = "hideNameButton";
            this.hideNameButton.Size = new System.Drawing.Size(381, 10);
            this.hideNameButton.TabIndex = 9;
            this.hideNameButton.UseVisualStyleBackColor = false;
            this.hideNameButton.Click += new System.EventHandler(this.hideNameButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.nameLabel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(381, 39);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // PlaybackStateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.hideNameButton);
            this.Controls.Add(this.loopCheckBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.fileButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.resumeButton);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PlaybackStateControl";
            this.Size = new System.Drawing.Size(381, 87);
            this.Load += new System.EventHandler(this.PlaybackStateControl_Load);
            this.MouseHover += new System.EventHandler(this.PlaybackStateControl_MouseHover);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label nameLabel;
        public System.Windows.Forms.Button resumeButton;
        public System.Windows.Forms.Button prevButton;
        public System.Windows.Forms.Button nextButton;
        public System.Windows.Forms.Button fileButton;
        public System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.CheckBox loopCheckBox;
        private System.Windows.Forms.Button hideNameButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
