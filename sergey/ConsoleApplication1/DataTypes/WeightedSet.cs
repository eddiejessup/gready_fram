﻿using ConsoleApplication1.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1.DataTypes
{
	public class WeightedSet<T> : IEnumerable<KeyValuePair<T, double>>
	{
		private readonly Dictionary<T, double> items;
		private double weightsSum;

		public WeightedSet()
		{
			items = new Dictionary<T, double>();
		}

		public WeightedSet(IEnumerable<T> seq)
		{
			items = seq.ToDictionary(i => i, i => 1d);
			weightsSum = items.Count;
		}

		public WeightedSet(IEnumerable<KeyValuePair<T, double>> itemsAndWeights)
		{
			items = itemsAndWeights.ToDictionary(i => i.Key, i => i.Value);
			weightsSum = items.Sum(kvp => kvp.Value);
		}

		public int Count => items.Count;
		public double WeightsSum => weightsSum;

		public double GetWeight(T item)
		{
			return items[item];
		}

		public double GetWeightOrDefault(T item)
		{
			double weight;
			return items.TryGetValue(item, out weight) ? weight : 0;
		}

		public void Clear()
		{
			items.Clear();
			weightsSum = 0;
		}

		public void SetWeight(T item, double w)
		{
			weightsSum = weightsSum - items[item] + w;
			items[item] = w;
		}

		public void SetUniform()
		{
			foreach (var key in items.Keys)
				items[key] = 1;
			weightsSum = items.Count;
		}

		public void MultiplyWeight(T item, double mul)
		{
			SetWeight(item, items[item] * mul);
		}

		public T RandomElement(Random rnd)
		{
			if (weightsSum <= 0.00000000001) return items.RandomElement(rnd).Key;

			var p = rnd.NextDouble() * weightsSum;

			var last = default(T);
			foreach (var kvp in items)
			{
				var w = kvp.Value;

				if (w > p)
					return kvp.Key;

				p -= w;

				last = kvp.Key;
			}

			return last;
		}

		public IEnumerable<T> RandomElements(Random rnd, int count)
		{
			var result = new List<T>();
			var copy = new WeightedSet<T>(this);

			while (result.Count < Math.Min(count, Count))
			{
				var element = copy.RandomElement(rnd);
				copy.SetWeight(element, 0);
				result.Add(element);
			}

			return result;
		}

		public void Add(T item, double weight)
		{
			items.Add(item, weight);
			weightsSum += weight;
		}

		public void Increment(T item, double delta)
		{
			double weight;
			if (items.TryGetValue(item, out weight))
			{
				items[item] = weight + delta;
				weightsSum += delta;
			}
			else
			{
				items.Add(item, weight);
				weightsSum += delta;
			}
		}

		public void AddOrUpdate(T item, Func<double> onCreate, Func<double, double> onUpdate)
		{
			double weight;
			if (items.TryGetValue(item, out weight))
			{
				var newWeight = onUpdate(weight);
				items[item] = newWeight;
				weightsSum = weightsSum - weight + newWeight;
			}
			else
			{
				Add(item, onCreate());
			}
		}

		public void Remove(T item)
		{
			var weight = items[item];
			items.Remove(item);
			weightsSum -= weight;
		}

		public IEnumerator<KeyValuePair<T, double>> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}