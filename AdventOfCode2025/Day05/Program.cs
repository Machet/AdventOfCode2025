using Utils;

var input = File.ReadAllLines("input-test.txt");

var ranges = input.TakeWhile(i => !string.IsNullOrWhiteSpace(i))
	.Select(line =>
	{
		var parts = line.Split("-");
		return new LongRange(long.Parse(parts[0]), long.Parse(parts[1]));
	})
	.ToList();

var numbers = input.SkipWhile(i => !string.IsNullOrWhiteSpace(i)).Skip(1)
	.Select(long.Parse)
	.ToList();

var result1 = numbers
	.Where(n => ranges.Any(r => r.Contains(n)))
	.Count();

Console.WriteLine("1: " + result1);

var uniformRanges = new List<LongRange>();
ranges.Sort((a, b) => a.start.CompareTo(b.start));
var currentRange = ranges[0];

for (int i = 1; i < ranges.Count; i++)
{
	var nextRange = ranges[i];
	var merged = currentRange.Merge(nextRange).ToList();

	if (merged.Count == 1)
	{
		currentRange = merged[0];
	}
	else
	{
		uniformRanges.Add(currentRange);
		currentRange = merged[1];
	}
}

uniformRanges.Add(currentRange);

var result2 = uniformRanges
	.Select(r => r.Length)
	.Sum();

Console.WriteLine("2: " + result2);
