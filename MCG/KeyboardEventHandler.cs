namespace MCG
{
	public class KeyboardEventHandler
	{
		public bool IsKeyDown;
		public ushort RepeatCount;
		public ushort VirtualKeyCode;
		public ushort VirtualScanCode;
		public char UnicodeChar;
		public ControlKeyState ControlKeyState;

		internal KeyboardEventHandler(NativeDLL.NativeData.KEY_EVENT_RECORD record)
		{
			IsKeyDown = record.bKeyDown;
			RepeatCount = record.wRepeatCount;
			VirtualKeyCode = record.wVirtualKeyCode;
			VirtualScanCode = record.wVirtualScanCode;
			UnicodeChar = record.UnicodeChar;
			ControlKeyState = (ControlKeyState)record.dwControlKeyState;
		}
	}
}