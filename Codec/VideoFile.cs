using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codec
{
    [Serializable]
    class VideoFile
    {
        public int width;
        public int height;
        public BitArray[] YBitArray;
        public BitArray[] CbBitArray;
        public BitArray[] CrBitArray;

        public VideoFile(int width, int height,BitArray[] YBitArray, BitArray[] CbBitArray, BitArray[] CrBitArray)
        {
            this.width = width;
            this.height = height;
            this.YBitArray = YBitArray;
            this.CbBitArray = CbBitArray;
            this.CrBitArray = CrBitArray;
        }
    }
}
