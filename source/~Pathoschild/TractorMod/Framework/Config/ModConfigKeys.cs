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

namespace Pathoschild.Stardew.TractorMod.Framework.Config
{
    /// <summary>A set of parsed key bindings.</summary>
    internal class ModConfigKeys
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The keys which summon the tractor.</summary>
        public KeybindList SummonTractor { get; set; } = new(SButton.Back);

        /// <summary>The keys which return the tractor to its home.</summary>
        public KeybindList DismissTractor { get; set; } = new(SButton.Back);

        /// <summary>The keys which activate the tractor when held, or none to activate automatically.</summary>
        public KeybindList HoldToActivate { get; set; } = new();
    }
}
