namespace TicTacToe;

internal class Program
{
	private static void Main(string[] args)
	{
		int state = -1;
		int player = 1;
		bool succesfulMove = false;
		TicTacToe ticTacToe = new();

		while (state == -1)
		{
			int x;
			int y;
			ticTacToe.PrintGrid();
			succesfulMove = false;

			while (!succesfulMove)
			{
				Console.WriteLine($"Player {player}'s turn!");
				Console.Write("Enter x coordinate: ");
				x = int.Parse(Console.ReadLine()!);
				Console.Write("Enter y coordinate: ");
				y = int.Parse(Console.ReadLine()!);

				try
				{
					state = ticTacToe.ChangeField((y, x), player);
				}
				catch (ArgumentException ex)
				{
					Console.WriteLine(ex.Message);
					continue;
				}

				succesfulMove = true;
			}

			player = player == 1 ? 2 : 1;
		}

		Console.WriteLine(state == 0 ? "Stalemate" : $"Player {state} has won!");
	}
}