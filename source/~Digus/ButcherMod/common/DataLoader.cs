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
using System.Collections.Immutable;
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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.FarmAnimals;
using StardewValley.GameData.Objects;
using StardewValley.Locations;

namespace AnimalHusbandryMod.common
{
    public class DataLoader
    {
        private static readonly string[] MeatDishes = { "652", "653", "654", "655", "656", "657", "658", "659", "660", "661", "662", "663", "664", "665", "666" };

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
        public static String ToolsSpritesName;

        public static ToolsLoader ToolsLoader;
        public RecipesLoader RecipeLoader { get; }

        public LivingWithTheAnimalsChannel LivingWithTheAnimalsChannel { get; }

        public static bool isLoadingFarmAnimals =  false;

        public static IDynamicGameAssetsApi DgaApi;

        public DataLoader(IModHelper helper, IManifest manifest)
        {
            Helper = helper;
            ModConfig = helper.ReadConfig<ModConfig>();
            i18n = Helper.Translation;
            DgaApi = DataLoader.Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");

            LooseSpritesName = Helper.ModContent.GetInternalAssetName("common/LooseSprites.png").Name;         
            LooseSprites = Helper.ModContent.Load<Texture2D>("common/LooseSprites.png");

            ToolsLoader = new ToolsLoader(Helper);               
            
            RecipeLoader = new RecipesLoader();

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
            Helper.Events.Content.AssetRequested += this.Edit;

            var eventLoader = new EventsLoader();
            Helper.Events.Content.AssetRequested += eventLoader.Edit;

            Helper.Events.Content.AssetRequested += RecipeLoader.Edit;

            InvalidateCache();
            ToolsLoader.LoadMail();
            RecipesLoader.LoadMails();
            LoadTreatsMail();
            CreateConfigMenu(manifest);
        }

        private static void InvalidateCache()
        {
            Helper.GameContent.InvalidateCache("Data/Objects");
            Helper.GameContent.InvalidateCache("Data/FarmAnimals");
            Helper.GameContent.InvalidateCache("Data/CookingRecipes");
            Helper.GameContent.InvalidateCache("Data/Tools");
            Helper.GameContent.InvalidateCache("Data/Bundles");
            Helper.GameContent.InvalidateCache("Data/NPCGiftTastes");
        }

        public void Edit(object sender, AssetRequestedEventArgs args)
        {
            if (args.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                args.Edit(asset =>
                {
                    if (ModConfig.DisableMeat) return;
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    //MEAT
                    MeatData = DataLoader.Helper.Data.ReadJsonFile<MeatData>("data\\meats.json") ?? new MeatData();
                    DataLoader.Helper.Data.WriteJsonFile("data\\meats.json", MeatData);

                    data[((int)Meat.Beef).ToString()] = Meat.Beef.GetObjectData();
                    data[((int)Meat.Pork).ToString()] = Meat.Pork.GetObjectData();
                    data[((int)Meat.Chicken).ToString()] = Meat.Chicken.GetObjectData();
                    data[((int)Meat.Duck).ToString()] = Meat.Duck.GetObjectData();
                    data[((int)Meat.Rabbit).ToString()] = Meat.Rabbit.GetObjectData();
                    data[((int)Meat.Mutton).ToString()] = Meat.Mutton.GetObjectData();
                    data[((int)Meat.Ostrich).ToString()] = Meat.Ostrich.GetObjectData();

                    //COOKING

                    data[((int)Cooking.Meatloaf).ToString()] = Cooking.Meatloaf.GetObjectData();
                    data[((int)Cooking.OrangeChicken).ToString()] = Cooking.OrangeChicken.GetObjectData();
                    data[((int)Cooking.MonteCristo).ToString()] = Cooking.MonteCristo.GetObjectData();
                    data[((int)Cooking.BaconCheeseburger).ToString()] = Cooking.BaconCheeseburger.GetObjectData();
                    data[((int)Cooking.RoastDuck).ToString()] = Cooking.RoastDuck.GetObjectData();
                    data[((int)Cooking.RabbitAuVin).ToString()] = Cooking.RabbitAuVin.GetObjectData();
                    data[((int)Cooking.SteakFajitas).ToString()] = Cooking.SteakFajitas.GetObjectData();
                    data[((int)Cooking.GlazedHam).ToString()] = Cooking.GlazedHam.GetObjectData();
                    data[((int)Cooking.SummerSausage).ToString()] = Cooking.SummerSausage.GetObjectData();
                    data[((int)Cooking.SweetAndSourPork).ToString()] = Cooking.SweetAndSourPork.GetObjectData();
                    data[((int)Cooking.RabbitStew).ToString()] = Cooking.RabbitStew.GetObjectData();
                    data[((int)Cooking.WinterDuck).ToString()] = Cooking.WinterDuck.GetObjectData();
                    data[((int)Cooking.SteakWithMushrooms).ToString()] = Cooking.SteakWithMushrooms.GetObjectData();
                    data[((int)Cooking.CowboyDinner).ToString()] = Cooking.CowboyDinner.GetObjectData();
                    data[((int)Cooking.Bacon).ToString()] = Cooking.Bacon.GetObjectData();
                });
                
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/Bundles"))
            {
                args.Edit(asset =>
                {
                    if (ModConfig.DisableMeatInBlundle || ModConfig.DisableMeat) return;
                    var data = asset.AsDictionary<string, string>().Data;
                    string value = data["Pantry/4"];
                    if (!value.Contains("639 1 0 640 1 0 641 1 0 642 1 0 643 1 0 644 1 0") && value.Contains("/4/5"))
                    {
                        value = value.Insert(value.LastIndexOf("/4/5"), " 639 1 0 640 1 0 641 1 0 642 1 0 643 1 0 644 1 0");
                    }

                    data["Pantry/4"] = value;

                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/NPCGiftTastes"))
            {
                args.Edit(asset =>
                {
                    if (ModConfig.DisableMeat) return;
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
                    AddNpcGiftTaste(data, "Alex", Taste.Love, "662", "664");
                    AddNpcGiftTaste(data, "Alex", Taste.Like, "639");
                    AddNpcGiftTaste(data, "Shane", Taste.Hate, "641", "653");
                    AddNpcGiftTaste(data, "Shane", Taste.Love, "655", "658");
                    AddNpcGiftTaste(data, "Leah", Taste.Love, "650");
                    AddNpcGiftTaste(data, "Leah", Taste.Hate, "644");
                    AddNpcGiftTaste(data, "Leah", Taste.Dislike, "655", "660", "666");
                    AddNpcGiftTaste(data, "Harvey", Taste.Dislike, "654", "655");
                    AddNpcGiftTaste(data, "Harvey", Taste.Love, "657", "664");
                    AddNpcGiftTaste(data, "Sam", Taste.Love, "655", "658");
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
                });
            }
            else if (args.NameWithoutLocale.IsEquivalentTo("Data/FarmAnimals"))
            {
                args.Edit(asset =>
                {
                    AddCustomAnimalsTemplate((IDictionary<string, FarmAnimalData>)(isLoadingFarmAnimals ? asset.Data : null));
                });
            }
        }
 
        private static void LoadTreatsMail()
        {
            MailRepository.SaveLetter(
                new Letter(
                    "DinosaursFirstTreat"
                    , DataLoader.i18n.Get("Feeding.TreatDaffodil.Letter")
                    , (letter) => !Game1.player.mailReceived.Contains(letter.Id) && !Game1.player.mailReceived.Contains("DinosaursFirtTreat")
                                  && LibraryMuseum.HasDonatedArtifact("107")
                                  && !ModConfig.DisableTreats
                    , (letter) => Game1.player.mailReceived.Add(letter.Id)
                )
                {
                    GroupId = "AHM.Gunther",
                    Title = DataLoader.i18n.Get("Feeding.TreatDaffodil.Letter.Title")
                }
            ); ;

            MailRepository.SaveLetter(
                new Letter(
                    "DinosaursSecondTreat"
                    , DataLoader.i18n.Get("Feeding.TreatCrocus.Letter")
                    , (letter) => !Game1.player.mailReceived.Contains(letter.Id) 
                                  && MuseumContainsTheseItems(new string[] { "579", "580", "581", "582", "583", "584", "585" })
                                  && !ModConfig.DisableTreats
                    , (letter) => Game1.player.mailReceived.Add(letter.Id)
                )
                {
                    GroupId = "AHM.Gunther",
                    Title = DataLoader.i18n.Get("Feeding.TreatCrocus.Letter.Title")
                }
            );
        }

        private static bool MuseumContainsTheseItems(IEnumerable<string> items)
        {
            return items.All(LibraryMuseum.HasDonatedArtifact);
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

        public void AddCustomAnimalsTemplate(IDictionary<string, FarmAnimalData> data =  null)
        {
            isLoadingFarmAnimals = true;
            bool animalDataChanged = false;
            data ??= Game1.farmAnimalData;
            data ??= ImmutableDictionary<string, FarmAnimalData>.Empty;
            HashSet<string> syringeItemsIds = new();
            
            foreach (KeyValuePair<string, FarmAnimalData> farmAnimal in data)
            {
                if (!AnimalData.BaseGameAnimals.Contains(farmAnimal.Key))
                {
                    if (!DataLoader.AnimalData.CustomAnimals.Exists(a => farmAnimal.Key.Contains(a.Name)))
                    {
                        AnimalHusbandryModEntry.monitor.Log($"Creating template in animal.json for {farmAnimal.Key}.", LogLevel.Trace);
                        CustomAnimalItem customAnimalItem = new CustomAnimalItem(farmAnimal.Key);
                        DataLoader.AnimalData.CustomAnimals.Add(customAnimalItem);
                        animalDataChanged = true;                        
                        if (farmAnimal.Value.CustomFields != null && farmAnimal.Value.CustomFields.TryGetValue("meatIndex", out string meatIndex))
                        {
                            if (Game1.objectData.TryGetValue(meatIndex, out ObjectData meatItem))
                            {
                                int meatPrice = Convert.ToInt32(meatItem.Price);
                                if (meatPrice > 0)
                                {
                                    int animalPrice = farmAnimal.Value.SellPrice;
                                    customAnimalItem.MinimalNumberOfMeat = Math.Max(1, (int)Math.Round((animalPrice * 0.3) / meatPrice, MidpointRounding.AwayFromZero));
                                    customAnimalItem.MaximumNumberOfMeat = Math.Max(1, (int)Math.Round((animalPrice * 1.3) / meatPrice, MidpointRounding.AwayFromZero));

                                }
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
                                syringeItemsIds.UnionWith(farmAnimal.Value.ProduceItemIds.Select(i => i.ItemId).ToHashSet());
                                if (impregnatableAnimalItem.CanUseDeluxeItemForPregnancy)
                                {
                                    syringeItemsIds.UnionWith(farmAnimal.Value.DeluxeProduceItemIds.Select(i => i.ItemId).ToHashSet());
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
            if (api == null) return;

            api.Register(manifest, () => DataLoader.ModConfig = new ModConfig(), () =>
            {
                Helper.WriteConfig(DataLoader.ModConfig);
                InvalidateCache();
            });

            api.AddSectionTitle(manifest, () => "Main Features:", () => "Properties to disable the mod main features.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMeat, (bool val) => DataLoader.ModConfig.DisableMeat = val, () => "Disable Meat", () => "Disable all features related to meat. Meat Cleaver/Wand will not be delivered , and if already owned, will not work. Meat items and meat dishes will not be loaded. Any item still on the inventory will be bugged. You should sell/trash all of them before disabling meat. Meat Friday will not show on TV. You will not receive any more meat recipe letter from the villagers. Learned recipes will still be known, but will not show on the cooking menu. If re-enabled, they will show again. Restart the game after changing this.");
                
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisablePregnancy, (bool val) => DataLoader.ModConfig.DisablePregnancy = val, () => "Disable Pregnancy", () => "Disable all features related to pregnancy. Syringe will not be delivered, and if already owned, it will not work. Pregnancy status will not update but will not reset. Animals that were pregnant will be with random pregnancy disabled unless changed. If re-enabled, everything will resume as it was before. Restart the game after changing this.");
                
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableTreats, (bool val) => DataLoader.ModConfig.DisableTreats = val, () => "Disable Treats", () => "Disable all features related to treats. The basket will not be delivered, and if already owned, it will not work. Treat status will update while the treat feature is disable. Animals that were feed treats before will be able to eat again if the appropriate amount of days has passed when the mod was disabled. Restart the game after changing this.");
                
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableAnimalContest, (bool val) => DataLoader.ModConfig.DisableAnimalContest = val, () => "Disable Animal Contest", () => "Disable all features related to the animal contest. You won't receive any more participant ribbons. Bonus from previous winners will still apply though. Restart the game after changing this.");

            api.AddSectionTitle(manifest, () => "Meat Properties:", () => "Properties to configure the meat feature.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.Softmode, (bool val) => DataLoader.ModConfig.Softmode = val, () => "Softmode", () => "Enable the Softmode. When enabled the Meat Cleaver is replaced with the Meat Want. They work the same, but sound, text and effects are changed to resemble magic. Restart the game after changing this.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableRancherMeatPriceAjust, (bool val) => DataLoader.ModConfig.DisableRancherMeatPriceAjust = val, () => "Disable Rancher Affect Meat", () => "Disable the patch that make Rancher Profession work on meat items.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMeatInBlundle, (bool val) => DataLoader.ModConfig.DisableMeatInBlundle = val, () => "Disable Meat In Bundle", () => "Disable the addition of meat to the Animal Bundle in the Community Center. Needs to start a new game so it can take effect.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMeatFromDinosaur, (bool val) => DataLoader.ModConfig.DisableMeatFromDinosaur = val, () => "Disable Meat From Dinosaur", () => "Disable dinosaur giving a random kind of meat.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMeatToolLetter, (bool val) => DataLoader.ModConfig.DisableMeatToolLetter = val, () => "Disable Meat Tool Letter", () => "Disable the sending of the meat cleaver or the meat wand. Meat will only be able to be obtained through the meat button.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMeatButton, (bool val) => DataLoader.ModConfig.DisableMeatButton = val, () => "Disable Meat Button", () => "Disable the meat button showing in the animal query menu. Meat will only be able to be obtained using the tools.");

            api.AddKeybind(manifest, () => DataLoader.ModConfig.AddMeatCleaverToInventoryKey ?? SButton.None, (SButton val) => DataLoader.ModConfig.AddMeatCleaverToInventoryKey = val == SButton.None ? (SButton?)null : val, () => "Add Meat Tool Key", () => "Set a keyboard key to directly add the Meat Cleaver/Want to your inventory.");

            api.AddSectionTitle(manifest, () => "Pregnancy Properties:", () => "Properties to configure the insemination feature.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableFullBuildingForBirthNotification, (bool val) => DataLoader.ModConfig.DisableFullBuildingForBirthNotification = val, () => "Disable Full Build Notif.", () => "Disable notifications for when an animals can't give birth because their building is full.");
                
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableTomorrowBirthNotification, (bool val) => DataLoader.ModConfig.DisableTomorrowBirthNotification = val, () => "Disable Birth Notif.", () => "Disable notifications for when an animal will give birth tomorrow.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableInseminationSyringeLetter, (bool val) => DataLoader.ModConfig.DisableInseminationSyringeLetter = val, () => "Disable Insemination Letter", () => "Disable the sending of the Insemination Syringe. You will need to get the syringe another way.");

            api.AddKeybind(manifest, () => DataLoader.ModConfig.AddInseminationSyringeToInventoryKey ?? SButton.None, (SButton val) => DataLoader.ModConfig.AddInseminationSyringeToInventoryKey = val == SButton.None ? (SButton?)null : val, () => "Add Insemination Syringe Key", () => "Set a keyboard key to directly add the Insemination Syringe to your inventory.");

            api.AddSectionTitle(manifest, () => "Treats Properties:", () => "Properties to configure the treats feature.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableFriendshipInscreseWithTreats, (bool val) => DataLoader.ModConfig.DisableFriendshipInscreseWithTreats = val, () => "Disable Friendship Increase", () => "Disable animal friendship being increased when given a treat.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMoodInscreseWithTreats, (bool val) => DataLoader.ModConfig.DisableMoodInscreseWithTreats = val, () => "Disable Mood Increase", () => "Disable animal mood being set to max when given a treat.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableTreatsCountAsAnimalFeed, (bool val) => DataLoader.ModConfig.EnableTreatsCountAsAnimalFeed = val, () => "Enable Treats Count As Feed", () => "Enable animal feed status being set to max when given a treat.");

            api.AddNumberOption(manifest, () => (float)DataLoader.ModConfig.PercentualAjustOnFriendshipInscreaseFromProfessions, (float val) => DataLoader.ModConfig.PercentualAjustOnFriendshipInscreaseFromProfessions = val, () => "Professions Adjust", () => "Change the percentage adjust for friendship increase when giving treats when you have the coopmaster or shepherd professions. 0.25 means 25% more than usual.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableFeedingBasketLetter, (bool val) => DataLoader.ModConfig.DisableFeedingBasketLetter = val, () => "Disable Feeding Basket Letter", () => "Disable the sending of the Feeding Basket. You will need to get the basket another way.");

            api.AddKeybind(manifest, () => DataLoader.ModConfig.AddFeedingBasketToInventoryKey ?? SButton.None, (SButton val) => DataLoader.ModConfig.AddFeedingBasketToInventoryKey = val == SButton.None ? (SButton?)null : val, () => "Add Feeding Basket Key", () => "Set a keyboard key to directly add the Feeding Basket to your inventory.");

            api.AddSectionTitle(manifest, () => "Animal Contest Properties:", () => "Properties to configure the animal contest feature.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableContestBonus, (bool val) => DataLoader.ModConfig.DisableContestBonus = val, () => "Disable Contest Bonus", () => "Disable the fertility and the production bonuses from the contest. If enabled again, all winners will receive the bonus again, no matter if the bonus was disabled when they won.");

            api.AddSectionTitle(manifest, () => "Misc. Properties:", () => "Miscellaneous Properties.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.ForceDrawAttachmentOnAnyOS, (bool val) => DataLoader.ModConfig.ForceDrawAttachmentOnAnyOS = val, () => "Force Draw Attachment", () => "Force the patch that draw the hover menu for the feeding basket and insemination syringe on any OS. Restart the game after changing this.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableTvChannels, (bool val) => DataLoader.ModConfig.DisableTvChannels = val, () => "Disable TV Channels", () => "Disable all TV channels added by this mod.");
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
