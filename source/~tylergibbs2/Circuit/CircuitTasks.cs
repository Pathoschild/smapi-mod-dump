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
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Circuit
{
    public enum TaskDifficulty
    {
        EASY,
        MEDIUM,
        HARD,
        EXPERT
    }

    public enum CircuitTask
    {
        // easy
        COLLECT_15_EGGS_AT_FESTIVAL,
        WIN_ICE_FESTIVAL_FISHING,
        LUAU_BEST_REACTION,
        WATCH_MOONLIGHT_JELLIES,
        ACHIEVEMENT_DIY,
        ACHIEVEMENT_FISHERMAN,
        ACHIEVEMENT_SINGULAR_TALENT,
        BUILD_COOP,
        BUILD_BARN,
        BUILD_WELL,
        ANIMAL_CHICKEN,
        ANIMAL_COW,
        CRAFT_TEA_SAPLING,
        CRAFT_FISHING_TACKLE,
        GIVE_SEA_URCHIN_A_HAT,
        BUY_HAT_FROM_HATMOUSE,
        GIFT_SHANE_HOTPEPPER,
        GIFT_ABIGAIL_AMETHYST,
        GIFT_SEBASTION_FROZEN_TEAR,
        GIFT_HARVEY_COFFEE,
        GIFT_HALEY_SUNFLOWER,
        GIFT_MARU_CAULIFLOWER,
        GIFT_PENNY_MELON,
        GIFT_DEMETRIUS_ICECREAM,
        GIFT_GEORGE_LEEK,
        GIFT_LINUS_YAM,
        GIFT_PAM_PARSNIP,
        GIFT_ROBIN_SPAGHETTI,
        GIFT_SANDY_FLOWER,
        GIFT_VINCENT_GRAPE,
        GIFT_WIZARD_PURPLE_MUSHROOM,
        UPGRADE_TOOL_COPPER,
        OBTAIN_VISTA_PAINTING,
        OBTAIN_PYRAMID_DECAL,
        EQUIP_FLOPPY_BEANIE,
        QUEST_ROBINS_LOST_AXE,
        QUEST_BLACKBERRY_BASKET,
        QUEST_CLINTS_ATTEMPT,
        QUEST_HOW_TO_WIN_FRIENDS,
        QUEST_COWS_DELIGHT,
        QUEST_MAYORS_SHORTS,
        QUEST_CARVING_PUMPKINS,

        //medium
        EQUIP_TWO_UNIQUE_PARTY_HATS,
        FAIR_DISQUALIFY,
        CC_COMPLETE_VAULT,
        GIFT_SAM_CACTUS_FRUIT,
        GIFT_LEAH_GOAT_CHEESE,
        GIFT_EMILY_CLOTH,
        GIFT_CAROLINE_GREEN_TEA,
        GIFT_CLINT_GOLD_BAR,
        GIFT_EVELYN_CHOCOLATE_CAKE,
        GIFT_GUS_ORANGE,
        GIFT_JAS_PINK_CAKE,
        GIFT_KENT_ROASTED_HAZELNUT,
        GIFT_LEWIS_GLAZED_YAM,
        DANCE_AT_FLOWER_DANCE,
        COMPLETE_SPIRITS_EVE_MAZE,
        GIFT_LOVED_GIFT_AT_WINTER_FEAST,
        UPGRADE_TOOL_STEEL,
        THREE_CONCURRENT_BUFFS,
        CRAFT_TUB_OF_FLOWERS,
        COMPLETE_SPECIAL_ORDER,
        RECEIVE_MERMAID_SHOW_PEARL,
        CATCH_ANGLER,
        GET_HIT_BY_TRAIN,
        BUILD_BIG_COOP,
        BUILD_BIG_BARN,
        ANIMAL_DUCK,
        ANIMAL_GOAT,
        BUILD_FISH_POND,
        EQUIP_PIRATE_HAT,
        EQUIP_PROPELLER_HAT,
        TALK_TO_LEWIS_WITH_SHORTS,
        EVENT_ABIGAIL_TWO_HEART,
        EVENT_SAM_TWO_HEART,
        EVENT_SHANE_TWO_HEART,
        EVENT_LEAH_TWO_HEART,
        EVENT_MARU_TWO_HEART,
        EVENT_PENNY_TWO_HEART,
        QUEST_FRESH_FRUIT,
        QUEST_AQUATIC_RESEARCH,
        QUEST_WANTED_LOBSTER,
        QUEST_FISH_CASSSEROLE,
        CC_COMPLETE_CRAFTS_ROOM,
        CC_COMPLETE_BOILER_ROOM,
        JUNIMO_KART_LEWIS_SCORE,
        ACHIEVEMENT_COOK,
        ACHIEVEMENT_HOMESTEADER,
        ACHIEVEMENT_NEW_FRIEND,
        ACHIEVEMENT_CLIQUES,
        ACHIEVEMENT_MONOCULTURE,
        ACHIEVEMENT_ARTISAN,

        // hard
        STARDEW_FAIR_GRANGE_DISPLAY,
        GIFT_ALEX_COMPLETE_BREAKFAST,
        GIFT_ELLIOTT_DUCK_FEATHER,
        GIFT_DWARF_GEM,
        GIFT_WILLY_OCTOPUS,
        GIFT_JODI_DIAMOND,
        GIFT_MARNIE_PUMPKIN_PIE,
        UPGRADE_TOOL_GOLD,
        SHIP_ANCIENT_FRUIT,
        COLLECT_THREE_RARECROWS,
        OBTAIN_ANY_THREE_SECRET_STATUES,
        CC_COMPLETE_PANTRY,
        CC_COMPLETE_FISH_TANK,
        CC_COMPLETE_BULLETIN_BOARD,
        CRAFT_ALL_SEASONAL_SEEDS,
        CRAFT_WILD_BAIT,
        CATCH_CRIMSONFISH,
        BUILD_DELUXE_COOP,
        BUILD_DELUXE_BARN,
        BUILD_STABLE,
        ANIMAL_BUNNY,
        ANIMAL_SHEEP,
        ANIMAL_PIG,
        INCUBATE_DINO_EGG,
        EVENT_ALEX_FOUR_HEART,
        EVENT_ELLIOTT_FOUR_HEART,
        EVENT_HALEY_FOUR_HEART,
        EVENT_EVELYN_FOUR_HEART,
        QUEST_SOLDIERS_STAR,
        QUEST_LINGCOD,
        TAPPER_ON_MUSHROOM_TREE,
        GROW_GIANT_CROP,
        OBTAIN_GOLDEN_SCYTHE,
        MONSTER_SLAYER,
        ACHIEVEMENT_GOFER,
        ACHIEVEMENT_MOTHER_CATCH,
        ACHIEVEMENT_OL_MARINER,
        ACHIEVEMENT_THE_BOTTOM,
        ACHIEVEMENT_SOUS_CHEF,

        // expert
        DATE_ANYONE,
        THREE_CANDLES,
        UNLOCK_CASINO,
        GIFT_ANY_IRIDIUM_LIKED_LOVED,
        ACHIEVEMENT_MILLIONAIRE,
        ACHIEVEMENT_TREASURE_TROVE,
        GIFT_KROBUS_PUMPKIN,
        GIFT_PIERRE_FRIED_CALAMARI,
        UPGRADE_TOOL_IRIDIUM,
        OBTAIN_GALAXY_SWORD,
        EVENT_HARVEY_SIX_HEART,
        EVENT_SEBASTION_SIX_HEART,
        EVENT_EMILY_SIX_HEART
    }

    public class CircuitTasks
    {
        public static HashSet<CircuitTask> FetchTasks(int easy, int medium, int hard, int expert, Random? random = null)
        {
            random ??= Game1.random;

            HashSet<CircuitTask> result = new();

            var allTasks = Enum.GetValues<CircuitTask>().ToList();
            while (easy > 0)
            {
                CircuitTask randomTask = allTasks[random.Next(allTasks.Count)];
                if (GetTaskDifficulty(randomTask) == TaskDifficulty.EASY && !result.Contains(randomTask))
                {
                    result.Add(randomTask);
                    allTasks.Remove(randomTask);
                    easy--;
                }
            }

            while (medium > 0)
            {
                CircuitTask randomTask = allTasks[random.Next(allTasks.Count)];
                if (GetTaskDifficulty(randomTask) == TaskDifficulty.MEDIUM && !result.Contains(randomTask))
                {
                    result.Add(randomTask);
                    allTasks.Remove(randomTask);
                    medium--;
                }
            }

            while (hard > 0)
            {
                CircuitTask randomTask = allTasks[random.Next(allTasks.Count)];
                if (GetTaskDifficulty(randomTask) == TaskDifficulty.HARD && !result.Contains(randomTask))
                {
                    result.Add(randomTask);
                    allTasks.Remove(randomTask);
                    hard--;
                }
            }

            while (expert > 0)
            {
                CircuitTask randomTask = allTasks[random.Next(allTasks.Count)];
                if (GetTaskDifficulty(randomTask) == TaskDifficulty.EXPERT && !result.Contains(randomTask))
                {
                    result.Add(randomTask);
                    allTasks.Remove(randomTask);
                    expert--;
                }
            }


            return result;
        }

        public static string GetTaskDisplayText(CircuitTask task)
        {
            return task switch
            {
                // easy
                CircuitTask.COLLECT_15_EGGS_AT_FESTIVAL => "Collect 15 eggs at the Egg Festival",
                CircuitTask.WIN_ICE_FESTIVAL_FISHING => "Win the fishing competition at the Festival of Ice",
                CircuitTask.LUAU_BEST_REACTION => "Get the best reaction at the Luau",
                CircuitTask.WATCH_MOONLIGHT_JELLIES => "Watch the Dance Of The Moonlight Jellies",
                CircuitTask.ACHIEVEMENT_DIY => "Earn the \"D.I.Y\" achievement",
                CircuitTask.ACHIEVEMENT_FISHERMAN => "Earn the \"Fisherman\" achievement",
                CircuitTask.ACHIEVEMENT_SINGULAR_TALENT => "Earn the \"Singular Talent\" achievement",
                CircuitTask.BUILD_COOP => "Build a Coop",
                CircuitTask.BUILD_BARN => "Build a Barn",
                CircuitTask.BUILD_WELL => "Build a Well",
                CircuitTask.ANIMAL_CHICKEN => "Buy a Chicken",
                CircuitTask.ANIMAL_COW => "Buy a Cow",
                CircuitTask.CRAFT_TEA_SAPLING => "Craft a Tea Sapling",
                CircuitTask.CRAFT_FISHING_TACKLE => "Craft any Fishing Tackle",
                CircuitTask.GIVE_SEA_URCHIN_A_HAT => "Give a Sea Urchin any hat",
                CircuitTask.BUY_HAT_FROM_HATMOUSE => "Buy a hat from Hat Mouse",
                CircuitTask.GIFT_SHANE_HOTPEPPER => "Give Shane a Hot Pepper",
                CircuitTask.GIFT_ABIGAIL_AMETHYST => "Give Abigail an Amethyst",
                CircuitTask.GIFT_SEBASTION_FROZEN_TEAR => "Give Sebastian a Frozen Tear",
                CircuitTask.GIFT_HARVEY_COFFEE => "Give Harvey a Coffee",
                CircuitTask.GIFT_HALEY_SUNFLOWER => "Give Haley a Sunflower",
                CircuitTask.GIFT_MARU_CAULIFLOWER => "Give Maru a Cauliflower",
                CircuitTask.GIFT_PENNY_MELON => "Give Penny a Melon",
                CircuitTask.GIFT_DEMETRIUS_ICECREAM => "Give Demetrius an Ice Cream",
                CircuitTask.GIFT_GEORGE_LEEK => "Give George a Leek",
                CircuitTask.GIFT_LINUS_YAM => "Give Linus a Yam",
                CircuitTask.GIFT_PAM_PARSNIP => "Give Pam a Parsnip",
                CircuitTask.GIFT_ROBIN_SPAGHETTI => "Give Robin a Spaghetti",
                CircuitTask.GIFT_SANDY_FLOWER => "Give Sandy a Flower",
                CircuitTask.GIFT_VINCENT_GRAPE => "Give Vincent a Grape",
                CircuitTask.GIFT_WIZARD_PURPLE_MUSHROOM => "Give the Wizard a Purple Mushroom",
                CircuitTask.UPGRADE_TOOL_COPPER => "Upgrade any tool to Copper",
                CircuitTask.OBTAIN_VISTA_PAINTING => "Obtain the \"Vista\" painting",
                CircuitTask.OBTAIN_PYRAMID_DECAL => "Obtain the \"Pyramid Decal\" wall hanging",
                CircuitTask.EQUIP_FLOPPY_BEANIE => "Wear the Floppy Beanie",
                CircuitTask.QUEST_ROBINS_LOST_AXE => "Return Robin's Axe, journal quest 'Robin's Lost Axe'",
                CircuitTask.QUEST_BLACKBERRY_BASKET => "Return Linus' basket, journal quest 'Blackberry Basket'",
                CircuitTask.QUEST_CLINTS_ATTEMPT => "Bring Emily an Amethyst for Clint, journal quest 'Clint's Attempt'",
                CircuitTask.QUEST_HOW_TO_WIN_FRIENDS => "Meet everyone in town, journal quest 'Introductions'",
                CircuitTask.QUEST_COWS_DELIGHT => "Bring Marnie Amaranth, journal quest 'Cow's Delight'",
                CircuitTask.QUEST_MAYORS_SHORTS => "Return Lewis's Shorts, journal quest 'Mayor's \"Shorts\"'",
                CircuitTask.QUEST_CARVING_PUMPKINS => "Bring Caroline a Pumpkin, journal quest 'Carving Pumpkins'",

                //medium
                CircuitTask.EQUIP_TWO_UNIQUE_PARTY_HATS => "Equip any two different Party Hats",
                CircuitTask.FAIR_DISQUALIFY => "Be disqualified from the Stardew Valley Fair",
                CircuitTask.CC_COMPLETE_VAULT => "Complete the Community Center Vault",
                CircuitTask.GIFT_SAM_CACTUS_FRUIT => "Give Sam a Cactus Fruit",
                CircuitTask.GIFT_LEAH_GOAT_CHEESE => "Give Leah a Goat Cheese",
                CircuitTask.GIFT_EMILY_CLOTH => "Give Emily a Cloth",
                CircuitTask.GIFT_CAROLINE_GREEN_TEA => "Give Caroline a Green Tea",
                CircuitTask.GIFT_CLINT_GOLD_BAR => "Give Clint a Gold Bar",
                CircuitTask.GIFT_EVELYN_CHOCOLATE_CAKE => "Give Evelyn a Chocolate Cake",
                CircuitTask.GIFT_GUS_ORANGE => "Give Gus an Orange",
                CircuitTask.GIFT_JAS_PINK_CAKE => "Give Jas a Pink Cake",
                CircuitTask.GIFT_KENT_ROASTED_HAZELNUT => "Give Kent a Roasted Hazelnut",
                CircuitTask.GIFT_LEWIS_GLAZED_YAM => "Give Lewis a Glazed Yam",
                CircuitTask.DANCE_AT_FLOWER_DANCE => "Dance with any bachelor or bachelorette at the Flower Dance",
                CircuitTask.COMPLETE_SPIRITS_EVE_MAZE => "Obtain the Golden Pumpkin from the maze in the Spirit's Eve Festival",
                CircuitTask.GIFT_LOVED_GIFT_AT_WINTER_FEAST => "Give any loved gift at the Feast of the Winter Star",
                CircuitTask.UPGRADE_TOOL_STEEL => "Upgrade any tool to Steel",
                CircuitTask.THREE_CONCURRENT_BUFFS => "Have any three buffs going at the same time",
                CircuitTask.CRAFT_TUB_OF_FLOWERS => "Craft the Tub of Flowers",
                CircuitTask.COMPLETE_SPECIAL_ORDER => "Complete any Special Order request",
                CircuitTask.RECEIVE_MERMAID_SHOW_PEARL => "Receive a Pearl from the Night Market's mermaid show",
                CircuitTask.CATCH_ANGLER => "Catch the Angler",
                CircuitTask.GET_HIT_BY_TRAIN => "Get hit by a train",
                CircuitTask.BUILD_BIG_COOP => "Build a Big Coop",
                CircuitTask.BUILD_BIG_BARN => "Build a Big Barn",
                CircuitTask.ANIMAL_DUCK => "Buy a Duck",
                CircuitTask.ANIMAL_GOAT => "Buy a Goat",
                CircuitTask.BUILD_FISH_POND => "Build a Fish Pond",
                CircuitTask.EQUIP_PIRATE_HAT => "Wear the Pirate Hat",
                CircuitTask.EQUIP_PROPELLER_HAT => "Wear the Propeller Hat",
                CircuitTask.TALK_TO_LEWIS_WITH_SHORTS => "Talk to Lewis while wearing his Lucky Purple Shorts",
                CircuitTask.EVENT_ABIGAIL_TWO_HEART => "Play Journey of the Prairie King with Abigail, Abigail's 2 heart cutscene",
                CircuitTask.EVENT_SAM_TWO_HEART => "Listen to Sam and Sebastian playing music, Sam's 2 heart cutscene",
                CircuitTask.EVENT_SHANE_TWO_HEART => "Share a beer with Shane, Shane's 2 heart cutscene",
                CircuitTask.EVENT_LEAH_TWO_HEART => "Walk into Leah making her sculpture, Leah's 2 heart cutscene",
                CircuitTask.EVENT_MARU_TWO_HEART => "Demetrius do be a creep, Maru's 2 heart cutscene",
                CircuitTask.EVENT_PENNY_TWO_HEART => "Watch Penny help George, Penny's 2 heart cutscene",
                CircuitTask.QUEST_FRESH_FRUIT => "Bring Emily an Apricot, journal quest 'Fresh Fruit'",
                CircuitTask.QUEST_AQUATIC_RESEARCH => "Bring Demetrius a Pufferfish, journal quest 'Aquatic Research'",
                CircuitTask.QUEST_WANTED_LOBSTER => "Bring Gus a Lobster, journal quest, 'Wanted: Lobster'",
                CircuitTask.QUEST_FISH_CASSSEROLE => "Bring a Largemouth Bass for dinner at Jodi's, journal quest 'Fish Casserole'",
                CircuitTask.CC_COMPLETE_CRAFTS_ROOM => "Complete the Community Center Crafts Room",
                CircuitTask.CC_COMPLETE_BOILER_ROOM => "Complete the Community Center Boiler Room",
                CircuitTask.JUNIMO_KART_LEWIS_SCORE => "Beat Lewis's score in Junimo Kart",
                CircuitTask.ACHIEVEMENT_COOK => "Earn the \"Cook\" achievement,",
                CircuitTask.ACHIEVEMENT_HOMESTEADER => "Earn the \"Homesteader\" achievement, earn 250,000g",
                CircuitTask.ACHIEVEMENT_NEW_FRIEND => "Earn the \"A New Friend\" achievement, reach a 5-heart friend level with someone. ",
                CircuitTask.ACHIEVEMENT_CLIQUES => "Earn the \"Cliques\" achievement, reach a 5-heart friend level with 4 people. ",
                CircuitTask.ACHIEVEMENT_MONOCULTURE => "Earn the \"Monoculture\" achievement, ship 300 of one crop",
                CircuitTask.ACHIEVEMENT_ARTISAN => "Earn the \"Artisan\" achievement, craft 30 different items",

                // hard
                CircuitTask.STARDEW_FAIR_GRANGE_DISPLAY => "Win the grange display contest at the Stardew Valley Fair",
                CircuitTask.GIFT_ALEX_COMPLETE_BREAKFAST => "Give Alex a Complete Breakfast",
                CircuitTask.GIFT_ELLIOTT_DUCK_FEATHER => "Give Elliott a Duck Feather",
                CircuitTask.GIFT_DWARF_GEM => "Give the Dwarf any gem",
                CircuitTask.GIFT_WILLY_OCTOPUS => "Give Willy an Octopus",
                CircuitTask.GIFT_JODI_DIAMOND => "Give Jodi a Diamond",
                CircuitTask.GIFT_MARNIE_PUMPKIN_PIE => "Give Marnie a Pumpkin Pie",
                CircuitTask.UPGRADE_TOOL_GOLD => "Upgrade any tool to Gold",
                CircuitTask.SHIP_ANCIENT_FRUIT => "Ship an Ancient Fruit",
                CircuitTask.COLLECT_THREE_RARECROWS => "Collect any three Rarecrows",
                CircuitTask.OBTAIN_ANY_THREE_SECRET_STATUES => "Obtain any of the three secret statues",
                CircuitTask.CC_COMPLETE_PANTRY => "Complete the Community Center Pantry",
                CircuitTask.CC_COMPLETE_FISH_TANK => "Complete the Community Center Fish Tank",
                CircuitTask.CC_COMPLETE_BULLETIN_BOARD => "Complete the Community Center Bulletin Board",
                CircuitTask.CRAFT_ALL_SEASONAL_SEEDS => "Craft all four seasonal seeds",
                CircuitTask.CRAFT_WILD_BAIT => "Craft Wild Bait",
                CircuitTask.CATCH_CRIMSONFISH => "Catch the Crimsonfish",
                CircuitTask.BUILD_DELUXE_COOP => "Build a Deluxe Coop",
                CircuitTask.BUILD_DELUXE_BARN => "Build a Deluxe Barn",
                CircuitTask.BUILD_STABLE => "Build a Stable",
                CircuitTask.ANIMAL_BUNNY => "Buy a Bunny",
                CircuitTask.ANIMAL_SHEEP => "Buy a Sheep",
                CircuitTask.ANIMAL_PIG => "Buy a Pig",
                CircuitTask.INCUBATE_DINO_EGG => "Incubate a Dinosaur Egg",
                CircuitTask.EVENT_ALEX_FOUR_HEART => "Meet Dusty, Alex's 4 heart cutscene",
                CircuitTask.EVENT_ELLIOTT_FOUR_HEART => "Drink with Elliott, Elliott's 4 heart cutscene",
                CircuitTask.EVENT_HALEY_FOUR_HEART => "Open a jar for Haley, Haley's 4 heart cutscene",
                CircuitTask.EVENT_EVELYN_FOUR_HEART => "Eat some of Evelyn's homemade cookie, Evelyn's 4 heart cutscene",
                CircuitTask.QUEST_SOLDIERS_STAR => "Bring Kent a starfruit, journal quest ‘A Soldier's Star'",
                CircuitTask.QUEST_LINGCOD => "Bring Willy a Lingcod, journal quest ‘Catch a Lingcod'",
                CircuitTask.TAPPER_ON_MUSHROOM_TREE => "Put a tapper on a Mushroom Tree",
                CircuitTask.GROW_GIANT_CROP => "Grow any giant crop",
                CircuitTask.OBTAIN_GOLDEN_SCYTHE => "Obtain the Golden Scythe",
                CircuitTask.MONSTER_SLAYER => "Complete any monster slayer goal",
                CircuitTask.ACHIEVEMENT_GOFER => "Earn the \"Gofer\" achievement, complete 10 help wanted quests",
                CircuitTask.ACHIEVEMENT_MOTHER_CATCH => "Earn the \"Mother Catch\" achievement, catch 100 fish",
                CircuitTask.ACHIEVEMENT_OL_MARINER => "Earn the \"Ol' Mariner\" achievement, catch 24 different fish",
                CircuitTask.ACHIEVEMENT_THE_BOTTOM => "Earn the \"the Bottom\" achievement, reach the lowest level of the mines",
                CircuitTask.ACHIEVEMENT_SOUS_CHEF => "Earn the \"Sous Chef\" achievement, cook 10 different recipes",

                // expert
                CircuitTask.DATE_ANYONE => "Date any bachelor or bachelorette",
                CircuitTask.THREE_CANDLES => "Receive three candles from Grandpa's Evaluation",
                CircuitTask.UNLOCK_CASINO => "Get the Club Card",
                CircuitTask.GIFT_ANY_IRIDIUM_LIKED_LOVED => "Give an iridium quality liked/loved gift ",
                CircuitTask.ACHIEVEMENT_MILLIONAIRE => "Earn the \"Millionaire\" achievement, earn 1,000,000g",
                CircuitTask.ACHIEVEMENT_TREASURE_TROVE => "Earn the \"Treasure Trove\" achievement, donate 40 different items to the museum",
                CircuitTask.GIFT_KROBUS_PUMPKIN => "Give Krobus a Pumpkin",
                CircuitTask.GIFT_PIERRE_FRIED_CALAMARI => "Give Pierre a Fried Calamari",
                CircuitTask.UPGRADE_TOOL_IRIDIUM => "Upgrade any tool to Iridium",
                CircuitTask.OBTAIN_GALAXY_SWORD => "Obtain the Galaxy Sword",
                CircuitTask.EVENT_HARVEY_SIX_HEART => "Watch Harvey do his dance aerobic session, Harvey's 6 heart cutscene",
                CircuitTask.EVENT_SEBASTION_SIX_HEART => "Play Solarion Chronicles: The Game with Sebastian and Sam, Sebastian's 6 heart cutscene",
                CircuitTask.EVENT_EMILY_SIX_HEART => "Watch Emily dance, Emily's 6 heart cutscene",
                _ => throw new NotImplementedException($"{task} missing from switch")
            };
        }

        public static int GetTaskPoints(CircuitTask task)
        {
            return GetTaskDifficulty(task) switch
            {
                TaskDifficulty.EASY => 5,
                TaskDifficulty.MEDIUM => 10,
                TaskDifficulty.HARD => 15,
                TaskDifficulty.EXPERT => 25,
                _ => throw new NotImplementedException("invalid difficulty"),
            };
        }

        public static Color GetTaskColor(CircuitTask task)
        {
            return GetTaskDifficulty(task) switch
            {
                TaskDifficulty.EASY => new Color(19, 138, 31),
                TaskDifficulty.MEDIUM => new Color(0, 0, 255),
                TaskDifficulty.HARD => Color.Magenta,
                TaskDifficulty.EXPERT => new Color(255, 0, 0),
                _ => throw new NotImplementedException("invalid difficulty"),
            };
        }

        public static TaskDifficulty GetTaskDifficulty(CircuitTask task)
        {
            return task switch
            {
                // easy
                CircuitTask.COLLECT_15_EGGS_AT_FESTIVAL => TaskDifficulty.EASY,
                CircuitTask.WIN_ICE_FESTIVAL_FISHING => TaskDifficulty.EASY,
                CircuitTask.LUAU_BEST_REACTION => TaskDifficulty.EASY,
                CircuitTask.WATCH_MOONLIGHT_JELLIES => TaskDifficulty.EASY,
                CircuitTask.ACHIEVEMENT_DIY => TaskDifficulty.EASY,
                CircuitTask.ACHIEVEMENT_FISHERMAN => TaskDifficulty.EASY,
                CircuitTask.ACHIEVEMENT_SINGULAR_TALENT => TaskDifficulty.EASY,
                CircuitTask.BUILD_COOP => TaskDifficulty.EASY,
                CircuitTask.BUILD_BARN => TaskDifficulty.EASY,
                CircuitTask.BUILD_WELL => TaskDifficulty.EASY,
                CircuitTask.ANIMAL_CHICKEN => TaskDifficulty.EASY,
                CircuitTask.ANIMAL_COW => TaskDifficulty.EASY,
                CircuitTask.CRAFT_TEA_SAPLING => TaskDifficulty.EASY,
                CircuitTask.CRAFT_FISHING_TACKLE => TaskDifficulty.EASY,
                CircuitTask.GIVE_SEA_URCHIN_A_HAT => TaskDifficulty.EASY,
                CircuitTask.BUY_HAT_FROM_HATMOUSE => TaskDifficulty.EASY,
                CircuitTask.GIFT_SHANE_HOTPEPPER => TaskDifficulty.EASY,
                CircuitTask.GIFT_ABIGAIL_AMETHYST => TaskDifficulty.EASY,
                CircuitTask.GIFT_SEBASTION_FROZEN_TEAR => TaskDifficulty.EASY,
                CircuitTask.GIFT_HARVEY_COFFEE => TaskDifficulty.EASY,
                CircuitTask.GIFT_HALEY_SUNFLOWER => TaskDifficulty.EASY,
                CircuitTask.GIFT_MARU_CAULIFLOWER => TaskDifficulty.EASY,
                CircuitTask.GIFT_PENNY_MELON => TaskDifficulty.EASY,
                CircuitTask.GIFT_DEMETRIUS_ICECREAM => TaskDifficulty.EASY,
                CircuitTask.GIFT_GEORGE_LEEK => TaskDifficulty.EASY,
                CircuitTask.GIFT_LINUS_YAM => TaskDifficulty.EASY,
                CircuitTask.GIFT_PAM_PARSNIP => TaskDifficulty.EASY,
                CircuitTask.GIFT_ROBIN_SPAGHETTI => TaskDifficulty.EASY,
                CircuitTask.GIFT_SANDY_FLOWER => TaskDifficulty.EASY,
                CircuitTask.GIFT_VINCENT_GRAPE => TaskDifficulty.EASY,
                CircuitTask.GIFT_WIZARD_PURPLE_MUSHROOM => TaskDifficulty.EASY,
                CircuitTask.UPGRADE_TOOL_COPPER => TaskDifficulty.EASY,
                CircuitTask.OBTAIN_VISTA_PAINTING => TaskDifficulty.EASY,
                CircuitTask.OBTAIN_PYRAMID_DECAL => TaskDifficulty.EASY,
                CircuitTask.EQUIP_FLOPPY_BEANIE => TaskDifficulty.EASY,
                CircuitTask.QUEST_ROBINS_LOST_AXE => TaskDifficulty.EASY,
                CircuitTask.QUEST_BLACKBERRY_BASKET => TaskDifficulty.EASY,
                CircuitTask.QUEST_CLINTS_ATTEMPT => TaskDifficulty.EASY,
                CircuitTask.QUEST_HOW_TO_WIN_FRIENDS => TaskDifficulty.EASY,
                CircuitTask.QUEST_COWS_DELIGHT => TaskDifficulty.EASY,
                CircuitTask.QUEST_MAYORS_SHORTS => TaskDifficulty.EASY,
                CircuitTask.QUEST_CARVING_PUMPKINS => TaskDifficulty.EASY,

                //medium
                CircuitTask.EQUIP_TWO_UNIQUE_PARTY_HATS => TaskDifficulty.MEDIUM,
                CircuitTask.FAIR_DISQUALIFY => TaskDifficulty.MEDIUM,
                CircuitTask.CC_COMPLETE_VAULT => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_SAM_CACTUS_FRUIT => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_LEAH_GOAT_CHEESE => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_EMILY_CLOTH => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_CAROLINE_GREEN_TEA => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_CLINT_GOLD_BAR => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_EVELYN_CHOCOLATE_CAKE => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_GUS_ORANGE => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_JAS_PINK_CAKE => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_KENT_ROASTED_HAZELNUT => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_LEWIS_GLAZED_YAM => TaskDifficulty.MEDIUM,
                CircuitTask.DANCE_AT_FLOWER_DANCE => TaskDifficulty.MEDIUM,
                CircuitTask.COMPLETE_SPIRITS_EVE_MAZE => TaskDifficulty.MEDIUM,
                CircuitTask.GIFT_LOVED_GIFT_AT_WINTER_FEAST => TaskDifficulty.MEDIUM,
                CircuitTask.UPGRADE_TOOL_STEEL => TaskDifficulty.MEDIUM,
                CircuitTask.THREE_CONCURRENT_BUFFS => TaskDifficulty.MEDIUM,
                CircuitTask.CRAFT_TUB_OF_FLOWERS => TaskDifficulty.MEDIUM,
                CircuitTask.COMPLETE_SPECIAL_ORDER => TaskDifficulty.MEDIUM,
                CircuitTask.RECEIVE_MERMAID_SHOW_PEARL => TaskDifficulty.MEDIUM,
                CircuitTask.CATCH_ANGLER => TaskDifficulty.MEDIUM,
                CircuitTask.GET_HIT_BY_TRAIN => TaskDifficulty.MEDIUM,
                CircuitTask.BUILD_BIG_COOP => TaskDifficulty.MEDIUM,
                CircuitTask.BUILD_BIG_BARN => TaskDifficulty.MEDIUM,
                CircuitTask.ANIMAL_DUCK => TaskDifficulty.MEDIUM,
                CircuitTask.ANIMAL_GOAT => TaskDifficulty.MEDIUM,
                CircuitTask.BUILD_FISH_POND => TaskDifficulty.MEDIUM,
                CircuitTask.EQUIP_PIRATE_HAT => TaskDifficulty.MEDIUM,
                CircuitTask.EQUIP_PROPELLER_HAT => TaskDifficulty.MEDIUM,
                CircuitTask.TALK_TO_LEWIS_WITH_SHORTS => TaskDifficulty.MEDIUM,
                CircuitTask.EVENT_ABIGAIL_TWO_HEART => TaskDifficulty.MEDIUM,
                CircuitTask.EVENT_SAM_TWO_HEART => TaskDifficulty.MEDIUM,
                CircuitTask.EVENT_SHANE_TWO_HEART => TaskDifficulty.MEDIUM,
                CircuitTask.EVENT_LEAH_TWO_HEART => TaskDifficulty.MEDIUM,
                CircuitTask.EVENT_MARU_TWO_HEART => TaskDifficulty.MEDIUM,
                CircuitTask.EVENT_PENNY_TWO_HEART => TaskDifficulty.MEDIUM,
                CircuitTask.QUEST_FRESH_FRUIT => TaskDifficulty.MEDIUM,
                CircuitTask.QUEST_AQUATIC_RESEARCH => TaskDifficulty.MEDIUM,
                CircuitTask.QUEST_WANTED_LOBSTER => TaskDifficulty.MEDIUM,
                CircuitTask.QUEST_FISH_CASSSEROLE => TaskDifficulty.MEDIUM,
                CircuitTask.CC_COMPLETE_CRAFTS_ROOM => TaskDifficulty.MEDIUM,
                CircuitTask.CC_COMPLETE_BOILER_ROOM => TaskDifficulty.MEDIUM,
                CircuitTask.JUNIMO_KART_LEWIS_SCORE => TaskDifficulty.MEDIUM,
                CircuitTask.ACHIEVEMENT_COOK => TaskDifficulty.MEDIUM,
                CircuitTask.ACHIEVEMENT_HOMESTEADER => TaskDifficulty.MEDIUM,
                CircuitTask.ACHIEVEMENT_NEW_FRIEND => TaskDifficulty.MEDIUM,
                CircuitTask.ACHIEVEMENT_CLIQUES => TaskDifficulty.MEDIUM,
                CircuitTask.ACHIEVEMENT_MONOCULTURE => TaskDifficulty.MEDIUM,
                CircuitTask.ACHIEVEMENT_ARTISAN => TaskDifficulty.MEDIUM,

                // hard
                CircuitTask.STARDEW_FAIR_GRANGE_DISPLAY => TaskDifficulty.HARD,
                CircuitTask.GIFT_ALEX_COMPLETE_BREAKFAST => TaskDifficulty.HARD,
                CircuitTask.GIFT_ELLIOTT_DUCK_FEATHER => TaskDifficulty.HARD,
                CircuitTask.GIFT_DWARF_GEM => TaskDifficulty.HARD,
                CircuitTask.GIFT_WILLY_OCTOPUS => TaskDifficulty.HARD,
                CircuitTask.GIFT_JODI_DIAMOND => TaskDifficulty.HARD,
                CircuitTask.GIFT_MARNIE_PUMPKIN_PIE => TaskDifficulty.HARD,
                CircuitTask.UPGRADE_TOOL_GOLD => TaskDifficulty.HARD,
                CircuitTask.SHIP_ANCIENT_FRUIT => TaskDifficulty.HARD,
                CircuitTask.COLLECT_THREE_RARECROWS => TaskDifficulty.HARD,
                CircuitTask.OBTAIN_ANY_THREE_SECRET_STATUES => TaskDifficulty.HARD,
                CircuitTask.CC_COMPLETE_PANTRY => TaskDifficulty.HARD,
                CircuitTask.CC_COMPLETE_FISH_TANK => TaskDifficulty.HARD,
                CircuitTask.CC_COMPLETE_BULLETIN_BOARD => TaskDifficulty.HARD,
                CircuitTask.CRAFT_ALL_SEASONAL_SEEDS => TaskDifficulty.HARD,
                CircuitTask.CRAFT_WILD_BAIT => TaskDifficulty.HARD,
                CircuitTask.CATCH_CRIMSONFISH => TaskDifficulty.HARD,
                CircuitTask.BUILD_DELUXE_COOP => TaskDifficulty.HARD,
                CircuitTask.BUILD_DELUXE_BARN => TaskDifficulty.HARD,
                CircuitTask.BUILD_STABLE => TaskDifficulty.HARD,
                CircuitTask.ANIMAL_BUNNY => TaskDifficulty.HARD,
                CircuitTask.ANIMAL_SHEEP => TaskDifficulty.HARD,
                CircuitTask.ANIMAL_PIG => TaskDifficulty.HARD,
                CircuitTask.INCUBATE_DINO_EGG => TaskDifficulty.HARD,
                CircuitTask.EVENT_ALEX_FOUR_HEART => TaskDifficulty.HARD,
                CircuitTask.EVENT_ELLIOTT_FOUR_HEART => TaskDifficulty.HARD,
                CircuitTask.EVENT_HALEY_FOUR_HEART => TaskDifficulty.HARD,
                CircuitTask.EVENT_EVELYN_FOUR_HEART => TaskDifficulty.HARD,
                CircuitTask.QUEST_SOLDIERS_STAR => TaskDifficulty.HARD,
                CircuitTask.QUEST_LINGCOD => TaskDifficulty.HARD,
                CircuitTask.TAPPER_ON_MUSHROOM_TREE => TaskDifficulty.HARD,
                CircuitTask.GROW_GIANT_CROP => TaskDifficulty.HARD,
                CircuitTask.OBTAIN_GOLDEN_SCYTHE => TaskDifficulty.HARD,
                CircuitTask.MONSTER_SLAYER => TaskDifficulty.HARD,
                CircuitTask.ACHIEVEMENT_GOFER => TaskDifficulty.HARD,
                CircuitTask.ACHIEVEMENT_MOTHER_CATCH => TaskDifficulty.HARD,
                CircuitTask.ACHIEVEMENT_OL_MARINER => TaskDifficulty.HARD,
                CircuitTask.ACHIEVEMENT_THE_BOTTOM => TaskDifficulty.HARD,
                CircuitTask.ACHIEVEMENT_SOUS_CHEF => TaskDifficulty.HARD,

                // expert
                CircuitTask.DATE_ANYONE => TaskDifficulty.EXPERT,
                CircuitTask.THREE_CANDLES => TaskDifficulty.EXPERT,
                CircuitTask.UNLOCK_CASINO => TaskDifficulty.EXPERT,
                CircuitTask.GIFT_ANY_IRIDIUM_LIKED_LOVED => TaskDifficulty.EXPERT,
                CircuitTask.ACHIEVEMENT_MILLIONAIRE => TaskDifficulty.EXPERT,
                CircuitTask.ACHIEVEMENT_TREASURE_TROVE => TaskDifficulty.EXPERT,
                CircuitTask.GIFT_KROBUS_PUMPKIN => TaskDifficulty.EXPERT,
                CircuitTask.GIFT_PIERRE_FRIED_CALAMARI => TaskDifficulty.EXPERT,
                CircuitTask.UPGRADE_TOOL_IRIDIUM => TaskDifficulty.EXPERT,
                CircuitTask.OBTAIN_GALAXY_SWORD => TaskDifficulty.EXPERT,
                CircuitTask.EVENT_HARVEY_SIX_HEART => TaskDifficulty.EXPERT,
                CircuitTask.EVENT_SEBASTION_SIX_HEART => TaskDifficulty.EXPERT,
                CircuitTask.EVENT_EMILY_SIX_HEART => TaskDifficulty.EXPERT,
                _ => throw new NotImplementedException($"{task} missing from switch")
            };
        }
    }
}
