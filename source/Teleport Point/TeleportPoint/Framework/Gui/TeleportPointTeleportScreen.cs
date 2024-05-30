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
using EnaiumToolKit.Framework.Screen.Elements;
using StardewValley;

namespace TeleportPoint.Framework.Gui;

public class TeleportPointTeleportScreen : ScreenGui
{
    public TeleportPointTeleportScreen()
    {
        AddElement(new Label(Get("teleportPoint.label.teleportPointList.title"),
            Get("teleportPoint.label.teleportPointList.title")));

        foreach (var variable in ModEntry.Config.TeleportData)
        {
            AddElement(new Button($"{Get("teleportPoint.button.teleport.title")}:{variable.Name}",
                $"{Get("teleportPoint.button.teleport.title")}:{variable.Name}")
            {
                OnLeftClicked = () =>
                {
                    Game1.exitActiveMenu();
                    Game1.warpFarmer(variable.LocationName, (int)variable.TileX, (int)variable.TileY,
                        Game1.player.getFacingDirection());
                }
            });
        }
    }

    private string Get(string key)
    {
        return ModEntry.GetInstance().Helper.Translation.Get(key);
    }
}

public class TeleportData
{
    public string Name { get; }

    public string LocationName { get; }

    public float TileX { get; }

    public float TileY { get; }

    public TeleportData(string name, string locationName, float tileX, float tileY)
    {
        Name = name;
        LocationName = locationName;
        TileX = tileX;
        TileY = tileY;
    }
}