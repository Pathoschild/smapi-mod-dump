/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public static class ConversationKeys
    {
        public const string TractorNotFound = "QuestableTractor.tractor_not_found";
        public const string LoaderNotFound = "QuestableTractor.loader_not_found";
        public const string ScytheNotFound = "QuestableTractor.scythe_not_found";
        public const string SeederNotFound = "QuestableTractor.seeder_not_found";
        public const string WatererNotFound = "QuestableTractor.waterer_not_found";

        public const string DwarfShoesTaken = "QuestableTractor.dwarf_shoes_taken";

        internal static void EditAssets(IAssetName nameWithoutLocale, IDictionary<string, string> topics)
        {
            if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Abigail"))
            {
                topics[ConversationKeys.TractorNotFound] = "Have you run across the old tractor yet?  It's off on the west side of the farm, in behind the some trees in an old lean-to.  It's just about one with the vegetation now, but it hasn't rusted away completely.  It sure would be fun to see it run!";
                topics[ConversationKeys.ScytheNotFound]
                  = topics[ConversationKeys.LoaderNotFound]
                  = "I used to tromp around your old farm; I loved the empty, haunted feel to it...$2#$b#Anyway...  I saw some things that probably work with the tractor, over on the South side of your farm near Marnie's ranch.#$b#One of them is buried under and old log and one is wedged into a boulder.#$b#Hey, if you get them working, does this mean I can drive the tractor?$4";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Demetrius"))
            {
                topics[ConversationKeys.TractorNotFound] = "Have you run across the old tractor yet?  I doubt it still works, but maybe it could be restored.";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Dwarf"))
            {
                topics[ConversationKeys.DwarfShoesTaken] = "Hey I saw you take those shoes.  I would have charged gold for them if I thought anybody would be stupid enough to want them.#$b#They don't fit. That's why I chucked them over there.  I'm glad you hauled off that junk.";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Evelyn"))
            {
                topics[ConversationKeys.LoaderNotFound]
                    = topics[ConversationKeys.ScytheNotFound]
                    = "You granddad loved that tractor of his, bless his heart, but you could never tell that judging by the dents and broken off parts!#$b#He left little bits of that thing scattered all over the farm, I'm afraid.  You'll probably come across bits of it here and there!";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/George"))
            {
                // It'd be kinda nice if this had a heart requirement
                topics[ConversationKeys.TractorNotFound] = "Have you run across the old tractor yet?  Your Grandpa kept it stored in a lean to out on the West side.#$b#It was a piece of junk when he bought it.  No idea how he kept it running.  He had no mechanical ability at all.  None.";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Gus"))
            {
                topics[ConversationKeys.WatererNotFound] = "I hear you found that old tractor - you should get Robin to tell you the tale of the irrigator.";
                topics[ConversationKeys.SeederNotFound] = "You know your Grandad and George were really good friends; it hit George pretty hard when your Granddad passed.$2#$b#You might do well to be nice to George, he might know a few secrets about your farm.";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Emily"))
            {
                topics[ConversationKeys.SeederNotFound] = "You know your Grandad and George were really good friends; it hit George pretty hard when your Granddad passed.$2#$b#You might do well to be nice to George, he might know a few secrets about your farm.";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Jaz")
                  || nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Vincent"))
            {
                // It'd be kinda nice if this had a heart requirement
                topics[ConversationKeys.ScytheNotFound]
                    = topics[ConversationKeys.LoaderNotFound]
                    = "Hey, wanna know a secret about your farm?  Down in the brambles near Marnie's house, there's Greebles.  They've made some weird machines too.$3#$b#No...  I've never seen a Greeble, but cats can!";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Jodi"))
            {
                topics[ConversationKeys.LoaderNotFound]
                    = topics[ConversationKeys.ScytheNotFound]
                    = "The kids used to play out in the south field near Marnie's house.  They often came home with tales of high adventure!$1#$b#You might poke around down there sometime when you need a break, who knows what you'll find!";
                topics[ConversationKeys.SeederNotFound] = "You know your Grandad and George were really good friends; it hit George pretty hard when your Granddad passed.$2#$b#You might do well to be nice to George, he might know a few secrets about your farm.";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Lewis"))
            {
                topics[ConversationKeys.WatererNotFound] = "I hear you found that old tractor - you should get Robin to tell you the tale of the irrigator.";
                topics[ConversationKeys.LoaderNotFound] = "One day your granddad came to ask me for help because he had wedged the front-end loader under a boulder he was moving on the south side of the farm.#$b#I told him that maybe his little tractor wasn't up to moving such a big rock...  He seemed to take that kinda personal; he did love that little tractor.$2#$b#But then his eyes lit up and he ran into Pierre's.$1#$b#I'm not sure what happened after that.";
                topics[ConversationKeys.SeederNotFound] = "You know your Grandad and George were really good friends; it hit George pretty hard when your Granddad passed.$2#$b#You might do well to be nice to George, he might know a few secrets about your farm.";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Marnie"))
            {
                topics[ConversationKeys.LoaderNotFound]
                    = topics[ConversationKeys.ScytheNotFound]
                    = "You know the kids used to play at your farm.  I've asked them to stay clear now that you're back.#$b#It's hard enough to run a farm with chickens running around, let alone kids!$1#$b#Ask me how I know...$2";
                topics[ConversationKeys.SeederNotFound] = "You know your Grandad and George were really good friends; it hit George pretty hard when your Granddad passed.$2#$b#You might do well to be nice to George, he might know a few secrets about your farm.";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Maru"))
            {
                topics[ConversationKeys.WatererNotFound] = "I hear you found the tractor!  Just a word of advice - might be best not to bring up the subject around my dad...$2#$b#It's a bit of a sore subject.#$b#You can talk to my Mom about it though - your only trouble will be getting her to stop!$1";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Pierre"))
            {
                topics[ConversationKeys.TractorNotFound] = "Have you run across the old tractor yet?  If you could get it running, it'd sure help you get more crops in the ground.#$b#If you need help finding it, Abigail might know where it is.";
                topics[ConversationKeys.WatererNotFound] = "I hear you found that old tractor - I wonder where all the attachments are?  One that's not a mystery is the irrigation rig.#$b#That's somewhere at the bottom of the farm pond, along with a big chunk of Demetrius' pride!#1";
                topics[ConversationKeys.LoaderNotFound] = "Did you ever find the front-end loader?  One day your Granddad came running into the store, bought a whole bunch of bombs and ran back out again.#$b#Lewis tells me he wedged the loader under a big rock and was trying to get it out...  So if you do find it, it might be in pieces!#1#$b#I'm just glad your Granddad didn't end up in pieces.$2#$b#That's why I don't sell explosives anymore.$2";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Robin"))
            {
                topics[ConversationKeys.TractorNotFound] = "Have you run across the old tractor yet?  That thing has some stories to tell, let me tell you.  It might make a good decoration for your front yard someday.";
                topics[ConversationKeys.WatererNotFound] = "Oh you want to find the tractor's irrigator?  Ha!  Good luck with that!$4#$b#Who knows, maybe you can fish it out!$1#$b#Demetrius tried to winch it out and ended up, well, let's just say the creases on his trousers weren't quite as crisp as usual after that attempt!$4";
                topics[ConversationKeys.SeederNotFound] = "You know your Grandad and George were really good friends and it hit George pretty hard when your Granddad passed.$2#$b#You might do well to be nice to George; he might know a few secrets about your farm.";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Willy"))
            {
                topics[ConversationKeys.TractorNotFound] = "Have you been out on the west side of your property yet?  Your Granddad had a tractor that he kept under a lean-to that always teetered on the brink of collapse.#$b#Know how he got it?  Me and Pappy boated it in from Knopperville.$l#$b#I didn't understand why he bought it -- the engine had thrown a rod, had bent crankshaft and a cracked case.$s#$b#But somehow he got it to live again!  Funny that.  He wasn't much of a mechanic...";
                topics[ConversationKeys.WatererNotFound] = "Ah, but yaknow, the biggest catch on your farm isn't the fish!  Nay laddy, it's the watering wagon for the tractor!$3#$b#That ol' thing sunk into the depths of the pond ne'er to be seen again!$1#$b#You'll probably lose a lure or two to it if you fish on your pond.$3";
                topics[ConversationKeys.SeederNotFound] = "You know your Grandad and George were really good friends; it hit George pretty hard when your Granddad passed.$2#$b#You might do well to be nice to George, he might know a few secrets about your farm.";
            }


            // Consider moving these somewhere else maybe?  Since they're not conversation keys
            if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Emily"))
            {
                // Just a little color to go with the shoes quest...  I picked this day to replace because the existing dialog is pretty weak.
                topics["winter_Sun"] = "I saw in one of Haley's magazines where it says that women unconsciously rate men based on their shoes...#$b#Hah!  I must be doing it wrong then.  If I ever did look at a man's shoes and I saw scuffed up old work boots, I'd be more attracted to him, not less!";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Gus"))
            {
                topics["fall_Thu6"] = "Your Grandad used to come in every Thursday night.  He had a standing date with George and Lewis; they'd play some cards and swap stories...#$b#That kept George going in the early years after the accident.  As you Granddad's mind started to slip away, that, well...$s#$b#George didn't need one more thing to feel bitter about, but life handed it to him anyway.$s";
            }
            else if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Wizard"))
            {
                // Consider making this a conversation key
                topics["fall_Thu8"] = topics["winter_Thu8"]
                    = "I have a bit of a confession to make...When I told you how to repair the tractor engine, you didn't *really* need to run around the chest six times...  I just...  Well...#$b#Sometimes I just can't help myself.";
            }
        }
    }
}
