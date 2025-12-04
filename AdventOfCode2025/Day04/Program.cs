using Utils;

var map = File.ReadAllLines("input.txt")
	.ToCharArray();

var toRemove = GetPackagesThatCanBeRemoved(map);
Console.WriteLine("1: " + toRemove.Count());

int totalRemovable = 0;
while(toRemove.Count > 0)
{
	map.SetItems(toRemove.Select(i => i.Point), '.');
	totalRemovable += toRemove.Count;
	toRemove = GetPackagesThatCanBeRemoved(map);
}

Console.WriteLine("2: " + totalRemovable);


static List<ArrayItem<char>> GetPackagesThatCanBeRemoved(char[,] map)
{
	return map.SelectItems()
		.Where(i => i.Item == '@')
		.Where(i => map.GetNeighbourItems(i.Point, true).Where(i => i.Item == '@').Count() < 4)
		.ToList();
}