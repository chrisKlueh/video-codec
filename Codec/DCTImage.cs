using System;
using System.Security.Cryptography;

namespace Codec
{
    class DctImage
    {
        //public int width { get; private set; }
        //public int height { get; private set; }
        public YCbCrImage image;
        public static int m = 8, n = 8;

        public DctImage(YCbCrImage image)
        {
            //this.width = image.width.get;
            //this.height = image.height.get;
            this.image = image;
        }

        //create a matrix containing only Y, Cb or Cr values
        private double[,] FillValueMatrix(YCbCrImage image, String channelString)
        {
            double[,] valueMatrix = new double[image.width][image.height];
            for (int height = 0; height < image.height; height++)
            {
                for (int width = 0; width < image.width; width++)
                {
                    switch (channelString)
                    {
                        case "Y":
                            valueMatrix[width][height] = image.GetPixel(width, height).getY();
                            break;
                        case "Cb":
                            valueMatrix[width][height] = image.GetPixel(width, height).getCb();
                            break;
                        case "Cr":
                            valueMatrix[width][height] = image.GetPixel(width, height).getCr();
                            break;
                        default:
                            break;
                    }
                }
            }
            return valueMatrix;
        }


        private double[,] PadValueMatrix(double[,] valueMatrix)
        {
            //calculate the additional columns and rows the paddedValueMatrix needs to have a multiple of 8 columns and rows
            paddingWidth = 8 - (valueMatrix.GetLength(0) % 8);
            paddingHeight = 8 - (valueMatrix.GetLength(1) % 8);

            //create the new matrix
            double[,] paddedValueMatrix = new double[valueMatrix.GetLength(0) + paddingWidth][valueMatrix.GetLength(1) + paddingHeight];

            for (int height = 0; height < paddedValueMatrix.height; height++)
            {
                for (int width = 0; width < paddedValueMatrix.width; width++)
                {
                    double value;
                    //fill paddedValueMatrix with all values of valueMatrix
                    if (width < valueMatrix.GetLength(1) && height < valueMatrix.GetLength(1))
                    {
                        value = valueMatrix[width][height];
                    }
                    //fill the rest of paddedValueMatrix with 0.0 values
                    else
                    {
                        value = 0.0;
                    }
                    paddedValueMatrix[width][height] = value;
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

            for (int height = 0; height < image.height; height++)
            {
                for (int width = 0; width < image.width; width++)
                {
                    result.getPixel(width, height).setY(valueMatrixY[width][height]);
                    result.getPixel(width, height).setCb(valueMatrixCb[width][height]);
                    result.getPixel(width, height).setCr(valueMatrixCr[width][height]);
                }
            }
            
            return result;
        }

        public double[,] DctSubArray(double[,] valueMatrix)
        {
            double[,] resultMatrix = new double[valueMatrix.GetLength(0)][valueMatrix.GetLength(1)];
            //iterate the valueMatrix in 8x8 blocks
            for (int height = 0; height < valueMatrix.GetLength(1) - 8; height += 8)
            {
                for (int width = 0; width < valueMatrix.GetLength(0) - 8; width += 8)
                {
                    //create a subArray for each block
                    double[,] subArray = new double[8][8];
                    //fill the subArray with the values of the corresponding block in the valueMatrix
                    for (int subArrayY = 0; subArrayY < 8; subArrayY++)
                    {
                        for (int subArrayX = 0; subArrayX < 8; subArrayX++)
                        {
                            subArray[subArrayX][subArrayY] = valueMatrix[width + subArrayX][height + subArrayY];
                        }
                    }
                    //perform Dct on subArray
                    subArray = Dct(subArray);
                    //fill corresponding block of resultMatrix with the values of subArray
                    for (int subArrayY = 0; subArrayY < 8; subArrayY++)
                    {
                        for (int subArrayX = 0; subArrayX < 8; subArrayX++)
                        {
                            resultMatrix[width + subArrayX][height + subArrayY] = subArray[subArrayX][subArrayY];
                        }
                    }
                }
            }
            //return the fully transformed resultMatrix
            return resultMatrix;
        }


        //mock Dct algorithm for testing purposes
        public double[,] Dct(double[,] subArray)
        {
            double[,] resultArray = new double[8][8];
            for (int subArrayY = 0; subArrayY < 8; subArrayY++)
            {
                for (int subArrayX = 0; subArrayX < 8; subArrayX++)
                {
                    resultArray[subArrayX][subArrayY] = resultArray[subArrayX][subArrayY] + 1;
                }
            }
            return resultArray;
        }

            /*
            // https://www.geeksforgeeks.org/discrete-cosine-transform-algorithm-program/
            // Function to find discrete cosine transform
            public double[,] Dct(double[,] subArray)
            {
                int i, j, k, l;

                // dct will store the discrete cosine transform 
                double[][] dct = new double[this.m][this.n];

                double ci, cj, dct1, sum;

                for (i = 0; i < this.m; i++)
                {
                    for (j = 0; j < this.n; j++)
                    {
                        // ci and cj depends on frequency as well as 
                        // number of row and columns of specified matrix 
                        if (i == 0)
                            ci = 1 / Math.sqrt(this.m);
                        else
                            ci = Math.sqrt(2) / Math.sqrt(this.m);

                        if (j == 0)
                            cj = 1 / Math.sqrt(this.n);
                        else
                            cj = Math.sqrt(2) / Math.sqrt(this.n);

                        // sum will temporarily store the sum of  
                        // cosine signals 
                        sum = 0;
                        for (k = 0; k < this.m; k++)
                        {
                            for (l = 0; l < n; l++)
                            {
                                dct1 = subArray[k][l] * Math.cos((2 * k + 1) * i * Math.PI / (2 * this.m)) * Math.cos((2 * l + 1) * j * Math.PI / (2 * this.n));
                                sum = sum + dct1;
                            }
                        }
                        dct[i][j] = ci * cj * sum;
                    }
                }

                //Print result in console
                //ONLY FOR TESTING
                /*for (i = 0; i < this.m; i++)
                {
                    for (j = 0; j < this.n; j++)
                    {
                        Console.printf("%f\t", dct[i][j]);
                    }
                    Console.println();
                }
                */
            /*
                return dct;

            }
            */

        }

    static void Main(string[] args)
    {
        // Display the number of command line arguments.
        Console.WriteLine(args.Length);
    }
}

