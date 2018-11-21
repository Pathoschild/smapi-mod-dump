using System;
using System.Collections.Generic;
using System.Linq;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.cooking;
using AnimalHusbandryMod.meats;
using AnimalHusbandryMod.recipes;
using AnimalHusbandryMod.tools;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace AnimalHusbandryMod.common
{
    public class DataLoader : IAssetEditor
    {
        public static IModHelper Helper;
        public static ModConfig ModConfig;
        public static ITranslationHelper i18n;

        public static MeatData MeatData;
        public static CookingData CookingData;
        public static AnimalData AnimalData;
        public static AnimalBuildingData AnimalBuildingData;

        public static String LooseSpritesName;
        public static Texture2D LooseSprites;
        public static Texture2D ToolsSprites;

        public ToolsLoader ToolsLoader { get; }
        public RecipesLoader RecipeLoader { get; }

        public LivingWithTheAnimalsChannel LivingWithTheAnimalsChannel { get; }

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            ModConfig = helper.ReadConfig<ModConfig>();
            i18n = Helper.Translation;

            LooseSpritesName = Helper.Content.GetActualAssetKey("common/LooseSprites.png", ContentSource.ModFolder);
            LooseSprites = Helper.Content.Load<Texture2D>("common/LooseSprites.png");

            var editors = Helper.Content.AssetEditors;

            //editors.Add(new EventsLoader());

            if (!ModConfig.DisableMeat)
            {               
                editors.Add(this);
            }

            ToolsSprites = Helper.Content.Load<Texture2D>("tools/Tools.png");
            ToolsLoader = new ToolsLoader(ToolsSprites, Helper.Content.Load<Texture2D>("tools/MenuTiles.png"), Helper.Content.Load<Texture2D>("common/CustomLetterBG.png"));   
            editors.Add(ToolsLoader);
            ToolsLoader.LoadMail();

            if (!ModConfig.DisableMeat)
            {
                RecipeLoader = new RecipesLoader();
                editors.Add(RecipeLoader);
                RecipeLoader.LoadMails();
            }

            AnimalBuildingData = DataLoader.Helper.Data.ReadJsonFile<AnimalBuildingData>("data\\animalBuilding.json") ?? new AnimalBuildingData();
            DataLoader.Helper.Data.WriteJsonFile("data\\animalBuilding.json", AnimalBuildingData);

            AnimalData = DataLoader.Helper.Data.ReadJsonFile<AnimalData>("data\\animals.json") ?? new AnimalData();
            DataLoader.Helper.Data.WriteJsonFile("data\\animals.json", AnimalData);

            CookingData = Helper.Data.ReadJsonFile<CookingData>("data\\cooking.json") ?? new CookingData();
            if (CookingData.Meatloaf.Recipe == null)
            {
                CookingData.CloneRecipeAndAmount(new CookingData());
            }
            Helper.Data.WriteJsonFile("data\\cooking.json", CookingData);

            LivingWithTheAnimalsChannel = new LivingWithTheAnimalsChannel();
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\ObjectInformation") || asset.AssetNameEquals("Data\\Bundles") || asset.AssetNameEquals("Data\\NPCGiftTastes");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data\\ObjectInformation"))
            {
                var data = asset.AsDictionary<int, string>().Data;
                //MEAT
                MeatData = DataLoader.Helper.Data.ReadJsonFile<MeatData>("data\\meats.json") ?? new MeatData();
                DataLoader.Helper.Data.WriteJsonFile("data\\meats.json", MeatData);

                data[(int)Meat.Beef] = Meat.Beef.GetObjectString();
                data[(int)Meat.Pork] = Meat.Pork.GetObjectString();
                data[(int)Meat.Chicken] = Meat.Chicken.GetObjectString();
                data[(int)Meat.Duck] = Meat.Duck.GetObjectString();
                data[(int)Meat.Rabbit] = Meat.Rabbit.GetObjectString();
                data[(int)Meat.Mutton] = Meat.Mutton.GetObjectString();

                //COOKING

                data[(int)Cooking.Meatloaf] = Cooking.Meatloaf.GetObjectString();
                data[(int)Cooking.OrangeChicken] = Cooking.OrangeChicken.GetObjectString();
                data[(int)Cooking.MonteCristo] = Cooking.MonteCristo.GetObjectString();
                data[(int)Cooking.BaconCheeseburger] = Cooking.BaconCheeseburger.GetObjectString();
                data[(int)Cooking.RoastDuck] = Cooking.RoastDuck.GetObjectString();
                data[(int)Cooking.RabbitAuVin] = Cooking.RabbitAuVin.GetObjectString();
                data[(int)Cooking.SteakFajitas] = Cooking.SteakFajitas.GetObjectString();
                data[(int)Cooking.GlazedHam] = Cooking.GlazedHam.GetObjectString();
                data[(int)Cooking.SummerSausage] = Cooking.SummerSausage.GetObjectString();
                data[(int)Cooking.SweetAndSourPork] = Cooking.SweetAndSourPork.GetObjectString();
                data[(int)Cooking.RabbitStew] = Cooking.RabbitStew.GetObjectString();
                data[(int)Cooking.WinterDuck] = Cooking.WinterDuck.GetObjectString();
                data[(int)Cooking.SteakWithMushrooms] = Cooking.SteakWithMushrooms.GetObjectString();
                data[(int)Cooking.CowboyDinner] = Cooking.CowboyDinner.GetObjectString();
                data[(int)Cooking.Bacon] = Cooking.Bacon.GetObjectString();
            }
            else if (asset.AssetNameEquals("Data\\Bundles"))
            {
                var data = asset.AsDictionary<string, string>().Data;
                string value = data["Pantry/4"];
                if (!value.Contains("644 1 0") && value.Contains("/4/5"))
                {
                    value = value.Insert(value.LastIndexOf("/4/5"), " 644 1 0");
                }
                
                data["Pantry/4"] = value;
            }
            else if (asset.AssetNameEquals("Data\\NPCGiftTastes"))
            {
                var data = asset.AsDictionary<string, string>().Data;
                AddUniversalGiftTaste(data, Taste.Dislike, "-14");
                AddNpcGiftTaste(data, "Linus", Taste.Neutral, "-14");
                AddNpcGiftTaste(data, "Linus", Taste.Like, "643");
                AddNpcGiftTaste(data, "Linus", Taste.Love, "657", "662");
                AddNpcGiftTaste(data, "Pam", Taste.Dislike, "656", "665");
                AddNpcGiftTaste(data, "Pam", Taste.Neutral, "-14");
                AddNpcGiftTaste(data, "Pam", Taste.Love, "657");
                AddNpcGiftTaste(data, "Gus", Taste.Neutral, "-14");
                AddNpcGiftTaste(data, "Gus", Taste.Love, "653", "660");
                AddNpcGiftTaste(data, "Jodi", Taste.Hate, "652", "660");
                AddNpcGiftTaste(data, "Jodi", Taste.Neutral, "-14");
                AddNpcGiftTaste(data, "Jodi", Taste.Love, "661");            
                AddNpcGiftTaste(data, "Jodi", Taste.Like, "640");            
                AddNpcGiftTaste(data, "Kent", Taste.Neutral, "-14");                
                AddNpcGiftTaste(data, "Kent", Taste.Like, "642");
                AddNpcGiftTaste(data, "Kent", Taste.Love, "656", "663");
                AddNpcGiftTaste(data, "Marnie", Taste.Hate, MeatDishes.Concat(new string[]{"-14"}).ToArray());
                AddNpcGiftTaste(data, "Evelyn", Taste.Hate, "-14");
                AddNpcGiftTaste(data, "Evelyn", Taste.Dislike, MeatDishes);
                AddNpcGiftTaste(data, "Emily", Taste.Hate, "-14");
                AddNpcGiftTaste(data, "Emily", Taste.Dislike, MeatDishes);                
                AddNpcGiftTaste(data, "Alex", Taste.Like, "639");
                AddNpcGiftTaste(data, "Shane", Taste.Hate, "641", "653");
                AddNpcGiftTaste(data, "Shane", Taste.Love, "655", "658");
                AddNpcGiftTaste(data, "Leah", Taste.Hate, "644");
                AddNpcGiftTaste(data, "Leah", Taste.Dislike, "655", "660", "666");
                AddNpcGiftTaste(data, "Harvey", Taste.Dislike, "654", "655");
                AddNpcGiftTaste(data, "Harvey", Taste.Love, "657", "664");
                AddNpcGiftTaste(data, "Sam", Taste.Dislike, "662", "664");
                AddNpcGiftTaste(data, "Sebastian", Taste.Dislike, "665");
                AddNpcGiftTaste(data, "Sebastian", Taste.Love, "661");
                AddNpcGiftTaste(data, "Abigail", Taste.Love, "666");
                AddNpcGiftTaste(data, "Haley", Taste.Love, "657", "663");
                AddNpcGiftTaste(data, "Maru", Taste.Dislike, "659");
                AddNpcGiftTaste(data, "Maru", Taste.Love, "656");
                AddNpcGiftTaste(data, "Penny", Taste.Dislike, "657");
                AddNpcGiftTaste(data, "Penny", Taste.Love, "662");
                AddNpcGiftTaste(data, "Caroline", Taste.Love, "653", "661");
                AddNpcGiftTaste(data, "Clint", Taste.Dislike, "663");
                AddNpcGiftTaste(data, "Demetrius", Taste.Dislike, "660");
                AddNpcGiftTaste(data, "Demetrius", Taste.Love, "665");
                AddNpcGiftTaste(data, "George", Taste.Hate, "658");
                AddNpcGiftTaste(data, "George", Taste.Love, "662", "664");             
                AddNpcGiftTaste(data, "Jas", Taste.Love, "659");
                AddNpcGiftTaste(data, "Lewis", Taste.Hate, "654");
                AddNpcGiftTaste(data, "Lewis", Taste.Love, "652", "659", "663");
                AddNpcGiftTaste(data, "Pierre", Taste.Dislike, "665");
                AddNpcGiftTaste(data, "Pierre", Taste.Love, "654", "661", "666");
                AddNpcGiftTaste(data, "Robin", Taste.Love, "652", "665");
                AddNpcGiftTaste(data, "Sandy", Taste.Hate, "661");
                AddNpcGiftTaste(data, "Vincent", Taste.Love, "659");
            }
        }

        private static string[] MeatDishes = { "652", "653", "654", "655", "656", "657", "658", "659", "660", "661", "662", "663", "664", "665", "666" };

        private static void AddUniversalGiftTaste(IDictionary<string, string> data, Taste taste, params string[] values)
        {
            string key = "Universal_" + taste;
            string currentValues = data[key];
            string valuesToAdd = values
                .Where(v => !currentValues.Contains(v))
                .Aggregate(string.Empty, (workingSentence,next)=> workingSentence + " " + next);
            if (valuesToAdd.Length > 0)
            {
                currentValues += valuesToAdd;
                data[key] = currentValues.Trim();
            }
        }

        private static void AddNpcGiftTaste(IDictionary<string, string> data, string npc, Taste taste, params string[] values)
        {
            string[] tastes = data[npc].Split('/');
            string currentValues = tastes[(int)taste];
            string valuesToAdd = values.Where(v => !currentValues.Contains(v)).Aggregate(string.Empty, (workingSentence, next) => workingSentence + " " + next);
            if (valuesToAdd.Length > 0)
            {
                currentValues += valuesToAdd;
                tastes[(int)taste] = currentValues.Trim();
                data[npc] = String.Join("/",tastes);
            }
        }

        enum Taste {
            Love = 1,
            Like = 3,
            Dislike = 5,
            Hate = 7,
            Neutral = 9
        }
    }
}
