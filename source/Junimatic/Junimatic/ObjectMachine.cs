/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.Machines;
using StardewValley.Inventories;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using NermNermNerm.Stardew.LocalizeFromSource;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Junimatic
{
    public class ObjectMachine
        : GameMachine
    {
        internal ObjectMachine(Object machine, Point accessPoint)
            : base(machine, accessPoint)
        {
        }

        public StardewValley.Object Machine => (StardewValley.Object)base.GameObject;

        internal static ObjectMachine? TryCreate(Object item, Point accessPoint)
        {
            if (item is CrabPot crabPot)
            {
                return new CrabPotMachine(crabPot, accessPoint);
            }
            else if (item.GetMachineData() is not null)
            {
                return new ObjectMachine(item, accessPoint);
            }
            else
            {
                return null;
            }
        }

        public override bool IsIdle => this.Machine.heldObject.Value is null && this.Machine.MinutesUntilReady == 0;

        public override Object? HeldObject => this.Machine.readyForHarvest.Value ? this.Machine.heldObject.Value : null;

        /// <summary>
        ///   Looks at the recipes allowed by this machine and the contents of the chest.  If there's
        ///   enough stuff in the chest to allow it, it builds a list of the items needed but doesn't
        ///   actually remove the items from the chest.
        /// </summary>
        public override List<Item>? GetRecipeFromChest(GameStorage storage)
        {
            if (this.IsManualFeedMachine)
            {
                return null;
            }

            var machineData = this.Machine.GetMachineData();
            var inputs = new List<Item>();

            var sourceInventory = storage.RawInventory;
            // Ensure it has the coal (aka all the 'AdditionalConsumedItems')
            if (machineData.AdditionalConsumedItems is not null)
            {
                if (!MachineDataUtility.HasAdditionalRequirements(sourceInventory, machineData.AdditionalConsumedItems, out _))
                {
                    return null;
                }

                inputs.AddRange(machineData.AdditionalConsumedItems.Select(i => ItemRegistry.Create(i.ItemId, i.RequiredCount)));
            }

            foreach (var item in sourceInventory)
            {
                if (MachineDataUtility.TryGetMachineOutputRule(this.Machine, machineData, MachineOutputTrigger.ItemPlacedInMachine, item, Game1.MasterPlayer, this.Machine.Location, out var rule, out var triggerRule, out var ruleIgnoringCount, out var triggerIgnoringCount))
                {
                    if (rule.OutputItem.Any(machineItemOutput => MachineDataUtility.GetOutputItem(this.Machine, machineItemOutput, item, Game1.MasterPlayer, true, out int? overrideMinutesUntilReady) is not null))
                    {
                        inputs.Add(ItemRegistry.Create(item.ItemId, amount: triggerRule.RequiredCount, quality: item.Quality));
                        return inputs;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///   Tries to populate the given machine's input with the contents of the chest.  If it
        ///   succeeds, it returns true and the necessary items are removed.
        /// </summary>
        public override bool FillMachineFromChest(GameStorage storage)
            => !this.IsManualFeedMachine && this.Machine.AttemptAutoLoad(storage.RawInventory, Game1.MasterPlayer);

        /// <summary>
        ///   Fills the machine from the supplied Junimo inventory.
        /// </summary>
        public override bool FillMachineFromInventory(Inventory inventory)
            => !this.IsManualFeedMachine && this.Machine.AttemptAutoLoad(inventory, Game1.MasterPlayer);

        private static Dictionary<string,bool> cachedCompatList = new Dictionary<string,bool>();

        /// <summary>
        ///   Returns true if the machine has a recipe that the Junimo can do.
        /// </summary>
        public override bool IsCompatibleWithJunimo(JunimoType projectType)
        {
            string cacheKey = $"{this.Machine.ItemId}:{projectType}";
            if (cachedCompatList.TryGetValue(cacheKey, out bool result))
            {
                return result;
            }

            result = this.IsCompatibleWithJunimoNoCache(projectType);
            cachedCompatList[cacheKey] = result;
            return result;
        }

        private bool IsManualFeedMachine => this.Machine.ItemId == "21"; // Crystalarium

        [NoStrict]
        private bool IsCompatibleWithJunimoNoCache(JunimoType projectType)
        {
            // The MachineData contains clues as to what the assignments should be, but it's definitely fuzzy.
            // The game's notion of "category" is largely a matter of taste and isn't really consistent, and
            // this mod's notion of categorization is also just a matter of taste.  By using categories, we're
            // just trying to make it so there's a fighting chance for getting it right for mods without having
            // to do special configuration.

            // Special cases.
            switch (this.Machine.ItemId)
            {
                case "12": // keg
                    return projectType == JunimoType.CropProcessing; // Otherwise it'll return true for Animals, because there's a recipe (forget which) that involves animal stuff.
                case "25": // seed maker
                    return projectType == JunimoType.CropProcessing; // There's no data at all in its MachineData.
                case "211": // wood chipper
                    return projectType == JunimoType.Forestry; // Else it gets to thinking that fishing would work.
                case "10": // bee house
                    return projectType == JunimoType.Animals; // no good data
                case "20": // recycling machine
                    return projectType == JunimoType.Fishing; // It has a zillion outputs, so the data isn't a good guide.
                case "154": // worm bin
                case "DeluxeWormBin":
                    return projectType == JunimoType.Fishing; // The output item makes it perfectly clear, but it's the only thing where OutputItem would add value.
                case "BaitMaker": // This seems like a dangerous machine since the use-case is so specialized that if it gets in the network, it's likely by accident.
                    return false;
                case "710": // crab pot
                    return projectType == JunimoType.Fishing; // There's no MachineData
                case "231": // solar panel
                case "9": // lightning rod
                    return projectType == JunimoType.MiningProcessing; // no good data
                case "105": // tapper
                case "264": // heavy tapper
                case "MushroomLog":
                    return projectType == JunimoType.Forestry; // no good data
                case "Dehydrator":
                    if (projectType == JunimoType.Forestry)
                    {
                        // The Data for this machine will return true for farming Junimos as well, but there's no
                        // category for woodsy stuff and we're kinda blazing our own path with the idea that forestry
                        // Junimos work mushrooms.
                        return true;
                    }
                    break;
            }

            // TODO: Add configurable special cases.

            string[][] tags = [
                ["category_minerals", "category_gem", "bone_item"],
                ["egg_item", "large_egg_item", "slime_egg_item"],
                ["category_vegetable", "category_fruit", "keg_wine", "preserves_pickle", "preserves_jelly"],
                ["category_fish"],
                []]; // there aren't any tags for wood stuff listed

            int[][] categories = [
                [StardewValley.Object.GemCategory, StardewValley.Object.mineralsCategory, StardewValley.Object.metalResources, StardewValley.Object.monsterLootCategory],
                [StardewValley.Object.EggCategory, StardewValley.Object.MilkCategory, StardewValley.Object.meatCategory, StardewValley.Object.sellAtPierresAndMarnies /* wool, duck feather, etc. */, StardewValley.Object.artisanGoodsCategory],
                [StardewValley.Object.VegetableCategory, StardewValley.Object.FruitsCategory, StardewValley.Object.SeedsCategory, StardewValley.Object.flowersCategory, StardewValley.Object.fertilizerCategory, StardewValley.Object.artisanGoodsCategory],
                [StardewValley.Object.junkCategory, StardewValley.Object.baitCategory],
                [StardewValley.Object.buildingResources]
                ];

            var machineData = this.Machine.GetMachineData();
            if (machineData.OutputRules is not null)
            {
                foreach (var rule in machineData.OutputRules)
                {
                    foreach (var trigger in rule.Triggers)
                    {
                        if (trigger.RequiredTags is not null && trigger.RequiredTags.Intersect(tags[(int)projectType]).Any())
                        {
                            return true;
                        }

                        if (trigger.RequiredItemId is not null)
                        {
                            var itemData = ItemRegistry.GetData(trigger.RequiredItemId);
                            if (itemData is null)
                            {
                                ModEntry.Instance.LogWarningOnce($"ItemRegistry.GetData failed for {trigger.RequiredItemId} - this could render {this.Machine.Name} unusable to Junimos");
                            }
                            else if (categories[(int)projectType].Contains(itemData.Category))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        protected override StardewValley.Object TakeItemFromMachine()
        {
            // Adapted from Pathoschild.Stardew.Automate.Framework.Machines.GetOutput
            StardewValley.Object machine = this.Machine;
            var result = machine.heldObject.Value;
            MachineData? machineData = machine.GetMachineData();

            // recalculate output if needed (e.g. bee house honey)
            if (machine.lastOutputRuleId.Value != null && machineData != null)
            {
                MachineOutputRule? outputRule = machineData.OutputRules?.FirstOrDefault(p => p.Id == machine.lastOutputRuleId.Value);
                if (outputRule?.RecalculateOnCollect == true)
                {
                    var prevOutput = machine.heldObject.Value;
                    machine.heldObject.Value = null;

                    machine.OutputMachine(machineData, outputRule, machine.lastInputItem.Value, null, machine.Location, false);

                    if (machine.heldObject.Value == null)
                        machine.heldObject.Value = prevOutput;
                }
            }

            // get output
            this.OnOutputCollected(result);
            return result;
        }

        private void OnOutputCollected(Item item)
        {
            // Adapted from Pathoschild.Stardew.Automate.Framework.Machines.OnOutputCollected
            StardewValley.Object machine = this.Machine;
            MachineData? machineData = machine.GetMachineData();

            // update stats
            MachineDataUtility.UpdateStats(machineData?.StatsToIncrementWhenHarvested, item, item.Stack);

            // reset machine data
            // This needs to happen before the OutputCollected check, which may start producing a new output.
            machine.heldObject.Value = null;
            machine.readyForHarvest.Value = false;
            machine.showNextIndex.Value = false;
            machine.ResetParentSheetIndex();

            // apply OutputCollected rule
            if (MachineDataUtility.TryGetMachineOutputRule(machine, machineData, MachineOutputTrigger.OutputCollected, item.getOne(), null, machine.Location, out MachineOutputRule outputCollectedRule, out _, out _, out _))
                machine.OutputMachine(machineData, outputCollectedRule, machine.lastInputItem.Value, null, machine.Location, false);

            // update tapper
            if (machine.IsTapper())
            {
                if (machine.Location.terrainFeatures.TryGetValue(machine.TileLocation, out TerrainFeature terrainFeature) && terrainFeature is Tree tree)
                    tree.UpdateTapperProduct(machine, item as StardewValley.Object);
            }

        }

        public override string ToString()
        {
            return IF($"{this.Machine.Name} at {this.Machine.TileLocation}");
        }
    }
}
