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

                icon = IconData.displays.effigy,

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
                    "\"For the new farmer. Before my forefathers came to the valley, the secluded grove behind the farm was a frequent meeting ground for a circle of Druids. " +
                    "An eeriness hangs over the place, so I have kept my father's tradition of leaving it well enough alone. Perhaps you should too.\"",

                instruction = "Investigate the secluded grove and old farm cave. Press " + Mod.instance.Config.riteButtons.ToString() + " to cast a rite at the quest icon.",

                explanation = "I heard a voice in the cave, and it bid me to speak words in a forgotten dialect. " +
                    "I raised my hands, recited a chant, and a wooden effigy crashed to the floor, which then started to talk! " +
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

                icon = IconData.displays.weald,

                // -----------------------------------------------

                give = Quest.questGivers.dialogue,

                runevent = true,

                trigger = true,

                triggerLocation = Location.LocationData.druid_grove_name,

                triggerTime = 0,

                triggerRite = Rite.rites.none,

                origin = new Vector2(21, 10) * 64,

                // -----------------------------------------------

                title = "The Two Kings",

                description = "The Effigy says I need the favour of the Kings of Oak and Holly to begin my apprencticeship to the circle of Druids. " +
                "He instructed me to pay homage to the forgotten monarchs at a sacred place in the secluded grove.",

                instruction = "Investigate the standing stones in the grove west of the farmcave. Press " + Mod.instance.Config.riteButtons.ToString() + " to cast a rite at the quest icon.",

                explanation = "The voices that spoke from beyond the standing stones made me a squire, and the sword I received seems to brim with woodland magic.",

                progression = null,

                requirement = 0,

                reward = 250,

                details = new(),

                // -----------------------------------------------

                before = new() {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "The standing stones to the west of here were important to the old circle of Druids. Stand before them, and pay homage to the two kings by performing the rite as you did before. " +
                            "If you truly possess a connection to the otherworld, then the latent energies there will be drawn to you. (New quest received)",

                    }
                },

                after = new()
                {
                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "So you have returned as a squire of the Two Kings.",
                        responses = new()
                        {
                            "I heard some really weird voices. Something touched me. I'm ok.",
                            "So it is real. The power of the Druids.",
                            "I have a sword shaped like a branch. It's a stick of power."
                        },
                        outro = "The energies of the Weald are unsettling. And dumb as rocks. Seek them out again if you would like to dedicate a different implement to the work of the Kings.",

                    }
                },

            };

            quests.Add("swordWeald", swordWeald);

            // =====================================================
            // WEALD LESSONS

            Quest wealdOne = new()
            {

                name = QuestHandle.wealdOne,

                icon = IconData.displays.weald,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.none,

                title = "Lesson One: Clearance",

                description = "I have been blessed by the energies of the Weald. I can practice my new found ability by clearing weeds and twigs from overgrown spaces.",

                instruction = "Perform Rite of the Weald: Clearance one hundred times. Check the effects page "+ Mod.instance.Config.effectsButtons.ToString() + " for details",

                progression = "weeds and twigs destroyed",

                requirement = 100,

                reward = 250,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "Good. You are now a subject of the two kingdoms, and bear authority over the weed and the twig. Use this new power to drive out decay and detritus. Return tomorrow for another lesson. (New quest received)",

                    }
                },

            };

            quests.Add(QuestHandle.wealdOne, wealdOne);

            // -----------------------------------------------------

            Quest wealdTwo = new()
            {

                name = QuestHandle.wealdTwo,

                icon = IconData.displays.weald,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Two: Wild Bounty",

                description = "As I perform the rite, the valley springs into new life around me, offering a sample of its hidden bounty.",

                instruction = "Perform Rite of the Weald: Bounty to rustle twenty large bushes for forageables. Unlocks wild seed gathering from grass.",

                progression = "bushes rustled",

                requirement = 20,

                reward = 500,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "Your journey continues with this lesson. The wild spaces hold many riches. Call out to the trees, the bushes and the grass, that they may offer you a portion of their bounty. " +
                        "Those creatures that are caught in the midst of your work should be delighted by the gentle caress of power. (New quest received)",

                    }
                },

            };

            quests.Add(QuestHandle.wealdTwo, wealdTwo);

            // -----------------------------------------------------

            Quest wealdThree = new()
            {

                name = QuestHandle.wealdThree,

                icon = IconData.displays.weald,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Three: Wild Growth",

                description = "Years of stagnation have starved the valley of it's wilderness. I now have the means and power to recolour the barren spaces with new plant-life.",

                instruction = "Perform Rite of the Weald: Wild Growth to sprout twenty forageables total in the Forest or elsewhere. Unlocks flowers.",

                progression = "forageables spawned",

                requirement = 20,

                reward = 750,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "This is your task today. Perform the rite as you have been taught, only this time, you may convince the wild to sprout new shoots and buds. Now go, fill the barren spaces with life. (New quest received)",

                    }
                },

            };

            quests.Add(QuestHandle.wealdThree, wealdThree);

            // -----------------------------------------------------

            Quest wealdFour = new()
            {

                name = QuestHandle.wealdFour,

                icon = IconData.displays.weald,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Four: Cultivation",

                description = "My connection to the otherworld deepens. I may channel the power of the Two Kings to enhance the quality and growth of my crops.",

                instruction = "Convert twenty planted seasonal wild seeds into domestic crops. Updates growth cycle of all planted seeds. Unlocks quality conversions.",

                progression = "seeds converted",

                requirement = 20,

                reward = 1000,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "For those who plan for a life of toil in the fields and meadows, they should know that farmer and druid share many of the same ideals. " +
                        "May the blessings of the two kings assist you in your farmwork. (New quest received)",

                    }
                },

            };

            quests.Add(QuestHandle.wealdFour, wealdFour);

            // -----------------------------------------------------

            Quest wealdFive = new()
            {

                name = QuestHandle.wealdFive,

                icon = IconData.displays.weald,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Five: Rockfall",

                description = "The power of the two Kings resonates through the deep earth, and in the mines, the earth responds in kind.",

                instruction = "Gather one hundred stone from creating rockfalls in the mines with Rite of the Weald. Unlocks rockfall damage to monsters.",

                progression = "rockfall events",

                requirement = 100,

                reward = 1250,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "Be careful in the mines. The Druid draws power from the deep rock, and it will answer your call, both above and below you. (New quest received)",

                    }
                },

            };

            quests.Add(QuestHandle.wealdFive, wealdFive);

            // =====================================================
            // WEALD CHALLENGE

            Quest challengeWeald = new()
            {

                name = "challengeWeald",

                type = Quest.questTypes.challenge,

                icon = IconData.displays.weald,

                // -----------------------------------------------

                give = Quest.questGivers.dialogue,

                runevent = true,

                trigger = true,

                triggerLocation = "UndergroundMine20",

                triggerTime = 0,

                triggerRite = Rite.rites.weald,

                origin = new Vector2(25f, 13f) * 64,

                // -----------------------------------------------

                title = "The Polluted Aquifier",

                description = "The mountain spring, from an aquifer of special significance to the otherworld, has been polluted by rubbish dumped in the abandoned mineshafts. " +
                "The Effigy believes I am strong enough to cleanse the waters with the power of the Two Kings.",

                instruction = "Perform a Rite of the Weald at the aquifier in level 20 of the mines.",

                explanation = "I reached a large cavern with a once pristine lake, and used the Rite of the Weald to purify it. " +
                "There was so much trash, and so many bats. A big one in a skull mask claimed to serve a higher power, one with a vendetta against the polluters.",

                progression = null,

                requirement = 0,

                reward = 1500,

                details = new()
                {
                    "The residents of the mountain and their friends are pleased with the cleanup: Sebastian, Sam, Maru, Abigail, Robin, Demetrius, Linus.",

                },

                // -----------------------------------------------

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "A trial presents itself. For some time now, foulness has seeped from a spring once cherished by the monarchs. " +
                        "You must travel to the source, some way into the caverns of the mountain, and cleanse it with the blessing of the Kings. (New quest received)",

                    }
                },

                after = new()
                {
                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "I sense a change. The foulness has dissipated.",
                        responses = new()
                        {
                            "The rite disturbed some bats. BIG BATS.",
                            "I removed the pollutants from the aquifer. The water quality of the mountain springs has already started to recover."
                        },
                        outro = "You have my gratitude. And there are still many more sacred places to restore.",

                    }
                },

            };

            quests.Add("challengeWeald", challengeWeald);

            // =====================================================
            // SWORD MISTS

            Quest swordMists = new()
            {

                name = "swordMists",

                type = Quest.questTypes.sword,

                icon = IconData.displays.mists,

                // -----------------------------------------------

                give = Quest.questGivers.dialogue,

                runevent = true,

                trigger = true,

                triggerLocation = Location.LocationData.druid_atoll_name,

                triggerTime = 0,

                triggerRite = Rite.rites.weald,

                origin = new Vector2(30,19) * 64,

                // -----------------------------------------------

                title = "The Voice Beyond the Shore",

                description = "The Effigy believes the religious coven of large bats serve none other than the Lady Beyond the Shore. " +
                "She has granted me an audience on a small atoll accessed from the furthest side of the beach. I'll have to repair the bridge to the tidal pools to reach it.",

                instruction = "Travel to the beach south of Pelican town, past the small wooden bridge, the tidal pools, and across the sandbridge to the atoll. " +
                "Cast the Rite of the Weald at the statue there.",

                explanation = "I called out across the waves, imagining my voice travelling over the gem sea to an isle of mystery and magic. " +
                "Then a voice answered, and it was like lightning, my body shook with the words, and when I opened my eyes my hands held a weapon.",

                progression = null,

                requirement = 0,

                reward = 250,

                details = new(),

                // -----------------------------------------------

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "The purification of the sacred waters has pleased a distant patron of the mists, the Lady Beyond the Shore. " +
                        "Perhaps you cannot hear her voice as I can, but it harkens to you. " +
                        "There is a special place east of the local shoreline where you can answer her call. (New quest received)",

                    }
                },

                after = new()
                {
                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "So you have been blessed by the Lady Beyond.",
                        responses = new()
                        {
                            "The Lady has a beautiful voice. Bit loud though.",
                            "There was a bolt of lightning. There was a voice, like thunder. There was a gannet, it went 'squawk'. I wasn't scared though.",
                        },
                        outro = "It takes a lot of power to speak over the distance between the valley and the isle of mists.",

                    }
                },

            };

            quests.Add("swordMists", swordMists);

            // =====================================================
            // MISTS LESSONS

            Quest mistsOne = new()
            {

                name = QuestHandle.mistsOne,

                icon = IconData.displays.mists,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.none,

                title = "Lesson Six: Sunder",

                description = "The Lady Beyond the Shore has granted me the power to remove common obstacles. Now I can be her representative to the further, wilder spaces of the valley.",

                instruction = "Perform Rite of Mists: Sunder to destroy ten obstacles. This includes boulders, stumps or logs.",

                progression = "obstacles destroyed",

                requirement = 10,

                reward = 1500,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "The relationship between our Circle and Lady Beyond the Shore has languished over many generations. It is good to know she still favours us, and our mission here. " +
                        "Enjoy the use of the power of the mists against the larger obstacles in your path. (New quest received)",

                    }
                },

            };

            quests.Add(QuestHandle.mistsOne, mistsOne);

            // -----------------------------------------------------

            Quest mistsTwo = new()
            {

                name = QuestHandle.mistsTwo,

                icon = IconData.displays.mists,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Seven: Artifice",

                description = "The raw energy provided by the mists is precise enough to charge artifacts with special power.",

                instruction = "Rite of Mists has special interactions with various map-specific and crafted items, including warp statues, lightning rods, scarecrows and torch candles. " +
                "Perform ten of these interactions. See the Druid Effects journal for details.",

                progression = "special interactions performed",

                requirement = 10,

                reward = 1750,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "The Lady is fascinated by the industriousness of humanity. Combine your artifice with her blessing and reap the rewards. (New quest received)",

                    }
                },

            };

            quests.Add(QuestHandle.mistsTwo, mistsTwo);

            // -----------------------------------------------------

            Quest mistsThree = new()
            {

                name = QuestHandle.mistsThree,

                icon = IconData.displays.mists,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Eight: Fishing",

                description = "I now have an advantage in the popular sport of fishing in the valley. The biggest and rarest specimens of the water answer to the authority of the Voice Beyond the Shore.",

                instruction = "Strike open water to create fishing spots. " +
                "Use a fishing rod to cast a line over the spot and wait for the fishing minigame to auto-trigger. " +
                "Attempt ten catches. Quest completion unlocks rarer fish.",

                progression = "catches attempted",

                requirement = 10,

                reward = 2000,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "The denizens of the deep water serve the Lady. Go now, and test your skill against them. (New quest received)",

                    }
                },

            };

            quests.Add(QuestHandle.mistsThree, mistsThree);

            // -----------------------------------------------------

            Quest mistsFour = new()
            {

                name = QuestHandle.mistsFour,

                icon = IconData.displays.mists,

                type = Quest.questTypes.lesson,

                give = Quest.questGivers.dialogue,

                title = "Lesson Nine: Smite",

                description = "I now have an answer for some of the more terrifying threats I've encountered in my adventures. Bolts of lightning strike at my foes.",

                instruction = "Smite enemies with Rite of Mists twenty times. Unlocks critical hits.",

                progression = "enemies hit",

                requirement = 20,

                reward = 2250,

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "Your connection to the plane beyond broadens. Call upon the Lady's Voice to destroy your foes. (New quest received)",

                    }
                },

            };

            quests.Add(QuestHandle.mistsFour, mistsFour);


            // =====================================================
            // QUEST EFFIGY

            Quest questEffigy = new()
            {

                name = "questEffigy",

                type = Quest.questTypes.challenge,

                icon = IconData.displays.effigy,

                // -----------------------------------------------

                give = Quest.questGivers.dialogue,

                runevent = true,

                trigger = true,

                triggerLocation = "Beach",

                triggerTime = 0,

                triggerRite = Rite.rites.weald,

                origin = new Vector2(12f, 13f) * 64,

                // -----------------------------------------------

                title = "At The Beach",

                description = "The Effigy feels a strange yearning for the shoreline. Whether for nostalgia, or for simple leisure, he is determined to visit the local beach, and I have agreed to accompany him.",

                instruction = "Go to the beach with 5-6 hours to spare and perform any rite at the marker to trigger the quest. " +
                    "Observe and talk to the Effigy as they go about various activities to learn more about their past.",

                explanation = "I watched the Effigy undertake many of the recreational activities once enjoyed by the first farmer. " +
                        "After several instances involving fish, slime and displays of raw, unbridled power, the Effigy used a veil of mists to conjure a vision of the past. " +
                        "The construct itself is a testament to the ingenuity of the Lady Beyond the Shore and her human acolyte, the first farmer. " +
                        "After they had completed their project, and taught the Effigy many things, the Lady deemed it an appropriate time to depart the valley. ",

                progression = null,

                requirement = 0,

                reward = 2500,

                // -----------------------------------------------

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "I feel drawn to the shore, to the enduring sands and tidal pools once walked by the first farmer. " +
                        "Though it is not related to your studies in Druidry, I think it would be of benefit for you to accompany me. (New quest received)",

                    }
                },

                after = new()
                {
                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "I think I am starting to see the past with a clearer, wiser perspective. Thank you for sharing this moment with me.",
                    }
                },

            };

            quests.Add("questEffigy", questEffigy);


            // =====================================================
            // MISTS CHALLENGE

            Quest challengeMists = new()
            {

                name = "challengeMists",

                type = Quest.questTypes.challenge,

                icon = IconData.displays.mists,

                // -----------------------------------------------

                give = Quest.questGivers.dialogue,

                runevent = true,

                trigger = true,

                triggerLocation = "Town",

                triggerTime = 1900,

                triggerRite = Rite.rites.mists,

                origin = new Vector2(47f, 88f) * 64,

                // -----------------------------------------------

                title = "The Shadow Invasion",

                description = "When I first discovered The Effigy, it was stuck in a nook in the farmcave ceiling. " +
                "It has admitted to hiding itself there after a chance encounter with a trinket-marauding shadowfolk at the village cemetary. " +
                "He has requested that we reconnoitre the last known site of the shadow invader's operation.",

                instruction = "Perform a Rite of Mists in Pelican Town's graveyard between 7:00 pm and Midnight.",

                explanation = "My use of the power of the Mists gained the attention of a small group of the shadow invaders. " +
                "One of the marauders had a cannon, and in the interest of town peace and property, I decided it would be prudent to confiscate such a destructive weapon. " +
                "Fortunately for the cannon-bearer the shadowfolk's captain arrived in time to coordinate a hasty retreat. " +
                "The town residents noticed only enough of the commotion to believe that I had chased away some vandals. They commended my efforts.",

                progression = null,

                requirement = 0,

                reward = 2750,

                // -----------------------------------------------

                before = new()
                {

                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "This might be an appropriate time to discuss the circumstances that led to my encasement in the roof of the farm cave. " +
                        "A group of shadowfolk marauders appeared in the valley some months ago. They appear to be interested in what archaeological evidence remains of the age of the elderborn. " +
                        "By happenstance I encountered their captain on the banks of the river south of the village cemetary. " +
                        "This rogue expressed a disconcerting interest in my 'make', but I was able to waylay him on the chase home through the brambled paths of the farm. " +
                        "I secreted myself away, partly out of self-preservation, partly so to wait until the new custodian of the farm arrived. And now you are here. " +
                        "I ask that we reconnoitre site of their operation to gauge the mercenary numbers and location. " +
                        "For this, I would be indebted to your kindness. (New quest received)",

                    }
                },

                after = new()
                {
                    [StardewDruid.Data.CharacterData.characters.Effigy] = new()
                    {
                        prompt = true,
                        intro = "So they were acting on behalf of the Deep one. Hmmm. " +
                        "Lord Deep was an elderborn noble who fell into obscurity in the aftermath of the war. " +
                        "Or so my mentors told me. It appears he has gathered a small measure of power and resources. " +
                        "He might send more forces to the surface to disturb the sacred places of the valley. We must be vigilant.",
                    }
                },

            };

            quests.Add("challengeMists", challengeMists);


            // =====================================================
            // RELICS MISTS


            Quest relicsMists = new()
            {

                name = "relicsMists",

                type = Quest.questTypes.relics,

                icon = IconData.displays.mists,

                // -----------------------------------------------

                give = Quest.questGivers.none,

                runevent = true,

                trigger = true,

                triggerLocation = "CommunityCenter",

                triggerTime = 0,

                triggerRite = Rite.rites.mists,

                origin = new Vector2(40, 9) * 64,

                // -----------------------------------------------

                title = "The Avalant",

                description = "I have gathered all the major components of the Avalant, an ancient navigation device used for passage to a city drowned within the abyssal trench. " +
                "I do not have the means to explore the trench, but I could use the components to spruce up the disused fishtank in the community center.",

                instruction = "Perform a Rite of Mists at the fish tank in the community center.",

                explanation = "Through artifice bolstered by the power of the Lady Beyond the Shore, the repairs have been completed.",

            };

            quests.Add("relicsMists", relicsMists);

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
            relics,
        }

        public enum questGivers
        {
            none,
            dialogue,
            chain,
        }

        public string name;

        public questTypes type = questTypes.none;

        public questGivers give = questGivers.none;

        public bool runevent;

        public Vector2 origin = Vector2.Zero;

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

        public List<string> details = new();

        public string progression;

        public int requirement;

        public int reward;

        // -----------------------------------------------
        // dialogues

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
