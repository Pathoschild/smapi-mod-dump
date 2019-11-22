using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Minigames;
using SpouseStuff.Spouses;

namespace SpouseStuff
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private Dictionary<string, ISpouseRoom> spouses = new Dictionary<string, ISpouseRoom>
        {
            { "Abigail", new AbigailRoom() },
            { "Alex", new AlexRoom() },
            { "Harvey", new HarveyRoom() },
            { "Maru", new MaruRoom() },
            { "Sam", new SamRoom() },
            { "Penny", new PennyRoom() },
            { "Leah", new LeahRoom() },
            { "Sebastian", new SebastianRoom() },
            { "Shane", new ShaneRoom() },
            { "Elliott", new ElliottRoom() },
            { "Emily", new EmilyRoom() },
            { "Haley", new HaleyRoom() }
        };

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPress;
        }

        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPress(object sender, ButtonPressedEventArgs e)
        {
            // ignore under most circumstances
            if (!Context.IsWorldReady || !e.Button.IsActionButton() || Game1.currentLocation.Name != "FarmHouse" || !Game1.player.isMarried() || !spouses.ContainsKey(Game1.player.spouse))
            {
                return;
            }

            // Uncomment this next line to log spots to interact with in the house
            //this.Monitor.Log($"{Game1.player.getTileX()}x{Game1.player.getTileY()}/{Game1.player.FacingDirection}.");

            spouses[Game1.player.spouse].InteractWithSpot(Game1.player.getTileX(), Game1.player.getTileY(), Game1.player.FacingDirection);
        }

    }
}
