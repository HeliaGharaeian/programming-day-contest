//using System;
//using System.Collections.Generic;

//public class MafiaSolver
//{
//    static int SegmentCost((int x, int y) a, (int x, int y) b)
//    {
//        int dx = Math.Abs(a.x - b.x);
//        int dy = Math.Abs(a.y - b.y);
//        return Math.Max(dx, dy); 
//    }

//    static int PathCost(IList<(int x, int y)> pts, bool countStartCell = false)
//    {
//        if (pts == null || pts.Count < 2) return 0;

//        int total = 0;
//        for (int i = 0; i < pts.Count - 1; i++)
//            total += SegmentCost(pts[i], pts[i + 1]);


//        return countStartCell ? total + 1 : total;
//    }

//    static void Main()
//    {
//        var points = new List<(int x, int y)>
//        {
//            (13, 14), (6, 13), (18, 7), (4, 11), (3, 14)
//        };

//        int answer = PathCost(points, countStartCell: false);
//        Console.WriteLine(answer); 
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string all = Console.In.ReadToEnd();
        var nums = Regex.Matches(all, @"-?\d+").Select(m => int.Parse(m.Value)).ToList();
        if (nums.Count < 4 || nums.Count % 2 != 0) { Console.WriteLine(0); return; }

        var pts = new List<(int x, int y)>();
        for (int i = 0; i < nums.Count; i += 2)
            pts.Add((nums[i], nums[i + 1]));

        var cells = new HashSet<(int, int)>();
        for (int i = 0; i + 1 < pts.Count; i++)
            RasterizeSegment(pts[i], pts[i + 1], cells);  

        Console.WriteLine(cells.Count);
    }

    static void RasterizeSegment((int x, int y) a, (int x, int y) b, HashSet<(int, int)> set)
    {
        int x0 = a.x, y0 = a.y, x1 = b.x, y1 = b.y;
        int dx = Math.Abs(x1 - x0), dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : (x0 > x1 ? -1 : 0);
        int sy = y0 < y1 ? 1 : (y0 > y1 ? -1 : 0);

        set.Add((x0, y0));
        if (dx >= dy)
        {
            int e = 0;
            for (int i = 0; i < dx; i++)
            {
                x0 += sx;
                e += dy;
                if ((e << 1) > dx) { y0 += sy; e -= dx; }   
                set.Add((x0, y0));
            }
        }
        else
        {
            int e = 0;
            for (int i = 0; i < dy; i++)
            {
                y0 += sy;
                e += dx;
                if ((e << 1) > dy) { x0 += sx; e -= dy; }
                set.Add((x0, y0));
            }
        }
    }
}
