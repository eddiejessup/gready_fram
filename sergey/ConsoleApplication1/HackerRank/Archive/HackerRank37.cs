using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.MyMath;

// https://www.hackerrank.com/challenges/longest-increasing-subsequence-arrays

namespace ConsoleApplication1.HackerRank
{
	class HackerRank37
	{
		public void Go()
		{
			//Console.WriteLine(Solve(4, 2, 1000000007ul));
		}

		public static ulong Solve(ulong m, ulong n, ulong mod)
		{
			var fact = new ulong[50001];
			fact[0] = 1;
			for (var i = 1ul; i < (ulong)fact.Length; i++)
				fact[i] = fact[i - 1] * i % mod;

			var invs = new ulong[fact.Length];
			for (var i = 0; i < invs.Length; i++)
				invs[i] = ModularInverse(fact[i], mod);

			if (m == n || n == 1) { return 1; }

			var s = 0ul;

			var powN = 1ul;
			var powN1 = ModularPow(n - 1, m - n, mod);
			var invN1 = ModularInverse(n - 1, mod);

			for (var k = 0ul; k <= m - n; k++)
			{
				var d = powN;

				d *= powN1;
				d %= mod;

				d *= fact[m - k - 1];
				d %= mod;

				d *= invs[n - 1];
				d %= mod;

				d *= invs[m - k - n];
				d %= mod;

				s += d;
				s %= mod;

				powN = (powN * n) % mod;
				powN1 = (powN1 * invN1) % mod;
			}

			return s;
		}
	}
}
