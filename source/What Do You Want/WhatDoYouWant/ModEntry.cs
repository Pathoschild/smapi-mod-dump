/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/WhatDoYouWant
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.ItemTypeDefinitions;

namespace WhatDoYouWant
{
    public class ModEntry : Mod
    {
        public const string LineBreak = "^";

        private const string ResponseToken_CommunityCenter = "CommunityCenter";
        private const string ResponseToken_GrandpasEvaluation = "GrandpasEvaluation";
        private const string ResponseToken_Walnuts = "Walnuts";
        private const string ResponseToken_Shipping = "Shipping";
        private const string ResponseToken_Cooking = "Cooking";
        private const string ResponseToken_Crafting = "Crafting";
        private const string ResponseToken_Fishing = "Fishing";
        private const string ResponseToken_Museum = "Museum";
        private const string ResponseToken_Stardrops = "Stardrops";
        private const string ResponseToken_Polyculture = "Polyculture";
        private const string ResponseToken_Cancel = "Cancel";

        // Used by Grandpa's Evaluation and some GetTitle_*() functions
        public const int AchievementID_Shipping = 34;
        public const int AchievementID_Cooking = 17;
        public const int AchievementID_Crafting = 22;
        public const int AchievementID_Fishing = 26;
        public const int AchievementID_Museum = 5;
        public const int AchievementID_Polyculture = 31;

        // Used by Community Center and GetIngredientText()
        public const string StringKey_AnyMilk = "Strings\\StringsFromCSFiles:CraftingRecipe.cs.573";
        public const string StringKey_AnyEgg = "Strings\\StringsFromCSFiles:CraftingRecipe.cs.572";
        public const string StringKey_AnyFish = "Strings\\StringsFromCSFiles:CraftingRecipe.cs.571";

        // Used by Community Center and Museum
        private static readonly List<string> ItemsWithCustomDescriptions = new()
        {
            "(O)174", // Strings\\Objects:LargeWhiteEgg_Name "Large Egg"
            "(O)182", // Strings\\Objects:LargeBrownEgg_Name "Large Egg"
            "(O)SmokedFish", // Strings\\Objects:SmokedFish_Name "Smoked" - in other contexts, game adds name of specific fish
            "(O)DriedFruit", // Strings\\Objects:DriedFruit_Name "Dried" - in other contexts, game adds name of specific fruit
            "(O)DriedMushrooms", // Strings\\Objects:DriedMushrooms_Name "Dried"
            "(O)126", // Strings\\Objects:StrangeDoll_Name "Strange Doll" (G)
            "(O)127" // Strings\\Objects:StrangeDoll_Name "Strange Doll" (Y)
        };

        public string GetItemDescription(ParsedItemData? item)
        {
            if (item == null)
            {
                return "???";
            }
            return ItemsWithCustomDescriptions.Contains(item.QualifiedItemId)
                ? Helper.Translation.Get($"ItemDescription_{item.QualifiedItemId}")
                : item.DisplayName;
        }

        // Used by Cooking and Crafting
        private const string ConditionToken_PierresShop = "Pierres_Shop";
        private const string ConditionToken_CarpentersShop = "Carpenters_Shop";
        private const string ConditionToken_Krobus = "Krobus";
        private const string ConditionToken_IslandTrader = "Island_Trader";
        private const string ConditionToken_QisWalnutRoom = "Qis_Walnut_Room";
        private const string ConditionToken_CombatMastery = "Combat_Mastery";
        private const string ConditionToken_ForagingMastery = "Foraging_Mastery";
        private const string ConditionToken_MiningMastery = "Mining_Mastery";

        private static readonly Dictionary<string, string?> RecipesWithCustomConditions = new()
        {
            // cooking
            { "Cookies", null },
            { "Triple Shot Espresso", null },
            { "Ginger Ale", null },
            { "Banana Pudding", ConditionToken_IslandTrader },
            { "Tropical Curry", null },
            // crafting
            { "Ancient Seeds", null },
            { "Anvil", ConditionToken_CombatMastery },
            { "Barrel Brazier", ConditionToken_CarpentersShop },
            { "Big Chest", ConditionToken_CarpentersShop },
            { "Big Stone Chest", null },
            { "Blue Grass Starter", ConditionToken_QisWalnutRoom },
            { "Bone Mill", null },
            { "Brick Floor", ConditionToken_CarpentersShop },
            { "Carved Brazier", ConditionToken_CarpentersShop },
            { "Cask", null },
            { "Challenge Bait", null },
            { "Crystal Floor", ConditionToken_Krobus },
            { "Crystal Path", ConditionToken_CarpentersShop },
            { "Dehydrator", ConditionToken_PierresShop },
            { "Deluxe Fertilizer", ConditionToken_QisWalnutRoom },
            { "Deluxe Retaining Soil", ConditionToken_IslandTrader },
            { "Deluxe Scarecrow", null },
            { "Drum Block", null },
            { "Fairy Dust", null },
            { "Farm Computer", null },
            { "Fiber Seeds", null },
            { "Fish Smoker", null },
            { "Flute Block", null },
            { "Furnace", null },
            { "Garden Pot", null },
            { "Geode Crusher", null },
            { "Gold Brazier", ConditionToken_CarpentersShop },
            { "Grass Starter", ConditionToken_PierresShop },
            { "Heavy Furnace", ConditionToken_MiningMastery },
            { "Heavy Tapper", ConditionToken_QisWalnutRoom },
            { "Hopper", ConditionToken_QisWalnutRoom },
            { "Hyper Speed-Gro", ConditionToken_QisWalnutRoom },
            { "Iron Lamp-post", ConditionToken_CarpentersShop },
            { "Jack-O-Lantern", null },
            { "Magic Bait", ConditionToken_QisWalnutRoom },
            { "Marble Brazier", ConditionToken_CarpentersShop },
            { "Mini-Forge", ConditionToken_IslandTrader },
            { "Mini-Jukebox", null },
            { "Mini-Obelisk", null },
            { "Monster Musk", null },
            { "Mystic Tree Seed", ConditionToken_ForagingMastery },
            { "Ostrich Incubator", null },
            { "Quality Bobber", null },
            { "Rustic Plank Floor", ConditionToken_CarpentersShop },
            { "Skull Brazier", ConditionToken_CarpentersShop },
            { "Solar Panel", null },
            { "Statue Of Blessings", null },
            { "Statue Of The Dwarf King", ConditionToken_MiningMastery },
            { "Stepping Stone Path", ConditionToken_CarpentersShop },
            { "Stone Brazier", ConditionToken_CarpentersShop },
            { "Stone Chest", null },
            { "Stone Floor", ConditionToken_CarpentersShop },
            { "Stone Walkway Floor", ConditionToken_CarpentersShop },
            { "Straw Floor", ConditionToken_CarpentersShop },
            { "Stump Brazier", ConditionToken_CarpentersShop },
            { "Tea Sapling", null },
            { "Treasure Totem", ConditionToken_ForagingMastery },
            { "Tub o' Flowers", null },
            { "Warp Totem: Desert", null },
            { "Warp Totem: Island", null },
            { "Weathered Floor", ConditionToken_CarpentersShop },
            { "Wicked Statue", ConditionToken_Krobus },
            { "Wild Bait", null },
            { "Wood Floor", ConditionToken_CarpentersShop },
            { "Wood Lamp-post", ConditionToken_CarpentersShop },
            { "Wooden Brazier", ConditionToken_CarpentersShop }
        };

        public string GetConditionDescription(string? recipe, string? condition, bool isCooking)
        {
            recipe = recipe ?? "";
            condition = condition ?? "";

            var descriptions = new List<string>();

            // Friendship level
            if (condition.StartsWith("f "))
            {
                descriptions.Add(condition.Substring(2));
            }

            // Skill level
            if (condition.StartsWith("s "))
            {
                condition = condition.Substring(2);
            }
            if (
                condition.StartsWith("Farming ")
                    || condition.StartsWith("Foraging ")
                    || condition.StartsWith("Fishing ")
                    || condition.StartsWith("Mining ")
                    || condition.StartsWith("Combat ")
            )
            {
                var skillName = ArgUtility.Get(condition.Split(' '), 0);
                var skillNumber = ArgUtility.Get(condition.Split(' '), 1);
                var skillDisplayName = Farmer.getSkillDisplayNameFromIndex(Farmer.getSkillNumberFromName(skillName));
                descriptions.Add($"{skillDisplayName} {skillNumber}");
            }

            // Queen of Sauce
            if (isCooking)
            {
                var QueenOfSauceDictionary = DataLoader.Tv_CookingChannel(Game1.temporaryContent);
                foreach (var QueenOfSauceEntry in QueenOfSauceDictionary)
                {
                    if (!QueenOfSauceEntry.Value.StartsWith(recipe + "/"))
                    {
                        continue;
                    }

                    int indexNumber = 0;
                    int.TryParse(QueenOfSauceEntry.Key, out indexNumber); // 1 to 32
                    int day = (((indexNumber - 1) % 4) + 1) * 7; // 1 to 28
                    int season = ((indexNumber - 1) / 4) % 4; // 0 = spring -> 3 = winter
                    int year = (indexNumber - 1) / 16 + 1;
                    var dateString = Utility.getDateStringFor(day, season, year);

                    var queenOfSauceTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13114");

                    descriptions.Add($"{queenOfSauceTitle} - {dateString}");
                }
            }

            // Other stuff not included in standard recipe data
            if (RecipesWithCustomConditions.ContainsKey(recipe))
            {
                var normalizedRecipe = RecipesWithCustomConditions[recipe] ?? recipe.Replace(" ", "_");
                descriptions.Add(Helper.Translation.Get($"RecipeCondition_{normalizedRecipe}"));
            }

            if (descriptions.Count == 0)
            {
                descriptions.Add("???");
            }

            return string.Join(", ", descriptions);
        }

        // Season logic used by Master Angler and Polyculture

        public static readonly List<Season> seasons = new()
        {
            Season.Spring,
            Season.Summer,
            Season.Fall,
            Season.Winter
        };

        private static Season GetNextSeason(Season season)
        {
            switch (season)
            {
                case Season.Spring:
                    return Season.Summer;
                case Season.Summer:
                    return Season.Fall;
                case Season.Fall:
                    return Season.Winter;
                case Season.Winter:
                    return Season.Spring;
                default: // should never happen
                    return Season.Spring;
            }
        }

        public static List<Season> GetSeasons(bool currentFirst = false)
        {
            if (!currentFirst)
            {
                return seasons;
            }

            var seasonsCurrentFirst = new List<Season>();
            var season = Game1.season;
            seasonsCurrentFirst.Add(season);
            for (int i = 1; i <= 3; ++i)
            {
                season = GetNextSeason(season);
                seasonsCurrentFirst.Add(season);
            }
            return seasonsCurrentFirst;
        }

        public string GetSeasonsDescription(List<Season> seasons, List<Season> seasonsSortOrderList)
        {
            if (seasons.Count == 4)
            {
                return Helper.Translation.Get("AllSeasons");
            }
            var seasonsList = new List<string>();
            foreach (var season in seasonsSortOrderList)
            {
                if (seasons.Contains(season))
                {
                    seasonsList.Add(Utility.getSeasonNameFromNumber((int)season));
                }
            }
            return String.Join(", ", seasonsList);
        }

        public static int GetSeasonsSortOrder(List<Season> seasons, List<Season> seasonsSortOrderList)
        {
            // Prioritize by what's available soonest, break ties by length of wait if a season is missed
            //   e.g. Polyculture, spring first: Parsnip (Spring) is ahead of Coffee (Spring, Summer), which is ahead of Blueberry (Summer)
            var sortOrderBits = 0;
            for (var seasonIndex = 0; seasonIndex < 4; ++seasonIndex)
            {
                if (seasons.Contains(seasonsSortOrderList[seasonIndex]))
                {
                    sortOrderBits += 1 << (3 - seasonIndex);
                }
            }
            switch (sortOrderBits)
            {
                case 8: return 1; // 1000
                case 9: return 2; // 1001
                case 10: return 3; // 1010
                case 11: return 4; // 1011
                case 12: return 5; // 1100
                case 13: return 6; // 1101
                case 14: return 7; // 1110
                case 15: return 8; // 1111
                case 4: return 9; // 0100
                case 5: return 10; // 0101
                case 6: return 11; // 0110
                case 7: return 12; // 0111
                case 2: return 13; // 0010
                case 3: return 14; // 0011
                case 1: return 15; // 0001
                default: return 16; // 0000, i.e. we couldn't figure out seasons
            }
        }

        private static string GetAchievementTitle(int achievementId)
        {
            // array elements: e.g. "A Complete Collection^Complete the museum collection.^true^28^0"
            return Game1.achievements[achievementId].Split(LineBreak)[0];
        }

        public static string GetTitle_CommunityCenter()
        {
            return Game1.content.LoadString("Strings\\UI:GameMenu_JunimoNote_Hover");
        }

        public string GetTitle_GrandpasEvaluation()
        {
            return Helper.Translation.Get("Menu_GrandpasEvaluation");
        }

        public string GetTitle_Walnuts() {
            return Helper.Translation.Get("Menu_Walnuts");
        }

        public static string GetTitle_Shipping()
        {
            return GetAchievementTitle(AchievementID_Shipping);
        }

        public static string GetTitle_Cooking()
        {
            return GetAchievementTitle(AchievementID_Cooking);
        }

        public static string GetTitle_Crafting()
        {
            return GetAchievementTitle(AchievementID_Crafting);
        }

        public static string GetTitle_Fishing()
        {
            return GetAchievementTitle(AchievementID_Fishing);
        }

        public static string GetTitle_Museum()
        {
            return GetAchievementTitle(AchievementID_Museum);
        }

        public string GetTitle_Stardrops()
        {
            return Helper.Translation.Get("Menu_Stardrops");
        }

        public static string GetTitle_Polyculture()
        {
            return GetAchievementTitle(AchievementID_Polyculture);
        }

        public ModConfig Config = new();

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            Helper.Events.GameLoop.GameLaunched += (_sender, _e) => OnGameLaunched(_sender, _e);
            Helper.Events.Input.ButtonsChanged += (_sender, _e) => ButtonsChanged(_sender, _e);
        }

        private void OnGameLaunched(object? _sender, GameLaunchedEventArgs _e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddKeybindList(
                mod: this.ModManifest,
                getValue: () => this.Config.WhatDoYouWantKeypress,
                setValue: value => this.Config.WhatDoYouWantKeypress = value,
                name: () => Helper.Translation.Get("Options_OpenMenuKey")
            );

            // TODO Community Center

            // TODO Grandpa's Evaluation

            // TODO Golden Walnuts

            // Full Shipment

            configMenu.AddTextOption(
                mod: this.ModManifest,
                getValue: () => this.Config.ShippingSortOrder,
                setValue: value => this.Config.ShippingSortOrder = value,
                name: () => Helper.Translation.Get("Options_ShippingSortOrder"),
                allowedValues: new string[] {
                    Shipping.SortOrder_Category,
                    Shipping.SortOrder_ItemName,
                    Shipping.SortOrder_CollectionsTab
                },
                formatAllowedValue: value => Helper.Translation.Get($"Options_ShippingSortOrder_{value}")
            );

            // Gourmet Chef

            configMenu.AddTextOption(
                mod: this.ModManifest,
                getValue: () => this.Config.CookingSortOrder,
                setValue: value => this.Config.CookingSortOrder = value,
                name: () => Helper.Translation.Get("Options_CookingSortOrder"),
                allowedValues: new string[] {
                    Cooking.SortOrder_KnownRecipesFirst,
                    Cooking.SortOrder_RecipeName,
                    Cooking.SortOrder_CollectionsTab
                },
                formatAllowedValue: value => Helper.Translation.Get($"Options_CookingSortOrder_{value}")
            );

            // Craft Master

            configMenu.AddTextOption(
                mod: this.ModManifest,
                getValue: () => this.Config.CraftingSortOrder,
                setValue: value => this.Config.CraftingSortOrder = value,
                name: () => Helper.Translation.Get("Options_CraftingSortOrder"),
                allowedValues: new string[] {
                    Crafting.SortOrder_KnownRecipesFirst,
                    Crafting.SortOrder_RecipeName,
                    Crafting.SortOrder_CraftingMenu
                },
                formatAllowedValue: value => Helper.Translation.Get($"Options_CraftingSortOrder_{value}")
            );

            // Master Angler

            configMenu.AddTextOption(
                mod: this.ModManifest,
                getValue: () => this.Config.FishingSortOrder,
                setValue: value => this.Config.FishingSortOrder = value,
                name: () => Helper.Translation.Get("Options_FishingSortOrder"),
                allowedValues: new string[] {
                    Fishing.SortOrder_SeasonsSpringFirst,
                    Fishing.SortOrder_SeasonsCurrentFirst,
                    Fishing.SortOrder_FishName,
                    Fishing.SortOrder_CollectionsTab
                },
                formatAllowedValue: value => Helper.Translation.Get($"Options_FishingSortOrder_{value}")
            );

            // A Complete Collection

            configMenu.AddTextOption(
                mod: this.ModManifest,
                getValue: () => this.Config.MuseumSortOrder,
                setValue: value => this.Config.MuseumSortOrder = value,
                name: () => Helper.Translation.Get("Options_MuseumSortOrder"),
                allowedValues: new string[] {
                    Museum.SortOrder_Type,
                    Museum.SortOrder_ItemName,
                    Museum.SortOrder_CollectionsTabs
                },
                formatAllowedValue: value => Helper.Translation.Get($"Options_MuseumSortOrder_{value}")
            );

            // TODO Stardrops

            // Polyculture

            configMenu.AddTextOption(
                mod: this.ModManifest,
                getValue: () => this.Config.PolycultureSortOrder,
                setValue: value => this.Config.PolycultureSortOrder = value,
                name: () => Helper.Translation.Get("Options_PolycultureSortOrder"),
                allowedValues: new string[] {
                    Polyculture.SortOrder_SeasonsSpringFirst,
                    Polyculture.SortOrder_SeasonsCurrentFirst,
                    Polyculture.SortOrder_CropName,
                    Polyculture.SortOrder_NumberNeeded
                },
                formatAllowedValue: value => Helper.Translation.Get($"Options_PolycultureSortOrder_{value}")
            );
        }

        private void ButtonsChanged(object? _sender, ButtonsChangedEventArgs _e)
        {
            if (!Config.WhatDoYouWantKeypress.JustPressed())
            {
                return;
            }

            if (!Game1.hasStartedDay)
            {
                return;
            }

            // TODO default to omitting things already completed or unavailable (check mail / achievement data when possible), option to include them anyway
            //   * CC if already completed or bought Joja membership
            //   * walnuts if island not yet unlocked
            List<Response> responseList = new();
            responseList.Add(new Response(responseKey: ResponseToken_CommunityCenter, responseText: GetTitle_CommunityCenter()));
            responseList.Add(new Response(responseKey: ResponseToken_GrandpasEvaluation, responseText: GetTitle_GrandpasEvaluation()));
            responseList.Add(new Response(responseKey: ResponseToken_Walnuts, responseText: GetTitle_Walnuts()));
            responseList.Add(new Response(responseKey: ResponseToken_Shipping, responseText: GetTitle_Shipping()));
            responseList.Add(new Response(responseKey: ResponseToken_Cooking, responseText: GetTitle_Cooking()));
            responseList.Add(new Response(responseKey: ResponseToken_Crafting, responseText: GetTitle_Crafting()));
            responseList.Add(new Response(responseKey: ResponseToken_Fishing, responseText: GetTitle_Fishing()));
            responseList.Add(new Response(responseKey: ResponseToken_Museum, responseText: GetTitle_Museum()));
            responseList.Add(new Response(responseKey: ResponseToken_Stardrops, responseText: GetTitle_Stardrops()));
            responseList.Add(new Response(responseKey: ResponseToken_Polyculture, responseText: GetTitle_Polyculture()));
            responseList.Add(new Response(responseKey: ResponseToken_Cancel, responseText: "(" + Helper.Translation.Get("Menu_Cancel") + ")"));
            Game1.currentLocation.createQuestionDialogue(
              question: Helper.Translation.Get("Menu_Question"),
              answerChoices: responseList.ToArray(),
              afterDialogueBehavior: new GameLocation.afterQuestionBehavior(this.GotResponse)
            );
        }

        public virtual void GotResponse(Farmer who, string answer)
        {
            switch (answer)
            {
                case ResponseToken_CommunityCenter:
                    CommunityCenter.ShowCommunityCenterList(modInstance: this);
                    break;
                case ResponseToken_GrandpasEvaluation:
                    GrandpasEvaluation.ShowGrandpasEvaluationList(modInstance: this);
                    break;
                case ResponseToken_Walnuts:
                    Walnuts.ShowWalnutsList(modInstance: this);
                    break;
                case ResponseToken_Shipping:
                    Shipping.ShowShippingList(modInstance: this, who: who);
                    break;
                case ResponseToken_Cooking:
                    Cooking.ShowCookingList(modInstance: this, who: who);
                    break;
                case ResponseToken_Crafting:
                    Crafting.ShowCraftingList(modInstance: this, who: who);
                    break;
                case ResponseToken_Fishing:
                    Fishing.ShowFishingList(modInstance: this, who: who);
                    break;
                case ResponseToken_Museum:
                    Museum.ShowMuseumList(modInstance: this);
                    break;
                case ResponseToken_Stardrops:
                    Stardrops.ShowStardropList(modInstance: this, who: who);
                    break;
                case ResponseToken_Polyculture:
                    Polyculture.ShowPolycultureList(modInstance: this);
                    break;
                case ResponseToken_Cancel:
                    break;
                default: // should never happen
                    Game1.drawDialogueNoTyping(Helper.Translation.Get("Response_NotYetImplemented"));
                    break;
            }
        }

        // used by cooking and crafting
        public static string GetIngredientText(string ingredients)
        {
            var ingredientList = ingredients.Trim().Split(' ');
            var ingredientTextList = new List<string>();
            for (var index = 0; index < ingredientList.Length; index += 2)
            {
                var ingredientId = ingredientList[index];
                string ingredientName;
                switch (ingredientId)
                {
                    case Cooking.CookingIngredient_AnyMilk:
                        ingredientName = Game1.content.LoadString(StringKey_AnyMilk);
                        break;
                    case Cooking.CookingIngredient_AnyEgg:
                        ingredientName = Game1.content.LoadString(StringKey_AnyEgg);
                        break;
                    case Cooking.CookingIngredient_AnyFish:
                        ingredientName = Game1.content.LoadString(StringKey_AnyFish);
                        break;
                    default:
                        var ingredientDataOrErrorItem = ItemRegistry.GetDataOrErrorItem(ingredientId);
                        ingredientName = ingredientDataOrErrorItem.DisplayName;
                        break;
                }

                var ingredientQuantity = ArgUtility.GetInt(ingredientList, index + 1, 1);
                if (ingredientQuantity != 1)
                {
                    ingredientName += $" x{ingredientQuantity}";
                }

                ingredientTextList.Add(ingredientName);
            }

            return String.Join(", ", ingredientTextList);
        }

        // non-static because Monitor is tied to the instance
        public void ShowLines(List<string> linesToDisplay, string? title = null, bool longLinesExpected = false, bool longerLinesExpected = false)
        {
            // Log output - can be copy/pasted from the SMAPI window while game is running, or from the log after it's closed
            if (title != null)
            {
                Monitor.Log(title, LogLevel.Info);
                foreach (var line in linesToDisplay)
                {
                    Monitor.Log(line.Replace(LineBreak, ""), LogLevel.Info);
                }
            }

            // Display output in-game
            // adapted from base game logic to display Perfection Tracker output
            var sectionsOfHeight = SpriteText.getStringBrokenIntoSectionsOfHeight(
                s: string.Concat(linesToDisplay),
                width: 9999,
                height: longerLinesExpected
                    ? Game1.uiViewport.Height / 3
                    : (longLinesExpected
                        ? Game1.uiViewport.Height / 2
                        : Game1.uiViewport.Height - 100
                    )
            );
            if (sectionsOfHeight.Count > 1)
            {
                for (var index = 0; index < sectionsOfHeight.Count; ++index)
                {
                    var sectionDescription = Helper.Translation.Get("Response_Section", new { section = index + 1, numberSections = sectionsOfHeight.Count });
                    sectionsOfHeight[index] += $"({sectionDescription})\n";
                }
            }

            Game1.drawDialogueNoTyping(sectionsOfHeight);
        }

    }
}
