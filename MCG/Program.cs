using System;
using System.Drawing;
using System.Runtime.CompilerServices;
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
		public static int playerX = 1;
		public static int playerY = 1;

		//☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼
		public static void Main(string[] args)
		{
			Console.CursorVisible = false;
			Console.OutputEncoding = Encoding.UTF8;
			Console.BufferWidth = Console.WindowWidth = ConsoleBuffer.Width;
			Console.BufferHeight = Console.WindowHeight = ConsoleBuffer.Hiegth + 1;

			var map = new bool[ConsoleBuffer.Width, ConsoleBuffer.Hiegth];

			bool forward = false;
			byte deltaRGB = 0;

			while (true)
			{
				ConsoleBuffer.Clear();

				if (deltaRGB >= 100)
					forward = false;
				
				if (deltaRGB <= 30)
					forward = true;

				if (forward)
					deltaRGB += 5;
				else
					deltaRGB -= 5;

				/*
				for (int x = 0; x < ConsoleBuffer.Width; x++)
				{
					for (int y = 0; y < ConsoleBuffer.Hiegth; y++)
					{
						var r = (byte)(byte.MaxValue * ((float)x / ConsoleBuffer.Width));
						var g = (byte)(1.0f / byte.MaxValue * ((float)y / ConsoleBuffer.Hiegth * x / ConsoleBuffer.Width));
						var b = (byte)(byte.MaxValue * ((float)y / ConsoleBuffer.Hiegth));

						var color = Color.FromArgb(r, g, b);

						ConsoleBuffer.Push(x, y, '.', color, Color.Black);
					}
				}*/

				for (int x = 0; x < ConsoleBuffer.Width; x++)
					for (int y = 0; y < ConsoleBuffer.Hiegth; y++)
						if(map[x, y])
							ConsoleBuffer.Push(x, y, 'X', Color.White, Color.Black);

				var splitBilly = BILLY.Split('\n');
				for (int y = 0; y < splitBilly.Length; y++)
				{
					for (int x = 0; x < splitBilly[y].Length; x++)
					{
						var r = (byte)(byte.MaxValue * ((float)x / ConsoleBuffer.Width));
						var g = (byte)(1.0f / byte.MaxValue * ((float)y / ConsoleBuffer.Hiegth * x / ConsoleBuffer.Width));
						var b = (byte)(byte.MaxValue * ((float)y / ConsoleBuffer.Hiegth));

						var delta = -100 + deltaRGB;

						var foregroundColor = Color.FromArgb(r, g, b);
						var backgroundColor = Color.FromArgb(Math.Max(0, r + delta), Math.Max(0, g + delta), Math.Max(0, b + delta));


						ConsoleBuffer.Push(x, y, splitBilly[y][x], foregroundColor, backgroundColor);
					}
				}
				//ConsoleBuffer.Push(playerX, playerY, '♂', Color.White, Color.Black);

				var str = "♂ GACHI REMIX ♂";
				for (var i = 0; i < str.Length; i++)
					ConsoleBuffer.Push(ConsoleBuffer.Width / 2 - str.Length / 2 - 1 + i, 11, str[i], Color.White, Color.Black);

				ConsoleBuffer.Draw(ConsoleDrawMode.DrawAll);
				ConsoleBuffer.Swap();

				//ExecuteControl(map);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsValidPosition(bool[,] map, int x, int y)
			=> x >= 0 && y >= 0 && x < ConsoleBuffer.Width && y < ConsoleBuffer.Hiegth && !map[x, y];

		private static void ExecuteControl(bool[,] map)
		{
			var ch = Console.ReadKey(true).KeyChar;

			var deltaX = 0;
			var deltaY = 0;

			if (ch == '1' || ch == '4' || ch == '7')
				deltaX = -1;

			else if (ch == '3' || ch == '6' || ch == '9')
				deltaX = 1;

			if (ch == '7' || ch == '8' || ch == '9')
				deltaY = -1;

			else if (ch == '1' || ch == '2' || ch == '3')
				deltaY = 1;

			if (IsValidPosition(map, playerX + deltaX, playerY + deltaY))
			{
				playerX += deltaX;
				playerY += deltaY;
			}
		}
	public const string BILLY = @"OOOOOOOOOOOOOOOOOOOO0KNWNKd,.   ..      .    .          .........';oOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
OOOOOOOOOOOOOOOOOOkxONNNXkc..  .                              ....'ckOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
OOOOOOOOOOOOOOOOkdooodkkxl;....    ........                       .,oOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
OOOOOOOOOOOOOOOOkoc:,',;::,'..   ...',''''''............            .oOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
OOOOOOOOOOOOOOOkkd:,'...'',;'....';,;;:ccccccc:;;,,,,'''...          ,xOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
OOOOOOOOOOOO00kddoc;,,',;cloolc;;clllcodddxxkkxdocc:;;,,,''..        .lOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
OOOOOOOOOOOOOkdlc:::;;cclxOKKKOolodxxdxxxOOOOOkkxdooc;;;;,,'...       .dOOOOOOOOOOOOOOOOOOOOOOOOOOOO
OOOOOO0OO0OOOxoc:::::okxxkO0KXKkddxxxkkk00OkOOOkxdooc::;;;;;,'...     .lOOOOOOOOOOOOOOOOOOOOOOOOOOOO
OO00000O00OO0kollllloOKOO0000OxddxkOOOkOKKOkkkkxxdollc::;;;;;,'...     :OOOOOOOOOOOOOOOOOOOOOOOOOOOO
O00000OOO000OocoddooxO0000KK0kkxxO000000KK0Okxkkkxddolc::;;;;;,'....   'x0O0OOOO0OOOOOOOOOOOOOOOOOOO
O00000000000Ol;:lddxkk0KK00OkOKKKXXXKKKKKKK0kkkOkxxddolcc:;;;::;,'...  .cO000000000000OOOOOO0OOOO000
000000000000Od;,:lloxkOK0KK0KKXXNNNNXKK0KKK0Okxxxdoolccccc::;;:::;,...  .d0000000000000O000000OOO000
0000000000000kc'',;cox0KKKXXXXXNNNNNXK0OO000kdlcc:;;,''''''',,;::::,...  :O0000000000000000000000000
00000000000000d;''';lk0KKKXXXNNNNNXK0Okkxkkkxl;''........',,,,,;;;;;'....,dO000000000000000000000000
00000000000000Ol'.';cdOKKKXXXXXXKOxolccloooooc:;;;,,'',,;:::::;;;;:;'....;:lk00000000000000000000000
000000000000000kc'';clk0KXXXKOxoc;...',,;cllccllc:;,'.....'',;;;;:::,. .,c;,ck0000000000000000000000
0000000000000000k;';:lkKKK0ko:,....''',,;cdxdlclc,........''',,,;;;;;;..;ll:,lO000000000000000000000
00000000000000000l.',ckKK0dc;,,,,,''...';l0XOdlc:,'...',,;;,,,,;;;::::;..;:c::d000000000000000000000
00000000000000000O:.':kKK0o:cl:,........:kXNKko:;;;;,,''''''',,;;::::::,.',,;;lO00000000000000000000
000000000000000000Oc',o0K0dodc,'''''..,lOKXNKkoc:;:;:::;;;,,,,;;;::ccc:,.,,';:ck00000000000000000000
00000000000000000000d;:xK0kkxdxoc:;;cd0XXKXNX0dl::::cccccccc::;:::ccccc;',::ccoO00000000000000000000
000000000000000000K0kl:o0KK0KK0OOOOKNWNNXXXNNXOxoc:cooc::::ccccccclllcc:;;clccx000000000000000000000
000000000000000000K0xdxk0KKKKKXXNNWWNKOkKNXXNKOxocclool::;;,;:clllllllcc:;;:coO000000000000000000000
0000KK000000000000KKkk0kk00000KKXXXKOdco0XK0Oxollc:::::;;;;,,,;:llllcccc:;:lxO0000000000000000000000
0000KK00K00000KKKKKK0Okook00O0KKK0Oxl:,;oko:::ccl:;;;,'',',,,,,;:clc::ccc:cx000000000000000000000000
000KKKKKKKKKK0KKKKKKK00kxkkOO000Okdl:;'':dl'.',;;;,,,,,,,,;;:;;;;;cc:;:::::x000000000000000000000000
0KKKKKKKKKKKKKKKKKKKKKXXNKOOOOOkxol:;,''lkkdc;,,'''',;:::c:;;,',;;;cc;;::::d0K000KK00000000000000000
KKKKKKKKKKKKKKKKKKKKKKXNNX0kOOkddoc:,'':dkxdol:;;;;:ccllooc. ..'',;cc;,;:::d0KK00KKK0000000000000000
KKKKKKKKKKKKKKKKKKKKKKKXXXX0kkdlodl:,',:lc::::::llcldxxxol'..',,,',:c;;;:;:xKKK0KKKKKKK000KKK00KKK00
KKKKKKKKKKKKKKKKKKKKKKKKKK00Okxlcddc,,,,'...;loodddoooc;'...,;;;,',::;,;;;:xKKKKKKKKKKKK0KKKK000KK0K
KKKKKKKKKKKKKKKKKKKKKKKKKKKK0kxocldo:;,'...';:ccc::;,'',,,;;:::;,',;;,,;;,:xKKKKKKKKKKK00KKKKKK0KKKK
KKKKKKKKKKKKKKKKKKKKKKKKKKKKK0xolcodl:,'',cxOOOOkxxdoc::::::::;,'',,,,;;,,;d0KKKKKKKKKKKKKKKKKKKKKKK
KKKKKKKKKKKKKKKKKKKKKKKKKKKKKK0xlclol:,,,:dkkkxxdolc:;;,,;;:;;,'''',,,,,,;;oOKKKKKKKKKKKKKKKKKKKKKKK
KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK0xl::::;,,:oxxxdol:;;,''''',,''''',,,,,,,;;;cxKKKKKKKKKKKKKKKKKKKKKKK
KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK0xc:;,;;;:codxxdolc:;,,,;;;,,,'',,,,,,,;;:::cdOKKKKKKKKKKKKKKKKKKKKK
KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK0xoc;;;::lodxkOOkkkkxxdddl:;,,,,,,,,,;;;::::;cx0KKKKKKKKKKKKKKKKKKK
KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKOxdlc:::clodxkkkOkkxolc:;;;,,,''',,,;;;:::::;;lx0KKKKKKKKKKKKKKKKK
KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKOkxdlc:::cllolcccc:;;,,,,,'''''',,,;;;;:::::;,;cx0KKKKKKKKKKKKKKK
KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK0Okkxoc:;;,,;;,,''',,,,''''''',,,,,;;;::::::;,,,,:ok0KKKKKKKKKKKK
KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK0OOkdlc;,,'''''''''',,,,,,,,',',,,,;;;::::::;,,,'',:ldO0KKKKKKKK
KKKKKKKKKKKKKKKKKKKKKKKKKXKKKKKKKKKKK0Okxdlc:,,'''''''',,,,,,,,,,,,,,,,;;;;:::::;;,,',,,,,:dOKKKKKKK";
	}

}
