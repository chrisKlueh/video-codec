using System.Collections.Generic;

public class RunLengthDecode
{
    public static int[,] Run(int[] data, int blockSize)
    {
        int[] decodedData = Decode(data);

        int[,] matrix = ZickZack.ToMatrix(decodedData, blockSize, 8, 8);

        return matrix;
    }

    public static int[] Decode(int[] data)
    {
        List<int> list = new List<int>();

        int valueLength = 1;

        for (int i = 0; i < data.Length; i++)
        {
            if (i % 2 == 0)
            {
                valueLength = data[i]; 
            } else
            {
                for (int y = 0; y < valueLength; y++)
                {
                    list.Add(data[i]);
                }
            }
        }

        return list.ToArray();
    }
}