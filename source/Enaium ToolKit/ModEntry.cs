/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Gui;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace EnaiumToolKit
{
    public class ModEntry : Mod
    {
        private static ModEntry _instance;

        public ModEntry()
        {
            _instance = this;
        }

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPress;
        }

        private void OnButtonPress(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.RightControl)
            {
                Game1.activeClickableMenu = new EnaiumToolKitScreen();
            }
        }

        public static ModEntry GetInstance()
        {
            return _instance;
        }
    }
}