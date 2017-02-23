using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication1.Helpers.BinarySearchHelper;

namespace ConsoleApplication1.HackerRank
{
	// https://www.hackerrank.com/contests/w22/challenges/submask-queries-

	class HackerRank43
	{
		public void Go()
		{
			//foreach (var i in EnumerateIndices("1111")) Console.WriteLine(i);
			Performance();
		}

		public static void Performance()
		{
			var rnd = new Random(1337);

			var n = 16;
			var m = 100000;
			var commands = GenerateCommands(rnd, n, m);

			var timer = Stopwatch.StartNew();
			Console.WriteLine(Solve(n, m, commands).Count());
			Console.WriteLine("Solution time: " + timer.ElapsedMilliseconds);
		}

		public static void Compare()
		{
			var rnd = new Random(1337);
			for (var t = 0; t < 100000; t++)
			{
				var n = 5;
				var m = 30; // TODO: n,m=5,10
				var commands = GenerateCommands(rnd, n, m);

				var brute = Solve_Brute(n, m, commands).ToArray();
				var actual = Solve(n, m, commands).ToArray();

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

		class Node
		{
			public Node(int n)
			{
				NextNodes = new Node[n];
				SetTime = -1;
				Xors = new List<ulong>();
				XorTimes = new List<int>();
			}

			public Node[] NextNodes;
			public ulong Set;
			public int SetTime;
			public List<ulong> Xors;
			public List<int> XorTimes;
		}

		public static IEnumerable<string> Solve(int n, int m, string[] commands)
		{
			var root = new Node(n);
			var queue = new Queue<Node>();
			var visited = new List<Node>();

			for (var time = 0; time < commands.Length; time++)
			{
				var command = commands[time];

				var type = command[0];

				var si2 = command.LastIndexOf(' ');

				var x = si2 == 1 ? 0ul : ulong.Parse(command.Substring(2, si2 - 2));
				si2++;

				if (type == '1')
				{
					var node = root;
					for (var bi = 0; bi < n; bi++)
						if (command[si2 + bi] == '0')
						{
							if (node.NextNodes[bi] == null) node.NextNodes[bi] = new Node(n);
							node = node.NextNodes[bi];
						}
					node.Set = x;
					node.SetTime = time;
					node.Xors.Clear();
					node.XorTimes.Clear();
					for (var bi = 0; bi < n; bi++)
						node.NextNodes[bi] = null;
				}
				else if (type == '2')
				{
					var node = root;
					for (var bi = 0; bi < n; bi++)
						if (command[si2 + bi] == '0')
						{
							if (node.NextNodes[bi] == null) node.NextNodes[bi] = new Node(n);
							node = node.NextNodes[bi];
						}
					node.Xors.Add(node.Xors.Count == 0 ? x : (x ^ node.Xors.Last()));
					node.XorTimes.Add(time);
				}
				else if (type == '3')
				{
					var result = 0ul;
					var lastSetTime = -1;

					visited.Clear();
					queue.Clear();
					queue.Enqueue(root);

					while (queue.Count > 0)
					{
						var node = queue.Dequeue();

						lastSetTime = Math.Max(lastSetTime, node.SetTime);

						visited.Add(node);

						for (var bi = 0; bi < n; bi++)
							if (node.NextNodes[bi] != null && command[si2 + bi] == '0')
								queue.Enqueue(node.NextNodes[bi]);
					}

					foreach (var node in visited)
					{
						if (node.SetTime == lastSetTime)
							result ^= node.Set;

						if (node.XorTimes.Count == 1)
						{
							if (node.XorTimes[0] > lastSetTime)
								result ^= node.Xors[0];
						}
						else if (node.XorTimes.Count >= 2)
						{
							var xistart = BinarySearchRightBiased(node.XorTimes, t => t - lastSetTime);

							if (node.XorTimes[xistart] > lastSetTime)
							{
								if (xistart == 0)
									result ^= node.Xors.Last();
								else
									result ^= node.Xors.Last() ^ node.Xors[xistart - 1];
							}
						}
					}

					yield return result.ToString();
				}
			}
		}

		public static string[] GenerateCommands(Random rnd, int n, int m)
		{
			var commands = new string[m];
			for (var mi = 0; mi < m; mi++)
			{
				var type = rnd.Next(3) + 1;
				var S = RandomGenerator.RandomString(rnd, "01", n);
				var x = rnd.Next(0, 8);

				if (mi == 0) { type = 1; x = rnd.Next(1, 8); }

				if (type == 1)
					commands[mi] = string.Concat("1 ", x, " ", S);
				else if (type == 2)
					commands[mi] = string.Concat("2 ", x, " ", S);
				else if (type == 3)
					commands[mi] = string.Concat("3 ", S);
			}
			return commands;
		}

		public static void Example1()
		{
			var commands = @"1 3 110
3 100
2 1 011
3 010".Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var line in Solve(3, 4, commands))
				Console.WriteLine(line);
		}

		public static IEnumerable<string> Solve_HZ2(int n, int m, string[] commands)
		{
			var setsCount = 1 << n;

			var bits = new int[n];
			for (var i = 0; i < bits.Length; i++)
				bits[i] = 1 << i;

			var graphPar = new List<int>[setsCount];
			for (var i = 0; i < graphPar.Length; i++)
			{
				var list = new List<int>();
				foreach (var b in bits)
					if ((i & b) == 0)
						list.Add(i | b);
				graphPar[i] = list;
			}

			var sets = new ulong[setsCount];
			var setsTimes = ArrayHelper.Create(setsCount, i => -1);
			var xors = new ulong[setsCount];
			var xorsTimes = ArrayHelper.Create(setsCount, i => -1);
			var visited = new bool[setsCount];

			for (var time = 0; time < commands.Length; time++)
			{
				var command = commands[time];

				var type = command[0];

				var si1 = 1;
				var si2 = command.LastIndexOf(' ');

				var x = si1 == si2 ? 0ul : ulong.Parse(command.Substring(si1 + 1, si2 - si1 - 1));
				var S = command.Substring(si2 + 1);
				var smask = GetMask(S);

				if (type == '1')
				{
					sets[smask] = x;
					setsTimes[smask] = time;
					xors[smask] = 0;
					xorsTimes[smask] = -1;
				}
				else if (type == '2')
				{
					xors[smask] ^= x;
					xorsTimes[smask] = time;
				}
				else if (type == '3')
				{
					var lastTime = -1;
					var value = 0ul;

					Array.Clear(visited, 0, visited.Length);
					var queue = new Queue<int>();
					queue.Enqueue(smask);
					while (queue.Count > 0)
					{
						var node = queue.Dequeue();
						if (setsTimes[node] > lastTime)
						{
							lastTime = setsTimes[node];
							value = sets[node];
						}
						foreach (var par in graphPar[node])
							if (!visited[par])
							{
								queue.Enqueue(par);
								visited[par] = true;
							}
					}

					Array.Clear(visited, 0, visited.Length);
					queue.Enqueue(smask);
					while (queue.Count > 0)
					{
						var node = queue.Dequeue();
						if (xors[node] > 0 && xorsTimes[node] > lastTime)
							value ^= xors[node];
						foreach (var par in graphPar[node])
							if (!visited[par])
							{
								queue.Enqueue(par);
								visited[par] = true;
							}
					}

					yield return value.ToString();
				}
			}
		}

		public static IEnumerable<string> Solve_HZ(int n, int m, string[] commands)
		{
			var setsCount = 1 << n;
			var trueBitsCount = n / 2;

			var bits = new int[n];
			for (var i = 0; i < bits.Length; i++)
				bits[i] = 1 << i;

			var graphSub = new List<int>[setsCount];
			for (var i = 0; i < graphSub.Length; i++)
			{
				var list = new List<int>();
				foreach (var b in bits)
					if ((i & b) > 0)
						list.Add(i & ~b);
				graphSub[i] = list;
			}

			var bitsCount = new int[setsCount];

			// TODO: build by graphSub
			var graphPar = new List<int>[setsCount];
			for (var i = 0; i < graphPar.Length; i++)
			{
				var list = new List<int>();
				foreach (var b in bits)
					if ((i & b) == 0)
					{
						list.Add(i | b);
						bitsCount[i | b] = bitsCount[i] + 1;
					}
				graphPar[i] = list;
			}

			var sets = new ulong[setsCount];
			var setsTimes = ArrayHelper.Create(setsCount, i => -1);
			var xors = new ulong[setsCount];
			var xorsTimes = ArrayHelper.Create(setsCount, i => -1);

			var trueMasks = new List<int>();
			for (var i = 0; i < bitsCount.Length; i++)
				if (bitsCount[i] == trueBitsCount)
					trueMasks.Add(i);

			for (var time = 0; time < commands.Length; time++)
			{
				var command = commands[time];

				var type = command[0];

				var si1 = 1;
				var si2 = command.LastIndexOf(' ');

				var x = si1 == si2 ? 0ul : ulong.Parse(command.Substring(si1 + 1, si2 - si1 - 1));
				var S = command.Substring(si2 + 1);
				var smask = GetMask(S);

				if (type == '1')
				{
					//var i = setsCount / 2 + 1; TODO!

					var queue = new Queue<int>();
					queue.Enqueue(smask);

					while (queue.Count > 0)
					{
						var node = queue.Dequeue();

						if (setsTimes[node] == time) continue;

						sets[node] = x;
						setsTimes[node] = time;
						xors[node] = 0;
						xorsTimes[node] = -1;

						if (bitsCount[node] > trueBitsCount)
							foreach (var child in graphSub[node])
								queue.Enqueue(child);
					}
				}
				else if (type == '2')
				{
					//var i = setsCount / 2 + 1; TODO!

					var queue = new Queue<int>();
					queue.Enqueue(smask);

					while (queue.Count > 0)
					{
						var node = queue.Dequeue();

						if (xorsTimes[node] == time) continue;

						xors[node] ^= x;
						xorsTimes[node] = time;

						if (bitsCount[node] > trueBitsCount)
							foreach (var child in graphSub[node])
								queue.Enqueue(child);
					}
				}
				else if (type == '3')
				{
					if (bitsCount[smask] >= trueBitsCount)
					{
						var result = setsTimes[smask] <= xorsTimes[smask] ? (sets[smask] ^ xors[smask]) : sets[smask];
						yield return result.ToString();
					}
					else
					{
						var parents = new HashSet<int>(trueMasks);
						while (parents.Count > 0)
						{
							var changeme = new HashSet<int>();
							foreach (var parent in parents)
								foreach (var child in graphSub[parent])
									changeme.Add(child);

							foreach (var node in changeme)
							{
								var lastTime = Math.Max(setsTimes[node], graphPar[node].Max(p => setsTimes[p]));

								foreach (var p in graphPar[node])
								{
									if (setsTimes[p] > setsTimes[node])
									{
										sets[node] = sets[p];
										setsTimes[node] = setsTimes[p];

										if (xorsTimes[p] > xorsTimes[node])
										{
											xors[node] = 0;
											xorsTimes[node] = -1;
										}
									}

									if (xors[p] > 0 && xorsTimes[p] > setsTimes[node])
									{
										xors[node] ^= xors[p];
										xorsTimes[node] = xorsTimes[p];
									}
								}

								if (node == smask)
								{
									changeme.Clear();
									break;
								}
							}

							parents = changeme;
						}

						var result = setsTimes[smask] <= xorsTimes[smask] ? (sets[smask] ^ xors[smask]) : sets[smask];
						yield return result.ToString();
					}
				}
			}
		}

		public static IEnumerable<string> Solve_BadOpt(int n, int m, string[] commands)
		{
			var setsCount = 1 << n;

			var TP = new int[m];
			var P = new List<int>();
			var isPrinted = new bool[setsCount];

			for (var t = commands.Length - 1; t >= 0; t--)
			{
				var command = commands[t];
				if (command.StartsWith("3 "))
				{
					var print = GetMask(command.Substring(2));
					if (!isPrinted[print])
					{
						P.Add(print);
						isPrinted[print] = true;
					}
				}

				TP[t] = P.Count;
			}

			var U = new ulong[1 << n];

			for (var t = 0; t < commands.Length; t++)
			{
				var command = commands[t];

				var type = command[0];

				var si1 = 1;
				var si2 = command.LastIndexOf(' ');

				var x = si1 == si2 ? 0ul : ulong.Parse(command.Substring(si1 + 1, si2 - si1 - 1));
				var S = command.Substring(si2 + 1);
				var smask = GetMask(S);

				if (type == '1')
				{
					var tpt = TP[t];
					for (var pi = 0; pi < tpt; pi++)
					{
						var print = P[pi];
						if ((print & ~smask) == 0)
							U[print] = x;
					}
				}
				else if (type == '2')
				{
					var tpt = TP[t];
					for (var pi = 0; pi < tpt; pi++)
					{
						var print = P[pi];
						if ((print & ~smask) == 0)
							U[print] ^= x;
					}
				}
				else if (type == '3')
				{
					yield return U[smask].ToString();
				}
			}
		}

		public static IEnumerable<string> Solve_2(int n, int m, string[] commands)
		{
			var printedMasksSet = new HashSet<int>();
			foreach (var command in commands)
				if (command.StartsWith("3 "))
					printedMasksSet.Add(GetMask(command.Substring(2)));

			var printedMasks = printedMasksSet.ToArray();

			var U = new ulong[1 << n];

			foreach (var command in commands)
			{
				var type = command[0];

				var si1 = 1;
				var si2 = command.LastIndexOf(' ');

				var x = si1 == si2 ? 0ul : ulong.Parse(command.Substring(si1 + 1, si2 - si1 - 1));
				var S = command.Substring(si2 + 1);
				var smask = GetMask(S);

				if (type == '1')
				{
					foreach (var pm in printedMasks)
						if ((pm & ~smask) == 0)
							U[pm] = x;
				}
				else if (type == '2')
				{
					foreach (var pm in printedMasks)
						if ((pm & ~smask) == 0)
							U[pm] ^= x;
				}
				else if (type == '3')
				{
					yield return U[smask].ToString();
				}
			}
		}

		public static IEnumerable<int> EnumerateIndices(string S)
		{
			var mask = GetMask(S);

			for (var i = 0; i < 1 << S.Length; i++)
				if ((i & ~mask) == 0)
					yield return i;
		}

		public static int GetMask(string S)
		{
			var mask = 0;
			for (var si = 0; si < S.Length; si++)
				if (S[si] == '1')
					mask |= 1 << (S.Length - si - 1);
			return mask;
		}

		public static IEnumerable<string> Solve_Brute(int n, int m, string[] commands)
		{
			var U = new ulong[1 << n];

			foreach (var command in commands)
			{
				var parts = command.Split();
				var type = parts[0];
				var S = parts[parts.Length - 1];

				if (type == "1")
				{
					var x = ulong.Parse(parts[1]);
					foreach (var i in EnumerateIndices(S))
						U[i] = x;
				}
				else if (type == "2")
				{
					var x = ulong.Parse(parts[1]);
					foreach (var i in EnumerateIndices(S))
						U[i] ^= x;
				}
				else if (type == "3")
				{
					var i = GetMask(S);
					yield return U[i].ToString();
				}
			}
		}



	}
}
