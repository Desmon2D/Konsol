using System;
using System.Drawing;

namespace MCG
{
	public readonly struct ConsoleCell : IEquatable<ConsoleCell>
	{
		public readonly char Symbol;
		public readonly Color ForegroundColor;
		public readonly Color BackgroundColor;

		public ConsoleCell(char symbol, Color foregroundColor, Color backgroundColor)
		{
			Symbol = symbol;
			ForegroundColor = foregroundColor;
			BackgroundColor = backgroundColor;
		}

		public override bool Equals(object obj)
			=> obj is ConsoleCell cell && Equals(cell);

		public bool Equals(ConsoleCell other)
			=> Symbol == other.Symbol && ForegroundColor == other.ForegroundColor && BackgroundColor == other.BackgroundColor;

		public override int GetHashCode()
			=>  HashCode.Combine(Symbol, ForegroundColor, BackgroundColor);

		public static bool operator ==(ConsoleCell left, ConsoleCell right)
			=> left.Symbol == right.Symbol && left.ForegroundColor == right.ForegroundColor && left.BackgroundColor == right.BackgroundColor;

		public static bool operator !=(ConsoleCell left, ConsoleCell right)
			=> left.Symbol != right.Symbol || left.ForegroundColor != right.ForegroundColor || left.BackgroundColor != right.BackgroundColor;
	}
}