using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Codec
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           // Application.Run(new Form1());
            int[][] test =
            {
                new int[] { 1, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 1, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 1, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 1, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 1, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 1, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 1, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 1, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 10, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 10, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 10, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 10, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 10, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 10, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 10, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 10, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 20, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 20, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 20, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 20, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 20, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 20, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 20, 3, 5, 5, 9, 7, 7, 7},
                new int[] { 20, 3, 5, 5, 9, 7, 7, 7},

            };
            int[][] dE = DifferentialEncoding.Run(test, 8);
            
            int[][] dD = DifferentialDecoding.Run(dE, 8);
            int[] result = RunLengthEncode.Run(dE, 8);

        }
    }
}
