using StardewValley.Menus;
using StardewValley;

namespace Spawn_Monsters
{
	class MonsterSettingsTab : IClickableMenu
	{
		public MonsterSettingsTab() : base(0,0,0,0) {
			this.width = 600;
			this.height = 600;
			this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
			this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2;


			this.initializeUpperRightCloseButton();
			if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
				return;
			this.populateClickableComponentList();
			this.snapToDefaultClickableComponent();
		}
	}
}
