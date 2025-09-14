using System;
using System.Collections.Generic;

class Program
{
    static int SegmentCost((int x, int y) a, (int x, int y) b)
    {
        int dx = Math.Abs(a.x - b.x);
        int dy = Math.Abs(a.y - b.y);
        return Math.Max(dx, dy); 
    }

    static int PathCost(IList<(int x, int y)> pts, bool countStartCell = false)
    {
        if (pts == null || pts.Count < 2) return 0;

        int total = 0;
        for (int i = 0; i < pts.Count - 1; i++)
            total += SegmentCost(pts[i], pts[i + 1]);


        return countStartCell ? total + 1 : total;
    }

    static void Main()
    {
        var points = new List<(int x, int y)>
        {
            (13, 14), (6, 13), (18, 7), (4, 11), (3, 14)
        };

        int answer = PathCost(points, countStartCell: false);
        Console.WriteLine(answer); 
    }
}
