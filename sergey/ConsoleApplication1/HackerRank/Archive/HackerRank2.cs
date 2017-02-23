using ConsoleApplication1.Helpers;
using System;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank2
	{
		private static int N;
		private static int T;
		private static int Max = 0;

		public struct Task
		{
			public int[] Ci;
			public bool[][] A;
			public bool[][] B;

			public Task(string Cstring, string[] wizardStrings)
			{
				Ci = Cstring.Split().Select(int.Parse).ToArray();
				A = new bool[T][];
				B = new bool[T][];
				for (var wi = 0; wi < T; wi++)
				{
					A[wi] = ToSkillsArray(N, wizardStrings[wi * 2]);
					B[wi] = ToSkillsArray(N, wizardStrings[wi * 2 + 1]);
				}
			}

			public Task(int[] Ci, bool[][] A, bool[][] B)
			{
				this.Ci = Ci;
				this.A = A;
				this.B = B;
			}

			public override string ToString()
			{
				var sb = new StringBuilder();

				sb.AppendLine(new { N, T }.ToString());
				sb.AppendLine("C = " + string.Join(" ", Ci));
				for (var wi = 0; wi < A.Length; wi++)
				{
					sb.AppendLine("Wizard #" + wi);
					sb.AppendLine("A = " + A[wi].IndicesWhere(s => s).Join(" "));
					sb.AppendLine("B = " + B[wi].IndicesWhere(s => s).Join(" "));
				}

				return sb.ToString();
			}

			private static bool[] ToSkillsArray(int N, string skillsString)
			{
				var ints = skillsString.Split().Skip(1).Select(int.Parse).ToArray();
				var result = new bool[N];
				foreach (var i in ints) result[i - 1] = true;
				return result;
			}
		}

		public void Go()
		{
			//var task = new Task(3, 1, "3 0 0", new[] { "1 1", "2 2 3" });
			N = 4;
			T = 3;
			var task = new Task("4 0 0 0", new[] { "1 1 3", "1 2 4", "1 1", "1 3", "1 1", "1 3" });
			Console.WriteLine(task);
			var result = SolveBrute(task);
			Console.WriteLine("Result = " + result);
		}

		public static void GoByConsole()
		{
			var NT = Console.ReadLine().Split().Select(int.Parse).ToArray();
			N = NT[0];
			T = NT[1];
			var Cstring = Console.ReadLine();
			var wizardStrings = new string[2 * T];
			for (var i = 0; i < 2 * T; i++)
				wizardStrings[i] = Console.ReadLine();

			var sol = SolveBrute(new Task(Cstring, wizardStrings));
			Console.WriteLine(sol);
		}

		public static int SolveBrute(Task task)
		{
			var sum = task.Ci.Count(ci => ci > 0);
			Max = Math.Max(Max, sum);
			var wizardIndexes = Enumerable.Range(0, T).OrderBy(wi => task.A[wi].Length * task.B[wi].Length).ToArray();

			var liveWizards = wizardIndexes.Count(wi => task.A[wi].Length > 0 && task.B[wi].Length > 0);
			if (liveWizards + sum <= Max) return Max;

			foreach (var wi in wizardIndexes)
			{
				var wA = task.A[wi];
				var wB = task.B[wi];

				if (wA.Length == 0 || wB.Length == 0) continue;

				for (var skillAi = 0; skillAi < wA.Length; skillAi++)
				{
					if (!wA[skillAi]) continue;

					for (var skillBi = 0; skillBi < wB.Length; skillBi++)
					{
						if (!wB[skillBi] || skillAi == skillBi) continue;
						if (task.Ci[skillAi] == 0) continue;

						var newCi = task.Ci.ToArray();
						newCi[skillAi]--;
						newCi[skillBi]++;

						var newWizardA = wA.ToArray();
						var newWizardB = wB.ToArray();
						newWizardA[skillAi] = false;
						newWizardB[skillBi] = false;

						var newA = task.A.ToArray();
						var newB = task.B.ToArray();
						newA[wi] = newWizardA;
						newB[wi] = newWizardB;

						var subTask = new Task(
							newCi,
							newA,
							newB);

						Max = Math.Max(Max, SolveBrute(subTask));
					}
				}
			}
			return Max;
		}

		public static int SolveBruteChecked(Task task)
		{
			// Passed: 0-8, 20-22, 24
			var max = task.Ci.Count(ci => ci > 0);
			for (var wi = 0; wi < T; wi++)
			{
				var wA = task.A[wi];
				var wB = task.B[wi];

				if (wA.Length == 0 || wB.Length == 0) continue;

				for (var skillAi = 0; skillAi < wA.Length; skillAi++)
				{
					if (!wA[skillAi]) continue;

					for (var skillBi = 0; skillBi < wB.Length; skillBi++)
					{
						if (!wB[skillBi]) continue;
						if (task.Ci[skillAi] == 0) continue;

						var newCi = task.Ci.ToArray();
						newCi[skillAi]--;
						newCi[skillBi]++;

						var newWizardA = wA.ToArray();
						var newWizardB = wB.ToArray();
						newWizardA[skillAi] = false;
						newWizardB[skillBi] = false;

						var newA = task.A.ToArray();
						var newB = task.B.ToArray();
						newA[wi] = newWizardA;
						newB[wi] = newWizardB;

						var subTask = new Task(
							newCi,
							newA,
							newB);

						max = Math.Max(max, SolveBruteChecked(subTask));
					}
				}
			}
			return max;
		}
	}
}
