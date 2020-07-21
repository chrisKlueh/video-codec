using System.Drawing;
using System.Reflection;

namespace Codec
{
    internal class YCbCrPixel
    {
        private double Y;
        private double Cb;
        private double Cr;

        public YCbCrPixel(double Y)
        {
            this.Y = Y;
        }
        public YCbCrPixel(double Y, double Cb, double Cr)
        {
            this.Y = Y;
            this.Cb = Cb;
            this.Cr = Cr;
        }

        public double getY()
        {
            return Y;
        }

        public double getCb()
        {
            return Cb;
        }

        public double getCr()
        {
            return Cr;
        }

        public void setY(double Y)
        {

            this.Y = Y;
        }

        public void setCb(double Cb)
        {
            this.Cb = Cb;
        }

        public void setCr(double Cr)
        {
            this.Cr = Cr;
        }
    }

    class YCbCrImage
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public YCbCrPixel[,] pixels;
        public string subsamplingMode = "4:4:4";

        public YCbCrImage(int width, int height, string subsamplingMode)
        {
            this.subsamplingMode = subsamplingMode;
            this.width = width;
            this.height = height;
            pixels = new YCbCrPixel[width, height];
        }

        public YCbCrPixel GetPixel(int x, int y)
        {
            if (subsamplingMode == "4:2:2")
            {
                if (x % 2 != 0)
                {
                    double Cb;
                    double Cr;
                    if (x == width - 1)
                    {
                        Cb = pixels[x - 1, y].getCb();
                        Cr = pixels[x - 1, y].getCr();
                    } else
                    {
                        Cb = (pixels[x - 1, y].getCb() + pixels[x + 1, y].getCb()) / 2;
                        Cr = (pixels[x - 1, y].getCr() + pixels[x + 1, y].getCr()) / 2;
                    }
                    return new YCbCrPixel(pixels[x, y].getY(), Cb, Cr);
                }
            }
            else if (subsamplingMode == "4:2:0")
            {
                double Cb;
                double Cr;
                if (x % 2 != 0 && y % 2 == 0)
                {
                    if (x == width - 1)
                    {
                        Cb = pixels[x - 1, y].getCb();
                        Cr = pixels[x - 1, y].getCr();
                    } else
                    {
                        Cb = (pixels[x - 1, y].getCb() + pixels[x + 1, y].getCb()) / 2;
                        Cr = (pixels[x - 1, y].getCr() + pixels[x + 1, y].getCr()) / 2;
                    }
                    return new YCbCrPixel(pixels[x, y].getY(), Cb, Cr);
                } else if (x % 2 == 0 && y % 2 != 0)
                {
                    if (y == height - 1)
                    {
                        Cb = pixels[x, y - 1].getCb();
                        Cr = pixels[x, y - 1].getCr();
                    } else
                    {
                        Cb = (pixels[x, y - 1].getCb() + pixels[x, y + 1].getCb()) / 2;
                        Cr = (pixels[x, y - 1].getCr() + pixels[x, y + 1].getCr()) / 2;
                    }
                    return new YCbCrPixel(pixels[x, y].getY(), Cb, Cr);
                }
                else if (x % 2 != 0 && y % 2 != 0)
                {
                    if (x == width - 1 && y == height - 1)
                    {
                        Cb = pixels[x - 1, y - 1].getCb();
                        Cr = pixels[x - 1, y - 1].getCr();
                    } else if (x == width - 1 && y != height - 1) {
                        Cb = (pixels[x - 1, y - 1].getCb() + pixels[x - 1, y + 1].getCb()) / 2;
                        Cr = (pixels[x - 1, y - 1].getCr() + pixels[x - 1, y + 1].getCr()) / 2;
                    } else if (x != width - 1 && y == height - 1)
                    {
                        Cb = (pixels[x - 1, y - 1].getCb() + pixels[x + 1, y - 1].getCb()) / 2;
                        Cr = (pixels[x - 1, y - 1].getCr() + pixels[x + 1, y - 1].getCr()) / 2;
                    } else
                    {
                        Cb = (pixels[x - 1, y - 1].getCb() + pixels[x + 1, y + 1].getCb()) / 2;
                        Cr = (pixels[x - 1, y - 1].getCr() + pixels[x + 1, y + 1].getCr()) / 2;
                    }
                    return new YCbCrPixel(pixels[x, y].getY(), Cb, Cr);
                }
            }
            return pixels[x, y];
        }

        public void SetSubsamplingMode()
        {
            if(subsamplingMode == "4:2:2")
            {
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        // remove chroma value of every odd numbered horizonal pixel
                        if(x % 2 != 0)
                        {
                            pixels[x, y] = new YCbCrPixel(pixels[x, y].getY());
                        }
                    }
                }
            } else if (subsamplingMode == "4:2:0")
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        // remove chroma value of every odd numbered horizonal and vertical pixel
                        if (x % 2 != 0 || y % 2 != 0)
                        {
                            pixels[x, y] = new YCbCrPixel(pixels[x, y].getY());
                        }
                    }
                }
            }
        }
    }
}
