using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://www.hackerrank.com/challenges/difference-and-product

namespace ConsoleApplication1.HackerRank
{
	class HackerRank49
	{
		public void Go()
		{
			Compare();
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var D = (long)rnd.Next(-10, 10);
				var P = (long)rnd.Next(-10, 10);
				var brute = SolveBrute(D, P, 10);
				var actual = Solve(D, P);

				if (actual != brute)
				{
					Console.WriteLine(new { D, P, t, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static long SolveBrute(long D, long P, long lim)
		{
			var count = 0L;
			for (var a = -lim; a <= lim; a++)
				for (var b = -lim; b <= lim; b++)
					if (a * b == P && Math.Abs(a - b) == D)
						count++;
			return count;
		}

		public static long Solve(long D, long P)
		{
			if (D < 0)
				return 0;
			else if (P == 0 && D == 0)
				return 1;
			else
			{
				var discr = D * D + 4 * P;
				var discr2 = (long)Math.Round(Math.Sqrt(discr));

				if (discr2 * discr2 != discr)
					return 0;
				else if (D == 0 || discr == 0)
					return 2;
				else
					return 4;
			}
		}
	}
}
