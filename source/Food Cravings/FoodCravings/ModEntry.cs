/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enteligenz/FoodCravings
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace FoodCravings
{
    internal sealed class ModEntry : Mod
    {
        Random rnd = new Random();
        StardewValley.Object DailyCravingItem;
        bool CravingFulfilled;
        // public Buff(int farming, int fishing, int mining, int digging, int luck, int foraging, int crafting, in maxStamina,
        // int magneticRadius, int speed, int defense, int attack, int minutesDuration, string source, string displaySource)
        Buff cravingBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 2, 0, "FoodCravings", "Craving fulfilled");
        Dictionary<string, string> recipeDict = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");


        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            this.cravingBuff.millisecondsDuration = 540000; // Buff lasts half an in-game day NOTE setting the time on init is weird, so we use this
        }


        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Randomize food craving
            var DailyCravingEntry = this.recipeDict.ElementAt(rnd.Next(0, this.recipeDict.Count));
            string DailyCravingValue = DailyCravingEntry.Value; // Food info

            // Get food object
            int DailyCravingId = int.Parse(DailyCravingValue.Split("/")[2]);
            DailyCravingItem = new StardewValley.Object(DailyCravingId, 1);
            Game1.addHUDMessage(new HUDMessage("Craving: " + DailyCravingItem.name, 2));

            // Reset flag
            this.CravingFulfilled = false;
        }

        private void OnUpdateTicked(object sender, EventArgs e)
        {
            if (Game1.player.isEating && !this.CravingFulfilled)
            {
                Item CurrentFood = Game1.player.itemToEat;
                if (this.DailyCravingItem.name.Equals(CurrentFood.Name))
                {
                    this.CravingFulfilled = true;

                    Game1.buffsDisplay.addOtherBuff(this.cravingBuff);
                }
            }
        }
    }
}