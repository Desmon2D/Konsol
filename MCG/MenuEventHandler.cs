using static MCG.NativeDLL;

namespace MCG
{
	public class MenuEventHandler
	{
		public uint Command;

		internal MenuEventHandler(MENU_EVENT_RECORD record)
		{
			Command = record.dwCommandId;
		}
	}
}