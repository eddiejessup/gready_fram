using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank13
	{
		public void Go()
		{
			Solve(new[] { 3, 2, 1 });

			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var len = rnd.Next(1, 20);
				var arr = Enumerable.Range(1, len).OrderBy(i => rnd.NextDouble()).ToArray();

				var expected = Solve_Ok(arr.ToArray());
				var actual = Solve(arr.ToArray());

				if (expected != actual)
				{
					Console.WriteLine(new { expected, actual });
					Console.WriteLine(arr.Join(", "));
					throw new InvalidOperationException();
				}
			}
		}

		static ulong Solve(int[] q)
		{
			var sum = 0ul;

			var indexes = new int[q.Length + 1];
			for (var i = 0; i < q.Length; i++)
				indexes[q[i]] = i;

			for (var i = 0; i < q.Length; i++)
			{
				var first = q[i] - i;

				if (first == 1)
				{
				}

				else if (first == 2)
				{
					sum += 1ul;

					var i1 = indexes[1 + i];
					var v1 = q[i1] + 1;

					q[i1] = v1;
					indexes[v1] = i1;
				}

				else if (first == 3)
				{
					sum += 2ul;

					var i1 = indexes[1 + i];
					var i2 = indexes[2 + i];

					var v1 = q[i1] + 1;
					var v2 = q[i2] + 1;

					q[i1] = v1;
					indexes[v1] = i1;

					q[i2] = v2;
					indexes[v2] = i2;
				}

				else
				{
					return ulong.MaxValue;
				}
			}

			return sum;
		}

		static ulong Solve_Ok2(int[] q)
		{
			var sum = 0ul;

			for (var i = 0; i < q.Length; i++)
			{
				var first = q[i];

				if (first > 3) return ulong.MaxValue;

				var df = first - 1;

				sum += (ulong)df;
				for (var j = i + 1; j < q.Length; j++)
					if (q[j] > df)
						q[j] -= 1;
			}

			return sum;
		}

		static ulong Solve_Ok(int[] q)
		{
			var sum = 0ul;

			for (var i = 0; i < q.Length; i++)
			{
				var first = q[i];

				if (first == 1)
				{
					for (var j = i + 1; j < q.Length; j++)
						q[j] -= 1;
				}

				else if (first == 2)
				{
					sum += 1ul;
					for (var j = i + 1; j < q.Length; j++)
						if (q[j] > 1)
							q[j] -= 1;
				}

				else if (first == 3)
				{
					sum += 2ul;
					for (var j = i + 1; j < q.Length; j++)
						if (q[j] > 2)
							q[j] -= 1;
				}

				else
				{
					return ulong.MaxValue;
				}
			}

			return sum;
		}
	}
}
