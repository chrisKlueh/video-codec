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
        public int YLength;
        public int CbLength;
        public int CrLength;
        public BitArray[] YBitArray;
        public BitArray[] CbBitArray;
        public BitArray[] CrBitArray;

        public VideoFile(BitArray[] YBitArray, BitArray[] CbBitArray, BitArray[] CrBitArray)
        {
            this.YBitArray = YBitArray;
            this.CbBitArray = CbBitArray;
            this.CrBitArray = CrBitArray;
            this.YLength = YBitArray.Length;
            this.CbLength = CbBitArray.Length;
            this.CrLength = CrBitArray.Length;
        }
    }
}
