using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApplication1.Helpers
{
	public static class TimeMeasureHelper
	{
		public static IEnumerable<T> ShowTimers<T>(this IEnumerable<T> seq)
		{
			var timer = Stopwatch.StartNew();
			var i = 0;

			var statCount = 0;
			var statTime = 0L;

			foreach (var item in seq)
			{
				timer.Stop();

				ShowElapsedToConsole(i, timer);

				statCount++;
				statTime += timer.ElapsedMilliseconds;

				yield return item;

				timer.Restart();

				i++;
			}
			
			timer.Stop();

			Console.WriteLine("Total items: {0}, average time {1} ms", statCount, (double)statTime / statCount);
		}

		public static void ShowElapsedToConsole(int i, Stopwatch timer)
		{
			Console.WriteLine(string.Concat("Item #", i.ToString(), " Elapsed ms: ", timer.ElapsedMilliseconds));
		}

		public static void ShowElapsedToConsole(Stopwatch timer)
		{
			Console.WriteLine(string.Concat("Elapsed ms: ", timer.ElapsedMilliseconds));
		}
	}
}