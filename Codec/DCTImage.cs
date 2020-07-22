using System;

namespace Codec
{
    class DctImage
    {
        private YCbCrImage image;

        public string subsamplingMode = "4:4:4";
        
        //qualityFactor range from 1 to 100; 
        //50 = no change to base matrix used for quantization; 
        //1 = poorest quality; 100 = highest quality
        private int qualityFactor;
        
        private int[,] quantizationBaseMatrix = new int[8, 8] {
            { 16, 11, 10, 16, 24, 40, 51, 61 },
            { 12, 12, 14, 19, 26, 58, 60, 55 },
            { 14, 13, 16, 24, 40, 57, 69, 56 },
            { 14, 17, 22, 29, 51, 87, 80, 62 },
            { 18, 22, 37, 56, 68, 109, 103, 77 },
            { 24, 35, 55, 64, 81, 104, 113, 92 },
            { 49, 64, 78, 87, 103, 121, 120, 101 },
            { 72, 92, 95, 98, 112, 100, 103, 99 }
        };

        private int[,] quantizationMatrix = new int[8, 8];
        
        ////////////////////////////////////////////////////////////////////////////////
        /////Constructors
        ////////////////////////////////////////////////////////////////////////////////

        public DctImage(int qualityFactor, string subsamplingMode)
        {
            this.image = null;
            this.subsamplingMode = subsamplingMode;
            //set qualityFactor to 50 (=base quantization matrix) if given qualityFactor is not in defined range
            this.qualityFactor = (qualityFactor > 0) && (qualityFactor <= 100) ? qualityFactor : 50;
            CalculateQuantizationMatrix(this.qualityFactor);
        }

        public DctImage(YCbCrImage image)
        {
            this.image = image;
            subsamplingMode = image.subsamplingMode;
            //set qualityFactor to 50 (=base quantization matrix)
            this.qualityFactor = 50;
            CalculateQuantizationMatrix(this.qualityFactor);
        }

        public DctImage(YCbCrImage image, int qualityFactor)
        {
            this.image = image;
            subsamplingMode = image.subsamplingMode;
            //set qualityFactor to 50 (=base quantization matrix) if given qualityFactor is not in defined range
            this.qualityFactor = (qualityFactor > 0) && (qualityFactor <= 100) ? qualityFactor : 50;
            CalculateQuantizationMatrix(this.qualityFactor);
        }

        ////////////////////////////////////////////////////////////////////////////////
        /////Methods
        ////////////////////////////////////////////////////////////////////////////////

        //calculate the quantization matrix based on the qualityFactor
        //credit to user rayryeng on https://stackoverflow.com/questions/29215879/how-can-i-generalize-the-quantization-matrix-in-jpeg-compression
        private void CalculateQuantizationMatrix(int qualityFactor)
        {
            double s = (qualityFactor < 50) ? (5000 / qualityFactor) : (200 - 2 * qualityFactor);
            
            for (int i = 0; i < this.quantizationBaseMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < this.quantizationBaseMatrix.GetLength(1); j++)
                {
                    this.quantizationMatrix[i, j] = (int)Math.Floor((s * (double)this.quantizationBaseMatrix[i, j] + 50) / 100);
                    if (this.quantizationMatrix[i,j] == 0)
                    {
                        this.quantizationMatrix[i, j] = 1;
                    }
                }
            }
        }

        //create a matrix containing only Y, Cb or Cr values
        public double[,] FillValueMatrix(YCbCrImage image, String channelString)
        {
            double[,] valueMatrix = null;

            // only save every Nth pixel depending on subsampling mode / channel string
            if (subsamplingMode == "4:4:4" || channelString == "Y")
            {
                valueMatrix = new double[image.width, image.height];
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
            } else if (subsamplingMode == "4:2:2")
            {
                valueMatrix = new double[image.width / 2, image.height];
                for (int height = 0; height < image.height; height++)
                {
                    for (int width = 0; width < image.width; width += 2)
                    {
                        YCbCrPixel pixel = image.GetPixel(width, height);
                        switch (channelString)
                        {
                            case "Cb":
                                valueMatrix[width / 2, height] = pixel.getCb();
                                break;
                            case "Cr":
                                valueMatrix[width / 2, height] = pixel.getCr();
                                break;
                            default:
                                break;
                        }
                    }
                }
            } else if (subsamplingMode == "4:2:0")
            {
                valueMatrix = new double[image.width / 2, image.height / 2];
                for (int height = 0; height < image.height; height += 2)
                {
                    for (int width = 0; width < image.width; width += 2)
                    {
                        YCbCrPixel pixel = image.GetPixel(width, height);
                        switch (channelString)
                        {
                            case "Cb":
                                valueMatrix[width / 2, height / 2] = pixel.getCb();
                                break;
                            case "Cr":
                                valueMatrix[width / 2, height / 2] = pixel.getCr();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return valueMatrix;
        }


        private double[,] PadValueMatrix(double[,] valueMatrix)
        {
            //calculate the additional columns and rows the paddedValueMatrix needs to have a multiple of 8 columns and rows
            int paddingHeight = valueMatrix.GetLength(0) == 8 ? 0 : 8 - (valueMatrix.GetLength(0) % 8);
            int paddingWidth = valueMatrix.GetLength(1) == 8 ? 0 : 8 - (valueMatrix.GetLength(1) % 8);

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

        private int[,] PadValueMatrix(int[,] valueMatrix)
        {
            //calculate the additional columns and rows the paddedValueMatrix needs to have a multiple of 8 columns and rows
            int paddingHeight = valueMatrix.GetLength(0) == 8 ? 0 : 8 - (valueMatrix.GetLength(0) % 8);
            int paddingWidth = valueMatrix.GetLength(1) == 8 ? 0 : 8 - (valueMatrix.GetLength(1) % 8);


            //create the new matrix
            int[,] paddedValueMatrix = new int[valueMatrix.GetLength(0) + paddingHeight, valueMatrix.GetLength(1) + paddingWidth];

            for (int height = 0; height < paddedValueMatrix.GetLength(0); height++)
            {
                for (int width = 0; width < paddedValueMatrix.GetLength(1); width++)
                {
                    int value;
                    //fill paddedValueMatrix with all values of valueMatrix
                    if (width < valueMatrix.GetLength(1) && height < valueMatrix.GetLength(0))
                    {
                        value = valueMatrix[height, width];
                    }
                    else { value = 0; }
                    paddedValueMatrix[height, width] = value;
                }
            }
            return paddedValueMatrix;
        }

        public int[,] TrimValueMatrix(int[,] valueMatrix, int width, int height)
        {
            int[,] trimmedValueMatrix = new int[width, height];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    trimmedValueMatrix[j, i] = valueMatrix[j,i];
                }
            }
            return trimmedValueMatrix;
        }

        private int[,] DctSubArray(double[,] valueMatrix)
        {
            int[,] resultMatrix = new int[valueMatrix.GetLength(0), valueMatrix.GetLength(1)];
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
                    subArray = Quantization(subArray);
                    //fill corresponding block of resultMatrix with the values of subArray
                    for (int subArrayY = 0; subArrayY < 8; subArrayY++)
                    {
                        for (int subArrayX = 0; subArrayX < 8; subArrayX++)
                        {
                            resultMatrix[height + subArrayY, width + subArrayX] = (int)subArray[subArrayY, subArrayX];
                        }
                    }
                }
            }
            //return the fully transformed and quantized resultMatrix
            return resultMatrix;
        }

        // http://www.informatik.uni-hamburg.de/TKRN/world/students/ts/dct.html
        private double[,] Dct(double[,] subArray)
        {
            double[,] res = new double[8,8];
            for (int i = 0; i < 8; ++i)
                for (int j = 0; j < 8; ++j)
                {
                    double herg = 0;
                    for (int k = 0; k < 8; ++k)
                        for (int l = 0; l < 8; ++l)
                            herg = herg + subArray[k,l] * Math.Cos(i * Math.PI * ((2 * k) + 1) / 16) * Math.Cos(j * Math.PI * ((2 * l) + 1) / 16);
                    if ((i == 0) && (j != 0))
                        herg = herg * 1 / Math.Sqrt(2);
                    if ((j == 0) && (i != 0))
                        herg = herg * 1 / Math.Sqrt(2);
                    if ((j == 0) && (i == 0))
                        herg = 0.5 * herg;
                    res[i,j] = Math.Round(0.25 * herg);
                }

            return res;
        }

        // http://www.informatik.uni-hamburg.de/TKRN/world/students/ts/jsidct.html
        private double[,] InvertDct(double[,] subArray)
        {
            double[,] res = new double[8, 8];

            for (int i = 0; i < 8; ++i)
                for (int j = 0; j < 8; ++j)
                {
                    res[i,j] = 0;
                    for (int k = 0; k < 8; ++k)
                        for (int l = 0; l < 8; ++l)
                        {
                            double herg = 0.25 * subArray[k,l] * Math.Cos((((2 * i) + 1) * k * Math.PI) / 16) * Math.Cos((((2 * j) + 1) * l * Math.PI) / 16);
                            if ((k == 0) && (l == 0))
                                herg = 0.5 * herg;
                            if ((k != 0) && (l == 0))
                                herg = herg * 1 / Math.Sqrt(2);
                            if ((k == 0) && (l != 0))
                                herg = herg * 1 / Math.Sqrt(2);
                            res[i,j] = res[i,j] + herg;
                        }
                }

            return res;
        }

        private double[,] Quantization(double[,] subArray)
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

        private double[,] DeQuantization(double[,] subArray)
        {
            int width = subArray.GetLength(1);
            int height = subArray.GetLength(0);


            double[,] deQuantizedMatrix = new double[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    deQuantizedMatrix[i, j] = Math.Round(subArray[i, j] * (double)this.quantizationMatrix[i, j]);
                }
            }
            return deQuantizedMatrix;
        }

        public int[,] PerformDctAndQuantization(YCbCrImage image, String channelString)
        {
            double[,] valueMatrixY = FillValueMatrix(image, "Y");
            double[,] valueMatrixCb = FillValueMatrix(image, "Cb");
            double[,] valueMatrixCr = FillValueMatrix(image, "Cr");

            valueMatrixY = PadValueMatrix(valueMatrixY);
            valueMatrixCb = PadValueMatrix(valueMatrixCb);
            valueMatrixCr = PadValueMatrix(valueMatrixCr);

            switch (channelString)
            {
                case "Y":
                    return DctSubArray(valueMatrixY);
                case "Cb":
                    return DctSubArray(valueMatrixCb);
                case "Cr":
                    return DctSubArray(valueMatrixCr);
                default:
                    return null;
            }
        }

        public int[,] RevertDctAndQuantization(int[,] array)
        {
            array = PadValueMatrix(array);

            int[,] resultMatrix = new int[array.GetLength(0), array.GetLength(1)];

            //iterate the matrix in 8x8 blocks
            for (int width = 0; width <= array.GetLength(1) - 8; width += 8)
            {
                for (int height = 0; height <= array.GetLength(0) - 8; height += 8)
                {
                    //create a subArray for each block
                    double[,] subArray = new double[8, 8];
                    //fill the subArray with the values of the corresponding block in the valueMatrix
                    for (int subArrayY = 0; subArrayY < 8; subArrayY++)
                    {
                        for (int subArrayX = 0; subArrayX < 8; subArrayX++)
                        {
                            subArray[subArrayY, subArrayX] = array[height + subArrayY, width + subArrayX];
                        }
                    }
                    //perform Dct on subArray
                    subArray = DeQuantization(subArray);
                    subArray = InvertDct(subArray);
                    //fill corresponding block of resultMatrix with the values of subArray
                    for (int subArrayY = 0; subArrayY < 8; subArrayY++)
                    {
                        for (int subArrayX = 0; subArrayX < 8; subArrayX++)
                        {
                            resultMatrix[height + subArrayY, width + subArrayX] = (int)subArray[subArrayY, subArrayX];
                        }
                    }
                }
            }
            //return the fully transformed and quantized resultMatrix
            return resultMatrix;
        }
    }
}

