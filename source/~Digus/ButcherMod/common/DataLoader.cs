/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.cooking;
using AnimalHusbandryMod.integrations;
using AnimalHusbandryMod.meats;
using AnimalHusbandryMod.recipes;
using AnimalHusbandryMod.tools;
using MailFrameworkMod;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace AnimalHusbandryMod.common
{
    public class DataLoader : IAssetEditor, IAssetLoader
    {
        private static readonly string[] MeatDishes = { "652", "653", "654", "655", "656", "657", "658", "659", "660", "661", "662", "663", "664", "665", "666" };
        private static readonly string[] BaseGameAnimals = new string[] { "White Chicken", "Brown Chicken", "Blue Chicken", "Void Chicken", "Duck", "Rabbit", "Dinosaur", "White Cow", "Brown Cow", "Goat", "Pig", "Hog", "Sheep" };

        public static IModHelper Helper;
        public static ModConfig ModConfig;
        public static ITranslationHelper i18n;

        public static MeatData MeatData;
        public static CookingData CookingData;
        public static AnimalData AnimalData;
        public static AnimalBuildingData AnimalBuildingData;
        public static AnimalContestData AnimalContestData;

        public static String LooseSpritesName;
        public static Texture2D LooseSprites;
        public static Texture2D ToolsSprites;

        public ToolsLoader ToolsLoader { get; }
        public RecipesLoader RecipeLoader { get; }

        public LivingWithTheAnimalsChannel LivingWithTheAnimalsChannel { get; }

        public static Dictionary<string, object> AssetsToLoad = new Dictionary<string, object>();

        public static bool isLoadingFarmAnimals =  false;

        public static IDynamicGameAssetsApi DgaApi;

        public DataLoader(IModHelper helper, IManifest manifest)
        {
            Helper = helper;
            ModConfig = helper.ReadConfig<ModConfig>();
            i18n = Helper.Translation;
            DgaApi = DataLoader.Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");

            LooseSpritesName = Helper.Content.GetActualAssetKey("common/LooseSprites.png", ContentSource.ModFolder);
            LooseSprites = Helper.Content.Load<Texture2D>("common/LooseSprites.png");

            // load tools
            ToolsSprites = Helper.Content.Load<Texture2D>("tools/Tools.png");
            ToolsLoader = new ToolsLoader(ToolsSprites, Helper.Content.Load<Texture2D>("tools/MenuTiles.png"), Helper.Content.Load<Texture2D>("common/CustomLetterBG.png"));   
            ToolsLoader.LoadMail();

            // load recipes
            if (!ModConfig.DisableMeat)
            {
                RecipeLoader = new RecipesLoader();
                RecipeLoader.LoadMails();
            }

            //load treats mail
            if (!ModConfig.DisableTreats)
            {
                LoadTreatsMail();
            }

            // load animal data
            AnimalBuildingData = DataLoader.Helper.Data.ReadJsonFile<AnimalBuildingData>("data\\animalBuilding.json") ?? new AnimalBuildingData();
            DataLoader.Helper.Data.WriteJsonFile("data\\animalBuilding.json", AnimalBuildingData);
            AnimalData = DataLoader.Helper.Data.ReadJsonFile<AnimalData>("data\\animals.json") ?? new AnimalData();
            DataLoader.Helper.Data.WriteJsonFile("data\\animals.json", AnimalData);
            AnimalContestData = DataLoader.Helper.Data.ReadJsonFile<AnimalContestData>("data\\animalContest.json") ?? new AnimalContestData();
            DataLoader.Helper.Data.WriteJsonFile("data\\animalContest.json", AnimalContestData);

            // look cooking data
            CookingData = Helper.Data.ReadJsonFile<CookingData>("data\\cooking.json") ?? new CookingData();
            if (CookingData.Meatloaf.Recipe == null)
            {
                CookingData.CloneRecipeAndAmount(new CookingData());
            }
            Helper.Data.WriteJsonFile("data\\cooking.json", CookingData);

            // load Livin' With The Animals channel
            TvController.AddChannel(new LivingWithTheAnimalsChannel());

            // add editors (must happen *after* data is initialised above, since SMAPI may reload affected assets immediately)
            var editors = Helper.Content.AssetEditors;
            var loaders = Helper.Content.AssetLoaders;
            if (!ModConfig.DisableAnimalContest)
            {
                editors.Add(new EventsLoader());
                loaders.Add(this);
            }
            editors.Add(ToolsLoader);
            editors.Add(this);
            if (!ModConfig.DisableMeat)
            {
                editors.Add(RecipeLoader);
            }

            CreateConfigMenu(manifest);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return (!ModConfig.DisableMeat && (asset.AssetNameEquals("Data\\ObjectInformation") || asset.AssetNameEquals("Data\\Bundles") || asset.AssetNameEquals("Data\\NPCGiftTastes"))) || asset.AssetNameEquals("Data\\FarmAnimals");
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
                data[(int)Meat.Ostrich] = Meat.Ostrich.GetObjectString();

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
                if (!ModConfig.DisableMeatInBlundle)
                {
                    if (!value.Contains("644 1 0") && value.Contains("/4/5"))
                    {
                        value = value.Insert(value.LastIndexOf("/4/5"), " 644 1 0");
                    }
                }
                else
                {
                    if (value.Contains(" 639 1 0 640 1 0 641 1 0 642 1 0 643 1 0"))
                    {
                        value = value.Replace(" 639 1 0 640 1 0 641 1 0 642 1 0 643 1 0","");
                    }
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
            else if (asset.AssetNameEquals("Data\\FarmAnimals"))
            {
                AddCustomAnimalsTemplate((Dictionary<string,string>) (isLoadingFarmAnimals? asset.Data : null));
            }
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return AssetsToLoad.ContainsKey(asset.AssetName);
        }

        public T Load<T>(IAssetInfo asset)
        {
            return (T)AssetsToLoad[asset.AssetName];
        }

        private static void LoadTreatsMail()
        {
            MailDao.SaveLetter(
                new Letter(
                    "DinosaursFirtTreat"
                    , DataLoader.i18n.Get("Feeding.TreatDaffodil.Letter")
                    , (letter) => !Game1.player.mailReceived.Contains(letter.Id)
                                  && Game1.getLocationFromName("ArchaeologyHouse") is LibraryMuseum libraryMuseum
                                  && libraryMuseum.museumAlreadyHasArtifact(107)
                    , (letter) => Game1.player.mailReceived.Add(letter.Id)
                )
                {
                    GroupId = "AHM.Gunther",
                    Title = DataLoader.i18n.Get("Feeding.TreatDaffodil.Letter.Title")
                }
            );

            MailDao.SaveLetter(
                new Letter(
                    "DinosaursSecondTreat"
                    , DataLoader.i18n.Get("Feeding.TreatCrocus.Letter")
                    , (letter) => !Game1.player.mailReceived.Contains(letter.Id) 
                                  && Game1.getLocationFromName("ArchaeologyHouse") is LibraryMuseum libraryMuseum
                                  && MuseumContainsTheseItems(new int[] { 579, 580, 581, 582, 583, 584, 585 }, new HashSet<int>(libraryMuseum.museumPieces.Values))
                    , (letter) => Game1.player.mailReceived.Add(letter.Id)
                )
                {
                    GroupId = "AHM.Gunther",
                    Title = DataLoader.i18n.Get("Feeding.TreatCrocus.Letter.Title")
                }
            );
        }

        private static bool MuseumContainsTheseItems(IEnumerable<int> items, ISet<int> museumItems)
        {
            return items.All(museumItems.Contains);
        }

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

        public void AddCustomAnimalsTemplateCommand(string command = null, string[] args = null)
        {
            AddCustomAnimalsTemplate();
        }

        public void AddCustomAnimalsTemplate(Dictionary<string, string> data =  null)
        {
            isLoadingFarmAnimals = true;
            data = data ?? Helper.Content.Load<Dictionary<string, string>>("Data\\FarmAnimals", ContentSource.GameContent);
            Dictionary<int, string> objects =  null;
            bool animalDataChanged = false;
            ISet<int> syringeItemsIds = new HashSet<int>();
            
            foreach (KeyValuePair<string, string> farmAnimal in data)
            {
                if (!BaseGameAnimals.Contains(farmAnimal.Key))
                {
                    if (!DataLoader.AnimalData.CustomAnimals.Exists(a => farmAnimal.Key.Contains(a.Name)))
                    {
                        AnimalHusbandryModEntry.monitor.Log($"Creating template in animal.json for {farmAnimal.Key}.", LogLevel.Trace);
                        CustomAnimalItem customAnimalItem = new CustomAnimalItem(farmAnimal.Key);
                        DataLoader.AnimalData.CustomAnimals.Add(customAnimalItem);
                        animalDataChanged = true;
                        int meatIndex = Convert.ToInt32(farmAnimal.Value.Split('/')[23]);
                        objects = objects ?? Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
                        if (objects.ContainsKey(meatIndex))
                        {
                            int meatPrice = Convert.ToInt32(objects[meatIndex].Split('/')[1]);
                            if (meatPrice > 0)
                            {
                                int animalPrice = Convert.ToInt32(farmAnimal.Value.Split('/')[24]);
                                customAnimalItem.MinimalNumberOfMeat = Math.Max(1, (int)Math.Round((animalPrice * 0.3) / meatPrice, MidpointRounding.AwayFromZero));
                                customAnimalItem.MaximumNumberOfMeat = Math.Max(1, (int)Math.Round((animalPrice * 1.3) / meatPrice, MidpointRounding.AwayFromZero));

                            }
                        }
                    }
                }

                if (!ModConfig.DisablePregnancy)
                {
                    AnimalItem animalItem = DataLoader.AnimalData.GetAnimalItem(farmAnimal.Key);
                    if (animalItem is ImpregnatableAnimalItem impregnatableAnimalItem)
                    {
                        try
                        {
                            if (impregnatableAnimalItem.MinimumDaysUtillBirth.HasValue)
                            {
                                syringeItemsIds.Add(Convert.ToInt32(farmAnimal.Value.Split('/')[2]));
                                if (impregnatableAnimalItem.CanUseDeluxeItemForPregnancy)
                                {
                                    syringeItemsIds.Add(Convert.ToInt32(farmAnimal.Value.Split('/')[3]));
                                }
                            }
                        }
                        catch (Exception)
                        {
                            AnimalHusbandryModEntry.monitor.Log($"Item to use in the syringe for {farmAnimal.Key} was not identified.", LogLevel.Warn);
                        }
                    }
                }
            }

            AnimalData.SyringeItemsIds = syringeItemsIds;
            if (animalDataChanged)
            {
                Helper.Data.WriteJsonFile("data\\animals.json", DataLoader.AnimalData);
            }

            isLoadingFarmAnimals = false;
        }

        public void LoadContentPacksCommand(string command = null, string[] args = null)
        {
            try
            {
                foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
                {
                    try
                    {
                        if (File.Exists(Path.Combine(contentPack.DirectoryPath, "customAnimals.json")))
                        {
                            AnimalHusbandryModEntry.monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                            List<CustomAnimalItem> customAnimalItems = contentPack.ReadJsonFile<List<CustomAnimalItem>>("customAnimals.json");
                            foreach (CustomAnimalItem customAnimalItem in customAnimalItems)
                            {
                                DataLoader.AnimalData.CustomAnimals.RemoveAll(c => c.Name.Contains(customAnimalItem.Name));
                                DataLoader.AnimalData.CustomAnimals.Add(customAnimalItem);
                            }
                        }
                        else
                        {
                            AnimalHusbandryModEntry.monitor
                                .Log(
                                    $"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\n" +
                                    $"It does not have an customAnimals.json file."
                                    , LogLevel.Warn);
                        }
                    }
                    catch (Exception ex)
                    {
                        AnimalHusbandryModEntry.monitor
                            .Log(
                                $"Error while trying to load the content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}. It'll be ignored.\n{ex}"
                                , LogLevel.Error);
                    }
                }
            }
            finally
            {
                AnimalData.FillLikedTreatsIds();
            }
        }

        private void CreateConfigMenu(IManifest manifest)
        {
            GenericModConfigMenuApi api = Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.RegisterModConfig(manifest, () => DataLoader.ModConfig = new ModConfig(), () => Helper.WriteConfig(DataLoader.ModConfig));

                api.RegisterLabel(manifest, "Main Features:", "Properties to disable the mod main features.");

                api.RegisterSimpleOption(manifest, "Disable Meat", "Disable all features related to meat. Meat Cleaver/Wand will not be delivered , and if already owned, will not work. Meat items and meat dishes will not be loaded. Any item still on the inventory will be bugged. You should sell/trash all of them before disabling meat. Meat Friday will not show on TV. You will not receive any more meat recipe letter from the villagers. Learned recipes will still be known, but will not show on the cooking menu. If re-enabled, they will show again.", () => DataLoader.ModConfig.DisableMeat, (bool val) => DataLoader.ModConfig.DisableMeat = val);
                
                api.RegisterSimpleOption(manifest, "Disable Pregnancy", "Disable all features related to pregnancy. Syringe will not be delivered, and if already owned, it will not work. Pregnancy status will not update but will not reset. Animals that were pregnant will be with random pregnancy disabled unless changed. If re-enabled, everything will resume as it was before.", () => DataLoader.ModConfig.DisablePregnancy, (bool val) => DataLoader.ModConfig.DisablePregnancy = val);
                
                api.RegisterSimpleOption(manifest, "Disable Treats", "Disable all features related to treats. The basket will not be delivered, and if already owned, it will not work. Treat status will update while the treat feature is disable. Animals that were feed treats before will be able to eat again if the appropriate amount of days has passed when the mod was disabled.", () => DataLoader.ModConfig.DisableTreats, (bool val) => DataLoader.ModConfig.DisableTreats = val);
                
                api.RegisterSimpleOption(manifest, "Disable Animal Contest", "Disable all features related to the animal contest. You won't receive any more participant ribbons. Bonus from previous winners will still apply though.", () => DataLoader.ModConfig.DisableAnimalContest, (bool val) => DataLoader.ModConfig.DisableAnimalContest = val);

                api.RegisterLabel(manifest, "Meat Properties:", "Properties to configure the meat feature.");

                api.RegisterSimpleOption(manifest, "Softmode", "Enable the Softmode. When enabled the Meat Cleaver is replaced with the Meat Want. They work the same, but sound, text and effects are changed to resemble magic.", () => DataLoader.ModConfig.Softmode, (bool val) => DataLoader.ModConfig.Softmode = val);

                api.RegisterSimpleOption(manifest, "Disable Rancher Affect Meat", "Disable the patch that make Rancher Profession work on meat items.", () => DataLoader.ModConfig.DisableRancherMeatPriceAjust, (bool val) => DataLoader.ModConfig.DisableRancherMeatPriceAjust = val);

                api.RegisterSimpleOption(manifest, "Disable Meat In Bundle", "Disable the addition of meat to the Animal Bundle in the Community Center.", () => DataLoader.ModConfig.DisableMeatInBlundle, (bool val) => DataLoader.ModConfig.DisableMeatInBlundle = val);

                api.RegisterSimpleOption(manifest, "Disable Meat From Dinosaur", "Disable dinosaur giving a random kind of meat.", () => DataLoader.ModConfig.DisableMeatFromDinosaur, (bool val) => DataLoader.ModConfig.DisableMeatFromDinosaur = val);

                api.RegisterSimpleOption(manifest, "Disable Meat Tool Letter", "Disable the sending of the meat cleaver or the meat wand. Meat will only be able to be obtained through the meat button.", () => DataLoader.ModConfig.DisableMeatToolLetter, (bool val) => DataLoader.ModConfig.DisableMeatToolLetter = val);

                api.RegisterSimpleOption(manifest, "Add Meat Tool Key", "Set a keyboard key to directly add the Meat Cleaver/Want to your inventory.", () => DataLoader.ModConfig.AddMeatCleaverToInventoryKey ?? SButton.None, (SButton val) => DataLoader.ModConfig.AddMeatCleaverToInventoryKey = val == SButton.None ? (SButton?)null : val);

                api.RegisterLabel(manifest, "Pregnancy Properties:", "Properties to configure the insemination feature.");

                api.RegisterSimpleOption(manifest, "Disable Full Build Notif.", "Disable notifications for when an animals can't give birth because their building is full.", () => DataLoader.ModConfig.DisableFullBuildingForBirthNotification, (bool val) => DataLoader.ModConfig.DisableFullBuildingForBirthNotification = val);
                
                api.RegisterSimpleOption(manifest, "Disable Birth Notif.", "Disable notifications for when an animal will give birth tomorrow.", () => DataLoader.ModConfig.DisableTomorrowBirthNotification, (bool val) => DataLoader.ModConfig.DisableTomorrowBirthNotification = val);

                api.RegisterSimpleOption(manifest, "Add Insemination Syringe Key", "Set a keyboard key to directly add the Insemination Syringe to your inventory.", () => DataLoader.ModConfig.AddInseminationSyringeToInventoryKey ?? SButton.None, (SButton val) => DataLoader.ModConfig.AddInseminationSyringeToInventoryKey = val == SButton.None ? (SButton?)null : val);

                api.RegisterLabel(manifest, "Treats Properties:", "Properties to configure the treats feature.");

                api.RegisterSimpleOption(manifest, "Disable Friendship Increase", "Disable animal friendship being increased when given a treat.", () => DataLoader.ModConfig.DisableFriendshipInscreseWithTreats, (bool val) => DataLoader.ModConfig.DisableFriendshipInscreseWithTreats = val);

                api.RegisterSimpleOption(manifest, "Disable Mood Increase", "Disable animal mood being set to max when given a treat.", () => DataLoader.ModConfig.DisableMoodInscreseWithTreats, (bool val) => DataLoader.ModConfig.DisableMoodInscreseWithTreats = val);

                api.RegisterSimpleOption(manifest, "Enable Treats Count As Feed", "Enable animal feed status being set to max when given a treat.", () => DataLoader.ModConfig.EnableTreatsCountAsAnimalFeed, (bool val) => DataLoader.ModConfig.EnableTreatsCountAsAnimalFeed = val);

                api.RegisterSimpleOption(manifest, "Professions Adjust", "Change the percentage adjust for friendship increase when giving treats when you have the coopmaster or shepherd professions. 0.25 means 25% more than usual.", () => (float)DataLoader.ModConfig.PercentualAjustOnFriendshipInscreaseFromProfessions, (float val) => DataLoader.ModConfig.PercentualAjustOnFriendshipInscreaseFromProfessions = val);

                api.RegisterSimpleOption(manifest, "Add Feeding Basket Key", "Set a keyboard key to directly add the Feeding Basket to your inventory.", () => DataLoader.ModConfig.AddFeedingBasketToInventoryKey ?? SButton.None, (SButton val) => DataLoader.ModConfig.AddFeedingBasketToInventoryKey = val == SButton.None ? (SButton?) null : val);

                api.RegisterLabel(manifest, "Animal Contest Properties:", "Properties to configure the animal contest feature.");

                api.RegisterSimpleOption(manifest, "Disable Contest Bonus", "Disable the fertility and the production bonuses from the contest. If enabled again, all winners will receive the bonus again, no matter if the bonus was disabled when they won.", () => DataLoader.ModConfig.DisableContestBonus, (bool val) => DataLoader.ModConfig.DisableContestBonus = val);

                api.RegisterLabel(manifest, "Misc. Properties:", "Miscellaneous Properties.");

                api.RegisterSimpleOption(manifest, "Force Draw Attachment", "Force the patch that draw the hover menu for the feeding basket and insemination syringe on any OS", () => DataLoader.ModConfig.ForceDrawAttachmentOnAnyOS, (bool val) => DataLoader.ModConfig.ForceDrawAttachmentOnAnyOS = val);

                api.RegisterSimpleOption(manifest, "Disable TV Channels", "Disable all TV channels added by this mod.", () => DataLoader.ModConfig.DisableTvChannels, (bool val) => DataLoader.ModConfig.DisableTvChannels = val);
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
