/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using StardewValley;

namespace EnaiumToolKit.Framework.Gui
{
    public class EnaiumToolKitScreen : ScreenGui
    {
        public EnaiumToolKitScreen() : base("Enaium toolKit")
        {
            AddElement(new Button("ElementScreen", "Element test")
            {
                OnLeftClicked = () => { OpenScreenGui(new ElementScreen()); }
            });

            AddElement(new Button("ComponentScreen", "Component test")
            {
                OnLeftClicked = () => { OpenScreenGui(new ComponentScreen()); }
            });
        }

        private string GetTranslation(string key)
        {
            return ModEntry.GetInstance().Helper.Translation.Get(key);
        }
    }
}