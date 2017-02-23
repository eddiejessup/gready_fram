using System.Collections.Generic;

namespace ConsoleApplication1.Helpers
{
	public static class GameTheoryHelper
	{
		public static int SpragueGrundy(ICollection<int> list)
		{
			var min = 0;
			while (true)
			{
				if (list.Contains(min))
					min++;
				else
					return min;
			}
		}

		public static ulong SpragueGrundy(ICollection<ulong> list)
		{
			var min = 0ul;
			while (true)
			{
				if (list.Contains(min))
					min++;
				else
					return min;
			}
		}
	}
}
