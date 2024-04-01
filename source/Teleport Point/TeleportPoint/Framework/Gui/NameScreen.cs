/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/TeleportPoint
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace TeleportPoint.Framework.Gui
{
    public class NamingScreen : GuiScreen
    {
        public NamingScreen()
        {
            var x = Game1.uiViewport.Width / 2 - 100;
            var y = Game1.uiViewport.Height / 2 - 35;
            var name = new TextField("",
                ModEntry.GetTranslation("teleportPoint.naming.textField.name.description"), x, y, 200, 70)
            {
                Text = Dialogue.randomName()
            };
            AddComponent(name);
            AddComponent(new Button(ModEntry.GetTranslation("teleportPoint.naming.button.done.title"),
                ModEntry.GetTranslation("teleportPoint.naming.button.done.title"), x, y + 100, 200, 80)
            {
                OnLeftClicked = delegate()
                {
                    ModEntry.Config.TeleportData.Add(new TeleportData(name.Text, Game1.player.currentLocation.Name,
                        Game1.player.Tile.X, Game1.player.Tile.Y));
                    ModEntry.ConfigReload();
                    Game1.exitActiveMenu();
                }
            });
            AddComponent(new Button(ModEntry.GetTranslation("teleportPoint.naming.button.cancel.title"),
                ModEntry.GetTranslation("teleportPoint.naming.button.cancel.title"), x, y + 200, 200, 80)
            {
                OnLeftClicked = new Action(Game1.exitActiveMenu)
            });
        }
    }
}