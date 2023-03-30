/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/vabrell/sdw-seed-maker-mod
**
*************************************************/

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using HarmonyLib;
using StardewValley;
using SM_bqms;

namespace SM_bqms
{
    public class AutomateSeedMakerMachinePatcher
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;
        private static System.Object Instance;
        private static Type SeedMakerMachine;
        private static string ModID;
        private static bool Patched;
        public static void Initialize(IModHelper helper, IMonitor monitor, string modID)
        {
            Patched = false;
            Monitor = monitor;
            Helper = helper;
            ModID = modID;

            helper.Events.GameLoop.GameLaunched += RegisterPatch;
        }

        public static void RegisterPatch(object sender, EventArgs e)
        {
            var automate = Helper.ModRegistry.GetApi("PathosChild.Automate");
            if (automate != null && Patched == false) {
                Harmony harmony = new Harmony(ModID);
                Assembly assembly = automate.GetType().Assembly;
                SeedMakerMachine = assembly.GetType("Pathoschild.Stardew.Automate.Framework.Machines.Objects.SeedMakerMachine");

                var orginal = AccessTools.Method(SeedMakerMachine, "SetInput");
                var prefix = new HarmonyMethod(typeof(AutomateSeedMakerMachinePatcher), nameof(AutomateSeedMakerMachinePatcher.SetInput_prefix));
                harmony.Patch(
                    original: orginal,
                    prefix: prefix
                );
                Patched = true;
            }
        }

        public static bool SetInput_prefix(System.Object __instance, System.Object input, ref bool __result)
        {
            try
            {
                Instance = __instance;
                Type type = __instance.GetType();
                string MachineTypeID = (string)type.GetProperty("MachineTypeID").GetValue(__instance);
                // If not a seed maker use base method
                if (MachineTypeID != "SeedMaker") return true;
                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

                MethodInfo UpdateSeedLookup = type.GetMethod("UpdateSeedLookup", bindingFlags);
                UpdateSeedLookup.Invoke(__instance, new object[] {});

                StardewValley.Object machine = (StardewValley.Object)type.GetProperty("Machine", bindingFlags).GetValue(__instance);

                // crop => seeds
                object crop;
                if (TryGetIngredient(input, IsValidCrop, 1, out crop))
                {
                    if (crop == null)
                    {
                        __result = false;
                        return false;
                    }
                    int seedMakerIndex = ModEntry.SeedMakers.FindIndex((sm) => sm.GameObject.TileLocation == machine.TileLocation);
                    if (seedMakerIndex < 0) {
                        SeedMaker newSeedMaker = new SeedMaker() {
                            GameObject = machine,
                            isHandled = false,
                        };
                        ModEntry.SeedMakers.Add(newSeedMaker);
                        seedMakerIndex = ModEntry.SeedMakers.LastIndexOf(newSeedMaker);
                    }
                    SeedMaker seedMaker = ModEntry.SeedMakers[seedMakerIndex];
                    if (seedMaker != null)
                    {
                        seedMaker.isHandled = true;
                    }

                    Type Consumable = type.Assembly.GetType("Pathoschild.Stardew.Automate.Framework.Consumable");
                    MethodInfo Take = Consumable.GetMethod("Take", bindingFlags);
                    StardewValley.Object item = (StardewValley.Object)Take.Invoke(crop, new object[] {});
                    IDictionary<int, int> SeedLookup = (IDictionary<int, int>)type.GetField("SeedLookup", bindingFlags).GetValue(Instance) ?? new Dictionary<int, int>();
                    if (ModEntry.Config.EnableDebug) {
                        Monitor.Log($"Automate patch: Seed validation", LogLevel.Debug);
                    }
                    if (SeedLookup == null)
                    {
                        if (ModEntry.Config.EnableDebug) {
                            Monitor.Log($"Automate patch: No valid seed for game object", LogLevel.Debug);
                        }
                        __result = false;
                        return false;
                    }
                    if (ModEntry.Config.EnableDebug) {
                        Monitor.Log($"Automate patch: Valid seed found", LogLevel.Debug);
                    }
                    int seedID = SeedLookup[item.ParentSheetIndex];
                    int cropModifier = GetCropModifier(item.Quality);

                    Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + (int)machine.TileLocation.X + (int)machine.TileLocation.Y * 77 + Game1.timeOfDay);
                    int amount = random.Next(1 + cropModifier, 4 + cropModifier);
                    machine.heldObject.Value = new StardewValley.Object(seedID, amount);
                    if (ModEntry.Config.EnableDebug) {
                        Monitor.Log($"\nAutomate patch: SeedMaker at {machine.TileLocation.ToString()}\nQuanity: {item.Quality}\nModifier: {cropModifier}\nSeeds: {amount}\n", LogLevel.Debug);
                    }
                    if (random.NextDouble() < 0.005) {
                        machine.heldObject.Value = new StardewValley.Object(499, 1);
                        if (ModEntry.Config.EnableDebug) {
                            Monitor.Log($"\nAutomate patch: SeedMaker at {machine.TileLocation.ToString()} Ancient Seeds chance triggered\n", LogLevel.Debug);
                        }
                    } else if (random.NextDouble() < 0.02) {
                        machine.heldObject.Value = new StardewValley.Object(770, random.Next(1, 5));
                        if (ModEntry.Config.EnableDebug) {
                            Monitor.Log($"\nAutomate patch: SeedMaker at {machine.TileLocation.ToString()} Mixed Seeds chance triggered\n", LogLevel.Debug);
                        }
                    }
                    machine.MinutesUntilReady = 20;
                    __result = true;
                    return false;
                }

                __result = false;
                return false;
            }
            catch (Exception e)
            {
                Monitor.Log($"Failed in [AutomateSeedMakerMachinePatcher.SetInput_prefix]:\n{e}", LogLevel.Error);
                return true;
            }
        }
        private static bool TryGetIngredient(object input, Func<object, bool> predicate, int count, out object consumable)
        {
            MethodInfo tryGetIngredient = input.GetType().GetMethods().ToList().Where((m) => m.ToString() == "Boolean TryGetIngredient(System.Func`2[Pathoschild.Stardew.Automate.ITrackedStack,System.Boolean], Int32, Pathoschild.Stardew.Automate.IConsumable ByRef)").First();

            object[] parameters = new object[] { predicate, count, null};
            object result = tryGetIngredient.Invoke(input, parameters);
            consumable = parameters[2];

            if (consumable == null)
            {
                return false;
            }
            return true;
        }
        private static bool IsValidCrop(object item)
        {
            Type type = Instance.GetType();
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            IDictionary<int, int> SeedLookup = (IDictionary<int, int>)type.GetField("SeedLookup", bindingFlags).GetValue(Instance);
            Enum ItemType = (Enum)item.GetType().GetProperty("Type", bindingFlags).GetValue(item);
            object sampleItem = item.GetType().GetProperty("Sample", bindingFlags).GetValue(item);
            List<string> validObjects = new List<string>(new string[] {"StardewValley.Object", "StardewValley.Objects.ColoredObject"});
            if (ModEntry.Config.EnableDebug) {
                Monitor.Log($"Automate patch: ItemName - {sampleItem.GetType().GetProperty("name")} :: ItemType {sampleItem.GetType()}", LogLevel.Debug);
                Monitor.Log($"Automate patch: ValidObjects - {String.Join(", ", validObjects.ToArray())}", LogLevel.Debug);
            }
            if(!validObjects.Contains(sampleItem.GetType().ToString()))
            {
                return false;
            }
            StardewValley.Object Item = (StardewValley.Object)sampleItem;
            Type PItems = type.Assembly.GetType("Pathoschild.Stardew.Automate.ItemType");
            var result = ItemType.GetType() == PItems.GetField("Object").GetValue(null).GetType()
                && Item.ParentSheetIndex != 433 // coffee beans
                && Item.ParentSheetIndex != 771 // fiber
                && SeedLookup.ContainsKey(Item.ParentSheetIndex);
            return result;
        }
        private static int GetCropModifier(int quality)
        {
            int modifier;
            switch (quality)
            {
                case 0: 
                    modifier = ModEntry.Config.NormalModifier;
                break; 
                case 1: 
                    modifier = ModEntry.Config.SilverModifier;
                break; 
                case 2: 
                case 3: 
                    modifier = ModEntry.Config.GoldModifier;
                break; 
                case 4: 
                    modifier = ModEntry.Config.IridiumModifier;
                break; 
                default:
                    modifier = 0;
                break;
            }
            return modifier;
        }
    }
}