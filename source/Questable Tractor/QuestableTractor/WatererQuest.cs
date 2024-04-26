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
using System.Linq;
using StardewValley;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public class WatererQuest
        : TractorPartQuest<WatererQuestState>
    {
        private const int goldBarCount = 10;

        public WatererQuest(WatererQuestController controller)
            : base(controller)
        {
            this.questTitle = "Fix the waterer";
            this.questDescription = "I found the watering attachment for the tractor, but it's in bad shape, I should ask around town.";
        }

        public override void CheckIfComplete(NPC n, Item? item)
        {
            bool itemIsWaterer = item?.ItemId == ObjectIds.BustedWaterer;
            if ((itemIsWaterer || this.State == WatererQuestState.NoCluesYet) && new string[] { "Clint", "Lewis", "Pierre", "Abigail", "Pam", "Marnie", "Willy", "Linus", "Gus", "George", "Caroline" }.Contains(n.Name))
            {
                this.Spout(n, "Oh, that isn't...  It is!  It's your grandpa's legendary irrigation attachment!  And you *fished* it up you say?  Hah!  Well you would'a had to, wouldn'tya!$1#$b#. . . #$b#Sorry, I gotta compose myself.  You'll want to take this up to the mountain.  Show it to Robin, she can give you the first-hand account.$1#$b#It might not be safe to show it to Demetrius.  I think he's still working through the afteraffects...$s");

                if (this.State == WatererQuestState.NoCluesYet)
                {
                    this.State = WatererQuestState.RobinFingered;
                }
            }
            else if (n.Name == "Demetrius" && itemIsWaterer && this.State < WatererQuestState.MaruFingeredByDemetrius)
            {
                this.Spout(n, "Oh my!  Is that the irrigation system?  It is!  None of us expected to see that again, not after that . . .$3#$b#Wait, did Robin put you up to this?#$b#Nope, I don't want to know.  Well, yes, I got very wet, but it wasn't any big deal.  Not nearly as much as she plays it up to be.  Not at all...#4#$b#But you just want the thing fixed, don't you.  Well, it seems like a complicated device, but I bet Maru would have no trouble with it.  Why don't you show it to her?");
                this.State = WatererQuestState.MaruFingeredByDemetrius;
                Game1.player.changeFriendship(-60, n);
                n.doEmote(12); // grumpy
            }
            else if (n.Name == "Robin" && (itemIsWaterer || this.State <= WatererQuestState.RobinFingered))
            {
                this.Spout(n, "Oh you didn't!!  You fished up the watering doohickey?  Oh my I'll never forget that day!  Your granddad had the idea that instead of using the pump to fill it up he could just back his tractor into the pond.  Suffice it to say the tractor came out, but the irrigator did not!  He huffed up to the mountain, half soaked, thinking that Demetrius would have a net.  Well Demetrius was feeling especially can-do that day and offered to come help and, well, Maru was quite small and just had to go with her Dad everywhere and so I came along to ride herd.  Long story short, Demetrius ended up covered in mud right up to his starched buttoned up collar.  Maru decided to go rescue him, I went chasing after her, and, well, we all ended up wet, but Demetrius, well, his self-image took a hit that day, heh.#$b#Heh, and you know what happened to the irrigator, donchanow!$l#$b#You should take it to Maru and, heh, youknow, best not to bring it up with Demetrius!$4");
                if (this.State < WatererQuestState.MaruFingered)
                {
                    this.State = WatererQuestState.MaruFingered;
                    Game1.player.changeFriendship(60, n);
                    n.doEmote(32); // smily
                }
            }
            else if (n.Name == "Maru")
            {
                switch (this.State)
                {
                    case WatererQuestState.NoCluesYet:
                    case WatererQuestState.RobinFingered:
                    case WatererQuestState.MaruFingered:
                    case WatererQuestState.MaruFingeredByDemetrius:
                        this.Spout(n, $"Sure, I'd love to have a go at fixing it, afterall, it's practically a family heirloom!$4#$b#But let's have a look at it...  Hm...$2#$b#Yeah, all the fittings are shot.  If you can get me {goldBarCount} gold bars, I can get it working again.#$b#I know, usually brass is used for this sort of thing, but it's surprisingly hard to come by around here.");
                        this.State = WatererQuestState.GetGoldBars;
                        break;
                    case WatererQuestState.GetGoldBars:
                        if (this.TryTakeItemsFromPlayer("(O)336", goldBarCount, ObjectIds.BustedWaterer, 1)) //336=gold bar
                        {
                            this.Spout(n, "Alrighty, I'll get to work on it and have it back to you in a couple days.  I'll just drop it in the mail for you.");
                            this.State = WatererQuestState.WaitForMaruDay1;
                        }
                        else if (itemIsWaterer)
                        {
                            this.Spout(n, "Have you found some gold bars yet?  Gotta go pretty deep in the mines to get it, but I'm sure you're up for it.");
                        }
                        break;
                    case WatererQuestState.WaitForMaruDay1:
                        this.Spout(n, "I'm still working on the irrigation system; you should have it day after tomorrow.");
                        break;
                    case WatererQuestState.WaitForMaruDay2:
                        this.Spout(n, "I'm working through the bugs on the irrigation system; you should have it tomorrow if nothing goes haywire.$3");
                        break;
                }
            }
        }

        protected override void SetObjective()
        {
            switch (this.State)
            {
                case WatererQuestState.NoCluesYet:
                    this.currentObjective = "Ask the people in town about this thing.";
                    break;
                case WatererQuestState.RobinFingered:
                    this.currentObjective = "Ask Robin about this thing.";
                    break;
                case WatererQuestState.MaruFingered:
                case WatererQuestState.MaruFingeredByDemetrius:
                    this.currentObjective = "Take it to Maru to see if she'll fix it.";
                    break;
                case WatererQuestState.GetGoldBars:
                    this.currentObjective = $"Bring the watering can and {goldBarCount} gold bars to Maru.";
                    break;
                case WatererQuestState.WaitForMaruDay1:
                case WatererQuestState.WaitForMaruDay2:
                    this.currentObjective = "Just gotta wait - should be in the mail any day now.";
                    break;
                case WatererQuestState.InstallPart:
                    this.currentObjective = "Bring the fixed waterer to the garage.";
                    break;
            }
        }

        public override void GotWorkingPart(Item workingPart)
        {
            this.Spout("Maru came through!  Time to take it to the garage and water some crops!");
            this.State = WatererQuestState.InstallPart;
        }
    }
}
