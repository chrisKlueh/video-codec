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
        public int keyFrameEvery;
        public int quality;
        public int width;
        public int height;
        public string subsamplingMode;
        public BitArray[] YBitArray;
        public BitArray[] CbBitArray;
        public BitArray[] CrBitArray;
        public Huffman<int>[] YHuffmans;
        public Huffman<int>[] CbHuffmans;
        public Huffman<int>[] CrHuffmans;

        public VideoFile(int keyFrameEvery, int quality, int width, int height, string subsamplingMode, BitArray[] YBitArray, BitArray[] CbBitArray, BitArray[] CrBitArray, Huffman<int>[] YHuffmans, Huffman<int>[] CbHuffmans, Huffman<int>[] CrHuffmans)
        {
            this.keyFrameEvery = keyFrameEvery;
            this.quality = quality;
            this.width = width;
            this.height = height;
            this.subsamplingMode = subsamplingMode;
            this.YBitArray = YBitArray;
            this.CbBitArray = CbBitArray;
            this.CrBitArray = CrBitArray;
            this.YHuffmans = YHuffmans;
            this.CbHuffmans = CbHuffmans;
            this.CrHuffmans = CrHuffmans;
        }
    }
}
