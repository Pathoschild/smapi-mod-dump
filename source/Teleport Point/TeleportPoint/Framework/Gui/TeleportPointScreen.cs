/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/TeleportPoint
**
*************************************************/

using System.Collections.Generic;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace TeleportPoint.Framework.Gui
{
    public class TeleportPointScreen : ScreenGui
    {
        public TeleportPointScreen()
        {
            AddElement(new Button(ModEntry.GetTranslation("teleportPoint.button.record.title"),
                ModEntry.GetTranslation("teleportPoint.button.record.title"))
            {
                OnLeftClicked = () =>
                {
                    Game1.activeClickableMenu = new NamingScreen();
                }
            });
            AddElement(new Button($"{ModEntry.GetTranslation("teleportPoint.button.teleport.title")}",
                $"{ModEntry.GetTranslation("teleportPoint.button.teleport.title")}")
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new TeleportPointTeleportScreen(); }
            });

            AddElement(new Button($"{ModEntry.GetTranslation("teleportPoint.button.delete.title")}",
                $"{ModEntry.GetTranslation("teleportPoint.button.delete.title")}")
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = new TeleportPointDeleteScreen(); }
            });
        }
    }
}