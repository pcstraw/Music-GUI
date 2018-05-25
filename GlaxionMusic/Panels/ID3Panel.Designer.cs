namespace Glaxion.Music
{
    partial class ID3Control
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
            this.updateButton = new System.Windows.Forms.Button();
            this.entryBox5 = new Glaxion.Music.EntryBox();
            this.entryBox4 = new Glaxion.Music.EntryBox();
            this.entryBox3 = new Glaxion.Music.EntryBox();
            this.entryBox2 = new Glaxion.Music.EntryBox();
            this.entryBox1 = new Glaxion.Music.EntryBox();
            this.entryBox5.SuspendLayout();
            this.entryBox4.SuspendLayout();
            this.entryBox3.SuspendLayout();
            this.entryBox2.SuspendLayout();
            this.entryBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // updateButton
            // 
            this.updateButton.BackColor = System.Drawing.Color.DarkGreen;
            this.updateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateButton.ForeColor = System.Drawing.Color.White;
            this.updateButton.Location = new System.Drawing.Point(61, 3);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(104, 36);
            this.updateButton.TabIndex = 5;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = false;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // entryBox5
            // 
            this.entryBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.entryBox5.Location = new System.Drawing.Point(0, 320);
            this.entryBox5.Name = "entryBox5";
            this.entryBox5.Size = new System.Drawing.Size(666, 47);
            this.entryBox5.TabIndex = 6;
            // 
            // entryBox4
            // 
            this.entryBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.entryBox4.Location = new System.Drawing.Point(0, 247);
            this.entryBox4.Name = "entryBox4";
            this.entryBox4.Size = new System.Drawing.Size(666, 47);
            this.entryBox4.TabIndex = 6;
            // 
            // entryBox3
            // 
            this.entryBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.entryBox3.Location = new System.Drawing.Point(0, 181);
            this.entryBox3.Name = "entryBox3";
            this.entryBox3.Size = new System.Drawing.Size(666, 47);
            this.entryBox3.TabIndex = 6;
            // 
            // entryBox2
            // 
            this.entryBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.entryBox2.Location = new System.Drawing.Point(0, 108);
            this.entryBox2.Name = "entryBox2";
            this.entryBox2.Size = new System.Drawing.Size(666, 47);
            this.entryBox2.TabIndex = 6;
            // 
            // entryBox1
            // 
            this.entryBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.entryBox1.Location = new System.Drawing.Point(0, 45);
            this.entryBox1.Name = "entryBox1";
            this.entryBox1.Size = new System.Drawing.Size(666, 47);
            this.entryBox1.TabIndex = 6;
            this.entryBox1.FontChanged += new System.EventHandler(this.entryBox1_FontChanged);
            // 
            // ID3Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.entryBox1);
            this.Controls.Add(this.entryBox2);
            this.Controls.Add(this.entryBox3);
            this.Controls.Add(this.entryBox4);
            this.Controls.Add(this.entryBox5);
            this.Controls.Add(this.updateButton);
            this.Name = "ID3Control";
            this.Size = new System.Drawing.Size(669, 501);
            this.Load += new System.EventHandler(this.ID3Control_Load);
            this.entryBox5.ResumeLayout(false);
            this.entryBox4.ResumeLayout(false);
            this.entryBox3.ResumeLayout(false);
            this.entryBox2.ResumeLayout(false);
            this.entryBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button updateButton;
        private EntryBox entryBox1;
        private EntryBox entryBox2;
        private EntryBox entryBox3;
        private EntryBox entryBox4;
        private EntryBox entryBox5;
    }
}
