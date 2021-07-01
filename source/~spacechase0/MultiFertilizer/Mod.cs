/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using MultiFertilizer.Patches;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;

namespace MultiFertilizer
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static string KeyFert => $"{Mod.Instance.ModManifest.UniqueID}/FertilizerLevel";
        public static string KeyRetain => $"{Mod.Instance.ModManifest.UniqueID}/WaterRetainLevel";
        public static string KeySpeed => $"{Mod.Instance.ModManifest.UniqueID}/SpeedGrowLevel";

        public static Mod Instance;

        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            HarmonyPatcher.Apply(this,
                new CropPatcher(),
                new GameLocationPatcher(),
                new HoeDirtPatcher(),
                new ObjectPatcher(),
                new UtilityPatcher()
            );
        }
    }
}
