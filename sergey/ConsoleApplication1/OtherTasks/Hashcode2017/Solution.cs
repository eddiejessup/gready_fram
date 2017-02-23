using ConsoleApplication1.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.OtherTasks.Hashcode2017
{
	public class CacheLoad
	{
		public int CacheId;
		public HashSet<int> VideoIds;
	}

	public class Solution
	{
		public CacheLoad[] CacheLoads;

		public float Score(Problem problem)
		{
			var timeSaved = 0ul;
			foreach (var request in problem.Requests)
			{
				var endpoint = problem.Endpoints[request.EndpointId];

				var latencyDc = endpoint.LatencyToDataCenter;

				var cacheLatencies = endpoint.CacheLatencies;

				var latencyClosestCache = CacheLoads
					.Where(c => c.VideoIds.Contains(request.VideoId))
					.OrderBy(c => cacheLatencies.ContainsKey(c.CacheId) ? cacheLatencies[c.CacheId] : ulong.MaxValue)
					.FirstOrDefault();

				var latencyCache =
					latencyClosestCache == null ? latencyDc 
					: cacheLatencies.ContainsKey(latencyClosestCache.CacheId) ? cacheLatencies[latencyClosestCache.CacheId]
					: latencyDc;

				timeSaved += (ulong)request.RequestsCount * (latencyDc - latencyCache);
			}

			return timeSaved * 1000f / problem.TotalRequestsCount;
		}

		public void Save(string taskName, float energy)
		{
			var lines = new List<string>();
			lines.Add(CacheLoads.Length.ToString());
			foreach (var cacheLoad in CacheLoads)
			{
				var sb = new StringBuilder();
				sb.Append(cacheLoad.CacheId.ToString());
				sb.Append(" ");
				sb.Append(cacheLoad.VideoIds.Join(" "));
				lines.Add(sb.ToString());
			}

			File.WriteAllLines($"{taskName} {energy}", lines.ToArray());
		}
	}
}
