using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

// https://www.hackerrank.com/challenges/whats-next

namespace ConsoleApplication1.HackerRank
{
	class HackerRank52
	{
		public void Go()
		{
			//Console.WriteLine(SolveBrute(new ulong[] { 4, 1, 3, 2, 4 }).Join());
			//Console.WriteLine(SolveBrute(new ulong[] { 2 }).Join());
			Compare();
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 20000; t++)
			{
				var len = rnd.Next(1, 8);
				var A = RandomGenerator.RandomUlongArray(rnd, len, 1, 10);
				var brute = SolveBrute(A);
				var actual = Solve(A.ToArray());

				if (!actual.SequenceEqual(brute))
				{
					Console.WriteLine(new { t, A = A.Join(), brute = brute.Join(), actual = actual.Join() });
					throw new InvalidOperationException();
				}
			}
		}

		public static List<ulong> Solve(ulong[] A)
		{
			if (A.Length == 1)
			{
				var last = A[0];
				var ans = new List<ulong> { 1, 1 };

				if (last > 1) ans.Add(last - 1);
				return ans;
			}

			if (A.Length % 2 == 1)
			{
				var prpr = A[A.Length - 3];
				var prev = A[A.Length - 2];
				var last = A[A.Length - 1];

				var ans = A.Take(A.Length - 3).ToList();

				if (prev == 1)
				{
					ans.Add(prpr + 1);
					ans.Add(1);
					if (last > 1) ans.Add(last - 1);
				}
				else
				{
					ans.Add(prpr);
					ans.Add(prev - 1);
					ans.Add(1);
					ans.Add(1);
					if (last > 1) ans.Add(last - 1);
				}

				return ans;
			}

			if (A.Length == 2)
			{
				var prev = A[A.Length - 2];
				var last = A[A.Length - 1];

				var ans = new List<ulong> { 1, last + 1 };
				if (prev > 1) ans.Add(prev - 1);
				return ans;
			}

			{
				var prrr = A[A.Length - 4];
				var prpr = A[A.Length - 3];
				var prev = A[A.Length - 2];
				var last = A[A.Length - 1];

				var ans = A.Take(A.Length - 4).ToList();

				if (prpr == 1)
				{
					ans.Add(prrr + 1);
					ans.Add(last + 1);
					if (prev > 1) ans.Add(prev - 1);
				}
				else
				{
					ans.Add(prrr);
					ans.Add(prpr - 1);
					ans.Add(1);
					ans.Add(last + 1);
					if (prev > 1) ans.Add(prev - 1);
				}

				return ans;
			}
		}

		public static List<ulong> SolveBrute(ulong[] A)
		{
			var num = BigInteger.Zero;
			var pow = BigInteger.One;
			for (var i = 0; i < A.Length; i++)
			{
				for (var j = 0ul; j < A[i]; j++)
					num *= 2;

				if (i % 2 == 0)
					num += (1ul << (int)A[i]) - 1;
			}

			var numBitsCount = num.ToByteArray().Sum(b => BitHelper.NumberOfSetBits(b));

			while (true)
			{
				num++;
				var count = num.ToByteArray().Sum(b => BitHelper.NumberOfSetBits(b));
				if (count == numBitsCount)
					break;
			}

			var str = num.ToBinaryString();

			var ans = new List<ulong>();
			var ansLast = 0ul;
			for (var i = 0; i < str.Length; i++)
			{
				if (ans.Count == 0 && ansLast == 0 && str[i] == '0') continue;

				if (ans.Count % 2 == 0 && str[i] == '1') ansLast++;

				else if (ans.Count % 2 == 1 && str[i] == '0') ansLast++;

				else if ((ans.Count % 2 == 0 && str[i] == '0') || (ans.Count % 2 == 1 && str[i] == '1'))
				{
					ans.Add(ansLast);
					ansLast = 1;
				}
			}

			if (ansLast > 0)
				ans.Add(ansLast);

			return ans;
		}
	}
}
