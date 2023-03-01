/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using StardewValley;
using Newtonsoft.Json.Linq;
using System.Collections;
using Newtonsoft.Json;
using static Pathoschild.Stardew.LookupAnything.Framework.Constants.Constant;
using System.Diagnostics;

namespace LookupAnything_FTMIntegration
{
    internal class ModEntry : Mod
    {
        internal static Dictionary<int, List<SpawnInfo>> db = new Dictionary<int, List<SpawnInfo>>();
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            LookupMenuPatches.Init(harmony, helper);
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
#if DEBUG
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
#endif
        }
#if DEBUG
        // Temporary so I can keep hot reloading without restarting the day
        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsMainPlayer && Context.IsMultiplayer)
            {
                if (e.Button == SButton.Enter)
                {

                    ProcessFTMData();
                }
            }
        }
#endif
        // FTM has Event Priority Low, so we are slightly after it
        [EventPriority(EventPriority.Low - 1)]
        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            ProcessFTMData();
        }

        private void ProcessFTMData()
        {
            var type = Type.GetType("FarmTypeManager.ModEntry+Utility,FarmTypeManager");
            var ftmDataType = Helper.Reflection.GetField<object>(type, "FarmDataList");
            var ftmData = ftmDataType.GetValue();

            // Get the T in List<List<T>>
            var typeInfo = ftmData.GetType().GetGenericArguments()[0];

            // Create a version of ProcessContents<> that accepts the runtime version of TimedSpawns, and invoke it
            var processMethod = typeof(ModEntry).GetMethod(nameof(ProcessList), BindingFlags.NonPublic|BindingFlags.InvokeMethod|BindingFlags.Instance);
            var processMethodGeneric = processMethod.MakeGenericMethod(typeInfo);


            db = new Dictionary<int, List<SpawnInfo>>();

            processMethodGeneric.Invoke(this, new object[] { ftmData });
#if DEBUG
            Monitor.Log(JsonConvert.SerializeObject(db, Formatting.Indented), LogLevel.Debug);
#endif
        }
        private void ProcessList<T>(List<T> ftmData)
        {
            Monitor.Log($"Processing {ftmData.Count} FTM Contents", LogLevel.Alert);
            foreach (var pack in ftmData)
            {
                ProcessSingle(pack);
            }
        }
        // Inner Contents being a stand alone method so it can be hot loaded when the generic can't.
        private void ProcessSingle(object pack)
        {
            var config = pack.GetType().GetProperty("Config").GetValue(pack);
            if (config == null) return;
            var a = config.GetType();
            var b = a.GetProperty("ForageSpawnEnabled");
            var forageEnabled = b.GetValue(config) as bool? ?? false;
            if (!forageEnabled) return;
            var forageSettings = config.GetType().GetProperty("Forage_Spawn_Settings").GetValue(config);
            if (forageSettings == null) return;

            var globalSpring = forageSettings.GetType().GetProperty("SpringItemIndex").GetValue(forageSettings) as object[];
            var globalSummer = forageSettings.GetType().GetProperty("SummerItemIndex").GetValue(forageSettings) as object[];
            var globalAutumn = forageSettings.GetType().GetProperty(  "FallItemIndex").GetValue(forageSettings) as object[];
            var globalWinter = forageSettings.GetType().GetProperty("WinterItemIndex").GetValue(forageSettings) as object[];

            var areas = forageSettings.GetType().GetProperty("Areas").GetValue(forageSettings) as object[];
            if (areas == null || areas.Length == 0) return;
            foreach (var area in areas)
            {
                var mapName = area.GetType().GetProperty("MapName").GetValue(area) as string;
                var minSpawns = area.GetType().GetProperty("MinimumSpawnsPerDay").GetValue(area) as int?;
                var maxSpawns = area.GetType().GetProperty("MaximumSpawnsPerDay").GetValue(area) as int?;

                var spring = area.GetType().GetProperty("SpringItemIndex").GetValue(area) as object[];
                if (spring == null) spring = globalSpring;
                ParseSeason(mapName, "spring", spring);


                var summer = area.GetType().GetProperty("SummerItemIndex").GetValue(area) as object[];
                if (summer == null) summer = globalSummer;
                ParseSeason(mapName, "summer", summer);

                var autumn = area.GetType().GetProperty(  "FallItemIndex").GetValue(area) as object[];
                if (autumn == null) autumn = globalAutumn;
                ParseSeason(mapName,   "fall", autumn);

                var winter = area.GetType().GetProperty("WinterItemIndex").GetValue(area) as object[];
                if (winter == null) winter = globalWinter;
                ParseSeason(mapName, "winter", winter);
            }
        }
        internal record ItemInfo(SpawnType type, int itemId, int stackSize, int? percentChance, int weight, List<ItemInfo> contents);
        internal enum SpawnType
        {
            Other,
            Forage,
            Container,
            ArtifactSpot,
        }
        internal record SpawnInfo(string mapName, string season, int stackSize, int? percentChance, int weight, SpawnType spawnType);
        internal record ContainerSpawnInfo(string mapName, string season, int stackSize, int? percentChance, int weight, SpawnType spawnType, List<ItemInfo> contents) : SpawnInfo(mapName, season, stackSize, percentChance, weight, spawnType);
        private void ParseSeason(string mapName, string season, object[] rawData)
        {
            var parsedData = ParseItemList(rawData);
            foreach (var item in parsedData)
            {

                switch (item.type)
                {
                    case SpawnType.Forage:
                        if (!db.TryGetValue(item.itemId, out _)) db[item.itemId] = new();
                        db[item.itemId].Add(new SpawnInfo(mapName, season, item.stackSize, item.percentChance, item.weight, item.type));
                        break;
                    case SpawnType.ArtifactSpot:
                    case SpawnType.Container:
                        foreach (var innerItem in item.contents)
                        {
                            if (!db.TryGetValue(innerItem.itemId, out _)) db[innerItem.itemId] = new();
                            db[innerItem.itemId].Add(new ContainerSpawnInfo(mapName, season, item.stackSize, item.percentChance, item.weight, item.type, item.contents));
                        }
                        break;
                }
            }
        }
        private int ParseObjectName(string objName)
        {
            int objectId = -1;
            // if its a number in a string, use it, if its not, try parsing Game1.objectInformation instead
            if (!int.TryParse(objName, out objectId))
            {
                var itemResult = Game1.objectInformation.FirstOrDefault(kv => kv.Value.Split('/')[0].Equals(objName, StringComparison.OrdinalIgnoreCase));
                // KeyValuePair is a struct, so its got a default and not null
                if (!itemResult.Equals(default(KeyValuePair<int, string>)))
                {
                    objectId = itemResult.Key;
                }
            }
            return objectId;
        }
        private List<ItemInfo> ParseItemList(IEnumerable itemList)
        {
            var returnList = new List<ItemInfo>();
            foreach (var item in itemList)
            {
                int objectId = -1;
                int stackSize = 1;
                int? percentChance = null;
                int weight = 1;
                List<ItemInfo> contents = new List<ItemInfo>();
                SpawnType spawnType = SpawnType.Other;

                if (item is long unqualifiedId)
                {
                    objectId = Convert.ToInt32(unqualifiedId);
                    spawnType = SpawnType.Forage;
                }
                else if (item is string itemName)
                {
                    objectId = ParseObjectName(itemName);
                    if (objectId != -1) spawnType = SpawnType.Forage;

                }
                else if (item is JObject rawObject)
                {
                    var itemCategory = (string)rawObject.GetValue("Category", StringComparison.OrdinalIgnoreCase);
                    if (itemCategory == null) continue;

                    if (rawObject.TryGetValue("PercentChanceToSpawn", StringComparison.OrdinalIgnoreCase, out JToken rawPercent))
                    {
                        percentChance = (int)rawPercent;
                    }
                    if (rawObject.TryGetValue("SpawnWeight", StringComparison.OrdinalIgnoreCase, out JToken rawWeight))
                    {
                        weight = (int)rawWeight;
                    }
                    switch (itemCategory.ToLower())
                    {
                        case "object":
                        case "objects":
                            // Handle Item Behavior
                            var itemComplexName = (string)rawObject.GetValue("Name", StringComparison.OrdinalIgnoreCase);
                            objectId = ParseObjectName(itemComplexName);
                            if (rawObject.TryGetValue("Stack", StringComparison.OrdinalIgnoreCase, out JToken itemStackCount))
                            {
                                stackSize = (int)itemStackCount;
                            }
                            spawnType = SpawnType.Forage;
                            break;
                        case "buried":
                        case "burieditem":
                        case "burieditems":
                        case "buried item":
                        case "buried items":
                            spawnType = SpawnType.ArtifactSpot;
                            goto case "crates";
                        case "barrel":
                        case "barrels":
                        case "breakable":
                        case "breakables":
                        case "chest":
                        case "chests":
                        case "crate":
                        case "crates":
                            // Handle "Container" logic (check Contents)
                            var itemContents = rawObject.GetValue("Contents", StringComparison.OrdinalIgnoreCase);
                            if (itemContents != null)
                            {
                                var itemContentArray = (JArray)itemContents;
                                contents = ParseItemList(itemContentArray);
                            }
                            if (spawnType != SpawnType.ArtifactSpot)
                            {
                                spawnType = SpawnType.Container;
                            }
                            break;
                        case "dga":
                        default:
                            // Ignore it, future me problem
                            break;
                    }
                }
                returnList.Add(new ItemInfo(spawnType, objectId, stackSize, percentChance, weight, contents));
            }
            return returnList;
        }
    }
}
