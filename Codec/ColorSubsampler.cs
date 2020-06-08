using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codec
{
    class ColorSubsampler
    {
        public static YCbCrImage[] GetSubSampledImages(Image[] inputImages, string subsamplingMode)
        {
            Image[] subSampledImages = new Image[inputImages.Length];
            for(int i = 0; i < subSampledImages.Length; i++)
            {

            }

            // TODO
            return null;
        }

        private static YCbCrImage GetSubSampledImage(Image image, string subsamplingMode)
        {
            switch(subsamplingMode)
            {
                case "4:4:4":
                    break;
                case "4:2:2":
                    for(int i = 0; i < bitmap.Width; i++)
                    {
                        for (int j = 0; j < bitmap.Height; j++)
                        {
                            Color col = bitmap.
                        }
                    }
                    break;
                case "4:2:0":
                    break;
            }

            return null;
        }
    }
}
