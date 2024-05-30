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

namespace Labeling.Framework.Gui;

public class ColorGui : ScreenGui
{
    public ColorGui()
    {
        foreach (var variable in ModEntry.GetInstance().Config.Labelings)
        {
            AddElement(new Label(variable.Name, variable.Name));
            AddElement(new ColorPicker(variable.Name, variable.Name, variable.Color)
            {
                OnCurrentChanged = (color) =>
                {
                    variable.Color = color;
                }
            });
        }
    }
}