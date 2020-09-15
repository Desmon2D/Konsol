using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using static MCG.NativeDLL;

namespace MCG
{
	public static class Konsole
	{
		public static event Action<KeyboardEventHandler> KeyEvent;
		public static event Action<MenuEventHandler> MenuEvent;
		public static event Action<MouseEventHandler> MouseEvent;
		public static event Action<WindowBufferSizeEventHandler> WindowBufferSizeEvent;

		private static ConsoleCell[,] _buffer
			= new ConsoleCell[BufferHeight, BufferWidth];

		private static bool[,] _changedCells
			= new bool[BufferHeight, BufferWidth];

		private static readonly Lazy<IntPtr> _stdInputHandler
			= new Lazy<IntPtr>(() => GetStdHandle(STD_INPUT_HANDLE));

		public static int _bufferWidth = Console.WindowWidth;
		public static int BufferWidth
		{
			get => _bufferWidth;
			set
			{
				_buffer = new ConsoleCell[BufferHeight, value];
				_changedCells = new bool[BufferHeight, value];
				_bufferWidth = Console.WindowWidth = Console.BufferWidth = Console.WindowWidth = value;
			}
		}

		public static int _bufferHeight = Console.WindowHeight;
		public static int BufferHeight
		{
			get => Console.WindowHeight - 1;
			set
			{
				_buffer = new ConsoleCell[value, BufferWidth];
				_changedCells = new bool[value, BufferWidth];
				_bufferHeight = value;
				Console.WindowHeight = Console.BufferHeight = Console.WindowHeight = value + 1;
			}
		}

		public static bool ProcessedInputMode
		{
			get => GetMode(ENABLE_PROCESSED_INPUT);
			set => SetMode(ENABLE_PROCESSED_INPUT, value);
		}

		public static bool LineInputMode
		{
			get => GetMode(ENABLE_LINE_INPUT);
			set => SetMode(ENABLE_LINE_INPUT, value);
		}

		public static bool EchoInputMode
		{
			get => GetMode(ENABLE_ECHO_INPUT);
			set => SetMode(ENABLE_ECHO_INPUT, value);
		}
		public static bool WindowInputMode
		{
			get => GetMode(ENABLE_WINDOW_INPUT);
			set => SetMode(ENABLE_WINDOW_INPUT, value);
		}

		public static bool MouseInputMode
		{
			get => GetMode(ENABLE_MOUSE_INPUT);
			set => SetMode(ENABLE_MOUSE_INPUT, value);
		}

		public static bool InsertMode
		{
			get => GetMode(ENABLE_INSERT_MODE);
			set => SetMode(ENABLE_INSERT_MODE, value);
		}

		public static bool QuickEditMode
		{
			get => GetMode(ENABLE_QUICK_EDIT_MODE);
			set => SetMode(ENABLE_QUICK_EDIT_MODE, value);
		}

		public static bool VirtualTerminalInputMode
		{
			get => GetMode(ENABLE_VIRTUAL_TERMINAL_INPUT);
			set => SetMode(ENABLE_VIRTUAL_TERMINAL_INPUT, value);
		}

		public static bool CursorVisible
		{
			get => Console.CursorVisible;
			set => Console.CursorVisible = value;
		}

		public static Encoding Encoding
		{
			get => Console.OutputEncoding;
			set => Console.OutputEncoding = value;
		}

		private static void SetMode(uint code, bool enable)
		{
			uint mode = 0;
			var handler = GetStdHandle(STD_INPUT_HANDLE);

			GetConsoleMode(handler, ref mode);

			if (enable)
				mode |= code;
			else
				mode &= ~code;

			SetConsoleMode(handler, mode);
		}

		private static bool GetMode(uint code)
		{
			uint mode = 0;
			GetConsoleMode(_stdInputHandler.Value, ref mode);
			return (mode & code) == code;
		}

		public static void RemoveMenuButton(MenuButton button)
		{
			var handle = GetConsoleWindow();
			var sysMenu = GetSystemMenu(handle, false);

			if (handle != IntPtr.Zero)
				RemoveMenu(sysMenu, (uint)button, 0x00000000);
		}

		public static void PollEvent()
		{
			const uint countToRead = 5;
			var eventCountRed = 0u;
			var buffer = new INPUT_RECORD[countToRead];
			ReadConsoleInput(GetStdHandle(STD_INPUT_HANDLE), buffer, countToRead, ref eventCountRed);

			for (var i = 0; i < countToRead; i++)
				InvokeMethodByEventType(buffer[i]);
		}

		private static void InvokeMethodByEventType(INPUT_RECORD eventBuffer)
		{
			switch (eventBuffer.EventType)
			{
				case INPUT_RECORD.KEY_EVENT:
					KeyEvent?.Invoke(new KeyboardEventHandler(eventBuffer.KeyEvent));
					break;
				case INPUT_RECORD.MOUSE_EVENT:
					MouseEvent?.Invoke(new MouseEventHandler(eventBuffer.MouseEvent));
					break;
				case INPUT_RECORD.WINDOW_BUFFER_SIZE_EVENT:
					WindowBufferSizeEvent?.Invoke(new WindowBufferSizeEventHandler(eventBuffer.WindowBufferSizeEvent));
					break;
				case INPUT_RECORD.MENU_EVENT:
					MenuEvent?.Invoke(new MenuEventHandler(eventBuffer.MenuEvent));
					break;
			}
		}

		private static string GetForegroundColorCode(Color color)
			=> $"\x1b[38;2;{color.R};{color.G};{color.B}m";
		private static string GetBackgroundColorCode(Color color)
			=> $"\x1b[48;2;{color.R};{color.G};{color.B}m";
		private static string GetCellColorCode(Color foreground, Color background)
			=> $"\x1b[38;2;{foreground.R};{foreground.G};{foreground.B};48;2;{background.R};{background.G};{background.B}m";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Push(int x, int y, char ch, Color foreground, Color background)
		{
			var cell = new ConsoleCell(ch, foreground, background);

			if (cell != _buffer[y, x])
			{
				_buffer[y, x] = cell;
				_changedCells[y, x] = true;
			}
		}

		public static void Push(int x, int y, string str, Color foregroundColor, Color backgroundColor)
		{
			for (var i = 0; i < str.Length && i < BufferWidth; i++)
				Push(x + i, y, str[i], foregroundColor, backgroundColor);
		}

		public static void ClearBuffer()
		{
			for (var y = 0; y < BufferHeight; y++)
				for (var x = 0; x < BufferWidth; x++)
					_changedCells[y, x] = false;
		}

		public enum DrawMode
		{
			DrawAll,
			DrawByOne
		}

		public static void Draw(DrawMode drawMode)
		{
			switch (drawMode)
			{
				case DrawMode.DrawAll:
					DrawAll();
					break;
				case DrawMode.DrawByOne:
					DrawByOne();
					break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string CaretPositionCode(int x, int y)
			=> $"\x1B[{y};{x}H";

		public static void DrawAll()
		{
			var builder = new StringBuilder();

			builder.Append(CaretPositionCode(0, 0));

			for (var y = 0; y < BufferHeight; y++)
			{
				for (var x = 0; x < BufferWidth; x++)
				{
					if (x == 0 || _buffer[y, x - 1].ForegroundColor != _buffer[y, x].ForegroundColor || _buffer[y, x - 1].BackgroundColor != _buffer[y, x].BackgroundColor)
						builder.Append(GetCellColorCode(_buffer[y, x].ForegroundColor, _buffer[y, x].BackgroundColor));
					builder.Append(_buffer[y, x].Symbol);
				}

				builder.AppendLine(/*GetBackgroundColorCode(Color.Black)*/);
			}

			Console.Write(builder.ToString());
		}


		public static void DrawByOne()
		{
			//var builder = new StringBuilder();

			for (var y = 0; y < BufferHeight; y++)
			{
				for (int x = 0; x < BufferWidth; x++)
				{
					if (_changedCells[y, x])
					{
						//builder.Append(CaretPositionCode(x, y));
						//builder.Append(GetCellColorCode(_buffer[y, x].ForegroundColor, _buffer[y, x].BackgroundColor));
						//builder.Append(_buffer[y, x].Symbol);
						Console.SetCursorPosition(x, y);
						Console.Write(GetCellColorCode(_buffer[y, x].ForegroundColor, _buffer[y, x].BackgroundColor));
						Console.Write(_buffer[y, x].Symbol);
					}
				}
			}

			//Console.Write(builder.ToString());
		}
	}
}