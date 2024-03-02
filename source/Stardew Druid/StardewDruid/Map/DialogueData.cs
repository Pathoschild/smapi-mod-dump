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
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Dialogue;
using StardewDruid.Monster;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Map
{
    public static class DialogueData
    {

        public static Random random = new();

        public static Dictionary<int,Dialogue.Narrator> DialogueNarrator(string scene)
        {
            Dictionary<int, Dialogue.Narrator> sceneNarrators = new();

            switch (scene)
            {

                case "challengeEarth":
                case "challengeEarthTwo":

                    sceneNarrators = new() { [0] = new("Maskbat", Color.Gray), };

                    break;

                case "challengeWater":
                case "challengeWaterTwo":

                    sceneNarrators = new() { [0] = new("Shadow Sergeant", Color.Gray), };

                    break;

                case "challengeStarsTwo":
                case "challengeStars":

                    sceneNarrators = new() { [0] = new("Jellyking", Color.LightGoldenrodYellow), };

                    break;

                case "challengeMariner":
                case "challengeMarinerTwo":

                    sceneNarrators = new() { [0] = new("Phantom Mariner", Color.Gray), };

                    break;

                case "challengeCanoli":
                case "challengeCanoliTwo":

                    sceneNarrators = new() { [0] = new("Phantom Gardener", Color.Gray), };

                    break;

                case "challengeMuseum":
                case "challengeMueumTwo":

                    sceneNarrators = new() { [0] = new("Gunther", Color.White), [1] = new("Phantom Dinosaur", Color.Gray), };

                    break;

                case "challengeGemShrine":
                case "challengeGemShrineTwo":

                    sceneNarrators = new() { [0] = new("Phantom Voice", Color.Gray), };

                    break;

                case "challengeSandDragon":
                case "challengeSandDragonTwo":

                    sceneNarrators = new() { [0] = new("Phantom Tyrant", Color.Gray), };

                    break;

                case "challengeFates":
                case "challengeFatesTwo":

                    sceneNarrators = new() { [0] = new("The Jester of Fate", new Color(1f, 0.8f, 0.4f, 1f)), };

                    break;

                case "swordEther":
                case "swordEtherTwo":

                    sceneNarrators = new() { [0] = new("The Jester of Fate", new Color(1f, 0.8f, 0.4f, 1f)), [1] = new("Tyrannus Prime", Color.Gray), };

                    break;

                case "challengeEther":

                    sceneNarrators = new() { [0] = new("Shadowtin Bear", Color.Purple), };

                    break;

            };

            return sceneNarrators;

        }


        public static Dictionary<int,Dictionary<int,string>> DialogueScene(string scene)
        {

            Dictionary<int, Dictionary<int, string>> sceneDialogue = new();

            switch (scene)
            {

                case "challengeEarth":
                case "challengeEarthTwo":

                    sceneDialogue = new()
                    {

                        [22] = new() { [0] = "...farmer...", },
                        [24] = new() { [0] = "...you tresspass...", },
                        [26] = new() { [0] = "cheeep cheep", },
                        [28] = new() { [0] = "...your kind...", },
                        [30] = new() { [0] = "...defile waters...", },
                        [32] = new() { [0] = "cheeep cheep", },
                        [34] = new() { [0] = "Our Favoured Lady", },
                        [37] = new() { [0] = "...is angry...", },
                        [39] = new() { [0] = "CHEEEP", },
                        [56] = new() { [0] = "...rocks hurt...", },
                        [58] = new() { [0] = "CHEEE--- aack", },
                    };

                    break;

                case "challengeWater":
                case "challengeWaterTwo":

                    sceneDialogue = new()
                    {

                        [2] = new() { [0] = "discovery!", },
                        [6] = new() { [0] = "ENGAGE", },
                        [9] = new() { [0] = "secure the graveyard perimeter", },
                        [12] = new() { [0] = "FIRE", },
                        [15] = new() { [0] = "seal the crypt", },
                        [18] = new() { [0] = "ENGAGE", },
                        [21] = new() { [0] = "surround the farmer", },
                        [24] = new() { [0] = "FIRE", },
                        [27] = new() { [0] = "secure the ether stores", },
                        [30] = new() { [0] = "ENGAGE", },
                        [33] = new() { [0] = "there is no retreat for us", },
                        [36] = new() { [0] = "the Deep One will know of our failure", },
                        [39] = new() { [0] = "ADVANCE", },
                        [56] = new() { [0] = "such power", },
                        [58] = new() { [0] = "Lord Deep... forgive us", },
                    };

                    break;

                case "challengeStarsTwo":
                case "challengeStars":

                    sceneDialogue = new()
                    {

                        [15] = new() { [0] = "HOW BORING", },
                        [18] = new() { [0] = "the monarchs must be asleep.", },
                        [21] = new() { [0] = "if they send only a farmer", },
                        [24] = new() { [0] = "to face the onslaught...", },
                        [27] = new() { [0] = "OF THE MIGHTY SLIME", },
                        [30] = new() { [0] = "you will be consumed", },
                        [33] = new() { [0] = "along with the whole valley", },
                        [36] = new() { [0] = "ALL WILL BE JELLY", },
                        [55] = new() { [0] = "bloop?", },
                        [56] = new() { [0] = "that's a lot of star power", },
                        [58] = new() { [0] = "!!!!", },

                    };

                    break;

                case "challengeMariner":
                case "challengeMarinerTwo":

                    sceneDialogue = new()
                    {

                        [1] = new() { [0] = "oi matey!", },
                        [3] = new() { [0] = "ya dare wield the Lady's power here?", },
                        [5] = new() { [0] = "the deep one take you!", },

                        [12] = new() { [0] = "ya can't touch me! I be a reflection!", },
                        [15] = new() { [0] = "this ere beach is for private members!", },
                        [24] = new() { [0] = "the Lady is not a friend to the drowned", },
                        [27] = new() { [0] = "she buried us with our boats on this shore", },
                        [30] = new() { [0] = "but soon Lord Deep will avenge us!", },
                        [33] = new() { [0] = "he'll swallow the ol' sea witch whole", },
                        [36] = new() { [0] = "then the waves will no longer wash our tattered bones", },
                        [39] = new() { [0] = "an we'll sink into the warm embrace of the earth", },

                        [991] = new() { [0] = "aye... maybe as strong as Deep himself...", },
                        [992] = new() { [0] = "eh. take this an sod off.", },
                        [993] = new() { [0] = "haha! not good enough even for the Lady", },
                        [994] = new() { [0] = "CANNONS AT THE READY!", },
                        [995] = new() { [0] = "FIRE!", },
                    };

                    break;

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
                        [38] = new() { [0] = "deploy the bombs", },
                        [41] = new() { [0] = "blow the coal and chaff away", },
                        [47] = new() { [0] = "the monarchs sleep", },
                        [50] = new() { [0] = "and meeps creep into the world", },

                        [991] = new() { [0] = "the woods breathe again", },
                        [992] = new() { [0] = "the dust settles", },
                        [993] = new() { [0] = "dust overwhelming", },

                    };

                    break;

                case "challengeMuseum":
                case "challengeMuseumTwo":

                    sceneDialogue = new()
                    {
                        [1] = new() { [0] = "Farmer? What's this?", },
                        [4] = new() { [1] = "croak", },
                        [3] = new() { [0] = "Oh... oh no no no", },
                        [4] = new() { [1] = "CROAK", },
                        [5] = new() { [0] = "Protect the library!", },

                        [10] = new() { [0] = "What have I got to throw here...", },
                        [15] = new() { [0] = "Pre-cretacious creep!", },
                        [20] = new() { [0] = "It's defacing my inlaid hardwood panelling!", },

                        [25] = new() { [0] = "Crikey! If only I didn't loan our weapon collection to Zuzu mid!", },
                        [30] = new() { [0] = "Marlon has a lot to answer for", },
                        [35] = new() { [0] = "Tell the guildmaster I wont accept any more cursed artifacts!", },
                        [40] = new() { [0] = "How are you doing these amazing feats of magic?", },

                        [42] = new() { [1] = "Stop throwing your trash at me old man!", },
                        [45] = new() { [0] = "Can't you perform a rite of banishment or something?", },
                        [50] = new() { [0] = "Goodbye, priceless artifact. Sniff.", },
                        [54] = new() { [0] = "Leave the corpse. I might be able to sell it's parts.", },
                        [57] = new() { [0] = "This is going to cost the historic trust society", },

                        [991] = new() { [0] = "One for the books.", },
                        [992] = new() { [0] = "ughh... what a mess", },
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

                case "challengeSandDragon":
                case "challengeSandDragonTwo":

                    sceneDialogue = new()
                    {
                        [1] = new() { [0] = "a taste of the stars", },
                        [3] = new() { [0] = "from the time when the shamans sang to us", },
                        [5] = new() { [0] = "and my kin held dominion", },
                        [7] = new() { [0] = "...my bones stir...", },

                        [991] = new() { [0] = "the power of the shamans lingers", },
                        [992] = new() { [0] = "You're no match for me", },

                    };

                    break;

                case "challengeFates":
                case "challengeFatesTwo":

                    sceneDialogue = new()
                    {

                        [5] = new() { [0] = "uh... portal?", },
                        [8] = new() { [0] = "get ready for a fight!", },
                        [20] = new() { [0] = "whoa... that one is massive!", },

                        [30] = new() { [0] = "keep it up farmer!", },
                        [34] = new() { [0] = "meet your fate voidspawn!", },
                        [40] = new() { [0] = "if only Lucky could see this", },
                        [52] = new() { [0] = "whew...the portal is finally closing", },

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

                        [991] = new() { [0] ="Thanatoshi... why...", },
                        [992] = new() { [1] = "...rwwwghhhh...", },
                    };

                    break;

                case "challengeEther":

                    sceneDialogue = new()
                    {


                        [10] = new() { [0] = "I can't believe it", },

                        [14] = new() { [0] = "A Dragon? Here? Now?", },

                        [18] = new() { [0] = "This changes everything", },

                        [22] = new() { [0] = "So you defeated the scouts", },

                        [26] = new() { [0] = "They shouldn't have invaded the surface", },

                        [30] = new() { [0] = "But the Deep one willed it", },

                        [34] = new() { [0] = "So they acquiesced", },

                        [38] = new() { [0] = "I found the squad leader's hat and crossbow", },

                        [42] = new() { [0] = "The power that slew him is unmistakable", },

                        [46] = new() { [0] = "The Lady Beyond punishes us still", },

                        [60] = new() { [0] = "How did you find this hideout?", },

                        [64] = new() { [0] = "We can't let you leave", },

                        [68] = new() { [0] = "Lord Deep will not be pleased", },

                        [72] = new() { [0] = "We haven't collected enough ether", },

                        [76] = new() { [0] = "The Starlet will fade without it", },

                    };

                    break;

            };

            return sceneDialogue;

        }

        public static List<string> DialogueHard(string monster)
        {
            List<string> hardDialogue = new();

            switch (monster)
            {

                case "PurpleDragon":

                    hardDialogue = new(){

                        "Where are my servants",
                        "The shamans have failed me",
                        "The only recourse for humanity, is subjugation",
                        "Beg for my mercy",
                        "Kneel Before Tyrannus!",
                        "I WILL BURN... EVERYTHING",

                    };

                    break;

                case "Rogue":

                    hardDialogue = new(){

                        "get out of here",
                        "how did you find us?",
                        "no mercy",
                        "into the shadows I go"

                    };

                    break;
            }

            return hardDialogue;

        }

        public static List<string> DialogueSmack(string monster)
        {
            List<string> smackDialogue = new();

            switch (monster)
            {
                
                case "PurpleDragon":

                    smackDialogue =  new(){

                        "behold",
                        "I am your new master",
                        "Kneel Before Tyrannus!",
                        "Why do you resist",
                        "Where are my servants?",
                    
                    };

                    break;

                case "Rogue":

                    smackDialogue = new(){

                        "get out of here",
                        "how did you find us?",
                        "no mercy",
                        "into the shadows I go"

                    };

                    break;

                case "Reaper":

                    smackDialogue = new(){

                        "Do not touch the Prime!",
                        "How long has it been since I saw...",
                        "The dragon's power is mine to use!",
                        "I will not stray from my purpose",
                        "Are you a spy of the fallen one?",
                        "The undervalley... I must...",
                        "I will reap, and reap, and reap",
                        "FORTUMEI... PLEASE... I BEG YOU",
                        "ALL WILL BE REAPED"
                    };

                    break;

                case "Dinosaur":

                    smackDialogue = new()
                    {
                        "Why am I here",
                        "The power of the Stars has seeped into the land",
                        "I should be at rest, I should be...",
                        "Surrender, and I'll give you a pony ride",
                        "STOP MOVING. JUST BURN.",
                        "My helmet provides +3 Intelligence!"

                    };

                    break;

            }

            return smackDialogue;

        }

        public static List<string> DialogueHurt(string monster)
        {

            List<string> hurtDialogue = new();

            switch (monster)
            {

                case "Dust Spirit":

                    hurtDialogue = new(){

                    "ow ow",
                    "ouchies",
                    "meep meep?",
                    "meep",
                    "MEEEP",

                    };

                    break;

                case "Green Slime":

                    hurtDialogue = new(){

                    "blup blup",
                    "bloop",

                    };

                    break;

                case "Shadow Sniper":

                    hurtDialogue = new(){

                    "ooft",
                    "a worthy opponent",
                    "deep deep!"

                    };

                    break;

                case "Skeleton":
                case "Stone Golem":
                    hurtDialogue = new(){

                    "deep",
                    "yeoww",
                    "crikey!",
                    "DEEP",
                    "shivers",
                    "timbers",
                    "yarr",

                    };

                    break;

                case "Shadow Brute":

                    hurtDialogue = new(){
                    "oooft",
                    "deep",

                    };

                    break;

                case "Big Slime":

                    hurtDialogue = new(){
                    "bloop",
                    "bloop bloop",
                    "jelly superiority!",

                    };

                    break;

                case "Bat":

                    hurtDialogue = new(){
                    "flap flap",
                    "flippity",
                    "cheeep"

                    };

                    break;

                case "FireBird":

                    hurtDialogue = new(){
                    "Tweep",
                    "TWEEEEPPPP",

                    };

                    break;

                case "PurpleDragon":

                    hurtDialogue = new(){
                    "Ah ha ha ha ha",
                    "Such pitiful strikes",
                    "insolence!",
                    "I'll Answer That... With FIRE!",
                    "The land has died... and so WILL YOU",
                    "CREEP"
                    };

                    break;

                case "BlackDragon":

                    hurtDialogue = new(){

                        "Ah ha ha ha ha",
                        "PATHETIC",
                        "You're nothing. NOTHING.",
                    };

                    break;
                case "ShadowTin":

                    hurtDialogue = new(){
                    "unbearable",
                    "the power",
                    "ooft",
                    };

                    break;

                case "Scavenger":

                    hurtDialogue = new(){
                    "meow meow",
                    "meow",
                    };

                    break;
                case "Rogue":

                    hurtDialogue = new(){

                        "ooft",
                        "ouch!",
                        "shadows take you"

                    };

                    break;
                case "Reaper":

                    hurtDialogue = new(){

                        "reap",
                        "...you cannot defy fate...",
                        "ENOUGH"

                    };

                    break;

                case "Dinosaur":

                    hurtDialogue = new(){
                        "ouch",
                        "croak",
                        "can't you aim for the helmet?"

                    };

                    break;



            }

            return hurtDialogue;

        }                

        public static List<string> DialoguePanic(string monster)
        {
            List<string> panicDialogue = new();

            switch (monster)
            {

                case "Skeleton":
                case "Stone Golem":
                    panicDialogue = new(){

                        "cover!",
                        "RUN",
                        "crikey!",
                        "CANNONBALL",
                    };

                    break;

                case "Scavenger":

                    panicDialogue = new(){

                        "meow",
                        "mine mine!",
                        "rwwwwrr",
                        "where bear"
                    };

                    break;
                case "Rogue":

                    panicDialogue = new(){
                        "go away!",
                        "the Ether belongs to Lord Deep",
                        "thanks for finding the treasure for me",
                        "where's Shadowtin when I need him"

                    };

                    break;
            }

            return panicDialogue;

        }

        public static void DisplayText(StardewValley.Monsters.Monster monster, int chance = 3, int type = 0, string name = "0")
        {

            if(name == "0")
            {

                name = monster.Name;

            }

            List<string> textList;

            switch (type)
            {

                case 3: // hardList

                    textList = DialogueHard(name);

                    break;

                case 2: // panicList

                    textList = DialoguePanic(name);

                    break;

                case 1: // smacklist

                    textList = DialogueSmack(name);

                    break;

                default: // 0 ouchlist

                    textList = DialogueHurt(name);

                    break;

            }

            if(textList.Count == 0)
            {

                return;

            }

            if(random.Next(chance) != 0)
            {

                return;
            }

            monster.showTextAboveHead(textList[random.Next(textList.Count)], duration: 2000);

        }

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

        }


    }

}
