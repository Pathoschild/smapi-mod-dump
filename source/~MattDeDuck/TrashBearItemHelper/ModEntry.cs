/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MattDeDuck/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Monsters;

using StardewValley;

namespace TrashBearName
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public bool bearTimerCheck = false;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonReleased += Input_ButtonReleased;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        private void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if(e.Button.IsActionButton())
            {
                // Set the timer check to false upon key release so it doesn't spam
                bearTimerCheck = false;
            }
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // ignore if the player can't move
            if (!Context.CanPlayerMove)
                return;

            // Check if the player is in the Forest area
            if (Game1.currentLocation.name == "Forest")
            {
                // Check if Trash Bear exists
                NPC tBear = null;
                foreach (var npc in Game1.currentLocation.characters)
                {
                    if (npc is TrashBear)
                    {
                        tBear = npc;
                    }
                }

                // If the bear doesn't exist...exit
                if (tBear == null)
                    return;

                // Grab the bubble timer from the Trash Bear
                int bearTimer = this.Helper.Reflection.GetField<int>(tBear, "showWantBubbleTimer").GetValue();

                if (bearTimerCheck == false)
                {
                    if (bearTimer > 2900 && bearTimer < 3000)
                    {
                        // Set the timer check to true to display once rather than spam
                        bearTimerCheck = true;

                        // Grab the item from the Trash Bear
                        int trashItemID = this.Helper.Reflection.GetField<int>(tBear, "itemWantedIndex").GetValue();
                        this.Monitor.Log("Item ID: " + trashItemID.ToString(), LogLevel.Debug);

                        // Load the object information dictionary
                        Dictionary<int, string> objData = Helper.Content.Load<Dictionary<int, string>>("Data/ObjectInformation", ContentSource.GameContent);

                        // Grab the item and category using the keyvalue in the dictionary
                        string wantedItem = objData[trashItemID];
                        string wantedItemName = wantedItem.Split('/')[0];
                        string wantedItemCategory = wantedItem.Split('/')[3].Split(' ')[0];

                        Game1.showGlobalMessage("Trash Bear wants\n" + wantedItemName + " - " + wantedItemCategory);
                        //Game1.addHUDMessage(new HUDMessage("Test", Color Blue));

                        this.Monitor.Log("Item: " + wantedItemName + " Type: " + wantedItemCategory, LogLevel.Debug);
                    }
                }
            }
        }
    }
}