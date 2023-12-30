/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Force.DeepCloner;
using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Event;
using StardewDruid.Event.Challenge;
using StardewDruid.Event.Scene;
using StardewDruid.Journal;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace StardewDruid.Map
{
    internal static class QuestData
    {

        public static int StaticVersion() => 152;

        public static int MaxProgress() => QuestProgress().Keys.ToList<int>().Last<int>() + 1;

        public static StaticData ConfigureProgress(StaticData staticData, int progressLevel)
        {
            staticData.questList = new Dictionary<string, bool>();
            staticData.taskList = new Dictionary<string, int>();
            staticData.characterList = new Dictionary<string, string>();
            staticData.activeProgress = progressLevel;
            staticData.staticVersion = StaticVersion();
            Dictionary<int, List<string>> dictionary1 = QuestProgress();
            Dictionary<string, Quest> dictionary2 = QuestList();
            foreach (KeyValuePair<int, List<string>> keyValuePair in dictionary1)
            {
                foreach (string key in keyValuePair.Value)
                {
                    if (dictionary2.ContainsKey(key))
                    {
                        Quest quest = dictionary2[key];
                        if (keyValuePair.Key < progressLevel)
                        {
                            if (quest.questId != 0)
                                staticData.questList[key] = true;
                            if (quest.taskFinish != null)
                                staticData.taskList[quest.taskFinish] = 1;
                            //if (quest.questCharacter != null)
                            //    staticData.characterList[quest.questCharacter] = "FarmCave";
                        }
                    }
                }
            }
            List<string> source = RitesProgress();
            staticData.activeBlessing = source.Last<string>();
            return staticData;
        }

        public static StaticData ReconfigureData(StaticData staticData)
        {
            foreach (KeyValuePair<int, string> keyValuePair in staticData.weaponAttunement)
            {
                if (keyValuePair.Value == "water")
                    staticData.weaponAttunement[keyValuePair.Key] = "mists";
                if (keyValuePair.Value == "earth")
                    staticData.weaponAttunement[keyValuePair.Key] = "weald";
            }
            if (staticData.activeBlessing == "water")
                staticData.activeBlessing = "mists";
            if (staticData.activeBlessing == "earth")
                staticData.activeBlessing = "weald";
            staticData.setProgress = -1;
            Dictionary<int, List<string>> dictionary = QuestProgress();
            List<int> list = dictionary.Keys.ToList<int>();
            for (int index = list.Count - 1; index >= 0; --index)
            {
                int key1 = list[index];
                foreach (string key2 in dictionary[key1])
                {
                    if (staticData.questList.ContainsKey(key2))
                    {
                        staticData = ConfigureProgress(staticData, key1 + 1);
                        return staticData;
                    }
                }
            }
            staticData = ConfigureProgress(staticData, 0);
            return staticData;
        }

        public static void QuestHandle(Vector2 vector, Rite rite, Quest questData)
        {
            switch (questData.type)
            {
                case "sword":
                    new Sword(vector, rite, questData).EventTrigger();
                    break;
                case "scythe":
                    new Scythe(vector, rite, questData).EventTrigger();
                    break;
                default:
                    ChallengeInstance(vector, rite, questData).EventTrigger();
                    break;
            }
        }

        public static ChallengeHandle ChallengeInstance(Vector2 target, Rite rite, Quest quest)
        {
            string questName = quest.name.Replace("Two", "");

            ChallengeHandle challengeHandle;

            switch (questName)
            {

                case "swordEther": challengeHandle = new Tyrannus(target, rite, quest); break;
                case "challengeFates": challengeHandle = new Quarry(target, rite, quest); break;
                case "challengeStars": challengeHandle = new Infestation(target, rite, quest); break;
                case "challengeWater": challengeHandle = new Graveyard(target, rite, quest); break;
                case "challengeCanoli": challengeHandle = new Canoli(target, rite, quest); break;
                case "challengeMuseum": challengeHandle = new Museum(target, rite, quest); break;
                case "challengeMariner": challengeHandle = new Mariner(target, rite, quest); break;
                case "challengeGemShrine": challengeHandle = new GemShrine(target, rite, quest); break;
                case "challengeSandDragon": challengeHandle = new SandDragon(target, rite, quest); break;
                default: challengeHandle = new Aquifer(target, rite, quest); break;


            }
            return challengeHandle;
        }

        public static void MarkerInstance(GameLocation location, Quest quest)
        {
            
            if (quest.triggerLocation != null && !quest.triggerLocation.Contains(location.Name))
            {
                return;

            }

            if (quest.triggerLocale != null && !quest.triggerLocale.Contains(location.GetType()))
            {

                return;

            }

            TriggerHandle triggerHandle;

            string questName = quest.name.Replace("Two", "");

            switch (questName)
            {
                case "approachJester":
                    CharacterData.CharacterLoad("Jester", "Mountain");
                    Mod.instance.characters["Jester"].SwitchFrozenMode();
                    triggerHandle = null;
                    break;
                case "challengeEarth":
                    triggerHandle = new Trash(location, quest);
                    break;
                case "approachEffigy":
                    triggerHandle = new Effigy(location, quest);
                    break;
                case "challengeMuseum":
                    triggerHandle = new Feature(location, quest);
                    break;
                default:
                    triggerHandle = new TriggerHandle(location, quest);
                    break;
            }

            if (triggerHandle == null || !triggerHandle.SetMarker())
            {
                return;
            }

            Mod.instance.markerRegister[quest.name] = triggerHandle;
        }

        public static Vector2 SpecialVector(GameLocation playerLocation, string questName)
        {
            questName = questName.Replace("Two", "");
            switch (questName)
            {
                case "challengeMariner":
                    if (playerLocation is Beach beach && Game1.isRaining && !Game1.isFestival())
                    {
                        object obj = typeof(Beach).GetField("oldMariner", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(beach);
                        if (obj != null)
                            return (obj as NPC).getTileLocation();
                        break;
                    }
                    break;
                case "challengeCanoli":
                    if (playerLocation is Woods)
                    {
                        Layer layer = playerLocation.Map.GetLayer("Buildings");
                        for (int index1 = 0; index1 < playerLocation.Map.DisplayWidth / 64; ++index1)
                        {
                            for (int index2 = 0; index2 < playerLocation.Map.DisplayHeight / 64; ++index2)
                            {
                                Tile tile = layer.Tiles[index1, index2];
                                if (tile != null)
                                {
                                    switch (tile.TileIndex)
                                    {
                                        case 1140:
                                        case 1141:
                                            return new Vector2(index1 + 1, index2);
                                        default:
                                            continue;
                                    }
                                }
                            }
                        }
                        break;
                    }
                    break;
                case "challengeSandDragon":
                    if (playerLocation is Desert)
                    {
                        Layer layer = playerLocation.Map.GetLayer("Buildings");
                        for (int index3 = 0; index3 < playerLocation.Map.DisplayWidth / 64; ++index3)
                        {
                            for (int index4 = 0; index4 < playerLocation.Map.DisplayHeight / 64; ++index4)
                            {
                                Tile tile = layer.Tiles[index3, index4];
                                if (tile != null)
                                {
                                    PropertyValue propertyValue;
                                    tile.Properties.TryGetValue("Action", out propertyValue);
                                    if (propertyValue != null && propertyValue.ToString() == "SandDragon")
                                        return new Vector2(index3 + 1, index4);
                                }
                            }
                        }
                        break;
                    }
                    break;
            }
            return new Vector2(-1f);
        }

        public static Dictionary<int, List<string>> QuestProgress()
        {
            return new Dictionary<int, List<string>>()
            {
                [0] = new() { "approachEffigy" },
                [1] = new() { "swordEarth" },
                [2] = new() { "lessonVillager" },
                [3] = new() { "lessonCreature" },
                [4] = new() { "lessonForage" },
                [5] = new() { "lessonCrop" },
                [6] = new() { "lessonRockfall" },
                [7] = new() { "challengeEarth" },
                [8] = new() { "swordWater" },
                [9] = new() { "lessonTotem" },
                [10] = new() { "lessonCookout" },
                [11] = new() { "lessonFishspot" },
                [12] = new() { "lessonSmite" },
                [13] = new() { "lessonPortal" },
                [14] = new() { "challengeWater" },
                [15] = new() { "swordStars" },
                [16] = new() { "lessonMeteor" },
                [17] = new() { "challengeStars" },
                [18] = new()
                {
                    "challengeCanoli",
                    "challengeMariner",
                    "challengeSandDragon",
                    "challengeGemShrine",
                    "challengeMuseum"
                },
                [19] = new() { "approachJester" },
                [20] = new() { "swordFates" },
                [21] = new() { "lessonWhisk" },
                [22] = new() { "lessonTrick" },
                [23] = new() { "lessonEnchant" },
                [24] = new() { "lessonGravity" },
                [25] = new() { "lessonDaze" },
                [26] = new() { "challengeFates" },
                [27] = new() { "swordEther" },
                [28] = new() { "lessonTransform" },
                [29] = new() { "lessonFlight" },
                [30] = new() { "lessonBlast" }
            };
        }

        public static List<string> RitesProgress()
        {
            int num = Mod.instance.CurrentProgress();
            List<string> stringList = new();
            if (num > 1)
                stringList.Add("weald");
            if (num > 8)
                stringList.Add("mists");
            if (num > 15)
                stringList.Add("stars");
            if (num > 20)
                stringList.Add("fates");
            if (num > 27)
                stringList.Add("ether");
            return stringList;
        }

        public static List<string> StageProgress()
        {
            int num = Mod.instance.CurrentProgress();

            List<string> stringList = new()
            {
                "weald"
            };
            if (num > 7)
                stringList.Add("mists");
            if (num > 14)
                stringList.Add("stars");
            if (num > 17)
                stringList.Add("hidden");
            if (num > 18)
                stringList.Add("Jester");
            if (num > 19)
                stringList.Add("fates");
            if (num > 26)
                stringList.Add("ether");
            if (num > 30)
                stringList.Add("complete");
            return stringList;
        }

        public static string NextProgress()
        {
            string str = "none";

            int progress = Mod.instance.CurrentProgress();

            Dictionary<int, List<string>> quests = QuestProgress();

            if (!quests.ContainsKey(progress))
            {
                return "none";

            }

            foreach (string quest in quests[progress])
            {
                if (quest == "approachJester")
                {

                    if (!(Game1.getLocationFromName("CommunityCenter") as CommunityCenter).areasComplete[1])
                    {
                        return "none";
                    }

                }

                if (!Mod.instance.QuestGiven(quest))
                {

                    Mod.instance.NewQuest(quest);
                
                }
                    
                str = quest;
            }

            return str;

        }

        public static int AchieveProgress(string questCheck)
        {
            int num = Mod.instance.CurrentProgress();
            foreach (KeyValuePair<int, List<string>> keyValuePair in QuestProgress())
            {
                if (keyValuePair.Key >= num)
                {
                    foreach (string str in keyValuePair.Value)
                    {
                        if (!(str != questCheck))
                        {
                            num = keyValuePair.Key;
                            ++num;
                            Mod.instance.blessingList = RitesProgress();
                            return num;
                        }
                    }
                }
            }
            return num;
        }

        public static Dictionary<string, Quest> QuestList()
        {
            Dictionary<string, Quest> dictionary = new Dictionary<string, Quest>()
            {
                ["approachEffigy"] = new Quest()
                {
                    name = "approachEffigy",
                    type = "Effigy",
                    triggerLocale = new List<Type>()
                    {
                    typeof (FarmCave)
                    },
                    triggerMarker = "icon",
                    triggerVector = (CharacterData.CharacterPosition() / 64f) + new Vector2(-1f, -2f),
                    questId = 18465001,
                    //questCharacter = "Effigy",
                    questValue = 6,
                    questTitle = "The Druid's Effigy",
                    questDescription = "There was a note amongst Grandpa's old tools about the farmcave. He left it alone, out of reverence or fear of his predecessors and their ancient druidic traditions.",
                    questObjective = "Investigate the old farm cave. Press the rite button while the quest journal is open for the Stardew Druid journal.",
                    questReward = 100,
                    questProgress = 1
                },
                ["swordEarth"] = new Quest()
                {
                    name = "swordEarth",
                    type = "sword",
                    triggerLocation = new() { "Forest" },
                    triggerMarker = "icon",
                    triggerVector = new Vector2(40f, 10f),
                    questId = 18465005,
                    questValue = 6,
                    questTitle = "The Two Kings",
                    questDescription = "The Effigy has directed you to a source of otherworldly power, the two Kings, who have the dominion over the weald.",
                    questObjective = "Malus trees bloom pink in the spring. Find the biggest specimen in the southern forest and perform a rite there.",
                    questReward = 100,
                    questProgress = 1,
                    questDiscuss = "Seek the patronage of the two Kings, and wield the power of the weald. Find the giant malus of the southern forest and perform a rite below it's boughs."
                },
                ["lessonVillager"] = new Quest()
                {
                    name = "lessonVillager",
                    questId = 18465011,
                    questValue = 6,
                    questTitle = "Druid Lesson One: Villagers",
                    questDescription = "Demonstrate your command over nature to the locals.",
                    questObjective = "Perform a rite in range of four different villagers. (Doesn't have to be all at once).",
                    questReward = 100,
                    questProgress = 2,
                    questDiscuss = "Good. You are now a subject of the two kingdoms, and bear authority over the weed and the twig. Use this new power to drive out decay and detritus. Return tomorrow for another lesson.",
                    taskCounter = 4,
                    taskFinish = "masterVillager"
                },
                ["lessonCreature"] = new Quest()
                {
                    name = "lessonCreature",
                    questId = 18465012,
                    questValue = 6,
                    questTitle = "Druid Lesson Two: Creatures",
                    questDescription = "The wild spaces hold many riches, and some are guarded jealously.",
                    questObjective = "Rustle twenty bushes for forageables with Rite of the Weald. Might disturb bats. Unlocks wild seed gathering from grass.",
                    questReward = 200,
                    questProgress = 2,
                    questDiscuss = "The valley springs into new life. Go now, sample its hidden bounty, and prepare to face those who guard its secrets.",
                    taskCounter = 20,
                    taskFinish = "masterCreature"
                },
                ["lessonForage"] = new Quest()
                {
                    name = "lessonForage",
                    questId = 18465013,
                    questValue = 6,
                    questTitle = "Druid Lesson Three: Flowers",
                    questDescription = "The Druid fills the barren spaces with life.",
                    questObjective = "Sprout ten forageables total in the Forest or elsewhere. Unlocks flowers.",
                    questReward = 300,
                    questProgress = 2,
                    questDiscuss = "Years of stagnation have starved the valley of it's wilderness. Go now, and recolour the barren spaces.",
                    taskCounter = 10,
                    taskFinish = "masterForage"
                },
                ["lessonCrop"] = new Quest()
                {
                    name = "lessonCrop",
                    questId = 18465014,
                    questValue = 6,
                    questTitle = "Druid Lesson Four: Crops",
                    questDescription = "The Farmer and the Druid share the same vision.",
                    questObjective = "Convert twenty planted wild seeds into domestic crops. Unlocks quality conversions.",
                    questReward = 400,
                    questProgress = 2,
                    questDiscuss = "Your connection to the earth deepens. You may channel the power of the Two Kings for your own purposes.",
                    taskCounter = 20,
                    taskFinish = "masterCrop"
                },
                ["lessonRockfall"] = new Quest()
                {
                    name = "lessonRockfall",
                    questId = 18465015,
                    questValue = 6,
                    questTitle = "Druid Lesson Five: Rocks",
                    questDescription = "The Druid draws power from the deep rock.",
                    questObjective = "Gather one hundred stone from rockfalls in the mines. Unlocks rockfall damage to monsters.",
                    questReward = 500,
                    questProgress = 2,
                    questDiscuss = "Be careful in the mines. The deep earth answers your call, both above and below you.",
                    taskCounter = 100,
                    taskFinish = "masterRockfall"
                },
                ["challengeEarth"] = new Quest()
                {
                    name = "challengeEarth",
                    type = "challenge",
                    triggerLocation = new()
                    {
                    "UndergroundMine20"
                    },
                    triggerBlessing = "weald",
                    triggerMarker = "icon",
                    triggerVector = new Vector2(25f, 13f),
                    questId = 18465002,
                    questValue = 6,
                    questTitle = "The Polluted Aquifier",
                    questDescription = "The mountain spring, from an aquifer of special significance to the otherworld, has been polluted by rubbish dumped in the abandoned mineshafts.",
                    questObjective = "Perform a Rite of the Weald over the aquifier in level 20 of the mines.",
                    questReward = 1000,
                    questProgress = 1,
                    questDiscuss = "A trial presents itself. Foulness has begun to seep from a spring once cherished by the monarchs. You must travel there, and cleanse the source with the blessing of the Kings."
                },
                ["swordWater"] = new Quest()
                {
                    name = "swordWater",
                    type = "sword",
                    triggerLocation = new() { "Beach" },
                    triggerMarker = "icon",
                    triggerVector = new Vector2(87f, 39f),
                    triggerColour = new Color(0.4f, 0.4f, 1f, 1f),
                    questId = 18465006,
                    questValue = 6,
                    questTitle = "The Voice Beyond the Shore",
                    questDescription = "The Effigy wants you to introduce yourself to another source of otherworldly power.",
                    questObjective = "Find the far eastern pier at the beach and perform a rite there.",
                    questReward = 100,
                    questProgress = 1,
                    questDiscuss = "The Voice Beyond the Shore harkens to you now. Perform a rite at the furthest pier, and behold her power."
                },
                ["lessonTotem"] = new Quest()
                {
                    name = "lessonTotem",
                    questId = 18465016,
                    questValue = 6,
                    questTitle = "Druid Lesson Six: Hidden Power",
                    questDescription = "The power of the valley gathers where ley lines intersect.",
                    questObjective = "Draw the power out of two different warp shrines. Unlocks chance for double extraction.",
                    questReward = 600,
                    questProgress = 2,
                    questDiscuss = "Good. The Lady Beyond the Shore has answered your call. Find the shrines to the patrons of the Valley, and strike them to draw out a portion of their essence. Do the same to any obstacle in your way.",
                    taskCounter = 2,
                    taskFinish = "masterTotem"
                },
                ["lessonCookout"] = new Quest()
                {
                    name = "lessonCookout",
                    questId = 18465017,
                    questValue = 6,
                    questTitle = "Druid Lesson Seven: Cookouts",
                    questDescription = "Every Druid should know how to cook",
                    questObjective = "Create two cookouts from campfires. Craft your own, or look around the Beach or Linus' tent. Unlocks extra recipes.",
                    questReward = 700,
                    questProgress = 2,
                    questDiscuss = "The Lady is fascinated by the industriousness of humanity. Combine your artifice with her blessing and reap the rewards.",
                    taskCounter = 2,
                    taskFinish = "masterCookout"
                },
                ["lessonFishspot"] = new Quest()
                {
                    name = "lessonFishspot",
                    questId = 18465018,
                    questValue = 6,
                    questTitle = "Druid Lesson Eight: Fishing",
                    questDescription = "Nature is always ready to test your skill. Create fishing spots by performing Rite of the Mists over open water.",
                    questObjective = "Cast on open water to create fishing spots and catch ten fish. Quest completion unlocks rarer fish.",
                    questReward = 800,
                    questProgress = 2,
                    questDiscuss = "The denizens of the deep water serve the Lady. Go now, and test your skill against them.",
                    taskCounter = 10,
                    taskFinish = "masterFishspot"
                },
                ["lessonSmite"] = new Quest()
                {
                    name = "lessonSmite",
                    questId = 18465019,
                    questValue = 6,
                    questTitle = "Druid Lesson Nine: Smite",
                    questDescription = "Call lightning down upon your enemies",
                    questObjective = "Smite enemies twenty times. Unlocks critical hits.",
                    questReward = 900,
                    questProgress = 2,
                    questDiscuss = "Your connection to the plane beyond broadens. Call upon the Lady's Voice to destroy your foes.",
                    taskCounter = 20,
                    taskFinish = "masterSmite"
                },
                ["lessonPortal"] = new Quest()
                {
                    name = "lessonPortal",
                    questId = 18465020,
                    questValue = 6,
                    questTitle = "Druid Lesson Ten: Summoning",
                    questDescription = "Who knows what lies beyond the veil",
                    questObjective = "Create a portal in the backwoods, railroad or secret woods by placing down a candle torch and striking it with Rite of the Mists. Every candle included in the rite increases the challenge.",
                    questReward = 1000,
                    questProgress = 2,
                    questDiscuss = "Are you yet a master of the veil between worlds? Focus your will to breach the divide.",
                    taskCounter = 1,
                    taskFinish = "masterPortal"
                },
                ["challengeWater"] = new Quest()
                {
                    name = "challengeWater",
                    type = "challenge",
                    triggerLocation = new() { "Town" },
                    startTime = 700,
                    triggerBlessing = "mists",
                    triggerMarker = "icon",
                    triggerVector = new Vector2(47f, 88f),
                    triggerColour = new Color(0.4f, 0.4f, 1f, 1f),
                    questId = 18465003,
                    questValue = 6,
                    questTitle = "The Shadow Invasion",
                    questDescription = "The monsters you summoned from the otherworld whispered about a secretive gathering in a dark corner of the village.",
                    questObjective = "Perform a Rite of the Mists in Pelican Town's graveyard between 7:00 pm and Midnight.",
                    questReward = 3000,
                    questProgress = 1,
                    questDiscuss = "I have heard the whispers of monsters summoned from beyond the veil. The village is threatened from where the barrier between the worlds has thinned most, in the hallowed grounds of the graveyard. You must answer this threat in the name of the Lady and Kings."
                },
                ["swordStars"] = new Quest()
                {
                    name = "swordStars",
                    type = "sword",
                    triggerLocation = new()
                    {
                    "UndergroundMine100"
                    },
                    triggerMarker = "icon",
                    triggerVector = new Vector2(25f, 13f),
                    triggerColour = new Color(1f, 0.4f, 0.4f, 1f),
                    questId = 18465007,
                    questValue = 6,
                    questTitle = "The Stars Beyond the Expanse",
                    questDescription = "The Effigy wants you to introduce yourself to another source of otherworldly power.",
                    questObjective = "Reach the lake of flame in level 100 of the mines and perform a rite there.",
                    questReward = 100,
                    questProgress = 1,
                    questDiscuss = "Your name is known within the celestial plane. Travel to the lake of flames under the halls of the lava folk. Retrieve the final vestige of the first farmer."
                },
                ["lessonMeteor"] = new Quest()
                {
                    name = "lessonMeteor",
                    questId = 18465021,
                    questValue = 6,
                    questTitle = "Druid Lesson Eleven: Meteor Shower",
                    questDescription = "Call down a meteor shower to clear the area around you.",
                    questObjective = "Summon fifty meteors. Unlocks priority targetting of stone nodes and monsters.",
                    questReward = 1200,
                    taskCounter = 50,
                    taskFinish = "masterMeteor",
                    questProgress = 2,
                    questDiscuss = "Excellent. The Stars Beyond the Expanse have chosen a new champion to wield their power."
                },
                ["challengeStars"] = new Quest()
                {
                    name = "challengeStars",
                    type = "challenge",
                    triggerBlessing = "stars",
                    triggerLocation = new() { "Forest" },
                    triggerMarker = "icon",
                    triggerVector = new Vector2(79f, 78f),
                    triggerColour = new Color(1f, 0.4f, 0.4f, 1f),
                    questId = 18465004,
                    questValue = 6,
                    questTitle = "The Slime Infestation",
                    questDescription = "Many of the trees in the local forest have been marred with a slimy substance. Has the old enemy of the farm returned?",
                    questObjective = "Perform a Rite of the Stars in the clearing east of arrowhead island in Cindersap Forest.",
                    questReward = 4500,
                    questProgress = 1,
                    questDiscuss = "The last trial of your apprenticeship awaits. The southern forest reeks of our mortal enemy, the all-consuming jelly. Rain judgement upon the slimes with the blessing of the Stars."
                },
                ["challengeCanoli"] = new Quest()
                {
                    name = "challengeCanoli",
                    type = "challenge",
                    triggerLocale = new List<Type>()
                    {
                    typeof (Woods)
                    },
                    triggerMarker = "icon",
                    triggerBlessing = "mists",
                    triggerColour = new Color(0.4f, 0.4f, 1f, 1f),
                    questId = 18465031,
                    questValue = 6,
                    questTitle = "The Dusting",
                    questDescription = "The secret woods is traced in dust.",
                    questObjective = "Perform a Rite of the Mists over the statue in the secret woods",
                    questReward = 2500,
                    questProgress = 1
                },
                ["challengeMariner"] = new Quest()
                {
                    name = "challengeMariner",
                    type = "challenge",
                    triggerLocale = new List<Type>()
                    {
                    typeof (Beach)
                    },
                    triggerBlessing = "mists",
                    triggerMarker = "icon",
                    triggerColour = new Color(0.4f, 0.4f, 1f, 1f),
                    questId = 18465032,
                    questValue = 6,
                    questTitle = "The Seafarer's Woes",
                    questDescription = "Much of the Gem Sea remains uncharted. Where have all the marine folk gone?",
                    questObjective = "Perform a Rite of the Mists over the ghost of the old mariner during a rainy day at the eastern beach.",
                    questReward = 2500,
                    questProgress = 1
                },
                ["challengeSandDragon"] = new Quest()
                {
                    name = "challengeSandDragon",
                    type = "challenge",
                    triggerLocale = new List<Type>()
                    {
                    typeof (Desert)
                    },
                    triggerBlessing = "stars",
                    triggerMarker = "icon",
                    triggerColour = new Color(1f, 0.4f, 0.4f, 1f),
                    questId = 18465033,
                    questValue = 6,
                    questTitle = "Tyrant Of The Sands",
                    questDescription = "The Calico desert bears the scars of a cataclysmic event.",
                    questObjective = "Perform a Rite of the Stars over the sun-bleached bones in the desert.",
                    questReward = 2500,
                    questProgress = 1
                },
                ["challengeGemShrine"] = new Quest()
                {
                    name = "challengeGemShrine",
                    type = "challenge",
                    triggerLocale = new List<Type>()
                    {
                    typeof (IslandShrine)
                    },
                    triggerBlessing = "stars",
                    triggerVector = new Vector2(24f, 27f),
                    triggerMarker = "icon",
                    triggerColour = new Color(1f, 0.4f, 0.4f, 1f),
                    questId = 18465034,
                    questValue = 6,
                    questTitle = "The Forgotten Shrine",
                    questDescription = "A strange presence lingers at a long forgotten shrine.",
                    questObjective = "Perform a rite of the stars between the pedestals of the Ginger Island forest shrine.",
                    questReward = 2500,
                    questProgress = 1
                },
                ["challengeMuseum"] = new Quest()
                {
                    name = "challengeMuseum",
                    type = "challenge",
                    triggerLocale = new List<Type>()
                    {
                    typeof (LibraryMuseum)
                    },
                    triggerBlessing = "mists",
                    triggerVector = new Vector2(17f, 10f),
                    triggerMarker = "icon",
                    triggerColour = new Color(0.4f, 0.4f, 1f, 1f),
                    questId = 18465035,
                    questValue = 6,
                    questTitle = "The Feature",
                    questDescription = "Something is wrong with a piece from Gunther's latest exhibit, an overly large helmet Marlon retrieved from the 'dino cavern'",
                    questObjective = "Perform a rite of the mists over the large battle helmet on display in the village museum.",
                    questReward = 2500,
                    questProgress = 1
                },
                ["approachJester"] = new Quest()
                {
                    name = "approachJester",
                    type = "Jester",
                    questId = 18465040,
                    triggerLocation = new()
                    {
                    "Mountain"
                    },
                    triggerMarker = "character",
                    //questCharacter = "Jester",
                    questValue = 6,
                    questTitle = "Across The Bridge",
                    questDescription = "Now that the bridge across the ravine is restored, the Effigy senses a fateful encounter awaits across the divide.",
                    questObjective = "Investigate the bridge between the mountain pass and the quarry.",
                    questReward = 100,
                    questProgress = 1
                },
                ["swordFates"] = new Quest()
                {
                    name = "swordFates",
                    type = "scythe",
                    questId = 18465047,
                    triggerLocation = new()
                    {
                    "UndergroundMine77377"
                    },
                    triggerMarker = "icon",
                    triggerVector = new Vector2(30f, 7f),
                    triggerColour = new Color(1f, 0.8f, 0.4f, 1f),
                    questValue = 6,
                    questTitle = "Instrument of Fate",
                    questDescription = "The Jester of Fate thinks it would be a good idea if you armed yourself with an instrument of Deat- uh - Fate.",
                    questObjective = "Travel to the end of the quarry tunnel and perform a rite at the shrine there.",
                    questReward = 1500,
                    questProgress = 1,
                    questDiscuss = "For a mortal to even comprehend the forces of destiny, they'll need an instrument of fate itself. Past the bridge where we first met is a cave. If I recall my family history correctly, there's a shrine to one of my kin inside, and maybe something you can use!"
                },
                ["lessonWhisk"] = new Quest()
                {
                    name = "lessonWhisk",
                    questId = 18465041,
                    questValue = 6,
                    questTitle = "Druid Lesson Twelve: Whisk",
                    questDescription = "The denizens of the otherworld traverse the material plane in immaterial ways.",
                    questObjective = "Complete ten warp jumps. Press the rite button to launch a warp projectile, then press action to trigger the warp. Uses cursor targetting by default. Quest completion extends the jump range.",
                    questReward = 1600,
                    questProgress = 2,
                    questDiscuss = "Now you can travel like the Others do. It's fun, but also a bit, well, (Jester grins). I'm sure you'll get used to it.",
                    taskCounter = 10,
                    taskFinish = "masterWhisk"
                },
                ["lessonTrick"] = new Quest()
                {
                    name = "lessonTrick",
                    questId = 18465042,
                    questValue = 6,
                    questTitle = "Druid Lesson Thirteen: Tricks!",
                    questDescription = "Apparently the wielders of fate are the greatest magicians.",
                    questObjective = "Amuse or annoy five villagers with tricks. Quest completion enables a higher chance for good friendship. Requires solar or void essence as a source.",
                    questReward = 1700,
                    questProgress = 2,
                    questDiscuss = "(Jester's eyes sparkle) Magic tricks! Fates are known for being the best at making others happy. Or soaked. Do you have essence from the otherworld? You will need either solar or void on hand.",
                    taskCounter = 5,
                    taskFinish = "masterTrick"
                },
                ["lessonEnchant"] = new Quest()
                {
                    name = "lessonEnchant",
                    questId = 18465043,
                    questValue = 6,
                    questTitle = "Druid Lesson Fourteen: Enchant",
                    questDescription = "Even a machine can answer to a higher purpose.",
                    questObjective = "Fill up ten farm machines with essence. Quest completion enables an instant complete effect for active machines. Requires solar or void essence as a source.",
                    questReward = 1800,
                    questProgress = 2,
                    questDiscuss = "So I checked out your 'machines'. Well built but... kind of clunky. What happens if you put something from the otherworld in them?",
                    taskCounter = 10,
                    taskFinish = "masterEnchant"
                },
                ["lessonGravity"] = new Quest()
                {
                    name = "lessonGravity",
                    questId = 18465044,
                    questValue = 6,
                    questTitle = "Druid Lesson Sixteen: Gravity",
                    questDescription = "Nothing can escape the pull of fate.",
                    questObjective = "Summon five gravity wells by casting around scarecrows or monsters. Uses cursor targetting by default. Quest completion improves pull radius. Requires solar or void essence as a source.",
                    questReward = 1900,
                    questProgress = 2,
                    questDiscuss = "(Jester looks determined) Alright farmer, time for some serious power. You want to take on big game? Nothing escapes a locus of fate, not even fate itself.",
                    taskCounter = 5,
                    taskFinish = "masterGravity"
                },
                ["lessonDaze"] = new Quest()
                {
                    name = "lessonDaze",
                    questId = 18465045,
                    questValue = 6,
                    questTitle = "Druid Lesson Sixteen: Daze",
                    questDescription = "The feeble mind cannot fathom it's own fate.",
                    questObjective = "Daze monsters ten times. Jester attacks and gravity wells add a dazzle debuff to nearby monsters. A warp strike can be triggered with the action button. Quest completion enables random morph effects.",
                    questReward = 2000,
                    questProgress = 2,
                    questDiscuss = "(Jester sighs) The fane creatures of this world cannot understand their own mundane purpose.",
                    taskCounter = 10,
                    taskFinish = "masterDaze"
                },
                ["challengeFates"] = new Quest()
                {
                    name = "challengeFates",
                    type = "challenge",
                    triggerBlessing = "stars",
                    triggerLocation = new()
                    {
                    "Mountain"
                    },
                    triggerMarker = "icon",
                    triggerVector = new Vector2(118f, 20f),
                    triggerColour = new Color(1f, 0.8f, 0.4f, 1f),
                    questId = 18465046,
                    questValue = 6,
                    questTitle = "Traces of the Fallen One",
                    questDescription = "The Jester of Fate believes a clue about the identity or whereabouts of an ancient entity lie in the abandoned quarry.",
                    questObjective = "Perform a Rite of the Stars in the center of the Mountain Quarry.",
                    questReward = 10000,
                    questProgress = 1,
                    questDiscuss = "Now I need your help. (Jester adopts a solemn tone). Every creature of Fate is born with a divine purpose. Of mine, there is little foretold, except a fragment of knowledge revealed to the oldest of my kin. (Jester's eyes narrow to cat slits) My path involves an ancient entity, with a name that has been kept hidden from me. The entity entered the valley once, in a time long past, and the site of their arrival is the abandoned quarry. It seems the secrets there are sealed with the power of the Stars. (Jester resumes his gleeful countenance) You possess the blessing of the stars, so you can perform the rite! See you there!"
                },
                ["swordEther"] = new Quest()
                {
                    name = "swordEther",
                    type = "challenge",
                    questId = 18465050,
                    triggerLocation = new()
                    {
                    "SkullCave"
                    },
                    triggerBlessing = "weald",
                    triggerMarker = "icon",
                    triggerVector = new Vector2(5f, 5f),
                    questValue = 6,
                    questTitle = "The Fate Of Tyrannus",
                    questDescription = "The Effigy has posited that information about the undervalley lies with the remains of Tyrannus Prime in the skull caverns. Jester is keen to accompany you on this quest.",
                    questObjective = "Travel to the Skull Caverns in the Calico Desert and perform a Rite of the Weald before the Skull door.",
                    questReward = 2100,
                    questProgress = 1,
                    questDiscuss = "Woodface remembered something about the undervalley! We need to find the writings of a bunch of long dead cultists. Out in the desert. In a cavern. Of skulls."
                },
                ["lessonTransform"] = new Quest()
                {
                    name = "lessonTransform",
                    questId = 18465051,
                    questValue = 6,
                    questTitle = "Druid Lesson 17: Transform",
                    questDescription = "Dragons were the ancient masters of the the ethereal plane. Now I have the means to become a master myself.",
                    questObjective = "The Rite of Ether transforms you into a Dragon! Impress or frighten five different villagers while in dragon form. Quest completion extends transformation time.",
                    questReward = 2200,
                    questProgress = 2,
                    questDiscuss = "Thanatoshi... he went after the fallen one too, but this is as far as he got. (Jester casts his sights downward) He must have tried to use the tooth of the Prime to create a path to the undervalley, but it proved too much for him, and drove him mad. I can't hope to succeed where he failed. (Jester looks hopefully at you) I think it's Fortumei's blessing that you found the dragontooth, farmer, with it you can do what me and Thanatoshi can't. You can assume the form of a Dragon and find a way to cross over.",
                    taskCounter = 5,
                    taskFinish = "masterTransform"
                },
                ["lessonFlight"] = new Quest()
                {
                    name = "lessonFlight",
                    questId = 18465052,
                    questValue = 6,
                    questTitle = "Druid Lesson 18: Flight",
                    questDescription = "Let your wings soar on the unseen streams of ether.",
                    questObjective = "Attempt a flight distance of four seconds. Hold the rite button while moving left or right to do a sweeping flight, release to land. Quest completion enables damage to foes.",
                    questReward = 2300,
                    questProgress = 2,
                    questDiscuss = "Have you figured out how to fly yet? I tried to practice using my tail as a spring to jump over trees. But I kept falling asleep.",
                    taskCounter = 1,
                    taskFinish = "masterFlight"
                },
                ["lessonBlast"] = new Quest()
                {
                    name = "lessonBlast",
                    questId = 18465053,
                    questValue = 6,
                    questTitle = "Druid Lesson 19: Blast",
                    questDescription = "Dragons possessed the ability to harness and focus elemental forces for their own purposes.",
                    questObjective = "Perform ten blast attacks. Press the special button/right click to blast nearby foes. Uses cursor and directional targetting. Quest completion extends the blast radius.",
                    questReward = 2400,
                    questProgress = 2,
                    questDiscuss = "I'm not sure if this will work for Ether, but I can show you how I blast-face with essence.",
                    taskCounter = 10,
                    taskFinish = "masterBlast"
                },
                ["lessonHunts"] = new Quest()
                {
                    name = "lessonHunts",
                    questId = 18465054,
                    questValue = 6,
                    questTitle = "Druid Lesson 20: Hunts",
                    questDescription = "The creatures of the ethereal plane have become greedy and brazen without the wisdom and oversight of the Dragons.",
                    questObjective = "Hunt down five treasure thieves and five essense bandits in locations around the valley. These opponents are visible only in dragon form. Quest completion enables you to consult the Jester about quintessence, a special quest resource.",
                    questReward = 2500,
                    questProgress = 2,
                    taskCounter = 5,
                    taskFinish = "masterHunts"
                },
                ["lessonAltar"] = new Quest()
                {
                    name = "lessonAltar",
                    questId = 18465055,
                    questValue = 6,
                    questTitle = "Druid Lesson 21: Altars",
                    questDescription = "The ancient guardians of the valley crafted and tended to special altars that drew on and enriched locuses of ethereal energy.",
                    questObjective = "Use quintessence to craft one altar in locations around the valley. Perform a Rite of the Ether at spots indicated by the altar marker on selected maps, such as Cindersap Forest and the Mountain.",
                    questReward = 2600,
                    questProgress = 2,
                    taskCounter = 1,
                    taskFinish = "masterAltar"
                },
                ["challengeEther"] = new Quest()
                {
                    name = "challengeEther",
                    type = "challenge",
                    triggerBlessing = "ether",
                    triggerLocation = new() { "Sewer" },
                    triggerMarker = "icon",
                    triggerVector = new Vector2(20f, 5f),
                    triggerColour = new Color(1f, 0.75f, 0.8f, 1f),
                    questId = 18465056,
                    questValue = 6,
                    questTitle = "Servant of the Fallen One",
                    questDescription = "Your investigation into the leylines of the valley has provided Jester with another clue about the whereabouts of the fallen one, and a dangerous figure that does it's bidding.",
                    questObjective = "Perform a Rite of the Ether at the point of disturbance in the sewers.",
                    questReward = 15000,
                    questProgress = 1
                }
            };
            foreach (KeyValuePair<string, string> secondQuest in SecondQuests())
            {
                Quest quest = DeepClonerExtensions.ShallowClone<Quest>(dictionary[secondQuest.Key]);
                quest.name = secondQuest.Key + "Two";
                quest.questId += 100;
                quest.questTitle = secondQuest.Value;
                quest.questReward = 5000;
                quest.questProgress = -1;
                dictionary[quest.name] = quest;
            }
            return dictionary;
        }

        public static List<string> ActiveSeconds()
        {
            
            Dictionary<string, string> dictionary = SecondQuests(true);
            
            List<string> stringList = new();
            
            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
            {
                
                string quest = keyValuePair.Key + "Two";
                
                if (Mod.instance.QuestGiven(quest) && !Mod.instance.QuestComplete(quest))
                {
                    
                    stringList.Add(quest);

                }

            }
           
            return stringList;

        }

        public static Dictionary<string, string> SecondQuests(bool all = false)
        {
            Dictionary<string, string> dictionary1 = new Dictionary<string, string>()
            {
                
                ["challengeEarth"] = "The Aquifer Revisited",
                
                ["challengeWater"] = "The Invasion Revisited",
                
                ["challengeStars"] = "The Infestation Revisited",
                
                ["challengeCanoli"] = "The Dusting Revisited",
                
                ["challengeSandDragon"] = "The Tyrant Revisited",
                
                ["challengeMuseum"] = "The Feature Revisited"

            };

            if (Game1.currentSeason != "winter" || all)
            {

                dictionary1.Add("challengeMariner", "The Seafarer Revisited");

            }

            if (Game1.player.hasOrWillReceiveMail("seenBoatJourney") || all)
            {

                dictionary1.Add("challengeGemShrine", "The Shrine Revisited");

            }

            if (StageProgress().Contains("ether") || all)
            {

                dictionary1.Add("challengeFates", "The Fallen Revisited");

            }

            if (all)
            {

                return dictionary1;

            }

            Dictionary<string, string> dictionary2 = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
            {
                
                if (Mod.instance.QuestComplete(keyValuePair.Key))
                {
                    
                    dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
                
                }
                else if (!Mod.instance.QuestGiven(keyValuePair.Key))
                {
                    
                    dictionary2.Add(keyValuePair.Key, keyValuePair.Value);
                
                }
                    
            }

            return dictionary2;

        }

        public static Quest RetrieveQuest(string quest) => QuestList()[quest];

        public static List<List<Page>> RetrievePages()
        {

            List<List<Page>> source = new List<List<Page>>();

            Dictionary<int, List<List<string>>> dictionary1 = JournalPages();

            int num = Mod.instance.CurrentProgress();

            Dictionary<string, int> dictionary2 = Mod.instance.TaskList();

            source.Add(new List<Page>());

            foreach (KeyValuePair<int, List<List<string>>> keyValuePair in dictionary1)
            {
                if (keyValuePair.Key <= num)
                {
                    foreach (List<string> stringList in keyValuePair.Value)
                    {
                        if (source.Last<List<Page>>().Count<Page>() == 6)
                            source.Add(new List<Page>());

                        switch (stringList[1])
                        {
                            case "quest":
                                if (stringList.Count > 5)
                                {
                                    if (Mod.instance.QuestGiven(stringList[5]))
                                    {
                                        Page page = new Page();
                                        page.title = stringList[0];
                                        page.icon = stringList[2];
                                        page.description = stringList[3];
                                        if (Mod.instance.QuestComplete(stringList[5]))
                                            page.objectives.Add(stringList[4]);
                                        else
                                            page.active = true;
                                        source.Last<List<Page>>().Add(page);
                                        break;
                                    }
                                    break;
                                }
                                Page page1 = new Page();
                                page1.title = stringList[0];
                                page1.icon = stringList[2];
                                page1.description = stringList[3];
                                if (num > keyValuePair.Key)
                                    page1.objectives.Add(stringList[4]);
                                source.Last<List<Page>>().Add(page1);
                                break;

                            case "lesson":
                                Page page2 = new Page();
                                page2.title = stringList[0];
                                page2.icon = stringList[2];
                                page2.description = stringList[3];
                                page2.objectives.Add("Lesson: " + stringList[4]);
                                if (dictionary2.ContainsKey(stringList[5]))
                                    page2.objectives.Add("Practice: " + dictionary2[stringList[5]].ToString() + " " + stringList[6]);
                                if (dictionary2.ContainsKey(stringList[7]))
                                    page2.objectives.Add("Mastery: " + stringList[8]);
                                else
                                    page2.active = true;
                                source.Last<List<Page>>().Add(page2);
                                break;

                            case "effect":
                                source.Last<List<Page>>().Add(new Page()
                                {
                                    title = stringList[0],
                                    icon = stringList[2],
                                    description = stringList[3],
                                    objectives = {
                                "Effect: " + stringList[4]
                                }
                                });
                                break;
                        }
                    }
                }
                else
                    break;
            }

            return source;

        }

        public static Dictionary<int, List<List<string>>> JournalPages()
        {
            return new Dictionary<int, List<List<string>>>()
            {
                [0] = new()
                {
                    new()
                    {
                    "Grandfather's note",
                    "quest",
                    "Effigy",
                    "There's a note stuck to grandpa's old scythe: \"For the new farmer. Before my forefathers came to the valley, the local farmcave was a frequent meeting ground for a circle of Druids. An eeriness hangs over the place, so I have kept my father's tradition of leaving it well enough alone. Perhaps you should too.\"",
                    "I heard a voice in the cave, and it bid me to speak words in a forgotten dialect. I raised my hands, recited a chant, and a wooden effigy crashed to the floor! It spoke to me and told me how I could learn the ways of the valley Druids. And so my apprenticeship begins..."
                    }
                },
                [1] = new()
                {
                    new()
                    {
                    "Squire of the Two Kings",
                    "quest",
                    "Weald",
                    "The Effigy says I need the favour of the Kings of Oak and Holly to begin my apprencticeship. He wants me to find a Malus tree in the forest and perform a rite of the weald under its boughs.",
                    "I found the Malus tree and paid homage to the Two Kings. The voices in the leaves made me a squire, and the sword I received seems to brim with forest magic.",
                    "swordEarth"
                    }
                },
                [2] = new()
                {
                    new()
                    {
                    "Effect: Rites of the Druids",
                    "effect",
                    "Weald",
                    "When I perform a rite once practiced by the valley druids, I feel the essence of the wild being drawn under me with each step.",
                    "Hold the rite button to steadily increase the range of the effect up to eight tiles away. You can run and ride a horse while holding the rite button. Performing a rite provides faster movement through grass and the consumption of roughage and other items to boost stamina."
                    },
                    new()
                    {
                    "Lesson: Community",
                    "lesson",
                    "Weald",
                    "The druids of antiquity played an important role in civil matters, as ceremonial leaders, mediators and physicians. The Rite of the Weald appears to have a positive effect on those who witness the rite.",
                    "Cast near NPCs and Farm animals to trigger daily dialogue counters and generate unique reactions.",
                    "lessonVillager",
                    "of 4 villagers witnessed rite",
                    "masterVillager",
                    "Small boost to friendship with villagers and farm animals who witness rite casts"
                    },
                    new()
                    {
                    "Effect: Banish Overgrowth",
                    "effect",
                    "Weald",
                    "When I inherited the farm from my Grandfather, it had become almost completely overrun with thicket. The Effigy has shown me how to make way for new growth.",
                    "Explode nearby weeds and twigs."
                    }
                },
                [3] = new()
                {
                    new()
                    {
                    "Lesson: Nature's Bounty",
                    "lesson",
                    "Weald",
                    "The Effigy has shown me how to gather the bounty of the wild, though some bounties are guarded jealously by creatures of the forest.",
                    "Extract foragables from large bushes, wood from trees, fibre from grass and small fish from water. Might disturb bats.",
                    "lessonCreature",
                    "of 20 bushes rustled with Rite",
                    "masterCreature",
                    "Wild seeds are gathered from grass."
                    }
                },
                [4] = new()
                {
                    new()
                    {
                    "Lesson: Wild Growth",
                    "lesson",
                    "Weald",
                    "The Druid fills the barren spaces with life. Seeds, sewn everywhere, freely, ready to sprout into tomorrow's wilderness.",
                    "Sprout trees, grass, seasonal forage and flowers in empty spaces.",
                    "lessonForage",
                    "of 10 forageables Sprouted",
                    "masterForage",
                    "Sprouts flowers."
                    }
                },
                [5] = new()
                {
                    new()
                    {
                    "Lesson: Druidic Agriculture",
                    "lesson",
                    "Weald",
                    "I have learned that the Farmer and the Druid share the same vision for a prosperous and well fed community, and so the wild seed is domesticated.",
                    "Cast over wild seeds sewn into tilled dirt to convert them into seasonal crops. Will also fertilise existing crops, and progress the growth rate of maturing fruit trees by one day (once per day).",
                    "lessonCrop",
                    "of 20 seeds converted",
                    "masterCrop",
                    "Wild seeds have a chance to convert into quality crops."
                    }
                },
                [6] = new()
                {
                    new()
                    {
                    "Lesson: Rockfall",
                    "lesson",
                    "Weald",
                    "The power of the two Kings resonates through the deep earth.",
                    "Cast in mineshafts to cause stones to fall from the ceiling.",
                    "lessonRockfall",
                    "of 100 Stone Collected",
                    "masterRockfall",
                    "Falling rocks damage monsters."
                    }
                },
                [7] = new()
                {
                    new()
                    {
                    "The Polluted Aquifer",
                    "quest",
                    "Weald",
                    "The mountain spring, from an aquifer of special significance to the otherworld, has been polluted by rubbish dumped in the abandoned mineshafts.",
                    "I reached a large cavern with a once pristine lake, and used the Rite of the Weald to purify it. There was so much trash, and so many bats. A big one in a skull mask claimed to serve a higher power, one with a vendetta against the polluters. The residents of the mountain and their friends are pleased with the cleanup; Sebastian, Sam, Maru, Abigail, Robin, Demetrius, Linus, ????.",
                    "challengeEarth"
                    }
                },
                [8] = new()
                {
                    new()
                    {
                    "The Voice Beyond the Shore",
                    "quest",
                    "Mists",
                    "The Effigy believes the protector of the mountain spring is none other than the Lady Beyond the Shore, and she has granted me an audience at the furthest pier of the beach. I'll have to repair the bridge to the tidal pools to reach it.",
                    "I called out across the waves, imagining my voice travelling over the gem sea to an isle of mystery and magic. Then a voice answered, and it was like lightning, my body shook with the words, and when I opened my eyes my hands held a weapon.",
                    "swordWater"
                    }
                },
                [9] = new()
                {   
                    new()
                    {
                    "Effect: Cursor Targetting",
                    "effect",
                    "Mists",
                    "The mists gather in front of me.",
                    "The Rite of the Mists uses directional and cursor based targetting to effect a point ahead of or away from the player, as opposed to centered-on-player targetting, so the direction and position of the farmer and/or mouse cursor is important to get precise hits."
                    },
                    new()
                    {
                    "Lesson: Totem Shrines",
                    "lesson",
                    "Mists",
                    "The old circle of druids left traces of their presence. Their work is visible in the delipidated structures and moss covered shrines of the valley. Some residual power remains.",
                    "Strike warp shrines once a day to extract totems.",
                    "lessonTotem",
                    "of 2 shrines struck",
                    "masterTotem",
                    "Chance for extra totems."
                    },
                    new()
                    {
                    "Effect: Trinket Tributes",
                    "effect",
                    "Mists",
                    "The Lady Beyond the Shore is often honoured with trinkets and little devices. There seem to be a lot of those buried around the Valley.",
                    "Extract items from artifact dig spots."
                    },
                    new()
                    {
                    "Effect: Sunder",
                    "effect",
                    "Mists",
                    "The Lady Beyond the Shore has granted me the power to remove common obstacles. Now I can be her representative to the further, wilder spaces of the valley.",
                    "Instantly destroy boulders and logs."
                    }
                },
                [10] = new()
                {
                    new()
                    {
                    "Lesson: Campfires",
                    "lesson",
                    "Mists",
                    "Druids were masters of the hearth and bonfire, often a central point for festive occasions. The raw energy from Rite of the Mists is precise enough to spark a controlled flame.",
                    "Strike crafted campfires and firepits throughout the valley to create cookouts. This includes the beach campfire, Linus's campfire, and the cliffs east of the poke' mouse house.",
                    "lessonCookout",
                    "of 2 cookouts created",
                    "masterCookout",
                    "Unlocks up to 16 base recipes."
                    },
                    new()
                    {
                    "Effect: Druidic Artifice",
                    "effect",
                    "Mists",
                    "The Lady is fascinated by the industriousness of humanity, and incorporating common farm implements into the Rite of the Mists produces interesting results",
                    "Strike scarecrows to produce a radial crop watering effect. Radius increases when watering can is upgraded to gold. Strike a lightning rod once a day to charge it."
                    }
                },
                [11] = new()
                {
                    new()
                    {
                    "Lesson: Master of the Rod",
                    "lesson",
                    "Mists",
                    "The denizens of the deep water serve the Lady Beyond the Shore. Rarer, bigger fish will gather where the Rite of the Mists strikes the open water.",
                    "Strike water at least three tiles away from water edge to produce a fishing-spot that yields rare species of fish. Cast the fishing line and wait for the fish mini-game to trigger automatically, then reel the fish in.",
                    "lessonFishspot",
                    "of 10 fish caught from fishing-spot",
                    "masterFishspot",
                    "Even rarer fish available to catch"
                    }
                },
                [12] = new()
                {
                    new()
                    {
                    "Lesson: Smite",
                    "lesson",
                    "Mists",
                    "I now have an answer for some of the more terrifying threats I've encountered in my adventures. Bolts of lightning strike at my foes.",
                    "Expend stamina to hit enemies with bolts of lightning.",
                    "lessonSmite",
                    "of 20 enemies hit with smite",
                    "masterSmite",
                    "Critical hit chance greatly increased for Smite."
                    },
                    new()
                    {
                    "Effect: Veil of Mist",
                    "effect",
                    "Mists",
                    "The discharge of lightning has the strange effect of drawing in mist that's imbued with the Lady's benevolence, and I feel myself invigorated when immersed in it.",
                    "A successful smite will spawn a misty zone at the player's position that provides regeneration every second."
                    }
                },
                [13] = new()
                {
                    new()
                    {
                    "Lesson: Ritual of Summoning",
                    "lesson",
                    "Mists",
                    "The druids would attempt to commune with spirits at times when the barrier between the material and ethereal world had waned. The Lady's power can punch right through the veil.",
                    "Strike candle torches that have been laid on the ground to produce a ritual of summoning, then fight off the monsters that step through the veil. The more candles included in the Rite (up to seven) the stronger the summoning, and the greater the rewards.",
                    "lessonPortal",
                    "ritual attempted",
                    "masterPortal",
                    "You summoned creatures from beyond the veil and survived"
                    }
                },
                [14] = new()
                {
                    new()
                    {
                    "The Shadow Invasion",
                    "quest",
                    "Mists",
                    "I summoned monsters from beyond the veil, and while I drove them all back to the ethereal plane, one thing became clear, that shadowfolk have the means to invade the valley from places where the veil is thinnest. The Effigy believes the graveyard is one such place. I will go and confront the shadow menace.",
                    "I confronted a small group of shadow scouts, and though they shot at me with bolts of darkness, the power of the Lady routed them. The leader spoke of a Deep One and a coming judgement. I must continue my Druidic apprenticeship so that I am strong enough to protect the Valley. My victory in the graveyard will ensure the safety of most of the townsfolk; Alex, Elliott, Harvey, Emily, Penny, Caroline, Clint, Evelyn, George, Gus, Jodi, Lewis, Pam, Pierre, Vincent, ????.",
                    "challengeWater"
                    }
                },
                [15] = new()
                {
                    new()
                    {
                    "Vestige of the First Farmer",
                    "quest",
                    "Stars",
                    "The first farmer possessed all the skills and knowledge I have attained in my journey to be a master of Druidry, but he also walked under the shining light of the celestials. There's a lake of flame deep in the mountain where the farmer offered his greatest weapon to the Stars Themselves.",
                    "I asked the voices in the flames to bless me as they had the farmer of yore, and I received his final vestige, newly forged in the heat of the mountain.",
                    "swordStars"
                    }
                },
                [16] = new()
                {
                    new()
                    {
                    "Lesson: Meteor Rain",
                    "lesson",
                    "Stars",
                    "If nature extends to the celestial realm, then the stars themselves are it's greatest force, a force now granted to me in order to burn away the taint and decay of a stagnated world.",
                    "Produce a meteor shower that strikes objects and monsters within the impact radii of random points around the Farmer. Will dislodge most set down objects.",
                    "lessonMeteor",
                    "of 50 meteors summoned",
                    "masterMeteor",
                    "Unlocks priority targetting of stone nodes and monsters"
                    }
                },
                [17] = new()
                {
                    new()
                    {
                    "The Slime Infestation",
                    "quest",
                    "Stars",
                    "Throughout my adventures I've engaged many a slime in combat. Unlike the bats and shadowfolk, I am uncertain of their origin or master, but while I spent time deep in the mountains looking for the lake of fire, a grand splattering of slime infested the forest. With the blessing of the Stars, I can confront even an army of jellies.",
                    "The pumpkin visaged king of the slimes mocked my lieges for leaving the valley wasted and unguarded. I am no longer a greenhorned Druid, but a fully fledged master of the Druidic tradition. The circle of Druids is reborn in the valley, but what role will it play in today's modernised society, I do not know. My victory in the forest has helped some of the villagers return to it's glens; Shane, Leah, Haley, Marnie, Jas, The Wizard, Willy, ????",
                    "challengeStars"
                    }
                },
                [18] = new()
                {
                    new()
                    {
                    "The Dusting",
                    "quest",
                    "Effigy",
                    "The Canoli was a gardener of the sacred groves who fell victim to his own avarice. His vines of sacred fruit have long since shrivelled and turned to dust, and the dust is everywhere.",
                    "I found a statue crafted in reverence of the Canoli, and when I attempted to destroy it with the Lady's power, I was immediately swamped by a horde of dust spirits. The woods breath easier without them, for now.",
                    "challengeCanoli"
                    },
                    new()
                    {
                    "The Tyrant of the Sands",
                    "quest",
                    "Effigy",
                    "The Effigy told me that the shamans of the Calico basin will have much lore and craft to share. He was confused when I told him I saw bones of ancient beasts where the huts of nomads should be, and an arid wasteland instead of once lush fields irrigated by a sacred body of water.",
                    "The Effigy's theory that the desert was devastated with star power proved correct. Some of the power I summoned disturbed an old evil, and it took everything I had to return the wretched spirit to the sands.",
                    "challengeSandDragon"
                    },
                    new()
                    {
                    "The Seafarers",
                    "quest",
                    "Effigy",
                    "The spectre of the old mariner is said to haunt the eastern beaches in times of rain and storm. The Effigy is not surprised by the supernatural phenomenon. If there were still Druids in the valley, they would have sent the ghost on to the next world long ago. So it is up to me.",
                    "The seafarers are restless on days where the Lady's storms and squalls rock the shoreline. They claim to have been drowned by the Lady herself, and that their vengeance is promised by the Deep One. A portent of war. I call out to the Lady for tidings, but she is silent.",
                    "challengeMariner"
                    },
                    new()
                    {
                    "The Gem Shrine",
                    "quest",
                    "Effigy",
                    "The Effigy tells me of a large volcanic island far out on the sea, where the central mountain hosted a fanatic cult of dwarven miners. They toiled without thought or care for the domain of the monarchs.",
                    "I found a shrine deep in the jungle, dwarven built, with pedestals for gems of brilliant lustre. I touched the site with the power of the Star Sisters, and something ominous chided me for it. Then fire breathing birds appeared. I had savoured some of the local mushrooms, so there's a possibility I hallucinated the whole thing.",
                    "challengeGemShrine"
                    },
                    new()
                    {
                    "The Feature",
                    "quest",
                    "Effigy",
                    "I've been assisting the museum caretaker with the curation of all the trinkets and minerals I've collected in my travels. One of the pieces Marlon's contributed to the collection, a battle helmet too large to fit a human, rattles in it's cabinet on odd occassions. Apparently the guildmaster found it in the 'dino cavern'.",
                    "The helmet activated at the touch of the Lady's power, and a furious battle erupted between myself and the spectre of a reptilian monster. When the creature was banished to the ether, though my clothes were slightly singed, Gunther was relieved to find none of the books or exhibit features showed any trace of damage.",
                    "challengeMuseum"
                    }
                },
                [19] = new()
                {
                    new()
                    {
                    "Fate Jests",
                    "quest",
                    "Jester",
                    "The apple bodied spirits of the forest accepted my offering of fruits and forageables. They have repaired the bridge to the western face of the mountain, and fate bids me to cross the ravine.",
                    "I met a strange cat-like being on the bridge, and a deal was struck to share the secrets of the Fates in exchange for my services in the cat's quest.",
                    "approachJester"
                    },
                    new()
                    {
                    "Gardener for the First Farmer",
                    "effect",
                    "Effigy",
                    "After my efforts to settle the ghosts of the past, the valley enjoys a respite from evil. It is time for the Effigy to walk amongst the furrows and fields of it's former master's home.",
                    "The Effigy can be invited to roam the farm, and will perform it's own version of Rite of the Weald where scarecrows have been placed. This version will plant new seed into empty tilled dirt, and water and fertilise existing crops in a radius around the scarecrow."
                    }
                },
                [20] = new()
                {
                    new()
                    {
                    "The Shrine of the Reaper",
                    "quest",
                    "Fates",
                    "The Jester of Fate seems unsure of how to begin my tutelage. For now, he has requested that I retrieve an instrument of fate from one it's kin, who is rumoured to have taken residence at a shrine within the western face of the mountain.",
                    "Another shrine in a forgotten alcove, unattended by the world, but seeped in the essence of the otherworld. I found a reaping scythe of unfathomable make, and learned that it belonged to Thanatoshi, a relative of Jester's who had long since vanished from the sight of the Fates.",
                    "swordFates"
                    }
                },
                [21] = new()
                {
                    new()
                    {
                    "Effect: Powered by Essence",
                    "effect",
                    "Fates",
                    "The efforts of the fates are sustained by Yoba.",
                    "This rite uses cursor and directional targetting as opposed to centered-on-player. Some of the effects of the Rite of the Fates require an offering of solar or void essence. It's prudent to collect and store this essence for when it will be useful to cast rites."
                    },
                    new()
                    {
                    "Effect: Teleport",
                    "effect",
                    "Fates",
                    "I surrender my being to destiny.",
                    "Hold the rite button without moving to warp to the furthest map exit in the direction you're facing."
                    },
                    new()
                    {
                    "Lesson: Whisk",
                    "lesson",
                    "Fates",
                    "The Fates are not constrained by the physical laws of this world.",
                    "Send out a warp projectile, then trigger a teleportation by pressing the action button before the projectile expires.",
                    "lessonWhisk",
                    "of 10 times whisked away",
                    "masterWhisk",
                    "Extra range unlocked."
                    }
                },
                [22] = new()
                {
                    new()
                    {
                    "Lesson: Magic Tricks",
                    "lesson",
                    "Fates",
                    "Druids train for many years to master the oral tradition, not only to safeguard esoteric knowledge, but to entertain and inspire their communities. Now, with Jester's help, I can add special effects.",
                    "Greet villagers with a special effect for a random amount of friendship.",
                    "lessonTrick",
                    "of 5 tricks performed",
                    "masterTrick",
                    "Chance for higher friendship gain."
                    }
                },
                [23] = new()
                {
                    new()
                    {
                    "Lesson: Enchant Machines",
                    "lesson",
                    "Fates",
                    "The otherworld has it's own devices of faye design, powered by essence of light and void. Now I can experiment with using essence to power my own artifice.",
                    "Enchant one of various types of farm machine to produce a randomised product without standard inputs. Each enchantment consumes one solar or void essence. Works on Deconstructors, Bone Mills, Kegs, Preserves Jars, Cheese Presses, Mayonnaise Machines, Looms, Oil Makers, Furnaces and Geode Crushers.",
                    "lessonEnchant",
                    "of 10 machines enchanted",
                    "masterEnchant",
                    "Machines in use can be enchanted to immediately finish production"
                    }
                },
                [24] = new()
                {
                    new()
                    {
                    "Lesson: Gravity Well",
                    "lesson",
                    "Fates",
                    "Meow. Nothing escapes fate. Meow. Jester's word's reverberate in my head as I watch the fabric of the world tighten into a knot of destiny.",
                    "Create gravity wells at the base of scarecrows and monsters. Nearby havestable crops will be harvested and pulled towards the scarecrow, with randomised quantities and quality. Monsters will be stunned and pulled in. Uses directional and cursor targetting.",
                    "lessonGravity",
                    "of 5 gravity wells created",
                    "masterGravity",
                    "Pull range increased"
                    }
                },
                [25] = new()
                {
                    new()
                    {
                    "Lesson: Warp Strike",
                    "lesson",
                    "Fates",
                    "Jester has taught me well. I interact with the otherworld in ways that would frighten the Druids of antiquity.",
                    "Jester attacks and Gravity wells incur a debuff that dazzles enemies. Warp strikes against dazed enemies can be triggered by using the action button.",
                    "lessonDaze",
                    "of 10 warp strikes performed",
                    "masterDaze",
                    "Strike range and damage increased"
                    }
                },
                [26] = new()
                {
                    new()
                    {
                    "Traces of the Fallen One.",
                    "quest",
                    "Fates",
                    "I am ready to honour my part of the bargain with the Jester of Fate. Now we go to the quarry to investigate traces of the fallen one. Could this mysterious quarry of Jester's be Lord Deep, the enigmatic master of those that have invaded the valley?",
                    "I performed a Rite of the Stars where the fallen one was said to have landed in the valley, and a portal to a dark region of the otherworld manifested. Violent creatures of raw essence spilled forth until the portal collapsed upon itself. There were no indicators of the Fallen One's, or Lord Deep's, presence. Jester believes further clues lie in the realm of the undervalley.",
                    "challengeFates"
                    }
                },
                [27] = new()
                {
                    new()
                    {
                    "The Fate Of Tyrannus",
                    "quest",
                    "Ether",
                    "The Effigy has remembered that the undervalley was once the ether-drenched domain of Tyrannus Prime, the ancient Lord attended to by the cult of Calico. The cultist shamans gathered unto themselves a wealth of knowledge about otherworldly things, and now I go with Jester to the Skull Caverns to search for clues as to their fate.",
                    "The Rite of the Weald unveiled the entrance to the desecrated throne room of Tyrannus Prime. Once inside, I was set upon by the wraith of Thanatoshi, who's mind had deteriorated with the burden of unfulfilled purpose. It seems the wraith was tethered to this world by the power of Prime's dragon tooth, which has been fashioned into a weapon of ethereal might.",
                    "swordEther",
                    },
                    new()
                    {
                    "Adventures with Jester",
                    "effect",
                    "Jester",
                    "Jester believes that his great purpose is intertwined with my own story. Despite the resoluton of our bargain, he will remain close by while he searches for a way to access the Undervalley.",
                    "Jester can be invited to roam the farm, or accompany you on journeys through the valley. He will automatically target nearby enemies, and his leaping melee attack applies the Daze effect. If positioned at right angles to a foe he can perform a powerful energy beam attack."
                    }
                },
                [28] = new()
                {
                    new()
                    {
                    "Lesson: Dragon Form",
                    "lesson",
                    "Ether",
                    "The Dragons have long been venerated as masters of the ether, the quintessential fifth element that defines the shape and nature of the spiritual domain. With the Dragontooth of Tyrannus Prime, I can adopt the guise of an Ancient One, and learn to master the ether myself.",
                    "Transform into a Dragon for one minute. Press the rite button again to detransform. Will also detransform when entering a new location.",
                    "lessonTransform",
                    "of 5 villagers impressed",
                    "masterTransform",
                    "Transformation time extended"
                    }
                },
                [29] = new()
                {
                    new()
                    {
                    "Lesson: Flight",
                    "lesson",
                    "Ether",
                    "I can feel the streams of ether rushing softly under my finger tips, and then my wing tips as I allow them to lift me into the sky.",
                    "Hold the rite button while moving left or right to do a sweeping flight, release to land.",
                    "lessonFlight",
                    "four-second flight times achieved",
                    "masterFlight",
                    "Nearby foes damaged on take off and landing"
                    }
                },
                [30] = new()
                {
                    new()
                    {
                    "Lesson: Blast",
                    "lesson",
                    "Ether",
                    "I can concentrate the ether within myself, and expel it as a torrent of violent energy.",
                    "Press the special button/right click to blast nearby foes. Uses cursor and directional targetting.",
                    "lessonBlast",
                    "of 10 blasts performed",
                    "masterBlast",
                    "Blast range increased"
                    }
                },
                [31] = new()
                {
                    new()
                    {
                    "Thank you!",
                    "effect",
                    "Effigy",
                    "Thank you for playing through Stardew Druid. " +
                    "I hope you had as much fun playing as I have had modding and sharing with the community. " +
                    "I have a lot more planned so please stay subscribed for future updates! " +
                    "Much love, Neosinf.",
                    "If you have the time, please consider endorsing the mod on Nexus and joining us on the Stardew Druid discord server!",
                    }
                }
            };

        }

    }

}
