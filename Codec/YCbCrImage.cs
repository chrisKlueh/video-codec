using System.Drawing;
using System.Reflection;

namespace Codec
{
    internal class YCbCrPixel
    {
        private double Y;
        private double Cb;
        private double Cr;

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
            return pixels[x, y];
        }
    }
}
