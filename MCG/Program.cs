using Microsoft.Win32.SafeHandles;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using static MCG.NativeMethods;

/*
	https://stackoverflow.com/questions/2754518/how-can-i-write-fast-colored-output-to-console
	https://www.lihaoyi.com/post/BuildyourownCommandLinewithANSIescapecodes.html
	https://tforgione.fr/posts/ansi-escape-codes/
	https://notes.burke.libbey.me/ansi-escape-codes/
	https://commons.wikimedia.org/wiki/File:Xterm_256color_chart.svg
*/

namespace MCG
{
	static class DisableConsoleQuickEdit
	{
		private const uint ENABLE_QUICK_EDIT = 0x0040;

		// STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
		private const int STD_INPUT_HANDLE = -10;

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll")]
		private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

		internal static bool Invoke()
		{

			var consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

			// get current console mode
			if (!GetConsoleMode(consoleHandle, out uint consoleMode))
			{
				// ERROR: Unable to get console mode.
				return false;
			}

			// Clear the quick edit bit in the mode flags
			consoleMode &= ~ENABLE_QUICK_EDIT;

			// set the new mode
			if (!SetConsoleMode(consoleHandle, consoleMode))
			{
				// ERROR: Unable to set console mode
				return false;
			}

			return true;
		}
	}


	public static class Program
	{
		public const int SC_CLOSE = 0xF060;
		public const int SC_MINIMIZE = 0xF020;
		public const int SC_MAXIMIZE = 0xF030;
		public const int SC_SIZE = 0xF000;

		[DllImport("user32.dll")]
		public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

		[DllImport("user32.dll")]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);

		public struct POINT
		{
			public int X;
			public int Y;
		}

		public static NativeMethods.COORD MousePosition;
		public static NativeMethods.COORD MouseClickPosition;

		//☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼
		public static void Main(string[] args)
		{
			var handle = GetConsoleWindow();
			var sysMenu = GetSystemMenu(handle, false);
			
			if (handle != IntPtr.Zero)
			{
				//DeleteMenu(sysMenu, SC_CLOSE, 0x00000000);
				DeleteMenu(sysMenu, SC_MINIMIZE, 0x00000000);
				DeleteMenu(sysMenu, SC_MAXIMIZE, 0x00000000);
				DeleteMenu(sysMenu, SC_SIZE, 0x00000000);
			}
			
			//DisableConsoleQuickEdit.Invoke();


			Console.CursorVisible = false;
			Console.OutputEncoding = Encoding.UTF8;
			Console.WindowWidth = Console.BufferWidth = Console.WindowWidth = ConsoleBuffer.Width;
			Console.WindowHeight = Console.BufferHeight = Console.WindowHeight = ConsoleBuffer.Hiegth + 1;

			IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
			uint mode = 0;
			GetConsoleMode(inHandle, ref mode);
			mode &= ~ENABLE_QUICK_EDIT_MODE; //disable
			mode |= ENABLE_WINDOW_INPUT; //enable (if you want)
			mode |= ENABLE_MOUSE_INPUT; //enable
			SetConsoleMode(inHandle, mode);

			ConsoleListener.MouseEvent += MousePositionChanged;
			ConsoleListener.Start();

			while (true)
			{
				ConsoleBuffer.Swap();
				ConsoleBuffer.Clear();

				for (var y = 0; y < ConsoleBuffer.Hiegth; y++)
					for (var x = 0; x < ConsoleBuffer.Width; x++)
						ConsoleBuffer.Push(x, y, ' ', Color.Empty, Color.FromArgb(x, 0, y));

				//GetCursorPos(out POINT point);
				ConsoleBuffer.Push(0, 0, MousePosition.X.ToString() + ' ' + MousePosition.Y.ToString(), Color.White, Color.Black);
				ConsoleBuffer.Push(0, 1, MouseClickPosition.X.ToString() + ' ' + MouseClickPosition.Y.ToString(), Color.White, Color.Black);

				ConsoleBuffer.Draw(ConsoleDrawMode.DrawByOne);
			}
		}

		private static void MousePositionChanged(NativeMethods.MOUSE_EVENT_RECORD r)
		{
			MousePosition = r.dwMousePosition;

			if (r.dwButtonState == NativeMethods.MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED)
				MouseClickPosition = r.dwMousePosition;
		}
	}
}
