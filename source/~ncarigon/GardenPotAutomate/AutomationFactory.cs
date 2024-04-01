/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace GardenPotAutomate {
    public class AutomationFactory : IAutomationFactory {
        private delegate IAutomatable? MakeMachineDelegate(IndoorPot obj, GameLocation location, Vector2 tile);
        private MakeMachineDelegate OnMakeMachine = null!;

        internal static void Register(IModHelper helper, Config config) {
            var machine = new AutomationFactory();
            machine.OnMakeMachine += (obj, location, tile) => new GardenPotMachine(config, obj, location, tile);

            helper.Events.GameLoop.GameLaunched += (s, e)
                => helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate")?.AddFactory(machine);
        }

        public IAutomatable? GetFor(SObject obj, GameLocation location, in Vector2 tile) {
            return obj switch {
                IndoorPot indoorPot => this.OnMakeMachine?.Invoke(indoorPot, location, tile),
                _ => null,
            };
        }

        public IAutomatable? GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile) => null;

        public IAutomatable? GetFor(Building building, GameLocation location, in Vector2 tile) => null;

        public IAutomatable? GetForTile(GameLocation location, in Vector2 tile) => null;
    }
}