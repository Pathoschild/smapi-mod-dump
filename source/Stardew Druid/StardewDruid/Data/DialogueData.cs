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
using Microsoft.Xna.Framework.Audio;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Dialogue;
using StardewDruid.Journal;
using StardewDruid.Monster;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Data
{
    public static class DialogueData
    {

        public static Dictionary<int, string> DialogueNarrators(string scene)
        {
            Dictionary<int, string> sceneNarrators = new();

            switch (scene)
            {

                case QuestHandle.challengeWeald:

                    sceneNarrators = new() { 
                        [0] = "Clericbat",
                    };

                    break;

                case QuestHandle.challengeMists:

                    sceneNarrators = new() { 
                        [0] = "Shadow Sergeant", 
                        [1] = "Shadow Thug",
                        [2] = "Shadow Leader",
                        [3] = "The Effigy",
                    };

                    break;

                case QuestHandle.challengeStars:

                    sceneNarrators = new()
                    {
                        [0] = "The Jellyking",
                        [1] = "The Effigy",
                    };

                    break;

                case QuestHandle.challengeAtoll:

                    sceneNarrators = new()
                    {
                        [0] = "Captain of the Drowned",
                        [1] = "The Effigy",
                    };

                    break;

                case QuestHandle.challengeDragon:

                    sceneNarrators = new()
                    {
                        [0] = "Lesser Dragon",
                    };

                    break;

                case QuestHandle.swordFates:

                    sceneNarrators = new()
                    {
                        [0] = "The Jester of Fate",
                    };

                    break;

                case QuestHandle.challengeFates:

                    sceneNarrators = new()
                    {
                        [0] = "The Effigy",
                        [1] = "Jester",
                        [2] = "Buffin",
                        [3] = "Shadow Leader",
                        [4] = "Shadow Sergeant",
                        [5] = "Shadow Goblin",
                        [6] = "Shadow Rogue",
                    };

                    break;

                case QuestHandle.swordEther:

                    sceneNarrators = new()
                    {
                        [0] = "The Jester of Fate",
                        [1] = "Thanatoshi, Twilight Reaper",
                    };

                    break;

            };

            return sceneNarrators;

        }

        public static Dictionary<int, Dictionary<int, string>> DialogueScene(string scene)
        {

            Dictionary<int, Dictionary<int, string>> sceneDialogue = new();

            switch (scene)
            {

                case QuestHandle.challengeWeald:

                    sceneDialogue = new()
                    {

                        [22] = new() { [0] = "Trespasser", },
                        [24] = new() { [0] = "Filthy two legger", },
                        [26] = new() { [0] = "Cheeep cheep", },
                        [28] = new() { [0] = "You and your kind", },
                        [30] = new() { [0] = "Have defiled the sacred waters", },
                        [32] = new() { [0] = "Cheeep cheep", },
                        [34] = new() { [0] = "Our Lady of Mists", },
                        [37] = new() { [0] = "Demands retribution!", },
                        [39] = new() { [0] = "CHEEEP", },
                        [56] = new() { [0] = "These damned rocks", },
                        [58] = new() { [0] = "CHEEE--- aack", },
                    };

                    break;

                case QuestHandle.challengeMists:

                    sceneDialogue = new()
                    {

                        [2] = new() { [0] = "Ah whats that then", },
                        [4] = new() { [1] = "One of them twinkle fingers", },
                        [6] = new() { [0] = "Alright lads get em", },
                        [9] = new() { [0] = "Loading charge", },
                        [11] = new() { [3] = "Beware those explosive rounds",},
                        [13] = new() { [0] = "Blasted thing jammed!", },
                        [18] = new() { [0] = "Loading again", },
                        [20] = new() { [3] = "We must prevent such callous destruction", },
                        [22] = new() { [0] = "Ah whiffed it", },
                        [25] = new() { [0] = "Stop waffling and pin them down", },
                        [28] = new() { [1] = "Its no good. Twinkly's too tricky", },
                        [31] = new() { [0] = "Ughhh... alright loading again", },
                        [35] = new() { [0] = "I'M UNDER PRESSURE HERE", },
                        [38] = new() { [1] = "Dont feel so good bout this", },
                        [40] = new() { [0] = "You'd rather anger the Deep one?", },
                        [42] = new() { [1] = "Sod that, I'd rather fight", },
                        [44] = new() { [0] = "Preparing incendiary", },
                        [48] = new() { [0] = "This isn't going well", },
                        [51] = new() { [2] = "Attempting a capture are we", },
                        [53] = new() { [3] = "That is the mercenary that hunted me", },
                        [55] = new() { [0] = "Trying, boss", },
                        [57] = new() { [2] = "Aim higher", },
                        [59] = new() { [0] = "Uh, sorry boss", },
                        [61] = new() { [2] = "We're too exposed here, call them all back", },
                        [63] = new() { [0] = "You heard the boss!", },
                        [65] = new() { [2] = "Watch yourself, farmer", },

                    };
                    
                    break;
                
                case QuestHandle.challengeStars:

                    sceneDialogue = new()
                    {

                        [3] = new() { [1] = "The infestation must be contained here", },

                        [12] = new() { [1] = "A larger threat approaches. An old enemy", },

                        [15] = new() { [0] = "HOW BORING", },
                        [18] = new() { [0] = "The monarchs must be asleep.", },
                        [21] = new() { [0] = "If they send only a farmer", },
                        [24] = new() { [0] = "To face the onslaught...", },
                        [27] = new() { [0] = "OF THE MIGHTY SLIME", },

                        [30] = new() { [1] = "Arrogance! You are far diminished since the last age", },
                        [33] = new() { [1] = "A sad reflection in a murky puddle", },

                        [36] = new() { [0] = "You're too late", },
                        [39] = new() { [0] = "The slumber of the kings has led to stagnation", },
                        [42] = new() { [0] = "The land must be destroyed to be renewed", },

                        [45] = new() { [1] = "No, the circle will be renewed", },

                        [48] = new() { [0] = "You will be consumed", },
                        [51] = new() { [0] = "Along with the whole valley", },
                        [54] = new() { [0] = "ALL WILL BE JELLY", },

                        [58] = new() { [1] = "Your jelly is overrated", },

                    };

                    break;

                case QuestHandle.challengeAtoll:

                    sceneDialogue = new()
                    {

                        [1] = new() { [0] = "Oi matey!", },
                        [4] = new() { [0] = "Ya dare wield the Lady's power here?", },
                        [7] = new() { [0] = "The Fates take you!", },
                        // cannons
                        [16] = new() { [0] = "The Lady is not a friend to the drowned", },
                        [19] = new() { [0] = "She buried us with our boats on this shore", },
                        [22] = new() { [0] = "And the fae won't let us cross over", },
                        [25] = new() { [0] = "Until the ol' sea witch gets what's owed", },
                        // cannons
                        [34] = new() { [0] = "Then the waves will no longer wash our tattered bones", },
                        [37] = new() { [0] = "An we'll sink into the warm embrace of the earth", },
                        [40] = new() { [0] = "I think it's time ye join us, matey", },
                        // cannons
                        [49] = new() { [0] = "Yeaarggh", },
                        [52] = new() { [1] = "Bizarre... the Justiciar of Fate", },
                        [55] = new() { [1] = "Why would he refuse them passage to the afterlife", },

                        [994] = new() { [0] = "CANNONS AT THE READY!", },
                        [995] = new() { [0] = "FIRE!", },

                    };

                    break;

                case QuestHandle.challengeDragon:

                    sceneDialogue = new()
                    {

                        [1] = new() { [0] = "(menacing chuckles)", },
                        [4] = new() { [0] = "Something new stumbles into my lair", },
                        [7] = new() { [0] = "Ah... I smell... the " + Mod.instance.rite.castType.ToString(), },

                        [27] = new() { [0] = "I dared to harness a power", },
                        [30] = new() { [0] = "That would make me the envy of all guardians", },
                        [33] = new() { [0] = "My ambition angered the Fates", },
                        [36] = new() { [0] = "And they trapped me in a prison of my own hubris", },

                        [54] = new() { [0] = "You should be grateful", },
                        [57] = new() { [0] = "You'll soon be naught but dust and ash", },
                        [60] = new() { [0] = "Better by my hands than by the Reapers", },

                        [81] = new() { [0] = "Your death might please the high priestess", },
                        [84] = new() { [0] = "Perhaps I will be favoured", },
                        [87] = new() { [0] = "Maybe even freed", },
                        [90] = new() { [0] = "Oh to be free of fate", },
                        [93] = new() { [0] = "Yes. Now, DIE", },

                    };

                    break;

                case QuestHandle.swordFates:

                    sceneDialogue = new()
                    {

                        [2] = new() { [0] = "DUNGEON TIME!", },
                        [5] = new() { [0] = "Uh... did you just see a ghost? I just saw a ghost.", },
                        [8] = new() { [0] = "Ok time to run", },
                        [11] = new() { [0] = "Well this is a great start", },

                        [27] = new() { [0] = "Well I know we are definitely on the right path", },
                        [30] = new() { [0] = "These spectres bear traces of judgement", },
                        [33] = new() { [0] = "Fragments of souls convicted by the Reaper", },
                        [36] = new() { [0] = "They will linger in undeath until their sentence is up", },

                        [54] = new() { [0] = "Ah is there an end to this place?", },
                        [57] = new() { [0] = "Why is everything so grey", },
                        [60] = new() { [0] = "Then again I am colourblind to half the rainbow", },

                        [81] = new() { [0] = "I think we're getting closer!", },
                        [84] = new() { [0] = "Dont think there will be anything spooky at the end of this... right?", },

                        [91] = new() { [0] = "There... Thanatoshi... the Reaper", },
                        [94] = new() { [0] = "Hey farmer, there's something back here", },
                        [97] = new() { [0] = "Fate wins again!", },
                        [121] = new() { [0] = "Oh... wow", },

                    };

                    break;

                case QuestHandle.questJester:

                    sceneDialogue = new()
                    {

                        [901] = new() { [3] = "Well, by the look of it, the palentological hypothesis is...", },
                        [903] = new() { [0] = "Meow", },
                        [904] = new() { [3] = "That it's very old. Pre-catastrophe, perhaps.", },
                        [906] = new() { [1] = "Jest? (cough) bark", },
                        [907] = new() { [3] = "I'm more of a mythologist myself", },
                        [910] = new() { [3] = "Could be a legendary saurus, once the dominant species", },
                        [912] = new() { [0] = "Meowwwww", },
                        [913] = new() { [3] = "Before the advent of dragons", },
                        [915] = new() { [1] = "Bark. Borinnnng...", },
                        [916] = new() { [3] = "Huh? Did your fox just speak?", },
                        [919] = new() { [0] = "I sense... sadness... and rage", },
                        [922] = new() { [4] = "(grizzled roaring)", },
                        [923] = new() { [3] = "Ahhh! Protect the library!", },
                        [925] = new() { [4] = "Why am I here", },
                        [928] = new() { [3] = "What have I got to throw here...", },
                        [931] = new() { [4] = "I should be at rest, I should be...", },
                        [934] = new() { [3] = "It's defacing my inlaid hardwood panelling!", },
                        [937] = new() { [4] = "The power of the Stars has seeped into the land", },
                        [940] = new() { [4] = "The Fates continue to shun us", },
                        [943] = new() { [0] = "Dear ancient lizard, I am an envoy of Fate, the Jester", },
                        [946] = new() { [4] = "What, furred one? You are naught but a morsel", },
                        [949] = new() { [3] = "Crikey! If only I didn't loan our weapon collection to Zuzu mid!", },
                        [952] = new() { [4] = "Have the dragons abandoned this world?", },
                        [955] = new() { [4] = "The furries have taken dominion", },
                        [958] = new() { [3] = "Tell the guildmaster I wont accept any more cursed artifacts!", },
                        [961] = new() { [3] = "Farmer??! Can't you perform a rite of banishment or something?", },
                        [963] = new() { [1] = "The blue guy has a good idea!", },
                        [965] = new() { [3] = "This is going to cost the historic trust society", },
                        [968] = new() { [0] = "Well that was fun. But it's a bit smokey in here.", },
                        
                    };

                    break;


                case QuestHandle.challengeFates:
                    /*
                    sceneNarrators = new()
                    {
                        [0] = "The Effigy",
                        [1] = "Jester",
                        [2] = "Buffin",
                        [3] = "Shadow Leader",
                        [4] = "Shadow Sergeant",
                        [5] = "Shadow Goblin",
                        [6] = "Shadow Rogue",
                    };*/
                    sceneDialogue = new()
                    {

                        [2] = new() { [3] = "Hmm. The druids got a step ahead of us", },
                        [5] = new() { [0] = "Caution, interloper. We will not tolerate any more trespasses", },
                        [8] = new() { [3] = "You have developed some courage since our first encounter", },
                        [11] = new() { [0] = "That was before the successor ascended to archdruid of the circle", },
                        [14] = new() { [1] = "Yea, tell them woodface! Who's hiding in caves now?", },
                        [17] = new() { [3] = "Strange thing to say, but no matter", },
                        [20] = new() { [3] = "Surrender, and we will deal with you fairly, with your circle intact", },
                        [23] = new() { [2] = "YOUR TERMS ARE UNACCEPTABLE! EN GARDE!", },
                        [26] = new() { [1] = "Buffin, wait! Why is she always so feisty", },
                        
                        [30] = new() { [4] = "Sir they're fielding... animals", },
                        [33] = new() { [3] = "Unexpected, but we're still better prepared", },

                        [40] = new() { [1] = "Fear not Buffy, I will protect thee", },
                        [43] = new() { [2] = "I don't think they are in the mood for tricks, Jester", },

                        [50] = new() { [6] = "Shadows take thee, twinkle fingers", },
                        [53] = new() { [0] = "By decree of the Kings and the Lady Beyond", },
                        [56] = new() { [0] = "The sacred spaces shall not bear those of ill intent", },

                        [60] = new() { [3] = "Sergeant, command your brutes to suppress the golem", },
                        [63] = new() { [3] = "I will engage the Druid", },

                        [70] = new() { [2] = "Grrrr....Bark bark!", },
                        [73] = new() { [1] = "Purrrrrr... (hack) I mean, WOOF!", },
                        [76] = new() { [5] = "Stay back beasts!", },

                        [80] = new() { [4] = "I'm almost spent on ammunition", },
                        [83] = new() { [3] = "Those creatures are clearly not of the earthly variety", },
                        [86] = new() { [3] = "I suspect the Fates work against us. Retreat!", },

                        [90] = new() { [3] = "We're beaten. I should treaty with the druids", },
                        [93] = new() { [4] = "Sir, the other humans won't pay if we expose ourselves", },
                        [96] = new() { [3] = "You're still concerned with compensation?", },
                        [99] = new() { [5] = "We put our trust in coin... and dragon magic.", },
                        [102] = new() { [6] = "This is it for me. Too many set backs. Too many wounded.", },
                        [105] = new() { [5] = "Yea I'm done with Bear. ", },
                        [108] = new() { [4] = "You're a great scholar, sir, but...", },
                        [111] = new() { [4] = "We need a better point man.", },
                        [114] = new() { [5] = "The Deep One will know what to do", },
                        [117] = new() { [3] = "You're making a mistake! He is the Lord of ruin, our ruin.", },
                        [120] = new() { [6] = "Pfft. He's got the power. What can you do.", },
                        [123] = new() { [5] = "See ya Bear.", },

                        [126] = new() { [1] = "I feel bad for Burgundy Bear. He's professional", },
                        [129] = new() { [2] = "His humiliation is almost complete. Now for the coup de grace", },
                        [132] = new() { [0] = "Well Successor... you may determine the vanquished's fate", },

                    };

                    break;

                case "swordEther":

                    sceneDialogue = new()
                    {
                        [3] = new() { [1] = "You", },
                        [6] = new() { [1] = "You bear the scent... OF HERESY", },
                        [9] = new() { [0] = "What a moment...", },
                        [12] = new() { [0] = "Thanatoshi?", },

                        [15] = new() { [0] = "The dragon's power is mine to use!", },
                        [18] = new() { [0] = "I will reap, and reap, and reap", },

                        [21] = new() { [0] = "Farmer, it's him, The Reaper", },
                        [24] = new() { [0] = "Thanatoshi!", },
                        [27] = new() { [0] = "It is I, your kin, the Jester", },
                        [30] = new() { [0] = "Stop this madness!", },
                        [33] = new() { [0] = "It's no use, he won't listen", },

                        [42] = new() { [0] = "The seal to the undervalley will not withstand me", },
                        [45] = new() { [0] = "I will remain true to my purpose", },
                        [48] = new() { [0] = "Yoba will forgive me", },
                        [51] = new() { [0] = "Justice will favour me", },

                        [60] = new() { [0] = "That's... a cutlass... on the shaft", },
                        [63] = new() { [0] = "What has he done to himself?", },

                        [75] = new() { [0] = "Are you a spy of the star general", },
                        [78] = new() { [0] = "He cannot hope to match me now", },
                        [81] = new() { [0] = "Now...", },
                        [84] = new() { [0] = "How long has it been since I saw...", },

                        [93] = new() { [0] = "I guess we have no choice...", },
                        [96] = new() { [0] = "For Fate and Fortune!", },

                        [991] = new() { [0] = "Thanatoshi... why...", },
                        [992] = new() { [0] = "Masayoshi... why...", },
                    };

                    break;


                    /*
                case "challengeCanoli":
                case "challengeCanoliTwo":

                    sceneDialogue = new()
                    {

                        [1] = new() { [0] = "can you feel it", },
                        [3] = new() { [0] = "all around us", },
                        [5] = new() { [0] = "THE DUST RISES", },

                        [20] = new() { [0] = "the flowers the Lord grew here", },
                        [23] = new() { [0] = "the sweet berries I tendered", },
                        [26] = new() { [0] = "ashes drifting from the embers of their war", },
                        [29] = new() { [0] = "and now I am dust", },

                        [35] = new() { [0] = "ha ha ha", },
                        [38] = new() { [0] = "use these", },
                        [41] = new() { [0] = "blow the coal and chaff away", },
                        [47] = new() { [0] = "the monarchs sleep", },
                        [50] = new() { [0] = "and meeps creep into the world", },

                        [991] = new() { [0] = "the woods breathe again", },
                        [992] = new() { [0] = "the dust settles", },
                        [993] = new() { [0] = "dust overwhelming", },

                    };

                    break;

                case "challengeGemShrine":
                case "challengeGemShrineTwo":

                    sceneDialogue = new()
                    {
                        [1] = new() { [0] = "So the Sisters raised another to their priesthood", },
                        [4] = new() { [0] = "It matters not", },
                        [6] = new() { [0] = "They will not reclaim her", },

                        [10] = new() { [0] = "It was I who made it possible", },
                        [15] = new() { [0] = "For the first star to fall from heaven", },
                        [20] = new() { [0] = "Why did I profane my sacred duty", },
                        [25] = new() { [0] = "Why did I desecrate the boundaries of the planes", },
                        [30] = new() { [0] = "I witnessed their love", },
                        [35] = new() { [0] = "Shine through the smoke of war and ignorance", },
                        [40] = new() { [0] = "Beauty that warmed my frozen heart", },
                        [45] = new() { [0] = "Yoba forgive me", },

                        [991] = new() { [0] = "Abandon your folly. It cannot be undone.", },
                        [992] = new() { [0] = "the Sisters chose poorly", },

                    };

                    break;

                case "swordEther":
                case "swordEtherTwo":

                    sceneDialogue = new()
                    {

                        [5] = new() { [0] = "What a moment...", },
                        [10] = new() { [0] = "Thanatoshi?", },
                        [15] = new() { [0] = "Farmer, it's him, The Reaper", },
                        [20] = new() { [0] = "Thanatoshi!", },
                        [25] = new() { [0] = "It is I, your kin, the Jester", },
                        [30] = new() { [0] = "Stop this madness!", },
                        [35] = new() { [0] = "It's no use, he won't listen", },
                        [40] = new() { [0] = "That's... a cutlass... on the shaft", },
                        [45] = new() { [0] = "What has he done to himself?", },
                        [50] = new() { [0] = "I guess we have no choice...", },
                        [55] = new() { [0] = "For Fate and Fortune!", },


                        [1] = new() { [0] = "a taste of the stars", },
                        [3] = new() { [0] = "from the time when the shamans sang to us", },
                        [5] = new() { [0] = "and my kin held dominion", },
                        [7] = new() { [0] = "...my bones stir...", },

                        [991] = new() { [0] = "the power of the shamans lingers", },
                        [992] = new() { [0] = "You're no match for me", },
                        [201] = new() { [1] = "...yesss...", },
                        [203] = new() { [1] = "you have done well, shaman", },
                        [205] = new() { [1] = "...I return...", },
                        [215] = new() { [1] = "For centuries I lingered in bone", },
                        [220] = new() { [1] = "As the reaper leeched my life force", },
                        [225] = new() { [1] = "But an ancient is never truly gone", },
                        [230] = new() { [1] = "As long as my ether remains", },
                        [235] = new() { [1] = "I will gather the essence of your soul", },
                        [240] = new() { [1] = "And fashion a new form from your pieces", },
                        [245] = new() { [1] = "The Mistress of Fortune will face my wrath", },
                        [250] = new() { [1] = "I will make her my servant", },

                        [991] = new() { [0] = "Thanatoshi... why...", },
                        [992] = new() { [1] = "...rwwwghhhh...", },
                    };

                    break;

                */
            };

            return sceneDialogue;

        }

        public static void DisplayText(StardewValley.Monsters.Monster monster, int chance = 3, int type = 0)
        {

            List<string> textList = new();

            switch (type)
            {

                case 2: // panicList

                    if(monster is Phantom)
                    {
                        textList = new(){

                            "cover!",
                            "RUN",
                            "crikey!",
                            "CANNONBALL",
                        };

                    }

                    break;

                case 1: // smacklist

                    textList = new();

                    if (monster is Dragon)
                    {
                        textList = new(){
                            "the circle is weak",
                            "you'll never compare to the druids of old",
                            "the valley is cursed"
                        };

                    }

                    break;

                default: // 0 ouchlist

                    textList = new();

                    if (monster is Dragon)
                    {
                        textList = new(){
                            "Ah ha ha ha ha",
                            "Such pitiful strikes",
                            "insolence!",
                            "I'll Answer That... With FIRE!",
                            "The old ways have died... and so WILL YOU",
                        };

                    }

                    break;

            }

            if (textList.Count == 0)
            {

                return;

            }

            if(chance != -1)
            {
                
                if (Mod.instance.randomIndex.Next(chance) != 0)
                {

                    return;
                }


            }

            monster.showTextAboveHead(textList[Mod.instance.randomIndex.Next(textList.Count)], duration: 2000);

        }
        /*
        public static Dictionary<int, List<string>> EtherPages()
        {

            Dictionary<int, List<string>> etherPages = new()
            {
                [0] = new() {
                    "Ether soaked letter #1, Part 1",
                    "I strolled alone, in the valley untouched by the war of our great kin,",
                    "and I came upon a spring that mirrors the celestial plane.",
                    "I saw her reflection, and from the mirrored side, she saw mine.", },
                [1] = new() {
                    "Ether soaked letter #1, Part 2",
                    "I made a garden for her here, where the fruits grow sweetest,",
                    "and dyed the blossoms with all the burgeoning colours of my heart.",
                    "Her light dappled over the grove, bringing it to life.",
                    "She was pleased, and I was captivated.", },
                [2] = new() {
                    "Ether soaked letter #1, Part 3",
                    "I climbed the heights of the sacred mountain, and there,",
                    "I constructed a marriage altar. I looked upward for many nights.",
                    "Her light came again, and confirmed her own yearning.",
                    "I convinced the Benevolus Prime to conduct a rite of union.", },
                [3] = new() {
                    "Ether soaked letter #1, Part 4",
                    "By the purity and power of our union, the barriers between us broke.",
                    "Dear Lady, it is Yoba's will that I am with my love,",
                    "and there is nothing more to explain.", },
                [4] = new() {
                    "Ether soaked letter #2, Part 1",
                    "Dear Lord, your bride cannot be accommodated.",
                    "The powers at court are enraptured by her brilliance as it shines through the weald. ", },
                [5] = new() {
                    "Ether soaked letter #2, Part 2",
                    "All have come to covet her celestial beauty.",
                    "Even if our Kings do not claim her, the Dragons will surely challenge you.",
                    "For the sake of peace, she must return to her sisters.", },
                [6] = new() {
                    "Ether soaked letter #3",
                    "Lady, I will not be parted from the second part of my soul.",
                    "We will flee the vanity of the masters, and make a sanctuary,",
                    "in the secretest place, deep under the valley.", },

            };

            return etherPages;

        }

        public static Narrator EtherNarrator(int index)
        {
            switch (index)
            {
                case 4:
                case 5:
                    return new Narrator("Lady of the Grove", Color.Pink);
                default:
                    return new Narrator("Lord of the Valley", Color.LightGray);

            }

        }*/


    }

}
