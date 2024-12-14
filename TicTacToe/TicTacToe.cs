public class TicTacToe
{
	//1 is X
	//2 is O

	public TicTacToe()
	{
		Grid =
		[
			[0, 0, 0],
			[0, 0, 0],
			[0, 0, 0]
		];
	}

	public int[][] Grid { get; }

	public int ChangeField((int, int) coordinate, int newState)
	{
		if (newState != 0 && newState != 1 && newState != 2)
		{
			throw new ArgumentException($"newState must be 0, 1 or 2 but it was {newState}");
		}

		if (coordinate.Item1 > 2 || coordinate.Item2 > 2 || coordinate.Item1 < 0 || coordinate.Item2 < 0)
		{
			throw new ArgumentException("Coordinate was out of bounds");
		}

		if (Grid[coordinate.Item1][coordinate.Item2] != 0)
		{
			throw new ArgumentException("The coordinate already has a non-zero value");
		}

		Grid[coordinate.Item1][coordinate.Item2] = newState;
		return checkIfPlayerHasWon();
	}

	private int checkIfPlayerHasWon()
	{
		int winningPlayer;

		foreach (int[] row in Grid)
		{
			winningPlayer = returnIfEqual(row[0], row[1], row[2]);

			if (winningPlayer is not 0 and not -1)
			{
				return winningPlayer;
			}
		}

		for (int i = 0; i < Grid[0].Length; i++)
		{
			winningPlayer = returnIfEqual(Grid[0][i], Grid[1][i], Grid[2][i]);

			if (winningPlayer is not 0 and not -1)
			{
				return winningPlayer;
			}
		}

		winningPlayer = returnIfEqual(Grid[0][0], Grid[1][1], Grid[2][2]);

		if (winningPlayer is not 0 and not -1)
		{
			return winningPlayer;
		}

		winningPlayer = returnIfEqual(Grid[0][2], Grid[1][1], Grid[2][0]);

		if (winningPlayer is not 0 and not -1)
		{
			return winningPlayer;
		}

		int fullRows = 0;

		foreach (int[] row in Grid)
		{
			if (row[0] is not 0 && row[1] is not 0 && row[2] is not 0)
			{
				fullRows++;
			}
			else
			{
				break;
			}
		}

		if (fullRows == 3)
		{
			return 0;
		}

		return -1;
	}

	private int returnIfEqual(int val1, int val2, int val3)
	{
		if (val1 == val2 && val2 == val3)
		{
			return val1;
		}

		return -1;
	}

	public void PrintGrid()
	{
		foreach (int[] row in Grid)
		{
			Console.WriteLine((row[0] != 0 ? row[0] == 1 ? "X" : "O" : " ") +
			                  (row[1] != 0 ? row[1] == 1 ? "X" : "O" : " ") +
			                  (row[2] != 0 ? row[2] == 1 ? "X" : "O" : " "));
		}
	}
}