using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank51
	{
		public void Go()
		{
			Compare();
		}

		public static void Compare()
		{
			var B = Solve_Brute_CreateB();
			var rnd = new Random(1337);
			for (var t = 0; t < 1000000; t++)
			{
				var L = (ulong)rnd.Next(1, 10);
				var R = (ulong)rnd.Next((int)L, 10);
				var brute = Solve_Brute(B, L, R);
				var actual = Solve(L, R);

				if (actual != brute)
				{
					Console.WriteLine(new { t, L, R, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static ulong Solve(ulong L, ulong R)
		{
			var ans = 0ul;

			for (var i = L; i <= L - L % 8 + 7; i++)
				ans ^= (i % 4 == 0) ? i : (i % 4 == 1) ? 1 : (i % 4 == 2) ? (i + 1) : 0;

			for (var i = R - R % 8; i <= R; i++)
				ans ^= (i % 4 == 0) ? i : (i % 4 == 1) ? 1 : (i % 4 == 2) ? (i + 1) : 0;

			return ans;
		}
		
		public static ulong[] Solve_Brute_CreateB()
		{
			var A = new ulong[100001];
			for (var i = 1ul; i < (ulong)A.Length; i++)
				A[i] = A[i - 1] ^ i;

			Console.WriteLine(A.Take(500).Join());
			//Console.WriteLine();
			//for (var i = 0; i < 100; i += 4)
			//	Console.WriteLine(A[i] ^ A[i + 1] ^ A[i + 2] ^ A[i + 3]);

			var B = new ulong[A.Length];
			var s = 0ul;
			for (var i = 1ul; i < (ulong)B.Length; i++)
			{
				s ^= A[i];
				B[i] = s;
			}
			return B;
		}

		public static ulong Solve_Brute(ulong[] B, ulong L, ulong R)
		{
			return B[R] ^ B[L - 1];
		}
	}
}
