/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace ContentPatcher.Framework.TriggerActions
{
    /// <summary>A data ID type which can be migrated using <see cref="MigrateIdsAction"/>.</summary>
    public enum MigrateIdType
    {
        /// <summary>Migrate cooking recipe IDs.</summary>
        CookingRecipes,

        /// <summary>Migrate crafting recipe IDs.</summary>
        CraftingRecipes,

        /// <summary>Migrate event IDs.</summary>
        Events,

        /// <summary>Migrate item local IDs.</summary>
        Items,

        /// <summary>Migrate mail IDs.</summary>
        Mail,

        /// <summary>Migrate songs-heard cue names.</summary>
        Songs
    }
}
