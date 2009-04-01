using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Capture;

namespace StopCrop
{
    public partial class OverlayWindow : Form
    {

        private bool Cropping;
        private Point CropStart;
        private Point CropStop;
        private Image Captured;
        private Image Cropped;

        private Rectangle CropRect
        {
            get{
                return TwoPointsToRect(CropStart, CropStop);
            }

        }

        private Image DoCrop(Rectangle rect)
        {
            Bitmap target = new Bitmap(CropRect.Width, CropRect.Height);
            Graphics g = Graphics.FromImage(target);
            g.DrawImage(Captured,
                0,
                0,
                rect,
                GraphicsUnit.Pixel);
            g.Save();
            return target;
        }


        public OverlayWindow()
        {
            InitializeComponent();
        }

        private void Cropper_Load(object sender, EventArgs e)
        {
        }

        private void Cropper_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                Close();
        }

        private void Cropper_MouseDown(object sender, MouseEventArgs e)
        {
            Cropping = true;

            CropStart = e.Location;


        }

        private Rectangle TwoPointsToRect(Point a, Point b)
        {
            Point TopLeft = new Point(
                Math.Min(a.X, b.X),
                Math.Min(a.Y, b.Y));
            int W = Math.Abs(a.X - b.X);
            int H = Math.Abs(a.Y - b.Y);
            return new Rectangle(TopLeft, new Size(W, H));
        }

        void DoUserChoice()
        {
            contextMenuStrip1.Show(Cursor.Position);


        }

        private void Cropper_MouseUp(object sender, MouseEventArgs e)
        {
            Cropping = false;
            CropStop = e.Location;
            if (CropStop == CropStart)
                return;
            Hide();
            CaptureBMP();
            Show();
            Cropped = DoCrop(TwoPointsToRect(CropStart, CropStop));
            DoUserChoice();
        }



        private void Cropper_MouseMove(object sender, MouseEventArgs e)
        {
            if (Cropping)
            {
                CropStop = e.Location;
                Refresh();
            }

        }

        private void CaptureBMP()
        {
            Captured = CaptureScreen.GetDesktopImage();
        }

        private void Cropper_Paint(object sender, PaintEventArgs e)
        {
            String header = "Stop, Crop!";
            Font f = new Font("Arial", 44, FontStyle.Bold | FontStyle.Italic);

            // Measure the text size
            Size headerSize = TextRenderer.MeasureText(header, f);

            // Figure out the correct location of the text so that 
            // it appears in the middle of the screen
            Point headerLocation = new Point(
                (Screen.PrimaryScreen.Bounds.Width - headerSize.Width) / 2,
                (Screen.PrimaryScreen.Bounds.Height - headerSize.Height) / 2
                );

            // Paint the header text in the middle of the screen
            e.Graphics.DrawString(
                header,
                f,
                new SolidBrush(Color.Black),
                headerLocation);

            // Paint the crop rectangle
            e.Graphics.FillRectangle(
                new SolidBrush(Color.White),
                TwoPointsToRect(CropStart, CropStop));

        }

        private void SaveCropped(System.Drawing.Imaging.ImageFormat format,
            String FileExtension)
        {
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = FileExtension;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Cropped.Save(saveFileDialog1.FileName, format);
                Close();
            };
        }

        private void jHPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCropped(System.Drawing.Imaging.ImageFormat.Jpeg, ".jpg");
        }

        private void bMPToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveCropped(System.Drawing.Imaging.ImageFormat.Bmp, ".bmp");

        }

        private void pNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCropped(System.Drawing.Imaging.ImageFormat.Png, ".png");

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        private string saveAsTempFile(System.Drawing.Imaging.ImageFormat format,
            String FileExtension)
        {
            String tempFile = Environment.GetEnvironmentVariable("TEMP")
                 + "\\tempCroppedImage" + Guid.NewGuid().ToString() + FileExtension;
            Cropped.Save(tempFile, format);
            return tempFile;
        }

        private void openInPaintBrushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("pbrush.exe", 
                saveAsTempFile(System.Drawing.Imaging.ImageFormat.Bmp, ".bmp"));
            Close();
        }

        private void CopyAsFile(System.Drawing.Imaging.ImageFormat format,
            String FileExtension)
        {
            String temp = saveAsTempFile(format, FileExtension);
            System.Collections.Specialized.StringCollection files = new
                System.Collections.Specialized.StringCollection();
            files.Add(temp);
            Clipboard.SetFileDropList(files);
        }
        private void copyAsFile_Click(object sender, EventArgs e)
        {
            CopyAsFile(System.Drawing.Imaging.ImageFormat.Bmp, ".bmp");
            Close();

        }

        private void copyToClipboardToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void copyAsJpg_Click(object sender, EventArgs e)
        {
            CopyAsFile(System.Drawing.Imaging.ImageFormat.Jpeg, ".jpg");
            Close();
        }

        private void CopyAsPNG_Click(object sender, EventArgs e)
        {
            CopyAsFile(System.Drawing.Imaging.ImageFormat.Png, ".png");
            Close();
        }

        private void copyToClipboardToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Clipboard.SetImage((Image)Cropped.Clone());
            Close();
        }




    }
}