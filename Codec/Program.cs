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
            //Application.Run(new Form1());

            int[,] test = new int[,] {
              { 1, 3, 5, 5, 9, 7, 7, 7}, { 1, 3, 5, 5, 9, 7, 7, 7}, { 10, 3, 5, 5, 9, 7, 7, 7}, { 10, 3, 5, 5, 9, 7, 7, 7},
              { 20, 3, 5, 5, 9, 7, 7, 7}, { 20, 3, 5, 5, 9, 7, 7, 7}, { 20, 3, 5, 5, 9, 7, 7, 7}, { 20, 3, 5, 5, 9, 7, 7, 7}
           };

            int[,] dE = DifferentialEncoding.Encode(test, 4);
      
            int[] rLE = RunLengthEncode.Encode(dE, 8);

            int[,] rLED = RunLengthEncode.Decode(rLE, 8);
            int a = 0;
            

        }
    }
}
