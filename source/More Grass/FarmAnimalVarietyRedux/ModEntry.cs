using FarmAnimalVarietyRedux.Enums;
using FarmAnimalVarietyRedux.Models;
using FarmAnimalVarietyRedux.Patches;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public bool CanEdit<T>(IAssetInfo asset) => asset.AssetNameEquals(Path.Combine("Data", "FarmAnimals"));

        /// <summary>Edit the farm animals asset to add the the custom data strings.</summary>
        /// <typeparam name="T">The type of the assets being loaded.</typeparam>
        /// <param name="asset">The asset data being loaded.</param>
        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;

            foreach (var dataString in DataStrings)
                data[dataString.Key] = dataString.Value;
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

                    // check if a sound file exists
                    var soundPath = Path.Combine(animalFolder, "sound.wav");
                    if (File.Exists(soundPath))
                    {
                        using (var stream = File.OpenRead(soundPath))
                            animalData.SoundEffect = SoundEffect.FromStream(stream);
                    }

                    if (animalData.UpdatePreviousAnimal)
                    {
                        if (ModEntry.Instance.Api.GetAnimalByName(animalName) == null)
                        {
                            ModEntry.Instance.Monitor.Log($"Trying to update animal: {animalName} but the aniaml hasn't been added. Animal entry will be ignored.");
                            continue;
                        }

                        UpdatePreviousAnimalEntry(animalData, contentPack, animalName);
                    }
                    else
                        AddNewAnimalEntry(animalData, contentPack, animalName);
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
        /// <summary>Edits a previously added animal from a content pack.</summary>
        /// <param name="animalData">The data of the animal.</param>
        /// <param name="contentPack">The content pack of the animal.</param>
        /// <param name="animalName">The name of the animal.</param>
        private void UpdatePreviousAnimalEntry(AnimalData animalData, IContentPack contentPack, string animalName)
        {
            // ensure animal data exists to edit
            var previousAnimalData = Api.GetAnimalByName(animalName);
            if (previousAnimalData == null)
            {
                this.Monitor.Log("Tried to update an animal that doesn't exist. Skipping content pack", LogLevel.Error);
                return;
            }

            // edit animal data
            previousAnimalData.Data.Buyable = animalData.Buyable;
            previousAnimalData.Data.AnimalShopInfo.BuyPrice = animalData.AnimalShopInfo.BuyPrice;
            previousAnimalData.Data.AnimalShopInfo.Description = animalData.AnimalShopInfo.Description;
            previousAnimalData.Data.DaysToProduce = animalData.DaysToProduce;
            previousAnimalData.Data.DaysTillMature = animalData.DaysTillMature;
            previousAnimalData.Data.SoundId = animalData.SoundId;
            previousAnimalData.Data.FrontAndBackSpriteWidth = animalData.FrontAndBackSpriteWidth;
            previousAnimalData.Data.FrontAndBackSpriteHeight = animalData.FrontAndBackSpriteHeight;
            previousAnimalData.Data.SideSpriteWidth = animalData.SideSpriteWidth;
            previousAnimalData.Data.SideSpriteHeight = animalData.SideSpriteHeight;
            previousAnimalData.Data.FullnessDrain = animalData.FullnessDrain;
            previousAnimalData.Data.HappinessDrain = animalData.HappinessDrain;
            previousAnimalData.Data.Buildings = animalData.Buildings;
            previousAnimalData.Data.WalkSpeed = animalData.WalkSpeed;
            previousAnimalData.Data.BedTime = animalData.BedTime;
            previousAnimalData.Data.SeasonsAllowedOutdoors = animalData.SeasonsAllowedOutdoors;

            // update sub types data
            foreach (var type in animalData.Types)
            {
                // check if the type has already been added (and should be edited instead)
                var animalSubType = ModEntry.Instance.Api.GetAnimalSubTypeByName(type.Name);
                if (animalSubType == null) // sub type should be added to animal
                {
                    var animal = ModEntry.Instance.Api.GetAnimalByName(animalName);
                    if (LoadAnimalSubType(type, contentPack, animalName))
                        animal.Data.Types.Add(type);
                }
                else // previous sub type should be edited
                {
                    animalSubType.Produce = type.Produce;
                }
            }
        }

        /// <summary>Loads a new animal from a content pack.</summary>
        private void AddNewAnimalEntry(AnimalData animalData, IContentPack contentPack, string animalName)
        {
            // set the shop display icon
            if (animalData.AnimalShopInfo != null)
                animalData.AnimalShopInfo.ShopIcon = GetSpriteByPath(Path.Combine(animalName, $"shopdisplay.png"), contentPack);

            // loop through each sub type to get sprites, resolve tokens, and validate
            var validTypes = new List<AnimalSubType>();
            foreach (var type in animalData.Types)
                if (LoadAnimalSubType(type, contentPack, animalName))
                    validTypes.Add(type);

            // set the data types to be the valid types
            animalData.Types = validTypes;

            // ensure there were valid sub types
            if (animalData.Types.Count == 0)
            {
                this.Monitor.Log($"No valid sub types for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName}.\n Animal will not be added.", LogLevel.Error);
                return;
            }

            // create, validate, and add the animal
            var animal = new Animal(
                name: animalName,
                data: animalData
            );

            if (!animal.IsValid())
            {
                this.Monitor.Log($"Animal is not valid at Content pack: {contentPack.Manifest.Name} >> Animal: {animalName}", LogLevel.Error);
                return;
            }

            Animals.Add(animal);

            // construct data string for game to use
            foreach (var subType in animal.Data.Types)
                DataStrings.Add(subType.Name, $"{animal.Data.DaysToProduce}/{animal.Data.DaysTillMature}///{animal.Data.SoundId}//////////{subType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{animal.Data.FrontAndBackSpriteWidth}/{animal.Data.FrontAndBackSpriteHeight}/{animal.Data.SideSpriteWidth}/{animal.Data.SideSpriteHeight}/{animal.Data.FullnessDrain}/{animal.Data.HappinessDrain}//0/{animal.Data.AnimalShopInfo?.BuyPrice}/{subType.Name}/");
        }

        /// <summary>Loads an animal sub type's assets and validates.</summary>
        /// <param name="subType">The sub type to load assets for and to validate.</param>
        /// <returns>Whether the sub type was valid.</returns>
        private bool LoadAnimalSubType(AnimalSubType subType, IContentPack contentPack, string animalName)
        {
            // get sprites
            var sprites = new AnimalSprites(
                adultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"{subType.Name}.png"), contentPack),
                babySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"Baby {subType.Name}.png"), contentPack),
                harvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"Harvested {subType.Name}.png"), contentPack),
                springAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"{subType.Name}.png"), contentPack),
                springHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"Harvested {subType.Name}.png"), contentPack),
                springBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"Baby {subType.Name}.png"), contentPack),
                summerAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"{subType.Name}.png"), contentPack),
                summerHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"Harvested {subType.Name}.png"), contentPack),
                summerBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"Baby {subType.Name}.png"), contentPack),
                fallAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"{subType.Name}.png"), contentPack),
                fallHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"Harvested {subType.Name}.png"), contentPack),
                fallBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"Baby {subType.Name}.png"), contentPack),
                winterAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"{subType.Name}.png"), contentPack),
                winterHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"Harvested {subType.Name}.png"), contentPack),
                winterBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"Baby {subType.Name}.png"), contentPack)
            );

            // ensure sprites are valid
            if (!sprites.IsValid())
            {
                this.Monitor.Log($"Sprites are not valid for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} >> Subtype: {subType.Name}", LogLevel.Error);
                return false;
            }
            subType.Sprites = sprites;

            // resolve API tokens in data and validate it
            subType.ResolveTokens();
            if (!subType.IsValid())
            {
                this.Monitor.Log($"Data is not valid for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} >> Subtype: {subType.Name}", LogLevel.Error);
                return false;
            }

            return true;
        }
        
        /// <summary>Apply the harmony patches for patching game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.FarmAnimal), new Type[] { typeof(string), typeof(long), typeof(long) }),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.ConstructorTranspile))),
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
                original: AccessTools.Method(typeof(StardewValley.FarmAnimal), nameof(FarmAnimal.makeSound)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.MakeSoundPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.FarmAnimal), nameof(FarmAnimal.dayUpdate)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmAnimalPatch), nameof(FarmAnimalPatch.DayUpdatePrefix)))
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
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveScrollWheelAction)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.ReceiveScrollWheelActionPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveKeyPress)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.ReceiveKeyPressPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.gamePadButtonHeld)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.GamePadButtonHeldPrefix)))
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
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.setUpForAnimalPlacement)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.SetUpForAnimalPlacementPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.setUpForReturnToShopMenu)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.SetUpForReturnToShopMenuPrefix)))
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
                original: AccessTools.Constructor(typeof(StardewValley.Menus.AnimalQueryMenu), new Type[] { typeof(FarmAnimal) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(AnimalQueryMenuPatch), nameof(AnimalQueryMenuPatch.ConstructorPostFix)))
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
        }

        /// <summary>Get the sprite at the passed relative path.</summary>
        /// <param name="path">The relative (to the content pack) path for the sprite.</param>
        /// <param name="contentPack">The content pack that should contain the sprite.</param>
        /// <returns>The sprite at the passed path.</returns>
        private Texture2D GetSpriteByPath(string path, IContentPack contentPack)
        {
            if (File.Exists(Path.Combine(contentPack.DirectoryPath, path)))
                return contentPack.LoadAsset<Texture2D>(path);

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
                this.Monitor.Log($"ANIMALDATA FOR: {animal.Name}\nBuyable: {animal.Data.Buyable}\tDaysToProduce: {animal.Data.DaysToProduce}\tDaysTillMature: {animal.Data.DaysTillMature}\tSoundId: {animal.Data.SoundId}\tCustomSoundEffectLoaded: {animal.Data.SoundEffect != null}\tFrontAndBackSpriteWidth: {animal.Data.FrontAndBackSpriteWidth}\tFrontAndBackSpriteHeight: {animal.Data.FrontAndBackSpriteHeight}\tSideSpriteWidth: {animal.Data.SideSpriteWidth}\tSideSpriteHeight: {animal.Data.SideSpriteHeight}\tFullnessDrain: {animal.Data.FullnessDrain}\tHappinessDrain: {animal.Data.HappinessDrain}\tWalkSpeed: {animal.Data.WalkSpeed}\tBedTime: {animal.Data.BedTime}\tBuildings: [{string.Join(",", animal.Data.Buildings ?? default)}]\tSeasonsAllowedOutdoors: [{string.Join(",", animal.Data.SeasonsAllowedOutdoors ?? default)}]\tShopDescription: {animal.Data.AnimalShopInfo?.Description}\tShopBuyPrice: {animal.Data.AnimalShopInfo?.BuyPrice}");
                this.Monitor.Log($"SUBTYPES FOR: {animal.Name}");
                foreach (var subType in animal.Data.Types)
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
                    this.Monitor.Log($"Id: {product?.Id}\tHarvestType: {product?.HarvestType}\tToolName: {product?.ToolName}\tHeartsRequired: {product?.HeartsRequired}");
            }

            if (animalProduceSeason.DeluxeProducts != null)
            {
                this.Monitor.Log("DeluxeProducts:");
                foreach (var deluxeProduct in animalProduceSeason.DeluxeProducts)
                    this.Monitor.Log($"Id: {deluxeProduct?.Id}\tHarvestType: {deluxeProduct?.HarvestType}\tToolName: {deluxeProduct?.ToolName}\tHeartsRequired: {deluxeProduct?.HeartsRequired}");
            }
        }

        /// <summary>Load all the default animals into the datastrings. This is so content packs can also edit default animals.</summary>
        private void LoadDefaultAnimals()
        {
            //
            // Chicken
            //
            // create chicken produce objects for white/blue and brown chickens
            var chickenProduce = new AnimalProduce(new AnimalProduceSeason(
                products: new List<AnimalProduct> { new AnimalProduct("176", HarvestType.Lay, null) },
                deluxeProducts: new List<AnimalProduct> { new AnimalProduct("174", HarvestType.Lay, null) }));

            var brownChickenProduce = new AnimalProduce(new AnimalProduceSeason(
                products: new List<AnimalProduct> { new AnimalProduct("180", HarvestType.Lay, null) },
                deluxeProducts: new List<AnimalProduct> { new AnimalProduct("182", HarvestType.Lay, null) }));

            // create animal sprites objects for the 3 chicken types
            var whiteChickenSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "White Chicken")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "BabyWhite Chicken")));
            var brownChickenSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "Brown Chicken")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "BabyBrown Chicken")));
            var blueChickenSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "Blue Chicken")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "BabyBlue Chicken")));

            // create chick sub types (checking for the event for the blue chicken)
            var chickenTypes = new List<AnimalSubType>();
            chickenTypes.Add(new AnimalSubType("White Chicken", chickenProduce, whiteChickenSprites));
            chickenTypes.Add(new AnimalSubType("Brown Chicken", brownChickenProduce, brownChickenSprites));

            // TODO: Game1.player is null at this point
            //if (Game1.player.eventsSeen.Contains(3900074)) // add the ability to get blue chicken if the player has seen the shane event for it
            //    chickenTypes.Add(new AnimalSubType("Blue Chicken", chickenProduce, blueChickenSprites));

            // create and add the chicken object
            var chickenShopInfo = new AnimalShopInfo(
                description: Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11334")) + Environment.NewLine + Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335")),
                buyPrice: 800,
                shopIcon: GetSubTexture(Game1.content.Load<Texture2D>(Path.Combine("LooseSprites", "Cursors")), new Rectangle(0, 448, 32, 16)));
            var chickenData = new AnimalData("Chicken", true, false, chickenShopInfo, chickenTypes, 1, 3, "cluck", 16, 16, 16, 16, 4, 7, new List<string> { "Coop", "Big Coop", "Deluxe Coop" }, 2, 1900, new List<Season> { Season.Spring, Season.Summer, Season.Fall });
            Animals.Add(new Animal("Chicken", chickenData));

            // construct data string for game to use
            foreach (var chickenSubType in chickenData.Types)
                DataStrings.Add(chickenSubType.Name, $"{chickenData.DaysToProduce}/{chickenData.DaysTillMature}///{chickenData.SoundId}//////////{chickenSubType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{chickenData.FrontAndBackSpriteWidth}/{chickenData.FrontAndBackSpriteHeight}/{chickenData.SideSpriteWidth}/{chickenData.SideSpriteHeight}/{chickenData.FullnessDrain}/{chickenData.HappinessDrain}//0/{chickenData.AnimalShopInfo?.BuyPrice}/{chickenSubType.Name}/");

            //
            // Duck
            //
            // create duck produce object
            var duckProduce = new AnimalProduce(new AnimalProduceSeason(
                products: new List<AnimalProduct> { new AnimalProduct("442", HarvestType.Lay, null) },
                deluxeProducts: new List<AnimalProduct> { new AnimalProduct("444", HarvestType.Lay, null) }));

            // create duck sprites
            var duckSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "Duck")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "BabyWhite Chicken")));

            // create duck sub type
            var duckTypes = new List<AnimalSubType>();
            duckTypes.Add(new AnimalSubType("Duck", duckProduce, duckSprites));

            // create and add duck object
            var duckShopInfo = new AnimalShopInfo(
                description: Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11337")) + Environment.NewLine + Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335")),
                buyPrice: 4000,
                shopIcon: GetSubTexture(Game1.content.Load<Texture2D>(Path.Combine("LooseSprites", "Cursors")), new Rectangle(0, 464, 32, 16)));
            var duckData = new AnimalData("Duck", true, false, duckShopInfo, duckTypes, 2, 5, "Duck", 16, 16, 16, 16, 3, 8, new List<string> { "Big Coop", "Deluxe Coop" }, 2, 1900, new List<Season> { Season.Spring, Season.Summer, Season.Fall });
            Animals.Add(new Animal("Duck", duckData));

            // construct data string for game to use
            foreach (var duckSubType in duckData.Types)
                DataStrings.Add(duckSubType.Name, $"{duckData.DaysToProduce}/{duckData.DaysTillMature}///{duckData.SoundId}//////////{duckSubType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{duckData.FrontAndBackSpriteWidth}/{duckData.FrontAndBackSpriteHeight}/{duckData.SideSpriteWidth}/{duckData.SideSpriteHeight}/{duckData.FullnessDrain}/{duckData.HappinessDrain}//0/{duckData.AnimalShopInfo?.BuyPrice}/{duckSubType.Name}/");

            //
            // Rabbit
            //
            // create rabbit produce object
            var rabbitProduce = new AnimalProduce(new AnimalProduceSeason(
                products: new List<AnimalProduct> { new AnimalProduct("440", HarvestType.Lay, null) },
                deluxeProducts: new List<AnimalProduct> { new AnimalProduct("446", HarvestType.Lay, null) }));

            // create rabbit sprites
            var rabbitSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "Rabbit")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "BabyRabbit")));

            // create rabbit sub type
            var rabbitTypes = new List<AnimalSubType>();
            rabbitTypes.Add(new AnimalSubType("Rabbit", rabbitProduce, rabbitSprites));

            // create and add rabbit object
            var rabbitShopInfo = new AnimalShopInfo(
                description: Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11340")) + Environment.NewLine + Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335")),
                buyPrice: 8000,
                shopIcon: GetSubTexture(Game1.content.Load<Texture2D>(Path.Combine("LooseSprites", "Cursors")), new Rectangle(64, 464, 32, 16)));
            var rabbitData = new AnimalData("Rabbit", true, false, rabbitShopInfo, rabbitTypes, 4, 6, "rabbit", 16, 16, 16, 16, 10, 5, new List<string> { "Deluxe Coop" }, 2, 1900, new List<Season> { Season.Spring, Season.Summer, Season.Fall });
            Animals.Add(new Animal("Rabbit", rabbitData));

            // construct data string for game to use
            foreach (var rabbitSubType in rabbitData.Types)
                DataStrings.Add(rabbitSubType.Name, $"{rabbitData.DaysToProduce}/{rabbitData.DaysTillMature}///{rabbitData.SoundId}//////////{rabbitSubType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{rabbitData.FrontAndBackSpriteWidth}/{rabbitData.FrontAndBackSpriteHeight}/{rabbitData.SideSpriteWidth}/{rabbitData.SideSpriteHeight}/{rabbitData.FullnessDrain}/{rabbitData.HappinessDrain}//0/{rabbitData.AnimalShopInfo?.BuyPrice}/{rabbitSubType.Name}/");

            //
            // Dinosaur
            //
            // create dinosaur produce object
            var dinosaurProduce = new AnimalProduce(new AnimalProduceSeason(
                products: new List<AnimalProduct> { new AnimalProduct("107", HarvestType.Lay, null) }));

            // create dinosaur sprites
            var dinosaurSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "Dinosaur")));

            // create dinosaur sub type
            var dinosaurTypes = new List<AnimalSubType>();
            dinosaurTypes.Add(new AnimalSubType("Dinosaur", dinosaurProduce, dinosaurSprites));

            // create and add dinosaur object
            var dinosaurData = new AnimalData("Dinosaur", true, false, null, dinosaurTypes, 7, 0, "none", 16, 16, 16, 16, 1, 8, new List<string> { "Deluxe Coop" }, 2, 1900, new List<Season> { Season.Spring, Season.Summer, Season.Fall });
            Animals.Add(new Animal("Dinosaur", dinosaurData));

            // construct data string for game to use
            foreach (var dinosaurSubType in dinosaurData.Types)
                DataStrings.Add(dinosaurSubType.Name, $"{dinosaurData.DaysToProduce}/{dinosaurData.DaysTillMature}///{dinosaurData.SoundId}//////////{dinosaurSubType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{dinosaurData.FrontAndBackSpriteWidth}/{dinosaurData.FrontAndBackSpriteHeight}/{dinosaurData.SideSpriteWidth}/{dinosaurData.SideSpriteHeight}/{dinosaurData.FullnessDrain}/{dinosaurData.HappinessDrain}//0/{dinosaurData.AnimalShopInfo?.BuyPrice}/{dinosaurSubType.Name}/");

            //
            // Cow
            //
            // create cow produce object
            var cowProduce = new AnimalProduce(new AnimalProduceSeason(
                products: new List<AnimalProduct> { new AnimalProduct("184", HarvestType.Tool, "Milk Pail") },
                deluxeProducts: new List<AnimalProduct> { new AnimalProduct("186", HarvestType.Tool, "Milk Pail") }));

            // create cow sprites
            var whiteCowSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "White Cow")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "BabyWhite Cow")));
            var brownCowSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "Brown Cow")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "BabyBrown Cow")));

            // create cow sub types
            var cowTypes = new List<AnimalSubType>();
            cowTypes.Add(new AnimalSubType("White Cow", cowProduce, whiteCowSprites));
            cowTypes.Add(new AnimalSubType("Brown Cow", cowProduce, brownCowSprites));

            // create and add cow object
            var cowShopInfo = new AnimalShopInfo(
                description: Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11343")) + Environment.NewLine + Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344")),
                buyPrice: 1500,
                shopIcon: GetSubTexture(Game1.content.Load<Texture2D>(Path.Combine("LooseSprites", "Cursors")), new Rectangle(32, 448, 32, 16)));
            var cowData = new AnimalData("Cow", true, false, cowShopInfo, cowTypes, 1, 5, "cow", 32, 32, 32, 32, 15, 5, new List<string> { "Barn", "Big Barn", "Deluxe Barn" }, 2, 1900, new List<Season> { Season.Spring, Season.Summer, Season.Fall });
            Animals.Add(new Animal("Cow", cowData));

            // construct data string for game to use
            foreach (var cowSubType in cowData.Types)
                DataStrings.Add(cowSubType.Name, $"{cowData.DaysToProduce}/{cowData.DaysTillMature}///{cowData.SoundId}//////////{cowSubType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{cowData.FrontAndBackSpriteWidth}/{cowData.FrontAndBackSpriteHeight}/{cowData.SideSpriteWidth}/{cowData.SideSpriteHeight}/{cowData.FullnessDrain}/{cowData.HappinessDrain}//0/{cowData.AnimalShopInfo?.BuyPrice}/{cowSubType.Name}/");

            //
            // Goat
            //
            // create goat produce object
            var goatProduce = new AnimalProduce(new AnimalProduceSeason(
                products: new List<AnimalProduct> { new AnimalProduct("436", HarvestType.Tool, "Milk Pail") },
                deluxeProducts: new List<AnimalProduct> { new AnimalProduct("438", HarvestType.Tool, "Milk Pail") }));

            // create goat sprites
            var goatSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "Goat")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "BabyGoat")));

            // create goat sub type
            var goatTypes = new List<AnimalSubType>();
            goatTypes.Add(new AnimalSubType("Goat", goatProduce, goatSprites));

            // create and add goat object
            var goatShopInfo = new AnimalShopInfo(
                description: Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11349")) + Environment.NewLine + Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344")),
                buyPrice: 4000,
                shopIcon: GetSubTexture(Game1.content.Load<Texture2D>(Path.Combine("LooseSprites", "Cursors")), new Rectangle(64, 448, 32, 16)));
            var goatData = new AnimalData("Goat", true, false, goatShopInfo, goatTypes, 2, 5, "goat", 32, 32, 32, 32, 10, 5, new List<string> { "Big Barn", "Deluxe Barn" }, 2, 1900, new List<Season> { Season.Spring, Season.Summer, Season.Fall });
            Animals.Add(new Animal("Goat", goatData));

            // construct data string for game to use
            foreach (var goatSubType in goatData.Types)
                DataStrings.Add(goatSubType.Name, $"{goatData.DaysToProduce}/{goatData.DaysTillMature}///{goatData.SoundId}//////////{goatSubType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{goatData.FrontAndBackSpriteWidth}/{goatData.FrontAndBackSpriteHeight}/{goatData.SideSpriteWidth}/{goatData.SideSpriteHeight}/{goatData.FullnessDrain}/{goatData.HappinessDrain}//0/{goatData.AnimalShopInfo?.BuyPrice}/{goatSubType.Name}/");

            //
            // Pig
            //
            // create pig produce object
            var pigProduce = new AnimalProduce(new AnimalProduceSeason(
                products: new List<AnimalProduct> { new AnimalProduct("430", HarvestType.Forage, null) }));

            // create pig sprites
            var pigSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "Pig")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "BabyPig")));

            // create pig sub type
            var pigTypes = new List<AnimalSubType>();
            pigTypes.Add(new AnimalSubType("Pig", pigProduce, pigSprites));

            // create and add pig object
            var pigShopInfo = new AnimalShopInfo(
                description: Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11346")) + Environment.NewLine + Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344")),
                buyPrice: 16000,
                shopIcon: GetSubTexture(Game1.content.Load<Texture2D>(Path.Combine("LooseSprites", "Cursors")), new Rectangle(0, 480, 32, 16)));
            var pigData = new AnimalData("Pig", true, false, pigShopInfo, pigTypes, 2, 5, "pig", 32, 32, 32, 32, 20, 5, new List<string> { "Deluxe Barn" }, 2, 1900, new List<Season> { Season.Spring, Season.Summer, Season.Fall });
            Animals.Add(new Animal("Pig", pigData));

            // construct data string for game to use
            foreach (var pigSubType in pigData.Types)
                DataStrings.Add(pigSubType.Name, $"{pigData.DaysToProduce}/{pigData.DaysTillMature}///{pigData.SoundId}//////////{pigSubType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{pigData.FrontAndBackSpriteWidth}/{pigData.FrontAndBackSpriteHeight}/{pigData.SideSpriteWidth}/{pigData.SideSpriteHeight}/{pigData.FullnessDrain}/{pigData.HappinessDrain}//0/{pigData.AnimalShopInfo?.BuyPrice}/{pigSubType.Name}/");

            //
            // Sheep
            //
            // create sheep produce object
            var sheepProduce = new AnimalProduce(new AnimalProduceSeason(
                products: new List<AnimalProduct> { new AnimalProduct("440", HarvestType.Tool, "Shears") }));

            // create sheep sprites
            var sheepSprites = new AnimalSprites(Game1.content.Load<Texture2D>(Path.Combine("Animals", "Sheep")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "BabySheep")), Game1.content.Load<Texture2D>(Path.Combine("Animals", "ShearedSheep")));

            // create sheep sub type
            var sheepTypes = new List<AnimalSubType>();
            sheepTypes.Add(new AnimalSubType("Sheep", sheepProduce, sheepSprites));

            // create and add sheep object
            var sheepShopInfo = new AnimalShopInfo(
                description: Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11352")) + Environment.NewLine + Game1.content.LoadString(Path.Combine("Strings", "StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344")),
                buyPrice: 8000,
                shopIcon: GetSubTexture(Game1.content.Load<Texture2D>(Path.Combine("LooseSprites", "Cursors")), new Rectangle(32, 464, 32, 16)));
            var sheepData = new AnimalData("Sheep", true, false, sheepShopInfo, sheepTypes, 2, 5, "sheep", 32, 32, 32, 32, 20, 5, new List<string> { "Deluxe Barn" }, 2, 1900, new List<Season> { Season.Spring, Season.Summer, Season.Fall });
            Animals.Add(new Animal("Sheep", sheepData));

            // construct data string for game to use
            foreach (var sheepSubType in sheepData.Types)
                DataStrings.Add(sheepSubType.Name, $"{sheepData.DaysToProduce}/{sheepData.DaysTillMature}///{sheepData.SoundId}//////////{sheepSubType.Sprites.HasDifferentSpriteSheetWhenHarvested()}//{sheepData.FrontAndBackSpriteWidth}/{sheepData.FrontAndBackSpriteHeight}/{sheepData.SideSpriteWidth}/{sheepData.SideSpriteHeight}/{sheepData.FullnessDrain}/{sheepData.HappinessDrain}//0/{sheepData.AnimalShopInfo?.BuyPrice}/{sheepSubType.Name}/");
        }

        /// <summary>Get a sub texture.</summary>
        /// <param name="texture">The source texture for the sub texture.</param>
        /// <param name="sourceRectangle">The bounds of the sub texture.</param>
        /// <returns>The texture at the source Rectangle of the passed texture.</returns>
        private Texture2D GetSubTexture(Texture2D texture, Rectangle sourceRectangle)
        {
            var textureData = new Color[texture.Width * texture.Height];
            var subTextureData = new Color[sourceRectangle.Width * sourceRectangle.Height];

            texture.GetData(textureData);

            // loop through the sub texture section and copy over to hte subTextureData
            for (int x = sourceRectangle.X; x < sourceRectangle.X + sourceRectangle.Width; x++)
                for (int y = sourceRectangle.Y; y < sourceRectangle.Y + sourceRectangle.Height; y++)
                    subTextureData[(y - sourceRectangle.Y) * sourceRectangle.Width + (x - sourceRectangle.X)] = textureData[y * texture.Width + x];

            var subTexture = new Texture2D(Game1.graphics.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            subTexture.SetData(subTextureData);
            return subTexture;
        }
    }
}
