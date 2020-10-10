/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Berisan/SpawnMonsters
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace Spawn_Monsters
{
    internal class MonsterSettingsTab : IClickableMenu
    {
        public MonsterSettingsTab(int x, int y, int width, int height) : base(x, y, width, height) {

            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls) {
                return;
            }

            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }
    }
}
