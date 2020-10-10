/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

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
