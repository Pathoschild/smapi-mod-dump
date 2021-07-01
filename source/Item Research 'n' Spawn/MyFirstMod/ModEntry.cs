/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MyFirstMod
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
            }
        }
    }
}