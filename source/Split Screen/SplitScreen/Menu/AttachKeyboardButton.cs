namespace SplitScreen.Menu
{
	class AttachKeyboardButton : BaseTextButton
	{
		Keyboards.MultipleKeyboardManager keyboardManager;

		private bool awaitingKeypress = false;

		public AttachKeyboardButton(int x, int y, Keyboards.MultipleKeyboardManager keyboardManager) : base(x, y, "Attach keyboard")
		{
			this.keyboardManager = keyboardManager;

			base.isDisabled = PlayerIndexController._PlayerIndex != null;
		}

		public override void OnClicked()
		{
			if (!awaitingKeypress)
			{
				awaitingKeypress = true;
				base.text = "Press any key..";
			}
		}

		public void Update()
		{
			if (awaitingKeypress && keyboardManager.CheckKeyboardsToAttach())
			{
				awaitingKeypress = false;
				base.text = "Attach keyboard";
			}
		}
	}
}
