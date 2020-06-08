using System;

namespace Codec
{
    // https://www.geeksforgeeks.org/discrete-cosine-transform-algorithm-program/

    class DCT
    {
        public static int n = 8, m = 8;

        // Function to find discrete cosine transform
        static int[][] dctTransform(int[][] matrix)
        {
            int i, j, k, l;

            // dct will store the discrete cosine transform 
            double[][] dct = new double[m][n];

            double ci, cj, dct1, sum;

            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                {
                    // ci and cj depends on frequency as well as 
                    // number of row and columns of specified matrix 
                    if (i == 0)
                        ci = 1 / Math.sqrt(m);
                    else
                        ci = Math.sqrt(2) / Math.sqrt(m);

                    if (j == 0)
                        cj = 1 / Math.sqrt(n);
                    else
                        cj = Math.sqrt(2) / Math.sqrt(n);

                    // sum will temporarily store the sum of  
                    // cosine signals 
                    sum = 0;
                    for (k = 0; k < m; k++)
                    {
                        for (l = 0; l < n; l++)
                        {
                            dct1 = matrix[k][l] *
                                   Math.cos((2 * k + 1) * i * Math.PI / (2 * m)) *
                                   Math.cos((2 * l + 1) * j * Math.PI / (2 * n));
                            sum = sum + dct1;
                        }
                    }
                    dct[i][j] = ci * cj * sum;
                }
            }

            //Print result in console
            //ONLY FOR TESTING
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                {
                    Console.printf("%f\t", dct[i][j]);
                }
                Console.println();
            }

            return dct;
        }
    }
}
