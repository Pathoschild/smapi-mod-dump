/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

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
