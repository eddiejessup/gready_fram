using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank6
	{
		public void Go()
		{
			//var time = "07:05:45PM";
			//var time = "12:00:00AM";
			var time = "12:01:02PM";

			var hh = int.Parse(time.Substring(0, 2));
			var mm = int.Parse(time.Substring(3, 2));
			var ss = int.Parse(time.Substring(6, 2));
			var mod = time.Substring(8);

			if (hh != 12)
			{
				if (mod == "PM") hh += 12;
			}
			else 
			{
				if (mod == "AM") hh = 0;
				else hh = 12;
			}

			Console.WriteLine("{0:00}:{1:00}:{2:00}", hh, mm, ss);
		}
	}
}
