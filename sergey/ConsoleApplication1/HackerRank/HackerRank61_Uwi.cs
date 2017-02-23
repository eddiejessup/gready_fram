using ConsoleApplication1.Helpers;
using System.Collections.Generic;

namespace ConsoleApplication1.HackerRank
{
	public class HackerRank61_Uwi
	{
		private List<int> input;
		private int inputIndex;

		private int ni() => input[inputIndex++];

		public ulong[] Solve(List<int> arguments)
		{
			input = arguments;

			int n = ni();
			int[] par = new int[n];
			par[0] = -1;
			for (int i = 1; i < n; i++)
				par[i] = ni() - 1;

			int[][] g = parentToG(par);
			int[][] pars = parents3(g, 0);
			int[] ord = pars[1];
			int[] dep = pars[2];
			long[] dp0 = new long[n];
			long[] dp1 = new long[n];
			long[] dp2 = new long[n];
			for (int i = n - 1; i >= 0; i--)
			{
				int cur = ord[i];
				dp0[cur] += 1;
				if (i > 0)
				{
					dp0[par[cur]] += dp0[cur];
					dp1[par[cur]] += dp1[cur] + dp0[cur];
					dp2[par[cur]] += dp2[cur] + 2 * dp1[cur] + dp0[cur];
				}
			}
			long[] udp0 = new long[n];
			long[] udp1 = new long[n];
			long[] udp2 = new long[n];
			for (int i = 1; i < n; i++)
			{
				int cur = ord[i];
				long odp0 = udp0[par[cur]] + dp0[par[cur]] - dp0[cur];
				long odp1 = udp1[par[cur]] + dp1[par[cur]] - (dp1[cur] + dp0[cur]);
				long odp2 = udp2[par[cur]] + dp2[par[cur]] - (dp2[cur] + 2 * dp1[cur] + dp0[cur]);

				udp0[cur] = odp0;
				udp1[cur] = odp1 + odp0;
				udp2[cur] = odp2 + 2 * odp1 + odp0;
			}
			int[][] spar = logstepParents(par);

			var result = new List<ulong>();

			for (int Q = ni(); Q > 0; Q--)
			{
				int a = ni() - 1;
				int b = ni() - 1;
				int lca = lca2(a, b, spar, dep);
				long dab = dep[a] + dep[b] - 2 * dep[lca];
				if (lca == b)
				{
					long ret = dp2[a] + udp2[a] - (udp2[b] + 2 * dab * udp1[b] + dab * dab * udp0[b]);
					result.Add((ulong)ret);
				}
				else
				{
					long d2 = dp2[b] + 2 * dab * dp1[b] + dab * dab * dp0[b];
					result.Add((ulong)d2);
				}
			}

			return result.ToArray();
		}

		public static int lca2(int a, int b, int[][] spar, int[] depth)
		{
			if (depth[a] < depth[b])
			{
				b = ancestor(b, depth[b] - depth[a], spar);
			}
			else if (depth[a] > depth[b])
			{
				a = ancestor(a, depth[a] - depth[b], spar);
			}

			if (a == b)
				return a;
			int sa = a, sb = b;
			for (int low = 0, high = depth[a], t = high.HighestOneBit(), k = t.NumberOfTrailingZeros(); t > 0; t >>= 1, k--)
			{
				if ((low ^ high) >= t)
				{
					if (spar[k][sa] != spar[k][sb])
					{
						low |= t;
						sa = spar[k][sa];
						sb = spar[k][sb];
					}
					else
					{
						high = low | t - 1;
					}
				}
			}
			return spar[0][sa];
		}

		protected static int ancestor(int a, int m, int[][] spar)
		{
			for (int i = 0; m > 0 && a != -1; m >>= 1, i++)
			{
				if ((m & 1) == 1)
					a = spar[i][a];
			}
			return a;
		}

		public static int[][] logstepParents(int[] par)
		{
			int n = par.Length;
			int m = BitHelper.NumberOfTrailingZeros(BitHelper.HighestOneBit(n - 1)) + 1;
			int[][] pars = ArrayHelper.Create<int>(m, n);
			pars[0] = par;
			for (int j = 1; j < m; j++)
			{
				for (int i = 0; i < n; i++)
				{
					pars[j][i] = pars[j - 1][i] == -1 ? -1 : pars[j - 1][pars[j - 1][i]];
				}
			}
			return pars;
		}


		public static int[][] parentToG(int[] par)
		{
			int n = par.Length;
			int[] ct = new int[n];
			for (int i = 0; i < n; i++)
			{
				if (par[i] >= 0)
				{
					ct[i]++;
					ct[par[i]]++;
				}
			}
			int[][] g = new int[n][];
			for (int i = 0; i < n; i++)
			{
				g[i] = new int[ct[i]];
			}
			for (int i = 0; i < n; i++)
			{
				if (par[i] >= 0)
				{
					g[par[i]][--ct[par[i]]] = i;
					g[i][--ct[i]] = par[i];
				}
			}
			return g;
		}


		public static int[][] parents3(int[][] g, int root)
		{
			int n = g.Length;

			int[] par = ArrayHelper.Create(n, i => -1);

			int[] depth = new int[n];
			depth[0] = 0;

			int[] q = new int[n];
			q[0] = root;
			for (int p = 0, r = 1; p < r; p++)
			{
				int cur = q[p];
				foreach (int nex in g[cur])
				{
					if (par[cur] != nex)
					{
						q[r++] = nex;
						par[nex] = cur;
						depth[nex] = depth[cur] + 1;
					}
				}
			}
			return new int[][] { par, q, depth };
		}

		static int[][] packU(int n, int[] from, int[] to)
		{
			int[][] g = new int[n][];
			int[] p = new int[n];
			foreach (int f in from)
				p[f]++;
			foreach (int t in to)
				p[t]++;
			for (int i = 0; i < n; i++)
				g[i] = new int[p[i]];
			for (int i = 0; i < from.Length; i++)
			{
				g[from[i]][--p[from[i]]] = to[i];
				g[to[i]][--p[to[i]]] = from[i];
			}
			return g;
		}

	}
}
