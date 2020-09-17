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
		static Konsole()
		{
			_bufferWidth = Console.WindowWidth;
			_bufferHeight = Console.WindowHeight;

			ResizeBuffer();
			_stdInputHandler = GetStdHandle(STD_INPUT_HANDLE);
		}

		public static event Action<KeyboardEventHandler> KeyEvent;
		public static event Action<MenuEventHandler> MenuEvent;
		public static event Action<MouseEventHandler> MouseEvent;
		public static event Action<WindowBufferSizeEventHandler> WindowBufferSizeEvent;

		private static ConsoleCell[][] _currentBuffer;
		private static ConsoleCell[][] _prevBuffer;
		private static readonly IntPtr _stdInputHandler;


		public static int _bufferWidth;
		public static int BufferWidth
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _bufferWidth;
			set
			{
				_bufferWidth = Console.WindowWidth = Console.BufferWidth = Console.WindowWidth = value;
				ResizeBuffer();
			}
		}

		public static int _bufferHeight;
		public static int BufferHeight
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _bufferHeight;
			set
			{
				_bufferHeight = value;
				Console.WindowHeight = Console.BufferHeight = Console.WindowHeight = value + 1;
				ResizeBuffer();
			}
		}

		private static void ResizeBuffer()
		{
			_currentBuffer = new ConsoleCell[_bufferHeight][];
			for (var i = 0; i < _currentBuffer.Length; i++)
				_currentBuffer[i] = new ConsoleCell[_bufferWidth];

			_prevBuffer = new ConsoleCell[_bufferHeight][];
			for (var i = 0; i < _prevBuffer.Length; i++)
				_prevBuffer[i] = new ConsoleCell[_bufferWidth];
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
			GetConsoleMode(_stdInputHandler, ref mode);
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
			const uint countToRead = 1;
			var eventCountRed = 0u;
			var buffer = new INPUT_RECORD[countToRead];
			ReadConsoleInput(GetStdHandle(STD_INPUT_HANDLE), buffer, countToRead, ref eventCountRed);

			//for (var i = 0; i < countToRead; i++)
			InvokeMethodByEventType(buffer[0]);
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
			=> $"\x1b[38;2;{(color.RGB >> 16) & 0xFF};{(color.RGB >> 8) & 0xFF};{color.RGB & 0xFF}m";
		private static string GetBackgroundColorCode(Color color)
			=> $"\x1b[48;2;{(color.RGB >> 16) & 0xFF};{(color.RGB >> 8) & 0xFF};{color.RGB & 0xFF}m";
		private static string GetCellColorCode(Color foreground, Color background)
			=> $"\x1b[38;2;{(foreground.RGB >> 16) & 0xFF};{(foreground.RGB >> 8) & 0xFF};{foreground.RGB & 0xFF};48;2;{(background.RGB >> 16) & 0xFF};{(background.RGB >> 8) & 0xFF};{background.RGB & 0xFF}m";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Push(int x, int y, in ConsoleCell cell)
			=> _currentBuffer[y][x] = cell;

		public static void Push(int x, int y, string str, Color foregroundColor, Color backgroundColor)
		{
			for (var i = 0; i < str.Length && i < _bufferWidth; i++)
				Push(x + i, y, new ConsoleCell(str[i], foregroundColor, backgroundColor));
		}

		private static void Swap<T>(ref T a, ref T b)
		{
			var temp = a;
			a = b;
			b = temp;
		}

		public enum DrawMode
		{
			DrawAll,
			DrawOnlyChanged
		}

		public static void Draw(DrawMode drawMode)
		{
			switch (drawMode)
			{
				case DrawMode.DrawAll:
					DrawAll();
					break;
				case DrawMode.DrawOnlyChanged:
					DrawOnlyChanged();
					break;
			}

			Swap(ref _currentBuffer, ref _prevBuffer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string CaretPositionCode(int x, int y)
			=> $"\x1B[{y + 1};{x + 1}H";

		public static void DrawAll()
		{
			var builder = new StringBuilder();

			builder.Append(CaretPositionCode(0, 0));

			for (int y = 0; y < _bufferHeight; y++)
			{
				for (int x = 0; x < _bufferWidth; x++)
				{
					if (x == 0 || _currentBuffer[y][x].ForegroundColor != _currentBuffer[y][x - 1].ForegroundColor || _currentBuffer[y][x].BackgroundColor != _currentBuffer[y][x - 1].BackgroundColor)
						builder.Append(GetCellColorCode(_currentBuffer[y][x].ForegroundColor, _currentBuffer[y][x].BackgroundColor));

					builder.Append(_currentBuffer[y][x].Symbol);
				}
			}

			builder.AppendLine();

			Console.Write(builder.ToString());
		}


		public static void DrawOnlyChanged()
		{

			var builder = new StringBuilder();

			for (int y = 0; y < _bufferHeight; y++)
			{
				for (int x = 0; x < _bufferWidth; x++)
				{
					if (_currentBuffer[y][x] != _prevBuffer[y][x])
					{
						builder.Append(CaretPositionCode(x, y));
						builder.Append(GetCellColorCode(_currentBuffer[y][x].ForegroundColor, _currentBuffer[y][x].BackgroundColor));
						builder.Append(_currentBuffer[y][x].Symbol);
					}
				}
			}

			Console.Write(builder.ToString());
		}
	}
}