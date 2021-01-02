/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using BarkingUpTheRightTree.Models.Converted;
using BarkingUpTheRightTree.Models.Parsed;
using BarkingUpTheRightTree.Patches;
using BarkingUpTheRightTree.Tools;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BarkingUpTheRightTree
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary>The sprite for the <see cref="BarkingUpTheRightTree.Tools.BarkRemover"/>.</summary>
        private Texture2D BarkRemoverTexture;

        /// <summary>The id map for the trees.</summary>
        /// <remarks>Key: tree name, Value: tree id.</remarks>
        private Dictionary<string, int> IdMap = new Dictionary<string, int>();


        /*********
        ** Accessors
        *********/
        /// <summary>Provides basic tree apis.</summary>
        public IApi Api { get; private set; }

        /// <summary>The custom trees before they get converted to <see cref="BarkingUpTheRightTree.Models.Converted.CustomTree"/>s.</summary>
        /// <remarks>This is needed because trees need to be able to be added before a save gets loaded. <br/>This is because <see cref="StardewValley.GameLocation.loadObjects"/> gets called before the save gets loaded if the player is creating a new save.</remarks>
        internal List<(int Id, ParsedCustomTree Data, Texture2D Texture)> RawCustomTrees { get; } = new List<(int Id, ParsedCustomTree ParsedCustomTree, Texture2D TreeTexture)>();

        /// <summary>All the custom trees.</summary>
        internal List<CustomTree> CustomTrees { get; } = new List<CustomTree>();

        /// <summary>The names of all the trees added by each mod.</summary>
        /// <remarks>Key: mod name, Value: tree names.</remarks>
        internal Dictionary<string, List<string>> TreesByMod { get; } = new Dictionary<string, List<string>>();

        /// <summary>The singleton instance of <see cref="BarkingUpTheRightTree.ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // run mod initialisation on game lauch so all mods can run initialise code first (required to access other mod apis)
            this.Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <inheritdoc/>
        public bool CanEdit<T>(IAssetInfo asset) => asset.AssetNameEquals(Path.Combine("TileSheets", "tools"));

        /// <inheritdoc/>
        public void Edit<T>(IAssetData asset)
        {
            var image = asset.AsImage();
            image.ExtendImage(336, 448);
            image.PatchImage(BarkRemoverTexture, targetArea: new Rectangle(0, 432, 96, 16));
        }


        /*********
        ** Internal Methods
        *********/
        /// <summary>Converts the raw tree data to regular tree data.</summary>
        /// <remarks>A save must be loaded inorder to parse the data (as that's when Json Assets gets loaded for parsing tokens).</remarks>
        internal void ConvertRawTrees()
        {
            CustomTrees.Clear();

            foreach (var rawTree in RawCustomTrees)
            {
                // create objects
                var tappedProductObject = new TapperTimedProduct(rawTree.Data.TappedProduct.DaysBetweenProduce, ResolveToken(rawTree.Data.TappedProduct.Product), rawTree.Data.TappedProduct.Amount);
                var barkProductObject = new TimedProduct(rawTree.Data.BarkProduct.DaysBetweenProduce, ResolveToken(rawTree.Data.BarkProduct.Product), rawTree.Data.BarkProduct.Amount);
                var shakingProductObjects = new List<SeasonalTimedProduct>();
                foreach (var shakingProduct in rawTree.Data.ShakingProducts)
                    shakingProductObjects.Add(new SeasonalTimedProduct(shakingProduct.DaysBetweenProduce, ResolveToken(shakingProduct.Product), shakingProduct.Amount, shakingProduct.Seasons));

                // add tree
                CustomTrees.Add(new CustomTree(rawTree.Id, rawTree.Data.Name, rawTree.Texture, tappedProductObject, ResolveToken(rawTree.Data.Wood), rawTree.Data.DropsSap, ResolveToken(rawTree.Data.Seed), rawTree.Data.RequiredToolLevel, shakingProductObjects, rawTree.Data.IncludeIfModIsPresent, rawTree.Data.ExcludeIfModIsPresent, barkProductObject, rawTree.Data.UnfertilisedGrowthChance, rawTree.Data.FertilisedGrowthChance));
            }
        }

        /// <summary>Gets the id of a tree type from a name.</summary>
        /// <param name="name">The name to get the id of.</param>
        /// <returns>The id of the tree name.</returns>
        internal int GetPersitantId(string name)
        {
            // multiplayer clients shouldn't try to access the id mapping file directly
            if (!Context.IsMainPlayer)
            {
                IdMap.TryGetValue(name.ToLower(), out var treeId);
                if (treeId == 0)
                    treeId = -1;
                return treeId;
            }

            IdMap = GetTreeTypesData();
            if (IdMap == null)
                IdMap = new Dictionary<string, int>();

            // check for the name in the saved types file
            if (IdMap.TryGetValue(name.ToLower(), out var id))
                return id;

            // the tree name wasn't present, generate a new id and save it to the tree types file
            var typeId = -1;
            for (int i = 20; typeId == -1; i++) // start i at 20 to accomodate for base game trees
            {
                // ensure the id isn't already used by a different tree
                if (IdMap.Any(typeMap => typeMap.Value == i))
                    continue;

                typeId = i;
            }
            IdMap.Add(name.ToLower(), typeId);
            this.Helper.Multiplayer.SendMessage(IdMap, "IdMap", new[] { this.ModManifest.UniqueID }); // update the client's id map

            SetTreeTypesData(IdMap);
            return typeId;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Invoked when the game is launched.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>Used so there is access to the SpaceCore API (you can't get an API instance from the <see cref="BarkingUpTheRightTree.ModEntry.Entry(IModHelper)"/> method).</remarks>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // register bark remover for serialisation
            var spaceCoreApi = this.Helper.ModRegistry.GetApi("spacechase0.SpaceCore");
            spaceCoreApi.GetType().GetMethod("RegisterSerializerType", BindingFlags.Public | BindingFlags.Instance).Invoke(spaceCoreApi, new[] { typeof(BarkRemover) });

            // load mod
            Instance = this;
            Api = new Api();
            BarkRemoverTexture = this.Helper.Content.Load<Texture2D>(Path.Combine("assets", "BarkRemover.png"));

            ApplyHarmonyPatches();
            LoadContentPacks();

            this.Helper.ConsoleCommands.Add("reset_custom_trees", "Resets the custom trees (using map tile data).", (string command, string[] args) => ResetCustomTreesCommand());

            this.Helper.Events.Multiplayer.PeerContextReceived += OnPeerContextReceived;
            this.Helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            this.Helper.Events.GameLoop.SaveCreating += OnSaveCreating;
            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            this.Helper.Events.GameLoop.Saving += OnSaving;
            this.Helper.Events.GameLoop.Saved += OnSaved;
            this.Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            this.Helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        /// <summary>Invoked when the a connection context is received.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The sender data.</param>
        /// <remarks>Used so connecting clients can get a copy of the id map.</remarks>
        private void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            // make sure the host sends the client the id map when they join
            if (Context.IsMainPlayer)
                this.Helper.Multiplayer.SendMessage(IdMap, "IdMap", new[] { this.ModManifest.UniqueID }, new[] { e.Peer.PlayerID });
        }

        /// <summary>Invoked when a mod message is received.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to get the tree id map from the game host.</remarks>
        private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != this.ModManifest.UniqueID)
                return;

            if (e.Type != "IdMap")
                return;

            IdMap = e.ReadAs<Dictionary<string, int>>();

            // reload content packs and reconvert using the new id map
            this.Monitor.Log("Reloading content packs to reflect host IdMap", LogLevel.Info);
            LoadContentPacks();
            ConvertRawTrees();
        }

        /// <summary>Invoked when the player is creating a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>Used to convert the custom trees to default trees for the initial save.</remarks>
        private void OnSaveCreating(object sender, SaveCreatingEventArgs e) => ConvertCustomTreesToDefaultTrees();

        /// <summary>Invoked when the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>Used to convert the raw custom trees and to convert the default saved trees to custom trees..</remarks>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            ConvertRawTrees();
            ConvertDefaultTreesToCustomTrees();
        }

        /// <summary>Invoked when the player saves the game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to change the tree ids so the save doesn't get ruined if the mod is uninstalled.</remarks>
        private void OnSaving(object sender, SavingEventArgs e) => ConvertCustomTreesToDefaultTrees();

        /// <summary>Invoked after the game was saved.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to convert from the default tree id to the custom tree id (if it was a custom tree before it was saved).</remarks>
        private void OnSaved(object sender, SavedEventArgs e) => ConvertDefaultTreesToCustomTrees();

        /// <summary>Invoked when the user returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to reset the id map (as it may be different from what's on the file system if they're returning from playing in a multiplayer session).</remarks>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            this.Monitor.Log("Reloading content packs", LogLevel.Info);
            LoadContentPacks();
            ConvertRawTrees();
        }

        /// <summary>Invoked when the <see cref="StardewValley.Game1.activeClickableMenu"/> changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>Used to add the <see cref="BarkingUpTheRightTree.Tools.BarkRemover"/> to Robin's shop.</remarks>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // ensure menu is a shop menu
            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is ShopMenu shopMenu))
                return;

            // ensure menu is Robin's
            if (shopMenu.portraitPerson.Name.ToLower() != "robin")
                return;

            // ensure there is atleast one custom tree that has bark to be harvested
            if (Api.GetAllTrees().All(tree => tree.BarkProduct.Product == -1))
            {
                this.Monitor.Log("BarkRemover wasn't added due to no trees with bark");
                return;
            }

            // add Bark Remover to the menu
            this.Monitor.Log("BarkRemover was added to shop inventory");
            var barkRemover = new BarkRemover();
            shopMenu.forSale.Add(barkRemover);
            shopMenu.itemPriceAndStock.Add(barkRemover, new[] { 1000, 1 });
        }

        /// <summary>Resets the custom tre using the map tile data.</summary>
        private void ResetCustomTreesCommand()
        {
            // ensure a save has been loaded
            if (!Context.IsWorldReady)
                return;

            // loop through each tile and look for tiles with tree properties
            foreach (var location in Game1.locations)
                for (int x = 0; x < location.Map.Layers[0].LayerWidth; x++)
                    for (int y = 0; y < location.Map.Layers[0].LayerHeight; y++)
                    {
                        // check if the tile has the "Tree" property
                        var treeName = location.doesTileHaveProperty(x, y, "Tree", "Back");
                        if (treeName == null)
                            continue;

                        // ensure tree has been loaded and get required data
                        if (!Api.GetRawTreeByName(treeName, out var treeId, out _, out _, out _, out _, out _, out _, out var shakingProducts, out _, out _, out _, out _, out _))
                        {
                            this.Monitor.Log($"No tree with the name: {treeName} could be found. (Will not be planted on map)", LogLevel.Warn);
                            continue;
                        }

                        // place tree
                        var tileLocation = new Vector2(x, y);
                        if (!location.terrainFeatures.ContainsKey(tileLocation) && !location.objects.ContainsKey(tileLocation))
                        {
                            var tree = new Tree(treeId, 5);
                            tree.modData[$"{this.ModManifest.UniqueID}/daysTillBarkHarvest"] = "0";
                            tree.modData[$"{this.ModManifest.UniqueID}/daysTillNextShakeProducts"] = JsonConvert.SerializeObject(new int[shakingProducts.Count]);
                            if (location.doesTileHaveProperty(x, y, "NonChoppable", "Back") != null)
                                tree.modData[$"{this.ModManifest.UniqueID}/nonChoppable"] = string.Empty; // the value is unused as only the presence of the key is checked to see if the tree is choppable
                            location.terrainFeatures.Add(tileLocation, tree);
                        }
                    }
        }

        /// <summary>Converts the custom trees to default trees.</summary>
        /// <remarks>This is so trees are saved as default trees which means if the player uninstalls the mod the save won't become unusable.</remarks>
        private void ConvertCustomTreesToDefaultTrees()
        {
            foreach (var location in Game1.locations)
                foreach (var terrainFeature in location.terrainFeatures.Values)
                {
                    if (!(terrainFeature is Tree tree) || tree.treeType < 20)
                        continue;

                    // save the current id and change the id to be a default tree (this is so if the mod is uninstalled it doesn't ruin the save)
                    tree.modData[$"{this.ModManifest.UniqueID}/treeId"] = tree.treeType.Value.ToString();
                    tree.treeType.Value = 1;

                    // decrement the number of days till next bark product
                    if (tree.modData.ContainsKey($"{this.ModManifest.UniqueID}/daysTillBarkHarvest"))
                    {
                        int.TryParse(tree.modData[$"{this.ModManifest.UniqueID}/daysTillBarkHarvest"], out var daysTillBarkHarvest);
                        tree.modData[$"{this.ModManifest.UniqueID}/daysTillBarkHarvest"] = Math.Max(0, daysTillBarkHarvest - 1).ToString();
                    }

                    // decrement the number of days for each shake product
                    if (tree.modData.ContainsKey($"{this.ModManifest.UniqueID}/daysTillNextShakeProducts"))
                    {
                        var daysTillNextShakeProducts = JsonConvert.DeserializeObject<int[]>(tree.modData[$"{this.ModManifest.UniqueID}/daysTillNextShakeProducts"]);
                        for (int i = 0; i < daysTillNextShakeProducts.Length; i++)
                            daysTillNextShakeProducts[i] = Math.Max(0, daysTillNextShakeProducts[i] - 1);
                        tree.modData[$"{this.ModManifest.UniqueID}/daysTillNextShakeProducts"] = JsonConvert.SerializeObject(daysTillNextShakeProducts);
                    }
                }
        }

        /// <summary>Converts the default trees to custom trees.</summary>
        /// <remarks>This is so trees that have been saved can be converted back to their custom versions (if they were custom before being being saved).</remarks>
        private void ConvertDefaultTreesToCustomTrees()
        {
            foreach (var location in Game1.locations)
                foreach (var terrainFeature in location.terrainFeatures.Values)
                {
                    if (!(terrainFeature is Tree tree))
                        continue;

                    if (tree.modData.ContainsKey($"{this.ModManifest.UniqueID}/treeId"))
                    {
                        int.TryParse(tree.modData[$"{this.ModManifest.UniqueID}/treeId"], out var customId);

                        // ensure a tree with this id exists
                        if (!Api.GetTreeById(customId, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _))
                        {
                            // get tree name from id mapping file
                            var name = "[Unknown]";
                            if (IdMap.ContainsValue(customId))
                                name = IdMap.FirstOrDefault(kvp => kvp.Value == customId).Key;

                            this.Monitor.Log($"A tree with the id of {customId} (name: {name}) was found in the save file but no tree with that id was loaded. Tree will be loaded as a default tree.", LogLevel.Warn);
                            continue;
                        }

                        tree.treeType.Value = customId;
                    }
                }
        }

        /// <summary>Applies the harmony patches.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching source code
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), "loadTexture"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.LoadTexturePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.dayUpdate)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.DayUpdatePrefix)))
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
                original: AccessTools.Method(typeof(Tree), nameof(Tree.tickUpdate)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.TickUpdateTranspile))),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.TickUpdatePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), "performTreeFall"),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.PerformTreeFallTranspile))),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.PerformTreeFallPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), "performBushDestroy"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.PerformBushDestroyPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), "performSproutDestroy"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.PerformSproutDestroyPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), "performSeedDestroy"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.PerformSeedDestroyPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.draw)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(TreePatch), nameof(TreePatch.DrawPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.loadObjects)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GameLocationPatch), nameof(GameLocationPatch.LoadObjectsPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(Debris), new[] { typeof(int), typeof(int), typeof(Vector2), typeof(Vector2), typeof(float) }),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(DebrisPatch), nameof(DebrisPatch.ConstructorTranspiler)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.createRadialDebris), new[] { typeof(GameLocation), typeof(int), typeof(int), typeof(int), typeof(int), typeof(bool), typeof(int), typeof(bool), typeof(int) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Game1Patch), nameof(Game1Patch.CreateRadialDebrisPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.createObjectDebris), new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(GameLocation) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Game1Patch), nameof(Game1Patch.CreateObjectDebrisPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.createObjectDebris), new[] { typeof(int), typeof(int), typeof(int), typeof(long), typeof(GameLocation) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Game1Patch), nameof(Game1Patch.CreateObjectDebrisPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isWildTreeSeed)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.IsWildTreeSeedPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.PlacementActionTranspile))),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.PlacementActionPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isPlaceable)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.IsPlaceablePostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.isPassable)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.IsPassablePostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.drawPlacementBounds)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.DrawPlacementBoundsPrefix)))
            );
        }

        /// <summary>Loads all the content packs.</summary>
        private void LoadContentPacks()
        {
            RawCustomTrees.Clear();

            foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
                try
                {
                    this.Monitor.Log($"Loading content pack: {contentPack.Manifest.Name}", LogLevel.Info);

                    // load each tree
                    var modDirectory = new DirectoryInfo(contentPack.DirectoryPath);
                    foreach (var treePath in modDirectory.EnumerateDirectories())
                    {
                        // check if required content pack files exist
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
                        }

                        var treeTexture = contentPack.LoadAsset<Texture2D>(Path.Combine(treePath.Name, "tree.png"));
                        var treeData = contentPack.ReadJsonFile<ParsedCustomTree>(Path.Combine(treePath.Name, "content.json"));

                        // create tuples
                        var tappedProduct = (treeData.TappedProduct?.DaysBetweenProduce ?? 0, treeData.TappedProduct?.Product, treeData.TappedProduct?.Amount ?? 0);
                        var barkProduct = (treeData.BarkProduct?.DaysBetweenProduce ?? 0, treeData.BarkProduct?.Product, treeData.BarkProduct?.Amount ?? 0);
                        var shakingProducts = new List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)>();
                        foreach (var shakingProduct in treeData.ShakingProducts)
                            shakingProducts.Add((shakingProduct.DaysBetweenProduce, shakingProduct.Product, shakingProduct.Amount, shakingProduct.Seasons));

                        Api.AddTree($"{contentPack.Manifest.UniqueID}.{treeData.Name}", treeTexture, tappedProduct, treeData.Wood, treeData.DropsSap, treeData.Seed, treeData.RequiredToolLevel, shakingProducts, treeData.IncludeIfModIsPresent, treeData.ExcludeIfModIsPresent, barkProduct, contentPack.Manifest.Name, treeData.UnfertilisedGrowthChance, treeData.FertilisedGrowthChance);
                    }
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Failed to load content pack: {ex}", LogLevel.Error);
                }
        }

        /// <summary>Converts a token into a numerical id.</summary>
        /// <param name="token">The token to convert.</param>
        /// <returns>A numerical id.</returns>
        private int ResolveToken(string token)
        {
            // ensure it's actually a token
            if (!token.Contains(":"))
            {
                // check the inputted value is a number
                if (int.TryParse(token, out int id))
                {
                    return id;
                }
                else
                {
                    this.Monitor.Log($"The value: '{token}' isn't a valid token and isn't a number");
                    return -1;
                }
            }

            // ensure there are enough sections of the token to be valid
            var splitToken = token.Split(':');
            if (splitToken.Length != 3)
            {
                this.Monitor.Log("Invalid number of arguments passed. Correct layout is: '[uniqueId]:[apiMethodName]:[valueToPass]'", LogLevel.Error);
                return -1;
            }

            var uniqueId = splitToken[0];
            var apiMethodName = splitToken[1];
            var valueToPass = splitToken[2];

            // ensure an api could be found with the unique id
            var api = this.Helper.ModRegistry.GetApi(uniqueId);
            if (api == null)
            {
                this.Monitor.Log($"No api could be found provided by: {uniqueId}", LogLevel.Error);
                return -1;
            }

            // ensure the api has the correct method
            var apiMethodInfo = api.GetType().GetMethod(apiMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (apiMethodInfo == null)
            {
                this.Monitor.Log($"No api method with the name: {apiMethodName} could be found for api provided by: {uniqueId}", LogLevel.Error);
                return -1;
            }

            // ensure the api returned a valid value
            if (!int.TryParse(apiMethodInfo.Invoke(api, new[] { valueToPass }).ToString(), out var apiResult) || apiResult == -1)
            {
                this.Monitor.Log($"No valid value was returned from method: {apiMethodName} in api provided by: {uniqueId} with a passed value of: {valueToPass}", LogLevel.Error);
                return -1;
            }

            return apiResult;
        }

        /// <summary>Gets the tree types data.</summary>
        /// <returns>The tree types data.</returns>
        private Dictionary<string, int> GetTreeTypesData()
        {
            // get the content of the treeTypes.json file
            var treeTypesFile = GetTreeTypesFile();
            var treeTypes = File.ReadAllText(treeTypesFile);

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, int>>(treeTypes);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"TreeTypes couldn't be deserialised. TreeTypes will be ignored, this could Id shifting (trees may be different from save). \nPath: {treeTypesFile}\n{ex}", LogLevel.Error);
                return null;
            }
        }

        /// <summary>Sets the tree types data.</summary>
        /// <param name="treeTypes">The tree type data to write save.</param>
        private void SetTreeTypesData(Dictionary<string, int> treeTypes)
        {
            var treeTypesFile = GetTreeTypesFile();
            File.WriteAllText(treeTypesFile, JsonConvert.SerializeObject(treeTypes));
        }

        /// <summary>Gets the tree types data file path.</summary>
        /// <returns>The tree types data file path.</returns>
        private string GetTreeTypesFile()
        {
            // get / create the directory containing the treeTypes file
            var treeTypesFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", "_BURT");
            if (!Directory.Exists(treeTypesFileDirectory))
                Directory.CreateDirectory(treeTypesFileDirectory);

            // get / create the treeTypes file
            var treeTypesFilePath = Path.Combine(treeTypesFileDirectory, "treeTypes.json");
            if (!File.Exists(treeTypesFilePath))
                File.Create(treeTypesFilePath).Close();

            return treeTypesFilePath;
        }
    }
}
