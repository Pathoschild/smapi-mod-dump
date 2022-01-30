/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ForageFantasy
{
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.TerrainFeatures;

    public interface IJsonAssetsApi
    {
        int GetObjectId(string name);
    }

    internal class GrapeLogic
    {
        public static bool AreGrapeJsonModsInstalled(ForageFantasy mod)
        {
            return mod.Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets") && mod.Helper.ModRegistry.IsLoaded("Goldenrevolver.TheGrapeDivide");
        }

        public static void SetDropToNewGrapes(ForageFantasy mod)
        {
            if (!Context.IsMainPlayer || !AreGrapeJsonModsInstalled(mod))
            {
                return;
            }

            if (TryToGetGrapeID(mod, out int res))
            {
                ReplaceGrapeStarterDrop(res);
            }
        }

        // reset every end of the day so we don't accidentally save and then permanently edit a crop, if the mod gets uninstalled
        public static void ResetGrapes(ForageFantasy mod)
        {
            if (!Context.IsMainPlayer || !AreGrapeJsonModsInstalled(mod))
            {
                return;
            }

            ReplaceGrapeStarterDrop(398);
        }

        private static void ReplaceGrapeStarterDrop(int id)
        {
            foreach (var location in Game1.locations)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    if (terrainfeature.Value is HoeDirt dirt && dirt?.crop?.netSeedIndex?.Value == 301)
                    {
                        dirt.crop.indexOfHarvest.Value = id;
                    }
                }
            }
        }

        private static bool TryToGetGrapeID(ForageFantasy mod, out int id)
        {
            if (!AreGrapeJsonModsInstalled(mod))
            {
                id = 0;
                return false;
            }

            var api = mod.Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");

            if (api == null)
            {
                id = 0;
                return false;
            }

            id = api.GetObjectId("Fine Grape");

            return true;
        }
    }
}