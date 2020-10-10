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
