using System.Collections.Generic;

public class ZickZack
{

    private static int[][] zickZackMapping8 = {
          new int[] {1, 2, 6, 7, 15, 16, 28, 29},
          new int[] {3, 5, 8, 14, 17, 27, 30, 43},
          new int[] {4, 9, 13, 18, 26, 31, 42, 44},
          new int[] {10, 12, 19, 25, 32, 41, 45, 54},
          new int[] {11, 20, 24, 33, 40, 46, 53, 55},
          new int[] {21, 23, 34, 39, 47, 52, 56, 61},
          new int[] {22, 35, 38, 48, 51, 57, 60, 62},
          new int[] {36, 37, 49, 50, 58, 59, 63, 64}};

    public static int[] ToArray(int[][] matrix, int blockSize)
    {
        if (blockSize != 8)
        {
            return null;
        }

        int yLenght = matrix[0].Length;
        int xLenght = matrix.Length;

        int[] result = new int[xLenght * yLenght];

        int offsetForBlock = 0;
        // for each block in each row...
        for (int y = 0; y < yLenght; y += blockSize)
        {
            //... and in each column...
            for (int x = 0; x < xLenght; x += blockSize)
            {
                //... are we performing the zigzack encoding
                Parse(matrix, y, x, blockSize, offsetForBlock, result);
                offsetForBlock += blockSize * blockSize;
            }
        }
        return result;
    }

    private static void Parse(int[][] matrix, int y, int x, int blockSize, int offsetForBlock, int[] result)
    {
        int rows = y;
        int columns = x;

        for (int i = 0; x < blockSize; x++)
        {
            for (int j = 0; y < blockSize; y++)
            {
                int indexInBlock = zickZackMapping8[j][i] - 1;
                result[offsetForBlock + indexInBlock] = matrix[y + j][x + i];
            }
        }
    }

    private static int[,] ReverseZickZackMapping8 = new int[,] {
          {0, 0}, {0, 1}, {1, 0}, {2, 0}, {1, 1}, {0, 2}, {0, 3}, {1, 2},
          {2, 1}, {3, 0}, {4, 0}, {3, 1}, {2, 2}, {1, 3}, {0, 4}, {0, 5},
          {1, 4}, {2, 3}, {3, 2}, {4, 1}, {5, 0}, {6, 0}, {5, 1}, {4, 2},
          {3, 3}, {2, 4}, {1, 5}, {0, 6}, {0, 7}, {1, 6}, {2, 5}, {3, 4},
          {4, 3}, {5, 2}, {6, 1}, {7, 0}, {7, 1}, {6, 2}, {5, 3}, {4, 4},
          {3, 5}, {2, 6}, {1, 7}, {2, 7}, {3, 6}, {4, 5}, {5, 4}, {6, 3},
          {7, 2}, {7, 3}, {6, 4}, {5, 5}, {4, 6}, {3, 7}, {4, 7}, {5, 6},
          {6, 5}, {7, 4}, {7, 5}, {6, 6}, {5, 7}, {6, 7}, {7, 6}, {7, 7}};

    public static int[,] ToMatrix(int[] data, int blockSize, int xLenght, int yLenght)
    {
        if (blockSize != 8)
        {
            return null;
        }

        int[,] result = new int[xLenght, yLenght];

        int offset = 0;
        // For each row in the image...
        for (int y = 0; y < yLenght; y += blockSize)
        {
            // ... for each row in the image...
            for (int x = 0; x < xLenght; x += blockSize)
            {
                //... we are inversing the zickzack encoding
                ZickzackInverseBlock(data, offset, y, x, blockSize, result);
                offset += blockSize * blockSize;
            }
        }
        return result;
    }

    private static void ZickzackInverseBlock(int[] data, int offsetInArr, int y, int x, int blockSize, int[,] result) 
    {
        for (int i = 0; i < (blockSize * blockSize); i++) 
        {
            int y_offset = ReverseZickZackMapping8[i,0];
            int x_offset = ReverseZickZackMapping8[i,1];
            result[y + y_offset,x + x_offset] = data[offsetInArr + i];
        }
    }
}