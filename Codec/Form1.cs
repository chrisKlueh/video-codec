using Codec.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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

        Dictionary<int, int>[] YHuffmanCounts;
        Dictionary<int, int>[] CbHuffmanCounts;
        Dictionary<int, int>[] CrHuffmanCounts;

        string outputFile = null;
        Image[] outputImages;

        public string subsamplingMode = "4:4:4";

        int maxThreads = 4;

        public Form1()
        {
            InitializeComponent();
            chromaBox.SetItemChecked(0, true);
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

                if(inputFileName.Substring(inputFileName.Length - 3) == "bfv")
                {
                    // file is already encoded
                    DisableEncodingUI();

                    // read video file
                    IFormatter decodingFormatter = new BinaryFormatter();
                    Stream decodingStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    VideoFile inputVideo = (VideoFile)decodingFormatter.Deserialize(decodingStream);
                    outputSizeLabel.Text = "Output file size: " + BytesToString(decodingStream.Length);
                    decodingStream.Close();
                    GC.Collect();

                    //init
                    tempImages = new YCbCrImage[inputVideo.YBitArray.Length];
                    inputImages = new Image[tempImages.Length];
                    outputImages = new Image[tempImages.Length];
                    progressBar.Maximum = tempImages.Length;

                    //DCT & Quantization & Differential Decoding & Run Lenght Decoding & Huffman Decoding
                    Decoding(inputVideo);

                    // Convert YCbCr images to RGB images
                    YCbCrToRGB();

                    GC.Collect();

                    // show first picture
                    outputPictureBox.Image = outputImages[timeBar.Value];
                    
                } else
                {
                    // normal file

                    progressLabel.Text = "Importing file...";
                    progressLabel.Visible = true;
                    progressBar.Value = 0;
                    progressBar.Visible = true;
                    // Convert input video to image array
                    var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

                    ArrayList inputImagesAL = new ArrayList();
                    var hasFrame = true;
                    var count = 0;

                    if (frameLimiter.Checked)
                    {
                        progressBar.Maximum = Decimal.ToInt32(frameInput.Value);
                    }

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
                    progressBar.Maximum = count + 1;
                    progressLabel.Visible = false;
                    progressBar.Visible = false;

                    // init result array lengths
                    YBitArray = new List<int>[inputImages.Length];
                    CbBitArray = new List<int>[inputImages.Length];
                    CrBitArray = new List<int>[inputImages.Length];
                    // init huffmans
                    UpdateHuffmanCounts();
                }
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
                UpdateHuffmanCounts();
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

            //DCT & Quantization & Differential Encoding & Run Lenght Encoding & Huffman Encoding
            Encoding();

            // Save our video file
            VideoFile outputVideo = new VideoFile(keyFrameEvery, quality, width, height, subsamplingMode, toBitArrayArray(YBitArray), toBitArrayArray(CbBitArray), toBitArrayArray(CrBitArray), YHuffmanCounts, CbHuffmanCounts, CrHuffmanCounts);

            IFormatter encodingFormatter = new BinaryFormatter();
            Stream encodingStream = new FileStream("akyio.bfv", FileMode.Create, FileAccess.Write, FileShare.None);
            encodingFormatter.Serialize(encodingStream, outputVideo);
            encodingStream.Close();

            //// Garbage collection
            int tempImagesLen = tempImages.Length;
            int huffmansLen = YHuffmanCounts.Length;
            int bitArrayLen = YBitArray.Length;
            tempImages = new YCbCrImage[tempImagesLen];
            YHuffmanCounts = new Dictionary<int, int>[huffmansLen];
            CbHuffmanCounts = new Dictionary<int, int>[huffmansLen];
            CrHuffmanCounts = new Dictionary<int, int>[huffmansLen];
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

        private void DisableEncodingUI()
        {
            frameLimiter.Visible = false;
            frameInput.Visible = false;

            keyFrameLabel1.Visible = false;
            keyFrameLabel2.Visible = false;
            keyFrameInput.Visible = false;
            keyFrameSaveButton.Visible = false;

            ColorSubSamplingLabel.Visible = false;
            chromaBox.Visible = false;

            qualityLabel.Visible = false;
            qualityInput.Visible = false;
            qualitySaveButton.Visible = false;

            convertButton.Visible = false;

            bfvLabel.Visible = true;

            inputPictureBox.Image = Resources.encodedDefault;

            inputCheckBox.Checked = false;
            inputCheckBox.Enabled = false;
            outputCheckBox.Checked = true;
        }

        private void UpdateHuffmanCounts()
        {
            int len;
            if (inputImages.Length % keyFrameEvery == 0)
            {
                len = inputImages.Length / keyFrameEvery;

            }
            else
            {
                len = (inputImages.Length / keyFrameEvery) + 1;
            }
            YHuffmanCounts = new Dictionary<int, int>[len];
            CbHuffmanCounts = new Dictionary<int, int>[len];
            CrHuffmanCounts = new Dictionary<int, int>[len];
        }

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
            outputImages = new Image[tempImages.Length];
            for (int i = 0; i < inputImages.Length; i++)
            {
                Bitmap bitmap = new Bitmap(inputImages[i]);
                YCbCrImage yCbCrImage = new YCbCrImage(bitmap.Width, bitmap.Height, subsamplingMode);
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

            // multi threading makes sense!
            if(possibleMultiFors > 0)
            {
                Parallel.For(0, maxThreads, (i) =>
                {
                    ParallelEncoding(i, possibleMultiFors, maxThreads);
                    GC.Collect();
                });
            }

            if(len % threadsXkeyFrames != 0)
            {
                int end = possibleMultiFors * keyFrameEvery * maxThreads;
                int leftOvers = len - end;
                int numOfThreadsForRest = leftOvers / keyFrameEvery;
                if(numOfThreadsForRest >= 1)
                {
                    Parallel.For(0, numOfThreadsForRest, (i) =>
                    {
                        ParallelEncoding(i, possibleMultiFors, numOfThreadsForRest, end + (i * keyFrameEvery), end + ((i + 1) * keyFrameEvery));
                    });
                }
                leftOvers = leftOvers - (numOfThreadsForRest * keyFrameEvery);
                if (leftOvers > 0)
                {
                    ParallelEncoding(0, possibleMultiFors, 1, tempImages.Length - leftOvers);
                }
            }

            GC.Collect();

            progressLabel.Visible = false;
            progressBar.Visible = false;
            // needed to update UI
            this.Update();
        }

        public void ParallelEncoding(int threadNum, int possibleMultiFors, int numOfThreads, int? startValue = null, int? endValue = null)
        {
            int[,] yDctQuan, cBDctQuan, cRDctQuan, yDiffEncoded, cBDiffEncoded, cRDiffEncoded;
            int[] yRunLenEncoded, cBRunLenEncoded, cRRunLenEncoded;
            int[,] accumulatedChangesY = null;
            int[,] accumulatedChangesCb = null;
            int[,] accumulatedChangesCr = null;
            int[,] actualValuesY = null;
            int[,] actualValuesCb = null;
            int[,] actualValuesCr = null;
            List<int[,]> actualValuesListY = new List<int[,]>();
            List<int[,]> actualValuesListCb = new List<int[,]>();
            List<int[,]> actualValuesListCr = new List<int[,]>();

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

            int[,] yDctQuanDiff = null;
            int[,] cBDctQuanDiff = null;
            int[,] cRDctQuanDiff = null;
            int[,] yDctQuanFromLastFrame = null;
            int[,] cBDctQuanFromLastFrame = null;
            int[,] cRDctQuanFromLastFrame = null;

            for (int i = start; i < finish; i++)
            {
                DctImage dctImage = new DctImage(tempImages[i], quality, actualValuesListY, actualValuesListCb, actualValuesListCr, actualValuesY, actualValuesCb, actualValuesCr, accumulatedChangesY, accumulatedChangesCb, accumulatedChangesCr);

                yDctQuan = dctImage.PerformDctAndQuantization(tempImages[i], "Y");
                cBDctQuan = dctImage.PerformDctAndQuantization(tempImages[i], "Cb");
                cRDctQuan = dctImage.PerformDctAndQuantization(tempImages[i], "Cr");

                // it's not a keyframe
                if (i % keyFrameEvery != 0)
                {
                    for (int j = 0; j < yDctQuanFromLastFrame.GetLength(0); j++)
                    {
                        for (int k = 0; k < yDctQuanFromLastFrame.GetLength(1); k++)
                        {

                            //yDctQuanDiff[j, k] = GetOptimizedDifference(yDctQuan[j, k] - yDctQuanFromLastFrame[j, k], "y");
                            yDctQuanDiff[j, k] = yDctQuan[j, k] - yDctQuanFromLastFrame[j, k];
                            if (subsamplingMode == "4:4:4")
                            {
                                //cBDctQuanDiff[j, k] = GetOptimizedDifference(cBDctQuan[j, k] - cBDctQuanFromLastFrame[j, k], "cB");
                                cBDctQuanDiff[j, k] = cBDctQuan[j, k] - cBDctQuanFromLastFrame[j, k];
                                //cRDctQuanDiff[j, k] = GetOptimizedDifference(cRDctQuan[j, k] - cRDctQuanFromLastFrame[j, k], "cR");
                                cRDctQuanDiff[j, k] = cRDctQuan[j, k] - cRDctQuanFromLastFrame[j, k];
                            }
                        }
                    }
                    if (subsamplingMode != "4:4:4")
                    {
                        for (int j = 0; j < cBDctQuanFromLastFrame.GetLength(0); j++)
                        {
                            for (int k = 0; k < cBDctQuanFromLastFrame.GetLength(1); k++)
                            {

                                //cBDctQuanDiff[j, k] = GetOptimizedDifference(cBDctQuan[j, k] - cBDctQuanFromLastFrame[j, k], "cB");
                                cBDctQuanDiff[j, k] = cBDctQuan[j, k] - cBDctQuanFromLastFrame[j, k];
                                //cRDctQuanDiff[j, k] = GetOptimizedDifference(cRDctQuan[j, k] - cRDctQuanFromLastFrame[j, k], "cR");

                                cRDctQuanDiff[j, k] = cRDctQuan[j, k] - cRDctQuanFromLastFrame[j, k];
                            }
                        }
                    }
                } else
                {
                    // but actually it's a keyframe
                    yDctQuanDiff = new int[yDctQuan.GetLength(0), yDctQuan.GetLength(1)];
                    cBDctQuanDiff = new int[cBDctQuan.GetLength(0), cBDctQuan.GetLength(1)];
                    cRDctQuanDiff = new int[cRDctQuan.GetLength(0), cRDctQuan.GetLength(1)];

                    accumulatedChangesY = new int[yDctQuan.GetLength(0), yDctQuan.GetLength(1)];
                    accumulatedChangesCb = new int[cBDctQuan.GetLength(0), cBDctQuan.GetLength(1)];
                    accumulatedChangesCr = new int[cRDctQuan.GetLength(0), cRDctQuan.GetLength(1)];
                    actualValuesY = new int[yDctQuan.GetLength(0), yDctQuan.GetLength(1)];
                    actualValuesCb = new int[cBDctQuan.GetLength(0), cBDctQuan.GetLength(1)];
                    actualValuesCr = new int[cRDctQuan.GetLength(0), cRDctQuan.GetLength(1)];
                    for (int x = 0; x < accumulatedChangesY.GetLength(0); x++)
                    {
                        for (int y = 0; y < accumulatedChangesY.GetLength(1); y++)
                        {
                            accumulatedChangesY[x, y] = int.MaxValue;
                            actualValuesY[x, y] = int.MaxValue;
                        }
                    }
                    for (int x = 0; x < accumulatedChangesCb.GetLength(0); x++)
                    {
                        for (int y = 0; y < accumulatedChangesCb.GetLength(1); y++)
                        {
                            accumulatedChangesCb[x, y] = int.MaxValue;
                            accumulatedChangesCr[x, y] = int.MaxValue;
                            actualValuesCb[x, y] = int.MaxValue;
                            actualValuesCr[x, y] = int.MaxValue;
                        }
                    }
                    //actualValuesListY.Clear();
                    //actualValuesListCb.Clear();
                    //actualValuesListCr.Clear();
                }

                yDctQuanFromLastFrame = yDctQuan;
                cBDctQuanFromLastFrame = cBDctQuan;
                cRDctQuanFromLastFrame = cRDctQuan;

                // it's not a keyframe
                if (i % keyFrameEvery != 0)
                {
                    yDctQuan = yDctQuanDiff;
                    cBDctQuan = cBDctQuanDiff;
                    cRDctQuan = cRDctQuanDiff;
                }

                if (subsamplingMode == "4:4:4")
                {
                    yDctQuan = dctImage.TrimValueMatrix(yDctQuan, width, height);
                    cBDctQuan = dctImage.TrimValueMatrix(cBDctQuan, width, height);
                    cRDctQuan = dctImage.TrimValueMatrix(cRDctQuan, width, height);
                } else if (subsamplingMode == "4:2:2")
                {
                    yDctQuan = dctImage.TrimValueMatrix(yDctQuan, width, height);
                    cBDctQuan = dctImage.TrimValueMatrix(cBDctQuan, width / 2, height);
                    cRDctQuan = dctImage.TrimValueMatrix(cRDctQuan, width / 2, height);
                } else if (subsamplingMode == "4:2:0")
                {
                    yDctQuan = dctImage.TrimValueMatrix(yDctQuan, width, height);
                    cBDctQuan = dctImage.TrimValueMatrix(cBDctQuan, width / 2, height / 2);
                    cRDctQuan = dctImage.TrimValueMatrix(cRDctQuan, width / 2, height / 2);
                }

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
                    int newValue = progressBar.Value + numOfThreads;
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

            int len = tempImages.Length;
            int threadsXkeyFrames = maxThreads * keyFrameEvery;
            int possibleMultiFors = len / threadsXkeyFrames;

            subsamplingMode = video.subsamplingMode;

            YBitArray = toIntListArray(video.YBitArray);
            CbBitArray = toIntListArray(video.CbBitArray);
            CrBitArray = toIntListArray(video.CrBitArray);

            // multi threading makes sense!
            if (possibleMultiFors > 0)
            {
                Parallel.For(0, maxThreads, (i) =>
                {
                    ParallelDecoding(i, video, possibleMultiFors, maxThreads);
                    GC.Collect();
                });
            }

            if (len % threadsXkeyFrames != 0)
            {
                int end = possibleMultiFors * keyFrameEvery * maxThreads;
                int leftOvers = len - end;
                int numOfThreadsForRest = leftOvers / keyFrameEvery;
                if (numOfThreadsForRest >= 1)
                {
                    Parallel.For(0, numOfThreadsForRest, (i) =>
                    {
                        ParallelDecoding(i, video, possibleMultiFors, numOfThreadsForRest, end + (i * keyFrameEvery), end + ((i + 1) * keyFrameEvery));
                        GC.Collect();
                    });
                }
                leftOvers = leftOvers - (numOfThreadsForRest * keyFrameEvery);
                if (leftOvers > 0)
                {
                    ParallelDecoding(0, video, possibleMultiFors, 1, tempImages.Length - leftOvers);
                }
            }

            progressLabel.Visible = false;
            progressBar.Visible = false;
            // needed to update UI
            this.Update();
        }

        private void ParallelDecoding(int threadNum, VideoFile video, int possibleMultiFors, int numOfThreads, int? startValue = null, int? endValue = null)
        {
            int[,] yDctQuan, cBDctQuan, cRDctQuan, yDiffEncoded, cBDiffEncoded, cRDiffEncoded;
            int[] yRunLenEncoded, cBRunLenEncoded, cRRunLenEncoded;

            int offset = possibleMultiFors * keyFrameEvery;
            int start;
            int finish;

            if (startValue != null)
            {
                start = (int)startValue;
                if (endValue != null)
                {
                    finish = (int)endValue;
                }
                else
                {
                    finish = tempImages.Length;
                }
            }
            else
            {
                start = threadNum * offset;
                finish = (threadNum + 1) * offset;
            }

            int[,] yDctQuanDiff = null;
            int[,] cBDctQuanDiff = null;
            int[,] cRDctQuanDiff = null;
            int[,] yDctQuanFromLastFrame = null;
            int[,] cBDctQuanFromLastFrame = null;
            int[,] cRDctQuanFromLastFrame = null;

            for (int i = start; i < finish; i++)
            {
                // huffman decoding
                yRunLenEncoded = HuffmanDecoding(YBitArray[i], video.YHuffmanCounts[i / keyFrameEvery]);
                cBRunLenEncoded = HuffmanDecoding(CbBitArray[i], video.CbHuffmanCounts[i / keyFrameEvery]);
                cRRunLenEncoded = HuffmanDecoding(CrBitArray[i], video.CrHuffmanCounts[i / keyFrameEvery]);

                //Tester.PrintToFile("yRunLenEncodedAfter", yRunLenEncoded);

                // run length decoding
                if (subsamplingMode == "4:4:4")
                {
                    yDiffEncoded = RunLengthEncode.Decode(yRunLenEncoded, 8, video.width, video.height);
                    cBDiffEncoded = RunLengthEncode.Decode(cBRunLenEncoded, 8, video.width, video.height);
                    cRDiffEncoded = RunLengthEncode.Decode(cRRunLenEncoded, 8, video.width, video.height);
                }
                else if (subsamplingMode == "4:2:2")
                {
                    yDiffEncoded = RunLengthEncode.Decode(yRunLenEncoded, 8, video.width, video.height);
                    cBDiffEncoded = RunLengthEncode.Decode(cBRunLenEncoded, 8, video.width / 2, video.height);
                    cRDiffEncoded = RunLengthEncode.Decode(cRRunLenEncoded, 8, video.width / 2, video.height);
                }
                else
                {
                    yDiffEncoded = RunLengthEncode.Decode(yRunLenEncoded, 8, video.width, video.height);
                    cBDiffEncoded = RunLengthEncode.Decode(cBRunLenEncoded, 8, video.width / 2, video.height / 2);
                    cRDiffEncoded = RunLengthEncode.Decode(cRRunLenEncoded, 8, video.width / 2, video.height / 2);
                }

                //Tester.PrintToFile("yDiffEncodedAfter", yDiffEncoded);

                // differential decoding
                yDctQuan = DifferentialEncoding.Decode(yDiffEncoded, 8);
                cBDctQuan = DifferentialEncoding.Decode(cBDiffEncoded, 8);
                cRDctQuan = DifferentialEncoding.Decode(cRDiffEncoded, 8);

                // it's not a keyframe
                if (i % keyFrameEvery != 0)
                {
                    yDctQuanDiff = yDctQuan;
                    cBDctQuanDiff = cBDctQuan;
                    cRDctQuanDiff = cRDctQuan;
                    for (int j = 0; j < yDctQuanFromLastFrame.GetLength(0); j++)
                    {
                        for (int k = 0; k < yDctQuanFromLastFrame.GetLength(1); k++)
                        {
                            yDctQuan[j, k] = yDctQuanFromLastFrame[j, k] + yDctQuanDiff[j, k];
                            if (subsamplingMode == "4:4:4")
                            {
                                cBDctQuan[j, k] = cBDctQuanFromLastFrame[j, k] + cBDctQuanDiff[j, k];
                                cRDctQuan[j, k] = cRDctQuanFromLastFrame[j, k] + cRDctQuanDiff[j, k];
                            }
                        }
                    }
                    if (subsamplingMode != "4:4:4")
                    {
                        for (int j = 0; j < cBDctQuanFromLastFrame.GetLength(0); j++)
                        {
                            for (int k = 0; k < cBDctQuanFromLastFrame.GetLength(1); k++)
                            {
                                cBDctQuan[j, k] = cBDctQuanFromLastFrame[j, k] + cBDctQuanDiff[j, k];
                                cRDctQuan[j, k] = cRDctQuanFromLastFrame[j, k] + cRDctQuanDiff[j, k];
                            }
                        }
                    }
                }

                yDctQuanFromLastFrame = yDctQuan;
                cBDctQuanFromLastFrame = cBDctQuan;
                cRDctQuanFromLastFrame = cRDctQuan;

                // Tester.PrintToFile("yDctQuanAfter", yDctQuan);

                // revert dct and quantization
                DctImage dctImage = new DctImage(video.quality, video.subsamplingMode);
                int[,] YMatrix = dctImage.RevertDctAndQuantization(yDctQuan);
                int[,] CbMatrix = dctImage.RevertDctAndQuantization(cBDctQuan);
                int[,] CrMatrix = dctImage.RevertDctAndQuantization(cRDctQuan);

                if (subsamplingMode == "4:4:4")
                {
                    YMatrix = dctImage.TrimValueMatrix(YMatrix, video.width, video.height);
                    CbMatrix = dctImage.TrimValueMatrix(CbMatrix, video.width, video.height);
                    CrMatrix = dctImage.TrimValueMatrix(CrMatrix, video.width, video.height);
                }
                else if (subsamplingMode == "4:2:2")
                {
                    YMatrix = dctImage.TrimValueMatrix(YMatrix, video.width, video.height);
                    CbMatrix = dctImage.TrimValueMatrix(CbMatrix, video.width / 2, video.height);
                    CrMatrix = dctImage.TrimValueMatrix(CrMatrix, video.width / 2, video.height);
                }
                else
                {
                    YMatrix = dctImage.TrimValueMatrix(YMatrix, video.width, video.height);
                    CbMatrix = dctImage.TrimValueMatrix(CbMatrix, video.width / 2, video.height / 2);
                    CrMatrix = dctImage.TrimValueMatrix(CrMatrix, video.width / 2, video.height / 2);
                }

                // instantiate YCbCr images
                YCbCrImage tempImage = new YCbCrImage(YMatrix.GetLength(0), YMatrix.GetLength(1), subsamplingMode);
             
                for (int j = 0; j < YMatrix.GetLength(0); j++)
                { 
                        for (int k = 0; k < YMatrix.GetLength(1); k++)
                        {

                            if (subsamplingMode == "4:4:4")
                            {
                                tempImage.pixels[j, k] = new YCbCrPixel(YMatrix[j, k], CbMatrix[j, k], CrMatrix[j, k]);
                            }
                            else if (subsamplingMode == "4:2:2")
                            {
                                double Cb = CbMatrix[(j / 2), k];
                                double Cr = CrMatrix[(j / 2), k];
                                tempImage.pixels[j, k] = new YCbCrPixel(YMatrix[j, k], Cb, Cr);
                            }
                            else if (subsamplingMode == "4:2:0")
                            {
                                double Cb = CbMatrix[(j / 2), (k / 2)];
                                double Cr = CrMatrix[(j / 2), (k / 2)];
                                tempImage.pixels[j, k] = new YCbCrPixel(YMatrix[j, k], Cb, Cr);
                            }

                        }
                    
                } 

                tempImages[i] = tempImage;

                MethodInvoker mi = new MethodInvoker(() => {
                    int newValue = progressBar.Value + numOfThreads;
                    if (newValue <= outputImages.Length)
                    {
                        progressBar.Value = newValue;
                    }
                    else
                    {
                        progressBar.Value = outputImages.Length;
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

                YHuffmanCounts[i / keyFrameEvery] = YHuffman.GetCounts();
                CbHuffmanCounts[i / keyFrameEvery] = CbHuffman.GetCounts();
                CrHuffmanCounts[i / keyFrameEvery] = CrHuffman.GetCounts();
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

                YHuffmanCounts[i / keyFrameEvery] = YHuffman.GetCounts();
                CbHuffmanCounts[i / keyFrameEvery] = CbHuffman.GetCounts();
                CrHuffmanCounts[i / keyFrameEvery] = CrHuffman.GetCounts();
            }
        }

        private List<int> HuffmanEncoding(Huffman<int> huffman, int[] array)
        {
            return huffman.Encode(array);
        }

        private int[] HuffmanDecoding(List<int> list, Dictionary<int, int> counts)
        {
            Huffman<int> huffman = new Huffman<int>(counts);
            return huffman.Decode(list).ToArray();
        }

        #endregion

        // only allow one item to be checked in the chroma box
        public void chromaBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iSelectedIndex = chromaBox.SelectedIndex;
            if (iSelectedIndex == -1)
            {
                return;
            }
            for (int iIndex = 0; iIndex < chromaBox.Items.Count; iIndex++)
            {
                chromaBox.SetItemCheckState(iIndex, CheckState.Unchecked);
            }
            chromaBox.SetItemCheckState(iSelectedIndex, CheckState.Checked);
            if(iSelectedIndex == 0)
            {
                subsamplingMode = "4:4:4";
            } else if (iSelectedIndex == 1)
            {
                subsamplingMode = "4:2:2";
            } else if (iSelectedIndex == 2)
            {
                subsamplingMode = "4:2:0";
            }
        }

        private void getOptimizedResult(int[,] currentFrame, int[,] previousFrame, int[,] accumulatedChanges) {
            int maxDifferenceThisFrame = 10;
            int maxDifferenceKeyFrame = 30;
            
            for(int i = 0; i < currentFrame.GetLength(0); i += 8) {
                for(int j = 0; j < currentFrame.GetLength(1); j += 8) {
                    int currentDiff = currentFrame[i,j] - previousFrame[i,j];
                    accumulatedChanges[i,j] = (accumulatedChanges[i,j] == int.MaxValue) ? currentDiff : accumulatedChanges[i,j] + currentDiff;
                    if(Math.Abs(currentDiff) < maxDifferenceThisFrame && Math.Abs(accumulatedChanges[i,j]) < maxDifferenceKeyFrame) {
                        currentFrame[i, j] = previousFrame[i, j];
                    } else
                    {
                        currentFrame[i, j] = previousFrame[i, j] + accumulatedChanges[i, j];
                        accumulatedChanges[i, j] = int.MaxValue;
                    }
                }
            }
        }
    }
}
