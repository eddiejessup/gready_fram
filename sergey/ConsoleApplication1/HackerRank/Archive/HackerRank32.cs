using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ConsoleApplication1.HackerRank
{
	// https://www.hackerrank.com/challenges/game-of-stones-1

	class HackerRank32
	{
		public void Go()
		{
			var T = HR.ReadInt();
			for (var t = 0; t < T; t++)
			{
				var n = HR.ReadInt();
				var gr = new int[n + 1];
				gr[0] = 0;
				gr[1] = 0;
				for (var i = 2; i <= n; i++)
				{
					var list = new List<int>();
					list.Add(gr[i - 2]);
					if (i >= 3) list.Add(gr[i - 3]);
					if (i >= 5) list.Add(gr[i - 5]);

					var min = 0;
					while (true)
					{
						if (list.Contains(min)) min++;
						else break;
					}

					gr[i] = min;
				}
				Console.WriteLine(gr[n] != 0 ? "First" : "Second");
			}
		}
	}
}
