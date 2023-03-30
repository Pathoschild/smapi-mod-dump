/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Circuit.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;

namespace Circuit
{
    public class TaskManager
    {
        public HashSet<CircuitTask> IncompleteTasks { get; private set; }

        public HashSet<CircuitTask> CompleteTasks { get; private set; } = new();

        public IReadOnlyCollection<CircuitTask> OriginalTaskOrder { get; }

        public PointDisplayMenu PointsMenu { get; private set; } = new(0);

        private int[] FishingTackleIds { get; } = new int[] { 686, 687, 694, 695, 693, 691, 877 };

        private int[] FlowerIds { get; } = new int[] { 18, 402, 418 };

        private int[] GemIds { get; } = new int[] { 60, 62, 64, 66, 68, 70, 72, 74 };

        private int[] SeasonalSeedsIds { get; } = new int[] { 495, 496, 497, 498 };

        private int[] RarecrowIds { get; } = new int[] { 110, 113, 126, 136, 137, 138, 139, 140 };

        private int[] SecretStatueIds { get; } = new int[] { 155, 161, 162 };

        private int[] PartyHatIds { get; } = new int[] { 57, 58, 59 };

        private HashSet<int> RarecrowsCollected { get; } = new();

        private HashSet<int> SeasonalSeedsCrafted { get; } = new();

        private HashSet<int> PartyHatsEquipped { get; } = new();

        private Random Random { get; }

        public TaskManager(Random? random = null)
        {
            random ??= Game1.random;
            Random = random;

            IncompleteTasks = CircuitTasks.FetchTasks(easy: 20, medium: 15, hard: 12, expert: 3, random: Random);

            OriginalTaskOrder = new HashSet<CircuitTask>(IncompleteTasks);

            Logger.Log($"TM: {IncompleteTasks.Count} tasks", LogLevel.Debug);
        }

        public void BindEvents(IModEvents events)
        {
            events.GameLoop.DayStarted += OnDayStarted;
            events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;

            Game1.onScreenMenus.Add(PointsMenu);

            Logger.Log($"TM: SMAPI events bound", LogLevel.Debug);
        }

        public void UnbindEvents(IModEvents events)
        {
            events.GameLoop.DayStarted -= OnDayStarted;
            events.GameLoop.OneSecondUpdateTicked -= OnOneSecondUpdateTicked;

            Game1.onScreenMenus.Remove(PointsMenu);

            Logger.Log($"TM: SMAPI events unbound", LogLevel.Debug);
        }

        public bool IsComplete(CircuitTask task)
        {
            return !IncompleteTasks.Contains(task);
        }

        public void MarkComplete(CircuitTask task)
        {
            if (IsComplete(task) || (!CompleteTasks.Contains(task) && !IncompleteTasks.Contains(task)))
                return;

            IncompleteTasks.Remove(task);
            CompleteTasks.Add(task);

            int points = CircuitTasks.GetTaskPoints(task);
            PointsMenu.AddPoints(points);
        }

        public static bool IsFestival(string which)
        {
            if (Game1.CurrentEvent is null)
                return false;

            return Game1.CurrentEvent.isSpecificFestival(which);
        }

        public static int CountBuffs()
        {
            int count = 0;

            count += Game1.buffsDisplay.food is null ? 0 : 1;
            count += Game1.buffsDisplay.drink is null ? 0 : 1;
            count += Game1.buffsDisplay.otherBuffs.Count;

            return count;
        }

        public void OnBuildingComplete(string building)
        {
            Logger.Log($"TM: building complete {building}", LogLevel.Debug);

            switch (building)
            {
                case "Well": MarkComplete(CircuitTask.BUILD_WELL); break;
                case "Coop": MarkComplete(CircuitTask.BUILD_COOP); break;
                case "Barn": MarkComplete(CircuitTask.BUILD_BARN); break;
                case "Big Coop": MarkComplete(CircuitTask.BUILD_BIG_COOP); break;
                case "Big Barn": MarkComplete(CircuitTask.BUILD_BIG_BARN); break;
                case "Deluxe Coop": MarkComplete(CircuitTask.BUILD_DELUXE_COOP); break;
                case "Deluxe Barn": MarkComplete(CircuitTask.BUILD_DELUXE_BARN); break;
                case "Stable": MarkComplete(CircuitTask.BUILD_STABLE); break;
                case "Fish Pond": MarkComplete(CircuitTask.BUILD_FISH_POND); break;
            }
        }

        public void OnItemCrafted(Item item)
        {
            Logger.Log($"TM: item crafted '{item.DisplayName}', {item.ParentSheetIndex}", LogLevel.Debug);

            if (item is not SObject obj)
                return;

            if (!obj.bigCraftable.Value)
            {
                switch (item.ParentSheetIndex)
                {
                    case 251: MarkComplete(CircuitTask.CRAFT_TEA_SAPLING); break;
                    case 774: MarkComplete(CircuitTask.CRAFT_WILD_BAIT); break;
                }

                if (FishingTackleIds.Contains(item.ParentSheetIndex))
                    MarkComplete(CircuitTask.CRAFT_FISHING_TACKLE);
                else if (SeasonalSeedsIds.Contains(item.ParentSheetIndex))
                    SeasonalSeedsCrafted.Add(item.ParentSheetIndex);

                if (SeasonalSeedsCrafted.Count == 4)
                    MarkComplete(CircuitTask.CRAFT_ALL_SEASONAL_SEEDS);
            }
            else
            {
                switch (obj.ParentSheetIndex)
                {
                    case 108:
                    case 109:
                        MarkComplete(CircuitTask.CRAFT_TUB_OF_FLOWERS); break;
                }
            }
        }

        public void OnHatEquipped(Hat hat)
        {
            Logger.Log($"TM: hat equipped {hat.DisplayName}", LogLevel.Debug);

            switch (hat.which.Value)
            {
                case 62: MarkComplete(CircuitTask.EQUIP_PIRATE_HAT); break;
                case 68: MarkComplete(CircuitTask.EQUIP_PROPELLER_HAT); break;
                case 54: MarkComplete(CircuitTask.EQUIP_FLOPPY_BEANIE); break;
            }

            if (PartyHatIds.Contains(hat.which.Value))
                PartyHatsEquipped.Add(hat.which.Value);

            if (PartyHatsEquipped.Count >= 2)
                MarkComplete(CircuitTask.EQUIP_TWO_UNIQUE_PARTY_HATS);
        }

        public void OnItemShipped(int objectIndex)
        {
            Logger.Log($"TM: item shipped {objectIndex}", LogLevel.Debug);

            if (objectIndex == 454)
                MarkComplete(CircuitTask.SHIP_ANCIENT_FRUIT);
        }

        public void OnItemGifted(NPC who, SObject item)
        {
            if (item.bigCraftable.Value)
                return;

            Logger.Log($"TM: item gifted to {who.Name}, {item.DisplayName} {item.ParentSheetIndex}", LogLevel.Debug);

            if (who.Name == "Shane" && item.ParentSheetIndex == 260)
                MarkComplete(CircuitTask.GIFT_SHANE_HOTPEPPER);
            else if (who.Name == "Abigail" && item.ParentSheetIndex == 66)
                MarkComplete(CircuitTask.GIFT_ABIGAIL_AMETHYST);
            else if (who.Name == "Sebastion" && item.ParentSheetIndex == 84)
                MarkComplete(CircuitTask.GIFT_SEBASTION_FROZEN_TEAR);
            else if (who.Name == "Harvey" && item.ParentSheetIndex == 395)
                MarkComplete(CircuitTask.GIFT_HARVEY_COFFEE);
            else if (who.Name == "Maru" && item.ParentSheetIndex == 190)
                MarkComplete(CircuitTask.GIFT_MARU_CAULIFLOWER);
            else if (who.Name == "Penny" && item.ParentSheetIndex == 254)
                MarkComplete(CircuitTask.GIFT_PENNY_MELON);
            else if (who.Name == "Demetrius" && item.ParentSheetIndex == 233)
                MarkComplete(CircuitTask.GIFT_DEMETRIUS_ICECREAM);
            else if (who.Name == "George" && item.ParentSheetIndex == 20)
                MarkComplete(CircuitTask.GIFT_GEORGE_LEEK);
            else if (who.Name == "Linus" && item.ParentSheetIndex == 280)
                MarkComplete(CircuitTask.GIFT_LINUS_YAM);
            else if (who.Name == "Pam" && item.ParentSheetIndex == 24)
                MarkComplete(CircuitTask.GIFT_PAM_PARSNIP);
            else if (who.Name == "Robin" && item.ParentSheetIndex == 224)
                MarkComplete(CircuitTask.GIFT_ROBIN_SPAGHETTI);
            else if (who.Name == "Sandy" && FlowerIds.Contains(item.ParentSheetIndex))
                MarkComplete(CircuitTask.GIFT_SANDY_FLOWER);
            else if (who.Name == "Vincent" && item.ParentSheetIndex == 398)
                MarkComplete(CircuitTask.GIFT_VINCENT_GRAPE);
            else if (who.Name == "Wizard" && item.ParentSheetIndex == 422)
                MarkComplete(CircuitTask.GIFT_WIZARD_PURPLE_MUSHROOM);
            else if (who.Name == "Sam" && item.ParentSheetIndex == 90)
                MarkComplete(CircuitTask.GIFT_SAM_CACTUS_FRUIT);
            else if (who.Name == "Leah" && item.ParentSheetIndex == 426)
                MarkComplete(CircuitTask.GIFT_LEAH_GOAT_CHEESE);
            else if (who.Name == "Emily" && item.ParentSheetIndex == 428)
                MarkComplete(CircuitTask.GIFT_EMILY_CLOTH);
            else if (who.Name == "Caroline" && item.ParentSheetIndex == 614)
                MarkComplete(CircuitTask.GIFT_CAROLINE_GREEN_TEA);
            else if (who.Name == "Clint" && item.ParentSheetIndex == 336)
                MarkComplete(CircuitTask.GIFT_CLINT_GOLD_BAR);
            else if (who.Name == "Evelyn" && item.ParentSheetIndex == 220)
                MarkComplete(CircuitTask.GIFT_EVELYN_CHOCOLATE_CAKE);
            else if (who.Name == "Gus" && item.ParentSheetIndex == 635)
                MarkComplete(CircuitTask.GIFT_GUS_ORANGE);
            else if (who.Name == "Jas" && item.ParentSheetIndex == 221)
                MarkComplete(CircuitTask.GIFT_JAS_PINK_CAKE);
            else if (who.Name == "Kent" && item.ParentSheetIndex == 607)
                MarkComplete(CircuitTask.GIFT_KENT_ROASTED_HAZELNUT);
            else if (who.Name == "Lewis" && item.ParentSheetIndex == 208)
                MarkComplete(CircuitTask.GIFT_LEWIS_GLAZED_YAM);
            else if (who.Name == "Alex" && item.ParentSheetIndex == 201)
                MarkComplete(CircuitTask.GIFT_ALEX_COMPLETE_BREAKFAST);
            else if (who.Name == "Elliott" && item.ParentSheetIndex == 444)
                MarkComplete(CircuitTask.GIFT_ELLIOTT_DUCK_FEATHER);
            else if (who.Name == "Dwarf" && GemIds.Contains(item.ParentSheetIndex))
                MarkComplete(CircuitTask.GIFT_DWARF_GEM);
            else if (who.Name == "Willy" && item.ParentSheetIndex == 149)
                MarkComplete(CircuitTask.GIFT_WILLY_OCTOPUS);
            else if (who.Name == "Jodi" && item.ParentSheetIndex == 72)
                MarkComplete(CircuitTask.GIFT_JODI_DIAMOND);
            else if (who.Name == "Marnie" && item.ParentSheetIndex == 608)
                MarkComplete(CircuitTask.GIFT_MARNIE_PUMPKIN_PIE);
            else if (who.Name == "Krobus" && item.ParentSheetIndex == 276)
                MarkComplete(CircuitTask.GIFT_KROBUS_PUMPKIN);
            else if (who.Name == "Pierre" && item.ParentSheetIndex == 202)
                MarkComplete(CircuitTask.GIFT_PIERRE_FRIED_CALAMARI);
            else
            {
                int taste = who.getGiftTasteForThisItem(item);
                if (item.Quality == SObject.bestQuality && taste <= NPC.gift_taste_like)
                    MarkComplete(CircuitTask.GIFT_ANY_IRIDIUM_LIKED_LOVED);

                if (taste == NPC.gift_taste_love && IsFestival("winter25"))
                    MarkComplete(CircuitTask.GIFT_LOVED_GIFT_AT_WINTER_FEAST);
            }
        }

        public void OnItemObtained(Item item)
        {
            if (item is SObject obj)
            {
                if (!obj.bigCraftable.Value)
                {
                    Logger.Log($"TM: object obtained {obj.ParentSheetIndex}", LogLevel.Debug);

                    switch (obj.ParentSheetIndex)
                    {
                        case 160: MarkComplete(CircuitTask.CATCH_ANGLER); break;
                        case 159: MarkComplete(CircuitTask.CATCH_CRIMSONFISH); break;
                        case 2423: MarkComplete(CircuitTask.OBTAIN_VISTA_PAINTING); break;
                        case 2334: MarkComplete(CircuitTask.OBTAIN_PYRAMID_DECAL); break;
                    }

                    if (IsFestival("fall27") && obj.ParentSheetIndex == 373)
                        MarkComplete(CircuitTask.COMPLETE_SPIRITS_EVE_MAZE);
                    else if (Game1.player.currentLocation.Name == "MermaidHouse" && obj.ParentSheetIndex == 797)
                        MarkComplete(CircuitTask.RECEIVE_MERMAID_SHOW_PEARL);
                }
                else
                {
                    Logger.Log($"TM: big craftable obtained {obj.ParentSheetIndex}", LogLevel.Debug);

                    if (RarecrowIds.Contains(obj.ParentSheetIndex))
                        RarecrowsCollected.Add(obj.ParentSheetIndex);
                    else if (SecretStatueIds.Contains(obj.ParentSheetIndex))
                        MarkComplete(CircuitTask.OBTAIN_ANY_THREE_SECRET_STATUES);
                }
            }
            else if (item is MeleeWeapon weapon)
            {
                Logger.Log($"TM: melee weapon obtained {weapon.getDrawnItemIndex()}", LogLevel.Debug);

                switch (weapon.getDrawnItemIndex())
                {
                    case 53: MarkComplete(CircuitTask.OBTAIN_GOLDEN_SCYTHE); break;
                    case 4: MarkComplete(CircuitTask.OBTAIN_GALAXY_SWORD); break;
                }
            }
            else if (item is Hat hat)
            {
                Logger.Log($"TM: hat item obtained {hat.which.Value}", LogLevel.Debug);

                switch (hat.ParentSheetIndex)
                {
                    case 17: MarkComplete(CircuitTask.WIN_ICE_FESTIVAL_FISHING); break;
                }
            }

            if (RarecrowsCollected.Count >= 3)
                MarkComplete(CircuitTask.COLLECT_THREE_RARECROWS);
        }

        public void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if (IsComplete(CircuitTask.THREE_CONCURRENT_BUFFS))
                return;

            if (CountBuffs() >= 3)
                MarkComplete(CircuitTask.THREE_CONCURRENT_BUFFS);
        }

        public void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            Logger.Log("TM: day started", LogLevel.Debug);

            Farm farm = (Farm)Game1.getLocationFromName("Farm");

            if (!IsComplete(CircuitTask.GROW_GIANT_CROP))
            {
                foreach (var resource in farm.resourceClumps)
                {

                    if (resource is GiantCrop)
                    {
                        MarkComplete(CircuitTask.GROW_GIANT_CROP);
                        break;
                    }
                }
            }

            if (!IsComplete(CircuitTask.INCUBATE_DINO_EGG))
            {
                foreach (FarmAnimal animal in farm.getAllFarmAnimals())
                {
                    if (animal.type.Value == "Dinosaur")
                    {
                        MarkComplete(CircuitTask.INCUBATE_DINO_EGG);
                        break;
                    }
                }
            }

        }

        public void OnJournalQuestComplete(int questId)
        {
            Logger.Log($"TM: quest complete {questId}", LogLevel.Debug);

            switch (questId)
            {
                case 100: MarkComplete(CircuitTask.QUEST_ROBINS_LOST_AXE); break;
                case 107: MarkComplete(CircuitTask.QUEST_BLACKBERRY_BASKET); break;
                case 110: MarkComplete(CircuitTask.QUEST_CLINTS_ATTEMPT); break;
                case 25: MarkComplete(CircuitTask.QUEST_HOW_TO_WIN_FRIENDS); break;
                case 106: MarkComplete(CircuitTask.QUEST_COWS_DELIGHT); break;
                case 102: MarkComplete(CircuitTask.QUEST_MAYORS_SHORTS); break;
                case 108: MarkComplete(CircuitTask.QUEST_CARVING_PUMPKINS); break;
                case 115: MarkComplete(CircuitTask.QUEST_FRESH_FRUIT); break;
                case 118: MarkComplete(CircuitTask.QUEST_AQUATIC_RESEARCH); break;
                case 121: MarkComplete(CircuitTask.QUEST_WANTED_LOBSTER); break;
                case 22: MarkComplete(CircuitTask.QUEST_FISH_CASSSEROLE); break;
                case 119: MarkComplete(CircuitTask.QUEST_SOLDIERS_STAR); break;
                case 124: MarkComplete(CircuitTask.QUEST_LINGCOD); break;
                case 5: MarkComplete(CircuitTask.UNLOCK_CASINO); break;
            }
        }

        public void OnAchievementEarned(int achievementId)
        {
            Logger.Log($"TM: achievement earned {achievementId}", LogLevel.Debug);

            switch (achievementId)
            {
                case 20: MarkComplete(CircuitTask.ACHIEVEMENT_DIY); break;
                case 24: MarkComplete(CircuitTask.ACHIEVEMENT_FISHERMAN); break;
                case 15: MarkComplete(CircuitTask.ACHIEVEMENT_COOK); break;
                case 2: MarkComplete(CircuitTask.ACHIEVEMENT_HOMESTEADER); break;
                case 6: MarkComplete(CircuitTask.ACHIEVEMENT_NEW_FRIEND); break;
                case 11: MarkComplete(CircuitTask.ACHIEVEMENT_CLIQUES); break;
                case 32: MarkComplete(CircuitTask.ACHIEVEMENT_MONOCULTURE); break;
                case 21: MarkComplete(CircuitTask.ACHIEVEMENT_ARTISAN); break;
                case 29: MarkComplete(CircuitTask.ACHIEVEMENT_GOFER); break;
                case 27: MarkComplete(CircuitTask.ACHIEVEMENT_MOTHER_CATCH); break;
                case 25: MarkComplete(CircuitTask.ACHIEVEMENT_OL_MARINER); break;
                case 16: MarkComplete(CircuitTask.ACHIEVEMENT_SOUS_CHEF); break;
                case 3: MarkComplete(CircuitTask.ACHIEVEMENT_MILLIONAIRE); break;
                case 28: MarkComplete(CircuitTask.ACHIEVEMENT_TREASURE_TROVE); break;
            }
        }

        public void OnSteamAchievementEarned(string achievementId)
        {
            Logger.Log($"TM: steam achievement earned {achievementId}", LogLevel.Debug);

            switch (achievementId)
            {
                case "Achievement_SingularTalent": MarkComplete(CircuitTask.ACHIEVEMENT_SINGULAR_TALENT); break;
                case "Achievement_TheBottom": MarkComplete(CircuitTask.ACHIEVEMENT_THE_BOTTOM); break;
            }
        }

        public void OnToolUpgrade(Tool newTool)
        {
            Logger.Log($"TM: tool upgrade {newTool.UpgradeLevel}", LogLevel.Debug);

            switch (newTool.UpgradeLevel)
            {
                case Tool.copper: MarkComplete(CircuitTask.UPGRADE_TOOL_COPPER); break;
                case Tool.steel: MarkComplete(CircuitTask.UPGRADE_TOOL_STEEL); break;
                case Tool.gold: MarkComplete(CircuitTask.UPGRADE_TOOL_GOLD); break;
                case Tool.iridium: MarkComplete(CircuitTask.UPGRADE_TOOL_IRIDIUM); break;
            }
        }

        public void OnCommunityCenterRoomComplete(int which)
        {
            Logger.Log($"TM: community center room complete {which}", LogLevel.Debug);

            switch (which)
            {
                case CommunityCenter.AREA_BoilerRoom: MarkComplete(CircuitTask.CC_COMPLETE_BOILER_ROOM); break;
                case CommunityCenter.AREA_Pantry: MarkComplete(CircuitTask.CC_COMPLETE_PANTRY); break;
                case CommunityCenter.AREA_FishTank: MarkComplete(CircuitTask.CC_COMPLETE_FISH_TANK); break;
                case CommunityCenter.AREA_Bulletin: MarkComplete(CircuitTask.CC_COMPLETE_BULLETIN_BOARD); break;
                case CommunityCenter.AREA_Vault: MarkComplete(CircuitTask.CC_COMPLETE_VAULT); break;
                case CommunityCenter.AREA_CraftsRoom: MarkComplete(CircuitTask.CC_COMPLETE_CRAFTS_ROOM); break;
            }
        }

        public void OnAnimalPurchased(FarmAnimal animal)
        {
            Logger.Log($"TM: animal purchased {animal.type.Value}", LogLevel.Debug);

            switch (animal.type.Value)
            {
                case "Duck": MarkComplete(CircuitTask.ANIMAL_DUCK); break;
                case "Goat": MarkComplete(CircuitTask.ANIMAL_GOAT); break;
                case "Rabbit": MarkComplete(CircuitTask.ANIMAL_BUNNY); break;
                case "Sheep": MarkComplete(CircuitTask.ANIMAL_SHEEP); break;
                case "Pig": MarkComplete(CircuitTask.ANIMAL_PIG); break;
            }

            if (animal.type.Value.EndsWith("Chicken"))
                MarkComplete(CircuitTask.ANIMAL_CHICKEN);
            else if (animal.type.Value.EndsWith("Cow"))
                MarkComplete(CircuitTask.ANIMAL_COW);
        }

        public void OnEventCheckAction(Event which, Location tileLocation)
        {
            Logger.Log($"TM: event check action {which.id}", LogLevel.Debug);

            NPC? festivalHost = (NPC)which.GetType().GetField("festivalHost", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(which)!;

            if (!IsFestival("fall16") || !which.grangeJudged || festivalHost is null || festivalHost.getTileX() != tileLocation.X || festivalHost.getTileY() != tileLocation.Y)
                return;

            if (which.grangeScore == -666)
                MarkComplete(CircuitTask.FAIR_DISQUALIFY);
            else if (which.grangeScore >= 90)
                MarkComplete(CircuitTask.STARDEW_FAIR_GRANGE_DISPLAY);

        }

        public void OnEventEnd(Event which)
        {
            Logger.Log($"TM: event end {which.id}, {which.FestivalName}", LogLevel.Debug);

            switch (which.id)
            {
                case 1: MarkComplete(CircuitTask.EVENT_ABIGAIL_TWO_HEART); break;
                case 44: MarkComplete(CircuitTask.EVENT_SAM_TWO_HEART); break;
                case 611944: MarkComplete(CircuitTask.EVENT_SHANE_TWO_HEART); break;
                case 50: MarkComplete(CircuitTask.EVENT_LEAH_TWO_HEART); break;
                case 6: MarkComplete(CircuitTask.EVENT_MARU_TWO_HEART); break;
                case 34: MarkComplete(CircuitTask.EVENT_PENNY_TWO_HEART); break;
                case 2481135: MarkComplete(CircuitTask.EVENT_ALEX_FOUR_HEART); break;
                case 40: MarkComplete(CircuitTask.EVENT_ELLIOTT_FOUR_HEART); break;
                case 12: MarkComplete(CircuitTask.EVENT_HALEY_FOUR_HEART); break;
                case 19: MarkComplete(CircuitTask.EVENT_EVELYN_FOUR_HEART); break;
                case 58: MarkComplete(CircuitTask.EVENT_HARVEY_SIX_HEART); break;
                case 27: MarkComplete(CircuitTask.EVENT_SEBASTION_SIX_HEART); break;
                case 917409: MarkComplete(CircuitTask.EVENT_EMILY_SIX_HEART); break;
            }

            if (which.FestivalName == "Dance Of The Moonlight Jellies")
                MarkComplete(CircuitTask.WATCH_MOONLIGHT_JELLIES);
            else if (which.FestivalName == "Egg Festival" && Game1.player.festivalScore >= 15)
                MarkComplete(CircuitTask.COLLECT_15_EGGS_AT_FESTIVAL);
        }

        public void OnGrandpaCandles(Farm farm)
        {
            Logger.Log($"TM: grandpa candles {farm.grandpaScore.Value}", LogLevel.Debug);

            if (farm.grandpaScore.Value >= 3)
                MarkComplete(CircuitTask.THREE_CANDLES);
        }

        public void OnTreeUpdateTapperProduct(Tree tree)
        {
            Logger.Log($"TM: tree update tapper product {tree.treeType.Value}", LogLevel.Debug);

            if (tree.tapped.Value && tree.treeType.Value == Tree.mushroomTree)
                MarkComplete(CircuitTask.TAPPER_ON_MUSHROOM_TREE);
        }

        public void OnEventGovernorTaste(Event which)
        {
            Logger.Log($"TM: governor taste {which.id}", LogLevel.Debug);

            if (!IsFestival("summer11"))
                return;

            string reactionCommand = which.eventCommands[which.CurrentCommand + 1];
            string reaction = reactionCommand.Split(' ')[^1];
            int likeLevel = int.Parse(reaction[^1].ToString());

            if (likeLevel == 4)
                MarkComplete(CircuitTask.LUAU_BEST_REACTION);
        }

        public void OnFishTankCheckForAction(FishTankFurniture tank)
        {
            Logger.Log($"TM: fish tank action", LogLevel.Debug);

            if (tank.localDepositedItem is not null && tank.localDepositedItem is Hat)
                MarkComplete(CircuitTask.GIVE_SEA_URCHIN_A_HAT);
        }

        public void OnChangeFriendship(NPC which, int amount)
        {
            Logger.Log($"TM: change friendship {which.Name}, {amount}", LogLevel.Debug);

            if (!Game1.player.friendshipData.ContainsKey(which.Name))
                return;

            if (which.datable.Value && Game1.player.friendshipData[which.Name].Points >= 2000)
                MarkComplete(CircuitTask.DATE_ANYONE);
        }

        public void OnMonsterSlayerQuestCompleted()
        {
            MarkComplete(CircuitTask.MONSTER_SLAYER);
        }

        public void OnHitByTrain()
        {
            MarkComplete(CircuitTask.GET_HIT_BY_TRAIN);
        }

        public void OnFlowerDancePartnerAcquired()
        {
            MarkComplete(CircuitTask.DANCE_AT_FLOWER_DANCE);
        }

        public void OnSpecialOrderCompleted()
        {
            MarkComplete(CircuitTask.COMPLETE_SPECIAL_ORDER);
        }

        public void OnNPCCheckAction(NPC which, Farmer who)
        {
            Logger.Log($"TM: npc check action, {which.Name}", LogLevel.Debug);

            if (who.pantsItem.Value != null && who.pantsItem.Value.ParentSheetIndex == 15 && which.Name.Equals("Lewis"))
                MarkComplete(CircuitTask.TALK_TO_LEWIS_WITH_SHORTS);
        }

        public bool OnShopMenuPurchase(ISalable item, Farmer who, int amount)
        {
            Logger.Log($"TM: purchased from hat mouse", LogLevel.Debug);

            MarkComplete(CircuitTask.BUY_HAT_FROM_HATMOUSE);

            return false;
        }

        public void OnMineCartDied()
        {
            Logger.Log($"TM: mine cart died", LogLevel.Debug);

            if (IsComplete(CircuitTask.JUNIMO_KART_LEWIS_SCORE))
                return;

            var scores = Game1.player.team.junimoKartScores.GetScores();
            if (scores[0].Key != "Lewis")
                MarkComplete(CircuitTask.JUNIMO_KART_LEWIS_SCORE);
        }
    }
}
