using System.Collections.Generic;

public class ZickZack
{

    private static int[,] zickZackMapping8 = new int[,] {
           {1, 2, 6, 7, 15, 16, 28, 29},
           {3, 5, 8, 14, 17, 27, 30, 43},
           {4, 9, 13, 18, 26, 31, 42, 44},
           {10, 12, 19, 25, 32, 41, 45, 54},
           {11, 20, 24, 33, 40, 46, 53, 55},
           {21, 23, 34, 39, 47, 52, 56, 61},
           {22, 35, 38, 48, 51, 57, 60, 62},
           {36, 37, 49, 50, 58, 59, 63, 64}
};

    public static int[] ToArray(int[,] matrix, int blockSize)
    {
        if (blockSize != 8)
        {
            return null;
        }

        int xLenght = matrix.GetLength(0);
        int yLenght = matrix.GetLength(1);

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

    private static void Parse(int[,] matrix, int y, int x, int blockSize, int offsetForBlock, int[] result)
    {
  
        for (int i = 0; i < blockSize; i++)
        {
            for (int j = 0; j < blockSize; j++)
            {
                int indexInBlock = zickZackMapping8[j,i] - 1;
                result[offsetForBlock + indexInBlock] = matrix[x + j,y + i];
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

        int[,] result = new int[xLenght,yLenght];

        int offset = 0;
        // For each row in the image...
        for (int y = 0; y < yLenght; y += blockSize)
        {
            // ... for each row in the image...
            for (int x = 0; x < xLenght; x += blockSize)
            {
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
            int y_offset = ReverseZickZackMapping8[i,1];
            int x_offset = ReverseZickZackMapping8[i,0];
            result[x + x_offset,y + y_offset] = data[offsetInArr + i];
        }
    }
}