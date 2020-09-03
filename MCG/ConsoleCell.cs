using System;

namespace MCG
{
	public readonly struct ConsoleCell : IEquatable<ConsoleCell>
	{
		public readonly char Symbol;
		public readonly ConsoleColor Color;

		public ConsoleCell(char symbol, ConsoleColor color)
		{
			Color = color;
			Symbol = symbol;
		}

		public override bool Equals(object obj)
			=> obj is ConsoleCell cell && Equals(cell);
		
		public bool Equals(ConsoleCell other)
			=> Color == other.Color && Symbol == other.Symbol;

		public override int GetHashCode()
			=> HashCode.Combine(Color, Symbol);

		public static bool operator ==(ConsoleCell left, ConsoleCell right)
			=> left.Color == right.Color && left.Symbol == right.Symbol;

		public static bool operator !=(ConsoleCell left, ConsoleCell right)
			=> left.Color != right.Color || left.Symbol != right.Symbol;
	}
}
