﻿partial class Program
{
    static async IAsyncEnumerable<string> GetStocksAsync()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return $"{AtoZ()}{AtoZ()}{AtoZ()}{AtoZ()}";
            await Task.Delay(3000);
        }
    }

    static string AtoZ()
    {
        return char.ConvertFromUtf32(Random.Shared.Next(65, 91));
    }
}