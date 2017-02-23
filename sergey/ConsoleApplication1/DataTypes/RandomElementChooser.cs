using System;

namespace ConsoleApplication1.DataTypes
{
	public class RandomElementChooser<T>
	{
		private readonly Random random;
		private int count;

		public T Current { get; private set; }

		public RandomElementChooser(Random random)
		{
			this.random = random;
			Current = default(T);
		}

		public void Reset()
		{
			Current = default(T);
			count = 0;
		}

		public void Advance(T element)
		{
			count++;
			if (random.Next(count) == 0)
				Current = element;
		}
	}
}