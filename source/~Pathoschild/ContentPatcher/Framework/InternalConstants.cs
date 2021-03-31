/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley.GameData;
using StardewValley.GameData.Crafting;
using StardewValley.GameData.FishPond;
using StardewValley.GameData.Movies;

namespace ContentPatcher.Framework
{
    /// <summary>Internal constant values.</summary>
    internal static class InternalConstants
    {
        /*********
        ** Fields
        *********/
        /// <summary>The character used as a separator between the token name and positional input arguments.</summary>
        public const string PositionalInputArgSeparator = ":";

        /// <summary>The character used as a separator between the token name (or positional input arguments) and named input arguments.</summary>
        public const string NamedInputArgSeparator = "|";

        /// <summary>The character used as a separator between the mod ID and token name for a mod-provided token.</summary>
        public const string ModTokenSeparator = "/";

        /// <summary>A prefix for player names when specified as an input argument.</summary>
        public const string PlayerNamePrefix = "@";

        /// <summary>A temporary value assigned to input arguments during early patch loading, before the patch is updated with the context values.</summary>
        public const string TokenPlaceholder = "$~placeholder";


        /*********
        ** Methods
        *********/
        /// <summary>Get the key for a list asset entry.</summary>
        /// <typeparam name="TValue">The list value type.</typeparam>
        /// <param name="entity">The entity whose ID to fetch.</param>
        /// <param name="reflection">Simplifies dynamic access to game code.</param>
        public static string GetListAssetKey<TValue>(TValue entity, IReflectionHelper reflection)
        {
            switch (entity)
            {
                case ConcessionItemData entry:
                    return entry.ID.ToString();

                case ConcessionTaste entry:
                    return entry.Name;

                case FishPondData entry:
                    return string.Join(",", entry.RequiredTags);

                case MovieCharacterReaction entry:
                    return entry.NPCName;

                case RandomBundleData entry:
                    return entry.AreaName;

                case TailorItemRecipe entry:
                    return string.Join(",", entry.FirstItemTags) + "|" + string.Join(",", entry.SecondItemTags);

                default:
                    {
                        var property = reflection.GetProperty<object>(entity, "ID", required: false);
                        if (property != null)
                            return property.GetValue()?.ToString();

                        var field = reflection.GetField<object>(entity, "ID", required: false);
                        if (field != null)
                            return field.GetValue()?.ToString();

                        throw new NotSupportedException($"No ID implementation for list asset value type {typeof(TValue).FullName}.");
                    }
            }
        }
    }
}
