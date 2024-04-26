/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using StardewValley.TerrainFeatures;

namespace BushBloomMod {
    public static class BushExtensions {
        // exists, not a town bush, medium size, not an island bush
        public static bool IsAbleToBloom(this Bush bush) =>
            bush is not null
            && !bush.townBush.Value
            && bush.size.Value == 1
            && !(bush.Location?.InIslandContext() ?? false);

        public static bool IsBloomingToday(this Bush bush) => Schedule.GetSchedule(bush) is not null;

        public static bool HasBloomedToday(this Bush bush) => Schedule.TryGetExistingSchedule(bush, out _);

        public static string GetShakeOffId(this Bush bush) => Schedule.TryGetExistingSchedule(bush, out var schedule) ? schedule?.ShakeOffId : null;

        private const string KeyBushSchedule = "bush-schedule", KeyBushTexture = "bush-texture";

        public static void DataSetSchedule(this Bush bush, string id) {
            if (id is null) {
                bush.modData.Remove($"{ModEntry.Instance.Helper.ModContent.ModID}/bush-day"); // legacy
                bush.modData.Remove($"{ModEntry.Instance.Helper.ModContent.ModID}/{KeyBushTexture}");
                bush.modData.Remove($"{ModEntry.Instance.Helper.ModContent.ModID}/{KeyBushSchedule}");
            } else {
                bush.modData[$"{ModEntry.Instance.Helper.ModContent.ModID}/{KeyBushSchedule}"] = id;
            }
        }

        public static void DataSetTexture(this Bush bush, bool hasTexture) {
            if (hasTexture) {
                bush.modData[$"{ModEntry.Instance.Helper.ModContent.ModID}/{KeyBushTexture}"] = "true";
            } else {
                bush.modData.Remove($"{ModEntry.Instance.Helper.ModContent.ModID}/{KeyBushTexture}");
            }
        }

        public static bool TryDataGetSchedule(this Bush bush, out string id) =>
            !string.IsNullOrWhiteSpace(id = bush.modData.TryGetValue($"{ModEntry.Instance.Helper.ModContent.ModID}/{KeyBushSchedule}", out var value) ? value : null);

        public static bool DataHasTexture(this Bush bush) =>
            bush.modData.TryGetValue($"{ModEntry.Instance.Helper.ModContent.ModID}/{KeyBushTexture}", out var value) && string.Compare(value, "true") == 0;
    }
}