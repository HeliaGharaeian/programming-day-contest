using System;
using System.Collections.Generic;
using System.Linq;

public class MafiaSolver
{
    public static string Solve(int[][] a, int seedIndex = 0, int seedValue = 1)
    {
        int n = a.Length;
        var edges = new List<(int to, int parity)>[n];
        for (int i = 0; i < n; i++) edges[i] = new List<(int, int)>();


        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (a[i][j] == 0) continue;
                int parity = (a[i][j] == 1) ? 0 : 1;
                edges[i].Add((j, parity));
                edges[j].Add((i, parity));
            }
        }

        var role = new int?[n];
        var q = new Queue<int>();

        role[seedIndex] = seedValue;
        q.Enqueue(seedIndex);

        while (q.Count > 0)
        {
            int u = q.Dequeue();
            foreach (var (v, p) in edges[u])
            {
                int expected = role[u]!.Value ^ p;
                if (role[v] == null)
                {
                    role[v] = expected;
                    q.Enqueue(v);
                }
                else if (role[v] != expected)
                {
                    throw new InvalidOperationException("Inconsistent statements: no valid assignment.");
                }
            }
        }

        for (int i = 0; i < n; i++)
            if (role[i] == null) role[i] = 0;

        return string.Concat(role.Select(x => x!.Value.ToString()));
    }

    public static void Main()
    {
        int[][] a =
        {
            new[] { 1, 0, 0, 0, 0, 1, -1, -1, 0},
            new[] { -1, 1, 1, 1, -1, -1, 0, 1, 1 },
            new[] { 0, 0, 1, 1, 0, 0, 1, 1, 1},
            new[] { -1, 0, 0, 1, -1, -1, 1, 1, 0},
            new[] { 0, 0, 0, -1, 1, 0, -1, -1, 0 },
            new[] { 0, -1, 0, 0, 1, 1, -1, 0, -1},
            new[] { 0, 1, 0, 1, 0, -1, 1, 1, 0},
            new[] { -1, 0, 1, 1, 0, -1, 1, 1, 0},
            new[] {-1, 1, 0, 1, 0, 0, 0, 1, 1},

        };

        string result = Solve(a, seedIndex: 0, seedValue: 1);
        Console.WriteLine(result);
    }
}
