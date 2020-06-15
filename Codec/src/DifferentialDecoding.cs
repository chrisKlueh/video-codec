﻿public class DifferentialDecoding
{
    public static int[][] Run(int[][] matrix, int blockSize)
    {

        int xLenght = matrix[0].Length;
        int yLenght = matrix.Length;

        int[][] result = matrix.DeepClone();

        int d = 0;
        int tempD = 0;

        for (int y = 0; y < yLenght; y += blockSize)
        {
            for (int x = 0; x < xLenght; x += blockSize)
            {
                result[y][x] += d;
                d = result[y][x];
            }
        }

        return result;
    }
}
