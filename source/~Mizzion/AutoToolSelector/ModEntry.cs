/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Tools;

namespace AutoToolSelector
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += ButtonPressed;
            helper.Events.GameLoop.OneSecondUpdateTicked += OneSecondUpdateTicked;
        }

        private void OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {

        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;


        }
    }
}
