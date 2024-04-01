/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/Labeling
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using EnaiumToolKit.Framework.Utils;
using StardewValley;

namespace Labeling.Framework.Gui;

public class ColorGui : ScreenGui
{
    public ColorGui()
    {
        foreach (var variable in ModEntry.GetInstance().Config.Labelings)
        {
            AddElement(new ColorButton(variable.Name, variable.Name)
            {
                Color = ColorUtils.Instance.Get(variable.Color),
                OnLeftClicked = () => { OpenScreenGui(new SettingColorGui(variable)); }
            });
        }
    }
}

public class SettingColorGui : ScreenGui
{
    public SettingColorGui(Labeling labeling)
    {
        foreach (var variable in ColorUtils.Instance.Colors)
        {
            AddElement(new ColorButton(
                $"{ModEntry.GetInstance().GetTranslation("labeling.labelingGui.color")}:{variable.Name}",
                ModEntry.GetInstance().GetTranslation("labeling.labelingGui.color"))
            {
                Color = ColorUtils.Instance.Get(variable.Name),
                OnLeftClicked = () =>
                {
                    labeling.Color = variable.Name;
                    ModEntry.GetInstance().ConfigReload();
                    Game1.exitActiveMenu();
                }
            });
        }
    }
}