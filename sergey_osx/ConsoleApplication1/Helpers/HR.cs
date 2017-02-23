using System;
using System.Linq;

namespace ConsoleApplication1.Helpers
{
	public static class HR
	{
		public static ulong[] ReadUlongArray()
		{
			return Console.ReadLine().Split().Select(ulong.Parse).ToArray();
		}

		public static int[] ReadIntArray()
		{
			return Console.ReadLine().Split().Select(int.Parse).ToArray();
		}

		public static long[] ReadLongArray()
		{
			return Console.ReadLine().Split().Select(long.Parse).ToArray();
		}

		public static ulong ReadUlong()
		{
			return ulong.Parse(Console.ReadLine());
		}

		public static int ReadInt()
		{
			return int.Parse(Console.ReadLine());
		}
	}
}
