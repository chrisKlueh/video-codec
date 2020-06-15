using System;
using System.Security.Cryptography;

namespace ConsoleApp1
{
    class DctImage
    {
        //public int width { get; private set; }
        //public int height { get; private set; }
        public YCbCrImage image;
        public static int m = 8, n = 8;
        private int[,] quantizationMatrix = new int[8, 8] {
            { 16, 11, 10, 16, 24, 40, 51, 61 },
            { 12, 12, 14, 19, 26, 58, 60, 55 },
            { 14, 13, 16, 24, 40, 57, 69, 56 },
            { 14, 17, 22, 29, 51, 87, 80, 62 },
            { 18, 22, 37, 56, 68, 109, 103, 77 },
            { 24, 35, 55, 64, 81, 104, 113, 92 },
            { 49, 64, 78, 87, 103, 121, 120, 101 },
            { 72, 92, 95, 98, 112, 100, 103, 99 }};

        public DctImage(YCbCrImage image)
        {
            //this.width = image.height.Get();
            //this.height = image.height.get;
            this.image = image;
        }

        //create a matrix containing only Y, Cb or Cr values
        public double[,] FillValueMatrix(YCbCrImage image, String channelString)
        {
            double[,] valueMatrix = new double[image.width, image.height];
            for (int height = 0; height < image.height; height++)
            {
                for (int width = 0; width < image.width; width++)
                {
                    YCbCrPixel pixel = image.GetPixel(width, height);


                    switch (channelString)
                    {
                        case "Y":
                            valueMatrix[width, height] = pixel.getY();
                            break;
                        case "Cb":
                            valueMatrix[width, height] = pixel.getCb();
                            break;
                        case "Cr":
                            valueMatrix[width, height] = pixel.getCr();
                            break;
                        default:
                            break;
                    }

                }
            }
            return valueMatrix;
        }


        public double[,] PadValueMatrix(double[,] valueMatrix)
        {
            //calculate the additional columns and rows the paddedValueMatrix needs to have a multiple of 8 columns and rows
            int paddingHeight = 8 - (valueMatrix.GetLength(0) % 8);
            int paddingWidth = 8 - (valueMatrix.GetLength(1) % 8);

            //create the new matrix
            double[,] paddedValueMatrix = new double[valueMatrix.GetLength(0) + paddingHeight, valueMatrix.GetLength(1) + paddingWidth];

            for (int height = 0; height < paddedValueMatrix.GetLength(0); height++)
            {
                for (int width = 0; width < paddedValueMatrix.GetLength(1); width++)
                {
                    double value;
                    //fill paddedValueMatrix with all values of valueMatrix
                    if (width < valueMatrix.GetLength(1) && height < valueMatrix.GetLength(0))
                    {
                        value = valueMatrix[height, width];
                    }
                    else { value = 0.0; }
                    paddedValueMatrix[height, width] = value;
                }
            }
            return paddedValueMatrix;
        }


        public YCbCrImage PerformDct(YCbCrImage image)
        {
            YCbCrImage result = new YCbCrImage(image.width, image.height);

            double[,] valueMatrixY = FillValueMatrix(image, "Y");
            double[,] valueMatrixCb = FillValueMatrix(image, "Cb");
            double[,] valueMatrixCr = FillValueMatrix(image, "Cr");

            valueMatrixY = PadValueMatrix(valueMatrixY);
            valueMatrixCb = PadValueMatrix(valueMatrixCb);
            valueMatrixCr = PadValueMatrix(valueMatrixCr);

            valueMatrixY = DctSubArray(valueMatrixY);
            valueMatrixCb = DctSubArray(valueMatrixCb);
            valueMatrixCr = DctSubArray(valueMatrixCr);

            for (int width = 0; width < image.width; width++)

            {
                for (int height = 0; height < image.height; height++)
                {
                    double Y = valueMatrixY[height, width];
                    double Cb = valueMatrixCb[height, width];
                    double Cr = valueMatrixCr[height, width];
                    result.pixels[height, width] = new YCbCrPixel(Y, Cb, Cr);
                }
            }

            return result;
        }

        public double[,] DctSubArray(double[,] valueMatrix)
        {
            double[,] resultMatrix = new double[valueMatrix.GetLength(0), valueMatrix.GetLength(1)];
            //iterate the valueMatrix in 8x8 blocks
            for (int width = 0; width <= valueMatrix.GetLength(1) - 8; width += 8)
            {
                for (int height = 0; height <= valueMatrix.GetLength(0) - 8; height += 8)
                {
                    //create a subArray for each block
                    double[,] subArray = new double[8, 8];
                    //fill the subArray with the values of the corresponding block in the valueMatrix
                    for (int subArrayY = 0; subArrayY < 8; subArrayY++)
                    {
                        for (int subArrayX = 0; subArrayX < 8; subArrayX++)
                        {
                            subArray[subArrayY, subArrayX] = valueMatrix[height + subArrayY, width + subArrayX];
                        }
                    }
                    //perform Dct on subArray
                    subArray = Dct(subArray);
                    //subArray = Quantization(subArray);

                    //fill corresponding block of resultMatrix with the values of subArray
                    for (int subArrayY = 0; subArrayY < 8; subArrayY++)
                    {
                        for (int subArrayX = 0; subArrayX < 8; subArrayX++)
                        {
                            resultMatrix[height + subArrayY, width + subArrayX] = subArray[subArrayY, subArrayX];
                        }
                    }
                }
            }
            //return the fully transformed resultMatrix
            return resultMatrix;
        }

        /*
        //mock Dct algorithm for testing purposes
        public double[,] Dct(double[,] subArray)
        {
            double[,] resultArray = new double[subArray.GetLength(0), subArray.GetLength(1)];
            for (int subArrayY = 0; subArrayY < subArray.GetLength(0); subArrayY++)
            {
                for (int subArrayX = 0; subArrayX < subArray.GetLength(1); subArrayX++)
                {
                    resultArray[subArrayX, subArrayY] = subArray[subArrayX, subArrayY] + 1;
                }
            }
            return resultArray;
        }
        */


        // https://www.geeksforgeeks.org/discrete-cosine-transform-algorithm-program/
        // Function to find discrete cosine transform
        public double[,] Dct(double[,] subArray)
        {
            int i, j, k, l;
            int width = subArray.GetLength(1);
            int height = subArray.GetLength(0);

            // dct will store the discrete cosine transform 
            double[,] dct = new double[height, width];

            double ci, cj, dct1, sum;

            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    // ci and cj depends on frequency as well as 
                    // number of row and columns of specified matrix 
                    if (i == 0)
                        ci = 1 / Math.Sqrt(height);
                    else
                        ci = Math.Sqrt(2) / Math.Sqrt(height);

                    if (j == 0)
                        cj = 1 / Math.Sqrt(width);
                    else
                        cj = Math.Sqrt(2) / Math.Sqrt(width);

                    // sum will temporarily store the sum of  
                    // cosine signals 
                    sum = 0;
                    for (k = 0; k < height; k++)
                    {
                        for (l = 0; l < n; l++)
                        {
                            dct1 = subArray[k, l] * Math.Cos((2 * k + 1) * i * Math.PI / (2 * height)) * Math.Cos((2 * l + 1) * j * Math.PI / (2 * width));
                            sum = sum + dct1;
                        }
                    }
                    dct[i, j] = ci * cj * sum;
                }
            }
            return dct;
        }

        public double[,] Quantization(double[,] subArray)
        {
            int width = subArray.GetLength(1);
            int height = subArray.GetLength(0);


            double[,] quantizedMatrix = new double[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    quantizedMatrix[i, j] = Math.Round(subArray[i, j] / (double)this.quantizationMatrix[i, j]);
                }
            }
            return quantizedMatrix;
        }
    }
}

