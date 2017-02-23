using ConsoleApplication1.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication1.OtherTasks.Hashcode2017
{
	public class Endpoint
	{
		public ulong LatencyToDataCenter;
		public Dictionary<int, ulong> CacheLatencies;
	}

	public class Request
	{
		public int EndpointId;
		public int VideoId;
		public int RequestsCount;
	}

	public class Video
	{
		public int VideoId;
		public ulong Size;
	}

	public class Problem
	{
		public int V;
		public int E;
		public int R;
		public int C;
		public ulong X;

		public Video[] Videos;
		public Endpoint[] Endpoints;
		public Request[] Requests;

		public ulong TotalRequestsCount;

		public static Problem Read(string fileName)
		{
			var lines = File.ReadAllLines(fileName);
			var vercx = lines[0].Split().Select(int.Parse).ToArray();
			var endpoints = new List<Endpoint>();

			var problem = new Problem
			{
				V = vercx[0],
				E = vercx[1],
				R = vercx[2],
				C = vercx[3],
				X = (ulong)vercx[4],
				Videos = lines[1].Split().Select(ulong.Parse).Select((s, i) => new Video { Size = s, VideoId = i }).ToArray(),
			};

			var li = 2;
			for (var e = 0; e < problem.E; e++)
			{
				var endpheader = lines[li].Split().Select(ulong.Parse).ToArray();
				li++;
				var cacheLatencies = new Dictionary<int, ulong>();

				for (var c = 0; c < (int)endpheader[1]; c++)
				{
					var cacheLine = lines[li + c].Split().Select(ulong.Parse).ToArray();
					cacheLatencies.Add((int)cacheLine[0], cacheLine[1]);
				}

				li += (int)endpheader[1];

				endpoints.Add(new Endpoint
				{
					LatencyToDataCenter = endpheader[0],
					CacheLatencies = cacheLatencies,
				});
			}

			var requests = new Request[problem.R];
			for (var r = 0; r < requests.Length; r++)
			{
				var line = lines[li].Split().Select(int.Parse).ToArray();
				li++;
				requests[r] = new Request
				{
					VideoId = line[0],
					EndpointId = line[1],
					RequestsCount = line[2]
				};
			}

			problem.Endpoints = endpoints.ToArray();
			problem.Requests = requests;
			problem.TotalRequestsCount = requests.Select(r => (ulong)r.RequestsCount).Sum();

			return problem;
		}
	}
}
