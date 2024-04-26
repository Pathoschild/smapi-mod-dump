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
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewValley.GameData.Characters;
using System.Collections.Generic;

namespace StardewDruid.Journal
{
    public static class QuestData
    {


        public static Dictionary<string,Quest> QuestList()
        {

            Dictionary<string,Quest> quests = new();

            // =====================================================
            // APPROACH EFFIGY

            Quest approachEffigy = new()
            {
                
                name = "approachEffigy",
                
                type = Quest.questTypes.approach,

                // -----------------------------------------------

                give = Quest.questGivers.none,

                runevent = true,

                trigger = true,

                triggerLocation = "FarmCave",

                triggerRite = Rite.rites.none,

                origin = WarpData.WarpStart("FarmCave"),

                // -----------------------------------------------

                title = "Grandfather's note",

                description = "There's a note stuck to grandpa's old scythe: " +
                    "\"For the new farmer. Before my forefathers came to the valley, the local farmcave was a frequent meeting ground for a circle of Druids. " +
                    "An eeriness hangs over the place, so I have kept my father's tradition of leaving it well enough alone. Perhaps you should too.\"",

                instruction = "Investigate the old farm cave. Press " + Mod.instance.Config.riteButtons.ToString() + " to cast a rite.",

                explanation = "I heard a voice in the cave, and it bid me to speak words in a forgotten dialect. " +
                    "I raised my hands, recited a chant, and a wooden effigy crashed to the floor! Then it stood up and started to talk!" +
                    "It spoke to me and told me how I could learn the ways of the valley Druids. And so my apprenticeship begins...",

                progression = null,

                // -----------------------------------------------

            };

            quests.Add("approachEffigy",approachEffigy);


            // =====================================================
            // SWORD WEALD

            Quest swordWeald = new()
            {

                name = "swordWeald",

                type = Quest.questTypes.sword,

                // -----------------------------------------------

                give = Quest.questGivers.dialogue,

                runevent = true,

                trigger = true,

                triggerLocation = Location.LocationData.druid_grove_name,

                triggerTime = 0,

                triggerRite = Rite.rites.none,

                origin = Vector2.Zero,

                // -----------------------------------------------

                title = "The Two Kings",

                description = "The Effigy says I need the favour of the Kings of Oak and Holly to begin my apprencticeship to the circle of Druids. He instructed me to pay homage to the forgotten monarchs at a shrine in the nearby grove.",

                instruction = "Investigate the shrine in the grove west of the farmcave. Press " + Mod.instance.Config.riteButtons.ToString() + " to cast a rite.",

                explanation = "I found the forlorn shrine and paid homage to the Two Kings. The voices that spoke from beyond the world made me a squire, and the sword I received seems to brim with woodland magic.",

                progression = null,

                requirement = 0,

                reward = 250,

                details = new(),

                // -----------------------------------------------

                delay = 0,

                before = new() {

                    [StardewDruid.Data.CharacterData.characters.effigy] = new()
                    {
                        prompt = true,
                        intro = "The Effigy: In the grove west of here is a shrine to the old monarchs. " +
                            "If you truly possess a connection to the otherworld, then the latent energies there will be drawn to you. " +
                            "This is your first task.",

                    }
                },

                after = new()
                {
                    [StardewDruid.Data.CharacterData.characters.effigy] = new()
                    {
                        prompt = true,
                        intro = "So you have returned as a squire of the Two Kings.",
                        responses = new()
                        {
                            "I heard some really wierd voices. Something touched me. I'm ok.",
                            "I have a sword shaped like a branch. A STICK OF POWER."
                        },
                        outros = new()
                        {
                            "The energies of the Weald are unsettling. And dumb as rocks. No matter, there is work to be done.",
                        }
                    }
                },

            };

            quests.Add("swordWeald", swordWeald);

            // =====================================================
            // WEALD LESSONS

            Quest wealdOne = new()
            {

                name = QuestHandle.clearLesson,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson One: Clearance",

                description = "Demonstrate your new abilities by clearing weeds and twigs from overgrown spaces.",

                instruction = "Perform Rite of the Weald: Clearance twenty times. Check the effects page "+ Mod.instance.Config.effectsButtons.ToString() + " for details",

                progression = " / 20 weeds and twigs destroyed",

                requirement = 20,

                reward = 250,

                delay = 0,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.effigy] = new()
                    {
                        prompt = true,
                        intro = "Good. You are now a subject of the two kingdoms, and bear authority over the weed and the twig. Use this new power to drive out decay and detritus. Return tomorrow for another lesson.",

                    }
                },

            };

            quests.Add(QuestHandle.clearLesson, wealdOne);

            // -----------------------------------------------------

            Quest wealdTwo = new()
            {

                name = QuestHandle.bushLesson,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Two: Extraction",

                description = "The wild spaces hold many riches, and some are guarded jealously.",

                instruction = "Perform Rite of the Weald: Extraction to rustle twenty bushes for forageables. Unlocks wild seed gathering from grass.",

                progression = " / 20 bushes rustled",

                requirement = 20,

                reward = 500,

                delay = 1,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.effigy] = new()
                    {
                        prompt = true,
                        intro = "The valley springs into new life. Go now, and sample its hidden bounty.",

                    }
                },

            };

            quests.Add(QuestHandle.bushLesson, wealdTwo);

            // -----------------------------------------------------

            Quest wealdThree = new()
            {

                name = QuestHandle.spawnLesson,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Three: Wild Growth",

                description = "The Druid fills the barren spaces with life.",

                instruction = "Perform Rite of the Weald: Wild Growth to sprout ten forageables total in the Forest or elsewhere. Unlocks flowers.",

                progression = " / 10 forageables spawned",

                requirement = 10,

                reward = 750,

                delay = 2,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.effigy] = new()
                    {
                        prompt = true,
                        intro = "Years of stagnation have starved the valley of it's wilderness. Go now, and recolour the barren spaces.",

                    }
                },

            };

            quests.Add(QuestHandle.spawnLesson, wealdThree);

            // -----------------------------------------------------

            Quest wealdFour = new()
            {

                name = QuestHandle.cropLesson,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Four: Cultivation",

                description = "The Farmer and the Druid share the same vision.",

                instruction = "Convert twenty planted seasonal wild seeds into domestic crops. Updates growth cycle of all planted seeds. Unlocks quality conversions.",

                progression = " / 20 seeds converted",

                requirement = 20,

                reward = 1000,

                delay = 3,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.effigy] = new()
                    {
                        prompt = true,
                        intro = "Your connection to the earth deepens. You may channel the power of the Two Kings for your own purposes.",

                    }
                },

            };

            quests.Add(QuestHandle.cropLesson, wealdFour);

            // -----------------------------------------------------

            Quest wealdFive = new()
            {

                name = QuestHandle.rockLesson,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Five: Rockfall",

                description = "The Druid draws power from the deep rock.",

                instruction = "Gather one hundred stone from rockfalls in the mines. Unlocks rockfall damage to monsters.",

                progression = " / 100 rockfall events",

                requirement = 100,

                reward = 1250,

                delay = 4,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.effigy] = new()
                    {
                        prompt = true,
                        intro = "Be careful in the mines. The deep earth answers your call, both above and below you.",

                    }
                },

            };

            quests.Add(QuestHandle.rockLesson, wealdFive);

            // -----------------------------------------------------

            return quests;

        }

    }
    public class Quest
    {

        public enum questTypes
        {
            none,
            approach,
            sword,
            challenge,
            lesson,
            heart,
        }

        public enum questGivers
        {
            none,
            dialogue,
        }

        public string name;

        public questTypes type = questTypes.none;

        public questGivers give = questGivers.none;

        public bool runevent;

        public Vector2 origin;

        // -----------------------------------------------
        // trigger

        public bool trigger;

        public string triggerLocation;

        public int triggerTime;

        public Rite.rites triggerRite = Rite.rites.none;

        // -----------------------------------------------
        // journal

        public string title;

        public IconData.displays icon = IconData.displays.none;

        public string description;

        public string instruction;

        public string explanation;

        public List<string> details;

        public string progression;

        public int requirement;

        public int reward;

        // -----------------------------------------------
        // dialogues

        public int delay;

        public Dictionary<StardewDruid.Data.CharacterData.characters, Dialogue.DialogueSpecial> before = new();

        public Dictionary<StardewDruid.Data.CharacterData.characters, Dialogue.DialogueSpecial> after = new();


    }

    public class QuestProgress
    {

        public int status;

        public int progress;

        public int replay;

        public int delay;

        public QuestProgress()
        {

        }

        public QuestProgress(int Status, int Delay = 0, int Progress = 0, int Replay = 0)
        {

            status = Status;

            progress = Progress;

            replay = Replay;

            delay = Delay;

        }

    }
}
