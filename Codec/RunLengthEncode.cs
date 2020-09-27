using System.Collections.Generic;

public class RunLengthEncode
{
    public static int[] Encode(int[,] matrix, int blockSize)
    {
        int[] data = ZickZack.ToArray(matrix, blockSize);

        int[] encodedData = Encode(data);

        //Codec.Tester.PrintToFile("encodedData", encodedData);

        return encodedData;
    }

    private static int[] Encode(int[] data)
    {
        List<int> list = new List<int>();

        int valueLength;

        for (int i = 0; i < data.Length; i++)
        {
            valueLength = 1;
            while(i + 1 < data.Length && data[i] == data[i+1])
            {
                valueLength++;
                i++;
            }

            list.Add(valueLength);
            list.Add(data[i]);
        }

        return list.ToArray();
    }

    public static int[,] Decode(int[] data, int blockSize, int width, int height)
    { 
        int[] decodedData = Decode(data);

        //Codec.Tester.PrintToFile("decodedData",decodedData);

        int[,] matrix = ZickZack.ToMatrix(decodedData, blockSize, width, height);

        return matrix;
    }

    private static int[] Decode(int[] data)
    {
        List<int> list = new List<int>();

        int valueLength = 1;

        for (int i = 0; i < data.Length; i++)
        {
            if (i % 2 == 0)
            {
                valueLength = data[i];
            }
            else
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