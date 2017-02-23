using ConsoleApplication1.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank21
	{
		public void Go()
		{
			Solve_Brute();
		}

		public void Solve_Brute()
		{
			var task = new[]
			{
				"....O..",
				"...O.O.",
				"..O...O",
				".O...O.",
				"O...O..",
				".O.O...",
				"..O....",
			};
			var R = task.Length;
			var C = task[0].Length;
			var N = 20;

			var deltas = new PointInt[] { new PointInt(0, 0), new PointInt(0, 1), new PointInt(0, -1), new PointInt(1, 0), new PointInt(-1, 0) };

			var board1 = string.Join("", task).ToCharArray();
			var board2 = new string('.', R * C).ToCharArray();

			if (N >= 2)
				for (var sec = 2; sec <= N; sec++)
				{
					if (sec % 2 == 0)
					{
						board2 = new char[R * C];
						for (var ii = 0; ii < board1.Length; ii++)
							board2[ii] = board1[ii] != 'O' ? 'O' : '.';
					}
					else if (sec % 2 == 1)
					{
						var bombs = new List<PointInt>();
						for (var i = 0; i < R; i++)
							for (var j = 0; j < C; j++)
								if (board1[i * C + j] == 'O')
									bombs.Add(new PointInt(i, j));

						for (var ii = 0; ii < board1.Length; ii++)
							board1[ii] = board2[ii];

						foreach (var bomb in bombs)
							foreach (var delta in deltas)
							{
								var explosion = bomb + delta;
								if (0 <= explosion.X && explosion.X < R && 0 <= explosion.Y && explosion.Y < C)
								{
									var ii = explosion.X * C + explosion.Y;
									board1[ii] = '.';
									board2[ii] = '.';
								}
							}

						var temp = board1;
						board1 = board2;
						board2 = temp;
					}

					Console.WriteLine($"{sec}");
					for (var i = 0; i < R; i++)
					{
						var r = new char[C];
						for (var j = 0; j < C; j++)
							r[j] = (board1[i * C + j] == 'O' || board2[i * C + j] == 'O') ? 'O' : '.';
						Console.WriteLine(new string(r));
					}
					Console.WriteLine();
				}
		}
	}
}
