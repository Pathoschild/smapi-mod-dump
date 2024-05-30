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
using StardewValley;

namespace TeleportPoint.Framework.Gui;

public class NamingScreen : GuiScreen
{
    protected override void Init()
    {
        var x = Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 100;
        var y = Game1.graphics.GraphicsDevice.Viewport.Height / 2 - 35;
        var name = new TextField("",
            ModEntry.GetTranslation("teleportPoint.naming.textField.name.description"), x, y, 200, 70)
        {
            Text = Dialogue.randomName()
        };
        AddComponent(name);
        AddComponent(new Button(ModEntry.GetTranslation("teleportPoint.naming.button.done.title"),
            ModEntry.GetTranslation("teleportPoint.naming.button.done.title"), x, y + 100, 200, 80)
        {
            OnLeftClicked = () =>
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
            OnLeftClicked = () =>
            {
                if (PreviousMenu != null)
                {
                    OpenScreenGui(PreviousMenu);
                }
            }
        });
        base.Init();
    }
}