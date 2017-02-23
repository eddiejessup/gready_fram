using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.OtherTasks
{
	public class YuraMs
	{
		public void Go()
		{
			Console.WriteLine(Solve("ABC", new[] { 1, 2, 0 }));
		}

		public static string Solve(string str, int[] perm)
		{
			var result = str.ToCharArray();

			for (var i = 0; i < perm.Length; i++)
			{
				var to = perm[i];

				var tmp = result[i];
				result[i] = result[to];
				result[to] = tmp;

				Console.WriteLine(new { i, to, s = new string(result) });
			}

			return new string(result);
		}
	}
}
