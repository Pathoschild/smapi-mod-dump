using StardewModdingAPI;

namespace SplitScreen.Menu
{
	class AttachMouseButton : BaseTextButton
	{
		Mice.MultipleMiceManager miceManager;

		public AttachMouseButton(int x, int y, Mice.MultipleMiceManager miceManager) : base(x,y, "Attach mouse")
		{
			this.miceManager = miceManager;

			base.isDisabled = PlayerIndexController._PlayerIndex != null;
		}

		public override void OnClicked()
		{
			Monitor.Log("Attach button clicked", LogLevel.Trace);
			miceManager.AttachMouseButtonClicked();
		}
	}
}
