namespace Glaxion.Music
{
    partial class SongControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SongControl));
            this.squashBoxControl1 = new Glaxion.Music.SquashBoxControl();
            this.titleLabel = new System.Windows.Forms.Label();
            this.iD3Control1 = new Glaxion.Music.ID3Control();
            this.picturePanel1 = new Glaxion.Music.PicturePanel();
            this.squashBoxControl1.TopPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.squashBoxControl1.MainSplitContainer)).BeginInit();
            this.squashBoxControl1.MainSplitContainer.Panel1.SuspendLayout();
            this.squashBoxControl1.MainSplitContainer.Panel2.SuspendLayout();
            this.squashBoxControl1.MainSplitContainer.SuspendLayout();
            this.squashBoxControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // squashBoxControl1
            // 
            // 
            // 
            // 
            this.squashBoxControl1.BackPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.squashBoxControl1.BackPanel.Location = new System.Drawing.Point(0, 0);
            this.squashBoxControl1.BackPanel.Name = "backPanel";
            this.squashBoxControl1.BackPanel.Size = new System.Drawing.Size(583, 343);
            this.squashBoxControl1.BackPanel.TabIndex = 0;
            this.squashBoxControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // squashBoxControl1.Panel
            // 
            this.squashBoxControl1.TopPanel.BackColor = System.Drawing.Color.Silver;
            this.squashBoxControl1.TopPanel.Controls.Add(this.titleLabel);
            this.squashBoxControl1.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.squashBoxControl1.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.squashBoxControl1.TopPanel.Name = "Panel";
            this.squashBoxControl1.TopPanel.Size = new System.Drawing.Size(583, 38);
            this.squashBoxControl1.TopPanel.TabIndex = 0;
            this.squashBoxControl1.Location = new System.Drawing.Point(0, 0);
            // 
            // squashBoxControl1.SplitContainer
            // 
            this.squashBoxControl1.MainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.squashBoxControl1.MainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.squashBoxControl1.MainSplitContainer.Location = new System.Drawing.Point(0, 38);
            this.squashBoxControl1.MainSplitContainer.Name = "SplitContainer";
            this.squashBoxControl1.MainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // squashBoxControl1.SplitContainer.Panel1
            // 
            this.squashBoxControl1.MainSplitContainer.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.squashBoxControl1.MainSplitContainer.Panel1.Controls.Add(this.iD3Control1);
            this.squashBoxControl1.MainSplitContainer.Panel1MinSize = 0;
            // 
            // squashBoxControl1.SplitContainer.Panel2
            // 
            this.squashBoxControl1.MainSplitContainer.Panel2.BackColor = System.Drawing.Color.Gray;
            this.squashBoxControl1.MainSplitContainer.Panel2.Controls.Add(this.picturePanel1);
            this.squashBoxControl1.MainSplitContainer.Panel2MinSize = 0;
            this.squashBoxControl1.MainSplitContainer.Size = new System.Drawing.Size(583, 690);
            this.squashBoxControl1.MainSplitContainer.SplitterDistance = 343;
            this.squashBoxControl1.MainSplitContainer.TabIndex = 2;
            this.squashBoxControl1.Name = "squashBoxControl1";
            this.squashBoxControl1.Size = new System.Drawing.Size(583, 728);
            this.squashBoxControl1.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titleLabel.BackColor = System.Drawing.Color.Black;
            this.titleLabel.Font = new System.Drawing.Font("Tw Cen MT", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.titleLabel.Location = new System.Drawing.Point(0, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(583, 38);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Title";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.titleLabel.Click += new System.EventHandler(this.titleLabel_Click);
            // 
            // iD3Control1
            // 
            this.iD3Control1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iD3Control1.Location = new System.Drawing.Point(0, 0);
            this.iD3Control1.Name = "iD3Control1";
            this.iD3Control1.Size = new System.Drawing.Size(583, 343);
            this.iD3Control1.TabIndex = 1;
            // 
            // picturePanel1
            // 
            this.picturePanel1.AllowDrop = true;
            this.picturePanel1.BackColor = System.Drawing.Color.Black;
            this.picturePanel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picturePanel1.BackgroundImage")));
            this.picturePanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picturePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picturePanel1.Location = new System.Drawing.Point(0, 0);
            this.picturePanel1.Name = "picturePanel1";
            this.picturePanel1.Size = new System.Drawing.Size(583, 343);
            this.picturePanel1.TabIndex = 1;
            this.picturePanel1.PictureChangedEvent += new Glaxion.Music.PicturePanel.PictureChangedEventHandler(this.picturePanel1_PictureChangedEvent);
            this.picturePanel1.Click += new System.EventHandler(this.picturePanel1_Click);
            // 
            // SongControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.squashBoxControl1);
            this.Name = "SongControl";
            this.Size = new System.Drawing.Size(583, 728);
            this.Load += new System.EventHandler(this.SongControl_Load);
            this.squashBoxControl1.TopPanel.ResumeLayout(false);
            this.squashBoxControl1.MainSplitContainer.Panel1.ResumeLayout(false);
            this.squashBoxControl1.MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.squashBoxControl1.MainSplitContainer)).EndInit();
            this.squashBoxControl1.MainSplitContainer.ResumeLayout(false);
            this.squashBoxControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private PicturePanel picturePanel1;
        public System.Windows.Forms.Label titleLabel;
        public SquashBoxControl squashBoxControl1;
        public ID3Control iD3Control1;
    }
}
