namespace SplitScreen.Menu
{
	class DetachKeyboardButton : BaseTextButton
	{
		Keyboards.MultipleKeyboardManager keyboardManager;

		public DetachKeyboardButton(int x, int y, Keyboards.MultipleKeyboardManager keyboardManager) : base(x, y, "Detach keyboard")
		{
			this.keyboardManager = keyboardManager;

			base.isDisabled = PlayerIndexController._PlayerIndex != null;
		}

		public override void OnClicked()
		{
			keyboardManager.OnDetachButtonClicked();
		}
	}
}
