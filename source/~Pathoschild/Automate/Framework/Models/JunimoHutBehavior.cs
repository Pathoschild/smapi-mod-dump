/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.Automate.Framework.Models
{
    /// <summary>Indicates how Junimo huts should be automated for a specific item type.</summary>
    internal enum JunimoHutBehavior
    {
        /// <summary>Apply the default logic based on the player's installed mods (e.g. leave seeds in the hut if Better Junimos is installed).</summary>
        AutoDetect,

        /// <summary>Ignore items of this type, so they're not transferred either way.</summary>
        Ignore,

        /// <summary>Move any items of this type from the Junimo Hut into connected chests.</summary>
        MoveIntoChests,

        /// <summary>Move any items of this type from connected chests into the Junimo Hut.</summary>
        MoveIntoHut
    }
}
