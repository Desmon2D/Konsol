using System.Drawing;
using static MCG.NativeDLL;

namespace MCG
{
	public class WindowBufferSizeEventHandler
	{
		public Point NewSize;

		internal WindowBufferSizeEventHandler(WINDOW_BUFFER_SIZE_RECORD record)
			=> NewSize = new Point(record.dwSize.X, record.dwSize.Y);
	}
}