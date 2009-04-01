namespace StopCrop
{
    partial class OverlayWindow
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jHPGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInPaintBrushToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToClipboardToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAsFile = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAsJpg = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyAsPNG = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToFileToolStripMenuItem,
            this.openInPaintBrushToolStripMenuItem,
            this.copyToClipboardToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(169, 70);
            // 
            // saveToFileToolStripMenuItem
            // 
            this.saveToFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bMPToolStripMenuItem,
            this.jHPGToolStripMenuItem,
            this.pNGToolStripMenuItem});
            this.saveToFileToolStripMenuItem.Image = global::StopCrop.Properties.Resources.save;
            this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
            this.saveToFileToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveToFileToolStripMenuItem.Text = "Save to file";
            // 
            // bMPToolStripMenuItem
            // 
            this.bMPToolStripMenuItem.Name = "bMPToolStripMenuItem";
            this.bMPToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.bMPToolStripMenuItem.Text = "Bitmap (bmp)";
            this.bMPToolStripMenuItem.Click += new System.EventHandler(this.bMPToolStripMenuItem_Click);
            // 
            // jHPGToolStripMenuItem
            // 
            this.jHPGToolStripMenuItem.Name = "jHPGToolStripMenuItem";
            this.jHPGToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.jHPGToolStripMenuItem.Text = "Jpeg (jpg)";
            this.jHPGToolStripMenuItem.Click += new System.EventHandler(this.jHPGToolStripMenuItem_Click);
            // 
            // pNGToolStripMenuItem
            // 
            this.pNGToolStripMenuItem.Name = "pNGToolStripMenuItem";
            this.pNGToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.pNGToolStripMenuItem.Text = "PNG";
            this.pNGToolStripMenuItem.Click += new System.EventHandler(this.pNGToolStripMenuItem_Click);
            // 
            // openInPaintBrushToolStripMenuItem
            // 
            this.openInPaintBrushToolStripMenuItem.Image = global::StopCrop.Properties.Resources.MSPaint3;
            this.openInPaintBrushToolStripMenuItem.Name = "openInPaintBrushToolStripMenuItem";
            this.openInPaintBrushToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openInPaintBrushToolStripMenuItem.Text = "Open in Paint Brush";
            this.openInPaintBrushToolStripMenuItem.Click += new System.EventHandler(this.openInPaintBrushToolStripMenuItem_Click);
            // 
            // copyToClipboardToolStripMenuItem1
            // 
            this.copyToClipboardToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToClipboardToolStripMenuItem,
            this.asFileToolStripMenuItem});
            this.copyToClipboardToolStripMenuItem1.Image = global::StopCrop.Properties.Resources.copy1;
            this.copyToClipboardToolStripMenuItem1.Name = "copyToClipboardToolStripMenuItem1";
            this.copyToClipboardToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
            this.copyToClipboardToolStripMenuItem1.Text = "Copy to Clipboard";
            this.copyToClipboardToolStripMenuItem1.Click += new System.EventHandler(this.copyToClipboardToolStripMenuItem1_Click);
            // 
            // copyToClipboardToolStripMenuItem
            // 
            this.copyToClipboardToolStripMenuItem.Name = "copyToClipboardToolStripMenuItem";
            this.copyToClipboardToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.copyToClipboardToolStripMenuItem.Text = "As Image (bitmap)";
            this.copyToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyToClipboardToolStripMenuItem_Click_1);
            // 
            // asFileToolStripMenuItem
            // 
            this.asFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyAsFile,
            this.copyAsJpg,
            this.CopyAsPNG});
            this.asFileToolStripMenuItem.Name = "asFileToolStripMenuItem";
            this.asFileToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.asFileToolStripMenuItem.Text = "As File";
            // 
            // copyAsFile
            // 
            this.copyAsFile.Name = "copyAsFile";
            this.copyAsFile.Size = new System.Drawing.Size(137, 22);
            this.copyAsFile.Text = "Bitmap (bmp)";
            // 
            // copyAsJpg
            // 
            this.copyAsJpg.Name = "copyAsJpg";
            this.copyAsJpg.Size = new System.Drawing.Size(137, 22);
            this.copyAsJpg.Text = "Jpeg (jpg)";
            this.copyAsJpg.Click += new System.EventHandler(this.copyAsJpg_Click);
            // 
            // CopyAsPNG
            // 
            this.CopyAsPNG.Name = "CopyAsPNG";
            this.CopyAsPNG.Size = new System.Drawing.Size(137, 22);
            this.CopyAsPNG.Text = "PNG";
            this.CopyAsPNG.Click += new System.EventHandler(this.CopyAsPNG_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // OverlayWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(475, 443);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "OverlayWindow";
            this.Opacity = 0.4;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OverlayWindow";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Cropper_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Cropper_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Cropper_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Cropper_MouseDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Cropper_KeyPress);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Cropper_MouseMove);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem saveToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jHPGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pNGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInPaintBrushToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAsFile;
        private System.Windows.Forms.ToolStripMenuItem copyAsJpg;
        private System.Windows.Forms.ToolStripMenuItem CopyAsPNG;
    }
}

