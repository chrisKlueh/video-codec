using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Codec
{
    // https://rosettacode.org/wiki/Huffman_coding#C.23
    public class Huffman<T> where T : IComparable
    {
        private readonly Dictionary<int, HuffmanNode<int>> _leafDictionary = new Dictionary<int, HuffmanNode<int>>();
        private readonly HuffmanNode<int> _root;

        Dictionary<int, int> counts = new Dictionary<int, int>();
        int valueCount = 0;

        public Huffman(IEnumerable<int> values)
        {
            var priorityQueue = new PriorityQueue<HuffmanNode<int>>();

            foreach (int value in values)
            {
                if (!counts.ContainsKey(value))
                {
                    counts[value] = 0;
                }
                counts[value]++;
            }

            valueCount = counts.Count;

            foreach (int value in counts.Keys)
            {
                var node = new HuffmanNode<int>((double)counts[value] / valueCount, value);
                priorityQueue.Add(node);
                _leafDictionary[value] = node;
            }

            while (priorityQueue.Count > 1)
            {
                HuffmanNode<int> leftSon = priorityQueue.Pop();
                HuffmanNode<int> rightSon = priorityQueue.Pop();
                var parent = new HuffmanNode<int>(leftSon, rightSon);
                priorityQueue.Add(parent);
            }

            _root = priorityQueue.Pop();
            _root.IsZero = false;
        }

        public Huffman(Dictionary<int, int> counts)
        {
            var priorityQueue = new PriorityQueue<HuffmanNode<int>>();
            valueCount = counts.Count;

            foreach (int value in counts.Keys)
            {
                var node = new HuffmanNode<int>((double)counts[value] / valueCount, value);
                priorityQueue.Add(node);
                _leafDictionary[value] = node;
            }

            while (priorityQueue.Count > 1)
            {
                HuffmanNode<int> leftSon = priorityQueue.Pop();
                HuffmanNode<int> rightSon = priorityQueue.Pop();
                var parent = new HuffmanNode<int>(leftSon, rightSon);
                priorityQueue.Add(parent);
            }

            _root = priorityQueue.Pop();
            _root.IsZero = false;
        }

        public Dictionary<int, int> GetCounts()
        {
            return counts;
        }

        public List<int> Encode(int value)
        {
            var returnValue = new List<int>();
            Encode(value, returnValue);
            return returnValue;
        }

        public void Encode(int value, List<int> encoding)
        {
            if (!_leafDictionary.ContainsKey(value))
            {
                throw new ArgumentException("Invalid value in Encode");
            }
            HuffmanNode<int> nodeCur = _leafDictionary[value];
            var reverseEncoding = new List<int>();
            while (!nodeCur.IsRoot)
            {
                reverseEncoding.Add(nodeCur.Bit);
                nodeCur = nodeCur.Parent;
            }

            reverseEncoding.Reverse();
            encoding.AddRange(reverseEncoding);
        }

        public List<int> Encode(IEnumerable<int> values)
        {
            var returnValue = new List<int>();

            foreach (int value in values)
            {
                Encode(value, returnValue);
            }
            return returnValue;
        }

        public int Decode(List<int> bitString, ref int position)
        {
            HuffmanNode<int> nodeCur = _root;
            while (!nodeCur.IsLeaf)
            {
                if (position > bitString.Count)
                {
                    throw new ArgumentException("Invalid bitstring in Decode");
                }
                nodeCur = bitString[position++] == 0 ? nodeCur.LeftSon : nodeCur.RightSon;
            }
            return nodeCur.Value;
        }

        public List<int> Decode(List<int> bitString)
        {
            int position = 0;
            var returnValue = new List<int>();

            while (position != bitString.Count)
            {
                returnValue.Add(Decode(bitString, ref position));
            }
            return returnValue;
        }
    }

    internal class HuffmanNode<T> : IComparable
    {
        internal HuffmanNode(double probability, T value)
        {
            Probability = probability;
            LeftSon = RightSon = Parent = null;
            Value = value;
            IsLeaf = true;
        }

        internal HuffmanNode(HuffmanNode<T> leftSon, HuffmanNode<T> rightSon)
        {
            LeftSon = leftSon;
            RightSon = rightSon;
            Probability = leftSon.Probability + rightSon.Probability;
            leftSon.IsZero = true;
            rightSon.IsZero = false;
            leftSon.Parent = rightSon.Parent = this;
            IsLeaf = false;
        }

        internal HuffmanNode<T> LeftSon { get; set; }
        internal HuffmanNode<T> RightSon { get; set; }
        internal HuffmanNode<T> Parent { get; set; }
        internal T Value { get; set; }
        internal bool IsLeaf { get; set; }

        internal bool IsZero { get; set; }

        internal int Bit
        {
            get { return IsZero ? 0 : 1; }
        }

        internal bool IsRoot
        {
            get { return Parent == null; }
        }

        internal double Probability { get; set; }

        public int CompareTo(object obj)
        {
            return -Probability.CompareTo(((HuffmanNode<T>)obj).Probability);
        }
    }

    internal class PriorityQueue<T> where T : IComparable
    {
        protected List<T> LstHeap = new List<T>();

        public virtual int Count
        {
            get { return LstHeap.Count; }
        }

        public virtual void Add(T val)
        {
            LstHeap.Add(val);
            SetAt(LstHeap.Count - 1, val);
            UpHeap(LstHeap.Count - 1);
        }

        public virtual T Peek()
        {
            if (LstHeap.Count == 0)
            {
                throw new IndexOutOfRangeException("Peeking at an empty priority queue");
            }

            return LstHeap[0];
        }

        public virtual T Pop()
        {
            if (LstHeap.Count == 0)
            {
                throw new IndexOutOfRangeException("Popping an empty priority queue");
            }

            T valRet = LstHeap[0];

            SetAt(0, LstHeap[LstHeap.Count - 1]);
            LstHeap.RemoveAt(LstHeap.Count - 1);
            DownHeap(0);
            return valRet;
        }

        protected virtual void SetAt(int i, T val)
        {
            LstHeap[i] = val;
        }

        protected bool RightSonExists(int i)
        {
            return RightChildIndex(i) < LstHeap.Count;
        }

        protected bool LeftSonExists(int i)
        {
            return LeftChildIndex(i) < LstHeap.Count;
        }

        protected int ParentIndex(int i)
        {
            return (i - 1) / 2;
        }

        protected int LeftChildIndex(int i)
        {
            return 2 * i + 1;
        }

        protected int RightChildIndex(int i)
        {
            return 2 * (i + 1);
        }

        protected T ArrayVal(int i)
        {
            return LstHeap[i];
        }

        protected T Parent(int i)
        {
            return LstHeap[ParentIndex(i)];
        }

        protected T Left(int i)
        {
            return LstHeap[LeftChildIndex(i)];
        }

        protected T Right(int i)
        {
            return LstHeap[RightChildIndex(i)];
        }

        protected void Swap(int i, int j)
        {
            T valHold = ArrayVal(i);
            SetAt(i, LstHeap[j]);
            SetAt(j, valHold);
        }

        protected void UpHeap(int i)
        {
            while (i > 0 && ArrayVal(i).CompareTo(Parent(i)) > 0)
            {
                Swap(i, ParentIndex(i));
                i = ParentIndex(i);
            }
        }

        protected void DownHeap(int i)
        {
            while (i >= 0)
            {
                int iContinue = -1;

                if (RightSonExists(i) && Right(i).CompareTo(ArrayVal(i)) > 0)
                {
                    iContinue = Left(i).CompareTo(Right(i)) < 0 ? RightChildIndex(i) : LeftChildIndex(i);
                }
                else if (LeftSonExists(i) && Left(i).CompareTo(ArrayVal(i)) > 0)
                {
                    iContinue = LeftChildIndex(i);
                }

                if (iContinue >= 0 && iContinue < LstHeap.Count)
                {
                    Swap(i, iContinue);
                }

                i = iContinue;
            }
        }
    }
}
