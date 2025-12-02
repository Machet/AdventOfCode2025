var ranges = File.ReadAllText("input.txt")
	.Split(",")
	.Select(l => l.Split("-"))
	.Select(p => new Range(long.Parse(p[0]), long.Parse(p[1])))
	.ToList();

var max = ranges.Max(r => r.End);
var maxLength = max.ToString().Length;
var lengthToCheck = (maxLength + 1) / 2;
var maxNumberToConsider = long.Parse(new string('9', lengthToCheck));

var invalidNumbers1 = new HashSet<long>();
var invalidNumbers2 = new HashSet<long>();

for (long i = 1; i <= maxNumberToConsider; i++)
{
	var pattern = i.ToString();

	invalidNumbers1.Add(long.Parse(pattern + pattern));

	for (int j = 1; j < maxLength / pattern.Length; j++)
	{
		var numberToAdd = string.Concat(Enumerable.Repeat(pattern, j + 1));
		invalidNumbers2.Add(long.Parse(numberToAdd));
	}
}

var foundInvalidNumbers1 = invalidNumbers1
	.Where(n => ranges.Any(r => r.Contains(n)))
	.ToList();

var foundInvalidNumbers2 = invalidNumbers2
	.Where(n => ranges.Any(r => r.Contains(n)))
	.ToList();

Console.WriteLine($"1: {foundInvalidNumbers1.Sum()}");
Console.WriteLine($"2: {foundInvalidNumbers2.Sum()}");

record Range(long Start, long End)
{
	public bool Contains(long value) => Start <= value && End >= value;
}