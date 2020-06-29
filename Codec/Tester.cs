using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codec
{
    class Tester
    {
        public static void PrintToFile(string filename, string line)
        {
            System.IO.File.WriteAllText(filename + ".txt", line);
        }

        public static void PrintToFile(string filename, int[,] ints)
        {
            string str = "";
            for (int i = 0; i < ints.GetLength(0); i++)
            {
                for (int j = 0; j < ints.GetLength(1); j++)
                {
                    str += ints[i, j];
                }
            }
            PrintToFile(filename, str);
        }

        public static void PrintToFile(string filename, int[] ints)
        {
            string str = "";
            for (int i = 0; i < ints.Length; i++)
            {
                    str += ints[i] + ", ";
            }
            PrintToFile(filename, str);
        }

        public static void PrintToFile(string filename, List<int>[] ints)
        {
            string str = "";
            for (int i = 0; i < ints[0].Count; i++)
            {
                str += ints[0][i] + ", ";
            }
            PrintToFile(filename, str);
        }
    }
}
