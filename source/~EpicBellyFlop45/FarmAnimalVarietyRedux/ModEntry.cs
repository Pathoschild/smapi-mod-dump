using FarmAnimalVarietyRedux.Models;
using FarmAnimalVarietyRedux.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;

namespace FarmAnimalVarietyRedux
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Fields 
        *********/
        /// <summary>All the new animals in the structure the game requires to read them.</summary>
        private List<string> AnimalDataStrings = new List<string>();


        /*********
        ** Accessors
        *********/
        /// <summary>Provides methods for interacting with the mod directory.</summary>
        public static IModHelper ModHelper { get; private set; }

        /// <summary>Provides methods for logging to the console.</summary>
        public static IMonitor ModMonitor { get; private set; }

        /// <summary>A list of all the animals.</summary>
        public static List<Animal> Animals { get; private set; } = new List<Animal>();


        /*********
        ** Public Methods 
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = this.Helper;
            ModMonitor = this.Monitor;

            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            this.Helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(Path.Combine("Data", "FarmAnimals"));
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            foreach (var animalData in AnimalDataStrings)
            {
                string animalType = animalData.Split('/')[25];
                data.Add(animalType, animalData);
            }
        }


        /*********
        ** Private Methods 
        *********/
        /****
        ** Event Handlers
        ****/
        /// <summary>Invoked when the player presses a button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.L)
            {
                if (Game1.activeClickableMenu is AnimalPurchaseMenu)
                {
                    Game1.activeClickableMenu = null;
                }
                else
                {
                    Game1.activeClickableMenu = new AnimalPurchaseMenu();
                }
            }

            if (e.Button == SButton.J)
            {
                if (Game1.activeClickableMenu is PurchaseAnimalsMenu)
                {
                    Game1.activeClickableMenu = null;
                }
                else
                {
                    Game1.activeClickableMenu = new PurchaseAnimalsMenu(Utility.getPurchaseAnimalStock());
                }
            }
        }

        /// <summary>Invoked when the player loads up a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // if the user loads another save without restarting the game then there will be duplicates in the Animals list
            Animals = new List<Animal>();

            LoadDefaultAnimals();
            LoadContentPacks();
        }

        /****
        ** Methods
        ****/
        /// <summary>Load all the animal data for the in-game animals.</summary>
        private void LoadDefaultAnimals()
        {
            // chicken
            Animals.Add(new Animal(
                name: "Chicken",
                data: new AnimalData(
                    description: "Well cared-for adult chickens lay eggs every day.\nLives in the coop.",
                    daysToProduce: 1,
                    daysTillMature: 3,
                    soundId: "cluck",
                    harvestType: HarvestType.Lay,
                    harvestToolName: null,
                    frontAndBackSpriteWidth: 16,
                    frontAndBackSpriteHeight: 16,
                    sideSpriteWidth: 16,
                    sideSpriteHeight: 16,
                    fullnessDrain: 4,
                    happinessDrain: 7,
                    happinessIncrease: 36,
                    buyPrice: 800,
                    buildings: new List<string>
                    {
                        "Coop",
                        "Big Coop",
                        "Deluxe Coop"
                    },
                    walkSpeed: 1,
                    bedTime: 1900,
                    seasonsAllowedOutdoors: new List<Season>
                    {
                        Season.Spring,
                        Season.Summer,
                        Season.Fall
                    }),
                shopIcon: GetDefaultShopIcon(new Rectangle(0, 448, 32, 16)),
                subTypes: new List<AnimalSubType>
                {
                    new AnimalSubType(
                        name: "White Chicken",
                        data: new AnimalSubTypeData(
                            productId: "176",
                            deluxeProductId: "174"),
                        sprites: new AnimalSprites(
                            adultSpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "white chicken"), ContentSource.GameContent),
                            babySpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "babywhite chicken"), ContentSource.GameContent))
                    ),
                    new AnimalSubType(
                        name: "Brown Chicken",
                        data: new AnimalSubTypeData(
                            productId: "180",
                            deluxeProductId: "182"),
                        sprites: new AnimalSprites(
                            adultSpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "brown chicken"), ContentSource.GameContent),
                            babySpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "babybrown chicken"), ContentSource.GameContent))
                    ),
                }
            ));

            // cow
            Animals.Add(new Animal(
                name: "Cow",
                data: new AnimalData(
                    description: "Adults can be milked daily. A milk pail is required to harvest the milk.\nLives in the barn.",
                    daysToProduce: 1,
                    daysTillMature: 5,
                    soundId: "cow",
                    harvestType: HarvestType.Tool,
                    harvestToolName: "Milk Pail",
                    frontAndBackSpriteWidth: 32,
                    frontAndBackSpriteHeight: 32,
                    sideSpriteWidth: 32,
                    sideSpriteHeight: 32,
                    fullnessDrain: 15,
                    happinessDrain: 5,
                    happinessIncrease:35 ,
                    buyPrice: 1500,
                    buildings: new List<string>
                    {
                        "Barn",
                        "Big Barn",
                        "Deluxe Barn"
                    },
                    walkSpeed: 1,
                    bedTime: 1900,
                    seasonsAllowedOutdoors: new List<Season>
                    {
                        Season.Spring,
                        Season.Summer,
                        Season.Fall
                    }),
                shopIcon: GetDefaultShopIcon(new Rectangle(32, 448, 32, 16)),
                subTypes: new List<AnimalSubType>
                {
                    new AnimalSubType(
                        name: "White Cow",
                        data: new AnimalSubTypeData(
                            productId: "184",
                            deluxeProductId: "186"),
                        sprites: new AnimalSprites(
                            adultSpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "white cow"), ContentSource.GameContent),
                            babySpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "babywhite cow"), ContentSource.GameContent))
                    ),
                }
            ));

            // goat
            Animals.Add(new Animal(
                name: "Goat",
                data: new AnimalData(
                    description: "Happy adults provide goat milk every other day. A milk pail is required to harvest the milk.\nLives in the barn.",
                    daysToProduce: 2,
                    daysTillMature: 5,
                    soundId: "goat",
                    harvestType: HarvestType.Tool,
                    harvestToolName: "Milk Pail",
                    frontAndBackSpriteWidth: 32,
                    frontAndBackSpriteHeight: 32,
                    sideSpriteWidth: 32,
                    sideSpriteHeight: 32,
                    fullnessDrain: 10,
                    happinessDrain: 5,
                    happinessIncrease: 30,
                    buyPrice: 4000,
                    buildings: new List<string>
                    {
                        "Big Barn",
                        "Deluxe Barn"
                    },
                    walkSpeed: 1,
                    bedTime: 1900,
                    seasonsAllowedOutdoors: new List<Season>
                    {
                        Season.Spring,
                        Season.Summer,
                        Season.Fall
                    }),
                shopIcon: GetDefaultShopIcon(new Rectangle(64, 448, 32, 16)),
                subTypes: new List<AnimalSubType>
                {
                    new AnimalSubType(
                        name: "Goat",
                        data: new AnimalSubTypeData(
                            productId: "436",
                            deluxeProductId: "438"),
                        sprites: new AnimalSprites(
                            adultSpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "goat"), ContentSource.GameContent),
                            babySpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "babygoat"), ContentSource.GameContent))
                    ),
                }
            ));

            // duck
            Animals.Add(new Animal(
                name: "Duck",
                data: new AnimalData(
                    description: "Happy adults lay duck eggs every other day.\nLives in the coop.",
                    daysToProduce: 2,
                    daysTillMature: 5,
                    soundId: "Duck",
                    harvestType: HarvestType.Lay,
                    harvestToolName: null,
                    frontAndBackSpriteWidth: 16,
                    frontAndBackSpriteHeight: 16,
                    sideSpriteWidth: 16,
                    sideSpriteHeight: 16,
                    fullnessDrain: 3,
                    happinessDrain: 8,
                    happinessIncrease: 32,
                    buyPrice: 4000,
                    buildings: new List<string>
                    {
                        "Big Coop",
                        "Deluxe Coop"
                    },
                    walkSpeed: 1,
                    bedTime: 1900,
                    seasonsAllowedOutdoors: new List<Season>
                    {
                        Season.Spring,
                        Season.Summer,
                        Season.Fall
                    }),
                shopIcon: GetDefaultShopIcon(new Rectangle(0, 464, 32, 16)),
                subTypes: new List<AnimalSubType>
                {
                    new AnimalSubType(
                        name: "Duck",
                        data: new AnimalSubTypeData(
                            productId: "442",
                            deluxeProductId: "444"),
                        sprites: new AnimalSprites(
                            adultSpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "duck"), ContentSource.GameContent),
                            babySpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "babywhite chicken"), ContentSource.GameContent))
                    ),
                }
            )); ; ;
        
            // sheep
            Animals.Add(new Animal(
                name: "Sheep",
                data: new AnimalData(
                    description: "Adults can be shorn for wool. Sheep who form a close bond with their owners can grow wool faster. A pair of shears is required to harvest the wool.\nLives in the barn.",
                    daysToProduce: 3,
                    daysTillMature: 4,
                    soundId: "sheep",
                    harvestType: HarvestType.Tool,
                    harvestToolName: "Shears",
                    frontAndBackSpriteWidth: 32,
                    frontAndBackSpriteHeight: 32,
                    sideSpriteWidth: 32,
                    sideSpriteHeight: 32,
                    fullnessDrain: 15,
                    happinessDrain: 5,
                    happinessIncrease: 35,
                    buyPrice: 8000,
                    buildings: new List<string>
                    {
                        "Deluxe Barn"
                    },
                    walkSpeed: 1,
                    bedTime: 1900,
                    seasonsAllowedOutdoors: new List<Season>
                    {
                        Season.Spring,
                        Season.Summer,
                        Season.Fall
                    }),
                shopIcon: GetDefaultShopIcon(new Rectangle(32, 464, 32, 16)),
                subTypes: new List<AnimalSubType>
                {
                    new AnimalSubType(
                        name: "Sheep",
                        data: new AnimalSubTypeData(
                            productId: "440",
                            deluxeProductId: "-1"),
                        sprites: new AnimalSprites(
                            adultSpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "sheep"), ContentSource.GameContent),
                            babySpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "babysheep"), ContentSource.GameContent),
                            harvestedSpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "shearedsheep"), ContentSource.GameContent))
                    ),
                }
            ));

            // rabbit
            Animals.Add(new Animal(
                name: "Rabbit",
                data: new AnimalData(
                    description: "These are wooly rabbits! They shed precious wool every few days.\nLives in the coop.",
                    daysToProduce: 4,
                    daysTillMature: 6,
                    soundId: "rabbit",
                    harvestType: HarvestType.Lay,
                    harvestToolName: null,
                    frontAndBackSpriteWidth: 16,
                    frontAndBackSpriteHeight: 16,
                    sideSpriteWidth: 16,
                    sideSpriteHeight: 16,
                    fullnessDrain: 10,
                    happinessDrain: 5,
                    happinessIncrease: 35,
                    buyPrice: 8000,
                    buildings: new List<string>
                    {
                        "Deluxe Coop"
                    },
                    walkSpeed: 1,
                    bedTime: 1900,
                    seasonsAllowedOutdoors: new List<Season>
                    {
                        Season.Spring,
                        Season.Summer,
                        Season.Fall
                    }),
                shopIcon: GetDefaultShopIcon(new Rectangle(64, 464, 32, 16)),
                subTypes: new List<AnimalSubType>
                {
                    new AnimalSubType(
                        name: "Rabbit",
                        data: new AnimalSubTypeData(
                            productId: "440",
                            deluxeProductId: "446"),
                        sprites: new AnimalSprites(
                            adultSpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "rabbit"), ContentSource.GameContent),
                            babySpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "babyrabbit"), ContentSource.GameContent))
                    ),
                }
            ));
            
            // pig
            Animals.Add(new Animal(
                name: "Pig",
                data: new AnimalData(
                    description: "These pigs are trained to find truffles!\nLives in the barn.",
                    daysToProduce: 1,
                    daysTillMature: 10,
                    soundId: "pig",
                    harvestType: HarvestType.Scavenge,
                    harvestToolName: null,
                    frontAndBackSpriteWidth: 32,
                    frontAndBackSpriteHeight: 32,
                    sideSpriteWidth: 32,
                    sideSpriteHeight: 32,
                    fullnessDrain: 20,
                    happinessDrain: 5,
                    happinessIncrease: 35,
                    buyPrice: 16000,
                    buildings: new List<string>
                    {
                        "Deluxe Barn"
                    },
                    walkSpeed: 1,
                    bedTime: 1900,
                    seasonsAllowedOutdoors: new List<Season>
                    {
                        Season.Spring,
                        Season.Summer,
                        Season.Fall
                    }),
                shopIcon: GetDefaultShopIcon(new Rectangle(0, 480, 32, 16)),
                subTypes: new List<AnimalSubType>
                {
                    new AnimalSubType(
                        name: "Pig",
                        data: new AnimalSubTypeData(
                            productId: "430",
                            deluxeProductId: "-1"),
                        sprites: new AnimalSprites(
                            adultSpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "pig"), ContentSource.GameContent),
                            babySpriteSheet: this.Helper.Content.Load<Texture2D>(Path.Combine("animals", "babypig"), ContentSource.GameContent))
                    ),
                }
            ));
        }

        /// <summary>Get the sub sprite of Game1.MouseCursors.</summary>
        /// <param name="bound">The rectangle of the sub sprite to get.</param>
        /// <returns>The sub sprite using bound.</returns>
        private Texture2D GetDefaultShopIcon(Rectangle bound)
        {
            Texture2D shopIconSprite = new Texture2D(Game1.graphics.GraphicsDevice, bound.Width, bound.Height);
            Color[] shopIconData = new Color[bound.Width * bound.Height];

            Game1.mouseCursors.GetData(0, bound, shopIconData, 0, shopIconData.Length);
            shopIconSprite.SetData(shopIconData);
            return shopIconSprite;
        }

        /// <summary>Load all the sprites for the new animals from the loaded content packs.</summary>
        private void LoadContentPacks()
        {
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
                    bool isAnimalValid = animalData.IsValid(animalName);
                    if (!isAnimalValid)
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
                            adultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"{type}.png"), contentPack),
                            babySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"Baby {type}.png"), contentPack),
                            harvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", $"Harvested {type}.png"), contentPack),
                            springAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"{type}.png"), contentPack),
                            springHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"Harvested {type}.png"), contentPack),
                            springBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "spring", $"Baby {type}.png"), contentPack),
                            summerAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"{type}.png"), contentPack),
                            summerHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"Harvested {type}.png"), contentPack),
                            summerBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "summer", $"Baby {type}.png"), contentPack),
                            fallAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"{type}.png"), contentPack),
                            fallHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"Harvested {type}.png"), contentPack),
                            fallBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "fall", $"Baby {type}.png"), contentPack),
                            winterAdultSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"{type}.png"), contentPack),
                            winterHarvestedSpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"Harvested {type}.png"), contentPack),
                            winterBabySpriteSheet: GetSpriteByPath(Path.Combine(animalName, "assets", "winter", $"Baby {type}.png"), contentPack)
                        );

                        // ensure sprites are valid
                        if (!sprites.IsValid())
                        {
                            this.Monitor.Log($"Sprites are not valid for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} >> Subtype: {type}", LogLevel.Error);
                            continue;
                        }

                        // get data
                        AnimalSubTypeData data;
                        if (File.Exists(Path.Combine(animalFolder, "assets", $"{type} content.json")))
                        {
                            data = contentPack.LoadAsset<AnimalSubTypeData>(Path.Combine(animalName, "assets", $"{type} content.json"));
                        }
                        else if (File.Exists(Path.Combine(animalFolder, "assets", "content.json")))
                        {
                            data = contentPack.LoadAsset<AnimalSubTypeData>(Path.Combine(animalName, "assets", "content.json"));
                        }
                        else
                        {
                            this.Monitor.Log($"Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} >> Type: {type} doesn't have a content.json file.", LogLevel.Error);
                            continue;
                        }

                        // resolve API tokens in data and validate it
                        data.ResolveTokens();
                        if (!data.IsValid(type))
                        {
                            this.Monitor.Log($"Data is not valid for Content pack: {contentPack.Manifest.Name} >> Animal: {animalName} >> Subtype: {type}", LogLevel.Error);
                            continue;
                        }

                        // create subtype and add to animal
                        var animalSubType = new AnimalSubType(
                            name: type,
                            data: data,
                            sprites: sprites
                        );

                        animalSubTypes.Add(animalSubType);
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
                }
            }
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
    }
}
