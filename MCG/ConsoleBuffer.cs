using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MCG
{
	public enum ConsoleDrawMode
	{
		DrawByOne,
		DrawAll
	}

	public static class ConsoleBuffer
	{
		public const int Width = 80;
		public const int Hiegth = 40;

		private static ConsoleCell[,] _currentBuffer
			= new ConsoleCell[Width, Hiegth + 1];

		//TODO: Заменить на bool[x, y] где сохранять изменявшиеся ячейки
		private static ConsoleCell[,] _prevBuffer
			= new ConsoleCell[Width, Hiegth];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Push(int x, int y, char ch, Color foregroundColor, Color backgroundColor)
			=> _currentBuffer[x, y] = new ConsoleCell(ch, foregroundColor, backgroundColor);

		public static void Push(int x, int y, string str, Color foregroundColor, Color backgroundColor)
		{
			for (var i = 0; i < str.Length && i < Width; i++)
				_currentBuffer[x + i, y] = new ConsoleCell(str[i], foregroundColor, backgroundColor);
		}

		private static void Swap<T>(ref T left, ref T rigth)
		{
			T temp = left;
			left = rigth;
			rigth = temp;
		}

		public static void Swap()
			=> Swap(ref _currentBuffer, ref _prevBuffer);

		/*
			"\x1b[38;2;" + r + ";" + g + ";" + b + "m" - set foreground by r, g, b values <br/>
			"\x1b[38;5;" + s + "m" - set foreground color by index in table(0-255) <br/>

			"\x1b[48;2;" + r + ";" + g + ";" + b + "m" - set background by r, g, b values <br/>
			"\x1b[48;5;" + s + "m" - set background color by index in table (0-255) <br/>
		*/
		private static Color _currentForegroundColor = ConsoleColor.Gray.ToColor();
		private static string GetForegroundColorCode(Color color)
			=> $"\x1b[38;2;{color.R};{color.G};{color.B}m";
		private static void ChangeForegroundColor(Color color)
		{
			if (_currentForegroundColor == color)
				return;

			_currentForegroundColor = color;
			Console.Write(GetForegroundColorCode(color));
		}


		private static Color _currentBackgroundColor = ConsoleColor.Black.ToColor();
		private static string GetBackgroundColorCode(Color color)
			=> $"\x1b[48;2;{color.R};{color.G};{color.B}m";
		private static void ChangeBackgroundColor(Color color)
		{
			if (_currentBackgroundColor == color)
				return;

			_currentBackgroundColor = color;

			Console.Write(GetBackgroundColorCode(color));
		}

		public static string GetCellColorCode(Color foreground, Color background)
			=> $"\x1b[38;2;{foreground.R};{foreground.G};{foreground.B};48;2;{background.R};{background.G};{background.B}m";

		public static void Draw(ConsoleDrawMode mode)
		{
			if (mode == ConsoleDrawMode.DrawByOne)
			{
				for (var y = 0; y < Hiegth; y++)
				{
					for (var x = 0; x < Width; x++)
					{
						if (_prevBuffer[x, y] != _currentBuffer[x, y])
						{
							ChangeForegroundColor(_currentBuffer[x, y].ForegroundColor);
							ChangeBackgroundColor(_currentBuffer[x, y].BackgroundColor);
							Console.SetCursorPosition(x, y);
							Console.WriteLine(_currentBuffer[x, y].Symbol);
						}
					}
				}
			}

			else if (mode == ConsoleDrawMode.DrawAll)
			{
				var builder = new StringBuilder();

				builder.Append("\x1B[0;0H");

				for (var y = 0; y < Hiegth; y++)
				{
					for (var x = 0; x < Width; x++)
					{
						if (x == 0 || _currentBuffer[x - 1, y].ForegroundColor != _currentBuffer[x, y].ForegroundColor || _currentBuffer[x - 1, y].BackgroundColor != _currentBuffer[x, y].BackgroundColor)
							builder.Append(GetCellColorCode(_currentBuffer[x, y].ForegroundColor, _currentBuffer[x, y].BackgroundColor));
						builder.Append(_currentBuffer[x, y].Symbol);
					}

					builder.AppendLine(GetBackgroundColorCode(Color.Black));
				}

				Console.Write(builder.ToString());
			}

		}

		public static void Clear()
		{
			for (var x = 0; x < Width; x++)
				for (var y = 0; y < Hiegth; y++)
					_currentBuffer[x, y] = default;
		}
	}
}