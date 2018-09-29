namespace SplitScreen.Menu
{
	class DetachMouseButton : BaseTextButton
	{
		Mice.MultipleMiceManager miceManager;

		public DetachMouseButton(int x, int y, Mice.MultipleMiceManager miceManager) : base(x, y, "Detach mouse")
		{
			this.miceManager = miceManager;

			base.isDisabled = PlayerIndexController._PlayerIndex != null;
		}

		public override void OnClicked()
		{
			miceManager.DetachMouseButtonClicked();
		}
	}
}
