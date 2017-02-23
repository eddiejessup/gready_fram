using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank4
	{
		private readonly double phi = (1 + Math.Sqrt(5)) / 2;

		public ulong[] Matrix2x2Mul(ulong[] a, ulong[] b)
		{
			return new[]
			{
				a[0] * b[0] + a[1] * b[2],
				a[0] * b[1] + a[1] * b[3],
				a[2] * b[0] + a[3] * b[2],
				a[2] * b[1] + a[3] * b[3],
			};
		}

		public ulong Fib(ulong n)
		{
			var pow = new ulong[] { 1, 1, 1, 0 };

			var result = new ulong[] { 1, 0, 0, 1 };

			for (; n != 0; n >>= 1)
			{
				if (n % 2 == 1)
					result = Matrix2x2Mul(result, pow);

				pow = Matrix2x2Mul(pow, pow);
			}

			return result[0];
		}

		public ulong GetIndex(ulong fib)
		{
			return (ulong)Math.Floor(Math.Log(fib * Math.Sqrt(5) + 0.5, phi));
		}

		public void Go()
		{
			//for (var i = 1ul; i <= 10; i++)
			//	Console.WriteLine(new { i, fib = Fib(i)});

			//for (var i = 1ul; i <= 100; i++)
			//	Console.WriteLine(new { i, index = GetIndex(i) });

			var n = 100ul;
			Console.WriteLine(new { n, result = Solve(n) });
		}

		public ulong Solve(ulong n)
		{
			if (n <= 3) return 0;

			var sum = 0ul;

			var i = 2ul;
			while (true)
			{
				var fib = Fib(i);

				if (fib >= n)
					break;

				sum += fib;
				i += 3;
			}

			return sum;
		}
	}
}
