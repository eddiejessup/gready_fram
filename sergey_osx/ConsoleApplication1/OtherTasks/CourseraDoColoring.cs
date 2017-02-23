using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.OtherTasks
{
	public class CourseraDoColoring
	{
		const string dir = @"D:\Develop\experiments\stuff\ConsoleApplication1\coursera_do_coloring\data";

		public void Go()
		{
			Solve("gc_4_1");
		}

		public void Solve(string file)
		{
			var content = File.ReadAllLines(Path.Combine(dir, file));
			var ne = content[0].Split().Select(int.Parse).ToArray();
			var n = ne[0];
			var e = ne[1];
			var edges = new int[e][];
			for (var i = 0; i < e; i++)
				edges[i] = content[i + 1].Split().Select(int.Parse).ToArray();

			Console.WriteLine(new { n, e });
			foreach (var edge in edges)
				Console.WriteLine($"{edge[0]} {edge[1]}");
			Console.WriteLine();

			var adj = GraphHelper.ToAdjacencyList_UnDirected(edges, n);

			for (var domainSize = 1; domainSize <= 1000; domainSize++)
			{
				var solution = Solve(n, e, adj, domainSize);
				if (solution != null)
				{
					Console.WriteLine("{0} 1", solution.Distinct().Count());
					Console.WriteLine(solution.Join());
					break;
				}
			}
		}

		public struct Decision
		{
			public int Node;
			public int Color;
			public List<int> NeighboorsAffected;
		}

		public int[] Solve(int n, int e, List<int>[] adj, int domainSize)
		{
			var solution = ArrayHelper.Create(n, _ => -1);

			var allColors = ArrayHelper.Create(domainSize, i => i);
			var domains = ArrayHelper.Create(n, _ => allColors.ToHashSet());

			var decisions = new Stack<Decision>();

			while (true)
			{
				var v = solution.FirstIndexOf(_ => _ < 0);
				if (v < 0)
					break;

				var doBacktrack = false;

				if (domains[v].Count == 0)
				{
					doBacktrack = true;
				}
				else
				{
					var color = domains[v].First();

					var neighboorsAffected = new List<int>();

					foreach (var neighboor in adj[v])
					{
						if (solution[neighboor] == color)
						{
							doBacktrack = true;
							break;
						}
						else if (solution[neighboor] < 0 && domains[neighboor].Contains(color))
						{
							neighboorsAffected.Add(neighboor);
						}
					}

					if (!doBacktrack)
					{
						solution[v] = color;
						foreach (var neighboor in neighboorsAffected)
							domains[neighboor].Remove(color);

						decisions.Push(new Decision
						{
							Node = v,
							Color = color,
							NeighboorsAffected = neighboorsAffected
						});
					}
				}

				if (doBacktrack)
				{
					var decision = decisions.Pop();
					solution[decision.Node] = -1;
					foreach (var neighboor in decision.NeighboorsAffected)
						domains[neighboor].Add(decision.Color);
				}
			}

			return solution;
		}
	}
}
