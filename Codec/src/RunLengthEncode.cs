using System.Collections.Generic;

public class RunLengthEncode
{
    public static int[] Run(int[][] matrix, int blockSize)
    {
        int[] data = ZickZack.ToArray(matrix, blockSize);

        int[] encodedData = Encode(data);

        return encodedData;
    }

    public static int[] Encode(int[] data)
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
}