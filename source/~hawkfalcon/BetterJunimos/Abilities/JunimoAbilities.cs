/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System.Linq;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using System;
using StardewValley.Characters;
using System.Collections.Generic;
using BetterJunimos.Abilities;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using StardewModdingAPI;

namespace BetterJunimos.Utils {
    public class JunimoAbilities {
        private Dictionary<string, bool> _enabledAbilities;
        private readonly IMonitor _monitor;

        /* ACTION FAILURE COOLDOWNS:
         Intent: don't retry actions that failed recently (in the last game hour)
         
         Rationale: if IsActionAvailable says yes but PerformAction says no, the Junimo will retry that action all day
         and appear to be stuck on a tile. 
         While these two functions should be as consistent as possible, it's impossible for IsActionAvailable to 
         predict all the ways that PerformAction could fail. 

         Registering a failed action:
         Call ActionFailed. Currently called in the Harmony patch PatchTryToHarvestHere
         
         Resetting cooldowns:
         Call ResetCooldowns. Currently called by the start-of-day event handler and the hut-menu-closed handler
         (player may have added seeds etc so that failed actions will now succeed)           
         */
        private static readonly Dictionary<(GameLocation, Vector2, IJunimoAbility), int> FailureCooldowns = new();


        /*
         * for each hut and map, the last known crop location/work to do
         * because each hut could be servicing several locations (e.g. farm, greenhouse)
         *
         * used in pathFindToNewCrop_doWork
         */
        internal readonly Dictionary<(JunimoHut, GameLocation), Point> lastKnownCropLocations = new();


        private readonly List<IJunimoAbility> _registeredJunimoAbilities = new();

        private static readonly Dictionary<Guid, Dictionary<int, bool>> ItemsInHuts = new();
        private readonly HashSet<int> _requiredItems = new() {SObject.fertilizerCategory, SObject.SeedsCategory};


        public JunimoAbilities(Dictionary<string, bool> enabledAbilities, IMonitor monitor) {
            _enabledAbilities = enabledAbilities;
            _monitor = monitor;
        }

        // register built in abilities, in order
        internal void RegisterDefaultAbilities() {
            var defaultAbilities = new List<IJunimoAbility> {
                new WaterAbility(),
                new FertilizeAbility(),
                new PlantCropsAbility(_monitor),
                new HarvestCropsAbility(),
                new HarvestBushesAbility(),
                new HarvestForageCropsAbility(),
                new ClearDeadCropsAbility(),
                new VisitGreenhouseAbility(),
            };
            foreach (var junimoAbility in defaultAbilities) {
                RegisterJunimoAbility(junimoAbility);
            }
        }

        /*
         * Add an IJunimoAbility to the list of possible actions if allowed
         */
        public void RegisterJunimoAbility(IJunimoAbility junimoAbility) {
            var name = junimoAbility.AbilityName();
            if (!BetterJunimos.Config.JunimoAbilities.ContainsKey(name)) {
                // BetterJunimos.SMonitor.Log($"RegisterJunimoAbility [for {name}]: no entry in JunimoAbilities", LogLevel.Info);
                BetterJunimos.Config.JunimoAbilities.Add(name, true);
            }

            if (!BetterJunimos.Config.JunimoAbilities[name]) {
                // BetterJunimos.SMonitor.Log($"RegisterJunimoAbility [for {name}]: disabled in JunimoAbilities", LogLevel.Info);
                return;
            }

            // BetterJunimos.SMonitor.Log($"RegisterJunimoAbility [for {name}]: registering", LogLevel.Debug);

            _registeredJunimoAbilities.Add(junimoAbility);
            _requiredItems.UnionWith(junimoAbility.RequiredItems());

            // add placeholder for ability in progressions system
            if (Util.Progression.UnlockCosts is null) {
                return;
            }

            if (!Util.Progression.UnlockCosts.ContainsKey(name)) {
                Util.Progression.UnlockCosts[name] = new UnlockCost {Item = 268, Stack = 1, Remarks = "Starfruit"};
            }
        }

        // Can the Junimo use a capability/ability here
        public bool IsActionable(GameLocation location, Vector2 pos, Guid id) {
            return IdentifyJunimoAbility(location, pos, id) != null;
        }

        public IJunimoAbility IdentifyJunimoAbility(GameLocation location, Vector2 pos, Guid hutGuid) {
            // BetterJunimos.SMonitor.Log($"IdentifyJunimoAbility at {location.Name} ({location.IsGreenhouse})", LogLevel.Debug);
            // if (location.IsGreenhouse) {
            //     BetterJunimos.SMonitor.Log($"IdentifyJunimoAbility at {location.Name} [{pos.X} {pos.Y}]",
            //         LogLevel.Debug);
            // }

            foreach (var ability in _registeredJunimoAbilities) {
                // if (location.IsGreenhouse && ability.AbilityName() == "HoeAroundTrees") {
                //     BetterJunimos.SMonitor.Log($"  {ability.AbilityName()}");
                // }

                // TODO: cooldowns for greenhouse
                if (ActionCoolingDown(location, ability, pos)) continue;

                if (!ItemInHut(hutGuid, ability.RequiredItems())) {
                    // if (location.IsGreenhouse && ability.AbilityName() == "HoeAroundTrees") {
                    //     BetterJunimos.SMonitor.Log($"    items not in hut");
                    // }

                    continue;
                }

                if (!ability.IsActionAvailable(location, pos, hutGuid)) {
                    // if (location.IsGreenhouse && ability.AbilityName() == "HoeAroundTrees") {
                    //     BetterJunimos.SMonitor.Log($"    action not available");
                    // }

                    continue;
                }

                if (!Util.Progression.CanUseAbility(ability)) {
                    // if (location.IsGreenhouse && ability.AbilityName() == "HoeAroundTrees") {
                    //     BetterJunimos.SMonitor.Log($"    progression locked");
                    // }

                    continue;
                }

                // if (location.IsGreenhouse) {
                //     BetterJunimos.SMonitor.Log(
                //         $"    {ability.AbilityName()} available at {location.Name} [{pos.X} {pos.Y}]");
                // }

                return ability;
            }

            // if (location.IsGreenhouse) {
            //     BetterJunimos.SMonitor.Log($"  no work at {location.Name} {pos.X} {pos.Y}");
            // }

            return null;
        }

        public bool PerformAction(IJunimoAbility ability, Guid id, GameLocation location, Vector2 pos,
            JunimoHarvester junimo) {
            var hut = Util.GetHutFromId(id);
            var chest = hut.output.Value;

            var success = ability.PerformAction(location, pos, junimo, id);

            // if (location.IsGreenhouse) {
            //     BetterJunimos.SMonitor.Log(
            //         $"    PerformAction: {ability.AbilityName()} performed at {location.Name} [{pos.X} {pos.Y}] ({success})",
            //         LogLevel.Trace);
            // }

            var requiredItems = ability.RequiredItems();
            if (requiredItems.Count > 0) {
                UpdateHutContainsItems(id, chest, requiredItems);
            }

            return success;
        }

        public void ActionFailed(GameLocation location, IJunimoAbility ability, Vector2 pos) {
            _monitor.Log($"Action {ability.AbilityName()} at {location.Name} [{pos.X} {pos.Y}] failed", LogLevel.Debug);
            var cd = (location, pos, ability);
            FailureCooldowns[cd] = Game1.timeOfDay;
        }

        public void ListCooldowns() {
            _monitor.Log($"Cooldowns:", LogLevel.Debug);

            foreach (var (key, timeOfDay) in FailureCooldowns) {
                var (gameLocation, (x, y), junimoAbility) = key;
                _monitor.Log($"    {junimoAbility} at {gameLocation} [{x} {y}] since {timeOfDay}", LogLevel.Debug);
            }
        }

        public static void ResetCooldowns() {
            FailureCooldowns.Clear();
        }

        public static bool ActionCoolingDown(GameLocation location, IJunimoAbility ability, Vector2 pos) {
            
            // TODO: cooldowns for greenhouse
            if (location.IsGreenhouse) {
                // BetterJunimos.SMonitor.Log($"ActionCoolingDown allowing {ability.AbilityName()} at [{pos.X} {pos.Y}]", LogLevel.Debug);
                return false;
            }

            var cd = (location, pos, ability);
            if (!FailureCooldowns.TryGetValue(cd, out var failureTime)) return false;
            if (failureTime <= Game1.timeOfDay - 1000) return false;
            BetterJunimos.SMonitor.Log($"Action {ability.AbilityName()} at [{pos.X} {pos.Y}] is in cooldown");
            return true;
        }

        private static bool ItemInHut(Guid id, int item) {
            return ItemsInHuts[id].TryGetValue(item, out var present) && present;
        }

        public static bool ItemInHut(Guid id, List<int> items) {
            if (items.Count == 0) return true;
            return items.Any(item => ItemInHut(id, item));
        }

        internal void UpdateHutItems(Guid id) {
            var hut = Util.GetHutFromId(id);
            var chest = hut.output.Value;
            UpdateHutContainsItems(id, chest, _requiredItems.ToList<int>());
        }

        private void UpdateHutContainsItems(Guid id, Chest chest, List<int> items) {
            foreach (var itemId in items) {
                if (!ItemsInHuts.ContainsKey(id)) {
                    ItemsInHuts.Add(id, new Dictionary<int, bool>());
                }

                if (itemId > 0) {
                    ItemsInHuts[id][itemId] = chest.items.Any(item =>
                        item != null && item.ParentSheetIndex == itemId &&
                        !(BetterJunimos.Config.JunimoImprovements.AvoidPlantingCoffee &&
                          item.ParentSheetIndex == Util.CoffeeId)
                    );
                }
                else {
                    UpdateHutContainsItemCategory(id, chest, itemId);
                }
            }
        }

        private static void UpdateHutContainsItemCategory(Guid id, Chest chest, int itemCategory) {
            if (!ItemsInHuts.ContainsKey(id)) {
                ItemsInHuts.Add(id, new Dictionary<int, bool>());
            }

            ItemsInHuts[id][itemCategory] = chest.items.Any(item =>
                item != null && item.Category == itemCategory &&
                !(BetterJunimos.Config.JunimoImprovements.AvoidPlantingCoffee && item.ParentSheetIndex == Util.CoffeeId)
            );
        }
    }
}