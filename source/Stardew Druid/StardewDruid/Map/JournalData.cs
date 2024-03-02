/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewDruid.Journal;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Map
{
    public static class JournalData
    {

        public static List<List<Page>> RetrievePages()
        {

            List<List<Page>> source = new List<List<Page>>();

            Dictionary<int, List<List<string>>> dictionary1 = JournalPages();

            int num = Mod.instance.CurrentProgress();

            Dictionary<string, int> dictionary2 = Mod.instance.TaskList();

            source.Add(new List<Page>());

            List<Page> pageList = new();

            foreach (KeyValuePair<int, List<List<string>>> keyValuePair in dictionary1)
            {
                if (keyValuePair.Key <= num)
                {
                    foreach (List<string> stringList in keyValuePair.Value)
                    {

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
                                        {
                                            
                                            page.objectives.Add(stringList[4]);
                                        
                                        }
                                        else
                                        {
                                            
                                            page.active = true;
                                        
                                        }

                                        Dictionary<int, Dictionary<int, string>> dialogueScene = DialogueData.DialogueScene(stringList[5]);

                                        if(dialogueScene.Count > 0)
                                        {

                                            Dictionary<int, Dialogue.Narrator> narrator = DialogueData.DialogueNarrator(stringList[5]);

                                            foreach(KeyValuePair<int, Dialogue.Narrator> sceneNarrator in narrator)
                                            {
                                                page.transcript.Add("(transcript) " + sceneNarrator.Value.name);

                                                foreach (KeyValuePair<int, Dictionary<int, string>> sceneEntry in dialogueScene)
                                                {

                                                    if(sceneEntry.Key > 900)
                                                    {
                                                        continue;
                                                    }

                                                    if (sceneEntry.Value.ContainsKey(sceneNarrator.Key))
                                                    {

                                                        page.transcript.Add(sceneEntry.Value[sceneNarrator.Key]);

                                                    }

                                                }

                                            }

                                        }

                                        pageList.Add(page);
                                        
                                        break;
                                    
                                    }
                                    
                                    break;
                                
                                }

                                Page page1 = new Page();

                                page1.title = stringList[0];

                                page1.icon = stringList[2];

                                page1.description = stringList[3];

                                if (num > keyValuePair.Key)
                                {
                                    
                                    page1.objectives.Add(stringList[4]);
                                
                                }
                                    
                                pageList.Add(page1);

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
                                pageList.Add(page2);
                                break;

                            case "effect":
                            case "journal":

                                string effectLead = stringList[1] == "effect" ? "Effect: " : "";

                                List<string> effectObjectives = new() { effectLead + stringList[4], };

                                if (stringList.Count >= 6)
                                {

                                    for (int i = 5; i < stringList.Count; i++)
                                    {

                                        effectObjectives.Add(stringList[i]);

                                    }

                                }

                                pageList.Add(new Page()
                                {

                                    title = stringList[0],

                                    icon = stringList[2],

                                    description = stringList[3],

                                    objectives = effectObjectives,

                                });

                                break;

                            case "ether":

                                List<string> etherPages = new() {};

                                int specialNotes = 0;

                                Dictionary<string, int> taskList = Mod.instance.TaskList();

                                if (taskList.ContainsKey("masterTreasure"))
                                {

                                    specialNotes = 7;

                                }
                                else if (taskList.ContainsKey("lessonTreasure"))
                                {

                                    specialNotes = taskList["lessonTreasure"];

                                }

                                Dictionary<int, List<string>> specialEntries = DialogueData.EtherPages();

                                for (int i = 0; i < specialNotes; i++)
                                {

                                    foreach (string entry in specialEntries[i])
                                    {
                                        
                                        etherPages.Add(entry);

                                    }

                                }

                                pageList.Add(new Page()
                                {

                                    title = stringList[0],

                                    icon = stringList[2],

                                    description = stringList[3],

                                    objectives = etherPages,

                                });

                                break;
                        }
                    }
                }
                else
                    break;
            }

            if (Mod.instance.ReverseJournal())
            {

                pageList.Reverse();

            }

            foreach(Page page in pageList)
            {
                
                if (source.Last().Count() == 6)
                {
                    source.Add(new List<Page>());

                }

                source.Last().Add(page);

            }
            
            return source;

        }

        public static Dictionary<int, List<List<string>>> JournalPages()
        {

            Dictionary<int, List<List<string>>> pages = new()
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
                    "Hold the rite button to steadily increase the range of the effect up to eight tiles away. You can run and ride a horse while holding the rite button. Performing a rite provides faster movement through grass."
                    },
                    new()
                    {
                    "Effect: Auto Consume",
                    "effect",
                    "Weald",
                    "The offerings of the valley sustain me.",
                    "When enabled in the configuration, auto-consume will trigger when health or stamina falls below a certain threshold, and consume items from the inventory to provide a stamina/health boost and even a temporary speed buff.",
                    "Roughage items: Sap, Tree seeds, Slime, Batwings, Red mushrooms.",
                    "Lunch items: SpringOnion, Snackbar, Mushrooms, Algae, Seaweed, Carrots, Sashimi, Salmonberry, Cheese, Salad, Tonic.",
                    "Coffee/Speed items: Cola, Tea Leaves, Tea, Coffee Bean, Coffee, Triple Espresso, Energy Tonic."
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
                    "The Effigy has shown me how to gather the bounty of the wild.",
                    "Extract foragables from large bushes, wood from trees, fibre from grass and small fish from water.",
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
                    "Cast over seasonal wild seeds sewn into tilled dirt to convert them into domestic crops. Will also fertilise and update the growth cycle of all crop seeds, and progress the growth rate of maturing fruit trees by one day (once per day).",
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
                    "Strike scarecrows to produce a radial crop watering effect. Radius increases after certain quest milestones. Strike a lightning rod once a day to charge it."
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
                    "The Effigy can be invited to roam the farm, and will perform it's own version of Rite of the Weald where scarecrows have been placed. This version will plant new seed into empty tilled dirt, and water and fertilise existing crops in a radius around the scarecrow. The Effigy will use the first chest placed in the farmcave as an inventory."
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
                    },
                    new()
                    {
                    "Effect: Comet",
                    "effect",
                    "Stars",
                    "The appearance of comets in the night sky would inpire the divinations of druid astronomers. Jester's teachings have bolstered my authority within the celestial realm.",
                    "Casting Rite of the Stars with Gravity Well active will attract a large comet to impact the immediate area with an extensive damage radius."
                    },
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
                    },
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
                    "Jester can be invited to roam the farm, or accompany you on journeys through the valley. He will automatically target nearby enemies, and his leaping melee attack applies the Daze effect. If positioned at right angles to a foe he can perform a powerful energy beam attack. Jester will use the second chest placed in the farmcave as an inventory if available."
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
                    "Press the special button/right click to blast nearby foes. Creates a zone fire with a damage-over-time effect. Uses cursor and directional targetting.",
                    "lessonBlast",
                    "of 10 blasts performed",
                    "masterBlast",
                    "Damage effect has a chance to immolate enemies and convert them into cooking items."
                    }
                },
                [31] = new()
                {
                    new()
                    {
                    "Lesson: Dive",
                    "lesson",
                    "Ether",
                    "It is time to nurture the gardens beneath the surface of the waters.",
                    "Press the action button/left click to fly over the water. Press the special button/right click to dive for treasure.",
                    "lessonDive",
                    "of 10 dives performed",
                    "masterDive",
                    "Rare treasure available"
                    }
                },
                [32] = new()
                {
                    new()
                    {
                    "Lesson: Treasure",
                    "lesson",
                    "Ether",
                    "The treasures of the ancient ones have been hoarded away in the ethereal plane.",
                    "Claim 7 dragon treasures. Search for the ether symbol on large map locations (Forest, Beach, Mountain, RailRoad, Desert, Island etc). The color of the symbol will change depending on the terrain. Move over the spot and either dig or dive (special/right click button) to claim the dragon treasure. Be careful, you might have to fight to keep the treasure contents.",
                    "lessonTreasure",
                    "of 7 treasures found",
                    "masterTreasure",
                    "Special journal entry completed"
                    },
                    new()
                    {
                    "Ether-drenched Pages",
                    "ether",
                    "Effigy",
                    "I've collected a series of ether-soaked pieces of vellum once scattered throughout the valley. The messages are written in a cuneform I can only decipher while in the guise of the ancient ones.",
                    }
                },
                [33] = new()
                {
                    new()
                    {
                    "The Ether Thieves",
                    "quest",
                    "Ether",
                    "It was a moment of bizarre misfortune. As soon as I had uncovered a treasure sealed away by the ethereal power of the ancient ones, a shadowfolk appeared to snatch it away. Jester wasn't around to witness the initial invasion of the town by these fiends, but now he's more familiar with the territory, he's sniffed out a lead. Another tomb.",
                    "The defunct town crypt was an ideal place for the shadowrogues to organise their operations in the material plane. They had laboured, most unsuccessfully, to gather ethereal matter and other supplies from this plane for the Deep One's cause. I challenged the ringleader, Shadowtin Bear, to a battle for the spoils he had accumulated. I triumphed, but the real trophy was Shadowtin's fealty, who is excited to ally with a Dragon, guise or not.",
                    "challengeEther",
                    },
                },
                [34] = new()
                {
                    new()
                    {
                    "Shadowtin Bear, Professional",
                    "quest",
                    "Shadow",
                    "The figure in the tin bear mask is a legend amongst the Shadowfolk. He has a fetish for rare collectibles, especially from the age of Dragons, and relishes the chance to partner with a Druid of my abilities. With Shadowtin's help, I hope to fulfil my promise to Jester to find the undervalley.",
                    "Shadowtin can join your other allies on the farm, cave or at your side. He can use his Carnyx to stun enemies, or twirl it in a spin attack. Shadowtin will pick up nearby forageables during idle moments when following the player. Shadowtin will use the third chest placed in the farmcave as an inventory if available.",
                    "approachShadowtin",
                    },
                },
                [35] = new()
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

            return pages;

        }

    }

}
