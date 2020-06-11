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
        Image[] outputImages;

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
                if(inputCheckBox.Checked)
                {
                    inputPictureBox.Image = inputImages[timeBar.Value];
                }
            }
            if (outputImages != null && timeBar.Value < outputImages.Length)
            {
                if (outputCheckBox.Checked)
                {
                    outputPictureBox.Image = outputImages[timeBar.Value];
                }
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
            if ((inputCheckBox.Checked && (inputImages != null && timeBar.Value < inputImages.Length)) || (outputCheckBox.Checked && (outputImages != null && timeBar.Value < outputImages.Length)))
            {
                while(timeBar.Value < inputImages.Length - 1)
                {
                    timeBar.Value += 1;
                    if (inputCheckBox.Checked)
                    {
                        inputPictureBox.Image = inputImages[timeBar.Value];
                        inputPictureBox.Refresh();
                    }
                    if (outputCheckBox.Checked)
                    {
                        outputPictureBox.Image = outputImages[timeBar.Value];
                        outputPictureBox.Refresh();
                    }
                    // 1000 / 33  -->  ~ 30 fps
                    Thread.Sleep(33);
                }
            }
        }

        // Convert input using our codec
        private void convertButton_Click(object sender, EventArgs e)
        {
            // Convert RGB images to YCbCr images
            RGBToYCbCr();

            progressLabel.Text = "Chroma subsampling...";
            progressLabel.Visible = true;
            progressBar.Value = 0;
            progressBar.Visible = true;
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

            YCbCrToRGB();
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

        private void RGBToYCbCr()
        {
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
                        double Y = 0.2989 * pixel.R + 0.5866 * pixel.G + 0.1145 * pixel.B;
                        double Cb = -0.1687 * pixel.R - 0.3313 * pixel.G + 0.5000 * pixel.B;
                        double Cr = 0.5000 * pixel.R - 0.4184 * pixel.G - 0.0816 * pixel.B;
                        yCbCrImage.pixels[x, y] = new YCbCrPixel(Y, Cb, Cr);
                    }
                }
                tempImages[i] = yCbCrImage;
                progressBar.Value = i;
            }

            progressLabel.Visible = false;
            progressBar.Visible = false;
            // needed to update UI
            this.Update();
        }

        private void YCbCrToRGB()
        {
            progressLabel.Text = "Converting ycbcr to rgb...";
            progressLabel.Visible = true;
            progressBar.Value = 0;
            progressBar.Visible = true;
            // needed to update UI
            this.Update();

            outputImages = new Image[tempImages.Length];
            for (int i = 0; i < tempImages.Length; i++)
            {
                Bitmap bitmap = new Bitmap(tempImages[i].width, tempImages[i].height);
                for (int x = 0; x < tempImages[i].width; x++)
                {
                    for (int y = 0; y < tempImages[i].height; y++)
                    {
                        YCbCrPixel pixel = tempImages[i].GetPixel(x, y);
                        int r = (int)Math.Max(0.0f, Math.Min(1.0f, (double)(pixel.getY() + 0.0000 * pixel.getCb() + 1.4022 * pixel.getCr())));
                        int g = (int)Math.Max(0.0f, Math.Min(1.0f, (double)(pixel.getY() - 0.3456 * pixel.getCb() - 0.7145 * pixel.getCr())));
                        int b = (int)Math.Max(0.0f, Math.Min(1.0f, (double)(pixel.getY() + 1.7710 * pixel.getCb() + 0.0000 * pixel.getCr())));
                        Color color = Color.FromArgb(r, g, b);
                        bitmap.SetPixel(x, y, color);
                    }
                }
                outputImages[i] = bitmap;
                progressBar.Value = i;
            }

            progressLabel.Visible = false;
            progressBar.Visible = false;
            // needed to update UI
            this.Update();
        }

        #endregion
    }
}
