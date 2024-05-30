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
using System.Diagnostics.CodeAnalysis;
using StardewValley;

namespace Pathoschild.Stardew.LookupAnything.Framework
{
    /// <summary>Parses game state queries into human-readable representations where possible.</summary>
    internal static class HumanReadableConditionParser
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get a human-readable representation of a game state query, if available.</summary>
        /// <param name="condition">The raw game state query to parse.</param>
        public static string Parse(string condition)
        {
            // parse query
            // (If we get unexpected values at this point, bail early.)
            GameStateQuery.ParsedGameStateQuery query;
            {
                GameStateQuery.ParsedGameStateQuery[]? queries = GameStateQuery.Parse(condition);
                if (queries.Length != 1)
                    return condition;

                query = queries[0];
                if (query.Error != null || query.Query.Length == 0 || string.IsNullOrWhiteSpace(query.Query[0]))
                    return condition;
            }

            // apply parser
            string? parsed = null;
            switch (query.Query[0].ToUpperInvariant().Trim())
            {
                case nameof(GameStateQuery.DefaultResolvers.ITEM_CONTEXT_TAG):
                    parsed = HumanReadableConditionParser.ParseItemContextTag(query.Query);
                    break;

                case nameof(GameStateQuery.DefaultResolvers.ITEM_EDIBILITY):
                    parsed = HumanReadableConditionParser.ParseItemEdibility(query.Query);
                    break;
            }

            // format value
            if (parsed != null)
            {
                if (query.Negated)
                    parsed = I18n.ConditionOrContextTag_Negate(value: parsed);

                return parsed;
            }

            return condition;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Parse a <see cref="GameStateQuery.DefaultResolvers.ITEM_CONTEXT_TAG"/> query into a human-readable representation, if possible.</summary>
        /// <param name="query">The raw query arguments.</param>
        private static string? ParseItemContextTag(string[] query)
        {
            // format item type
            if (!HumanReadableConditionParser.TryTranslateItemTarget(ArgUtility.Get(query, 1), out string? itemType))
                return null;

            // format context tags
            string contextTags;
            {
                string[] rawTags = ArgUtility.GetSubsetOf(query, 2);
                for (int i = 0; i < rawTags.Length; i++)
                    rawTags[i] = I18n.Condition_ItemContextTag_Value(tag: rawTags[i]);
                contextTags = I18n.List(rawTags);
            }

            // build translation
            return query.Length > 3
                ? I18n.Condition_ItemContextTags(item: itemType, tags: contextTags)
                : I18n.Condition_ItemContextTag(item: itemType, tags: contextTags);
        }

        /// <summary>Parse a <see cref="GameStateQuery.DefaultResolvers.ITEM_EDIBILITY"/> query into a human-readable representation, if possible.</summary>
        /// <param name="query">The raw query arguments.</param>
        private static string? ParseItemEdibility(string[] query)
        {
            // format item type
            if (!HumanReadableConditionParser.TryTranslateItemTarget(ArgUtility.Get(query, 1), out string? itemType))
                return null;

            // format edibility
            if (!ArgUtility.HasIndex(query, 2))
                return I18n.Condition_ItemEdibility_Edible(item: itemType);

            int min = ArgUtility.GetInt(query, 2);
            int max = ArgUtility.GetInt(query, 3, int.MaxValue);
            return max != int.MaxValue
                ? I18n.Condition_ItemEdibility_Range(item: itemType, min: min, max: max)
                : I18n.Condition_ItemEdibility_Min(item: itemType, min: min);
        }

        /// <summary>Get the translated representation for an item target like 'input' or 'target', if known.</summary>
        /// <param name="itemType">The raw item type.</param>
        /// <param name="translated">The translated representation.</param>
        /// <returns>Returns whether a translation was found for the target type.</returns>
        private static bool TryTranslateItemTarget(string? itemType, [NotNullWhen(true)] out string? translated)
        {
            if (string.Equals(itemType, "Input", StringComparison.OrdinalIgnoreCase))
            {
                translated = I18n.Condition_ItemType_Input();
                return true;
            }

            if (string.Equals(itemType, "Target", StringComparison.OrdinalIgnoreCase))
            {
                translated = I18n.Condition_ItemType_Target();
                return true;
            }

            translated = null;
            return false;
        }
    }
}
