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
        public BitArray[] YBitArray;
        public BitArray[] CbBitArray;
        public BitArray[] CrBitArray;

        public VideoFile(BitArray[] YBitArray, BitArray[] CbBitArray, BitArray[] CrBitArray)
        {
            this.YBitArray = YBitArray;
            this.CbBitArray = CbBitArray;
            this.CrBitArray = CrBitArray;
        }
    }
}
