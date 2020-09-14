using static MCG.NativeDLL;

namespace MCG
{
	public class FocusEventHandler
	{
		public bool InFocus;

		internal FocusEventHandler(NativeData.FOCUS_EVENT_RECORD record)
			=> InFocus = record.bSetFocus;
	}
}