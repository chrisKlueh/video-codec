using System;

namespace Codec
{
    class DctImage
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public YCbCrImage image;
        public static int m = 8, n = 8;

        public DctImage (YCbCrImage image)
        {
            this.width = image.width.get;
            this.height = image.height.get;
            this.image = image;
        }

        // https://www.geeksforgeeks.org/discrete-cosine-transform-algorithm-program/
        // Function to find discrete cosine transform
        public int[][] PerformDct()
        {
            //1.
            
            int i, j, k, l;
         
            // dct will store the discrete cosine transform 
            double[][] dct = new double[this.m][this.n];

            double ci, cj, dct1, sum;

            for (i = 0; i < this.m; i++)
            {
                for (j = 0; j < this.n; j++)
                {
                    // ci and cj depends on frequency as well as 
                    // number of row and columns of specified matrix 
                    if (i == 0)
                        ci = 1 / Math.sqrt(this.m);
                    else
                        ci = Math.sqrt(2) / Math.sqrt(this.m);

                    if (j == 0)
                        cj = 1 / Math.sqrt(this.n);
                    else
                        cj = Math.sqrt(2) / Math.sqrt(this.n);

                    // sum will temporarily store the sum of  
                    // cosine signals 
                    sum = 0;
                    for (k = 0; k < this.m; k++)
                    {
                        for (l = 0; l < n; l++)
                        {
                            dct1 = matrix[k][l] *
                                   Math.cos((2 * k + 1) * i * Math.PI / (2 * this.m)) *
                                   Math.cos((2 * l + 1) * j * Math.PI / (2 * this.n));
                            sum = sum + dct1;
                        }
                    }
                    dct[i][j] = ci * cj * sum;
                }
            }

            //Print result in console
            //ONLY FOR TESTING
            for (i = 0; i < this.m; i++)
            {
                for (j = 0; j < this.n; j++)
                {
                    Console.printf("%f\t", dct[i][j]);
                }
                Console.println();
            }

            return dct;
        }
    }
}
