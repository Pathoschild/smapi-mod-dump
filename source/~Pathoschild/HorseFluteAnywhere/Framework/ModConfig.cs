/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace Pathoschild.Stardew.HorseFluteAnywhere.Framework
{
    /// <summary>The raw mod configuration.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The keys which play the flute and summon the horse.</summary>
        public KeybindList SummonHorseKey { get; set; } = KeybindList.ForSingle(SButton.H);

        /// <summary>Whether the player must be holding a horse flute to summon the horse.</summary>
        public bool RequireHorseFlute { get; set; } = false;
    }
}
