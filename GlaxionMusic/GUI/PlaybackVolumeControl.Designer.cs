namespace Glaxion.Music
{
    partial class PlaybackVolumeControl
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
            gTrackBar.ColorLinearGradient colorLinearGradient1 = new gTrackBar.ColorLinearGradient();
            this.gTrackBarMain = new gTrackBar.gTrackBar();
            this.SuspendLayout();
            // 
            // gTrackBarMain
            // 
            this.gTrackBarMain.BackColor = System.Drawing.SystemColors.Control;
            this.gTrackBarMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.gTrackBarMain.BrushDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            colorPack1.Border = System.Drawing.Color.DarkBlue;
            colorPack1.Face = System.Drawing.Color.Blue;
            colorPack1.Highlight = System.Drawing.Color.AliceBlue;
            this.gTrackBarMain.ColorDown = colorPack1;
            colorPack2.Border = System.Drawing.Color.CornflowerBlue;
            colorPack2.Face = System.Drawing.Color.DarkSlateBlue;
            colorPack2.Highlight = System.Drawing.Color.AliceBlue;
            this.gTrackBarMain.ColorUp = colorPack2;
            this.gTrackBarMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gTrackBarMain.FloatValueFont = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.gTrackBarMain.JumpToMouse = true;
            this.gTrackBarMain.Label = "Volume";
            this.gTrackBarMain.LabelAlighnment = System.Drawing.StringAlignment.Far;
            this.gTrackBarMain.LabelColor = System.Drawing.Color.DarkBlue;
            this.gTrackBarMain.Location = new System.Drawing.Point(0, 0);
            this.gTrackBarMain.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.gTrackBarMain.MaxValue = 100;
            this.gTrackBarMain.Name = "gTrackBarMain";
            this.gTrackBarMain.ShowFocus = false;
            this.gTrackBarMain.Size = new System.Drawing.Size(557, 40);
            colorLinearGradient1.ColorA = System.Drawing.Color.Blue;
            colorLinearGradient1.ColorB = System.Drawing.Color.MediumSpringGreen;
            this.gTrackBarMain.SliderColorLow = colorLinearGradient1;
            this.gTrackBarMain.SliderSize = new System.Drawing.Size(20, 20);
            this.gTrackBarMain.SliderWidthHigh = 2F;
            this.gTrackBarMain.SliderWidthLow = 12F;
            this.gTrackBarMain.SnapToValue = false;
            this.gTrackBarMain.TabIndex = 4;
            this.gTrackBarMain.TickInterval = 1;
            this.gTrackBarMain.TickThickness = 1F;
            this.gTrackBarMain.TickWidth = 1;
            this.gTrackBarMain.Value = 25;
            this.gTrackBarMain.ValueAdjusted = 25F;
            this.gTrackBarMain.ValueBox = gTrackBar.gTrackBar.eValueBox.Right;
            this.gTrackBarMain.ValueBoxBorder = System.Drawing.Color.AliceBlue;
            this.gTrackBarMain.ValueBoxShape = gTrackBar.gTrackBar.eShape.Ellipse;
            this.gTrackBarMain.ValueBoxSize = new System.Drawing.Size(30, 30);
            this.gTrackBarMain.ValueDivisor = gTrackBar.gTrackBar.eValueDivisor.e1;
            this.gTrackBarMain.ValueStrFormat = null;
            this.gTrackBarMain.ValueChanged += new gTrackBar.gTrackBar.ValueChangedEventHandler(this.gTrackBarMain_ValueChanged);
            this.gTrackBarMain.Scroll += new gTrackBar.gTrackBar.ScrollEventHandler(this.gTrackBarMain_Scroll);
            this.gTrackBarMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gTrackBarMain_MouseClick);
            // 
            // PlaybackVolumeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Controls.Add(this.gTrackBarMain);
            this.Name = "PlaybackVolumeControl";
            this.Size = new System.Drawing.Size(557, 40);
            this.Load += new System.EventHandler(this.PlaybackVolumeControl_Load);
            this.ResumeLayout(false);

        }

        #endregion
        public gTrackBar.gTrackBar gTrackBarMain;
    }
}
