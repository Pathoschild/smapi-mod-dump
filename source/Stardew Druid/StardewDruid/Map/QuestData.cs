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
using StardewDruid.Cast;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using StardewModdingAPI;
using StardewDruid.Event.Challenge;
using StardewDruid.Event;

namespace StardewDruid.Map
{
    static class QuestData
    {

        public static int StableVersion()
        {

            // 116

            return 132;

        }

        public static int MaxProgress()
        {

            return 21;

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

                if(questData.Value.questLevel == -1)
                {

                    continue;

                }

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

                    if(questData.Value.questCharacter != null)
                    {

                        staticData.characterList[questData.Value.questCharacter] = "FarmCave";

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

            if (blessingLevel > 15)
            {
                staticData.blessingList["fates"] = Math.Min(6, blessingLevel - 15);

                staticData.activeBlessing = "fates";
            }

            return staticData;

        }

        public static StaticData QuestCheck(StaticData staticData)
        {

            int blessingLevel = BlessingCheck();

            staticData = ConfigureProgress(staticData, blessingLevel);

            return staticData;

        }

        public static int BlessingCheck()
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            int blessingLevel = 0;

            if (blessingList.ContainsKey("earth"))
            {
                blessingLevel = blessingList["earth"];

            }

            if (blessingList.ContainsKey("water"))
            {
                blessingLevel = 6 + blessingList["water"];

            }

            if (blessingList.ContainsKey("stars"))
            {
                blessingLevel = 12 + blessingList["stars"];

            }

            if (blessingList.ContainsKey("fates"))
            {
                blessingLevel = 15 + blessingList["fates"];

            }

            return blessingLevel;

        }

        public static bool ChallengeCompleted()
        {
            
            List<string> challengeQuests = ChallengeQuests();

            foreach(string challenge in challengeQuests)
            {

                if (!Mod.instance.QuestComplete(challenge))
                {

                    return false;

                }

            }

            return true;

        }

        public static bool JourneyCompleted()
        {

            return (Mod.instance.QuestComplete("challengeStars"));

        }

        public static bool JesterCompleted()
        {

            return (Mod.instance.QuestComplete("challengeFates"));

        }

        public static void IntroductionQuests()
        {

            Dictionary<string, bool> questList = Mod.instance.QuestList();

            if (!questList.ContainsKey("approachEffigy"))
            {
                // First quest
                Mod.instance.NewQuest("approachEffigy");

                return;

            }

            // Jester event triggered
            if (ChallengeCompleted() && (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).areasComplete[1] && !questList.ContainsKey("approachJester"))
            {

                Mod.instance.NewQuest("approachJester");

                Mod.instance.CharacterRegister("Jester", "Mountain");

                CharacterData.CharacterLoad("Jester", "Mountain");

                Mod.instance.characters["Jester"].SwitchFrozenMode();

                return;

            }

        }

        public static void QuestHandle(Vector2 vector, Rite rite, Quest questData)
        {

            switch (questData.type)
            {
                case "sword":

                    SwordHandle swordHandle = new(vector, rite, questData);

                    swordHandle.EventTrigger();

                    break;

                default: // challenge

                    ChallengeHandle challengeHandle = ChallengeInstance(vector, rite, questData);

                    challengeHandle.EventTrigger();

                    break;

            }

        }

        public static void MarkerInstance(GameLocation location, Quest quest)
        {

            if (quest.triggerLocation != null)
            {
                if (!quest.triggerLocation.Contains(location.Name))
                {

                    return;

                }

            }

            if (quest.triggerLocale != null)
            {

                if (!quest.triggerLocale.Contains(location.GetType()))
                {

                    return;

                }

            }

            TriggerHandle markerHandle;

            string questName = quest.name.ShallowClone();

            questName = questName.Replace("Two", "");

            switch (questName)
            {
                case "challengeEarth":

                    markerHandle = new Trash(location, quest);

                    break;

                case "approachEffigy":

                    markerHandle = new Event.Character.Effigy(location, quest);

                    break;

                default:

                    markerHandle = new TriggerHandle(location, quest);

                    break;
            }


            if (markerHandle != null)
            {

                if (markerHandle.SetMarker())
                {

                    Mod.instance.markerRegister[quest.name] = markerHandle;

                }

            }

        }

        public static ChallengeHandle ChallengeInstance(Vector2 target, Rite rite, Quest quest)
        {

            ChallengeHandle challengeHandle;

            string questName = quest.name;

            questName = questName.Replace("Two", "");

            switch (questName)
            {

                case "challengeCanoli":

                    challengeHandle = new Event.Challenge.Canoli(target, rite, quest);

                    break;

                case "challengeMariner":

                    challengeHandle = new Mariner(target, rite, quest);

                    break;

                case "challengeSandDragon": // figureSandDragon

                    challengeHandle = new SandDragon(target, rite, quest);

                    break;

                case "challengeStars":

                    challengeHandle = new Infestation(target, rite, quest);

                    break;

                case "challengeWater":

                    challengeHandle = new Graveyard(target, rite, quest);

                    break;

                case "challengeGemShrine":

                    challengeHandle = new GemShrine(target, rite, quest);

                    break;

                case "challengeFates":

                    challengeHandle = new Quarry(target, rite, quest);

                    break;

                default: //case "challengeEarth":

                    challengeHandle = new Event.Challenge.Aquifer(target, rite, quest);

                    break;
            }

            return challengeHandle;

        }

        public static Vector2 SpecialVector(GameLocation playerLocation, string questName)
        {

            questName = questName.Replace("Two", "");

            switch (questName)
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


                case "challengeCanoli":

                    if (playerLocation is Woods)
                    {   

                        Layer buildingLayer = playerLocation.Map.GetLayer("Buildings");

                        for (int i = 0; i < playerLocation.Map.DisplayWidth / 64; i++)
                        {

                            for(int j = 0; j < playerLocation.Map.DisplayHeight /64; j++)
                            {

                                Tile buildingTile = buildingLayer.Tiles[i, j];

                                if(buildingTile != null)
                                {

                                    int tileIndex = buildingTile.TileIndex;

                                    if ((uint)(tileIndex - 1140) <= 1u)
                                    {

                                        return new Vector2(i+1, j);

                                    }

                                }

                            }

                        }

                    }

                    break;

                case "challengeSandDragon":

                    if (playerLocation is Desert)
                    {

                        Layer buildingLayer = playerLocation.Map.GetLayer("Buildings");

                        for (int i = 0; i < playerLocation.Map.DisplayWidth / 64; i++)
                        {

                            for (int j = 0; j < playerLocation.Map.DisplayHeight / 64; j++)
                            {

                                Tile buildingTile = buildingLayer.Tiles[i, j];
                                
                                if (buildingTile != null)
                                {
                                    
                                    buildingTile.Properties.TryGetValue("Action", out var value3);
                                    
                                    if (value3 != null)
                                    {
                                        if (value3.ToString() == "SandDragon")
                                        {

                                            return new Vector2(i+1, j);

                                        }

                                    }

                                }

                            }

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
                    type = "Effigy",
                    triggerLocale = new() { typeof(FarmCave), },
                    triggerMarker = "icon",
                    triggerVector = (CharacterData.CharacterPosition("FarmCave") / 64) + new Vector2(-1,-2),
                    questId = 18465001,
                    questCharacter = "Effigy",
                    questValue = 6,
                    questTitle = "The Druid's Effigy",
                    questDescription = "Grandpa told me that the first farmers of the valley practiced an ancient druidic tradition.",
                    questObjective = "Investigate the old farm cave.",
                    questReward = 100,
                    questLevel = 0,

                },
                ["swordEarth"] = new()
                {

                    name = "swordEarth",
                    type = "sword",
                    triggerLocation = new() { "Forest", },
                    triggerMarker = "icon",
                    triggerVector = new(40, 10),
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
                    type = "sword",
                    triggerLocation = new() { "Beach", },
                    triggerMarker = "icon",
                    triggerVector = new(87, 39),
                    triggerColour = new(0.4f,0.4f,1f,1f),
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
                    type = "sword",
                    triggerLocation = new() { "UndergroundMine100", },
                    triggerMarker = "icon",
                    triggerVector = new(25, 13),
                    triggerColour = new(1f, 0.4f, 0.4f, 1f),
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
                    type = "challenge",
                    triggerLocation = new() { "UndergroundMine20", },
                    triggerBlessing = "earth",
                    triggerMarker = "icon",
                    triggerVector = new(25, 13),
                   
                    questId = 18465002,
                    questValue = 6,
                    questTitle = "The Polluted Aquifier",
                    questDescription = "The Effigy believes the mountain spring has been polluted by rubbish dumped in the abandoned mineshafts.",
                    questObjective = "Perform a Rite of the Earth over the aquifier in level 20 of the mines.",
                    questReward = 1000,
                    questLevel = 5,

                },
                ["challengeWater"] = new()
                {

                    name = "challengeWater",
                    type = "challenge",
                    triggerLocation = new() { "Town", },
                    startTime = 700,
                    triggerBlessing = "water",
                    triggerMarker = "icon",
                    triggerVector =  new(47, 88),
                    triggerColour = new(0.4f, 0.4f, 1f, 1f),
                    questId = 18465003,
                    questValue = 6,
                    questTitle = "The Shadow Invasion",
                    questDescription = "The Effigy has heard the whispers of shadowy figures that loiter in the dark spaces of the village.",
                    questObjective = "Perform a Rite of the Water in Pelican Town's graveyard between 7:00 pm and Midnight.",
                    questReward = 3000,
                    questLevel = 11,

                },
                ["challengeStars"] = new()
                {

                    name = "challengeStars",
                    type = "challenge",
                    triggerBlessing = "stars",
                    triggerLocation = new() { "Forest", },
                    triggerMarker = "icon",
                    triggerVector = new(79, 78),
                    triggerColour = new(1f, 0.4f, 0.4f, 1f),
                    questId = 18465004,
                    questValue = 6,
                    questTitle = "The Slime Infestation",
                    questDescription = "Many of the trees in the local forest have been marred with a slimy substance. Has the old enemy of the farm returned?",
                    questObjective = "Perform a Rite of the Stars in the clearing east of arrowhead island in Cindersap Forest.",
                    questReward = 4500,
                    questLevel = 13,

                },

                ["challengeCanoli"] = new()
                {

                    name = "challengeCanoli",
                    type = "challenge",
                    triggerLocale = new() { typeof(Woods), },
                    triggerMarker = "icon",
                    triggerBlessing = "water",
                    triggerColour = new(0.4f, 0.4f, 1f, 1f),
                    questLevel = 14,

                    questId = 18465031,
                    questValue = 6,
                    questTitle = "The Dusting",
                    questDescription = "The secret woods is traced in dust.",
                    questObjective = "Perform a Rite of the Water over the statue in the secret woods",
                    questReward = 2500,

                },

                ["challengeMariner"] = new()
                {

                    name = "challengeMariner",
                    type = "challenge",
                    triggerLocale = new() { typeof(Beach), },
                    triggerBlessing = "water",
                    triggerMarker = "icon",
                    triggerColour = new(0.4f, 0.4f, 1f, 1f),
                    questLevel = 14,

                    questId = 18465032,
                    questValue = 6,
                    questTitle = "The Seafarer's Woes",
                    questDescription = "Much of the Gem Sea remains unchartered. Where have all the marine folk gone?",
                    questObjective = "Perform a Rite of the Water over the ghost of the old mariner during a rainy day at the eastern beach.",
                    questReward = 2500,

                },

                ["challengeSandDragon"] = new()
                {

                    name = "challengeSandDragon",
                    type = "challenge",
                    triggerLocale = new() { typeof(Desert), },
                    triggerBlessing = "stars",
                    triggerMarker = "icon",
                    triggerColour = new(1f, 0.4f, 0.4f, 1f),
                    questLevel = 14,

                    questId = 18465033,
                    questValue = 6,
                    questTitle = "Tyrant Of The Sands",
                    questDescription = "The Calico desert bears the scars of a cataclysmic event.",
                    questObjective = "Perform a Rite of the Stars over the sun-bleached bones in the desert.",
                    questReward = 2500,

                },

                ["challengeGemShrine"] = new()
                {

                    name = "challengeGemShrine",
                    type = "challenge",
                    triggerLocale = new() { typeof(IslandShrine), },
                    triggerBlessing = "stars",
                    triggerVector = new(24, 27),
                    triggerMarker = "icon",
                    triggerColour = new(1f, 0.4f, 0.4f, 1f),
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
                    questDescription = "Nature is always ready to test your skill. Create fishing spots by performing Rite of the Water over open water.",
                    questObjective = "Try to catch ten fish from created fishing spots. Cast your line into the spot and wait for the fishing minigame to trigger automatically. Quest completion unlocks rarer fish.",
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

                //=======================================
                // Jester Content
                //=======================================

                ["approachJester"] = new()
                {

                    name = "approachJester",
                    questId = 18465040,
                    questCharacter = "Jester",
                    questValue = 6,
                    questTitle = "Across The Bridge",
                    questDescription = "Now that the bridge across the ravine is restored, the Effigy senses a fateful encounter awaits across the divide.",
                    questObjective = "Investigate the bridge between the mountain pass and the quarry.",
                    questReward = 1500,
                    questLevel = 15,

                },

                ["lessonWhisk"] = new()
                {
                    name = "lessonWhisk",
                    questId = 18465041,
                    questValue = 6,
                    questTitle = "Druid Lesson Twelve: Whisk",
                    questDescription = "The denizens of the otherworld traverse the material plane in immaterial ways.",
                    questObjective = "Complete ten warp jumps. Press the rite button to launch a warp projectile, then press action to trigger the warp. Uses cursor targetting by default. Quest completion extends the jump range.",
                    questReward = 1600,

                    taskCounter = 10,
                    taskFinish = "masterWhisk",
                    questLevel = 15,
                },

                ["lessonTrick"] = new()
                {

                    name = "lessonTrick",
                    questId = 18465042,
                    questValue = 6,
                    questTitle = "Druid Lesson Thirteen: Tricks!",
                    questDescription = "Apparently the wielders of fate are the greatest magicians.",
                    questObjective = "Amuse or annoy five villagers with tricks. Quest completion enables a higher chance for good friendship. Requires solar or void essence as a source.",
                    questReward = 1700,

                    taskCounter = 5,
                    taskFinish = "masterTrick",
                    questLevel = 16,
                },

                ["lessonEnchant"] = new()
                {
                    name = "lessonEnchant",
                    questId = 18465043,
                    questValue = 6,
                    questTitle = "Druid Lesson Fourteen: Enchant",
                    questDescription = "Even a machine can answer to a higher purpose.",
                    questObjective = "Fill up twenty farm machines with essence. Quest completion enables an instant complete effect for active machines. Requires solar or void essence as a source.",
                    questReward = 1800,

                    taskCounter = 20,
                    taskFinish = "masterEnchant",
                    questLevel = 17,
                },

                ["lessonGravity"] = new()
                {
                    name = "lessonGravity",
                    questId = 18465044,
                    questValue = 6,
                    questTitle = "Druid Lesson Sixteen: Gravity",
                    questDescription = "Nothing can escape the pull of fate.",
                    questObjective = "Summon five gravity wells by casting around scarecrows or monsters. Uses cursor targetting by default. Quest completion improves pull radius. Requires solar or void essence as a source.",
                    questReward = 2000,

                    taskCounter = 5,
                    taskFinish = "masterGravity",
                    questLevel = 18,
                },

                ["lessonDaze"] = new()
                {
                    name = "lessonDaze",
                    questId = 18465045,
                    questValue = 6,
                    questTitle = "Druid Lesson Sixteen: Daze",
                    questDescription = "The feeble mind cannot fathom it's own fate.",
                    questObjective = "Daze monsters ten times. Jester attacks and gravity wells add a dazzle debuff to nearby monsters. A warp strike can be triggered with the action button. Quest completion enables random morph effects.",
                    questReward = 1900,

                    taskCounter = 10,
                    taskFinish = "masterDaze",
                    questLevel = 19,
                },

                ["challengeFates"] = new()
                {

                    name = "challengeFates",
                    type = "challenge",
                    triggerBlessing = "stars",
                    triggerLocation = new() { "Mountain", },
                    triggerMarker = "icon",
                    triggerVector = new(118, 20),
                    triggerColour = new(1f, 0.8f, 0.4f, 1),
                    questId = 18465046,
                    questValue = 6,
                    questTitle = "Traces of the Fallen One",
                    questDescription = "The Jester of Fate believes a clue about the identity or whereabouts of an ancient entity lie in the abandoned quarry.",
                    questObjective = "Perform a Rite of the Stars in the center of the Mountain Quarry.",
                    questReward = 10000,
                    questLevel = 20,

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
                newQuest.questLevel = -1;

                questList[newQuest.name] = newQuest;

            }

            return questList;

        }

        public static List<string> ChallengeQuests()
        {

            List<string> secondList = new()
            {

                "challengeCanoli",

                "challengeSandDragon",

            };

            if (Game1.currentSeason != "winter")
            {

                secondList.Add("challengeMariner");

            }

            if (Game1.player.hasOrWillReceiveMail("seenBoatJourney"))
            {

                secondList.Add("challengeGemShrine");

            }

            return secondList;

        }

        public static List<string> ActiveSeconds()
        {

            Dictionary<string, string> challengeList = SecondQuests();

            List<string> activeList = new();

            foreach (KeyValuePair<string, string> challenge in challengeList)
            {

                string questName = challenge.Key + "Two";

                if (Mod.instance.QuestGiven(questName) && !Mod.instance.QuestComplete(questName))
                {

                    activeList.Add(questName);

                }

            }

            return activeList;

        }

        public static Dictionary<string, string> SecondQuests()
        {

            Dictionary<string, string> secondList = new()
            {
                ["challengeEarth"] = "The Aquifer Revisited",

                ["challengeWater"] = "The Invasion Revisited",

                ["challengeStars"] = "The Infestation Revisited",

                ["challengeCanoli"] = "The Dusting Revisited",

                ["challengeSandDragon"] = "The Tyrant Revisited",

            };

            if (Game1.currentSeason != "winter")
            {

                secondList.Add("challengeMariner", "The Seafarer Revisited");

            }

            if (Game1.player.hasOrWillReceiveMail("seenBoatJourney"))
            {

                secondList.Add("challengeGemShrine", "The Shrine Revisited");

            }

            if (Mod.instance.QuestComplete("challengeFates"))
            {

                secondList.Add("challengeFates", "The Fallen Revisited");

            }

            return secondList;

        }

        public static Quest RetrieveQuest(string quest)
        {

            Dictionary<string, Quest> questList = QuestList();

            return questList[quest];

        }

    }

    public class Quest
    {

        public string name;

        public string type;

        public List<string> triggerLocation;

        public int startTime;

        public string triggerBlessing;

        public Vector2 triggerVector;

        public List<Type> triggerLocale;

        public string triggerMarker;

        public bool triggerAnywhere;

        public string triggerCompanion;

        public Color triggerColour = Color.White;

        public int questId;

        public string questCharacter;

        public int questValue;

        public string questTitle;

        public string questDescription;

        public string questObjective;

        public int questReward;

        public int questLevel;

        public int taskCounter;

        public string taskFinish;

    }


}
