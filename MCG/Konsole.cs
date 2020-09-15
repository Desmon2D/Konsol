using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using static MCG.NativeDLL;

namespace MCG
{
	public static class Konsole
	{
		public static event Action<FocusEventHandler> FocusEvent;
		public static event Action<KeyboardEventHandler> KeyEvent;
		public static event Action<MenuEventHandler> MenuEvent;
		public static event Action<MouseEventHandler> MouseEvent;
		public static event Action<WindowBufferSizeEventHandler> WindowBufferSizeEvent;

		private static ConsoleCell[,] _buffer
			= new ConsoleCell[BufferHeight, BufferWidth];

		private static readonly Lazy<IntPtr> _stdInputHandler
			= new Lazy<IntPtr>(() => ConsoleMode.GetStdHandle(ConsoleMode.STD_INPUT_HANDLE));

		public static int BufferWidth
		{
			get => Console.WindowWidth;
			set
			{
				_buffer = new ConsoleCell[BufferHeight, value];
				Console.WindowWidth = Console.BufferWidth = Console.WindowWidth = value;
			}
		}

		public static int BufferHeight
		{
			get => Console.WindowHeight - 1;
			set
			{
				_buffer = new ConsoleCell[value, BufferWidth];
				Console.WindowHeight = Console.BufferHeight = Console.WindowHeight = value + 1;
			}
		}

		public static bool ProcessedInputMode
		{
			get => GetMode(ConsoleMode.ENABLE_PROCESSED_INPUT);
			set => SetMode(ConsoleMode.ENABLE_PROCESSED_INPUT, value);
		}

		public static bool LineInputMode
		{
			get => GetMode(ConsoleMode.ENABLE_LINE_INPUT);
			set => SetMode(ConsoleMode.ENABLE_LINE_INPUT, value);
		}

		public static bool EchoInputMode
		{
			get => GetMode(ConsoleMode.ENABLE_ECHO_INPUT);
			set => SetMode(ConsoleMode.ENABLE_ECHO_INPUT, value);
		}
		public static bool WindowInputMode
		{
			get => GetMode(ConsoleMode.ENABLE_WINDOW_INPUT);
			set => SetMode(ConsoleMode.ENABLE_WINDOW_INPUT, value);
		}

		public static bool MouseInputMode
		{
			get => GetMode(ConsoleMode.ENABLE_MOUSE_INPUT);
			set => SetMode(ConsoleMode.ENABLE_MOUSE_INPUT, value);
		}

		public static bool InsertMode
		{
			get => GetMode(ConsoleMode.ENABLE_INSERT_MODE);
			set => SetMode(ConsoleMode.ENABLE_INSERT_MODE, value);
		}

		public static bool QuickEditMode
		{
			get => GetMode(ConsoleMode.ENABLE_QUICK_EDIT_MODE);
			set => SetMode(ConsoleMode.ENABLE_QUICK_EDIT_MODE, value);
		}

		public static bool VirtualTerminalInputMode
		{
			get => GetMode(ConsoleMode.ENABLE_VIRTUAL_TERMINAL_INPUT);
			set => SetMode(ConsoleMode.ENABLE_VIRTUAL_TERMINAL_INPUT, value);
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
			ConsoleMode.GetConsoleMode(_stdInputHandler.Value, ref mode);

			if (enable)
				mode |= code;
			else
				mode &= ~code;

			ConsoleMode.SetConsoleMode(_stdInputHandler.Value, mode);
		}

		private static bool GetMode(uint code)
		{
			uint mode = 0;
			ConsoleMode.GetConsoleMode(_stdInputHandler.Value, ref mode);
			return (mode & code) != 0;
		}

		public static void RemoveMenuButton(MenuButton button)
		{
			var handle = MenuButtons.GetConsoleWindow();
			var sysMenu = MenuButtons.GetSystemMenu(handle, false);

			if (handle != IntPtr.Zero)
				MenuButtons.RemoveMenu(sysMenu, (uint)button, 0x00000000);
		}

		public static void PollEvent()
		{
			var eventCount = 0u;
			var eventBuffer = new NativeData.INPUT_RECORD[10];
			ConsoleMode.ReadConsoleInput(_stdInputHandler.Value, eventBuffer, (uint)eventBuffer.Length, ref eventCount);

			for (var i = 0; i < eventCount; i++)
				InvokeMethodByEventType(eventBuffer[i]);
		}

		private static void InvokeMethodByEventType(NativeData.INPUT_RECORD eventBuffer)
		{
			switch (eventBuffer.EventType)
			{
				case NativeData.INPUT_RECORD.MOUSE_EVENT:
					MouseEvent?.Invoke(new MouseEventHandler(eventBuffer.MouseEvent));
					break;
				case NativeData.INPUT_RECORD.KEY_EVENT:
					KeyEvent?.Invoke(new KeyboardEventHandler(eventBuffer.KeyEvent));
					break;
				case NativeData.INPUT_RECORD.WINDOW_BUFFER_SIZE_EVENT:
					WindowBufferSizeEvent?.Invoke(new WindowBufferSizeEventHandler(eventBuffer.WindowBufferSizeEvent));
					break;
				case NativeData.INPUT_RECORD.MENU_EVENT:
					MenuEvent?.Invoke(new MenuEventHandler(eventBuffer.MenuEvent));
					break;
				case NativeData.INPUT_RECORD.FOCUS_EVENT:
					FocusEvent?.Invoke(new FocusEventHandler(eventBuffer.FocusEvent));
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
			=> _buffer[y, x] = new ConsoleCell(ch, foreground, background);

		public static void Push(int x, int y, string str, Color foregroundColor, Color backgroundColor)
		{
			for (var i = 0; i < str.Length && i < BufferWidth; i++)
				_buffer[y, x + i] = new ConsoleCell(str[i], foregroundColor, backgroundColor);
		}

		public static void ClearBuffer()
		{
			for (var y = 0; y < BufferHeight; y++)
				for (var x = 0; x < BufferWidth; x++)
					_buffer[y, x] = default;
		}

		public static void Draw()
		{
			var builder = new StringBuilder();

			builder.Append("\x1B[0;0H");

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
	}
}