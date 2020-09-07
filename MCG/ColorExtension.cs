using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MCG
{
    public static class ColorExtension
    {
        public static Color ToColor(this ConsoleColor color)
        {
			return color switch
			{
				ConsoleColor.Black => Color.Black,
				ConsoleColor.Blue => Color.Blue,
				ConsoleColor.Cyan => Color.Cyan,
				ConsoleColor.DarkBlue => Color.DarkBlue,
				ConsoleColor.DarkGray => Color.DarkGray,
				ConsoleColor.DarkGreen => Color.DarkGreen,
				ConsoleColor.DarkMagenta => Color.DarkMagenta,
				ConsoleColor.DarkRed => Color.DarkRed,
				ConsoleColor.DarkYellow => Color.FromArgb(255, 128, 128, 0),
				ConsoleColor.Gray => Color.Gray,
				ConsoleColor.Green => Color.Green,
				ConsoleColor.Magenta => Color.Magenta,
				ConsoleColor.Red => Color.Red,
				ConsoleColor.White => Color.White,
				ConsoleColor.DarkCyan => Color.DarkCyan,
				ConsoleColor.Yellow => Color.Yellow,
				_ => Color.White,
			};
		}
	}
}
