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

namespace Labeling.Framework.Gui
{
    public class DisplayGui : ScreenGui
    {
        public DisplayGui()
        {
            foreach (var variable in ModEntry.GetInstance().Config.Labelings)
            {
                var toggle = new ToggleButton(variable.Name, variable.Name)
                {
                    Toggled = variable.Display
                };
                toggle.OnLeftClicked = () =>
                {
                    variable.Display = toggle.Toggled;
                    ModEntry.GetInstance().ConfigReload();
                };
                AddElement(toggle);
            }
        }
    }
}