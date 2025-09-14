using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string text;
        if (Console.IsInputRedirected)
        {
            text = Console.In.ReadToEnd();
        }
        else
        {
            Console.WriteLine("Paste your matrix. Enter rows on separate lines (numbers by spaces or commas).");
            Console.WriteLine("Press Enter on an empty line to finish.");
            Console.WriteLine("Examples:\n  1 2 3\n  4 5 6\n  7 8 9\n\n  or:  1,2,3;4,5,6;7,8,9\n");
            var lines = new List<string>();
            while (true)
            {
                string? line = Console.ReadLine();
                if (line == null) break;
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (lines.Count > 0) break;
                    else continue;
                }
                lines.Add(line);
            }
            text = string.Join(Environment.NewLine, lines);
        }

        try
        {
            var mat = ParseMatrix(text);
            int nrows = mat.GetLength(0);
            int ncols = mat.GetLength(1);

            Console.WriteLine($"Read a {nrows}×{ncols} matrix:");
            Console.WriteLine(FormatMatrix(mat));

            while (true)
            {
                Console.WriteLine("\nChoose an action: [t]ranspose, [s]hape, [q]uit");
                Console.Write("> ");
                string? choice = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (choice == "q") break;
                else if (choice == "s")
                {
                    Console.WriteLine($"Shape: {nrows}×{ncols}");
                }
                else if (choice == "t")
                {
                    var tr = Transpose(mat);
                    Console.WriteLine("Transpose:");
                    Console.WriteLine(FormatMatrix(tr));
                }
                else
                {
                    Console.WriteLine("Unknown option. Try t, s, or q.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error: " + ex.Message);
            Environment.ExitCode = 1;
        }
    }

    // --- Parsing ---

    private static double[,] ParseMatrix(string text)
    {
        text = (text ?? "").Trim();
        if (text.Length == 0) throw new ArgumentException("No input received.");

        List<string> rows;
        if (!text.Contains('\n') && text.Contains(';'))
            rows = text.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        else
            rows = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        if (rows.Count == 0) throw new ArgumentException("No rows found.");

        var parsedRows = new List<double[]>();
        foreach (var row in rows)
            parsedRows.Add(ParseRow(row));

        int ncols = parsedRows[0].Length;
        if (ncols == 0) throw new ArgumentException("First row has no numbers.");

        for (int i = 0; i < parsedRows.Count; i++)
            if (parsedRows[i].Length != ncols)
                throw new ArgumentException($"Row {i + 1} has {parsedRows[i].Length} columns; expected {ncols}.");

        // Convert to rectangular 2D array
        int nrows = parsedRows.Count;
        var mat = new double[nrows, ncols];
        for (int r = 0; r < nrows; r++)
            for (int c = 0; c < ncols; c++)
                mat[r, c] = parsedRows[r][c];

        return mat;
    }

    private static double[] ParseRow(string rowText)
    {
        string cleaned = rowText.Trim().Trim('[', ']', '(', ')', '{', '}');
        // Split on commas or any whitespace
        var tokens = Regex.Split(cleaned, @"[,\s]+").Where(t => t.Length > 0).ToArray();
        if (tokens.Length == 0) throw new ArgumentException("Empty row.");

        var vals = new double[tokens.Length];
        for (int i = 0; i < tokens.Length; i++)
        {
            string t = tokens[i];
            // Try strict invariant culture first (expects '.' as decimal sep)
            if (!double.TryParse(t, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double v))
            {
                // Fallback: try current culture
                if (!double.TryParse(t, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out v))
                    throw new ArgumentException($"Not a number: '{t}'");
            }
            vals[i] = v;
        }
        return vals;
    }

    // --- Formatting & ops ---

    private static string FormatMatrix(double[,] m)
    {
        int r = m.GetLength(0), c = m.GetLength(1);
        var svals = new string[r, c];
        var widths = new int[c];

        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                string s = m[i, j].ToString("G", CultureInfo.InvariantCulture);
                svals[i, j] = s;
                if (s.Length > widths[j]) widths[j] = s.Length;
            }
        }

        var sb = new StringBuilder();
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
            {
                if (j > 0) sb.Append(' ');
                sb.Append(svals[i, j].PadLeft(widths[j]));
            }
            if (i < r - 1) sb.AppendLine();
        }
        return sb.ToString();
    }

    private static double[,] Transpose(double[,] m)
    {
        int r = m.GetLength(0), c = m.GetLength(1);
        var t = new double[c, r];
        for (int i = 0; i < r; i++)
            for (int j = 0; j < c; j++)
                t[j, i] = m[i, j];
        return t;
    }
}
