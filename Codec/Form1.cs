using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Codec
{
    public partial class Form1 : Form
    {
        string inputFileName = null;
        Image[] inputImages;

        string outputFile = null;

        public Form1()
        {
            InitializeComponent();
        }

        // If no input file is selected, clicking the input picture makes the user choose a file.
        private void inputPictureBox_Click(object sender, EventArgs e)
        {
            if (inputFileName == null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Application.StartupPath;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    inputFileName = ofd.FileName;
                    inputSizeLabel.Text = "Input file size: " + BytesToString(new FileInfo(ofd.FileName).Length);
                }
                // Show conversion progress
                progressLabel.Text = "Importing file...";
                progressLabel.Visible = true;
                progressBar.Value = 0;
                progressBar.Visible = true;
                // Convert input video to image array
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

                ArrayList inputImagesAL = new ArrayList();
                var hasFrame = true;
                var count = 0;

                while (hasFrame == true)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        // video has 30 fps
                        ffMpeg.GetVideoThumbnail(inputFileName, stream, (count / 30f));
                        if (stream.Length != 0)
                        {
                            inputImagesAL.Add(Image.FromStream(stream));
                            progressBar.Value = count;
                            count++;
                        }
                        else
                        {
                            hasFrame = false;
                        }
                    }
                }
                inputImages = Array.ConvertAll(inputImagesAL.ToArray(), image => (Image)image);
                inputPictureBox.Image = inputImages[timeBar.Value];
                progressLabel.Visible = false;
                progressBar.Visible = false;
            }
        }

        // show the chosen image(s)
        private void timeBar_ValueChanged(object sender, EventArgs e)
        {
            if(inputImages != null && timeBar.Value < inputImages.Length)
            {
                inputPictureBox.Image = inputImages[timeBar.Value];
            }
        }

        // Convert input using our codec
        private void convertButton_Click(object sender, EventArgs e)
        {
            // TODO
        }

        #region Helper Methods

        // easy way to display filesize how we are used to see it
        // https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
        static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        #endregion
    }
}
