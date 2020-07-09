using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Codec
{
    public partial class Form1 : Form
    {
        int keyFrameEvery = 30;
        int quality = 50;

        int width = 0;
        int height = 0;

        string inputFileName = null;
        Image[] inputImages;

        YCbCrImage[] tempImages;

        List<int>[] YBitArray;
        List<int>[] CbBitArray;
        List<int>[] CrBitArray;

        Huffman<int>[] YHuffmans;
        Huffman<int>[] CbHuffmans;
        Huffman<int>[] CrHuffmans;

        string outputFile = null;
        Image[] outputImages;

        int maxThreads = 4;

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



                // TODO: use full video?
                while (hasFrame == true && (!frameLimiter.Checked || count < Decimal.ToInt32(frameInput.Value)))
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

                // init result array lengths
                YBitArray = new List<int>[inputImages.Length];
                CbBitArray = new List<int>[inputImages.Length];
                CrBitArray = new List<int>[inputImages.Length];
                // init huffmans
                int len;
                if(inputImages.Length % keyFrameEvery == 0)
                {
                    len = inputImages.Length / keyFrameEvery;
                    
                } else
                {
                    len = (inputImages.Length / keyFrameEvery) + 1;
                }
                YHuffmans = new Huffman<int>[len];
                CbHuffmans = new Huffman<int>[len];
                CrHuffmans = new Huffman<int>[len];
            }
        }

        // show the chosen image(s)
        private void timeBar_ValueChanged(object sender, EventArgs e)
        {
            if (inputImages != null && timeBar.Value < inputImages.Length)
            {
                if (inputCheckBox.Checked)
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

        #region SaveButtons

        // set which frames will be a key frames (valid range is every 1 - 60 frames)
        private void keyFrameSaveButton_Click(object sender, EventArgs e)
        {
            int tempNum = Int32.Parse(keyFrameInput.Text);
            if (tempNum > 0 && tempNum <= 60)
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

        // choose the number of Threads
        private void multiThreadSaveButton_Click(object sender, EventArgs e)
        {
            maxThreads = Decimal.ToInt32(multiThreadInput.Value);
        }

        // choose the quality of the DCT
        private void qualitySaveButton_Click(object sender, EventArgs e)
        {
            quality = Decimal.ToInt32(qualityInput.Value);
        }

        #endregion

        // play the video
        private void playButton_Click(object sender, EventArgs e)
        {
            if ((inputCheckBox.Checked && (inputImages != null && timeBar.Value < inputImages.Length)) || (outputCheckBox.Checked && (outputImages != null && timeBar.Value < outputImages.Length)))
            {
                while (timeBar.Value < inputImages.Length - 1)
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
            GC.Collect();

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
            for (int i = 0; i < tempImages.Length; i++)
            {
                tempImages[i].SetSubsamplingMode(subsamplingMode);
                progressBar.Value = i;

                if(i % keyFrameEvery == 0)
                {
                    GC.Collect();
                }
            }

            progressLabel.Visible = false;
            progressBar.Visible = false;

            //DCT & Quantization & Differential Encoding & Run Lenght Encoding & Huffman Encoding
            Encoding();

            // Save our video file
            VideoFile outputVideo = new VideoFile(keyFrameEvery, quality, width, height, toBitArrayArray(YBitArray), toBitArrayArray(CbBitArray), toBitArrayArray(CrBitArray), YHuffmans, CbHuffmans, CrHuffmans);

            IFormatter encodingFormatter = new BinaryFormatter();
            Stream encodingStream = new FileStream("akyio.bfv", FileMode.Create, FileAccess.Write, FileShare.None);
            encodingFormatter.Serialize(encodingStream, outputVideo);
            encodingStream.Close();

            //// Garbage collection
            int tempImagesLen = tempImages.Length;
            int huffmansLen = YHuffmans.Length;
            int bitArrayLen = YBitArray.Length;
            tempImages = new YCbCrImage[tempImagesLen];
            YHuffmans = new Huffman<int>[huffmansLen];
            CbHuffmans = new Huffman<int>[huffmansLen];
            CrHuffmans = new Huffman<int>[huffmansLen];
            YBitArray = new List<int>[bitArrayLen];
            CbBitArray = new List<int>[bitArrayLen];
            CrBitArray = new List<int>[bitArrayLen];
            GC.Collect();

            ///////////////////////////////////////
            /// Encoding done - file saved.
            ///////////////////////////////////////
            /// Start Decoding
            ///////////////////////////////////////

            // TODO

            // read video file
            IFormatter decodingFormatter = new BinaryFormatter();
            Stream decodingStream = new FileStream("akyio.bfv", FileMode.Open, FileAccess.Read, FileShare.Read);
            VideoFile inputVideo = (VideoFile)decodingFormatter.Deserialize(decodingStream);
            outputSizeLabel.Text = "Output file size: " + BytesToString(decodingStream.Length);
            decodingStream.Close();
            GC.Collect();

            //DCT & Quantization & Differential Decoding & Run Lenght Decoding & Huffman Decoding
            Decoding(inputVideo);

            // Convert YCbCr images to RGB images
            YCbCrToRGB();
            GC.Collect();
        }

        #region Helper Methods

        private BitArray[] toBitArrayArray(List<int>[] listArray)
        {
            BitArray[] bitArrayArray = new BitArray[listArray.Length];
            for (int i = 0; i < listArray.Length; i++)
            {
                BitArray bitArray = new BitArray(listArray[i].Count);
                for (int j = 0; j < listArray[i].Count; j++)
                {
                    bitArray[j] = Convert.ToBoolean(listArray[i][j]);
                }
                bitArrayArray[i] = bitArray;
            }
            return bitArrayArray;
        }

        private List<int>[] toIntListArray(BitArray[] bitArrays)
        {
            List<int>[] listArray = new List<int>[bitArrays.Length];
            for (int i = 0; i < bitArrays.Length; i++)
            {
                List<int> list = new List<int>();
                for (int j = 0; j < bitArrays[i].Length; j++)
                {
                    if (bitArrays[i][j])
                    {
                        list.Add(1);
                    } else
                    {
                        list.Add(0);
                    }
                }
                listArray[i] = list;
            }
            return listArray;
        }

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

            // we need this later to save in our video file
            Bitmap tempbm = new Bitmap(inputImages[0]);
            width = tempbm.Width;
            height = tempbm.Height;

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

                if(i % keyFrameEvery == 0)
                {
                    GC.Collect();
                }
            }

            progressLabel.Visible = false;
            progressBar.Visible = false;
            // needed to update UI
            this.Update();
        }

        private void getValidRGBValue(int value, out int validValue)
        {
            if (value < 0)
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

            int len = tempImages.Length;
            int threadsXkeyFrames = maxThreads * keyFrameEvery;
            int possibleMultiFors = len / threadsXkeyFrames;

            Parallel.For(0, maxThreads, (i) =>
            {
                ParallelEncoding(i, possibleMultiFors);
                GC.Collect();
            });

            if(len % threadsXkeyFrames != 0)
            {
                int end = possibleMultiFors * keyFrameEvery * maxThreads;
                int leftOvers = len - end;
                int numOfThreadsForRest = leftOvers / keyFrameEvery;
                if(numOfThreadsForRest >= 1)
                {
                    Parallel.For(0, numOfThreadsForRest, (i) =>
                    {
                        ParallelEncoding(i, possibleMultiFors, end + (i * keyFrameEvery), end + ((i + 1) * keyFrameEvery));
                    });
                }
                leftOvers = leftOvers - (numOfThreadsForRest * keyFrameEvery);
                if (leftOvers > 0)
                {
                    ParallelEncoding(0, possibleMultiFors, tempImages.Length - leftOvers);
                }
            }

            GC.Collect();

            progressLabel.Visible = false;
            progressBar.Visible = false;
            // needed to update UI
            this.Update();
        }

        public void ParallelEncoding(int threadNum, int possibleMultiFors, int? startValue = null, int? endValue = null)
        {
            int[,] yDctQuan, cBDctQuan, cRDctQuan, yDiffEncoded, cBDiffEncoded, cRDiffEncoded;
            int[] yRunLenEncoded, cBRunLenEncoded, cRRunLenEncoded;

            // needed for multi huffman encoding
            int[][] YHuffmanValues = new int[keyFrameEvery][];
            int[][] CbHuffmanValues = new int[keyFrameEvery][];
            int[][] CrHuffmanValues = new int[keyFrameEvery][];

            int offset = possibleMultiFors * keyFrameEvery;
            int start;
            int finish;

            if (startValue != null)
            {
                start = (int)startValue;
                if(endValue != null)
                {
                    finish = (int)endValue;
                } else
                {
                    finish = tempImages.Length;
                }
            } else
            {
                start = threadNum * offset;
                finish = (threadNum + 1) * offset;
            }
            

            for (int i = start; i < finish; i++)
            {
                DctImage dctImage = new DctImage(tempImages[i], quality);
                yDctQuan = dctImage.PerformDctAndQuantization(tempImages[i], "Y");
                cBDctQuan = dctImage.PerformDctAndQuantization(tempImages[i], "Cb");
                cRDctQuan = dctImage.PerformDctAndQuantization(tempImages[i], "Cr");

                yDctQuan = dctImage.TrimValueMatrix(yDctQuan, width, height);
                cBDctQuan = dctImage.TrimValueMatrix(cBDctQuan, width, height);
                cRDctQuan = dctImage.TrimValueMatrix(cRDctQuan, width, height);

                // Tester.PrintToFile("yDctQuanBefore", yDctQuan);

                yDiffEncoded = DifferentialEncoding.Encode(yDctQuan, 8);
                cBDiffEncoded = DifferentialEncoding.Encode(cBDctQuan, 8);
                cRDiffEncoded = DifferentialEncoding.Encode(cRDctQuan, 8);

                //Tester.PrintToFile("yDiffEncodedBefore", yDiffEncoded);

                yRunLenEncoded = RunLengthEncode.Encode(yDiffEncoded, 8);
                cBRunLenEncoded = RunLengthEncode.Encode(cBDiffEncoded, 8);
                cRRunLenEncoded = RunLengthEncode.Encode(cRDiffEncoded, 8);

                //Tester.PrintToFile("yRunLenEncodedBefore", yRunLenEncoded);

                // huffman encoding
                bool lastFrame = false;
                if(i == tempImages.Length - 1)
                {
                    lastFrame = true;
                }
                MultiHuffmanEncoding(i, yRunLenEncoded, cBRunLenEncoded, cRRunLenEncoded, lastFrame, YHuffmanValues, CbHuffmanValues, CrHuffmanValues);

                // Tester.PrintToFile("huffmanBefore", YBitArray);

                // garbage collection
                //tempImages[i] = null;

                MethodInvoker mi = new MethodInvoker(() => {
                    int newValue = progressBar.Value + maxThreads;
                    if(newValue <= inputImages.Length)
                    {
                        progressBar.Value = newValue;
                    } else
                    {
                        progressBar.Value = inputImages.Length;
                    }
                });
                if (!progressBar.InvokeRequired)
                {
                    mi.Invoke();
                }
            }
        }

        private void Decoding(VideoFile video)
        {
            progressLabel.Text = "Decoding...";
            progressLabel.Visible = true;
            progressBar.Value = 0;
            progressBar.Visible = true;
            // needed to update UI
            this.Update();

            YBitArray = toIntListArray(video.YBitArray);
            CbBitArray = toIntListArray(video.CbBitArray);
            CrBitArray = toIntListArray(video.CrBitArray);

            Parallel.For(0, maxThreads, (i) =>
            {
                ParallelDecoding(i, video);
                GC.Collect();
            });

            progressLabel.Visible = false;
            progressBar.Visible = false;
            // needed to update UI
            this.Update();
        }

        private void ParallelDecoding(int threadNum, VideoFile video)
        {
            int[,] yDctQuan, cBDctQuan, cRDctQuan, yDiffEncoded, cBDiffEncoded, cRDiffEncoded;
            int[] yRunLenEncoded, cBRunLenEncoded, cRRunLenEncoded;

            int offset = tempImages.Length / maxThreads;
            int start = threadNum * offset;
            int finish = threadNum != (maxThreads - 1) ? (threadNum + 1) * offset : tempImages.Length;

            for (int i = start; i < finish; i++)
            {
                // huffman decoding
                yRunLenEncoded = HuffmanDecoding(YBitArray[i], video.YHuffmans[i / keyFrameEvery]);
                cBRunLenEncoded = HuffmanDecoding(CbBitArray[i], video.CbHuffmans[i / keyFrameEvery]);
                cRRunLenEncoded = HuffmanDecoding(CrBitArray[i], video.CrHuffmans[i / keyFrameEvery]);

                //Tester.PrintToFile("yRunLenEncodedAfter", yRunLenEncoded);

                // run length decoding
                yDiffEncoded = RunLengthEncode.Decode(yRunLenEncoded, 8, video.width, video.height);
                cBDiffEncoded = RunLengthEncode.Decode(cBRunLenEncoded, 8, video.width, video.height);
                cRDiffEncoded = RunLengthEncode.Decode(cRRunLenEncoded, 8, video.width, video.height);

                //Tester.PrintToFile("yDiffEncodedAfter", yDiffEncoded);

                // differential decoding
                yDctQuan = DifferentialEncoding.Decode(yDiffEncoded, 8);
                cBDctQuan = DifferentialEncoding.Decode(cBDiffEncoded, 8);
                cRDctQuan = DifferentialEncoding.Decode(cRDiffEncoded, 8);

                //Tester.PrintToFile("yDctQuanAfter", yDctQuan);

                // revert dct and quantization
                DctImage dctImage = new DctImage(quality);
                int[,] YMatrix = dctImage.RevertDctAndQuantization(yDctQuan);
                int[,] CbMatrix = dctImage.RevertDctAndQuantization(cBDctQuan);
                int[,] CrMatrix = dctImage.RevertDctAndQuantization(cRDctQuan);

                YMatrix = dctImage.TrimValueMatrix(YMatrix, video.width, video.height);
                CbMatrix = dctImage.TrimValueMatrix(CbMatrix, video.width, video.height);
                CrMatrix = dctImage.TrimValueMatrix(CrMatrix, video.width, video.height);

                // instantiate YCbCr images
                YCbCrImage tempImage = new YCbCrImage(YMatrix.GetLength(0), YMatrix.GetLength(1));
                for (int j = 0; j < YMatrix.GetLength(0); j++)
                {
                    for (int k = 0; k < YMatrix.GetLength(1); k++)
                    {
                        tempImage.pixels[j, k] = new YCbCrPixel(YMatrix[j, k], CbMatrix[j, k], CrMatrix[j, k]);
                    }
                }
                tempImages[i] = tempImage;

                MethodInvoker mi = new MethodInvoker(() => {
                    int newValue = progressBar.Value + maxThreads;
                    if (newValue <= inputImages.Length)
                    {
                        progressBar.Value = newValue;
                    }
                    else
                    {
                        progressBar.Value = inputImages.Length;
                    }
                });
                if (!progressBar.InvokeRequired)
                {
                    mi.Invoke();
                }
            }
        }

        private void MultiHuffmanEncoding(int i, int[] yRunLenEncoded, int[] cBRunLenEncoded, int[] cRRunLenEncoded, bool isLastFrame, int[][] YHuffmanValues, int[][] CbHuffmanValues, int[][] CrHuffmanValues)
        {
            YHuffmanValues[i % keyFrameEvery] = yRunLenEncoded;
            CbHuffmanValues[i % keyFrameEvery] = cBRunLenEncoded;
            CrHuffmanValues[i % keyFrameEvery] = cRRunLenEncoded;

            if (isLastFrame)
            {
                int[] yTemp = new int[0];
                int[] cBTemp = new int[0];
                int[] cRTemp = new int[0];

                int frameAfterLastKeyFrame = i - (i % keyFrameEvery);
                int max = (i % keyFrameEvery) + 1;

                for (int j = 0; j < max; j++)
                {
                    int yLength = yTemp.Length;
                    int cBLength = cBTemp.Length;
                    int cRLength = cRTemp.Length;
                    Array.Resize(ref yTemp, yLength + YHuffmanValues[j].Length);
                    Array.Resize(ref cBTemp, cBLength + CbHuffmanValues[j].Length);
                    Array.Resize(ref cRTemp, cRLength + CrHuffmanValues[j].Length);
                    Array.Copy(YHuffmanValues[j], 0, yTemp, yLength, YHuffmanValues[j].Length);
                    Array.Copy(CbHuffmanValues[j], 0, cBTemp, cBLength, CbHuffmanValues[j].Length);
                    Array.Copy(CrHuffmanValues[j], 0, cRTemp, cRLength, CrHuffmanValues[j].Length);
                }

                Huffman<int> YHuffman = new Huffman<int>(yTemp);
                Huffman<int> CbHuffman = new Huffman<int>(cBTemp);
                Huffman<int> CrHuffman = new Huffman<int>(cRTemp);

                for (int j = 0; j < max; j++)
                {
                    int k = frameAfterLastKeyFrame + j;
                    YBitArray[k] = HuffmanEncoding(YHuffman, YHuffmanValues[j]);
                    CbBitArray[k] = HuffmanEncoding(CbHuffman, CbHuffmanValues[j]);
                    CrBitArray[k] = HuffmanEncoding(CrHuffman, CrHuffmanValues[j]);
                }

                YHuffmans[i / keyFrameEvery] = YHuffman;
                CbHuffmans[i / keyFrameEvery] = CbHuffman;
                CrHuffmans[i / keyFrameEvery] = CrHuffman;
            } else if (i % keyFrameEvery == keyFrameEvery - 1)
            {
                int[] yTemp = new int[0];
                int[] cBTemp = new int[0];
                int[] cRTemp = new int[0];

                for(int j = 0; j < keyFrameEvery; j++)
                {
                    int yLength = yTemp.Length;
                    int cBLength = cBTemp.Length;
                    int cRLength = cRTemp.Length;
                    Array.Resize(ref yTemp, yLength + YHuffmanValues[j].Length);
                    Array.Resize(ref cBTemp, cBLength + CbHuffmanValues[j].Length);
                    Array.Resize(ref cRTemp, cRLength + CrHuffmanValues[j].Length);
                    Array.Copy(YHuffmanValues[j], 0, yTemp, yLength, YHuffmanValues[j].Length);
                    Array.Copy(CbHuffmanValues[j], 0, cBTemp, cBLength, CbHuffmanValues[j].Length);
                    Array.Copy(CrHuffmanValues[j], 0, cRTemp, cRLength, CrHuffmanValues[j].Length);
                }

                Huffman<int> YHuffman = new Huffman<int>(yTemp);
                Huffman<int> CbHuffman = new Huffman<int>(cBTemp);
                Huffman<int> CrHuffman = new Huffman<int>(cRTemp);

                for (int j = 0; j < keyFrameEvery; j++)
                {
                    int k = i - keyFrameEvery + 1 + j;
                    YBitArray[k] = HuffmanEncoding(YHuffman, YHuffmanValues[j]);
                    CbBitArray[k] = HuffmanEncoding(CbHuffman, CbHuffmanValues[j]);
                    CrBitArray[k] = HuffmanEncoding(CrHuffman, CrHuffmanValues[j]);
                }

                YHuffmans[i / keyFrameEvery] = YHuffman;
                CbHuffmans[i / keyFrameEvery] = CbHuffman;
                CrHuffmans[i / keyFrameEvery] = CrHuffman;
            }
        }

        private List<int> HuffmanEncoding(Huffman<int> huffman, int[] array)
        {
            return huffman.Encode(array);
        }

        private int[] HuffmanDecoding(List<int> list, Huffman<int> huffman)
        {
            return huffman.Decode(list).ToArray();
        }

        #endregion
    }
}
