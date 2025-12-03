var result1 = File.ReadAllLines("input.txt")
	.Select(l => FindBestVoltage(l, 2))
	.Sum();

var result2 = File.ReadAllLines("input.txt")
	.Select(l => FindBestVoltage(l, 12))
	.Sum();

Console.WriteLine("1: " + result1);
Console.WriteLine("2: " + result2);

long FindBestVoltage(string bank, int batteryCount)
{
	var sorted = bank.Index()
		.OrderByDescending(i => i.Item)
		.ThenBy(i => i.Index)
		.ToList();

	var batteries = new List<char>();
	var minPos = -1;
	
	for (int maxPos = batteryCount - 1; maxPos >= 0; maxPos--)
	{
		var highest = sorted.First(i => i.Index < bank.Length - maxPos && i.Index > minPos);
		batteries.Add(highest.Item);
		minPos = highest.Index;
	}

	return long.Parse(new string(batteries.ToArray()));
}