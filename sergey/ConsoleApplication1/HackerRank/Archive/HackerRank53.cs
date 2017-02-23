using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.MyMath;

// https://www.hackerrank.com/challenges/xoring-ninja

namespace ConsoleApplication1.HackerRank
{
	class HackerRank53
	{
		const ulong MOD = 1000000007ul;
		//const ulong MOD = 13ul;

		public void Go()
		{
			//Console.WriteLine(Solve(new ulong[] { 1, 2, 3 }));
			//Console.WriteLine(CountSubsetsWhereBitIsOn(new ulong[] { 1, 2, 3 }, 2));
			Compare();
		}

		public void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 20000; t++)
			{
				var len = rnd.Next(1, 8);
				var A = RandomGenerator.RandomUlongArray(rnd, len, 0, 10);
				var brute = SolveBrute(A);
				var actual = Solve(A);

				if (actual != brute)
				{
					Console.WriteLine(new { t, A = A.Join(), brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public ulong Solve(ulong[] A)
		{
			var sum = 0ul;
			var pow = 1ul;

			for (var i = 0; i <= 64; i++)
			{
				var cswbih = CountSubsetsWhereBitIsOn(A, i);
				var addme = pow * cswbih % MOD;
				sum = (sum + addme) % MOD;
				pow <<= 1;
			}

			return sum;
		}

		public ulong CountSubsetsWhereBitIsOn(ulong[] A, int i)
		{
			var n = (ulong)A.Length;

			var factorials = new ulong[n + 1];
			factorials[0] = 1;
			for (var j = 1ul; j <= n; j++)
				factorials[j] = factorials[j - 1] * j % MOD;

			var factInverses = new ulong[factorials.Length];
			for (var j = 0; j < factInverses.Length; j++)
				factInverses[j] = ModularInverse(factorials[j], MOD);

			var count = 0ul;

			var mask = 1ul << i;
			var Bi = (ulong)A.Count(a => (a & mask) > 0);
			var Di = n - Bi;

			if (Bi == 0) return 0ul;

			var dsum = 0ul;

			for (var j = (n % 2 == 1 ? n : (n-1)); j >= 1; j -= 2)
			{
				var cjbi = Bi >= j
					? (factorials[Bi] * factInverses[j] % MOD) * factInverses[Bi - j] % MOD
					: 0;

				var d2 = (Di >= n - j)
					? (factorials[Di] * factInverses[n - j] % MOD) * factInverses[Di - n + j] % MOD
					: 0;

				var d1 = (Di + 1 >= n - j && n - j >= 1)
					? (factorials[Di] * factInverses[n - j - 1] % MOD) * factInverses[Di - n + j + 1] % MOD
					: 0;

				dsum = (dsum + d1 + d2) % MOD;

				count = (count + cjbi * dsum % MOD) % MOD;

				if (j == 1) break;
			}

			return count % MOD;
		}

		public ulong CountSubsetsWhereBitIsOn_Brute(ulong[] A, int i)
		{
			var count = 0ul;

			var mask = 1ul << i;
			for (var len = 1; len <= A.Length; len++)
				foreach (var comb in new Combinations<ulong>(A, len))
					if ((comb.Aggregate(0ul, (x, y) => x ^ y) & mask) > 0)
						count++;

			return count % MOD;
		}

		public ulong SolveBrute(ulong[] A)
		{
			var sum = 0ul;

			for (var len = 1; len <= A.Length; len++)
				foreach (var comb in new Combinations<ulong>(A, len))
					sum += comb.Aggregate(0ul, (x, y) => x ^ y);

			return sum % MOD;
		}
	}
}
