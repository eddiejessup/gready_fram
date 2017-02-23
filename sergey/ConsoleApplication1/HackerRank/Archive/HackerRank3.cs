using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using static ConsoleApplication1.Helpers.GameTheoryHelper;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank3
	{
	//	https://www.hackerrank.com/challenges/chocolate-game

		public void Go()
		{
			//Solve(new[] { 1, 1 });
			//var arr = new[] { 1,1,7,7,12 };
			//ShowState(arr, CalcWinner(arr, true));
			//ShowAll(9, 7);
			Compare();
		}

		public static void Compare()
		{
			var knownSequences = new Dictionary<string, bool>();

			var rnd = new Random(1337);
			for (var t = 0; t < 100; t++)
			{
				var len = rnd.Next(1, 5);
				var arr = RandomGenerator.RandomIntArray(rnd, len, 1, 5).OrderBy(i => i).ToArray();
				var brute = CalcWinner(arr, true, knownSequences, true);
				var actual = Solve(arr);

				if (actual != brute)
				{
					Console.WriteLine(arr.Join());
					Console.WriteLine(new { t, brute, actual });
					throw new InvalidOperationException();
				}
			}
		}

		public static bool Solve(int[] seq)
		{
			var v = new int[seq.Length];

			for (var i = 0; i < seq.Length; i++)
				v[i] = seq[i] - (i == 0 ? 0 : seq[i - 1]);

			var xor = 0;

			for (var i = v.Length - 1; i >= 0; i -= 2)
				xor ^= v[i];

			return xor > 0;
		}

		public static void ShowAll(int maxLen, int max)
		{
			var knownSequences = new Dictionary<string, bool>();

			var vars = Enumerable.Range(1, max).ToArray();

			for (var len = 1; len <= maxLen; len++)
				foreach (var comb in new Combinations<int>(vars, maxLen, GenerateOption.WithRepetition))
				{
					var arr = comb.ToArray();
					var winner = CalcWinner(arr, true, knownSequences, true);

					knownSequences[arr.Join(" ")] = winner;

					if (!winner)
						ShowState(arr, winner);
				}
		}

		public void ShowSimpleTests()
		{
			Console.WriteLine("Expected Laurel to win:");
			foreach (var arr in new[]
			{
				new [] {1,2},
				new [] {1,3},
				new [] {2,3},
				new [] {2,4},
				new [] {2,6},
				new [] {3,6},
				new [] {1,1,1},
				new [] {1,1,3},
				new [] {1,1,4},
				new [] {1,2,2},
				new [] {1,2,4},
				new [] {1,2,5},
				new [] {2,2,2},
				new [] {2,3,3},
				new [] {2,3,6},
				new [] {1,1,2,2,3},
			})
				ShowState(arr, CalcWinner(arr, true, null, false));

			Console.WriteLine();
			Console.WriteLine("Expected Hardie to win:");
			foreach (var arr in new[]
			{
				new [] {1,1},
				new [] {2,2},
				new [] {3,3},
				new [] {5,5},
				new [] {1,1,2},
				new [] {1,2,3},
				new [] {2,3,5},
				new [] {3,3,6},
				new [] {3,5,8},
				new [] {1,1,2,2},
				new [] {1,2,2,3},
			})
				ShowState(arr, CalcWinner(arr, true, null, false));
		}

		public static void ShowState(int[] seq, bool player)
		{
			Console.WriteLine($"{(player ? "Laurel" : "Hardie")}: [{(seq.Join())}]");
		}

		public static bool CalcWinner(int[] seq, bool player, Dictionary<string, bool> knownSequences, bool cacheEnabled)
		{
			if (seq.Sum() == 0)
				return !player;

			if (cacheEnabled && seq[0] == 0)
			{
				var str = seq.SkipWhile(i => i == 0).Join(" ");
				bool knownWinner;
				if (knownSequences.TryGetValue(str, out knownWinner))
					return knownWinner;
			}

			for (var i = 0; i < seq.Length; i++)
			{
				var left = i == 0 ? 0 : seq[i - 1];

				for (var j = left; j < seq[i]; j++)
				{
					var copy = seq.ToArray();
					copy[i] = j;

					var winner = CalcWinner(copy, !player, knownSequences, cacheEnabled);
					if (winner == player)
						return winner;
				}
			}

			return !player;
		}
	}
}
