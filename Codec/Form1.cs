using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Codec
{
    public partial class Form1 : Form
    {
        int keyFrameEvery = 30;

        string inputFileName = null;
        Image[] inputImages;

        YCbCrImage[] tempImages;

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
        
        // set which frames will be a key frames (valid range is every 1 - 60 frames)
        private void keyFrameSaveButton_Click(object sender, EventArgs e)
        {
            int tempNum = Int32.Parse(keyFrameInput.Text);
            if(tempNum > 0 && tempNum <= 60)
            {
                keyFrameEvery = tempNum;
                timeBar.LargeChange = keyFrameEvery;
                timeBar.SmallChange = keyFrameEvery;
                timeBar.TickFrequency = keyFrameEvery;
            } else
            {
                // TODO: alert error?
            }
        }

        // play the video
        private void playButton_Click(object sender, EventArgs e)
        {
            if (inputImages != null && timeBar.Value < inputImages.Length)
            {
                while(timeBar.Value < inputImages.Length - 1)
                {
                    timeBar.Value += 1;
                    inputPictureBox.Image = inputImages[timeBar.Value];
                    inputPictureBox.Refresh();
                    // 1000 / 33  -->  ~ 30 fps
                    Thread.Sleep(33);
                }
            }
        }

        // Convert input using our codec
        private void convertButton_Click(object sender, EventArgs e)
        {
            // Convert RGB images to YCbCr images
            progressLabel.Text = "Converting rgb to ycbcr...";
            progressLabel.Visible = true;
            progressBar.Value = 0;
            progressBar.Visible = true;
            // needed to update UI
            this.Update();

            tempImages = new YCbCrImage[inputImages.Length];
            for (int i = 0; i < inputImages.Length; i++)
            {
                Bitmap bitmap = new Bitmap(inputImages[i]);
                YCbCrImage yCbCrImage = new YCbCrImage(bitmap.Width, bitmap.Height);
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Color pixel = bitmap.GetPixel(x, y);
                        double Y =  pixel.R * 0.299000 + pixel.G * 0.587000 + pixel.B * 0.114000;
                        double Cb = pixel.R * -0.168736 + pixel.G * -0.331264 + pixel.B * 0.500000 + 128;
                        double Cr = pixel.R * 0.500000 + pixel.G * -0.418688 + pixel.B * -0.081312 + 128;
                        yCbCrImage.pixels[x, y] = new YCbCrPixel(Y, Cb, Cr);
                    }
                }
                tempImages[i] = yCbCrImage;
                progressBar.Value = i;
            }

            progressLabel.Text = "Chroma subsampling...";
            progressBar.Value = 0;
            // needed to update UI
            this.Update();

            // Color subsampling
            string subsamplingMode = "4:4:4";
            int B = Int32.Parse(colorBinput.Text);
            int C = Int32.Parse(colorCinput.Text);
            if (B == 2)
            {
                if (C == 2)
                {
                    subsamplingMode = "4:2:2";
                }
                else if (C == 0)
                {
                    subsamplingMode = "4:2:0";
                }
            }
            for(int i = 0; i < tempImages.Length; i++)
            {
                tempImages[i].SetSubsamplingMode(subsamplingMode);
                progressBar.Value = i;
            }

            progressLabel.Visible = false;
            progressBar.Visible = false;

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
