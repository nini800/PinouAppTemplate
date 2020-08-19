namespace Pinou.InputSystem
{
	public class UI_PinouInputReceiver : PinouInputReceiver
	{
		public void ReceiveAbsoluteFocus_Additive()
		{
			base.ReceiveAbsoluteFocus(FocusMode.Additive);
		}
		public void ReceiveAbsoluteFocus_Exclusive()
		{
			base.ReceiveAbsoluteFocus(FocusMode.Exclusive);
		}
	}
}