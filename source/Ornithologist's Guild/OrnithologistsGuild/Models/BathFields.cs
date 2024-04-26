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
    public record BathFields
    {
        public const string CONTEXT_TAG = "Ivy_OrnithologistsGuild_Bath";

        private const string FIELD_HEATED = "Ivy_OrnithologistsGuild_Heated";
        private const string FIELD_Z_OFFSET = "Ivy_OrnithologistsGuild_ZOffset";

        public bool Heated { get; private set; }

        public int ZOffset { get; private set; }

        public BathFields(Object bath)
        {
            if (DataLoader.BigCraftables(Game1.content).TryGetValue(bath.ItemId, out var bigCraftableData))
            {
                if (bigCraftableData.CustomFields.TryGetValue(FIELD_HEATED, out string heated)) {
                    Heated = bool.Parse(heated);
                } else {
                    throw new System.ArgumentException($"Must contain {FIELD_HEATED} custom field", nameof(bath));
                }

                if (bigCraftableData.CustomFields.TryGetValue(FIELD_Z_OFFSET, out string zOffset)) {
                    ZOffset = int.Parse(zOffset);
                } else {
                    throw new System.ArgumentException($"Must contain {FIELD_Z_OFFSET} custom field", nameof(bath));
                }
            } else {
                throw new System.ArgumentException("Must contain custom fields", nameof(bath));
            }
        }
    }

    public static class ObjectBathFieldsExtensions
    {
        private static Dictionary<string, BathFields> cachedBathFields = new Dictionary<string, BathFields>();

        public static BathFields GetBathFields(this Object bath)
        {
            if (!IsBath(bath)) return null;

            if (!cachedBathFields.ContainsKey(bath.QualifiedItemId))
            {
                cachedBathFields[bath.QualifiedItemId] = new BathFields(bath);
            }

            return cachedBathFields[bath.QualifiedItemId];
        }

        public static bool IsBath(this Object maybeBath) => maybeBath.bigCraftable.Value && maybeBath.HasContextTag(BathFields.CONTEXT_TAG);
    }
}
