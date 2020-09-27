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
        public Dictionary<int, int>[] YHuffmanCounts;
        public Dictionary<int, int>[] CbHuffmanCounts;
        public Dictionary<int, int>[] CrHuffmanCounts;

        public VideoFile(int keyFrameEvery, int quality, int width, int height, string subsamplingMode, BitArray[] YBitArray, BitArray[] CbBitArray, BitArray[] CrBitArray, Dictionary<int, int>[] YHuffmanCounts, Dictionary<int, int>[] CbHuffmanCounts, Dictionary<int, int>[] CrHuffmanCounts)
        {
            this.keyFrameEvery = keyFrameEvery;
            this.quality = quality;
            this.width = width;
            this.height = height;
            this.subsamplingMode = subsamplingMode;
            this.YBitArray = YBitArray;
            this.CbBitArray = CbBitArray;
            this.CrBitArray = CrBitArray;
            this.YHuffmanCounts = YHuffmanCounts;
            this.CbHuffmanCounts = CbHuffmanCounts;
            this.CrHuffmanCounts = CrHuffmanCounts;
        }
    }
}
