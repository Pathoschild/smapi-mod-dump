/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Harmony;
using Spacechase.Shared.Patching;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BuildableLocationsFramework.Patches
{
    /// <summary>Applies Harmony patches to <see cref="SaveGame"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class SaveGamePatcher : BasePatcher
    {
        /*********
        ** Accessors
        *********/
        internal static List<GameLocation> Locations;


        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<SaveGame>(nameof(SaveGame.loadDataToLocations)),
                prefix: this.GetHarmonyMethod(nameof(Before_LoadDataToLocations)),
                postfix: this.GetHarmonyMethod(nameof(After_LoadDataToLocations))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="SaveGame.loadDataToLocations"/>.</summary>
        private static void Before_LoadDataToLocations(List<GameLocation> gamelocations)
        {
            SaveGamePatcher.Locations = gamelocations;

            foreach (GameLocation gamelocation in gamelocations)
            {
                if (gamelocation.Name == "Farm")
                    continue;
                if (gamelocation is BuildableGameLocation buildableGameLocation)
                {
                    BuildableGameLocation locationFromName = (BuildableGameLocation)Game1.getLocationFromName(gamelocation.Name);
                    foreach (Building building in buildableGameLocation.buildings)
                        building.load();
                    locationFromName.buildings.Set(buildableGameLocation.buildings);
                }
                else if (gamelocation is IAnimalLocation al)
                {
                    foreach (FarmAnimal farmAnimal in al.Animals.Values)
                        farmAnimal.reload(null);
                }
            }
        }

        /// <summary>The method to call after <see cref="SaveGame.loadDataToLocations"/>.</summary>
        private static void After_LoadDataToLocations()
        {
            SaveGamePatcher.Locations = null;
        }
    }
}
