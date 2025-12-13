using System.Collections.Immutable;
using Utils;
using static Utils.StateSearch;

var input = File.ReadAllLines("input.txt")
	.Select(Parse)
	.ToList();

var minLightFlips = input.Select(i => FindBestPath<LightToggleState, int>(new LightToggleState(0, 0, i)))
	.Select(s => s!.FlipCount)
	.ToList();

Console.WriteLine("1: " + minLightFlips.Sum());

var minJoltagePressures = input.Select(SolveJoltagePresses).ToList();
Console.WriteLine("2: " + minJoltagePressures.Sum());

Input Parse(string line)
{
	var parts = line.Split(" ");
	var flipsList = parts.Where(p => p.StartsWith("(")).Select(ConvertToRange).ToList();
	var jolts = parts.Where(p => p.StartsWith("{")).Select(ConvertToRange).Single();
	return new Input(parts[0][1..^1], flipsList, jolts);
}

List<int> ConvertToRange(string range)
	=> range[1..^1].Split(",").Select(int.Parse).ToList();

int SolveJoltagePresses(Input input)
{
	var buttonPressesMatrix = input.GetJoltageCalculationMatrix();
	var (reduced, unknownCols) = GaussJordan.Elimination(buttonPressesMatrix);

	if (unknownCols.Count == 0)
	{
		return Solve(reduced, [], ImmutableList<int>.Empty);
	}

	var availableValues = unknownCols
		.Select(input.GetMaxAvailableNumberPresses)
		.ToImmutableList();

	var possibleUnknowns = GenerateCombinations(ImmutableList<ImmutableList<int>>.Empty, availableValues);
	return possibleUnknowns
		.Select(unknown => Solve(buttonPressesMatrix, unknownCols, unknown))
		.Where(result => result > 0)
		.Min();
}

int Solve(decimal[,] matrix, List<int> unknownCols, ImmutableList<int> possibleValues)
{
	int rowCount = matrix.GetLength(0);
	var results = Enumerable.Range(0, rowCount)
		.Select(row => SolveRow(matrix.GetRow(row).ToList(), unknownCols, possibleValues))
		.ToList();

	if (results.Any(r => Math.Abs(r - Math.Round(r)) > 0.0000000000000000001m))
	{
		return -1;
	}

	var roundedResults = results.Select(r => (int)Math.Round(r)).ToList();

	if (roundedResults.Any(r => r < 0))
	{
		return -1;
	}

	return roundedResults.Concat(possibleValues).Sum();
}

decimal SolveRow(IList<decimal> row, List<int> unknownCols, ImmutableList<int> values)
{
	var result = row[^1];
	for (int i = 0; i < unknownCols.Count; i++)
	{
		result -= row[unknownCols[i]] * values[i];
	}

	return result;
}

ImmutableList<ImmutableList<int>> GenerateCombinations(ImmutableList<ImmutableList<int>> current, ImmutableList<int> toProcess)
{
	if (toProcess.IsEmpty)
	{
		return current;
	}

	var nextRange = Enumerable.Range(0, toProcess[0] + 1).ToImmutableList();

	var next = current.IsEmpty
		? nextRange.Select(ImmutableList<int>.Empty.Add).ToImmutableList()
		: current.SelectMany(c => nextRange.Select(n => c.Add(n))).ToImmutableList();

	return GenerateCombinations(next, toProcess.RemoveAt(0));
}

record Input(string LightPattern, List<List<int>> ButtonLights, List<int> JoltagePattern)
{
	public int LightPatternInt => ConvertPatternToInt(LightPattern);
	public List<int> ButtonLightsInt => ButtonLights.Select(ConvertToInt).ToList();

	public decimal[,] GetJoltageCalculationMatrix()
	{
		return JoltagePattern
			.Select((j, jPos) => ButtonLights.Select(pos => pos.Contains(jPos) ? 1 : 0).Concat([j]).ToList())
			.ToList()
			.To2DArray<int, decimal>();
	}

	public int GetMaxAvailableNumberPresses(int buttonPos)
	{
		return ButtonLights[buttonPos]
			.Select(n => JoltagePattern[n])
			.Min();
	}

	internal bool CheckAnswer(Dictionary<int, int> answer)
	{
		var joltages = JoltagePattern.ToList();
		for (int buttonNr = 0; buttonNr < ButtonLights.Count; buttonNr++)
		{
			foreach (var pos in ButtonLights[buttonNr])
			{
				joltages[pos] -= answer[buttonNr];
			}
		}

		return joltages.All(v => v == 0);
	}

	int ConvertPatternToInt(string pattern)
		=> Convert.ToInt32(new string(pattern.Replace('.', '0').Replace('#', '1').Reverse().ToArray()), 2);

	int ConvertToInt(IEnumerable<int> numbers)
		=> numbers.Aggregate(0, (intValue, n) => intValue | (1 << n));
}

record LightToggleState(int Current, int FlipCount, Input Settings) : SearchState<int>
{
	public int Key => Current;
	public int Length => FlipCount;
	public int ScoreHeuristic => FlipCount;
	public bool IsFinal => Settings.LightPatternInt == Current;

	public IEnumerable<SearchState<int>> GetNextStates()
	{
		return Settings.ButtonLightsInt.Select(f => new LightToggleState(Current ^ f, FlipCount + 1, Settings));
	}
}