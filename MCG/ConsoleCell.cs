using System;
using System.Runtime.CompilerServices;

namespace MCG
{
	public struct Color : IEquatable<Color>
	{
		// 00 RR GG BB
		public int RGB;

		public Color(byte r, byte g, byte b)
		{
			RGB = (r << 16) | (g << 8) | b;
		}

		public Color(int rgb)
		{
			RGB = rgb;
		}

		internal static readonly Color White = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
		internal static readonly Color Black = new Color(0, 0, 0);
		internal static readonly Color Orange = new Color(180, 90 , 60);

		public override bool Equals(object obj)
			=> obj is Color color && Equals(color);

		public bool Equals(Color other)
			=> RGB == other.RGB;

		public override int GetHashCode()
			=> RGB;

		public static bool operator ==(Color left, Color right)
			=> left.RGB == right.RGB;

		public static bool operator !=(Color left, Color right)
			=> left.RGB != right.RGB;
	}

	public struct ConsoleCell : IEquatable<ConsoleCell>
	{
		public Color BackgroundColor;
		public Color ForegroundColor;
		public char Symbol;

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