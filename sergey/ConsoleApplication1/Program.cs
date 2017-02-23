using ConsoleApplication1.OtherTasks.Hashcode2017;
using System;
using System.Diagnostics;

namespace ConsoleApplication1
{
	public class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var timer = Stopwatch.StartNew();
				new VideosAndCaches().Go();
				Console.WriteLine("Elapsed milliseconds: " + timer.ElapsedMilliseconds);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
