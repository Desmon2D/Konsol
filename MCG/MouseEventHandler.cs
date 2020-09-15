using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MCG
{
	public class MouseEventHandler
	{
		public Point Position;
		public ButtonState ButtonState;
		public ControlKeyState ControlKeyState;
		public MouseEventFlag MouseEventFlag;

		internal MouseEventHandler(NativeDLL.MOUSE_EVENT_RECORD record)
		{
			Position = new Point(record.dwMousePosition.X, record.dwMousePosition.Y);
			ButtonState = (ButtonState)record.dwButtonState;
			ControlKeyState = (ControlKeyState)record.dwControlKeyState;
			MouseEventFlag = (MouseEventFlag)record.dwEventFlags;
		}
	}
}
