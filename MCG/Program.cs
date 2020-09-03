using System;
using System.Threading;

namespace MCG
{
	public static class Program
	{
		public static int x = 1;
		public static int y = 1;

		public static void Main(string[] args)
		{
			Console.CursorVisible = false;
			//Console.
			Console.BufferWidth = Console.WindowWidth = ConsoleBuffer.ConsoleWidth;
			Console.BufferHeight = Console.WindowHeight = ConsoleBuffer.ConsoleHiegth;			

			while (true)
			{
				ConsoleBuffer.Clear();

				ConsoleBuffer.Push(x, y, 'X');

				ConsoleBuffer.Draw();
				ConsoleBuffer.Swap();

				ExecuteControl();
			}
		}

		private static void ExecuteControl()
		{
			var ch = Console.ReadKey(true).KeyChar;

			//x
			if (ch == '1' || ch == '4' || ch == '7')
				x = Math.Max(0, x - 1);

			else if (ch == '3' || ch == '6' || ch == '9')
				x = Math.Min(ConsoleBuffer.ConsoleWidth - 1, x + 1);

			//y
			if (ch == '7' || ch == '8' || ch == '9')
				y = Math.Max(0, y - 1);

			else if (ch == '1' || ch == '2' || ch == '3')
				y = Math.Min(ConsoleBuffer.ConsoleHiegth - 1, y + 1);
		}
	}
}
