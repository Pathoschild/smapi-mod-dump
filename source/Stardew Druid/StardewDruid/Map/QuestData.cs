/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Quests;
using StardewValley.Locations;
using System.Reflection;
using System.Runtime.InteropServices;
using Force.DeepCloner;

namespace StardewDruid.Map
{
    static class QuestData
    {

        public static int StableVersion()
        {

            return 116;

        }

        public static StaticData ConfigureProgress(StaticData staticData, int blessingLevel)
        {

            staticData.questList = new();

            staticData.blessingList["setProgress"] = blessingLevel;

            if (blessingLevel == 0)
            {
                return staticData;

            }

            Dictionary<string, Map.Quest> questIndex = QuestList();

            foreach (KeyValuePair<string, Quest> questData in questIndex)
            {

                if(questData.Value.questLevel < blessingLevel)
                {   

                    if(questData.Value.questId != 0)
                    {
                        staticData.questList[questData.Key] = true;
                        
                    }

                    if (questData.Value.taskFinish != null)
                    {

                        staticData.taskList[questData.Value.taskFinish] = 1;

                    }

                }

            }

            staticData.blessingList["earth"] = Math.Min(6, blessingLevel);
            staticData.activeBlessing = "earth";

            if (blessingLevel > 6)
            {
                staticData.blessingList["water"] = Math.Min(6,blessingLevel - 6);
                staticData.activeBlessing = "water";
            }
            if (blessingLevel > 12)
            {
                staticData.blessingList["stars"] = Math.Min(2, blessingLevel - 12);
                staticData.activeBlessing = "stars";
            }

            

            return staticData;

        }

        public static StaticData QuestCheck(StaticData staticData)
        {

            int blessingLevel = 0;

            if (staticData.blessingList.ContainsKey("earth"))
            {
                blessingLevel = staticData.blessingList["earth"];

            }

            if (staticData.blessingList.ContainsKey("water"))
            {
                blessingLevel = 6 + staticData.blessingList["water"];

            }

            if (staticData.blessingList.ContainsKey("stars"))
            {
                blessingLevel = 12 + staticData.blessingList["stars"];

            }

            staticData = ConfigureProgress(staticData, blessingLevel);

            return staticData;

        }

        public static Vector2 SpecialVector(GameLocation playerLocation, string triggerString)
        {

            switch (triggerString)
            {

                case "challengeMariner":

                    if (playerLocation is Beach beachLocation && Game1.isRaining && !Game1.isFestival())
                    {

                        Type reflectType = typeof(Beach);

                        FieldInfo reflectField = reflectType.GetField("oldMariner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                        var oldMariner = reflectField.GetValue(beachLocation);

                        if (oldMariner != null)
                        {

                            return (oldMariner as NPC).getTileLocation();

                        }

                    }

                    break;

                default:
                    break;


            }

            return new(-1);

        }

        public static Dictionary<string, Quest> QuestList()
        {

            Dictionary<string, Quest> questList = new()
            {
                ["approachEffigy"] = new()
                {

                    name = "approachEffigy",
                    questId = 18465001,
                    questValue = 6,
                    questTitle = "The Druid's Effigy",
                    questDescription = "Grandpa told me that the first farmers of the valley practiced an ancient druidic tradition.",
                    questObjective = "Investigate the effigy in the old farm cave.",
                    questReward = 100,
                    questLevel = 0,

                },
                ["swordEarth"] = new()
                {

                    name = "swordEarth",
                    triggerType = "sword",
                    triggerLocation = new() { "Forest", },
                    markerType = "icon",
                    vectorList = new()
                    {
                        ["triggerVector"] = new(38, 9),
                        ["triggerLimit"] = new(4, 4),
                        ["markerVector"] = new(40, 10),

                    },
                    questId = 18465005,
                    questValue = 6,
                    questTitle = "The Two Kings",
                    questDescription = "The Effigy has directed you to a source of otherworldly power.",
                    questObjective = "Malus trees bloom pink in the spring. Find the biggest specimen in the southern forest and perform a rite there.",
                    questReward = 100,
                    questLevel = 0,
                },
                ["swordWater"] = new()
                {

                    name = "swordWater",
                    triggerType = "sword",
                    triggerLocation = new() { "Beach", },
                    markerType = "icon",
                    vectorList = new()
                    {
                        ["triggerVector"] = new(82, 38),
                        ["triggerLimit"] = new(9, 3),
                        ["markerVector"] = new(87, 39),

                    },
                    questId = 18465006,
                    questValue = 6,
                    questTitle = "The Voice Beyond the Shore",
                    questDescription = "The Effigy wants you to introduce yourself to another source of otherworldly power.",
                    questObjective = "Find the far eastern pier at the beach and perform a rite there.",
                    questReward = 100,
                    questLevel = 6,
                },

                ["swordStars"] = new()
                {

                    name = "swordStars",
                    triggerType = "sword",
                    triggerLocation = new() { "UndergroundMine100", },
                    markerType = "icon",
                    vectorList = new()
                    {
                        ["triggerVector"] = new(24, 13),
                        ["triggerLimit"] = new(10, 10),
                        ["markerVector"] = new(25, 13),

                    },
                    questId = 18465007,
                    questValue = 6,
                    questTitle = "The Stars Themselves",
                    questDescription = "The Effigy wants you to introduce yourself to another source of otherworldly power.",
                    questObjective = "Find the lake of flame deep in the mountain and perform a rite there.",
                    questReward = 100,
                    questLevel = 12,
                },

                ["challengeEarth"] = new()
                {

                    name = "challengeEarth",
                    triggerType = "challenge",
                    triggerLocation = new() { "UndergroundMine20", },
                    markerType = "icon",
                    vectorList = new()
                    {
                        ["triggerVector"] = new(23, 12),
                        ["triggerLimit"] = new(4, 3),
                        ["markerVector"] = new(25, 13),
                    },
                    questId = 18465002,
                    questValue = 6,
                    questTitle = "The Polluted Aquifier",
                    questDescription = "The Effigy believes the mountain spring has been polluted by rubbish dumped in the abandoned mineshafts.",
                    questObjective = "Perform a rite over the aquifier in level 20 of the mines.",
                    questReward = 1000,
                    questLevel = 5,

                },
                ["challengeWater"] = new()
                {

                    name = "challengeWater",
                    triggerType = "challenge",
                    triggerLocation = new() { "Town", },
                    startTime = 700,
                    markerType = "icon",
                    vectorList = new()
                    {
                        ["triggerVector"] = new(45, 87),
                        ["triggerLimit"] = new(5, 5),
                        ["markerVector"] = new(47, 88),
                    },
                    questId = 18465003,
                    questValue = 6,
                    questTitle = "The Shadow Invasion",
                    questDescription = "The Effigy has heard the whispers of shadowy figures that loiter in the dark spaces of the village.",
                    questObjective = "Perform a rite in Pelican Town's graveyard between 7:00 pm and Midnight.",
                    questReward = 3000,
                    questLevel = 11,

                },
                ["challengeStars"] = new()
                {

                    name = "challengeStars",
                    triggerType = "challenge",
                    triggerLocation = new() { "Forest", },
                    markerType = "icon",
                    vectorList = new()
                    {
                        ["triggerVector"] = new(72, 71),
                        ["triggerLimit"] = new(14, 13),
                        ["markerVector"] = new(79, 78),
                    },
                    questId = 18465004,
                    questValue = 6,
                    questTitle = "The Slime Infestation",
                    questDescription = "Many of the trees in the local forest have been marred with a slimy substance. Has the old enemy of the farm returned?",
                    questObjective = "Perform a rite in the clearing east of arrowhead island in Cindersap Forest.",
                    questReward = 4500,
                    questLevel = 13,

                },

                ["challengeCanoli"] = new()
                {

                    name = "challengeCanoli",
                    triggerType = "challenge",
                    triggerLocale = new() { typeof(Woods), },
                    triggerBlessing = "water",
                    triggerTile = 1140,
                    triggerRadius = 2,
                    useTarget = true,
                    questLevel = 14,

                    questId = 18465031,
                    questValue = 6,
                    questTitle = "The Dusting",
                    questDescription = "The secret woods is traced in dust.",
                    questObjective = "Perform a rite of the water over the statue in the secret woods",
                    questReward = 2500,

                },

                ["challengeMariner"] = new()
                {

                    name = "challengeMariner",
                    triggerType = "challenge",
                    triggerLocale = new() { typeof(Beach), },
                    triggerBlessing = "water",
                    triggerSpecial = true,
                    useTarget = true,
                    questLevel = 14,

                    questId = 18465032,
                    questValue = 6,
                    questTitle = "The Seafarer's Woes",
                    questDescription = "Much of the Gem Sea remains unchartered. Where have all the marine folk gone?",
                    questObjective = "Perform a rite of the water over the ghost of the old mariner during a rainy day at the eastern beach.",
                    questReward = 2500,

                },

                ["challengeSandDragon"] = new()
                {

                    name = "challengeSandDragon",
                    triggerType = "challenge",
                    triggerLocale = new() { typeof(Desert), },
                    triggerBlessing = "stars",
                    triggerAction = "SandDragon",
                    triggerRadius = 3,
                    questLevel = 14,

                    questId = 18465033,
                    questValue = 6,
                    questTitle = "Tyrant Of The Sands",
                    questDescription = "The Calico desert bears the scars of a cataclysmic event.",
                    questObjective = "Perform a rite of the stars over the sun-bleached bones in the desert.",
                    questReward = 2500,

                },

                ["challengeGemShrine"] = new()
                {

                    name = "challengeGemShrine",
                    triggerType = "challenge",
                    triggerLocale = new() { typeof(IslandShrine), },
                    triggerBlessing = "stars",
                    triggerMilestone = "Island",
                    markerType = "icon",
                    vectorList = new()
                    {
                        ["triggerVector"] = new(22, 26),
                        ["triggerLimit"] = new(5, 3),
                        ["markerVector"] = new(24, 27),
                    },
                    questLevel = 14,

                    questId = 18465034,
                    questValue = 6,
                    questTitle = "The Forgotten Shrine",
                    questDescription = "A strange presence lingers at a long forgotten shrine.",
                    questObjective = "Perform a rite of the stars between the pedestals of the Ginger Island forest shrine.",
                    questReward = 2500,

                },

                ["lessonVillager"] = new() {

                    name = "lessonVillager",
                    questId = 18465011,
                    questValue = 6,
                    questTitle = "Druid Lesson One: Villagers",
                    questDescription = "Demonstrate your command over the Earth to the locals.",
                    questObjective = "Perform a rite in range of four different villagers. Unlocks friendship gain.",
                    questReward = 100,

                    taskCounter = 4,
                    taskFinish = "masterVillager",
                    questLevel = 1,
                },

                ["lessonCreature"] = new() {

                    name = "lessonCreature",
                    questId = 18465012,
                    questValue = 6,
                    questTitle = "Druid Lesson Two: Creatures",
                    questDescription = "The Earth holds many riches, and some are guarded jealously.",
                    questObjective = "Draw out ten local creatures. Unlocks wild seed gathering from grass.",
                    questReward = 200,

                    taskCounter = 10,
                    taskFinish = "masterCreature",
                    questLevel = 2,
                },

                ["lessonForage"] = new() {

                    name = "lessonForage",
                    questId = 18465013,
                    questValue = 6,
                    questTitle = "Druid Lesson Three: Flowers",
                    questDescription = "The Druid fills the barren spaces with life.",
                    questObjective = "Sprout ten forageables total in the Forest or elsewhere. Unlocks flowers.",
                    questReward = 300,

                    taskCounter = 10,
                    taskFinish = "masterForage",
                    questLevel = 3,
                },

                ["lessonCrop"] = new()
                {
                    name = "lessonCrop",
                    questId = 18465014,
                    questValue = 6,
                    questTitle = "Druid Lesson Four: Crops",
                    questDescription = "The Farmer and the Druid share the same vision.",
                    questObjective = "Convert twenty planted wild seeds into domestic crops. Unlocks quality conversions.",
                    questReward = 400,

                    taskCounter = 20,
                    taskFinish = "masterCrop",
                    questLevel = 4,
                },

                ["lessonRockfall"] = new()
                {
                    name = "lessonRockfall",
                    questId = 18465015,
                    questValue = 6,
                    questTitle = "Druid Lesson Five: Rocks",
                    questDescription = "The Druid draws power from the deep rock.",
                    questObjective = "Draw one hundred stone from rockfalls in the mines. Unlocks rockfall damage to monsters.",
                    questReward = 500,

                    taskCounter = 100,
                    taskFinish = "masterRockfall",
                    questLevel = 5,
                },

                ["lessonTotem"] = new()
                {
                    name = "lessonTotem",
                    questId = 18465016,
                    questValue = 6,
                    questTitle = "Druid Lesson Six: Hidden Power",
                    questDescription = "The power of the valley gathers where ley lines intersect.",
                    questObjective = "Draw the power out of two different warp shrines. Unlocks chance for double extraction.",
                    questReward = 600,

                    taskCounter = 2,
                    taskFinish = "masterTotem",
                    questLevel = 7,
                },

                ["lessonCookout"] = new()
                {
                    name = "lessonCookout",
                    questId = 18465017,
                    questValue = 6,
                    questTitle = "Druid Lesson Seven: Cookouts",
                    questDescription = "Every Druid should know how to cook",
                    questObjective = "Create two cookouts from campfires. Craft your own, or look around the Beach or Linus' tent. Unlocks extra recipes.",
                    questReward = 700,

                    taskCounter = 2,
                    taskFinish = "masterCookout",
                    questLevel = 8,
                },

                ["lessonFishspot"] = new()
                {

                    name = "lessonFishspot",
                    questId = 18465018,
                    questValue = 6,
                    questTitle = "Druid Lesson Eight: Fishing",
                    questDescription = "Nature is always ready to test your skill",
                    questObjective = "Catch ten fish from a spawning ground created by Rite of the Water. Unlocks rarer fish.",
                    questReward = 800,

                    taskCounter = 10,
                    taskFinish = "masterFishspot",
                    questLevel = 9,
                },

                ["lessonSmite"] = new()
                {
                    name = "lessonSmite",
                    questId = 18465019,
                    questValue = 6,
                    questTitle = "Druid Lesson Nine: Smite",
                    questDescription = "Call lightning down upon your enemies",
                    questObjective = "Smite enemies twenty times. Unlocks critical hits.",
                    questReward = 900,

                    taskCounter = 20,
                    taskFinish = "masterSmite",
                    questLevel = 10,
                },

                ["lessonPortal"] = new()
                {
                    name = "lessonPortal",
                    questId = 18465020,
                    questValue = 6,
                    questTitle = "Druid Lesson Ten: Portals",
                    questDescription = "Who knows what lies beyond the veil",
                    questObjective = "Create a portal in the backwoods, railroad or secret woods by placing down a candle torch and striking it with Rite of the Water. Every candle included in the rite increases the challenge.",
                    questReward = 1000,

                    taskCounter = 1,
                    taskFinish = "masterPortal",
                    questLevel = 11,
                },

                ["lessonMeteor"] = new()
                {
                    name = "lessonMeteor",
                    questId = 18465021,
                    questValue = 6,
                    questTitle = "Druid Lesson Eleven: Firerain",
                    questDescription = "Call down a meteor shower to clear areas for new growth",
                    questObjective = "Summon fifty fireballs. Unlocks priority targetting of stone nodes.",
                    questReward = 1200,

                    taskCounter = 50,
                    taskFinish = "masterMeteor",
                    questLevel = 13,
                },

            };

            Dictionary<string, string> secondList = SecondQuests();

            foreach(KeyValuePair<string,string> secondData in secondList)
            {

                Quest newQuest = questList[secondData.Key].ShallowClone();

                newQuest.name = secondData.Key + "Two";

                newQuest.questId += 100;
                newQuest.questTitle = secondData.Value;
                newQuest.questReward = 5000;
                newQuest.questLevel = 14;

                questList[newQuest.name] = newQuest;

            }

            return questList;

        }

        public static List<string> ChallengeQuests()
        {

            List<string> secondList = new()
            {

                "challengeCanoli",
                "challengeMariner",
                "challengeSandDragon",

            };

            if (Game1.player.hasOrWillReceiveMail("seenBoatJourney"))
            {

                secondList.Add("challengeGemShrine");

            }

            return secondList;

        }

        public static Dictionary<string, string> SecondQuests()
        {

            Dictionary<string, string> secondList = new()
            {
                ["challengeEarth"] = "The Aquifer Revisited",
                ["challengeWater"] = "The Invasion Revisited",
                ["challengeStars"] = "The Infestation Revisited",
                ["challengeCanoli"] = "The Dusting Revisited",
                ["challengeMariner"] = "The Seafarer Revisited",
                ["challengeSandDragon"] = "The Tyrant Revisited",
            };

            if (Game1.player.hasOrWillReceiveMail("seenBoatJourney"))
            {

                secondList.Add("challengeGemShrine", "The Shrine Revisited");

            }

            return secondList;

        }

        public static Quest RetrieveQuest(string quest)
        {

            Dictionary<string, Quest> questList = QuestList();

            return questList[quest];

        }

    }

}
