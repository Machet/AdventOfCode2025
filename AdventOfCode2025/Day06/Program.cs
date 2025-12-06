using System.Text;

var input = File.ReadAllLines("input.txt");

var numbers = input[0..^1]
	.Select(i => i.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList())
	.ToList();

var operations = input[^1]
	.Split(' ', StringSplitOptions.RemoveEmptyEntries)
	.Select(i => i.Trim())
	.ToList();

var hNumbers = numbers.SelectMany(ns => ns.Select((number, pos) => (number, pos)))
	.GroupBy(t => t.pos)
	.OrderBy(g => g.Key)
	.Select(g => g.Select(gg => gg.number).ToList())
	.ToList();

var res = Caculate(operations, hNumbers);
Console.WriteLine("1: " + res.Sum());

var vNumbersAcc = input[0].Select(i => new StringBuilder()).ToList();

for (int i = 0; i < input[0].Length; i++)
{
	for (int j = 0; j < input.Length - 1; j++)
	{
		vNumbersAcc[i].Append(input[j][i]);
	}
}

var vNumbers = operations
	.Select(o => new List<long>())
	.ToList();

int pos = 0;
foreach (var vNumber in vNumbersAcc)
{
	var n = vNumber.ToString().Trim();
	if (string.IsNullOrEmpty(n))
	{
		pos++;
	}
	else
	{
		vNumbers[pos].Add(long.Parse(n));
	}
}

List<long> vResults = Caculate(operations, vNumbers);
Console.WriteLine("2: " + vResults.Sum());

static List<long> Caculate(List<string> operations, List<List<long>> vNumbers)
{
	return vNumbers.Select((n, i) =>
	{
		return operations[i] switch
		{
			"+" => n.Sum(),
			"*" => n.Aggregate(1L, (a, b) => a * b),
			_ => 0L
		};
	}).ToList();
}