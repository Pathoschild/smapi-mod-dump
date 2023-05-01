/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Artinity/SaloonWarpTotem
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;

namespace ClassLibrary1
{
    public class ModEntry : Mod
    {
        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /// <summary>
        /// Raised after the player presses a button on the keyboard, controller, or mouse.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || e.Button != SButton.MouseRight)
            {
                return;
            }

            if (Game1.player.ActiveObject?.ParentSheetIndex == 346 // Beer
                && Game1.player.ActiveObject.Stack == 12)
            {
                Game1.player.warpFarmer(new Warp(0, 0, "Saloon", 34, 18, false));
            }
        }
    }
}
