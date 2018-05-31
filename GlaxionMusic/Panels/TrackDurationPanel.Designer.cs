namespace Glaxion.Music
{
    partial class PlaybackTrackbarControl
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
            gTrackBar.ColorPack colorPack1 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack2 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack3 = new gTrackBar.ColorPack();
            gTrackBar.ColorLinearGradient colorLinearGradient1 = new gTrackBar.ColorLinearGradient();
            this.endLabel2 = new System.Windows.Forms.Label();
            this.startLabel = new System.Windows.Forms.Label();
            this.gTrackBarMain = new gTrackBar.gTrackBar();
            this.stopButton = new System.Windows.Forms.Button();
            this.fileButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.prevButton = new System.Windows.Forms.Button();
            this.resumeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // endLabel2
            // 
            this.endLabel2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.endLabel2.Font = new System.Drawing.Font("Tw Cen MT", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.endLabel2.Location = new System.Drawing.Point(296, 46);
            this.endLabel2.Name = "endLabel2";
            this.endLabel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.endLabel2.Size = new System.Drawing.Size(58, 36);
            this.endLabel2.TabIndex = 2;
            this.endLabel2.Text = "End";
            this.endLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // startLabel
            // 
            this.startLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.startLabel.Font = new System.Drawing.Font("Tw Cen MT", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startLabel.Location = new System.Drawing.Point(49, 46);
            this.startLabel.Name = "startLabel";
            this.startLabel.Size = new System.Drawing.Size(61, 36);
            this.startLabel.TabIndex = 1;
            this.startLabel.Text = "Start";
            this.startLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gTrackBarMain
            // 
            this.gTrackBarMain.BackColor = System.Drawing.SystemColors.Control;
            colorPack1.Border = System.Drawing.Color.Indigo;
            colorPack1.Face = System.Drawing.Color.DarkOrchid;
            colorPack1.Highlight = System.Drawing.Color.Fuchsia;
            this.gTrackBarMain.ColorDown = colorPack1;
            colorPack2.Border = System.Drawing.Color.Indigo;
            colorPack2.Face = System.Drawing.Color.DarkOrchid;
            colorPack2.Highlight = System.Drawing.Color.MediumPurple;
            this.gTrackBarMain.ColorHover = colorPack2;
            colorPack3.Border = System.Drawing.Color.DarkOrchid;
            colorPack3.Face = System.Drawing.Color.DarkOrchid;
            colorPack3.Highlight = System.Drawing.Color.DarkOrchid;
            this.gTrackBarMain.ColorUp = colorPack3;
            this.gTrackBarMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.gTrackBarMain.FloatValue = false;
            this.gTrackBarMain.JumpToMouse = true;
            this.gTrackBarMain.Label = null;
            this.gTrackBarMain.Location = new System.Drawing.Point(0, 0);
            this.gTrackBarMain.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.gTrackBarMain.MaxValue = 100;
            this.gTrackBarMain.Name = "gTrackBarMain";
            this.gTrackBarMain.ShowFocus = false;
            this.gTrackBarMain.Size = new System.Drawing.Size(435, 43);
            this.gTrackBarMain.SliderCapEnd = System.Drawing.Drawing2D.LineCap.Triangle;
            this.gTrackBarMain.SliderCapStart = System.Drawing.Drawing2D.LineCap.Triangle;
            colorLinearGradient1.ColorA = System.Drawing.Color.Black;
            colorLinearGradient1.ColorB = System.Drawing.Color.DarkOrchid;
            this.gTrackBarMain.SliderColorLow = colorLinearGradient1;
            this.gTrackBarMain.SliderShape = gTrackBar.gTrackBar.eShape.Rectangle;
            this.gTrackBarMain.SliderSize = new System.Drawing.Size(30, 30);
            this.gTrackBarMain.SliderWidthHigh = 15F;
            this.gTrackBarMain.SliderWidthLow = 4F;
            this.gTrackBarMain.SnapToValue = false;
            this.gTrackBarMain.TabIndex = 3;
            this.gTrackBarMain.TickInterval = 1;
            this.gTrackBarMain.TickThickness = 1F;
            this.gTrackBarMain.Value = 0;
            this.gTrackBarMain.ValueAdjusted = 0F;
            this.gTrackBarMain.ValueBoxBorder = System.Drawing.Color.SlateGray;
            this.gTrackBarMain.ValueBoxShape = gTrackBar.gTrackBar.eShape.Ellipse;
            this.gTrackBarMain.ValueBoxSize = new System.Drawing.Size(30, 30);
            this.gTrackBarMain.ValueDivisor = gTrackBar.gTrackBar.eValueDivisor.e1;
            this.gTrackBarMain.ValueStrFormat = null;
            this.gTrackBarMain.ValueChanged += new gTrackBar.gTrackBar.ValueChangedEventHandler(this.gTrackBarMain_ValueChanged);
            this.gTrackBarMain.Scroll += new gTrackBar.gTrackBar.ScrollEventHandler(this.gTrackBarMain_Scroll);
            this.gTrackBarMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gTrackBarMain_MouseDown);
            this.gTrackBarMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gTrackBarMain_MouseUp);
            // 
            // stopButton
            // 
            this.stopButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.stopButton.BackgroundImage = global::Glaxion.Music.Properties.Resources.stop;
            this.stopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.stopButton.Location = new System.Drawing.Point(224, 50);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(30, 30);
            this.stopButton.TabIndex = 13;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // fileButton
            // 
            this.fileButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.fileButton.BackgroundImage = global::Glaxion.Music.Properties.Resources.undo_file_318_30946;
            this.fileButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.fileButton.Location = new System.Drawing.Point(260, 50);
            this.fileButton.Name = "fileButton";
            this.fileButton.Size = new System.Drawing.Size(30, 30);
            this.fileButton.TabIndex = 12;
            this.fileButton.UseVisualStyleBackColor = true;
            this.fileButton.Click += new System.EventHandler(this.fileButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.nextButton.BackgroundImage = global::Glaxion.Music.Properties.Resources.last_512;
            this.nextButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.nextButton.Location = new System.Drawing.Point(152, 50);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(30, 30);
            this.nextButton.TabIndex = 11;
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // prevButton
            // 
            this.prevButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.prevButton.BackgroundImage = global::Glaxion.Music.Properties.Resources.first_256;
            this.prevButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.prevButton.Location = new System.Drawing.Point(188, 50);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(30, 30);
            this.prevButton.TabIndex = 10;
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
            // 
            // resumeButton
            // 
            this.resumeButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.resumeButton.BackgroundImage = global::Glaxion.Music.Properties.Resources.media_play_pause_resume;
            this.resumeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.resumeButton.Location = new System.Drawing.Point(116, 50);
            this.resumeButton.Name = "resumeButton";
            this.resumeButton.Size = new System.Drawing.Size(30, 30);
            this.resumeButton.TabIndex = 9;
            this.resumeButton.UseVisualStyleBackColor = true;
            this.resumeButton.Click += new System.EventHandler(this.resumeButton_Click);
            // 
            // PlaybackTrackbarControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.fileButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.resumeButton);
            this.Controls.Add(this.gTrackBarMain);
            this.Controls.Add(this.startLabel);
            this.Controls.Add(this.endLabel2);
            this.Name = "PlaybackTrackbarControl";
            this.Size = new System.Drawing.Size(435, 82);
            this.Load += new System.EventHandler(this.PlaybackTrackbarControl_Load);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Label endLabel2;
        public System.Windows.Forms.Label startLabel;
        private gTrackBar.gTrackBar gTrackBarMain;
        public System.Windows.Forms.Button stopButton;
        public System.Windows.Forms.Button fileButton;
        public System.Windows.Forms.Button nextButton;
        public System.Windows.Forms.Button prevButton;
        public System.Windows.Forms.Button resumeButton;
    }
}
