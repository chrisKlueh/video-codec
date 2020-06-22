using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

        BitArray[] YBitArray;
        BitArray[] CbBitArray;
        BitArray[] CrBitArray;

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

                // prepare bit arrays
                YBitArray = new BitArray[inputImages.Length];
                CbBitArray = new BitArray[inputImages.Length];
                CrBitArray = new BitArray[inputImages.Length];
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

            // YCbCrToRGB();

            //DCT & Quantization & Differential Encoding & Run Lenght Encoding
            Encoding();

            // Save our video file
            VideoFile video = new VideoFile(YBitArray, CbBitArray, CrBitArray);
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("akyio.bfv", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, video);
            stream.Close();
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
                        // Color conversion values from
                        // https://www.renesas.com/eu/en/www/doc/application-note/an9717.pdf
                        Color pixel = bitmap.GetPixel(x, y);
                        double Y = 0.257 * pixel.R + 0.504 * pixel.G + 0.098 * pixel.B + 16;
                        double Cb = -0.148 * pixel.R - 0.291 * pixel.G + 0.439 * pixel.B + 128;
                        double Cr = 0.439 * pixel.R - 0.368 * pixel.G - 0.071 * pixel.B + 128;
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
                        // Color conversion values from
                        // https://www.renesas.com/eu/en/www/doc/application-note/an9717.pdf
                        YCbCrPixel pixel = tempImages[i].GetPixel(x, y);
                        int r = (int)(1.164 * (pixel.getY() - 16) + 1.596 * (pixel.getCr() - 128));
                        int g = (int)(1.164 * (pixel.getY() - 16) - 0.813 * (pixel.getCr() - 128) - 0.392 * (pixel.getCb() - 128));
                        int b = (int)(1.164 * (pixel.getY() - 16) + 2.017 * (pixel.getCb() - 128));
                        // safety mechanism
                        getValidRGBValue(r, out r);
                        getValidRGBValue(g, out g);
                        getValidRGBValue(b, out b);
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

        private void getValidRGBValue(int value, out int validValue)
        {
            if(value < 0)
            {
                validValue = 0;
            } else if (value > 255)
            {
                validValue = 255;
            } else
            {
                validValue = value;
            }
        }

        private void Encoding()
        {
            progressLabel.Text = "Encoding...";
            progressLabel.Visible = true;
            progressBar.Value = 0;
            progressBar.Visible = true;
            // needed to update UI
            this.Update();

            int[,] yDctQuan, cBDctQuan, cRDctQuan, yDiffEncoded, cBDiffEncoded, cRDiffEncoded;
            int[] yRunLenEncoded, cBRunLenEncoded, cRRunLenEncoded;

            for (int i = 0; i < tempImages.Length; i++)
            {
                DctImage dctImage = new DctImage(tempImages[i]);
                yDctQuan = dctImage.PerformDctAndQuantization(tempImages[i], "Y");
                cBDctQuan = dctImage.PerformDctAndQuantization(tempImages[i], "Cb");
                cRDctQuan = dctImage.PerformDctAndQuantization(tempImages[i], "Cr");

                yDiffEncoded = DifferentialEncoding.Encode(yDctQuan, 8);
                cBDiffEncoded = DifferentialEncoding.Encode(cBDctQuan, 8);
                cRDiffEncoded = DifferentialEncoding.Encode(cRDctQuan, 8);

                yRunLenEncoded = RunLengthEncode.Encode(yDiffEncoded, 8);
                cBRunLenEncoded = RunLengthEncode.Encode(cBDiffEncoded, 8);
                cRRunLenEncoded = RunLengthEncode.Encode(cRDiffEncoded, 8);

                // huffman encoding
                YBitArray[i] = HuffmanEncoding(yRunLenEncoded);
                CbBitArray[i] = HuffmanEncoding(cBRunLenEncoded);
                CrBitArray[i] = HuffmanEncoding(cRRunLenEncoded);

                // garbage collection
                tempImages[i] = null;

                progressBar.Value = i;
            }

            progressLabel.Visible = false;
            progressBar.Visible = false;
            // needed to update UI
            this.Update();
        }

        private BitArray HuffmanEncoding(int[] array)
        {
            string huffmanData = "";
            IList<HuffmanNode> list = new List<HuffmanNode>();
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(new HuffmanNode("S" + (i + 1), array[i]));
            }
            Stack<HuffmanNode> stack = HuffmanNode.GetSortedStack(list);
            while (stack.Count > 1)
            {
                HuffmanNode leftChild = stack.Pop();
                HuffmanNode rightChild = stack.Pop();
                HuffmanNode parentNode = new HuffmanNode(leftChild, rightChild);
                stack.Push(parentNode);
                stack = HuffmanNode.GetSortedStack(stack.ToList<HuffmanNode>());
            }
            HuffmanNode parentNode1 = stack.Pop();
            HuffmanNode.GenerateCode(parentNode1, out huffmanData);

            BitArray bitArray = new BitArray(huffmanData.Length);
            for(int i = 0; i < huffmanData.Length; i++)
            {
                if(huffmanData[i].Equals("0"))
                {
                    bitArray[i] = false;
                } else
                {
                    bitArray[i] = true;
                }
            }
            return bitArray;
        }

        #endregion
    }
}
