using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank5
	{
		public void Go()
		{
			GetLargestPrimeFactor(21);

			for (var i = 2ul; i <= 50; i++)
				Console.WriteLine(new { i, factor = GetLargestPrimeFactor(i) });
		}

		public static ulong GetLargestPrimeFactor(ulong N)
		{
			var n = N;
			var factor = 1ul;
			var i = 2ul;
			while (n > 1)
			{
				if (n % i == 0)
				{
					factor = i;
					do { n = n / i; } while (n % i == 0);
				}

				i++;

				if (i * i > n)
				{
					if (n > 1) factor = n;
					break;
				}
			}

			return factor;
		}
	}
}
