using Microsoft.Win32.SafeHandles;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

/*
	https://stackoverflow.com/questions/2754518/how-can-i-write-fast-colored-output-to-console
	https://www.lihaoyi.com/post/BuildyourownCommandLinewithANSIescapecodes.html
	https://tforgione.fr/posts/ansi-escape-codes/
	https://notes.burke.libbey.me/ansi-escape-codes/
	https://commons.wikimedia.org/wiki/File:Xterm_256color_chart.svg
*/

namespace MCG
{
	public static class Program
	{
		public static Point MousePosition;
		public static Point MouseClickPosition;

		//☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼
		public static void Main(string[] args)
		{
			Konsole.CursorVisible = false;
			Konsole.QuickEditMode = false;
			Konsole.WindowInputMode = true;
			Konsole.MouseInputMode = true;
			Konsole.Encoding = Encoding.UTF8;
			Konsole.RemoveMenuButton(MenuButton.MaximizeButton);
			Konsole.RemoveMenuButton(MenuButton.MinimizeButton);
			Konsole.RemoveMenuButton(MenuButton.SizeButton);

			Konsole.BufferWidth = 20;
			Konsole.BufferHeight = 20;

			Konsole.MouseEvent += MousePositionChanged;			

			while (true)
			{
				Konsole.ClearBuffer();			

				for (var y = 0; y < Konsole.BufferHeight; y++)
					for (var x = 0; x < Konsole.BufferWidth; x++)
						Konsole.Push(x, y, ' ', Color.Empty, Color.FromArgb(x, 0, y));


				var hoverMe = "Hover me!";
				if(MousePosition.Y == Konsole.BufferHeight / 2 && MousePosition.X >= (int)(Konsole.BufferWidth / 2.0 - hoverMe.Length / 2.0) && MousePosition.X < (int)(Konsole.BufferWidth / 2.0 + hoverMe.Length / 2.0))
					Konsole.Push((int)(Konsole.BufferWidth / 2.0 - hoverMe.Length / 2.0) , Konsole.BufferHeight / 2, hoverMe, Color.White, Color.Black);
				else
					Konsole.Push((int)(Konsole.BufferWidth / 2.0 - hoverMe.Length / 2.0), Konsole.BufferHeight / 2, hoverMe, Color.Orange, Color.Black);

				var clickMe = "Click me!";
				if (MouseClickPosition.Y == Konsole.BufferHeight / 2 + 4 && MouseClickPosition.X >= (int)(Konsole.BufferWidth / 2.0 - clickMe.Length / 2.0) && MouseClickPosition.X < (int)(Konsole.BufferWidth / 2.0 + clickMe.Length / 2.0))
					Konsole.Push((int)(Konsole.BufferWidth / 2.0 - clickMe.Length / 2.0), Konsole.BufferHeight / 2 + 4, clickMe, Color.White, Color.Black);
				else
					Konsole.Push((int)(Konsole.BufferWidth / 2.0 - clickMe.Length / 2.0), Konsole.BufferHeight / 2 + 4, clickMe, Color.Orange, Color.Black);

				Konsole.PollEvent();

				Konsole.Push(0, 0, MousePosition.X.ToString() + ' ' + MousePosition.Y.ToString(), Color.White, Color.Black);
				Konsole.Push(0, 1, MouseClickPosition.X.ToString() + ' ' + MouseClickPosition.Y.ToString(), Color.White, Color.Black);

				Konsole.Draw();
			}
		}

		private static void MousePositionChanged(MouseEventHandler e)
		{
			MousePosition = e.Position;

			Konsole.Push(MousePosition.X, MousePosition.Y, 'X', Color.Orange, Color.Empty);

			if (e.ButtonState == ButtonState.FromLeftFirstButton)
				MouseClickPosition = e.Position;
		}
	}
}
