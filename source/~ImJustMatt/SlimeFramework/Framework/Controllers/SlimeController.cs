/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using ImJustMatt.SlimeFramework.Framework.Models;

namespace ImJustMatt.SlimeFramework.Framework.Controllers
{
    internal class SlimeController : SlimeModel
    {
        public string Data => string.Join("/",
            Health,
            DamageToFarmer,
            MinCoinsToDrop,
            MaxCoinsToDrop,
            false,
            DurationOfRandomMovements,
            "", // ObjectsToDrop
            Resilience,
            Jitteriness,
            MoveTowardPlayer,
            Speed,
            MissChance,
            true,
            ExperienceGained,
            DisplayName
        );
    }
}