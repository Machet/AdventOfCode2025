using Utils;

var points = File.ReadAllLines("input.txt")
	.Select(l => l.Split(",").Select(long.Parse).ToArray())
	.Select(arr => new Point3D(arr[0], arr[1], arr[2]))
	.ToList();

var combos = points.GenerateAllCombinations()
	.OrderBy(pair => pair.first.DistanceTo(pair.second))
	.Take(1000);

var circuits = points.Select(p => new HashSet<Point3D> { p }).ToList();
foreach (var combo in combos)
{
	Connect(circuits, combo);
}

var result1 = circuits
	.Select(c => c.Count)
	.OrderByDescending(c => c)
	.Take(3)
	.Multiply();

Console.WriteLine("1: " + result1);

var combos2 = points.GenerateAllCombinations()
	.OrderBy(pair => pair.first.DistanceTo(pair.second));

var circuits2 = points.Select(p => new HashSet<Point3D> { p }).ToList();
foreach (var combo in combos2)
{
	Connect(circuits2, combo);

	if (circuits2.Count == 1)
	{
		Console.WriteLine("2: " + combo.first.X * combo.second.X);
		break;
	}
}

static void Connect(List<HashSet<Point3D>> circuits, (Point3D first, Point3D second) combo)
{
	var found = circuits.Where(c => c.Contains(combo.first) || c.Contains(combo.second)).ToList();
	if (found.Count == 0)
	{
		circuits.Add([combo.first, combo.second]);
	}
	else if (found.Count == 1)
	{
		found[0].Add(combo.first);
		found[0].Add(combo.second);
	}
	else
	{
		found[0].AddRange(found[1]);
		circuits.Remove(found[1]);
	}
}

record Point3D(long X, long Y, long Z)
{
	public double DistanceTo(Point3D other)
	{
		var dx = X - other.X;
		var dy = Y - other.Y;
		var dz = Z - other.Z;
		return Math.Sqrt(dx * dx + dy * dy + dz * dz);
	}
}