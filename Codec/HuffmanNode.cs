using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codec
{
    // https://seedotnet.wordpress.com/2011/04/16/huffman-coding/
    class HuffmanNode
    {
        public int frequency;
        public string data;
        public HuffmanNode leftChild, rightChild;

        public HuffmanNode(string data, int frequency)
        {
            this.data = data;
            this.frequency = frequency;
        }

        public HuffmanNode(HuffmanNode leftChild, HuffmanNode rightChild)
        {
            this.leftChild = leftChild;
            this.rightChild = rightChild;

            this.data = leftChild.data + ":" + rightChild.data;
            this.frequency = leftChild.frequency + rightChild.frequency;
        }

        #region Helper Methods

        public static Stack<HuffmanNode> GetSortedStack(IList<HuffmanNode> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[i].frequency > list[j].frequency)
                    {
                        HuffmanNode tempNode = list[j];
                        list[j] = list[i];
                        list[i] = tempNode;
                    }
                }
            }

            Stack<HuffmanNode> stack = new Stack<HuffmanNode>();

            for (int j = 0; j < list.Count; j++)
                stack.Push(list[j]);

            return stack;
        }

        public static void GenerateCode(HuffmanNode parentNode, out string huffmanData, string code = "")
        {
            if (parentNode != null)
            {
                GenerateCode(parentNode.leftChild, out huffmanData, code + "0");

                if (parentNode.leftChild == null && parentNode.rightChild == null)
                {
                    huffmanData = code;
                } else
                {
                    huffmanData = "";
                }

                GenerateCode(parentNode.rightChild, out huffmanData, code + "1");
            } else
            {
                huffmanData = code;
            }
        }

        // TODO: make it work
        public static void DecodeData(HuffmanNode parentNode, HuffmanNode currentNode, int pointer, string input)
        {
            if (input.Length == pointer)
            {
                if (currentNode.leftChild == null && currentNode.rightChild == null)
                {
                    Console.WriteLine(currentNode.data);
                }

                return;
            }
            else
            {
                if (currentNode.leftChild == null && currentNode.rightChild == null)
                {
                    Console.WriteLine(currentNode.data);
                    DecodeData(parentNode, parentNode, pointer, input);
                }
                else
                {
                    if (input.Substring(pointer, 1) == "0")
                    {
                        DecodeData(parentNode, currentNode.leftChild, ++pointer, input);
                    }
                    else
                    {
                        DecodeData(parentNode, currentNode.rightChild, ++pointer, input);
                    }
                }
            }
        }

        #endregion
    }
}
