using System;
using System.Collections.Generic;

namespace ConsoleApplication1.DataTypes
{
	public struct Stat
	{
		public Stat(int opportunities, int facts)
			: this()
		{
			Opportunities = opportunities;
			Facts = facts;
			Ratio = Opportunities == 0 ? double.NaN : (double)Facts / Opportunities;
		}

		public Stat(int opportunities, double ratioInPercentage, bool ignored)
			: this()
		{
			Opportunities = opportunities;
			Facts = (int)(ratioInPercentage * Opportunities / 100);
			Ratio = Opportunities == 0 ? double.NaN : (ratioInPercentage / 100);
		}

		public Stat(double ratioInPercentage)
			: this(1000, ratioInPercentage, true)
		{
		}

		public int Opportunities { get; }
		public int Facts { get; }
		public double Ratio { get; }

		public int Failures => Opportunities - Facts;

		public override string ToString()
		{
			return $"{Ratio:0.###} ({Opportunities})";
		}

		public string ToFullDoubleString()
		{
			return $"{Ratio} ({Opportunities})";
		}

		public string ToPercentageString()
		{
			return $"{Ratio * 100:0.#}%";
		}

		public Stat IncreaseFact(bool isFactHappens)
		{
			return new Stat(Opportunities + 1, Facts + (isFactHappens ? 1 : 0));
		}

		public static Stat operator +(Stat a, Stat b)
		{
			return new Stat(a.Opportunities + b.Opportunities, a.Facts + b.Facts);
		}

		public Stat MultiplyImportance(double multiplier)
		{
			return new Stat((int)(Opportunities * multiplier), (int)(Facts * multiplier));
		}

		public double GetRatioOrZero()
		{
			return Opportunities == 0 ? 0 : Ratio;
		}

		public static readonly Stat Unknown = new Stat(0, 0);
	}

	public static class StatHelper
	{
		public static Stat ToStat<T>(this IEnumerable<T> seq, Func<T, bool> isSuccess)
		{
			var stat = Stat.Unknown;
			foreach (var item in seq)
				stat = stat.IncreaseFact(isSuccess(item));
			return stat;
		}

		public static Stat ToStat<T>(this IEnumerable<T> seq, Func<T, int, bool> isSuccess)
		{
			var stat = Stat.Unknown;
			var i = 0;
			foreach (var item in seq)
			{
				stat = stat.IncreaseFact(isSuccess(item, i));
				i++;
			}
			return stat;
		}

		public static Stat Sum(this IEnumerable<Stat> stats)
		{
			var agg = Stat.Unknown;
			foreach (var stat in stats)
				agg = agg + stat;
			return agg;
		}
	}
}