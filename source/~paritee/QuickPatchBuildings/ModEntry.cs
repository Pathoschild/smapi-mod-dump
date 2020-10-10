/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickPatchBuildings
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
    {
        IEnumerable<IContentPack> ContentPacks;
        private Dictionary<string, List<BuildingPatch>> BuildingPatches;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.BuildingPatches = new Dictionary<string, List<BuildingPatch>>();

            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            foreach (BuildingPatch patch in this.BuildingPatches.Values.SelectMany(x => x).ToList())
            {
                if (asset.AssetNameEquals($"Buildings/{patch.Type}"))
                    return true;
            }

            return false;
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            // change building blueprints
            if (asset.AssetNameEquals("Data/Blueprints"))
                return true;

            return false;
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            // go through the content packs
            foreach (IContentPack contentPack in this.ContentPacks)
            {
                // go through the patches
                foreach (BuildingPatch patch in this.BuildingPatches[contentPack.Manifest.UniqueID])
                {
                    if (asset.AssetNameEquals($"Buildings/{patch.Type}"))
                    {
                        // try to load the seasonal asset first
                        try
                        {
                            if (patch.Seasonal)
                                return contentPack.LoadAsset<T>(patch.GetSeasonalAsset(Game1.currentSeason));
                        }
                        catch
                        {
                            this.Monitor.Log($"Could not find an asset that matched {patch.GetSeasonalAsset(Game1.currentSeason)} in {contentPack.Manifest.UniqueID}.", LogLevel.Trace);
                        }

                        // no seasonal found, try to load the default
                        return contentPack.LoadAsset<T>(patch.Asset);
                    }
                }
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            // change data buildings
            if (asset.AssetNameEquals("Data/Blueprints"))
            {
                // read data file
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                foreach (KeyValuePair<string, List<BuildingPatch>> patches in this.BuildingPatches)
                {
                    foreach (BuildingPatch patch in patches.Value)
                        data.Add(patch.Type, patch.Data); // add building patches
                }
            }
        }

        /// <summary>The event called after the game is launched.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.ContentPacks = this.Helper.ContentPacks.GetOwned();

            Dictionary<string, string> data = this.Helper.Content.Load<Dictionary<string, string>>("Data/Blueprints", ContentSource.GameContent);

            foreach (IContentPack contentPack in this.ContentPacks)
            {
                try
                {
                    this.BuildingPatches.Add(contentPack.Manifest.UniqueID, new List<BuildingPatch>());

                    Content content = contentPack.ReadJsonFile<Content>("content.json");

                    foreach (BuildingPatch patch in content.Changes)
                    {
                        // validate against duplicates
                        if (data.ContainsKey(patch.Type))
                        {
                            this.Monitor.Log($"{contentPack.Manifest.UniqueID} is attempting to add a duplicate blueprint for {patch.Type} and will be ignored", LogLevel.Warn);

                            continue;
                        }

                        this.BuildingPatches[contentPack.Manifest.UniqueID].Add(patch);
                    }
                }
                catch
                {
                    this.Monitor.Log($"Could not find valid content.json in {contentPack.Manifest.UniqueID}", LogLevel.Warn);
                }
            }
        }

        /// <summary>The event called after the day is started.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Invalidate the cache just in case seasonal assets exist
            foreach (IContentPack contentPack in this.ContentPacks)
            {
                foreach (BuildingPatch patch in this.BuildingPatches[contentPack.Manifest.UniqueID])
                {
                    if (patch.Seasonal)
                        this.Helper.Content.InvalidateCache($"Buildings/{patch.Type}");
                }
            }
        }

        /// <summary>The event called after an active menu is opened or closed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // add blueprints
            if (e.NewMenu is CarpenterMenu)
            {
                CarpenterMenu carpenterMenu = e.NewMenu as CarpenterMenu;

                bool magicalConstruction = this.Helper.Reflection
                    .GetField<bool>(e.NewMenu, "magicalConstruction")
                    .GetValue();

                // get field
                IList<BluePrint> blueprints = this.Helper.Reflection
                    .GetField<List<BluePrint>>(e.NewMenu, "blueprints")
                    .GetValue();

                List<BuildingPatch> buildingPatches = this.BuildingPatches.Values // to get just the List<BuildingPatch>s
                     .SelectMany(x => x.Where(patch => (magicalConstruction && patch.IsMagical()) || (!magicalConstruction && !patch.IsMagical())))  // flatten and remove magical if necessary
                     .ToList(); // convert to list

                foreach (BuildingPatch patch in buildingPatches)
                    blueprints.Add(new BluePrint(patch.Type)); // add unloaded blueprints
            }
        }
    }
}
