using System;

// http://stackoverflow.com/questions/4236673/sample-code-for-fast-primality-testing-in-c-sharp/4236870#4236870

namespace ConsoleApplication1.Helpers
{
	public class MillerRabinPrimality
	{
		private const ulong smallThreshold = 4759123141ul;
		private static ulong[] small = new ulong[] { 2, 7, 61 };

		private const ulong normalThreshold = 341550071728321ul;
		private static ulong[] normal = new ulong[] { 2, 3, 5, 7, 11, 13, 17 };

		private static ulong[] big = new ulong[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };

		public static bool IsPrime(ulong n)
		{
			if (n == 2) return true;

			if (n < 2 || n % 2 == 0)
				return false;

			var arr =
				n < smallThreshold ? small
				: n < normalThreshold ? normal
				: big;

			ulong d = n - 1;
			int s = 0;
			while ((d & 1) == 0) { d >>= 1; s++; }
			int i, j;

			for (i = 0; i < arr.Length; i++)
			{
				ulong a = Math.Min(n - 2, arr[i]);
				ulong now = MyMath.ModularPow(a, d, n);
				if (now == 1) continue;
				if (now == n - 1) continue;
				for (j = 1; j < s; j++)
				{
					now = MyMath.ModularMul(now, now, n);
					if (now == n - 1) break;
				}
				if (j == s) return false;
			}

			return true;
		}
	}
}
