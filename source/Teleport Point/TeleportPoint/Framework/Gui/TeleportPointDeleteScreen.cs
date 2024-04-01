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

namespace TeleportPoint.Framework.Gui
{
    public class TeleportPointDeleteScreen : ScreenGui
    {
        public TeleportPointDeleteScreen()
        {
            AddElement(new Label(ModEntry.GetTranslation("teleportPoint.label.teleportPointList.title"),
                ModEntry.GetTranslation("teleportPoint.label.teleportPointList.title")));

            foreach (var variable in ModEntry.Config.TeleportData)
            {
                Button delete = new Button($"{ModEntry.GetTranslation("teleportPoint.button.delete.title")}:{variable.Name}",
                    $"{ModEntry.GetTranslation("teleportPoint.button.delete.title")}:{variable.Name}");
                delete.OnLeftClicked = () =>
                {
                    ModEntry.Config.TeleportData.Remove(variable);
                    RemoveElement(delete);
                };
                AddElement(delete);
            }
        }
    }
}