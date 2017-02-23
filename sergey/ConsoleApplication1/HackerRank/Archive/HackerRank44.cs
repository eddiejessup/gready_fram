using Combinatorics.Collections;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.HackerRank
{
	class HackerRank44
	{
		public void Go()
		{
			//foreach (var seq in Reduce_Rec(new long[] { 1, 2, 3, 1, 2, 3, 1, 2, 4, 1, 4, 1, 1, 1, 2 }.ToList(), null))
			//foreach (var seq in Reduce_Rec(new long[] { 1,1,1,1,1,1,2,1,1,1,3,1,4,1,1,1,1,1,1,1,1 }.ToList(), null))
			//	Console.WriteLine(seq.Join());
			//Console.WriteLine(Solve(new long[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 3, 1, 4, 1, 1, 1, 1, 1, 1, 1, 1 }).Join());
			Compare();
		}

		public static void Performance()
		{
			// TODO: тест на много 1111111111111111

			var n = 10000;
			var maxX = 4;

			var rnd = new Random(1337);

			var commands = new string[n + 1];
			for (var i = 1; i < commands.Length; i++)
			{
				var type = rnd.NextDouble() >= 0.5 ? '+' : '-';
				if (type == '+')
					commands[i] = string.Concat(type, " ", rnd.Next(maxX));
				else
					commands[i] = "-";
			}

			var timer = Stopwatch.StartNew();
			Console.WriteLine(Solve(commands).Count());
			Console.WriteLine("Solve time: " + timer.ElapsedMilliseconds);
		}

		public static void Compare()
		{
			var n = 15;
			var maxX = 5;

			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var commands = new string[n + 1];
				for (var i = 1; i < commands.Length; i++)
				{
					var type = rnd.NextDouble() >= 0.5 ? '+' : '-';
					if (type == '+')
						commands[i] = string.Concat(type, " ", rnd.Next(maxX));
					else
						commands[i] = "-";
				}

				var brute = Solve_Brute(commands).ToArray();
				var actual = Solve(commands).ToArray();

				if (!actual.SequenceEqual(brute))
				{
					Console.WriteLine("Brute:");
					Console.WriteLine(brute.Join());
					Console.WriteLine("Actual:");
					Console.WriteLine(actual.Join());
					Console.WriteLine("Commands:");
					Console.WriteLine(commands.Join(Environment.NewLine));
					Console.WriteLine(new { t });
					throw new InvalidOperationException();
				}
			}
		}

		public static IEnumerable<long> Solve(string[] commands)
		{
			var level = new SequenceLevel();
			var solutions = new Stack<long>();
			for (var t = 0; t < commands.Length - 1; t++)
			{
				var command = commands[t + 1];
				var sol = 0L;
				if (command[0] == '+')
				{
					var x = long.Parse(command.Substring(2));
					//var shortcut = level.Seq.Count > 0 && x == level.Seq[(int)solutions.Peek()] ? (long?)(solutions.Peek() + 1) : null;
					level.AddLetter(x);
					sol = Solve_Reduce_Rec(level);
					solutions.Push(sol);

					//Console.WriteLine();
					//var lll = level;
					//while (lll.Seq.Count > 0)
					//{
					//	Console.WriteLine(lll.Seq.Join() + " -> " + CalcMinLen(lll.lengths));
					//	lll = lll.DeeperLevel;
					//}
				}
				else if (level.Seq.Count > 0)
				{
					level.RemoveLastLetter();
					solutions.Pop();
					sol = solutions.Count > 0 ? solutions.Peek() : 0;
				}

				yield return Solve_Reduce_Rec(level);
			}
		}

		public static int CalcMinLen(List<int> lengths)
		{
			var minLen = int.MaxValue;
			var len = 0;
			for (var i = 0; i < lengths.Count; i++)
				if (lengths[i] == 0)
					len++;
				else
				{
					minLen = Math.Min(minLen, len);
					len = 0;
				}
			return minLen == int.MaxValue ? -1 : minLen;
		}

		public static IEnumerable<List<long>> GroupByA(List<long> seq, long a)
		{
			var list = new List<long>();
			var one = new List<long>();
			one.Add(0);
			foreach (var s in seq)
			{
				if (s == a)
					list.Add(s);
				else
				{
					if (list.Count > 0)
					{
						yield return list;
						list.Clear();
					}

					one[0] = s;
					yield return one;
				}
			}
			if (list.Count > 0) yield return list;
		}

		public class SequenceLevel
		{
			public List<long> Seq = new List<long>();
			public LettersMapRoot LettersMap = new LettersMapRoot();
			public int Level = 0;
			public List<long> Tag = new List<long>();
			public SequenceLevel DeeperLevel => deeperLevel ?? (deeperLevel = new SequenceLevel { Level = Level + 1 });

			private SequenceLevel deeperLevel;
			private long a;
			public List<int> lengths = new List<int>();
			private int minLength = -1;

			public void AddLetter(long letter)
			{
				if (Seq.Count == 0)
				{
					a = letter;
					Seq.Add(letter);
				}
				else
				{
					if (letter == a)
					{
						if (Tag.Count == 0)
						{
							var minLen = CalcMinLen(lengths);
							if (minLen != minLength && DeeperLevel.Seq.Count > 0)
							{
								var deepA = DeeperLevel.Seq[0];
								var deepSeq = DeeperLevel.Seq.Take(DeeperLevel.Seq.Count - DeeperLevel.Tag.Count).ToList();
								var deepTag = DeeperLevel.Tag;
								var letterMap = DeeperLevel.LettersMap;
								deeperLevel = new SequenceLevel { Level = Level + 1, LettersMap = letterMap };

								foreach (var list in GroupByA(deepSeq, deepA))
								{
									if (list[0] == deepA)
										for (var i = 0; i < list.Count; i++)
											deeperLevel.AddLetter(deepA);
									else
										foreach (var l in list)
											deeperLevel.AddLetter(l);
								}

								foreach (var l in deepTag) deeperLevel.AddLetter(l);

								minLength = minLen;
							}
						}

						var b = LettersMap.GetMap(Tag);
						lengths.Add(Tag.Count);
						Tag.Clear();
						DeeperLevel.AddLetter(b);
					}
					else
					{
						Tag.Add(letter);
					}
					Seq.Add(letter);
				}
			}

			public void RemoveLastLetter()
			{
				if (Seq.Count == 0) return;

				if (deeperLevel != null && Tag.Count == 0 && deeperLevel.Seq.Count > 0)
				{
					lengths.RemoveAt(lengths.Count - 1);
					DeeperLevel.RemoveLastLetter();
				}

				var isRemoveA = Seq.Last() == a;

				Seq.RemoveAt(Seq.Count - 1);

				if (!isRemoveA)
					Tag.RemoveAt(Tag.Count - 1);

				if (Seq.Count > 0 && isRemoveA)
				{
					var aIndex = Seq.LastIndexOf(a);
					Tag = Seq.Skip(aIndex + 1).ToList();
				}
			}

			public long TranslateLength(List<long> seq, long len)
			{
				var result = 0L;
				for (var i = 0; i < (int)len; i++)
					result += lengths[i] + 1;
				return result;
			}
		}

		public static long Solve_Reduce_Rec(SequenceLevel level)
		{
			var seq = level.Seq;

			if (seq.Count <= 1) return 0;
			if (seq.AllSame()) return seq.Count - 1;
			if (seq.Count == 2) return 0;

			var nextSeq = level.DeeperLevel.Seq;

			if (level.DeeperLevel.Seq.Count == 0)
				return 0;

			if (seq[0] == seq.Last())
			{
				var sub = Solve_Reduce_Rec(level.DeeperLevel);
				return 1 + level.TranslateLength(nextSeq, sub);
			}

			{
				var tagLen = 1 + level.Tag.Count;

				var k = 0L;

				var firstLetter = new List<long>();
				for (var i = 1; i < seq.Count; i++)
					if (seq[i] == seq[0])
						break;
					else
						firstLetter.Add(seq[i]);

				var isPrefix = true;
				for (var i = 0; i < level.Tag.Count; i++)
					if (i >= firstLetter.Count || firstLetter[i] != level.Tag[i])
					{
						isPrefix = false;
						break;
					}
				if (isPrefix)
					k = tagLen;

				foreach (var repl in level.LettersMap.GetAllContinuations(level.Tag))
				{
					level.DeeperLevel.AddLetter(repl.Value);

					var sub = Solve_Reduce_Rec(level.DeeperLevel);
					var subLen = level.TranslateLength(nextSeq, sub);
					k = Math.Max(subLen - repl.Length - 1 + tagLen, k);

					level.DeeperLevel.RemoveLastLetter();
				}

				return k;
			}
		}

		public class LettersMapRoot
		{
			public long GetMap(List<long> num)
			{
				var node = root;
				foreach (var n in num)
				{
					LettersMapNode nextNode;
					if (!node.Nodes.TryGetValue(n, out nextNode))
					{
						nextNode = new LettersMapNode();
						node.Nodes.Add(n, nextNode);
					}
					node = nextNode;
				}

				if (node.Value == -1)
				{
					node.Value = maxValue;
					node.Length = num.Count;
					maxValue++;
				}

				return node.Value;
			}

			public IEnumerable<LettersMapNode> GetAllContinuations(List<long> num)
			{
				var node = root;
				foreach (var n in num)
				{
					LettersMapNode nextNode;
					if (!node.Nodes.TryGetValue(n, out nextNode))
						yield break;
					node = nextNode;
				}

				var queue = new Queue<LettersMapNode>();
				queue.Enqueue(node);
				while (queue.Count > 0)
				{
					var deq = queue.Dequeue();
					if (deq.Value >= 0)
						yield return deq;
					foreach (var child in deq.Nodes)
						queue.Enqueue(child.Value);
				}
			}

			private long maxValue = 1;
			private LettersMapNode root = new LettersMapNode();
		}

		public class LettersMapNode
		{
			public long Value = -1;
			public long Length = -1;
			public Dictionary<long, LettersMapNode> Nodes = new Dictionary<long, LettersMapNode>();
		}

		public static void ShowData()
		{
			var arr = ArrayHelper.Create(4, i => (long)i);
			foreach (var comb in new Variations<long>(arr, arr.Length, GenerateOption.WithRepetition))
				Console.WriteLine($"{comb.Join()} -> {CalcK_Brute(comb.ToList())}");
		}

		public static void Example1()
		{
			foreach (var i in Solve_Brute(@"9
+ 1
+ 1
+ 2
-
+ 1
-
-
-
+ 3".Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)))
				Console.WriteLine(i);
		}

		public static IEnumerable<List<long>> Reduce_Rec(List<long> seq, Dictionary<long, int> symbolLengths)
		{
			yield return seq;

			if (seq.Count <= 1) yield break;
			if (seq.Count == 2) yield break;
			if (seq.AllSame()) yield break;

			if (symbolLengths == null)
			{
				symbolLengths = new Dictionary<long, int>();
				foreach (var i in seq)
					symbolLengths[i] = 1;
			}

			var a = seq[0];
			var nextSeq = new List<long>();
			var map = new Dictionary<string, long>();
			var b = 1;
			var num = new List<long> { a };
			var nextSymbolLengths = new Dictionary<long, int>();
			for (var i = 1; i <= seq.Count; i++)
			{
				if (i == seq.Count || seq[i] == a)
				{
					var key = num.Join();
					if (!map.ContainsKey(key))
					{
						map.Add(key, b);
						nextSymbolLengths[b] = num.Sum(j => symbolLengths[j]);
						b++;
					}
					var val = map[key];
					nextSeq.Add(val);
					num.Clear();
					num.Add(a);
				}
				else
				{
					num.Add(seq[i]);
				}
			}

			foreach (var sub in Reduce_Rec(nextSeq, nextSymbolLengths))
				yield return sub;
		}


		public static IEnumerable<long> Solve_Old(string[] commands)
		{
			var seq = new List<long>();
			for (var t = 0; t < commands.Length - 1; t++)
			{
				var command = commands[t + 1];
				if (command[0] == '+')
				{
					var x = long.Parse(command.Substring(2));
					seq.Add(x);
				}
				else
				{
					seq.RemoveAt(seq.Count - 1);
				}

				yield return Solve_Reduce_Rec_Old(seq);
			}
		}

		public static long Solve_Reduce_Rec_Old(List<long> seq)
		{
			if (seq.Count <= 1) return 0;
			if (seq.AllSame()) return seq.Count - 1;
			if (seq.Count == 2) return 0;

			var a = seq[0];
			var nextSeqKeys = new List<string>();
			var nextSeq = new List<long>();
			var map = new Dictionary<string, long>();
			var b = 1;
			var num = new List<long>();
			var nextSymbolLengths = new Dictionary<long, int>();
			for (var i = 1; i < seq.Count; i++)
			{
				if (seq[i] == a)
				{
					var key = num.Join();
					if (!map.ContainsKey(key))
					{
						map.Add(key, b);
						nextSymbolLengths[b] = 1 + num.Count;
						b++;
					}
					nextSeqKeys.Add(key);
					nextSeq.Add(map[key]);
					num.Clear();
				}
				else
				{
					num.Add(seq[i]);
				}
			}

			if (nextSeq.Count == 0)
				return 0;

			if (seq[0] == seq.Last())
			{
				var sub = Solve_Reduce_Rec_Old(nextSeq);
				return 1 + TranslateLength(nextSymbolLengths, nextSeq, sub);
			}

			{
				var tagKey = num.Join();
				var tagLen = 1 + num.Count;

				var k = 0L;

				if (nextSeqKeys[0].StartsWith(tagKey))
					k = tagLen;

				foreach (var replKvp in map)
					if (replKvp.Key.StartsWith(tagKey))
					{
						var repl = replKvp.Value;
						nextSeq.Add(repl);
						var sub = Solve_Reduce_Rec_Old(nextSeq);
						var subLen = TranslateLength(nextSymbolLengths, nextSeq, sub);
						k = Math.Max(subLen - nextSymbolLengths[repl] + tagLen, k);
						nextSeq.RemoveAt(nextSeq.Count - 1);
					}

				return k;
			}
		}

		private static long TranslateLength(Dictionary<long, int> symbolLengths, List<long> seq, long len)
		{
			var result = 0L;
			for (var i = 0; i < (int)len; i++)
				result += symbolLengths[seq[i]];
			return result;
		}


		public static IEnumerable<long> Solve_Brute(string[] commands)
		{
			var seq = new List<long>();
			var solutions = new Stack<long>();
			for (var t = 0; t < commands.Length - 1; t++)
			{
				var command = commands[t + 1];
				var sol = 0L;
				if (command[0] == '+')
				{
					var x = long.Parse(command.Substring(2));
					var shortcut = seq.Count > 0 && x == seq[(int)solutions.Peek()] ? (long?)(solutions.Peek() + 1) : null;
					seq.Add(x);
					sol = shortcut ?? CalcK_Brute(seq);
					solutions.Push(sol);
				}
				else if (seq.Count > 0)
				{
					seq.RemoveAt(seq.Count - 1);
					solutions.Pop();
					sol = solutions.Count > 0 ? solutions.Peek() : 0;
				}
				yield return sol;
			}
		}

		public static long CalcK_Brute(List<long> seq)
		{
			var maxk = 0L;
			for (var k = 0; k < seq.Count; k++)
			{
				var isOk = true;
				for (var i = 0; i < k; i++)
				{
					if (seq[i] != seq[seq.Count - k + i])
					{
						isOk = false;
						break;
					}
				}

				if (isOk) maxk = k;
			}
			return maxk;
		}

		public static IEnumerable<long> Solve(long[] arr)
		{
			var commands = new string[arr.Length + 1];
			commands[0] = arr.Length.ToString();
			for (var t = 0; t < arr.Length; t++)
				commands[t + 1] = "+ " + arr[t];
			return Solve(commands);
		}
	}
}
