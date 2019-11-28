using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Locations;

namespace CraneManBegone
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Checking to make sure no menu is up, we are in the theater with a weapon equipped,
            //   the cursor is pointing at the man, the man is actually there, and it's a left click
            if (Context.IsWorldReady &&
                Game1.activeClickableMenu == null &&
                Game1.currentLocation is MovieTheater &&
                Game1.player.CurrentTool is MeleeWeapon &&
                e.Cursor.GrabTile.X == 2 &&
                (e.Cursor.GrabTile.Y == 8 || e.Cursor.GrabTile.Y == 9 ) &&
                Game1.currentLocation.getTileIndexAt(2, 9, "Buildings") == 215 &&
                e.Button.IsUseToolButton())
            {
                this.Monitor.Log("Banishing Crane Man into the void!", LogLevel.Trace);
                Game1.playSound("thunder");
                // Multiple smoke effects to cover a larger area
                int SmokeX = 2 * 64 + 24;
                int SmokeY = 9 * 64 + 16;
                int ScaleY = 12;
                Utility.addSmokePuff(Game1.currentLocation, new Vector2(SmokeX + 8, SmokeY - ScaleY * 5));
                Utility.addSmokePuff(Game1.currentLocation, new Vector2(SmokeX + 2, SmokeY - ScaleY * 4));
                Utility.addSmokePuff(Game1.currentLocation, new Vector2(SmokeX + 4, SmokeY - ScaleY * 3));
                Utility.addSmokePuff(Game1.currentLocation, new Vector2(SmokeX - 2, SmokeY - ScaleY * 2));
                Utility.addSmokePuff(Game1.currentLocation, new Vector2(SmokeX , SmokeY - ScaleY));
                Utility.addSmokePuff(Game1.currentLocation, new Vector2(SmokeX - 6, SmokeY));
                // Actual removal of the man
                Game1.currentLocation.removeTile(2, 8, "Front");
                Game1.currentLocation.removeTile(2, 9, "Buildings");
                Game1.currentLocation.removeTileProperty(2, 9, "Buildings", "Action");
            }
        }

    }
}
