using FarmAnimalVarietyRedux.Models;
using FarmAnimalVarietyRedux.Patches;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FarmAnimalVarietyRedux
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Provides methods for interacting with the mod directory.</summary>
        public static IModHelper ModHelper { get; private set; }

        /// <summary>Provides methods for logging to the console.</summary>
        public static IMonitor ModMonitor { get; private set; }

        /// <summary>A list of all the animals.</summary>
        public List<Animal> Animals { get; private set; } = new List<Animal>();

        /// <summary>The custom animal data for the custom farm animals.</summary>
        public Dictionary<string, string> DataStrings { get; private set; } = new Dictionary<string, string>();

        /// <summary>Whether the content packs have been loaded.</summary>
        public bool ContentPacksLoaded { get; set; }

        /// <summary>The singleton instance of the <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; set; }

        /// <summary>Provides basic FAVR apis.</summary>
        public IApi Api { get; private set; }


        /*********
        ** Public Methods 
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = this.Helper;
            ModMonitor = this.Monitor;
            Instance = this;
            Api = new Api();

            ApplyHarmonyPatches();

            // add the default animals to the data strings
            LoadDefaultAnimals();
        }

        /// <summary>Expose the Api to other mods.</summary>
        /// <returns>An instance of the <see cref="Api"/>.</returns>
        public override object GetApi() => new Api();

        /// <summary>This will call when loading each asset, if the mail asset is being loaded, return true as we want to edit this.</summary>
        /// <typeparam name="T">The type of the assets being loaded.</typeparam>
        /// <param name="asset">The asset info being loaded.</param>
        /// <returns>True if the assets being loaded needs to be edited.</returns>
        public bool CanEdit<T>(IAssetInfo asset) => asset.AssetNameEquals("Data\\FarmAnimals");

        /// <summary>Edit the farm animals asset to add the the custom data strings.</summary>
        /// <typeparam name="T">The type of the assets being loaded.</typeparam>
        /// <param name="asset">The asset data being loaded.</param>
        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            data = DataStrings;
        }

        /// <summary>Load all the sprites for the new animals from the loaded content packs.</summary>
        public void LoadContentPacks()
        {
            // loading Json Assets early is only required on connected clients - Game1.IsClient can't be used as that doesn't get set yet - this should have no effect on host
            LoadJAEarly();

            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                Monitor.Log($"Loading {contentPack.Manifest.Name}");

                // loop through each animal folder in the current content pack
                foreach (var animalFolder in Directory.GetDirectories(contentPack.DirectoryPath))
                {
                    var animalFolderSplit = animalFolder.Split(Path.DirectorySeparatorChar);
                    var animalName = animalFolderSplit[animalFolderSplit.Length - 1];

                    // ensure content.json file exists
                    if (!File.Exists(Path.Combine(animalFolder, "content.json")))
                    {
                        this.Monitor.Log($"Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} doesn't contain a content.json file.", LogLevel.Error);
                        continue;
                    }

                    // serialize and validate content.json file
                    AnimalData animalData = contentPack.LoadAsset<AnimalData>(Path.Combine(animalName, "content.json"));
                    if (!animalData.IsValid(animalName))
                    {
                        this.Monitor.Log($"Content.json is not valid for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName}", LogLevel.Error);
                        continue;
                    }

                    // loop through each sub type and add them (such as colour varients etc)
                    List<AnimalSubType> animalSubTypes = new List<AnimalSubType>();
                    foreach (var type in animalData.Types)
                    {
                        // get sprites
                        var sprites = new AnimalSprites(
                            adultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"{type.Name}.png"), contentPack),
                            babySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"Baby {type.Name}.png"), contentPack),
                            harvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"Harvested {type.Name}.png"), contentPack),
                            springAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"{type.Name}.png"), contentPack),
                            springHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"Harvested {type.Name}.png"), contentPack),
                            springBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"Baby {type.Name}.png"), contentPack),
                            summerAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"{type.Name}.png"), contentPack),
                            summerHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"Harvested {type.Name}.png"), contentPack),
                            summerBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"Baby {type.Name}.png"), contentPack),
                            fallAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"{type.Name}.png"), contentPack),
                            fallHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"Harvested {type.Name}.png"), contentPack),
                            fallBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"Baby {type.Name}.png"), contentPack),
                            winterAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"{type.Name}.png"), contentPack),
                            winterHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"Harvested {type.Name}.png"), contentPack),
                            winterBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"Baby {type.Name}.png"), contentPack)
                        );

                        // ensure sprites are valid
                        if (!sprites.IsValid())
                        {
                            this.Monitor.Log($"Sprites are not valid for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} >> Subtype: {type}", LogLevel.Error);
                            continue;
                        }
                        type.Sprites = sprites;

                        // resolve API tokens in data and validate it
                        type.ResolveTokens();
                        if (!type.IsValid())
                        {
                            this.Monitor.Log($"Data is not valid for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} >> Subtype: {type.Name}", LogLevel.Error);
                            continue;
                        }

                        animalSubTypes.Add(type);
                    }

                    // ensure there were valid sub types
                    if (animalSubTypes.Count == 0)
                    {
                        this.Monitor.Log($"No valid sub types for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName}.\n Animal will not be added.", LogLevel.Error);
                        continue;
                    }

                    // create, validate, and add the animal
                    var animal = new Animal(
                        name: animalName,
                        data: animalData,
                        shopIcon: GetSpriteByPath(Path.Combine(animalName, $"shopdisplay.png"), contentPack),
                        subTypes: animalSubTypes
                    );

                    if (!animal.IsValid())
                    {
                        this.Monitor.Log($"Animal is not valid at Content pack: {contentPack.Manifest.Name} >> Animal: {animalName}", LogLevel.Error);
                        continue;
                    }

                    Animals.Add(animal);

                    // construct data string for game to use
                    foreach (var subType in animal.SubTypes)
                        DataStrings.Add(subType.Name, $"{animal.Data.DaysToProduce}/{animal.Data.DaysTillMature}///{animal.Data.SoundId}//////////{subType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{animal.Data.FrontAndBackSpriteWidth}/{animal.Data.FrontAndBackSpriteHeight}/{animal.Data.SideSpriteWidth}/{animal.Data.SideSpriteHeight}/{animal.Data.FullnessDrain}/{animal.Data.HappinessDrain}//0/{animal.Data.AnimalShopInfo?.BuyPrice}/{subType.Name}/");
                }
            }

            // print all added farm animals to trace
            PrintAnimalData();

            // invalidate farm animal cache to add the new data strings to it
            this.Helper.Content.InvalidateCache("Data/FarmAnimals");
        }


        /*********
        ** Private Methods 
        *********/
        /// <summary>Apply the harmony patches for patching game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.FarmAnimal), new Type[] { typeof(string), typeof(long), typeof(long) }),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ConstructorTranspile)))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.FarmAnimal), new Type[] { typeof(string), typeof(long), typeof(long) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ConstructorPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.FarmAnimal), nameof(FarmAnimal.reloadData)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ReloadDataPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.FarmAnimal), nameof(FarmAnimal.updateWhenNotCurrentLocation)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.UpdateWhenNotCurrentLocationPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.FarmAnimal), "behaviors"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.BehaviorsPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.FarmAnimal), "findTruffle"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.FindTrufflePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.Menus.PurchaseAnimalsMenu), new Type[] { typeof(List<StardewValley.Object>) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.ConstructorPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.getAnimalDescription)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.GetAnimalDescriptionPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveKeyPress)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.ReceiveKeyPressPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveGamePadButton)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.ReceiveGamePadButtonPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.performHoverAction)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.PerformHoverActionPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveLeftClick)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.ReceiveLeftClickPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.draw), new Type[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.DrawPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Utility), nameof(Utility.getPurchaseAnimalStock)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(UtilityPatch), nameof(UtilityPatch.GetPurchaseAnimalStockPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.AnimatedSprite), "loadTexture"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(AnimatedSpritePatch), nameof(AnimatedSpritePatch.LoadTexturePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.AnimalQueryMenu), nameof(AnimalQueryMenu.draw), new Type[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(AnimalQueryMenuPatch), nameof(AnimalQueryMenuPatch.DrawPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.AnimalQueryMenu), nameof(AnimalQueryMenu.performHoverAction)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(AnimalQueryMenuPatch), nameof(AnimalQueryMenuPatch.PerformHoverActionPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.AnimalQueryMenu), nameof(AnimalQueryMenu.receiveLeftClick)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(AnimalQueryMenuPatch), nameof(AnimalQueryMenuPatch.ReceiveLeftClickPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.DayUpdatePrefix)))
            );
        }

        /// <summary>Get the sprite at the passed relative path.</summary>
        /// <param name="path">The relative (to the content pack) path for the sprite.</param>
        /// <param name="contentPack">The content pack that should contain the sprite.</param>
        /// <returns>The sprite at the passed path.</returns>
        private Texture2D GetSpriteByPath(string path, IContentPack contentPack)
        {
            if (File.Exists(Path.Combine(contentPack.DirectoryPath, path)))
            {
                return contentPack.LoadAsset<Texture2D>(path);
            }

            return null;
        }

        /// <summary>Run the initialisation code for Json Assets early. This is required for connected multiplayer clients as Farm Animals get loaded before JA loads assets for clients.</summary>
        private void LoadJAEarly()
        {
            if (!this.Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
                return;

            this.Monitor.Log("Initialising JA early");

            var jaModData = this.Helper.ModRegistry.Get("spacechase0.JsonAssets");
            var jaInstance = (Mod)jaModData.GetType().GetProperty("Mod", BindingFlags.Public | BindingFlags.Instance).GetValue(jaModData);
            jaInstance.GetType().GetMethod("initStuff", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(jaInstance, new object[] { false });
        }

        /// <summary>Print all the custom animal data.</summary>
        private void PrintAnimalData()
        {
            // print data string
            foreach (var animalDataString in DataStrings)
                this.Monitor.Log($"{animalDataString.Key}: {animalDataString.Value}");

            // print animal objects
            foreach (var animal in Animals)
            {
                this.Monitor.Log($"ANIMALDATA FOR: {animal.Name}\nBuyable: {animal.Data.Buyable}\tDaysToProduce: {animal.Data.DaysToProduce}\tDaysTillMature: {animal.Data.DaysTillMature}\tSoundId: {animal.Data.SoundId}\tFrontAndBackSpriteWidth: {animal.Data.FrontAndBackSpriteWidth}\tFrontAndBackSpriteHeight: {animal.Data.FrontAndBackSpriteHeight}\tSideSpriteWidth: {animal.Data.SideSpriteWidth}\tSideSpriteHeight: {animal.Data.SideSpriteHeight}\tFullnessDrain: {animal.Data.FullnessDrain}\tHappinessDrain: {animal.Data.HappinessDrain}\tWalkSpeed: {animal.Data.WalkSpeed}\tBedTime: {animal.Data.BedTime}\tBuildings: [{string.Join(",", animal.Data.Buildings ?? default)}]\tSeasonsAllowedOutdoors: [{string.Join(",", animal.Data.SeasonsAllowedOutdoors ?? default)}]\tShopDescription: {animal.Data.AnimalShopInfo?.Description}\tShopBuyPrice: {animal.Data.AnimalShopInfo?.BuyPrice}");
                this.Monitor.Log($"SUBTYPES FOR: {animal.Name}");
                foreach (var subType in animal.SubTypes)
                {
                    this.Monitor.Log($"Name: {subType.Name}");
                    PrintAnimalProduce(subType.Produce);
                }
            }
        }

        /// <summary>Print the passed <see cref="AnimalProduce"/>.</summary>
        /// <param name="animalProduce">The <see cref="AnimalProduce"/> to print.</param>
        private void PrintAnimalProduce(AnimalProduce animalProduce)
        {
            if (animalProduce.AllSeasons != null)
                PrintAnimalProduceSeason(animalProduce.AllSeasons);
            if (animalProduce.Spring != null)
                PrintAnimalProduceSeason(animalProduce.Spring);
            if (animalProduce.Summer != null)
                PrintAnimalProduceSeason(animalProduce.Summer);
            if (animalProduce.Fall != null)
                PrintAnimalProduceSeason(animalProduce.Fall);
            if (animalProduce.Winter != null)
                PrintAnimalProduceSeason(animalProduce.Winter);
        }

        /// <summary>Print the passed <see cref="AnimalProduceSeason"/>.</summary>
        /// <param name="animalProduceSeason">The <see cref="AnimalProduceSeason"/> to print.</param>
        private void PrintAnimalProduceSeason(AnimalProduceSeason animalProduceSeason)
        {
            if (animalProduceSeason.Products != null)
            {
                this.Monitor.Log("Products:");
                foreach (var product in animalProduceSeason.Products)
                    this.Monitor.Log($"Id: {product?.Id}\tHarvestType: {product?.HarvestType}\tToolName: {product?.ToolName}");
            }

            if (animalProduceSeason.DeluxeProducts != null)
            {
                this.Monitor.Log("DeluxeProducts:");
                foreach (var deluxeProduct in animalProduceSeason.DeluxeProducts)
                    this.Monitor.Log($"Id: {deluxeProduct?.Id}\tHarvestType: {deluxeProduct?.HarvestType}\tToolName: {deluxeProduct?.ToolName}");
            }
        }

        /// <summary>Load all the default animals into the datastrings. This is so content packs can also edit default animals.</summary>
        private void LoadDefaultAnimals()
        {
            // TODO: chicken

            // TODO: duck

            // TODO: rabbit

            // TODO: dinosaur

            // TODO: cow

            // TODO: goat

            // TODO: pig

            // TODO: sheep
        }
    }
}
