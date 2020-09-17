using Microsoft.Win32.SafeHandles;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static MCG.NativeDLL;

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
		public static ConsoleCell[,] ConsoleBackground;

		public static void Main(string[] args)
		{
			Konsole.CursorVisible = false;
			Konsole.QuickEditMode = false;
			Konsole.MouseInputMode = true;
			Konsole.WindowInputMode = true;
			Konsole.BufferWidth = 150;
			Konsole.BufferHeight = 45;

			ConsoleBackground = new ConsoleCell[Konsole.BufferWidth, Konsole.BufferHeight];
			for (byte y = 0; y < Konsole.BufferHeight; y++)
				for (byte x = 0; x < Konsole.BufferWidth; x++)
					ConsoleBackground[x, y] = new ConsoleCell(' ', new Color(0), new Color(x, 0, y));

			Konsole.Encoding = Encoding.UTF8;
			Konsole.RemoveMenuButton(MenuButton.MaximizeButton);
			Konsole.RemoveMenuButton(MenuButton.MinimizeButton);
			Konsole.RemoveMenuButton(MenuButton.SizeButton);

			
			Konsole.MouseEvent += MousePositionChanged;


			while (true)
				Konsole.PollEvent();
		}

		private static void MousePositionChanged(MouseEventHandler e)
		{
			MousePosition = e.Position;

			if (e.ButtonState == ButtonState.FromLeftFirstButton)
				MouseClickPosition = e.Position;

			//////////////////////////////////////
			for (byte y = 0; y < Konsole.BufferHeight; y++)
				for (byte x = 0; x < Konsole.BufferWidth; x++)
					Konsole.Push(x, y, ConsoleBackground[x, y]);


			var hoverMe = "Hover me!";
			if (MousePosition.Y == Konsole.BufferHeight / 2 && MousePosition.X >= (int)(Konsole.BufferWidth / 2.0 - hoverMe.Length / 2.0) && MousePosition.X < (int)(Konsole.BufferWidth / 2.0 + hoverMe.Length / 2.0))
				Konsole.Push((int)(Konsole.BufferWidth / 2.0 - hoverMe.Length / 2.0), Konsole.BufferHeight / 2, hoverMe, Color.White, Color.Black);
			else
				Konsole.Push((int)(Konsole.BufferWidth / 2.0 - hoverMe.Length / 2.0), Konsole.BufferHeight / 2, hoverMe, Color.Orange, Color.Black);

			var clickMe = "Click me!";
			if (MouseClickPosition.Y == Konsole.BufferHeight / 2 + 4 && MouseClickPosition.X >= (int)(Konsole.BufferWidth / 2.0 - clickMe.Length / 2.0) && MouseClickPosition.X < (int)(Konsole.BufferWidth / 2.0 + clickMe.Length / 2.0))
				Konsole.Push((int)(Konsole.BufferWidth / 2.0 - clickMe.Length / 2.0), Konsole.BufferHeight / 2 + 4, clickMe, Color.White, Color.Black);
			else
				Konsole.Push((int)(Konsole.BufferWidth / 2.0 - clickMe.Length / 2.0), Konsole.BufferHeight / 2 + 4, clickMe, Color.Orange, Color.Black);

			Konsole.Push(MousePosition.X, MousePosition.Y - 2, MousePosition.X.ToString() + ' ' + MousePosition.Y.ToString(), Color.White, Color.Black);
			Konsole.Push(0, 1, MouseClickPosition.X.ToString() + ' ' + MouseClickPosition.Y.ToString(), Color.White, Color.Black);

			Konsole.Push(MousePosition.X, MousePosition.Y, new ConsoleCell('X', Color.Orange, Color.Black));

			Konsole.Draw(Konsole.DrawMode.DrawOnlyChanged);
		}
	}
}