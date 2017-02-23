using ConsoleApplication1.DataTypes;
using ConsoleApplication1.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Не забудь запускать в релиз моде

/*
	In the original description of SA, the probability {\displaystyle P(e,e',T)} P(e,e',T) was equal to 1 when {\displaystyle e'<e} e'<e — i.e., 
	the procedure always moved downhill when it found a way to do so, irrespective of the temperature. Many descriptions and implementations of 
	SA still take this condition as part of the method's definition. However, this condition is not essential for the method to work.
	The {\displaystyle P} P function is usually chosen so that the probability of accepting a move decreases when the 
	difference {\displaystyle e'-e} e'-e increases—that is, small uphill moves are more likely than large ones. However, this requirement is 
	not strictly necessary, provided that the above requirements are met.

	In the formulation of the method by Kirkpatrick et al., the acceptance probability function {\displaystyle P(e,e',T)} P(e,e',T) was defined
	as 1 if {\displaystyle e'<e} e'<e, and {\displaystyle \exp(-(e'-e)/T)} \exp(-(e'-e)/T) otherwise.
	
	When choosing the candidate generator neighbour(), one must consider that after a few iterations of the SA algorithm, the current state is 
	expected to have much lower energy than a random state. Therefore, as a general rule, one should skew the generator towards candidate moves 
	where the energy of the destination state {\displaystyle s'} s' is likely to be similar to that of the current state

	Выбор параметров:
	* Диаметр графа - функция neighbour() должна быть такой, чтобы в графе всех возможных состояний можно было пройти из любой вершины в любую
		за достаточно небольшое число шагов
	* Гладкость - функция neighbour() должна быть такой, чтобы возвращаемый кандидат имел небольшое отличие энергии от текущего состояния. Более точно,
		когда температура достаточно большая, neighbour() должен возвращать тех кандидатов, у которых отличие большое. При низких T нужно возвращать
		близких кандидатов
	* Избегание барьеров - нужно избегать ситуаций, когда текущее состояние (или несколько рядом расположенных состояний) сильно лучше чем большинство
		соседей. Иначе, есть риск что алгоритм застрянет в текущей окрестности и не сможет выбраться
*/

namespace ConsoleApplication1.OtherTasks
{
	public class HashCodePizza
	{
		/*
			Состояние - массив непересекающихся кусочков пиццы

			Изменения:
				Увеличить кусок
				Уменьшить кусок
				Создать кусок
				Удалить кусок

			Энергия:
				Размер пиццы минус сумма всех корректных площадей, плюс штрафы некорректных площадей
				E = pizza_size - sum(correct_i.area) - coeff * sum(incorrect_i.area)

				Сумма площадей больше -> энергия меньше
				Алгоритму более выгодно увеличивать площадь у корректных площадей
		*/

		private float Energy(IEnumerable<Slice> slices, Task task, float coeff)
		{
			var result = (float)task.Size;
			var L = task.L;
			var H = task.H;
			foreach (var slice in slices)
			{
				if (task.IsFillingCorrect(slice))
					result -= slice.Area;
				else
					result -= coeff * slice.Area;
			}
			return result;
		}

		private long maxSliceId = 0;
		public class Slice
		{
			public int Row;
			public int Col;
			public int Width;
			public int Height;
			public long SliceId;

			public int Area => Width * Height;

			public Slice(long id)
			{
				SliceId = id;
			}

			public override string ToString() => $"[Slice {SliceId}: {Row},{Col} height {Height}, width {Width}]";
		}

		public class Task
		{
			public int L;
			public int H;
			public bool[][] Pizza;

			public int[][] TomatoesCounts;
			public int Size;

			public bool IsFillingCorrect(Slice slice)
			{
				var filling = CalcFilling(slice);
				return filling.Item1 >= L && filling.Item2 >= L && slice.Area <= H;
			}

			public Pair<int, int> CalcFilling(Slice slice)
			{
				var tomatoes = 0;
				for (var r = slice.Row; r < slice.Row + slice.Height; r++)
					tomatoes += TomatoesCounts[r][slice.Col + slice.Width - 1] - (slice.Col == 0 ? 0 : TomatoesCounts[r][slice.Col - 1]);
				return new Pair<int, int>(tomatoes, slice.Area - tomatoes);
			}

			public void CalcCounts()
			{
				TomatoesCounts = GetCounts(Pizza, true);
				Size = Pizza.Length * Pizza[0].Length;
			}

			private static int[][] GetCounts(bool[][] pizza, bool ingridient)
			{
				var result = new int[pizza.Length][];
				for (var r = 0; r < pizza.Length; r++)
				{
					var pizzaRow = pizza[r];
					var row = new int[pizzaRow.Length];
					var sum = 0;
					for (var i = 0; i < pizzaRow.Length; i++)
					{
						if (pizzaRow[i] == ingridient) sum++;
						row[i] = sum;
					}
					result[r] = row;
				}
				return result;
			}
		}

		public Task Example1()
		{
			return new Task
			{
				L = 1,
				H = 6,
				Pizza = "TTTTT\nTMMMT\nTTTTT".Split('\n').Select(s => s.Select(c => c == 'T').ToArray()).ToArray()
			};
		}

		public Task ExampleBig()
		{
			var lines = File.ReadAllLines(@"D:\Develop\experiments\stuff\ConsoleApplication1\ConsoleApplication1\Files\HashcodePizza\big.in");
			var header = lines[0].Split().Select(int.Parse).ToArray();
			return new Task
			{
				L = header[2],
				H = header[3],
				Pizza = lines.Skip(1).Select(s => s.Select(c => c == 'T').ToArray()).ToArray()
			};
		}

		private void TestCounts(Task task, Random rnd)
		{
			for (var t = 0; t < 100000; t++)
			{
				var rows = new[] { task.Pizza.RandomIndex(rnd), task.Pizza.RandomIndex(rnd) };
				if (rows[1] < rows[0]) rows.Swap(0, 1);
				var cols = new[] { task.Pizza[0].RandomIndex(rnd), task.Pizza[0].RandomIndex(rnd) };
				if (cols[1] < cols[0]) cols.Swap(0, 1);

				var slice = new Slice(maxSliceId) { Row = rows[0], Col = cols[0], Width = cols[1] - cols[0] + 1, Height = rows[1] - rows[0] + 1 };
				maxSliceId++;

				var filling = task.CalcFilling(slice);

				var expectedTomatoes = 0;
				var expectedMushrooms = 0;
				for (var r = 0; r < slice.Height; r++)
					for (var c = 0; c < slice.Width; c++)
					{
						var ingridient = task.Pizza[r + slice.Row][c + slice.Col];
						if (ingridient) expectedTomatoes++; else expectedMushrooms++;
					}

				if (expectedTomatoes != filling.Item1 || expectedMushrooms != filling.Item2)
					throw new InvalidOperationException(new { expectedTomatoes, expectedMushrooms, filling }.ToString());
			}
		}

		private State FillInitState(Task task, Random rnd)
		{
			var pizza = task.Pizza;
			var count = rnd.Next(task.Size / task.H / 2, task.Size / task.H);

			var slices = new List<Slice>();
			var map = ArrayHelper.Create(pizza.Length, _ => ArrayHelper.Create(pizza[0].Length, __ => (long)-1));

			for (var i = 0; i < count; i++)
				while (true)
				{
					var row = pizza.RandomIndex(rnd);
					var col = pizza[0].RandomIndex(rnd);
					if (map[row][col] < 0)
					{
						var slice = new Slice(maxSliceId) { Row = row, Col = col, Width = 1, Height = 1 };
						maxSliceId++;
						slices.Add(slice);
						map[row][col] = slice.SliceId;
						break;
					}
				}

			return new State
			{
				Map = map,
				Slices = slices,
			};
		}

		private struct State
		{
			public List<Slice> Slices;
			public long[][] Map;
		}

		private static long[][] UpdateImmutableMap(long[][] map, int row, int col, int width, int height, long setTo)
		{
			var newMap = map.ShallowClone();
			for (var r = row; r < row + height; r++)
			{
				var newRow = newMap[r].ShallowClone();
				for (var c = col; c < col + width; c++)
					newRow[c] = setTo;
				newMap[r] = newRow;
			}
			return newMap;
		}

		private int[][] modifyVectorsRCWH = new[]
		{
			new[] {  0, -1,  1,  0 },    // Grow left
			new[] { -1,  0,  0,  1 },    // Grow up
			new[] {  0,  0,  1,  0 },    // Grow right
			new[] {  0,  0,  0,  1 },    // Grow down

			new[] {  0,  1, -1,  0 },    // Shrink left		4
			new[] {  1,  0,  0, -1 },    // Shrink up		5
			new[] {  0,  0, -1,  0 },    // Shrink right	6
			new[] {  0,  0,  0, -1 },    // Shrink down		7
		};

		private string[] modifiedAreasRCWH = new[]
		{
			"001*",   // Grow left
			"00*1",   // Grow up
			"0+1*",   // Grow right
			"+0*1",   // Grow down

			"0-1*",   // Shrink left
			"-0*1",   // Shrink up
			"0.1*",   // Shrink right
			".0*1",   // Shrink down
		};

		private int[] intersectedModificationsRCWH = new[]
		{
			6,    // Grow left  causes other slice to shrink right
			7,    // Grow up    causes other slice to shrink down
			4,    // Grow right causes other slice to shrink left
			5,    // Grow down  causes other slice to shrink up
		};

		private KeyValuePair<int, double>[] smallSliceModifyVectorProbabilities = new[]
		{
			new KeyValuePair<int, double>(0, 3.0),
			new KeyValuePair<int, double>(1, 3.0),
			new KeyValuePair<int, double>(2, 3.0),
			new KeyValuePair<int, double>(3, 3.0),
			new KeyValuePair<int, double>(4, 1.0),
			new KeyValuePair<int, double>(5, 1.0),
			new KeyValuePair<int, double>(6, 1.0),
			new KeyValuePair<int, double>(7, 1.0),
		};

		private int[] bigSliceModifyVectors = new[] { 4, 5, 6, 7 }; // Shrink only

		private bool isDebug;
		private void EnsureOk(State state)
		{
			if (!isDebug) return;

			var busy = ArrayHelper.Create(state.Map.Length, _ => new bool[state.Map[0].Length]);
			foreach (var slice in state.Slices)
			{
				for (var r = slice.Row; r < slice.Row + slice.Height; r++)
					for (var c = slice.Col; c < slice.Col + slice.Width; c++)
					{
						if (busy[r][c])
							throw new InvalidOperationException(new { r, c, slice }.ToString());
						busy[r][c] = true;
						if (state.Map[r][c] != slice.SliceId)
							throw new InvalidOperationException(new { r, c, slice }.ToString());
					}
			}

			for (var r = 0; r < busy.Length; r++)
				for (var c = 0; c < busy[r].Length; c++)
					if (!busy[r][c] && state.Map[r][c] >= 0)
						throw new InvalidOperationException(new { r, c, sliceId = state.Map[r][c] }.ToString());
					else if (busy[r][c] && state.Map[r][c] < 0)
						throw new InvalidOperationException(new { r, c, sliceId = state.Map[r][c] }.ToString());
		}

		private IEnumerable<State> GetNeighboors(Random rnd, Task task, State state)
		{
			var slices = state.Slices;
			var map = state.Map;

			var slicesModifyProbabilities = state.Slices.Select((s, i) => new KeyValuePair<int, double>(i, s.Area < task.H ? 2.0 : 1.0)).ToArray();

			while (true)
			{
				var cmdProb = rnd.NextDouble();

				if (cmdProb < 0.005 || slices.Count == 0)
				{
					// Spawn
					var totalArea = slices.Sum(s => s.Area);
					if (totalArea + task.H < task.Size)
					{
						PointInt free;

						if (totalArea > task.Size / 100 * 99)
							free = map.ToPoints().Where(p => map[p.X][p.Y] < 0).RandomElement(rnd);
						else
							do free = new PointInt(map.RandomIndex(rnd), map[0].RandomIndex(rnd));
							while (map[free.X][free.Y] >= 0);

						var newSlice = new Slice(maxSliceId) { Row = free.X, Col = free.Y, Width = 1, Height = 1 };
						maxSliceId++;

						var newState = new State
						{
							Slices = slices.ConcatWithElement(newSlice).ToList(),
							Map = UpdateImmutableMap(map, free.X, free.Y, 1, 1, newSlice.SliceId)
						};
						EnsureOk(newState);
						yield return newState;

						continue;
					}
				}
				else if (cmdProb < 0.01)
				{
					// Kill
					var sliceIndex = slices.RandomIndex(rnd);
					var slice = slices[sliceIndex];
					var newSlices = slices.ToList();
					newSlices.RemoveAt(sliceIndex);
					var newMap = UpdateImmutableMap(map, slice.Row, slice.Col, slice.Width, slice.Height, -1);
					var newState = new State { Slices = newSlices, Map = newMap };
					EnsureOk(newState);
					yield return newState;
				}
				else
				{
					// Modify
					var sliceIndex = slicesModifyProbabilities.RandomWeightedElement(rnd);
					var slice = slices[sliceIndex];
					var newSlices = slices.ToList();

					var modifyIndex = slice.Area < task.H
						? smallSliceModifyVectorProbabilities.RandomWeightedElement(rnd)
						: bigSliceModifyVectors.RandomElement(rnd);

					var vectorRCWH = modifyVectorsRCWH[modifyIndex];
					var areaRCWH = modifiedAreasRCWH[modifyIndex];

					var newSlice = new Slice(slice.SliceId)
					{
						Row = slice.Row + vectorRCWH[0],
						Col = slice.Col + vectorRCWH[1],
						Width = slice.Width + vectorRCWH[2],
						Height = slice.Height + vectorRCWH[3],
					};

					if (newSlice.Row < 0 || newSlice.Col < 0) continue;
					if (newSlice.Width <= 0 || newSlice.Height <= 0) continue;
					if (newSlice.Row + newSlice.Height > task.Pizza.Length) continue;
					if (newSlice.Col + newSlice.Width > task.Pizza[0].Length) continue;

					newSlices[sliceIndex] = newSlice;

					var modifiedRow = areaRCWH[0] == '+' ? newSlice.Row + newSlice.Height - 1 
									: areaRCWH[0] == '-' ? newSlice.Row - 1
									: areaRCWH[0] == '.' ? newSlice.Row + newSlice.Height
									: newSlice.Row;
					var modifiedCol = areaRCWH[1] == '+' ? newSlice.Col + newSlice.Width - 1 
									: areaRCWH[1] == '-' ? newSlice.Col - 1 
									: areaRCWH[1] == '.' ? newSlice.Col + newSlice.Width
									: newSlice.Col;
					var modifiedWidth = areaRCWH[2] == '*' ? newSlice.Width : 1;
					var modifiedHeight = areaRCWH[3] == '*' ? newSlice.Height : 1;

					if (modifyIndex <= 3)
					{
						// Grow
						var newMap = map;

						for (var r = modifiedRow; r < modifiedRow + modifiedHeight; r++)
							for (var c = modifiedCol; c < modifiedCol + modifiedWidth; c++)
							{
								var otherSliceId = newMap[r][c];
								if (otherSliceId >= 0)
								{
									var otherModifyIndex = intersectedModificationsRCWH[modifyIndex];
									var modifyOtherRCWH = modifyVectorsRCWH[otherModifyIndex];
									var otherAreaRCWH = modifiedAreasRCWH[otherModifyIndex];
									var otherSliceIndex = newSlices.FirstIndexOf(s => s.SliceId == otherSliceId);
									var otherSlice = newSlices[otherSliceIndex];

									var newModifiedSlice = new Slice(otherSlice.SliceId)
									{
										Row = otherSlice.Row + modifyOtherRCWH[0],
										Col = otherSlice.Col + modifyOtherRCWH[1],
										Width = otherSlice.Width + modifyOtherRCWH[2],
										Height = otherSlice.Height + modifyOtherRCWH[3],
									};

									if (newModifiedSlice.Width == 0 || newModifiedSlice.Height == 0)
									{
										newSlices.RemoveAt(otherSliceIndex);
										newMap = UpdateImmutableMap(newMap, otherSlice.Row, otherSlice.Col, otherSlice.Width, otherSlice.Height, -1);
									}
									else
									{
										newSlices[otherSliceIndex] = newModifiedSlice;

										newMap = UpdateImmutableMap(
											newMap,
											otherAreaRCWH[0] == '+' ? newModifiedSlice.Row + newModifiedSlice.Height - 1
												: otherAreaRCWH[0] == '-' ? newModifiedSlice.Row - 1
												: otherAreaRCWH[0] == '.' ? newModifiedSlice.Row + newModifiedSlice.Height
												: newModifiedSlice.Row,
											otherAreaRCWH[1] == '+' ? newModifiedSlice.Col + newModifiedSlice.Width - 1
												: otherAreaRCWH[1] == '-' ? newModifiedSlice.Col - 1
												: otherAreaRCWH[1] == '.' ? newModifiedSlice.Col + newModifiedSlice.Width
												: newModifiedSlice.Col,
											otherAreaRCWH[2] == '*' ? newModifiedSlice.Width : 1,
											otherAreaRCWH[3] == '*' ? newModifiedSlice.Height : 1,
											-1);
									}
								}
							}

						newMap = UpdateImmutableMap(newMap, modifiedRow, modifiedCol, modifiedWidth, modifiedHeight, newSlice.SliceId);
						var newState = new State { Slices = newSlices, Map = newMap };
						EnsureOk(newState);
						yield return newState;
					}
					else
					{
						// Shrink		
						var newMap = UpdateImmutableMap(map, modifiedRow, modifiedCol, modifiedWidth, modifiedHeight, -1);
						var newState = new State { Slices = newSlices, Map = newMap };
						EnsureOk(newState);
						yield return newState;
					}
				}
			}
		}

		public void Go()
		{
			var rnd = new Random(123);

			const float energyCoeff = 0.5f;
			isDebug = true;

			var task = ExampleBig();
			task.CalcCounts();
			var pizza = task.Pizza;

			var state = FillInitState(task, rnd);

			var energy = Energy(state.Slices, task, energyCoeff);

			var bestState = state;
			var bestEnergy = energy;

			ShowStateInfo(task, state.Slices, energy);

			var temperatureSteps = 10000000;
			for (var step = temperatureSteps; step > 0; step--)
			{
				var T = (float)task.Size / temperatureSteps * step;

				if (step % (temperatureSteps / 10) == 0)
					Console.WriteLine($"Temparature: {T}, best energy so far {bestEnergy}");

				var stepTryCount = 50;
				foreach (var nextState in GetNeighboors(rnd, task, state))
				{
					var nextEnergy = Energy(nextState.Slices, task, energyCoeff);

					if (nextEnergy < bestEnergy)
					{
						bestEnergy = nextEnergy;
						bestState = nextState;
					}

					var isOk = nextEnergy < energy;

					if (!isOk)
					{
						var P = Math.Exp(-(nextEnergy - energy) / T);
						isOk = P > rnd.NextDouble();
					}

					if (isOk)
					{
						energy = nextEnergy;
						state = nextState;
						break;
					}

					stepTryCount--;
					if (stepTryCount == 0) break;
				}

				if (step % 1000 == 0)
				{
					SaveBest(bestState, bestEnergy);
					Console.WriteLine("Saved best energy " + bestEnergy);
					ShowStateInfo(task, bestState.Slices, bestEnergy);
				}
			}

			Console.WriteLine();
			Console.WriteLine("Best solution:");
			ShowStateInfo(task, bestState.Slices, bestEnergy);
			SaveBest(bestState, bestEnergy);
		}

		private void SaveBest(State state, float energy)
		{
			var lines = new List<string>();
			lines.Add(state.Slices.Count.ToString());
			foreach (var slice in state.Slices)
				lines.Add($"{slice.Row} {slice.Col} {slice.Row + slice.Height - 1} {slice.Col + slice.Width - 1}");
			File.WriteAllLines($"result_energy_{energy}.txt", lines.ToArray());
		}

		private void ShowStateInfo(Task task, ICollection<Slice> slices, float energy)
		{
			var count = 0;
			var area = 0;

			foreach (var slice in slices)
				if (task.IsFillingCorrect(slice))
				{
					count++;
					area += slice.Area;
				}
			Console.WriteLine($"State: slices {slices.Count}, energy {energy}, correct slices {count}, score {area}");
		}
	}
}
