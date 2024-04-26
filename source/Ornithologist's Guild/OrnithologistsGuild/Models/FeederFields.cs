/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System.Collections.Generic;
using StardewValley;

namespace OrnithologistsGuild.Models
{
    public record FeederFields
    {
        public const string CONTEXT_TAG = "Ivy_OrnithologistsGuild_Feeder";

        private const string FIELD_TYPE = "Ivy_OrnithologistsGuild_Type";
        private const string FIELD_RANGE = "Ivy_OrnithologistsGuild_Range";
        private const string FIELD_MAX_FLOCKS = "Ivy_OrnithologistsGuild_MaxFlocks";
        private const string FIELD_Z_OFFSET = "Ivy_OrnithologistsGuild_ZOffset";

        public string Type { get; private set; }
        public int Range { get; private set; }
        public int MaxFlocks { get; private set; }

        public int ZOffset { get; private set; }

        public FeederFields(Object feeder)
        {
            if (DataLoader.BigCraftables(Game1.content).TryGetValue(feeder.ItemId, out var bigCraftableData))
            {
                if (bigCraftableData.CustomFields.TryGetValue(FIELD_TYPE, out string type)) {
                    Type = type;
                } else {
                    throw new System.ArgumentException($"Must contain {FIELD_TYPE} custom field", nameof(feeder));
                }

                if (bigCraftableData.CustomFields.TryGetValue(FIELD_RANGE, out string range)) {
                    Range = int.Parse(range);
                } else {
                    throw new System.ArgumentException($"Must contain {FIELD_RANGE} custom field", nameof(feeder));
                }

                if (bigCraftableData.CustomFields.TryGetValue(FIELD_MAX_FLOCKS, out string maxFlocks)) {
                    MaxFlocks = int.Parse(maxFlocks);
                } else {
                    throw new System.ArgumentException($"Must contain {FIELD_MAX_FLOCKS} custom field", nameof(feeder));
                }

                if (bigCraftableData.CustomFields.TryGetValue(FIELD_Z_OFFSET, out string zOffset)) {
                    ZOffset = int.Parse(zOffset);
                } else {
                    throw new System.ArgumentException($"Must contain {FIELD_Z_OFFSET} custom field", nameof(feeder));
                }
            } else {
                throw new System.ArgumentException($"Must contain custom fields", nameof(feeder));
            }
        }
    }

    public static class ObjectFeederFieldsExtensions
    {
        private static Dictionary<string, FeederFields> cachedFeederFields = new Dictionary<string, FeederFields>();

        public static FeederFields GetFeederFields(this Object feeder)
        {
            if (!IsFeeder(feeder)) return null;

            if (!cachedFeederFields.ContainsKey(feeder.QualifiedItemId))
            {
                cachedFeederFields[feeder.QualifiedItemId] = new FeederFields(feeder);
            }

            return cachedFeederFields[feeder.QualifiedItemId];
        }

        public static bool IsFeeder(this Object maybeFeeder) => maybeFeeder.bigCraftable.Value && maybeFeeder.HasContextTag(FeederFields.CONTEXT_TAG);
    }
}
