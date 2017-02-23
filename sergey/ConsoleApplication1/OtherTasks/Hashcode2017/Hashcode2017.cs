using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication1.OtherTasks.Hashcode2017
{
	public class VideosAndCaches
	{
		private JsonSerializerMaster jsonSerializer = new JsonSerializerMaster();

		private void ShowExapmle()
		{
			var problem = Problem.Read(@"D:\Develop\experiments\stuff\ConsoleApplication1\ConsoleApplication1\Files\Hashcode\example_statement.in");
			Console.WriteLine(jsonSerializer.SerializeUserFriendly(problem));

			Console.WriteLine(new Solution
			{
				CacheLoads = new[]
				{
					new CacheLoad { CacheId = 0, VideoIds = new[] { 2 }.ToHashSet() },
					new CacheLoad { CacheId = 1, VideoIds = new[] { 3, 1 }.ToHashSet() },
					new CacheLoad { CacheId = 2, VideoIds = new[] { 0, 1 }.ToHashSet() },
				}
			}.Score(problem));
		}

		public void Go()
		{
			Solve();
		}

		public void Solve()
		{
			var rnd = new Random(123);

			var problemFile = @"D:\Develop\experiments\stuff\ConsoleApplication1\ConsoleApplication1\Files\Hashcode\me_at_the_zoo.in";

			var problem = Problem.Read(problemFile);

			var maxEnergy = problem.Requests.Select(r => (ulong)r.RequestsCount * problem.Endpoints[r.EndpointId].LatencyToDataCenter).Sum()
				* 1000f / problem.TotalRequestsCount;
			maxEnergy /= 1;

			var solution = CreateInitSolution(problem, rnd);
			var energy = solution.Score(problem);

			var bestSolution = solution;
			var bestEnergy = energy;

			var temperatureSteps = 1000000;
			var T = maxEnergy;
			for (var step = temperatureSteps; step > 0; step--)
			{
				//var T = maxEnergy / temperatureSteps * step;
				T *= 0.9999f;

				var nextSolution = GetNeighboor(problem, solution, rnd);
				var nextEnergy = nextSolution.Score(problem);

				if (nextEnergy > bestEnergy)
				{
					bestEnergy = nextEnergy;
					bestSolution = nextSolution;
				}

				var isOk = nextEnergy > energy;
				if (!isOk)
				{
					var P = Math.Exp(-(energy - nextEnergy) / T);
					//Console.WriteLine($"{energy - nextEnergy}   {T}    {P}");
					isOk = P > rnd.NextDouble();
				}

				if (isOk)
				{
					energy = nextEnergy;
					solution = nextSolution;
				}

				if (step % 10000 == 0)
				{
					Console.WriteLine($"Step {step}/{temperatureSteps}. Temperature {T}. Current energy {energy}. Best energy so far: {bestEnergy}");
					bestSolution.Save(Path.GetFileName(problemFile), bestEnergy);
				}
			}
		}

		public Solution GetNeighboor(Problem problem, Solution solution, Random rnd)
		{
			var cacheLoads = solution.CacheLoads;

			while (true)
			{
				var p = rnd.NextDouble();

				if (p < 0.25)
				{
					// Add video
					var cacheLoadIndex = cacheLoads.RandomIndex(rnd);
					var cacheLoad = cacheLoads[cacheLoadIndex];

					var size = cacheLoad.VideoIds.Select(v => problem.Videos[v].Size).Sum();
					var video = problem.Videos.Where(v => v.Size <= problem.X - size).RandomElement(rnd, false);
					if (video != null)
					{
						var newLoads = cacheLoads.ShallowClone();
						newLoads[cacheLoadIndex] = new CacheLoad
						{
							CacheId = cacheLoad.CacheId,
							VideoIds = cacheLoad.VideoIds.ToHashSet(),
						};
						newLoads[cacheLoadIndex].VideoIds.Add(video.VideoId);
						return new Solution { CacheLoads = newLoads };
					}
				}
				else if (p < 0.5)
				{
					// Swap video between server and pool
					var cacheLoadIndex = cacheLoads.RandomIndex(rnd);
					var cacheLoad = cacheLoads[cacheLoadIndex];

					if (cacheLoad.VideoIds.Count == 0) continue;

					var solVideoId = cacheLoad.VideoIds.RandomElement(rnd);
					var size = cacheLoad.VideoIds.Select(v => problem.Videos[v].Size).Sum() - problem.Videos[solVideoId].Size;

					var poolVideo = problem.Videos.Where(v => v.Size <= problem.X - size).RandomElement(rnd, false);
					if (poolVideo == null) continue;

					var newLoads = cacheLoads.ShallowClone();
					newLoads[cacheLoadIndex] = new CacheLoad
					{
						CacheId = cacheLoad.CacheId,
						VideoIds = cacheLoad.VideoIds.ToHashSet(),
					};
					newLoads[cacheLoadIndex].VideoIds.Remove(solVideoId);
					newLoads[cacheLoadIndex].VideoIds.Add(poolVideo.VideoId);
					return new Solution { CacheLoads = newLoads };
				}
				else if (p < 0.85)
				{
					// Swap video between servers
					var cacheLoadIndexOne = cacheLoads.IndicesWhere(c => c.VideoIds.Count > 0).RandomElement(rnd, false);
					var cacheLoadIndexTwo = cacheLoads.IndicesWhere(c => c.VideoIds.Count > 0).RandomElement(rnd, false);

					if (cacheLoadIndexOne == cacheLoadIndexTwo
						|| cacheLoads[cacheLoadIndexOne].VideoIds.Count == 0
						|| cacheLoads[cacheLoadIndexTwo].VideoIds.Count == 0)
						continue;

					var cacheLoadOne = cacheLoads[cacheLoadIndexOne];
					var cacheLoadTwo = cacheLoads[cacheLoadIndexTwo];

					var videoOneId = cacheLoadOne.VideoIds.RandomElement(rnd);
					var videoOneSize = problem.Videos[videoOneId].Size;
					var sizeOne = cacheLoadOne.VideoIds.Select(v => problem.Videos[v].Size).Sum() - videoOneSize;

					var sizeTwoAll = cacheLoadTwo.VideoIds.Select(v => problem.Videos[v].Size).Sum();

					var videoTwo = cacheLoadTwo.VideoIds.Select(v => problem.Videos[v]).Where(v => sizeTwoAll - v.Size + videoOneSize <= problem.X).RandomElement(rnd, false);

					if (videoTwo == null) continue;
					if (sizeOne + videoTwo.Size > problem.X) continue;

					var newLoads = cacheLoads.ShallowClone();
					newLoads[cacheLoadIndexOne] = new CacheLoad
					{
						CacheId = cacheLoadOne.CacheId,
						VideoIds = cacheLoadOne.VideoIds.ToHashSet(),
					};
					newLoads[cacheLoadIndexTwo] = new CacheLoad
					{
						CacheId = cacheLoadTwo.CacheId,
						VideoIds = cacheLoadTwo.VideoIds.ToHashSet(),
					};
					newLoads[cacheLoadIndexOne].VideoIds.Remove(videoOneId);
					newLoads[cacheLoadIndexOne].VideoIds.Add(videoTwo.VideoId);
					newLoads[cacheLoadIndexTwo].VideoIds.Remove(videoTwo.VideoId);
					newLoads[cacheLoadIndexTwo].VideoIds.Add(videoOneId);
					return new Solution { CacheLoads = newLoads };
				}
				else
				{
					// Remove video
					var cacheLoadIndex = cacheLoads.RandomIndex(rnd);
					var cacheLoad = cacheLoads[cacheLoadIndex];

					if (cacheLoad.VideoIds.Count > 0)
					{
						var removeVideoId = cacheLoad.VideoIds.RandomElement(rnd);

						var newLoads = cacheLoads.ShallowClone();
						newLoads[cacheLoadIndex] = new CacheLoad
						{
							CacheId = cacheLoad.CacheId,
							VideoIds = cacheLoad.VideoIds.ToHashSet(),
						};
						newLoads[cacheLoadIndex].VideoIds.Remove(removeVideoId);
						return new Solution { CacheLoads = newLoads };
					}
				}
			}
		}

		public Solution CreateInitSolution(Problem problem, Random rnd)
		{
			var cacheLoads = new CacheLoad[problem.C];
			for (var c = 0; c < problem.C; c++)
			{
				var videos = new HashSet<int>();
				var size = 0ul;

				while (true)
				{
					var video = problem.Videos.Where(v => v.Size <= problem.X - size).RandomElement(rnd, false);
					if (video == null)
						break;
					videos.Add(video.VideoId);
					size += video.Size;
				}

				cacheLoads[c] = new CacheLoad
				{
					CacheId = c,
					VideoIds = videos
				};
			}

			return new Solution
			{
				CacheLoads = cacheLoads
			};
		}
	}
}
