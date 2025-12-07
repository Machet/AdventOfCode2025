
var lines = File.ReadAllLines("input.txt");

var start = lines[0]
	.Select((c, i) => new { c, i })
	.First(p => p.c == 'S').i;

HashSet<Beam> beams2 = [new Beam(start, 0, 1)];
foreach (var line in lines.Skip(1))
{
	beams2 = Split2(beams2, line).ToHashSet();
}

Console.WriteLine("1: " + beams2.Sum(b => b.splitCounts));
Console.WriteLine("2: " + beams2.Sum(b => b.possiblePaths));

IEnumerable<Beam> Split2(IEnumerable<Beam> beams, string line)
{
	return beams
		.SelectMany(beam =>
		{
			if (line[beam.position] == '^')
			{
				var b1 = new Beam(beam.position - 1, beam.splitCounts + 1, beam.possiblePaths);
				var b2 = new Beam(beam.position + 1, 0, beam.possiblePaths);
				return new List<Beam>() { b1, b2 };
			}
			return [beam];
		})
		.GroupBy(b => b.position)
		.Select(g => new Beam(g.Key, g.Sum(gb => gb.splitCounts), g.Sum(gb => gb.possiblePaths)));
}

record Beam(int position, decimal splitCounts, decimal possiblePaths);