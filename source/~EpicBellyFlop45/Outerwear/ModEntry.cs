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
using Outerwear.Models;
using Outerwear.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Outerwear
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The currently equipped outerwear.</summary>
        public static Item EquippedOuterwear { get; set; }

        /// <summary>The sprite that is drawn in the outerwear slot when no outerwear item is in it.</summary>
        public static Texture2D OuterwearSlotPlaceholder { get; private set; }

        /// <summary>All the metadata of all loaded outerwear items.</summary>
        public static List<OuterwearData> OuterwearData { get; set; } = new List<OuterwearData>();

        /// <summary>Provides methods for logging to the console.</summary>
        public static IMonitor ModMonitor { get; private set; }

        /// <summary>Provides basic outerwear apis.</summary>
        public static IApi Api { get; private set; }


        /*********
        ** Public Methods 
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            Api = new Api();
            OuterwearSlotPlaceholder = this.Helper.Content.Load<Texture2D>(Path.Combine("assets", "OuterwearPlaceholder.png"));
            ModMonitor = this.Monitor;

            ApplyHarmonyPatches();

            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            this.Helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        /// <summary>Expose the API to other mods.</summary>
        /// <returns>An instance of the api.</returns>
        public override object GetApi()
        {
            return new Api();
        }


        /*********
        ** Private Methods 
        *********/
        /****
        ** Event Handlers
        ****/
        /// <summary>Invoked when the player has loaded a save. Used for loading the content packs.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            LoadContentPacks();
        }

        /// <summary>Invoked when the active menu changed. Used for injecting outerwear objects into NPCs buy menus.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu == null || !(e.NewMenu is ShopMenu))
                return;

            var shopMenu = e.NewMenu as ShopMenu;
            foreach (var outerwearData in ModEntry.OuterwearData)
            {
                // sort out buying outerwear
                if (outerwearData.BuyFrom != null && outerwearData.BuyPrice > 0)
                {
                    // ensure npc sells the outerwear
                    if (shopMenu.portraitPerson?.Name.ToLower() == outerwearData.BuyFrom.ToLower())
                    {
                        var outerwear = new Models.Outerwear(outerwearData);
                        shopMenu.forSale.Add(outerwear);
                        shopMenu.itemPriceAndStock.Add(outerwear, new int[] { outerwear.salePrice(), int.MaxValue });
                    }
                }
            }
        }

        /****
        ** Methods
        ****/
        /// <summary>Apply the harmony patches for patching game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.Menus.InventoryPage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.ConstructorPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.InventoryPage), nameof(InventoryPage.receiveLeftClick)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.ReceiveLeftClickPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.InventoryPage), nameof(InventoryPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.DrawPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.FarmerRenderer), nameof(FarmerRenderer.draw), new Type[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FarmerRendererPatch), nameof(FarmerRendererPatch.DrawPostFix)))
            );
        }

        /// <summary>Load all the content packs for this mod.</summary>
        private void LoadContentPacks()
        {
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                Monitor.Log($"Loading {contentPack.Manifest.Name}");

                foreach (var contentPackOuterwearPath in Directory.GetDirectories(contentPack.DirectoryPath))
                {
                    // get folder name for this outerwear item
                    var pathSplit = contentPackOuterwearPath.Split(Path.DirectorySeparatorChar);
                    var outerwearFolderName = pathSplit[pathSplit.Length - 1];

                    // verify the files exist
                    if (!VerifyContentPackFilesExist(contentPack, outerwearFolderName))
                        continue;

                    // get files
                    var outerwearContent = contentPack.ReadJsonFile<ContentPackContent>(Path.Combine(outerwearFolderName, "content.json"));
                    var outerwearMenuIcon = contentPack.LoadAsset<Texture2D>(Path.Combine(outerwearFolderName, "menuicon.png"));
                    var outerwearEquippedTexture = contentPack.LoadAsset<Texture2D>(Path.Combine(outerwearFolderName, "equippedtexture.png"));

                    // ensure an item with the same name hasn't already been added
                    if (ModEntry.OuterwearData.Where(data => data.DisplayName.ToLower() == outerwearContent.DisplayName.ToLower()).Any())
                    {
                        this.Monitor.Log($"An item with the name \"{outerwearContent.DisplayName}\" has already been added.");
                        continue;
                    }

                    // add outerwear to collection
                    var outerwearData = new OuterwearData(
                        id: Api.CreateId(outerwearContent.DisplayName),
                        displayName: outerwearContent.DisplayName,
                        description: outerwearContent.Description,
                        buyFrom: outerwearContent.BuyFrom,
                        buyPrice: outerwearContent.BuyPrice,
                        menuIcon: outerwearMenuIcon,
                        equippedTexture: outerwearEquippedTexture
                    );

                    ModEntry.OuterwearData.Add(outerwearData);
                }
            }

            // log all added items
            this.Monitor.Log("All added overwear items: ");
            foreach (var data in ModEntry.OuterwearData)
            {
                this.Monitor.Log($"Id: {data.Id}, DisplayName: {data.DisplayName}, Description: {data.Description}, BuyFrom: {data.BuyFrom}, BuyPrice: {data.BuyPrice}, SellTo: {data.SellTo}");
            }
        }

        /// <summary>Verify the given folder contains all the required files to be loaded.</summary>
        /// <param name="contentPack">The content pack that contains the outerwear to check.</param>
        /// <param name="outerwearFolderName">The outerwear folder to check contains the required files.</param>
        /// <returns>Whether the outerwear folder contains all the required files.</returns>
        private bool VerifyContentPackFilesExist(IContentPack contentPack, string outerwearFolderName)
        {
            bool allFilesExist = true;

            if (!contentPack.HasFile(Path.Combine(outerwearFolderName, "content.json")))
            {
                this.Monitor.Log($"'content.json' file couldn't be found in content pack: {contentPack.Manifest.Name}", LogLevel.Error);
                allFilesExist = false;
            }

            if (!contentPack.HasFile(Path.Combine(outerwearFolderName, "menuicon.png")))
            {
                this.Monitor.Log($"'menuicon.png' couldn't be found in content pack: {contentPack.Manifest.Name}", LogLevel.Error);
                allFilesExist = false;
            }

            if (!contentPack.HasFile(Path.Combine(outerwearFolderName, "equippedtexture.png")))
            {
                this.Monitor.Log($"'equippedtexture.png' couldn't be found in content pack: {contentPack.Manifest.Name}", LogLevel.Error);
                allFilesExist = false;
            }

            return allFilesExist;
        }
    }
}
