/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using TehPers.CoreMod.Api.Items.Recipes;

namespace TehPers.CoreMod.Api.Items.Events {
    public class RecipeCraftedEventArgs : EventArgs {
        /// <summary>The name of the recipe.</summary>
        public string RecipeName { get; }

        /// <summary>Whether the recipe is a cooking recipe. False indicates it is a normal crafting recipe.</summary>
        public bool IsCooking { get; }

        /// <summary>The total number of times the recipe has been crafted, including this time.</summary>
        public int TotalTimesCrafted { get; }

        /// <summary>The recipe being crafted.</summary>
        public IRecipe Recipe { get; }

        public RecipeCraftedEventArgs(string recipeName, bool isCooking, int totalTimesCrafted, IRecipe recipe) {
            this.RecipeName = recipeName;
            this.IsCooking = isCooking;
            this.TotalTimesCrafted = totalTimesCrafted;
            this.Recipe = recipe;
        }
    }
}