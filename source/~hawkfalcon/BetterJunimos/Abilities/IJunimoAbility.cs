/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using System.Collections.Generic;

namespace BetterJunimos.Abilities {
    /* 
     * Provides abilities for Junimos 
     */
    public interface IJunimoAbility {
        /*
         * What is the name of this ability 
         */
        string AbilityName();

        /*
         * Is the action available at the position? E.g. is the crop ready to harvest
         */
        bool IsActionAvailable(Farm farm, Vector2 pos, Guid guid);

        bool IsActionAvailable(GameLocation location, Vector2 pos, Guid guid) {
         return location is Farm farm && IsActionAvailable(farm, pos, guid);
        }

        /*
         * Action to take if it is available, return false if action failed
         */
        bool PerformAction(Farm farm, Vector2 pos, JunimoHarvester junimo, Guid guid);

        bool PerformAction(GameLocation location, Vector2 pos, JunimoHarvester junimo, Guid guid) {
         return location is Farm farm && PerformAction(farm, pos, junimo, guid);
        }

        /*
         * Does this action require specific items (or SObject.SeedsCategory, etc)?
         * Return empty list if no item needed
         */
        List<int> RequiredItems();
    }
}