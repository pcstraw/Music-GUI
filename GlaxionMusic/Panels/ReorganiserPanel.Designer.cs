namespace Glaxion.Music
{
    partial class ReorganiserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReorganiserControl));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.add_contect_button = new System.Windows.Forms.ToolStripMenuItem();
            this.remove_context_item = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.copyButton = new System.Windows.Forms.Button();
            this.moveButton = new System.Windows.Forms.Button();
            this.trackInfoUserControl = new Glaxion.Music.SongControl();
            this.add_year_checkbox = new System.Windows.Forms.CheckBox();
            this.treeView = new Glaxion.Music.TreeViewMS();
            this.splitContainerBack = new System.Windows.Forms.SplitContainer();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBack)).BeginInit();
            this.splitContainerBack.Panel1.SuspendLayout();
            this.splitContainerBack.Panel2.SuspendLayout();
            this.splitContainerBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.add_contect_button,
            this.remove_context_item});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(118, 48);
            // 
            // add_contect_button
            // 
            this.add_contect_button.Name = "add_contect_button";
            this.add_contect_button.Size = new System.Drawing.Size(117, 22);
            this.add_contect_button.Text = "Add";
            this.add_contect_button.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // remove_context_item
            // 
            this.remove_context_item.Name = "remove_context_item";
            this.remove_context_item.Size = new System.Drawing.Size(117, 22);
            this.remove_context_item.Text = "Remove";
            this.remove_context_item.Click += new System.EventHandler(this.remove_context_item_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "icons8-folder-50.png");
            this.imageList1.Images.SetKeyName(1, "multimedia-audio-player-icon.png");
            // 
            // copyButton
            // 
            this.copyButton.BackColor = System.Drawing.Color.DimGray;
            this.copyButton.Font = new System.Drawing.Font("OCR A Extended", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyButton.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.copyButton.Location = new System.Drawing.Point(116, 15);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 36);
            this.copyButton.TabIndex = 3;
            this.copyButton.Text = "COPY";
            this.copyButton.UseVisualStyleBackColor = false;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // moveButton
            // 
            this.moveButton.BackColor = System.Drawing.Color.DimGray;
            this.moveButton.Font = new System.Drawing.Font("OCR A Extended", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.moveButton.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.moveButton.Location = new System.Drawing.Point(229, 15);
            this.moveButton.Name = "moveButton";
            this.moveButton.Size = new System.Drawing.Size(75, 36);
            this.moveButton.TabIndex = 4;
            this.moveButton.Text = "Move";
            this.moveButton.UseVisualStyleBackColor = false;
            this.moveButton.Click += new System.EventHandler(this.moveButton_Click);
            // 
            // trackInfoUserControl
            // 
            this.trackInfoUserControl.BackColor = System.Drawing.Color.LightGray;
            this.trackInfoUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackInfoUserControl.Location = new System.Drawing.Point(0, 0);
            this.trackInfoUserControl.Name = "trackInfoUserControl";
            this.trackInfoUserControl.Size = new System.Drawing.Size(325, 471);
            this.trackInfoUserControl.TabIndex = 2;
            this.trackInfoUserControl.Load += new System.EventHandler(this.trackInfoUserControl_Load);
            // 
            // add_year_checkbox
            // 
            this.add_year_checkbox.AutoSize = true;
            this.add_year_checkbox.Location = new System.Drawing.Point(16, 26);
            this.add_year_checkbox.Name = "add_year_checkbox";
            this.add_year_checkbox.Size = new System.Drawing.Size(70, 17);
            this.add_year_checkbox.TabIndex = 5;
            this.add_year_checkbox.Text = "Add Year";
            this.add_year_checkbox.UseVisualStyleBackColor = true;
            this.add_year_checkbox.CheckedChanged += new System.EventHandler(this.add_year_checkbox_CheckedChanged);
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.BackColor = System.Drawing.Color.WhiteSmoke;
            this.treeView.ContextMenuStrip = this.contextMenuStrip;
            this.treeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView.ForeColor = System.Drawing.Color.Black;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList1;
            this.treeView.Indent = 15;
            this.treeView.ItemHeight = 16;
            this.treeView.LabelEdit = true;
            this.treeView.Location = new System.Drawing.Point(3, 57);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("treeView.SelectedNodes")));
            this.treeView.Size = new System.Drawing.Size(344, 411);
            this.treeView.TabIndex = 1;
            this.treeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_BeforeLabelEdit);
            this.treeView.Click += new System.EventHandler(this.treeView_Click);
            // 
            // splitContainerBack
            // 
            this.splitContainerBack.BackColor = System.Drawing.Color.LightGray;
            this.splitContainerBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBack.Location = new System.Drawing.Point(0, 0);
            this.splitContainerBack.Name = "splitContainerBack";
            // 
            // splitContainerBack.Panel1
            // 
            this.splitContainerBack.Panel1.Controls.Add(this.treeView);
            this.splitContainerBack.Panel1.Controls.Add(this.copyButton);
            this.splitContainerBack.Panel1.Controls.Add(this.add_year_checkbox);
            this.splitContainerBack.Panel1.Controls.Add(this.moveButton);
            // 
            // splitContainerBack.Panel2
            // 
            this.splitContainerBack.Panel2.Controls.Add(this.trackInfoUserControl);
            this.splitContainerBack.Size = new System.Drawing.Size(678, 471);
            this.splitContainerBack.SplitterDistance = 349;
            this.splitContainerBack.TabIndex = 6;
            // 
            // ReorganiserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerBack);
            this.Name = "ReorganiserControl";
            this.Size = new System.Drawing.Size(678, 471);
            this.contextMenuStrip.ResumeLayout(false);
            this.splitContainerBack.Panel1.ResumeLayout(false);
            this.splitContainerBack.Panel1.PerformLayout();
            this.splitContainerBack.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBack)).EndInit();
            this.splitContainerBack.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public SongControl trackInfoUserControl;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Button moveButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem add_contect_button;
        private System.Windows.Forms.ToolStripMenuItem remove_context_item;
        private System.Windows.Forms.CheckBox add_year_checkbox;
        private TreeViewMS treeView;
        private System.Windows.Forms.SplitContainer splitContainerBack;
    }
}
