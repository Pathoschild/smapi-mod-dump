/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BetterJunimos.Utils {
    public class JunimoGreenhouse {
        private readonly IManifest _manifest;
        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;

        internal JunimoGreenhouse(IManifest manifest, IMonitor monitor, IModHelper helper) {
            _manifest = manifest;
            _monitor = monitor;
            _helper = helper;
        }

        public bool HutHasGreenhouse(Guid id) {
            return GreenhouseBuildingNearHut(id) is not null;
        }
        
        internal static GreenhouseBuilding GreenhouseBuildingAtPos(GameLocation location, Vector2 tile) {
            if (!location.IsBuildableLocation()) return null;
            foreach (var building in location.buildings) {
                if (building is not GreenhouseBuilding greenhouseBuilding) continue;
                if (greenhouseBuilding.occupiesTile(tile)) {
                    return greenhouseBuilding;
                }
            }

            return null;
        }
        
        public GreenhouseBuilding GreenhouseBuildingNearHut(Guid id) {
            var hut = Util.GetHutFromId(id);
            var radius = Util.CurrentWorkingRadius;
            var farm = Game1.getFarm();
        
            for (var x = hut.tileX.Value + 1 - radius; x < hut.tileX.Value + 2 + radius; ++x) {
                for (var y = hut.tileY.Value + 1 - radius; y < hut.tileY.Value + 2 + radius; ++y) {
                    var pos = new Vector2(x, y);

                    var ghb = GreenhouseBuildingAtPos(farm, pos);
                    if (ghb is not null) return ghb;
                }
            }

            return null;
        }

        internal bool IsGreenhouseAtPos(GameLocation location, Vector2 tile) {
            if (!location.IsBuildableLocation()) return false;
            return location.buildings.Any(building =>
                building.occupiesTile(tile) && building is GreenhouseBuilding);
        }
    }
}