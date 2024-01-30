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
using StardewDruid.Event.Boss;
using StardewDruid.Event.Challenge;
using StardewDruid.Event.Scene;
using StardewDruid.Journal;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;
using static StardewValley.Minigames.TargetGame;

namespace StardewDruid.Map
{
    internal static class QuestData
    {

        public static int StaticVersion() => 152;

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
                        }
                    }
                }
            }

            staticData.activeBlessing = "none";

            List<string> source = RitesProgress();

            if (source.Count > 0)
            {
                staticData.activeBlessing = source.Last<string>();
            }

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

                    string questName = questData.name.Replace("Two", "");

                    switch (questName)
                    {

                        case "challengeEarth": new Aquifer(vector, rite, questData).EventTrigger(); break;
                        case "challengeFates": new Quarry(vector, rite, questData).EventTrigger(); break;
                        case "challengeStars": new Infestation(vector, rite, questData).EventTrigger(); break;
                        case "challengeWater": new Graveyard(vector, rite, questData).EventTrigger(); break;
                        case "challengeCanoli": new Canoli(vector, rite, questData).EventTrigger(); break;
                        case "challengeMariner": new Mariner(vector, rite, questData).EventTrigger(); break;
                        case "challengeGemShrine": new GemShrine(vector, rite, questData).EventTrigger(); break;

                        case "challengeMuseum": new StardewDruid.Event.Boss.Museum(vector, rite, questData).EventTrigger(); break;
                        case "challengeEther": new StardewDruid.Event.Boss.Crypt(vector, rite, questData).EventTrigger(); break;
                        case "swordEther": new StardewDruid.Event.Boss.SkullCavern(vector, rite, questData).EventTrigger(); break;
                        case "challengeSandDragon": new StardewDruid.Event.Boss.SandDragon(vector, rite, questData).EventTrigger(); break;

                    }

                    break;
            
            }
        
        }

        public static void MarkerInstance(GameLocation location, Quest quest)
        {

            if (!Context.IsMainPlayer)
            {

                List<string> safeTriggers = new() { "sword", "scythe", };

                if (!safeTriggers.Contains(quest.type))
                {
                    return;
                }

            }

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

                    triggerHandle = null;

                        if (Context.IsMainPlayer)
                        {

                            if ((Game1.getLocationFromName("CommunityCenter") as CommunityCenter).areasComplete[1])
                            {

                                CharacterData.CharacterLoad("Jester", "Mountain");

                                Mod.instance.characters["Jester"].SwitchSceneMode();

                            }

                        }

                    break;

                case "approachShadowtin":

                    triggerHandle = null;

                        if (location is Location.Crypt)
                        {
                            CharacterData.CharacterLoad("Shadowtin", location.Name);

                        }
                        else
                        {
                            CharacterData.CharacterLoad("Shadowtin", "FarmCave");

                        }

                        Mod.instance.characters["Shadowtin"].SwitchSceneMode();

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
                [30] = new() { "lessonBlast" },
                [31] = new() { "lessonDive" },
                [32] = new() { "lessonTreasure" },
                [33] = new() { "challengeEther" },
                [34] = new() { "approachShadowtin" },
            };
        }

        public static List<string> RitesProgress()
        {
            int num = Mod.instance.CurrentProgress();

            List<string> stringList = new();
            if (num > 1)
            {
                stringList.Add("weald");
            }

            if (num > 8)
            {
                stringList.Add("mists");
            }

            if (num > 15)
            {
                stringList.Add("stars");
            }

            if (num > 20)
            {
                stringList.Add("fates");
            }

            if (num > 27)
            {
                stringList.Add("ether");
            }

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
            { 
                stringList.Add("mists"); 
            }
            if (num > 14)
            {
                stringList.Add("stars"); 
            }
            if (num > 17)
            { 
                stringList.Add("hidden"); 
            }
            if (num > 18)
            { 
                stringList.Add("Jester"); 
            }
            if (num > 19)
            { 
                stringList.Add("fates"); 
            }
            if (num > 26)
            { 
                stringList.Add("ether"); 
            }
            if (num > 34)
            {
                stringList.Add("complete"); 
            }
            return stringList;
        }

        public static int MaxProgress()
        {
            return 35; //QuestProgress().Keys.ToList<int>().Last<int>() + 1;

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

            string rite = RitesProgress().Last();
            
            foreach (KeyValuePair<int, List<string>> keyValuePair in QuestProgress())
            {
                
                if (keyValuePair.Key >= num)
                {
                    
                    foreach (string str in keyValuePair.Value)
                    {
                        
                        if (str == questCheck)
                        {
                            
                            num = keyValuePair.Key;
                            
                            num++;
                            
                            Mod.instance.blessingList = RitesProgress();

                            if(Mod.instance.blessingList.Last() != rite)
                            {

                                Mod.instance.ChangeBlessing(Mod.instance.blessingList.Last());

                            }
 
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
                    questDescription = "There was a note amongst Grandpa's old tools about the farmcave. He never ventured there, out of reverence or fear of his predecessors and their ancient druidic traditions.",
                    questObjective = "Investigate the old farm cave. Press the rite button while the quest journal is open for the Stardew Druid journal.",
                    questReward = 100,
                    questProgress = 0
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
                    questTitle = "Druid Lesson 1: Villagers",
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
                    questTitle = "Druid 2: Rustling",
                    questDescription = "The wild spaces hold many riches, and some are guarded jealously.",
                    questObjective = "Rustle twenty bushes for forageables with Rite of the Weald. Unlocks wild seed gathering from grass.",//Might disturb bats. 
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
                    questTitle = "Druid 3: Flowers",
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
                    questTitle = "Druid 4: Crops",
                    questDescription = "The Farmer and the Druid share the same vision.",
                    questObjective = "Convert twenty planted seasonal wild seeds into domestic crops. Updates growth cycle of all planted seeds. Unlocks quality conversions.",
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
                    questTitle = "Druid 5: Rocks",
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
                    questTitle = "Druid 6: Hidden Power",
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
                    questTitle = "Druid 7: Cookouts",
                    questDescription = "Every Druid should know how to cook",
                    questObjective = "Create two cookouts by striking crafted campfires. Also works on the firepits at the Beach or by Linus' tent. Unlocks extra recipes. Strike scarecrows and Lightning rods for additional effects.",
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
                    questTitle = "Druid 8: Fishing",
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
                    questTitle = "Druid 9: Smite",
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
                    questTitle = "Druid 10: Summoning",
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
                    startTime = 1900,
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
                    questTitle = "Druid 11: Meteor Rain",
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
                    questDescription = "The Effigy senses a fateful encounter awaits on the bridge over the mountain ravine.",
                    questObjective = "Investigate the bridge between the mountain pass and the quarry once it has been restored.",
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
                    triggerColour = new Color(1f, 0.4f, 0.4f, 1f),
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
                    questTitle = "Druid 12: Whisk",
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
                    questTitle = "Druid 13: Tricks!",
                    questDescription = "Apparently the wielders of fate are the greatest magicians.",
                    questObjective = "Amuse or annoy five villagers with tricks. Quest completion enables a higher chance for good friendship.",
                    questReward = 1700,
                    questProgress = 2,
                    questDiscuss = "(Jester's eyes sparkle) Magic tricks! Fates are known for being the best at making others happy. Or soaked.",
                    taskCounter = 5,
                    taskFinish = "masterTrick"
                },
                ["lessonEnchant"] = new Quest()
                {
                    name = "lessonEnchant",
                    questId = 18465043,
                    questValue = 6,
                    questTitle = "Druid 14: Enchant",
                    questDescription = "Even a machine can answer to a higher purpose.",
                    questObjective = "Fill up ten farm machines with essence. Quest completion enables an instant complete effect for active machines. Requires solar or void essence as a source.",
                    questReward = 1800,
                    questProgress = 2,
                    questDiscuss = "So I checked out your 'machines'. Well built but... kind of clunky. What happens if you put something from the otherworld in them? Do you have solar or void essence from the otherworld?",
                    taskCounter = 10,
                    taskFinish = "masterEnchant"
                },
                ["lessonGravity"] = new Quest()
                {
                    name = "lessonGravity",
                    questId = 18465044,
                    questValue = 6,
                    questTitle = "Druid 15: Gravity",
                    questDescription = "Nothing can escape the pull of fate.",
                    questObjective = "Summon five gravity wells by casting around scarecrows or monsters. Uses cursor targetting by default. Quest completion unlocks an additional comet effect when used in combination with Rite of the Stars.",
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
                    questTitle = "Druid 16: Daze",
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
                    questTitle = "Druid 17: Transform",
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
                    questTitle = "Druid 18: Flight",
                    questDescription = "Let your wings soar on the unseen streams of ether.",
                    questObjective = "Attempt a flight distance of four seconds. Press the action/use tool while transformed to begin flight. Hold to extend. Release to land. Performs a tail sweep attack instead if enemies are within close range.",
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
                    questTitle = "Druid 19: Blast",
                    questDescription = "Dragons possessed the ability to harness and focus ethereal potential for their own purposes.",
                    questObjective = "Perform ten firebreath attacks with the special button / right click. Uses directional targetting. Quest completion enables monster immolation.",
                    questReward = 2400,
                    questProgress = 2,
                    questDiscuss = "I'm not sure if this will work for Ether, but I can show you how I blast-face with essence.",
                    taskCounter = 10,
                    taskFinish = "masterBlast"
                },
                ["lessonDive"] = new Quest()
                {
                    name = "lessonDive",
                    questId = 18465054,
                    questValue = 6,
                    questTitle = "Druid 20: Dive",
                    questDescription = "It could be said that Dragons were the first to bury secrets in the depths.",
                    questObjective = "Fly onto the water and perform ten dives with the special button/right click.",
                    questReward = 2500,
                    questProgress = 2,
                    questDiscuss = "You might find this very unsettling, as I did when I learned of it's existence. Are you aware, that there is a strange technique that can be learned by some landborne creatures to stay alive in water. It's called swimming. It's unnatural.",
                    taskCounter = 10,
                    taskFinish = "masterDive"
                },
                ["lessonTreasure"] = new Quest()
                {
                    name = "lessonTreasure",
                    questId = 18465055,
                    questValue = 6,
                    questTitle = "Druid 21: Treasure",
                    questDescription = "The ancient masters of the valley ensured the the secrets of their dominion were vaulted within the Ethereal world.",
                    questObjective = "Claim seven dragon treasures. Search for the ether symbol on large map locations (a minimum map size applies). The color of the symbol will change depending on the terrain. Target the spot with blast or dive (special/right click button) to claim the dragon treasure.",
                    questReward = 2600,
                    questProgress = 2,
                    questDiscuss = "We're on a hunt for a way into the undervalley. Might as well hunt for Dragon treasure!",
                    taskCounter = 7,
                    taskFinish = "masterTreasure"
                },
                ["challengeEther"] = new Quest()
                {
                    name = "challengeEther",
                    type = "challenge",
                    triggerLocation = new() { "Town" },
                    triggerBlessing = "ether",
                    triggerMarker = "icon",
                    triggerVector = new Vector2(47f, 88f),
                    triggerColour = new Color(1f, 0.75f, 0.8f),
                    questId = 18465056,
                    questValue = 6,
                    questTitle = "The Ether Thieves",
                    questDescription = "The thieves that looted the treasure caches have been tracked to their hideout, an old burial chamber underneath the town graveyard.",
                    questObjective = "Perform a Rite of the Ether in the graveyard in Pelican Town.",
                    questReward = 15000,
                    questProgress = 1,
                    questDiscuss = "Woodface showed me how to use my nose to track the scent of prey. Then he told me to hunt vermin like a good farmcat. Am I a just a joke to him? Because I like jokes. Anyway, I tracked a different kind of pest, the thieves who have been raiding our dragon treasure! Their hideout is in an old crypt underneath the town.",
                },
                ["approachShadowtin"] = new Quest()
                {
                    name = "approachShadowtin",
                    type = "Shadowtin",
                    triggerLocale = new() { typeof(StardewDruid.Location.Crypt),typeof(FarmCave),},
                    triggerMarker = "character",
                    questId = 18465057,
                    questValue = 6,
                    questTitle = "Shadowtin Bear, Professional",
                    questDescription = "Your encounter with the ether thieves left a good impression on their ringleader.",
                    questObjective = "Introduce yourself to Shadowtin Bear. He will await you in the town crypt or in the farmcave.",
                    questReward = 100,
                    questProgress = 1,
                    questDiscuss = "Hey, great news! I invited the big ether thief to join our undervalley-search-party. Now we're sure to find the way there.",

                },

            };

            foreach (KeyValuePair<string, string> secondQuest in SecondQuests(true))
            {
                Quest quest = DeepClonerExtensions.ShallowClone<Quest>(dictionary[secondQuest.Key]);
                quest.name = secondQuest.Key + "Two";
                quest.questId += 100;
                quest.questTitle = secondQuest.Value;
                quest.questDescription = "This is a stronger version of the previous quest, with increased difficulty and better rewards.";
                quest.questReward *= 2;
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
                
                if (Mod.instance.QuestOpen(quest))
                {

                    stringList.Add(quest);

                }

            }
           
            return stringList;

        }

        public static Dictionary<string, string> SecondQuests(bool all = false)
        {

            int progress = Mod.instance.CurrentProgress();

            Dictionary<string, string> challenges = new Dictionary<string, string>()
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

                challenges.Add("challengeMariner", "The Seafarer Revisited");

            }

            if (Game1.player.hasOrWillReceiveMail("seenBoatJourney") || all)
            {

                challenges.Add("challengeGemShrine", "The Shrine Revisited");

            }

            if (progress > 26 || all)
            {

                challenges.Add("challengeFates", "The Fallen Revisited");

            }

            if (progress > 27 || all)
            {

                challenges.Add("swordEther", "The Tomb Revisited");

            }

            if (progress > 33 || all)
            {

                challenges.Add("challengeEther", "The Thieves Revisited");

            }

            if (all)
            {

                return challenges;

            }

            Dictionary<string, string> enabled = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> keyValuePair in challenges)
            {
                
                if (Mod.instance.QuestComplete(keyValuePair.Key))
                {

                    enabled.Add(keyValuePair.Key, keyValuePair.Value);
                
                }
                else if (!Mod.instance.QuestGiven(keyValuePair.Key))
                {

                    enabled.Add(keyValuePair.Key, keyValuePair.Value);
                
                }
                    
            }

            return enabled;

        }

        public static Quest RetrieveQuest(string quest)
        {

            return QuestList()[quest];

        }

    }

}
