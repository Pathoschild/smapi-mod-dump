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
    public record BinocularsFields
    {
        public const string CONTEXT_TAG = "Ivy_OrnithologistsGuild_Binoculars";

        private const string FIELD_RANGE = "Ivy_OrnithologistsGuild_Range";

        public int Range { get; private set; }

        public BinocularsFields(Object binoculars)
        {
            if (DataLoader.Objects(Game1.content).TryGetValue(binoculars.ItemId, out var objectData))
            {
                if (objectData.CustomFields.TryGetValue(FIELD_RANGE, out string range)) {
                    Range = int.Parse(range);
                } else {
                    throw new System.ArgumentException($"Must contain {FIELD_RANGE} custom field", nameof(binoculars));
                }
            } else {
                throw new System.ArgumentException("Must contain custom fields", nameof(binoculars));
            }
        }
    }

    public static class ObjectBinocularsFieldsExtensions
    {
        private static Dictionary<string, BinocularsFields> cachedBinocularsFields = new Dictionary<string, BinocularsFields>();

        public static BinocularsFields GetBinocularsFields(this Object binoculars)
        {
            if (!IsBinoculars(binoculars)) return null;

            if (!cachedBinocularsFields.ContainsKey(binoculars.QualifiedItemId))
            {
                cachedBinocularsFields[binoculars.QualifiedItemId] = new BinocularsFields(binoculars);
            }

            return cachedBinocularsFields[binoculars.QualifiedItemId];
        }

        public static bool IsBinoculars(this Object maybeBinoculars) => maybeBinoculars.HasContextTag(BinocularsFields.CONTEXT_TAG);
    }
}
