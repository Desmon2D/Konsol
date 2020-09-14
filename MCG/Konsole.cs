using System;
using System.Collections.Generic;
using System.Text;
using static MCG.NativeDLL;

namespace MCG
{


	public static class Konsole
	{
		private static readonly Lazy<IntPtr> StdInputHandler
			= new Lazy<IntPtr>(() => ConsoleMode.GetStdHandle(ConsoleMode.STD_INPUT_HANDLE));

		public static void PollEvent()
		{
			var eventCount = 0u;
			var eventBuffer = new NativeData.INPUT_RECORD[5];
			ConsoleMode.ReadConsoleInput(StdInputHandler.Value, eventBuffer, (uint)eventBuffer.Length, ref eventCount);

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

		public static event Action<MouseEventHandler> MouseEvent;
		public static event Action<KeyboardEventHandler> KeyEvent;
		public static event Action<WindowBufferSizeEventHandler> WindowBufferSizeEvent;
		public static event Action<MenuEventHandler> MenuEvent;
		public static event Action<FocusEventHandler> FocusEvent;
	}
}