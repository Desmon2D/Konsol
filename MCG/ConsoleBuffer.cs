using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MCG
{
	public static class ConsoleBuffer
	{
		public const int ConsoleWidth = 100;
		public const int ConsoleHiegth = 40;

		private static ConsoleCell[,] _currentBuffer
			= new ConsoleCell[ConsoleWidth, ConsoleHiegth];

		private static ConsoleCell[,] _prevBuffer
			= new ConsoleCell[ConsoleWidth, ConsoleHiegth];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Push(int x, int y, char ch, ConsoleColor color = ConsoleColor.Gray)
			=> _currentBuffer[x, y] = new ConsoleCell(ch, color);

		private static void Swap<T>(ref T left, ref T rigth)
		{
			T temp = left;
			left = rigth;
			rigth = temp;
		}

		public static void Swap()
			=> Swap(ref _currentBuffer, ref _prevBuffer);

		public static void Draw()
		{
			for (var x = 0; x < ConsoleWidth; x++)
			{
				for (var y = 0; y < ConsoleHiegth; y++)
				{
					if (_prevBuffer[x, y] != _currentBuffer[x, y])
					{
						Console.SetCursorPosition(x, y);
						Console.Write(_currentBuffer[x, y].Symbol);
					}
				}
			}
		}

		public static void Clear()
		{
			for (var x = 0; x < ConsoleWidth; x++)
				for (var y = 0; y < ConsoleHiegth; y++)
					_currentBuffer[x, y] = default;
		}
	}
}