
var dialPos = 50;
var password1 = 0;
var password2 = 0;

var turns = File.ReadAllLines("input.txt")
	.Select(line => new Turn(line[0], int.Parse(line[1..])))
	.ToList();

foreach (var turn in turns)
{
	var leftClicks = turn.Degrees % 100;
	var prevPos = dialPos;
	
	password2 += turn.Degrees / 100;
	dialPos += turn.Direction == 'R' ? leftClicks : -leftClicks;

	if (turn.Direction == 'L' && dialPos <= 0)
	{
		dialPos += dialPos < 0 ? 100 : 0;
		password2 += prevPos > 0 ? 1 : 0;
	}

	password2 += dialPos / 100;
	dialPos %= 100;

	if (dialPos == 0)
	{
		password1++;
	}
}

Console.WriteLine($"1: {password1}");
Console.WriteLine($"2: {password2}");

record Turn(char Direction, int Degrees);
