using System;
using System.Collections.Generic;
using System.Text;

namespace MCG
{
	public enum MenuButton : uint
	{
		SizeButton = 0xF000,
		MinimizeButton = 0xF020,
		MaximizeButton = 0xF030,
		CloseButton = 0xF060,
	}

	public enum ButtonState : uint
	{
		None = default,
		FromLeftFirstButton = 0x0001,
		RightmostButton = 0x0002,
		FromLeftSecondButton = 0x0004,
		FromLeftThirdButton = 0x0008,
		FromLeftFourthButton = 0x0010,
	}

	public enum ControlKeyState : uint
	{
		None = default,
		RightAlt = 0x0001,
		LeftAlt = 0x0002,
		RightCtrl = 0x0004,
		LeftCtrl = 0x0008,
		Shift = 0x0010,
		NumlockOn = 0x0020,
		ScrolllockOn = 0x0040,
		CapslockOn = 0x0080,
		EnhancedKey = 0x0100,
	}

	public enum MouseEventFlag : uint
	{
		None = default,
		MouseMoved = 0x0001,
		DoubleClick = 0x0002,
		MouseWheeled = 0x0004,
		MouseHwheeled = 0x0008,
	}
}
