using Combinatorics.Collections;
using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank10
	{
		const double TWOPI = 2 * Math.PI;
		const double HALFPI = Math.PI / 2;
		const double EPSILON = 1e-10;

		public static ulong Solve(PointInt[] points)
		{
			var N = points.Length;

			var factors = new int[N][];

			var indices = Enumerable.Range(0, N).ToArray();

			for (var ai = 0; ai < points.Length; ai++)
			{
				var pointA = points[ai];

				var tailIndexShift = 0;

				var angles = new double[N];
				for (var bi = 0; bi < points.Length; bi++)
				{
					if (bi == ai)
					{
						angles[bi] = double.PositiveInfinity;
						continue;
					}

					var ba = points[bi] - pointA;
					var len = Math.Sqrt((ulong)ba.X * (ulong)ba.X + (ulong)ba.Y * (ulong)ba.Y);
					var angle = Math.Acos(ba.X / len);
					if (ba.Y < 0) angle = TWOPI - angle;
					angles[bi] = angle;

					if (angle < Math.PI + HALFPI - EPSILON)
						tailIndexShift++;
				}

				var sorted = indices.OrderBy(i => angles[i]).ToList();
				sorted.RemoveAt(sorted.Count - 1);

				var tailStart = 0;

				var tailSize = sorted.Count - tailIndexShift;

				var pointFactors = new int[N];
				var prevAngle = double.NegativeInfinity;

				for (var i = 0; i < sorted.Count; i++)
				{
					var itemIndex = sorted[i];
					var itemAngle = angles[itemIndex];

					while (true)
					{
						var angle = tailStart + tailIndexShift < sorted.Count
							? (itemAngle - angles[sorted[tailStart + tailIndexShift]] + TWOPI)
							: (itemAngle - angles[sorted[tailStart + tailIndexShift - sorted.Count]]);

						if (angle <= HALFPI + EPSILON) break;

						tailStart++;
						tailSize--;
					}

					if (Math.Abs(prevAngle - itemAngle) < EPSILON)
						pointFactors[itemIndex] = pointFactors[sorted[i - 1]];
					else
						pointFactors[itemIndex] += tailSize;

					tailSize++;
					prevAngle = itemAngle;
				}

				factors[ai] = pointFactors;
			}

			var sum = 0ul;
			for (var ai = 0; ai < points.Length; ai++)
				for (var bi = 0; bi < ai; bi++)
				{
					var s = (ulong)factors[ai][bi] * (ulong)factors[bi][ai];
					sum += s;
				}

			return sum;
		}

		private static double GetAnglesDiff(double a2, double a1)
		{
			var a = a2 - a1;
			if (a < TWOPI) a += TWOPI;
			if (a >= TWOPI) a -= TWOPI;
			return a;
		}

		public static int GetAngle(PointInt a, PointInt b)
		{
			var s = a.X * b.X + a.Y * b.Y;
			var ps = a.X * b.Y - a.Y * b.X;

			if (s == 0) return ps > 0 ? 90 : -90;
			if (ps == 0) return s > 0 ? 0 : 180;
			if (s > 0) return ps > 0 ? 45 : -45;
			return ps > 0 ? 135 : -135;
		}

		public void Go()
		{
			Console.WriteLine(GetAngle(new PointInt(0, 1), new PointInt(2, 2)));
		}

		public void Performance()
		{
			var random = new Random(1337);
			var N = 2318;
			var points = Enumerable.Repeat(0, N).Select(i => new PointInt(random.Next(20000) - 10000, random.Next(20000) - 10000)).Distinct().ToArray();
			Console.WriteLine(points.Length);
			Console.WriteLine(Solve(points));
		}

		public void CompareBrute()
		{
			var random = new Random(1337);
			for (var _ = 0; _ < 30000; _++)
			{
				var N = random.Next(4,10);
				var points = Enumerable.Repeat(0, N).Select(i => new PointInt(random.Next(10), random.Next(10))).Distinct().ToArray();
				var expected = (ulong)Solve_Brute(points).Count() / 2;
				var actual = Solve(points);
				if (expected != actual)
				{
					Console.WriteLine(N);
					foreach (var point in points)
						Console.WriteLine(point + ",");
					Console.WriteLine(new { expected, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public void ShowAngles()
		{
			var points = new[] { new PointInt(0, 2), new PointInt(2, 2), new PointInt(2, 0), new PointInt(0, -2), new PointInt(-2, -2) };
			foreach (var comb in new Combinations<PointInt>(points, 2, GenerateOption.WithRepetition))
			{
				var a = comb[0];
				var b = comb[1];

				Console.WriteLine(new { a, b, angle = GetAngle(a, b) });
				Console.WriteLine(new { scalar = a.X * b.X + a.Y * b.Y , pseudoscalar = a.X * b.Y - a.Y * b.X });
				Console.WriteLine();
			}
		}

		public void Example1()
		{
			var points = new[] { new PointInt(0, 0), new PointInt(0, 2), new PointInt(2, 0), new PointInt(2, 2) };
			var brute = Solve_Brute(points).ToArray();
			foreach (var n in brute)
				Console.WriteLine(n.Join());
			Console.WriteLine(new { expected = brute.Length / 2, actual = Solve(points) });
		}

		public void Example2()
		{
			var points = new[] { new PointInt(0, 0), new PointInt(0, 2), new PointInt(2, 0), new PointInt(2, 2), new PointInt(1, 4) };
			var brute = Solve_Brute(points).ToArray();
			foreach (var n in brute)
				Console.WriteLine(n.Join());
			Console.WriteLine(new { expected = brute.Length / 2, actual = Solve(points) });
		}

		public void Example3()
		{
			var points = new[] {
				new PointInt(0,2),
				new PointInt(5,5),
				new PointInt(0,4),
				new PointInt(2,2),
				new PointInt(0,5),
				new PointInt(-2,2),
			};
			var brute = Solve_Brute(points).ToArray();
			foreach (var n in brute)
				Console.WriteLine(n.Join());
			Console.WriteLine(new { expected = brute.Length / 2, actual = Solve(points) });
		}

		public void Example4()
		{
			var points = new[] {
				new PointInt(4,5),
				new PointInt(2,2),
				new PointInt(2,5),
				new PointInt(4,4),
				new PointInt(5,5),
			};
			var brute = Solve_Brute(points).ToArray();
			foreach (var n in brute)
				Console.WriteLine(n.Join());
			Console.WriteLine(new { expected = brute.Length / 2, actual = Solve(points) });
		}

		public void Example5()
		{
			var points = new[] {
				new PointInt(0, 0    ),
				new PointInt(0, 2    ),
				new PointInt(2, 0    ),
				new PointInt(2, 2    ),
				new PointInt(4, 0    ),
				new PointInt(4, 2    ),
			};
			var brute = Solve_Brute(points).ToArray();
			foreach (var n in brute)
				Console.WriteLine(n.Join());
			Console.WriteLine(new { expected = brute.Length / 2, actual = Solve(points) });
		}

		public void Example6()
		{
			var points = new[] {
				new PointInt(0, 0),
				new PointInt(1, 0),
				new PointInt(2, 0),
				new PointInt(0, 1),
				new PointInt(1, 1),
				new PointInt(2, 1),
				new PointInt(0, 2),
				new PointInt(1, 2),
				new PointInt(2, 2),
			};
			var brute = Solve_Brute(points).ToArray();
			foreach (var n in brute)
				Console.WriteLine(n.Join());
			Console.WriteLine(new { expected = brute.Length / 2, actual = Solve(points) });
		}

		public IEnumerable<int[]> Solve_Brute(PointInt[] points)
		{
			// O(N^4)
			// Количество нужно потом делить на 2

			var cnt = 0;

			var indexes = Enumerable.Range(0, points.Length).ToArray();

			for (var a = 0; a < indexes.Length; a++)
				for (var b = 0; b < indexes.Length; b++)
					if (a != b)
					{
						var ab = points[b] - points[a];

						for (var c = 0; c < indexes.Length; c++)
							if (c != a && c != b)
							{
								var ac = points[c] - points[a];
								var cab = GetAngle(ab, ac);

								if (-90 <= cab && cab < 0)
									for (var d = 0; d < indexes.Length; d++)
										if (d != a && d != b && d != c)
										{
											var bd = points[d] - points[b];
											var ba = -ab;
											var dba = GetAngle(ba, bd);

											if (-90 <= dba && dba < 0)
											{
												yield return new[] { c, a, b, d };
												cnt++;
											}
										}
							}
					}
		}
	}
}
