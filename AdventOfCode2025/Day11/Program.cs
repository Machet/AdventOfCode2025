using Utils;

var topology = File.ReadAllLines("input.txt")
	.Select(l => l.Split(":"))
	.ToDictionary(p => p[0], p => p[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToHashSet());

topology["out"] = new HashSet<string>();

var youToOut = FindPathCount("you", "out", topology);

Console.WriteLine("1: " + youToOut);

var fftToOut = FindPathCount("fft", "out", topology);
var dacToOut = FindPathCount("dac", "out", topology);
var dacToFft = FindPathCount("dac", "fft", topology);
var fftToDac = FindPathCount("fft", "dac", topology);
var svrToDac = FindPathCount("svr", "dac", topology);
var svrToFft = FindPathCount("svr", "fft", topology);

var svrToOutThroughDacFft = svrToFft * fftToDac * dacToOut + svrToDac * dacToFft * fftToOut;
Console.WriteLine("2: " + svrToOutThroughDacFft);

static long FindPathCount(string from, string to, Dictionary<string, HashSet<string>> tree)
{
	var toProcess = new Stack<string>([from]);
	var calculated = new Dictionary<string, long>();

	while (toProcess.Count > 0)
	{
		var current = toProcess.Pop();
		var connections = tree[current];
		if (connections.Contains(to))
		{
			calculated[current] = 1;
			continue;
		}

		var missing = connections.Where(c => !calculated.ContainsKey(c));
		if (missing.Any())
		{
			toProcess.Push(current);
			toProcess.PushRange(missing);
		}
		else
		{
			calculated[current] = connections.Select(c => calculated[c]).Sum();
		}
	}

	return calculated[from];
}

