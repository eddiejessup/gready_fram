using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank62
	{
		public void Go()
		{
			Console.WriteLine(Solve(new long[]{ 1, 2, 3, 4, 5 }).Select(p => $"({p[0]}, {p[1]})").Join(Environment.NewLine));
			Console.WriteLine();
			Console.WriteLine(Solve(new long[]{ 1, 2, 1, 2 }).Select(p => $"({p[0]}, {p[1]})").Join(Environment.NewLine));
			Console.WriteLine();
			Console.WriteLine(Solve(new long[] { 10, 2, 11 }).Select(p => $"({p[0]}, {p[1]})").Join(Environment.NewLine));
			Console.WriteLine();
			Console.WriteLine(Solve(new long[] { 20, 2, 22, 2 }).Select(p => $"({p[0]}, {p[1]})").Join(Environment.NewLine));
		}

		public static double[][] Solve(long[] llong)
		{
			const int iterations = 10000;
			const double epsilon = 0.0000000000001d;

			var L = new double[llong.Length];
			for (var i = 0; i < L.Length; i++)
				L[i] = llong[i];

			var ans = new double[llong.Length][];
			ans[0] = new double[] { 0, 0 };
			ans[1] = new double[] { 0, L[0] };

			var maxI = 0;
			var max = L[0];
			for (var i = 1; i < L.Length; i++)
				if (L[i] > max)
				{
					maxI = i;
					max = L[i];
				}

			var isTricky = false;

			var R = FindR(iterations, epsilon, L, -1, 2 * Math.PI);
			if (R < 0)
			{
				isTricky = true;
				R = FindR(iterations, epsilon, L, maxI, 0d);
			}

			var RR = 2d * R;
			var a = Math.PI - Math.Asin(L[0] / RR);
			var dy = L[0] / 2;
			var dx = Math.Sqrt(R * R - dy * dy);

			for (var i = 1; i < L.Length - 1; i++)
			{
				var da = 2d * Math.Asin(L[i] / RR);

				if (isTricky && i == maxI)
					a += da;
				else
					a -= da;

				ans[i + 1] = new double[]
				{
					dx + Math.Cos(a) * R,
					dy + Math.Sin(a) * R
				};
			}

			return ans;
		}

		public static double FindR(int iterations, double epsilon, double[] L, int minus, double target)
		{
			var left = L.Max() / 2;
			var right = L.Sum() / 2;

			var leftVal = AnglesSum(L, left, minus);
			var rightVal = AnglesSum(L, right, minus);

			if (double.IsNaN(leftVal)) throw new InvalidOperationException("leftVal NAN");
			if (double.IsNaN(rightVal)) throw new InvalidOperationException("rightVal NAN");

			if (leftVal < target) return -1;
			if (rightVal > target) return -1;

			for (var iter = 0; iter < iterations; iter++)
			{
				var middle = (left + right) / 2;
				var middleVal = AnglesSum(L, middle, minus);

				if (double.IsNaN(middleVal)) throw new InvalidOperationException("middleVal NAN");

				if (Math.Abs(middleVal - target) <= epsilon)
				{
					//Console.WriteLine(new { iter, middle });
					return middle;
				}

				if (middleVal < target)
				{
					right = middle;
				}
				else
				{
					left = middle;
				}
			}

			return (left + right) / 2;
		}

		public static double AnglesSum(double[] L, double R, int minus)
		{
			var result = 0d;

			var RR = 2d * R;

			for (var i = 0; i < L.Length; i++)
			{
				var len = L[i];

				var a = 2d * Math.Asin(len / RR);

				if (i != minus)
					result += a;
				else
					result -= a;
			}

			return minus >= 0 ? (-result) : result;
		}
	}
}
