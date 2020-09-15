using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MCG
{
	internal static class NativeDLL
	{
		internal static class MenuButtons
		{
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
		}

		internal static class ConsoleMode
		{
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
			public static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] NativeData.INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsRead);
			[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
			public static extern bool WriteConsoleInput(IntPtr hConsoleInput, NativeData.INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsWritten);
		}

		internal static class NativeData
		{
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

				public const int RIGHT_ALT_PRESSED = 0x0001;
				public const int LEFT_ALT_PRESSED = 0x0002;
				public const int RIGHT_CTRL_PRESSED = 0x0004;
				public const int LEFT_CTRL_PRESSED = 0x0008;
				public const int SHIFT_PRESSED = 0x0010;
				public const int NUMLOCK_ON = 0x0020;
				public const int SCROLLLOCK_ON = 0x0040;
				public const int CAPSLOCK_ON = 0x0080;
				public const int ENHANCED_KEY = 0x0100;
				[FieldOffset(12)]
				public uint dwControlKeyState;
			}

			public struct MOUSE_EVENT_RECORD
			{
				public COORD dwMousePosition;

				public const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001;
				public const uint RIGHTMOST_BUTTON_PRESSED = 0x0002;
				public const uint FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004;
				public const uint FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008;
				public const uint FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010;
				public uint dwButtonState;

				public const int RIGHT_ALT_PRESSED = 0x0001;
				public const int LEFT_ALT_PRESSED = 0x0002;
				public const int RIGHT_CTRL_PRESSED = 0x0004;
				public const int LEFT_CTRL_PRESSED = 0x0008;
				public const int SHIFT_PRESSED = 0x0010;
				public const int NUMLOCK_ON = 0x0020;
				public const int SCROLLLOCK_ON = 0x0040;
				public const int CAPSLOCK_ON = 0x0080;
				public const int ENHANCED_KEY = 0x0100;
				public uint dwControlKeyState;

				public const int MOUSE_MOVED = 0x0001;
				public const int DOUBLE_CLICK = 0x0002;
				public const int MOUSE_WHEELED = 0x0004;
				public const int MOUSE_HWHEELED = 0x0008;
				public uint dwEventFlags;
			}

			public struct WINDOW_BUFFER_SIZE_RECORD
			{
				public COORD dwSize;

				public WINDOW_BUFFER_SIZE_RECORD(short x, short y)
				{
					this.dwSize = new COORD(x, y);
				}
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct MENU_EVENT_RECORD
			{
				public uint dwCommandId;
			}

			[StructLayout(LayoutKind.Sequential)]
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
				public const ushort MENU_EVENT = 0x0008;
				public const ushort FOCUS_EVENT = 0x0010;

				[FieldOffset(0)]
				public ushort EventType;
				[FieldOffset(4)]
				public KEY_EVENT_RECORD KeyEvent;
				[FieldOffset(4)]
				public MOUSE_EVENT_RECORD MouseEvent;
				[FieldOffset(4)]
				public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
				[FieldOffset(4)]
				public MENU_EVENT_RECORD MenuEvent;
				[FieldOffset(4)]
				public FOCUS_EVENT_RECORD FocusEvent;
			}
		}
	}
}