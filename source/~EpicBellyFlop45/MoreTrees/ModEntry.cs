/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoreTrees.Config;
using MoreTrees.Models;
using MoreTrees.Patches;
using MoreTrees.Tools;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoreTrees
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary>The sprite for the <see cref="Tools.BarkRemover"/>.</summary>
        private Texture2D BarkRemoverTexture;


        /*********
        ** Accessors
        *********/
        /// <summary>The singletong instance of <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; set; }

        /// <summary>The mod configuration.</summary>
        public ModConfig Config { get; set; }

        /// <summary>Provides basic More Trees apis.</summary>
        public IApi Api { get; set; }

        /// <summary>All the loaded trees.</summary>
        public List<CustomTree> LoadedTrees { get; set; } = new List<CustomTree>();

        /// <summary>Data about trees that gets saved.</summary>
        public List<SavePersistantTreeData> SavedTreeData { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Api = new Api();

            Config = this.Helper.ReadConfig<ModConfig>();
            BarkRemoverTexture = this.Helper.Content.Load<Texture2D>(Path.Combine("assets", "BarkRemover.png"));

            if (Config.EnableExtendedMode && BarkRemoverTexture == null)
            {
                this.Monitor.Log("ExtendedMode is enabled but the BarkRemover texture couldn't be found. Try reinstalling More Trees. ExtendedMode disabled.", LogLevel.Error);
                Config.EnableExtendedMode = false;
            }

            ApplyHarmonyPatches();

            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            this.Helper.Events.GameLoop.Saved += OnSaved;
            this.Helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        // TODO: remove
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.T)
            {
                Game1.player.addItemToInventory(new BarkRemover());
            }
        }

        /// <summary>This will call when loading each asset, if the mail asset is being loaded, return true as we want to edit this.</summary>
        /// <typeparam name="T">The type of the assets being loaded.</typeparam>
        /// <param name="asset">The asset info being loaded.</param>
        /// <returns>True if the assets being loaded needs to be edited.</returns>
        public bool CanEdit<T>(IAssetInfo asset) => ModEntry.Instance.Config.EnableExtendedMode && asset.AssetNameEquals(Path.Combine("TileSheets", "tools"));

        /// <summary>Edit the tools asset to add the the Bark Remover.</summary>
        /// <typeparam name="T">The type of the assets being loaded.</typeparam>
        /// <param name="asset">The asset data being loaded.</param>
        public void Edit<T>(IAssetData asset)
        {
            var image = asset.AsImage();

            // extend image
            image.ExtendImage(336, 416);

            // add custom tool sprites
            image.PatchImage(ModEntry.Instance.BarkRemoverTexture, targetArea: new Rectangle(0, 400, 96, 16));
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Apply the harmony patches.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), "loadTexture"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.LoadTexturePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), "shake"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.ShakePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.performToolAction)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.PerformToolActionPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.UpdateTapperProduct)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.UpdateTapperProductPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.draw)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.DrawPrefix)))
            );
        }

        /// <summary>Invoked when the player loads a save.</summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // load all content packs
            foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
            {
                try
                {
                    LoadContentPack(contentPack);
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Failed to load content pack: {ex}");
                }
            }

            // load saved tree data
            SavedTreeData = GetSavePersistantTreeData();

            // place trees on maps using tree tile data
            foreach (var location in Game1.locations)
            {
                var map = location.Map;
                var backLayer = map.GetLayer("Back");
                if (backLayer == null)
                    continue;

                for (int x = 0; x < backLayer.LayerWidth; x++)
                {
                    for (int y = 0; y < backLayer.LayerHeight; y++)
                    {
                        // check if a tree property is on the tile
                        var treeProperty = location.doesTileHaveProperty(x, y, "Tree", "Back");
                        if (treeProperty == null)
                            continue;

                        // ensure the tree tile value is valid
                        var treeType = ModEntry.Instance.Api.GetTreeType(treeProperty);
                        if (treeType == -1)
                        {
                            ModEntry.Instance.Monitor.Log($"Tree property found at tile: (X:{x} Y:{y}) on map: {location.Name} but tree type: {treeProperty} couldn't be found.", LogLevel.Warn);
                            continue;
                        }

                        // ensure a terrain feature isn't already in it's place
                        if (!location.terrainFeatures.ContainsKey(new Vector2(x, y)))
                        {
                            location.terrainFeatures.Add(new Vector2(x, y), new Tree(treeType, 5));

                            var customTree = ModEntry.Instance.Api.GetTreeByType(treeType);

                            // set default shake produce values to 0 (so player doesn't need to wait for the required number of days straight away)
                            List<int> daysTillNextShakeProduce = new List<int>();
                            for (int i = 0; i < customTree.Data.ShakingProducts.Count; i++)
                                daysTillNextShakeProduce.Add(0);

                            SavedTreeData.Add(new SavePersistantTreeData(new Vector2(x, y), 0, daysTillNextShakeProduce));
                        }
                    }
                }
            }
        }

        /// <summary>Invoked when the player saves the game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            // decrement the number of days till next produce
            foreach (var savedTreeData in SavedTreeData)
            {
                savedTreeData.DaysTillNextBarkHarvest = Math.Max(0, savedTreeData.DaysTillNextBarkHarvest - 1);
                for (int i = 0; i < savedTreeData.DaysTillNextShakeProduct.Count; i++)
                    savedTreeData.DaysTillNextShakeProduct[i] = Math.Max(0, savedTreeData.DaysTillNextShakeProduct[i] - 1);
            }

            // add newly planted trees
            foreach (var terrainFeature in Game1.getFarm().terrainFeatures.Values)
            {
                var tree = terrainFeature as Tree;
                if (tree == null || tree.treeType <= 7) // don't add to the persistant save if it's a default tree
                    continue;

                if (!SavedTreeData.Where(treeData => treeData.TileLocation == terrainFeature.currentTileLocation).Any())
                {
                    // tree doesn't exist in custom saved data, add it
                    var customTree = Api.GetTreeByType(tree.treeType);

                    List<int> daysTillNextShakeProduce = new List<int>();
                    for (int i = 0; i < customTree.Data.ShakingProducts.Count; i++)
                        daysTillNextShakeProduce.Add(0);

                    SavedTreeData.Add(new SavePersistantTreeData(terrainFeature.currentTileLocation, 0, daysTillNextShakeProduce));
                }
            }

            // remove newly cut down trees
            for (int i = SavedTreeData.Count - 1; i >= 0; i--)
            {
                // check if the tree still exists, if not, delete it
                if (!Game1.getFarm().terrainFeatures.ContainsKey(SavedTreeData[i].TileLocation))
                    SavedTreeData.RemoveAt(i);
            }

            // save the custom save data
            SetSavePersistantTreeData();
        }

        /// <summary>Get the treeData.json file content.</summary>
        private List<SavePersistantTreeData> GetSavePersistantTreeData()
        {
            // get the content of the treeTypes.json file
            var treeDataPath = GetSavePersistantTreeDataPath();
            var treeData = File.ReadAllText(treeDataPath);
            List<SavePersistantTreeData> savedTreeData = null;

            // parse file data
            try
            {
                savedTreeData = JsonConvert.DeserializeObject<List<SavePersistantTreeData>>(treeData);
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"TreeData couldn't be deserialised. TreeData will be ignored, this will result in all trees having bark and drops being reset.\nPath: {treeDataPath}\nError: {ex}", LogLevel.Error);
            }

            return savedTreeData ?? new List<SavePersistantTreeData>();
        }

        /// <summary>Set the treeData.json file content.</summary>
        private void SetSavePersistantTreeData()
        {
            var treeDataPath = GetSavePersistantTreeDataPath();

            try
            {
                File.WriteAllText(treeDataPath, JsonConvert.SerializeObject(SavedTreeData));
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"TreeData couldn't be serialized.\nPath: {treeDataPath}\tError: {ex}", LogLevel.Error);
            }
        }

        /// <summary>Get the treeData.json path.</summary>
        /// <returns>The treeData.json path.</returns>
        private string GetSavePersistantTreeDataPath()
        {
            // get/create the directory containing the treeData File
            var treeTypesFileDirectory = Path.Combine(Constants.CurrentSavePath, "MoreTrees");
            if (!Directory.Exists(treeTypesFileDirectory))
                Directory.CreateDirectory(treeTypesFileDirectory);

            // get/create the treeData File
            var treeTypesFilePath = Path.Combine(treeTypesFileDirectory, "treeData.json");
            if (!File.Exists(treeTypesFilePath))
                File.Create(treeTypesFilePath).Close();

            return treeTypesFilePath;
        }
        
        /// <summary>Load the passed content pack.</summary>
        /// <param name="contentPack">The content pack to load.</param>
        private void LoadContentPack(IContentPack contentPack)
        {
            this.Monitor.Log($"Loading content pack: {contentPack.Manifest.Name}", LogLevel.Info);

            // load each tree
            var modDirectory = new DirectoryInfo(contentPack.DirectoryPath);
            foreach (var treePath in modDirectory.EnumerateDirectories())
            {
                // ensure tree.png exists
                var isValid = true;
                if (!File.Exists(Path.Combine(treePath.FullName, "tree.png")))
                {
                    this.Monitor.Log($"tree.png couldn't be found for {contentPack.Manifest.Name}.", LogLevel.Error);
                    isValid = false;
                }

                // ensure content.json exists
                if (!File.Exists(Path.Combine(treePath.FullName, "content.json")))
                {
                    this.Monitor.Log($"content.json couldn't be found for {contentPack.Manifest.Name}.", LogLevel.Error);
                    isValid = false;
                }

                if (!isValid)
                    continue;

                var treeTexture = contentPack.LoadAsset<Texture2D>(Path.Combine(treePath.Name, "tree.png"));
                var treeData = contentPack.ReadJsonFile<TreeData>(Path.Combine(treePath.Name, "content.json"));
                if (treeData == null)
                {
                    this.Monitor.Log($"Content.json couldn't be found for: {treePath.Name}.", LogLevel.Error);
                    continue;
                }

                treeData.ResolveTokens();
                if (!treeData.IsValid())
                {
                    this.Monitor.Log($"Validation for treeData for: {treePath.Name} failed, skipping.", LogLevel.Error);
                    continue;
                }

                // ensure the tree can be loaded (using IncludeIfModIsPresent)
                {
                    var loadTree = true;
                    if (treeData.IncludeIfModIsPresent != null && treeData.IncludeIfModIsPresent.Count > 0)
                    {
                        // set this to false so it can be set to true if a required mod is found
                        loadTree = false;
                        foreach (var requiredMod in treeData.IncludeIfModIsPresent)
                        {
                            if (!this.Helper.ModRegistry.IsLoaded(requiredMod))
                                continue;

                            loadTree = true;
                            break;
                        }
                    }
                    if (!loadTree)
                    {
                        this.Monitor.Log("Tree won't get loaded as no mods specified in 'IncludeIfModIsPresent' were present.", LogLevel.Info);
                        continue;
                    }
                }

                // ensure the tree can be loaded (using ExcludeIfModIsPresent)
                {
                    var loadTree = true;
                    if (treeData.ExcludeIfModIsPresent != null && treeData.ExcludeIfModIsPresent.Count > 0)
                    {
                        foreach (var unwantedMod in treeData.ExcludeIfModIsPresent)
                        {
                            if (!this.Helper.ModRegistry.IsLoaded(unwantedMod))
                                continue;

                            loadTree = false;
                            break;
                        }
                    }
                    if (!loadTree)
                    {
                        this.Monitor.Log("Tree won't get loaded as a mod specified in 'ExcludeIfModIsPresent' was present.", LogLevel.Info);
                        continue;
                    }
                }

                // ensure the tree hasn't been added by another mod
                if (LoadedTrees.Where(tree => tree.Name.ToLower() == treePath.Name.ToLower()).Any())
                {
                    this.Monitor.Log($"A tree by the name: {treePath.Name} has already been added.", LogLevel.Error);
                    continue;
                }

                // get the tree type, use the api as they're save persitant
                var treeType = Api.GetTreeType(treePath.Name);

                // add the tree to the loaded trees
                LoadedTrees.Add(new CustomTree(treeType, treePath.Name, treeData, treeTexture));
            }
        }
    }
}
