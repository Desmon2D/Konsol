using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MCG
{
	internal static class NativeDLL
	{
		//Buttons
		public const int SC_SIZE = 0xF000;
		public const int SC_MINIMIZE = 0xF020;
		public const int SC_MAXIMIZE = 0xF030;
		public const int SC_CLOSE = 0xF060;

		[DllImport("user32.dll")]
		internal static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
		[DllImport("user32.dll")]
		internal static extern int RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("kernel32.dll", ExactSpelling = true)]
		internal static extern IntPtr GetConsoleWindow();

		//Mode
		public const uint STD_INPUT_HANDLE = unchecked((uint)-10);
		public const uint STD_OUTPUT_HANDLE = unchecked((uint)-11);
		public const uint STD_ERROR_HANDLE = unchecked((uint)-12);
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetStdHandle(uint nStdHandle);

		public const uint ENABLE_PROCESSED_INPUT = 0x0001;
		public const uint ENABLE_LINE_INPUT = 0x0002;
		public const uint ENABLE_ECHO_INPUT = 0x0004;
		public const uint ENABLE_WINDOW_INPUT = 0x0008;
		public const uint ENABLE_MOUSE_INPUT = 0x0010;
		public const uint ENABLE_INSERT_MODE = 0x0020;
		public const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
		public const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;
		[DllImport("kernel32.dll")]
		public static extern bool GetConsoleMode(IntPtr hConsoleInput, ref uint lpMode);

		[DllImport("kernel32.dll")]
		public static extern bool SetConsoleMode(IntPtr hConsoleInput, uint dwMode);


		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsRead);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		public static extern bool WriteConsoleInput(IntPtr hConsoleInput, INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsWritten);

		public struct COORD
		{
			public short X;
			public short Y;

			public COORD(short x, short y)
			{
				X = x;
				Y = y;
			}
		}

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct KEY_EVENT_RECORD
		{
			[FieldOffset(0)]
			public bool bKeyDown;
			[FieldOffset(4)]
			public ushort wRepeatCount;
			[FieldOffset(6)]
			public ushort wVirtualKeyCode;
			[FieldOffset(8)]
			public ushort wVirtualScanCode;
			[FieldOffset(10)]
			public char UnicodeChar;
			[FieldOffset(10)]
			public byte AsciiChar;

			public const int CAPSLOCK_ON = 0x0080,
				ENHANCED_KEY = 0x0100,
				LEFT_ALT_PRESSED = 0x0002,
				LEFT_CTRL_PRESSED = 0x0008,
				NUMLOCK_ON = 0x0020,
				RIGHT_ALT_PRESSED = 0x0001,
				RIGHT_CTRL_PRESSED = 0x0004,
				SCROLLLOCK_ON = 0x0040,
				SHIFT_PRESSED = 0x0010;
			[FieldOffset(12)]
			public uint dwControlKeyState;
		}

		public struct MOUSE_EVENT_RECORD
		{
			public COORD dwMousePosition;

			public const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001,
				FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004,
				FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008,
				FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010,
				RIGHTMOST_BUTTON_PRESSED = 0x0002;
			public uint dwButtonState;

			public const int CAPSLOCK_ON = 0x0080,
				ENHANCED_KEY = 0x0100,
				LEFT_ALT_PRESSED = 0x0002,
				LEFT_CTRL_PRESSED = 0x0008,
				NUMLOCK_ON = 0x0020,
				RIGHT_ALT_PRESSED = 0x0001,
				RIGHT_CTRL_PRESSED = 0x0004,
				SCROLLLOCK_ON = 0x0040,
				SHIFT_PRESSED = 0x0010;
			public uint dwControlKeyState;

			public const int DOUBLE_CLICK = 0x0002,
				MOUSE_HWHEELED = 0x0008,
				MOUSE_MOVED = 0x0001,
				MOUSE_WHEELED = 0x0004;
			public uint dwEventFlags;
		}

		public struct WINDOW_BUFFER_SIZE_RECORD
		{
			public COORD dwSize;
		}

		public struct MENU_EVENT_RECORD
		{
			public uint dwCommandId;
		}

		public struct FOCUS_EVENT_RECORD
		{
			public bool bSetFocus;
		}

		/// <summary>
		/// https://docs.microsoft.com/en-gb/windows/console/input-record-str
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		public struct INPUT_RECORD
		{
			public const ushort KEY_EVENT = 0x0001;
			public const ushort MOUSE_EVENT = 0x0002;
			public const ushort WINDOW_BUFFER_SIZE_EVENT = 0x0004;
			//public const ushort MENU_EVENT = 0x0008;
			//public const ushort FOCUS_EVENT = 0x0010;

			[FieldOffset(0)]
			public ushort EventType;
			[FieldOffset(4)]
			public KEY_EVENT_RECORD KeyEvent;
			[FieldOffset(4)]
			public MOUSE_EVENT_RECORD MouseEvent;
			[FieldOffset(4)]
			public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
			//[FieldOffset(4)]
			//public MENU_EVENT_RECORD MenuEvent;
			//[FieldOffset(4)]
			//public FOCUS_EVENT_RECORD FocusEvent;
		}
	}
}