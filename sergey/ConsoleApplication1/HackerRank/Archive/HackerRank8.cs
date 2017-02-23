using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank8
	{
		public void Go()
		{
			var arr = new[] { 1, 2, 2 };

			var factorials = new ulong[19];
			factorials[1] = 1;
			for (var i = 2; i < factorials.Length; i++)
				factorials[i] = factorials[i - 1] * (ulong)i;

			var counts = arr.GroupBy(i => i).Select(gr => factorials[gr.Count()]).Aggregate(1ul, (x, y) => x * y);

			var Q = factorials[arr.Length] / counts;

			Console.WriteLine(Q + ".000000");

			Console.WriteLine(IsSorted(new[] { 5, 2 }));
			Console.WriteLine(IsSorted(new[] { 2, 2, 2, 5, 5 }));
		}

		public static bool IsSorted(int[] arr)
		{
			for (var i = 1; i < arr.Length; i++)
				if (arr[i] < arr[i - 1])
					return false;

			return true;
		}
	}
}


/*
        Console.ReadLine();
        var arr = Console.ReadLine().Split().Select(int.Parse).ToArray();
        
			var factorials = new ulong[19];
			factorials[1] = 1;
			for (var i = 2; i < factorials.Length; i++)
				factorials[i] = factorials[i - 1] * (ulong)i;

			var counts = arr.GroupBy(i => i).Select(gr => factorials[gr.Count()]).Aggregate(1ul, (x, y) => x * y);

			var Q = factorials[arr.Length] / counts;

			var result = (double)Q / (Q - 1) / (Q - 1);

			Console.WriteLine("{0:0.000000}", result);
 */
