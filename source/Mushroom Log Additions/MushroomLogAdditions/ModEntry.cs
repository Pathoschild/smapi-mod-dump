/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/justastranger/MushroomLogAdditions
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;
using StardewValley.GameData.Machines;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace MushroomLogAdditions
{
    public class ModEntry : Mod
    {
        internal Config config;
        internal ITranslationHelper i18n => Helper.Translation;

        internal static MushroomLogData treeToOutputDict = new();

        internal static ModEntry instance;
        internal static Harmony harmony;

        public override void Entry(IModHelper helper)
        {
            string startingMessage = i18n.Get("MushroomLogAdditions.start");
            Monitor.Log(startingMessage, LogLevel.Trace);
            instance = this;
            config = helper.ReadConfig<Config>();
            helper.Events.GameLoop.SaveLoaded += CollectOutputs;
            Helper.Events.Content.AssetRequested += AssetRequested;
        }

        // Thanks Wren

        private void AssetRequested(object? sender, AssetRequestedEventArgs ev)
        {
            if (ev.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
              ev.Edit(EditMachines, AssetEditPriority.Default);
        }

        private void EditMachines(IAssetData asset)
        {
            if (asset.Data is Dictionary<string, MachineData> data && data.TryGetValue("(BC)MushroomLog", out var machine))
            {
                var output = machine.OutputRules.Where(r => r.Id == "Default").FirstOrDefault();
                if (output is null) // not found
                    return;
                var item = output.OutputItem.Where(i => i.Id == "???").FirstOrDefault();
                if (item is not null)
                    item.OutputMethod = "MushroomLogAdditions.ModEntry, MushroomLogAdditions: OutputMushroomLog";
            }
        }

        private void CollectOutputs(object? sender, SaveLoadedEventArgs e)
        {

            // This framework comes with one addition
            // It was the entire point of writing this and doubles as an example of the format

            // Tell SMAPI that the `.\internal` folder is a content pack
            IContentPack internalContentPack = Helper.ContentPacks.CreateTemporary(
                directoryPath: Path.Combine(Helper.DirectoryPath, "internal"),
                id: "JAS.MushroomLogAdditions.Internal",
                name: "Mushroom Log Additions Internal Pack",
                description: "Adds mushroom trees->mushroom seeds to the Mushroom Log results.",
                author: instance.ModManifest.Author,
                version: instance.ModManifest.Version
            );
            // initialize the default vanilla behavior or die trying
            treeToOutputDict = internalContentPack.ReadJsonFile<MushroomLogData>("VanillaMushroomLogData.json") ?? throw(new NullReferenceException("Vanilla Mushroom Log Data returned Null value."));

            MushroomLogData? data;
            // true by default
            if (instance.config.loadInternal)
            {
                // load the internal custom datapack
                data = internalContentPack.ReadJsonFile<MushroomLogData>("MushroomLogData.json");
                if (data != null && data.Count > 0)
                {
                    // this should never fail the check
                    data.ToList().ForEach(x => { treeToOutputDict[x.Key] = x.Value; });
                    Monitor.Log("Loaded internal content pack.");
                }
                else Monitor.Log("Internal content pack failed to load.", LogLevel.Error); // *cough*
            }

            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}", LogLevel.Trace);
                if (contentPack.HasFile("MushroomLogData.json"))
                {
                    data = contentPack.ReadJsonFile<MushroomLogData>("MushroomLogData.json");
                    if (data != null && data.Count > 0)
                    {
                        // merge the two dictionaries, overwriting values
                        // TODO merge the List value too
                        Monitor.Log($"Content pack loaded: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}", LogLevel.Trace);
                        var overlap = data.Keys.Intersect(treeToOutputDict.Keys);
                        if (overlap.Any())
                        {
                            Monitor.Log($"Detected duplicate TreeTypes when loading {contentPack.Manifest.Name} : {JsonConvert.SerializeObject(overlap)}. Overwriting old outputs with new values.", LogLevel.Info);
                        }
                        data.ToList().ForEach(x => {treeToOutputDict[x.Key] = x.Value;});
                    }
                }
            }

            instance.Monitor.Log("Content packs loaded, current additions:", LogLevel.Trace);
            instance.Monitor.Log(JsonConvert.SerializeObject(treeToOutputDict), LogLevel.Trace);
        }

        public static Item OutputMushroomLog(StardewValley.Object machine, Item inputItem, bool probe, MachineItemOutput outputData, out int? overrideMinutesUntilReady)
        {
            overrideMinutesUntilReady = null;

            // we have to clone the vanilla code since we can't access any of the original method's local variables
            // otherwise this would've been a simple postfix...
            List<Tree> nearbyTrees = new();
            int scanRadius = instance.config.scanRadius;
            for (int x = (int)machine.TileLocation.X - scanRadius; x < (int)machine.TileLocation.X + scanRadius + 1; x++)
            {
                for (int y = (int)machine.TileLocation.Y - scanRadius; y < (int)machine.TileLocation.Y + scanRadius + 1; y++)
                {
                    Vector2 v = new(x, y);
                    if (machine.Location.terrainFeatures.ContainsKey(v) && machine.Location.terrainFeatures[v] is Tree tree)
                    {
                        nearbyTrees.Add(tree);
                    }
                    // TODO else if (machine.Location.terrainFeatures.ContainsKey(v) && machine.Location.terrainFeatures[v] is FruitTree fruitTree)
                    // nearbyTrees would have to be swapped to a List<TerrainFeature>
                }
            }
            int treeCount = nearbyTrees.Count;
            List<string> mushroomPossibilities = new();
            int mossyCount = 0;
            foreach (Tree tree in nearbyTrees)
            {
                if (tree.growthStage.Value >= 5)
                {
                    string treeType = tree.treeType.Value;
                    // Default result from any tree
                    string mushroomType = (Game1.random.NextBool(0.05) ? "(O)422" : (Game1.random.NextBool(0.15) ? "(O)420" : "(O)404"));
                    // check to see if the scanned tree is registered as having an output
                    if (treeToOutputDict.TryGetValue(treeType, out List<OutputWithChance>? mushroomTypes))
                    {
                        // if there's something registered and there's no chicanery with the list
                        if (mushroomTypes != null && mushroomTypes.Any())
                        {
                            // iterate through list
                            foreach (OutputWithChance output in mushroomTypes)
                            {
                                // Roll to select entry in the list and move on so that tree's output can be added to the pool
                                // grabs the first item in the list that it can
                                // mushroomType doesn't get reassigned from the default if none of the outputs are selected
                                if (Game1.random.NextBool(output.Item2))
                                {
                                    mushroomType = output.Item1;
                                    break;
                                }
                            }
                        }
                    }
                    // if none were registered, the originally assigned output is used
                    mushroomPossibilities.Add(mushroomType);
                    // Vanilla function uses moss below as a factor in the quality level
                    if (tree.hasMoss.Value)
                    {
                        mossyCount++;
                    }
                }
            }

            for (int i = 0; i < Math.Max(1, (int)(nearbyTrees.Count * 0.75f)); i++)
            {
                mushroomPossibilities.Add(Game1.random.NextBool(0.05) ? "(O)422" : (Game1.random.NextBool(0.15) ? "(O)420" : "(O)404"));
            }
            int amount = Math.Max(1, Math.Min(5, Game1.random.Next(1, 3) * (nearbyTrees.Count / 2)));
            int quality = 0;
            float qualityBoostChance = mossyCount * 0.025f + treeCount * 0.025f;
            // repeatedly roll unitl the first failure or iridium quality is hit
            while (Game1.random.NextDouble() < (double)qualityBoostChance)
            {
                quality++;
                if (quality == 3)
                {
                    quality = 4;
                    break;
                }
            }
            // re-roll the output using the new pool with the old stack amount and quality values
            return ItemRegistry.Create(Game1.random.ChooseFrom(mushroomPossibilities), amount, quality, false);
        }
    }
}