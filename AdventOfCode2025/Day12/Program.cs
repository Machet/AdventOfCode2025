using System.Linq;
using Utils;

var input = File.ReadAllText("input.txt")
	.Split(["\r\n\r\n", "\n\n"], StringSplitOptions.RemoveEmptyEntries)
	.Select(x => x.Trim())
	.ToList();

var shapes = input.Where(i => i[1] == ':')
	.Select(ParseShape)
	.ToDictionary(s => s.Id, s => s);

var tests = input.Single(i => i[1] != ':').Split('\n', StringSplitOptions.RemoveEmptyEntries)
	.Select(ParseTest)
	.ToList();

var emptySpace = tests.Select(t => t.Size - t.ShapeSize(shapes));

Console.WriteLine("1: " + emptySpace.Where(x => x > 0).Count());

Shape ParseShape(string line)
{
	var val = line.Split('\n');
	var id = int.Parse(val[0].Trim().TrimEnd(':'));
	var points = val[1..]
		.SelectMany((line, y) => line.Select((c, x) => (c, x, y)))
		.Where(t => t.c == '#')
		.Select(t => new Point(t.x, t.y))
		.ToList();

	return new Shape(id, points);
}

Test ParseTest(string line)
{
	var val = line.Split(' ');
	var dim = val[0].Split("x");
	var width = int.Parse(dim[0].Trim());
	var height = int.Parse(dim[1].TrimEnd(":").Trim());
	var shapeCounts = val[1..].Select(int.Parse).ToList();

	return new Test(width, height, shapeCounts);
}

record Shape(int Id, List<Point> Points)
{
	public int Size => Points.Count;
};

record Test(int Width, int Height, List<int> ShapeCounts)
{
	public int Size => Width * Height;

	internal int ShapeSize(Dictionary<int, Shape> shapes)
	{
		return ShapeCounts.Select((count, index) =>
		{
			return count * shapes[index].Size;
		}).Sum();
	}
};