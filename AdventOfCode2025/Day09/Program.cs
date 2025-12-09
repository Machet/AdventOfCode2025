using Utils;
// 4602912072 ^^
var input = File.ReadAllLines("input.txt")
	.Select(l => l.Split(',').Select(long.Parse).ToList())
	.Select(i => new Point(i[0], i[1]))
	.ToList();

var maxArea = input.GenerateAllCombinations()
	.Select(i => (1 + Math.Abs(i.first.X - i.second.X)) * (1 + Math.Abs(i.first.Y - i.second.Y)))
	.OrderDescending()
	.First();

Console.WriteLine("1: " + maxArea);

var vLines = new List<Line>();
var hLines = new List<Line>();
var current = input[0];
foreach (var next in input.Skip(1))
{
	if (next.X == current.X)
	{
		vLines.Add(new Line("V", current.X, current.Y, next.Y));
	}
	else
	{
		hLines.Add(new Line("H", current.Y, current.X, next.X));
	}

	current = next;
}

var maxArea2 = input
	.GenerateAllCombinations()
	.Select(pair => new Rect(pair.first, pair.second))
	.Where(rect => !input.Any(i => rect.IsWithin(i)))
	.OrderByDescending(r => r.GetArea())
	.Where(rect => IsRectWithin(rect, vLines, hLines))
	.First();

Console.WriteLine("2: " + maxArea2.GetArea());

bool IsRectWithin(Rect rect, List<Line> vLines, List<Line> hLines)
{
	return IsPointWithin(new Point(rect.Start.X, rect.End.Y), vLines, hLines)
		&& IsPointWithin(new Point(rect.End.X, rect.Start.Y), vLines, hLines)
		&& !vLines.Any(rect.Collide)
		&& !hLines.Any(rect.Collide);
}

bool IsPointWithin(Point point, List<Line> vLines, List<Line> hLines)
{
	var hCount = vLines.Where(l => l.Pos <= point.X && l.Casts(point.Y)).Count();
	var isOnHLine = hLines.Any(l => l.Pos == point.Y && l.Contains(point.X));
	return isOnHLine || (hCount % 2 == 1);
}

record Point(long X, long Y);
record Line(string Type, long Pos, long Start, long End)
{
	public Range R => new Range(Math.Min(Start, End), Math.Max(Start, End));
	public bool Contains(long value) => R.Contains(value);
	public bool Casts(long value) => R.Contains(value) && value != Start;
}

record Range(long Start, long End)
{
	public bool Contains(long value) => Start <= value && End >= value;
	public bool IsWithin(long value) => Start < value && End > value;
	public long Length => End - Start + 1;
}

record Rect(Point Start, Point End)
{
	public Range XRange = new Range(Math.Min(Start.X, End.X), Math.Max(Start.X, End.X));
	public Range YRange = new Range(Math.Min(Start.Y, End.Y), Math.Max(Start.Y, End.Y));

	public bool IsWithin(Point point)
		=> XRange.IsWithin(point.X) && YRange.IsWithin(point.Y);

	public long GetArea() => XRange.Length * YRange.Length;

	internal bool Collide(Line line)
	{
		if (line.Type == "V")
		{
			return XRange.IsWithin(line.Pos) && line.R.Start < YRange.End && line.R.End > YRange.Start;
		}
		else 
		{
			return YRange.IsWithin(line.Pos) && line.R.Start < XRange.End && line.R.End > XRange.Start;
		}
	}
}