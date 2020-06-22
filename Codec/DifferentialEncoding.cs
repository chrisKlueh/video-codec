[System.Serializable]
public class DifferentialEncoding  
{
    public static int[,] Encode(int[,] matrix, int blockSize)
    {

        int xLenght = matrix.GetLength(0);
        int yLenght = matrix.GetLength(1);

        int d = 0;
        int tempD = 0;

        for (int y = 0; y < yLenght; y += blockSize)
        {
            for (int x = 0; x < xLenght; x += blockSize)
            {
                tempD = matrix[x,y];
                matrix[x,y] -= d;
                d = tempD;
            }
        }

        return matrix;
    }

    public static int[,] Decode(int[,] matrix, int blockSize)
    {

        int xLenght = matrix.GetLength(0);
        int yLenght = matrix.GetLength(1);

        int d = 0;
        int tempD = 0;

        for (int y = 0; y < yLenght; y += blockSize)
        {
            for (int x = 0; x < xLenght; x += blockSize)
            {
                matrix[x,y] += d;
                d = matrix[x,y];
            }
        }

        return matrix;
    }
}
