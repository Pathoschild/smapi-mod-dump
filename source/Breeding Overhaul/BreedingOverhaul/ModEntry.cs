/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StarAmy/BreedingOverhaul
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using HarmonyLib;
using System.Reflection;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using xTile;
using System.Threading.Tasks;
using System.Collections;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using System.IO;using xTile.Layers;
using xTile.Tiles;
using StardewValley.Objects;
using StardewValley.Buildings;
using StardewValley.Characters;

namespace BreedingOverhaul
{
    public class ModEntry : Mod
    {
        public static IModHelper MyHelper;
        public static IMonitor MyMonitor;
        public static HarmonyLib.Harmony harmony;
        public static ITranslationHelper i18n;

        private static IncubatorPatch ipatch;

        public static IncubatorData incubatorData;
        public static PregnancyData pregnancyData;

        // Mod loading entry point, first code that is executed.
        public override void Entry(IModHelper helper)
        {
            this.Monitor.Log($"Mod entry in Breeeding Overhaul.", LogLevel.Debug);
            harmony = new HarmonyLib.Harmony("StarAmy.BreedingOverhaul");
            MyHelper = helper;
            i18n = helper.Translation;
            MyMonitor = this.Monitor;
            ipatch = new IncubatorPatch();

            // Load mod JSON config files
            loadJsonData();

            // hook into game events
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            helper.ConsoleCommands.Add("list_animals", $"Lists the animal IDs of all animals", OnCommandReceived);
        }

        internal void OnCommandReceived(string command, string[] args)
        {
            switch (command)
            {
                case "list_animals":
                    List<string> animalInfo = new List<string>();

                    foreach (FarmAnimal animal in GetAnimals())
                        animalInfo.Add(GetPrintString(animal));

                    ModEntry.MyMonitor.Log("Animals:", LogLevel.Debug);
                    ModEntry.MyMonitor.Log($"{string.Join(", ", animalInfo)}\n", LogLevel.Info);
                    return;
            }
        }

        public static IEnumerable<FarmAnimal> GetAnimals()
        {
            Farm farm = Game1.getFarm();

            if (farm == null)
                yield break;

            foreach (FarmAnimal animal in farm.getAllFarmAnimals())
                yield return animal;
        }

        internal static string GetPrintString(Character creature)
        {
            string name = creature.Name;
            string type = GetInternalType(creature);

            return $"\n # {name}:  Type - {type}";
        }

        public static string GetInternalType(Character creature)
        {
            if (creature is Pet || creature is Horse)
                return ModEntry.Sanitize(creature.GetType().Name);
            else if (creature is FarmAnimal animal)
                return ModEntry.Sanitize(animal.type.Value);
            return "";
        }

        public static string Sanitize(string input)
        {
            input = input.ToLower().Replace(" ", "");
            return string.IsInterned(input) ?? input;
        }

        private void loadJsonData()
        {
            incubatorData = MyHelper.Data.ReadJsonFile<IncubatorData>("data\\incubatordata.json") ?? null;
            if (incubatorData == null)
            {
                this.Monitor.Log($"No incubator data file.", LogLevel.Trace);
            }
            else
            {
                this.Monitor.Log($"Incubator data file loaded.", LogLevel.Trace);
            }

            pregnancyData = MyHelper.Data.ReadJsonFile<PregnancyData>("data\\pregnancydata.json") ?? null;
            if (pregnancyData == null)
            {
                this.Monitor.Log($"No pregnancy data file.", LogLevel.Trace);
            }
            else
            {
                this.Monitor.Log($"Incubator pregnancy file loaded.", LogLevel.Trace);
            }

            // Load content pack JSON config files
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                try
                {
                    if (File.Exists(Path.Combine(contentPack.DirectoryPath, "incubatordata.json")))
                    {
                        this.Monitor.Log($"Reading content pack incubator data: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                        try
                        {
                            IncubatorData contentPackData = contentPack.ReadJsonFile<IncubatorData>("incubatordata.json");
                            foreach (string incubatorEntry in contentPackData.IncubatorItems.Keys)
                            {
                                if (incubatorData.IncubatorItems.Remove(incubatorEntry))
                                {
                                    this.Monitor.Log($"Removing old incubator to offspring map entry for {incubatorEntry}, replacing with {contentPackData.IncubatorItems[incubatorEntry]}", LogLevel.Trace);
                                }
                                incubatorData.IncubatorItems.Add(incubatorEntry, contentPackData.IncubatorItems[incubatorEntry]);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Monitor
                                .Log(
                                    $"Error while loading content pack incubatordata.json: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}. It'll be ignored.\n{ex}"
                                    , LogLevel.Error);
                        }
                    }
                    if (File.Exists(Path.Combine(contentPack.DirectoryPath, "pregnancydata.json")))
                    {
                        this.Monitor.Log($"Reading content pack pregnancy data: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                        try
                        {
                            PregnancyData contentPackData = contentPack.ReadJsonFile<PregnancyData>("pregnancydata.json");
                            foreach (string pregnancyItemKey in contentPackData.PreganancyItems.Keys)
                            {
                                if (pregnancyData.PreganancyItems.Remove(pregnancyItemKey))
                                {
                                    this.Monitor.Log($"Removing old pregnancy item entry for {pregnancyItemKey}, replacing with {contentPackData.PreganancyItems[pregnancyItemKey]}", LogLevel.Trace);
                                }
                                pregnancyData.PreganancyItems.Add(pregnancyItemKey, contentPackData.PreganancyItems[pregnancyItemKey]);
                            }
                            foreach (string offspringKey in contentPackData.Offspring.Keys)
                            {
                                if (pregnancyData.Offspring.Remove(offspringKey))
                                {
                                    this.Monitor.Log($"Removing old animal to offspring entry for {offspringKey}, replacing with {contentPackData.Offspring[offspringKey]}", LogLevel.Trace);
                                }
                                pregnancyData.Offspring.Add(offspringKey, contentPackData.Offspring[offspringKey]);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Monitor
                                .Log(
                                    $"Error while loading content pack pregnancydata.json: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}. It'll be ignored.\n{ex}"
                                    , LogLevel.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Monitor
                        .Log(
                            $"Error while trying to load the content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}. It'll be ignored.\n{ex}"
                            , LogLevel.Error);
                }
            }
        }

        private void aboutPatch(Type typ, string method)
        {
            MyMonitor.Log($"Checking {typ.Name} {method} patches", LogLevel.Trace);

            // get the MethodBase of the original
            var original =typ.GetMethod(method);

            // retrieve all patches
            var patches = Harmony.GetPatchInfo(original);
            if (patches is null)
            {
                MyMonitor.Log($"{method} not patched", LogLevel.Trace);
            }
            else
            {
                // get a summary of all different Harmony ids involved
                MyMonitor.Log("all owners: " + patches.Owners, LogLevel.Trace);

                // get info about all Prefixes/Postfixes/Transpilers
                foreach (var patch in patches.Prefixes)
                {
                    MyMonitor.Log("index: " + patch.index, LogLevel.Trace);
                    MyMonitor.Log("owner: " + patch.owner, LogLevel.Trace);
                    MyMonitor.Log("patch method: " + patch.PatchMethod, LogLevel.Trace);
                    MyMonitor.Log("priority: " + patch.priority, LogLevel.Trace);
                    MyMonitor.Log("before: " + patch.before, LogLevel.Trace);
                    MyMonitor.Log("after: " + patch.after, LogLevel.Trace);
                }
            }
        }

        private void applyPatches()
        {
            //aboutPatch(typeof(AnimalHouse), "incubator");
            //aboutPatch(typeof(GameLocation), "performAction");
            //aboutPatch(typeof(AnimalHouse), "addNewHatchedAnimal");

            MyMonitor.Log($"Applying all patches", LogLevel.Trace);

            // patch dropping items into the incubator
            harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.performObjectDropInAction)),
                          prefix: new HarmonyMethod(typeof(IncubatorPatch), nameof(IncubatorPatch.performObjectDropInActionPrefix)));
            harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.performObjectDropInAction)),
                          postfix: new HarmonyMethod(typeof(IncubatorPatch), nameof(IncubatorPatch.performObjectDropInActionPostfix)));

            // patch hatching items from the incubator
            harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Events.QuestionEvent), nameof(StardewValley.Events.QuestionEvent.setUp)),
                         prefix: new HarmonyMethod(typeof(QuestionEventPatch), nameof(QuestionEventPatch.setUpPrefix)));
            harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Events.QuestionEvent), nameof(StardewValley.Events.QuestionEvent.setUp)),
                         postfix: new HarmonyMethod(typeof(QuestionEventPatch), nameof(QuestionEventPatch.setUpPostfix)));


            var ahm = Type.GetType("AnimalHusbandryMod.tools.InseminationSyringeOverrides, AnimalHusbandryMod");
            if (ahm == null)
            {
                MyMonitor.Log($"NO TYPE FOR SyringeOverrides", LogLevel.Trace);
            }
            else
            {
                var m = ahm.GetMethod("CheckCorrectProduct", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (m == null)
                {
                    MyMonitor.Log($"NO METHOD for CheckCorrectProduct", LogLevel.Trace);
                }
                else
                {
                    MyMonitor.Log($"PATCHING CheckCorrectProduct", LogLevel.Trace);

                    harmony.Patch(original: AccessTools.Method(ahm, m.Name),
                             prefix: new HarmonyMethod(typeof(AnimalHusbandryModPatch), nameof(AnimalHusbandryModPatch.CheckCorrectProductPrefix)));
                    
                }
                m = ahm.GetMethod("canThisBeAttached", BindingFlags.Public | BindingFlags.Static);
                if (m == null)
                {
                    MyMonitor.Log($"NO METHOD for canThisBeAttached", LogLevel.Trace);
                } else
                {
                    MyMonitor.Log($"PATCHING canThisBeAttached", LogLevel.Trace);

                    harmony.Patch(original: AccessTools.Method(ahm, m.Name),
                             prefix: new HarmonyMethod(typeof(AnimalHusbandryModPatch), nameof(AnimalHusbandryModPatch.canThisBeAttachedPrefix)));
                }
                m = ahm.GetMethod("beginUsing", BindingFlags.Public | BindingFlags.Static);
                if (m == null)
                {
                    MyMonitor.Log($"NO METHOD for beginUsing", LogLevel.Trace);
                }
                else
                {
                    MyMonitor.Log($"PATCHING beginUsing", LogLevel.Trace);

                    harmony.Patch(original: AccessTools.Method(ahm, m.Name),
                             prefix: new HarmonyMethod(typeof(AnimalHusbandryModPatch), nameof(AnimalHusbandryModPatch.beginUsingPrefix)));
                }
            }

            ahm = Type.GetType("AnimalHusbandryMod.animals.PregnancyController, AnimalHusbandryMod");
            if (ahm == null)
            {
                MyMonitor.Log($"NO TYPE FOR PregnancyController", LogLevel.Trace);
            }
            else
            {
                var m = ahm.GetMethod("addNewHatchedAnimal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (m == null)
                {
                    MyMonitor.Log($"NO METHOD for addNewHatchedAnimal", LogLevel.Trace);
                }
                else
                {
                    MyMonitor.Log($"PATCHING addNewHatchedAnimal", LogLevel.Trace);

                    harmony.Patch(original: AccessTools.Method(ahm, m.Name),
                             prefix: new HarmonyMethod(typeof(IncubatorPatch), nameof(IncubatorPatch.addNewHatchedAnimalPrefix)));

                }
            }

            ahm = Type.GetType("Paritee.StardewValley.Core.Locations.AnimalHouse, Paritee.StardewValley.Core");
            if (ahm == null)
            {
                MyMonitor.Log($"NO TYPE FOR AnimalHouse", LogLevel.Trace);
            }
            else
            {
                var m = ahm.GetMethod("GetRandomTypeFromIncubator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (m == null)
                {
                    MyMonitor.Log($"NO METHOD for GetRandomTypeFromIncubator", LogLevel.Trace);
                }
                else
                {
                    MyMonitor.Log($"PATCHING GetRandomTypeFromIncubator", LogLevel.Trace);

                    harmony.Patch(original: AccessTools.Method(ahm, m.Name),
                             prefix: new HarmonyMethod(typeof(IncubatorPatch), nameof(IncubatorPatch.getRandomTypeFromIncubatorPatch)));

                }

                m = ahm.GetMethod("GetIncubatorHatchEvent", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (m == null)
                {
                    MyMonitor.Log($"NO METHOD for GetIncubatorHatchEvent", LogLevel.Trace);
                }
                else
                {
                    MyMonitor.Log($"PATCHING GetIncubatorHatchEvent", LogLevel.Trace);

                    harmony.Patch(original: AccessTools.Method(ahm, m.Name),
                             prefix: new HarmonyMethod(typeof(IncubatorPatch), nameof(IncubatorPatch.getIncubatorHatchEventPatch)));

                }
            }

            ahm = Type.GetType("Paritee.StardewValley.Core.Characters.FarmAnimal, Paritee.StardewValley.Core");
            if (ahm == null)
            {
                MyMonitor.Log($"NO TYPE FOR FarmAnimal", LogLevel.Trace);
            }
            else
            {
                MethodInfo m = null;
                foreach(MethodInfo mx in ahm.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    if ((mx.Name == "GetRandomTypeFromProduce") && (mx.GetParameters().Length == 2) && mx.GetParameters()[0].ParameterType == typeof(FarmAnimal) && mx.GetParameters()[1].ParameterType == typeof(Dictionary<string, List<string>>) && mx.ReturnType == typeof(string))
                    {
                        m = mx;
                        break;
                    }
                }
                if (m == null)
                {
                    MyMonitor.Log($"NO METHOD for GetRandomTypeFromProduce", LogLevel.Trace);
                }
                else
                {
                    MyMonitor.Log($"PATCHING GetRandomTypeFromProduce", LogLevel.Trace);

                    harmony.Patch(original: m,
                             prefix: new HarmonyMethod(typeof(IncubatorPatch), nameof(IncubatorPatch.getRandomTypeFromProducePatch)));

                }
            }
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            this.Monitor.Log($"Patching in Breeeding Overhaul for {MyHelper.ModRegistry.ModID}.", LogLevel.Trace);
            applyPatches();
        }
    }
}