using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank18
	{
		public void Go()
		{
			Solve(3, new[]
			{
				new[] {10, 2, 5},
				new[] {12, 4, 4},
				new[] {6, 2, 2},
			});
		}

		public void Solve(int t, int[][] tokens)
		{
			var money = 0;
			var wrappers = 0;

			for (int a0 = 0; a0 < t; a0++)
			{
				int n = tokens[a0][0];
				int c = tokens[a0][1];
				int m = tokens[a0][2];

				money += n;
				var eat = 0;

				var eatNow = money / c;
				eat += eatNow;
				money = money % c;
				wrappers += eatNow;

				while (wrappers >= m)
				{
					eatNow = wrappers / m;
					eat += eatNow;
					wrappers = (wrappers % m) + eatNow;
				}

				Console.WriteLine(eat);
			}
		}
	}
}
