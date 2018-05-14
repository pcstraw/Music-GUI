﻿namespace Glaxion.Music
{
    partial class PicturePanel
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
            this.SuspendLayout();
            // 
            // PicturePanel
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::Glaxion.Music.Properties.Resources.music_gui_logo;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.DoubleBuffered = true;
            this.Name = "PicturePanel";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.PicturePanel_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.PicturePanel_DragEnter);
            this.DoubleClick += new System.EventHandler(this.PicturePanel_DoubleClick);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
