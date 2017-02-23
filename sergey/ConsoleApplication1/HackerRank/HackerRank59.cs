using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank59
	{
		public void Go()
		{
			Example1();
		}

		public static void Performance()
		{
			var lines = new string[50];
			for (var i = 0; i < lines.Length; i++)
				lines[i] = new string('.', lines.Length);
			Solve(lines);
		}

		public static void Example1()
		{
			//var lines = new[]
			//{
			//	".*.*.",
			//	"*...*",
			//	".....",
			//	"*...*",
			//	".*.*.",
			//};

			var lines = new[]
			{
				".....",
				".....",
				".....",
				".....",
				".....",
			};

			Solve(lines);
		}

		public static void Solve(string[] lines)
		{
			var n = lines.Length;

			var board = new char[n, n];
			for (var i = 0; i < n; i++)
			{
				var line = lines[i];
				for (var j = 0; j < n; j++)
					board[i, j] = line[j];
			}

			if (n == 1)
			{
				Console.WriteLine(0);
				return;
			}

			var ans = 0;

			for (var i = 0; i < n; i++)
				for (var j = 0; j < n; j++)
				{
					if (board[i, j] == '.')
					{
						var min = int.MaxValue;
						for (var y = 0; y < n; y++)
							for (var x = 0; x < n; x++)
								if (board[x, y] == '*')
									min = Math.Min(min, (x - j) * (x - j) + (y - i) * (y - i));

						//Console.WriteLine(new {i, j, min}.ToString());

						var cand = (int)Math.Round(Math.Sqrt(min));

						if (cand * cand >= min) cand--;

						cand = Math.Min(cand, i);
						cand = Math.Min(cand, j);
						cand = Math.Min(cand, n - i - 1);
						cand = Math.Min(cand, n - j - 1);

						for (var y = 0; y < n; y++)
							for (var x = 0; x < n; x++)
								if (board[x, y] == '*')
								{
									var dist2 = (x - j) * (x - j) + (y - i) * (y - i);

									var dist = (int)Math.Round(Math.Sqrt(dist2));

									if (dist * dist <= dist2) dist++;

									if (dist == cand)
									{
										// Console.WriteLine(new {i, j, x, y, dist, dist2}.ToString());
										cand--;
									}
								}

						ans = Math.Max(ans, cand);
					}
				}

			Console.WriteLine(ans);
		}
	}
}
