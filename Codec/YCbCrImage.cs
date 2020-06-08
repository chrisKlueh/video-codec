using System.Drawing;

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
        private int width;
        private int height;
        public YCbCrPixel[,] pixels;

        public YCbCrImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            pixels = new YCbCrPixel[width, height];
        }
    }
}
