/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using StardewValley;

namespace NermNermNerm.Stardew.QuestableTractor
{
    internal class SeederQuest
        : TractorPartQuest<SeederQuestState>
    {
        private bool pesteredGeorgeToday = false; // Ensure the player doesn't zero out their hearts with George banging on this quest.

        public const int GeorgeSendsBrokenPartHeartLevel = 3;
        private const int evelynWillingToHelpLevel = 3;
        private const int alexWillingToHelpLevel = 2;

        // Story:
        //   Before George's accident, Grandpa gave it to George to repair, and George had given him a parts list,
        //   but Grandpa had left it for a long time, then George had the accident and Grandpa didn't ask for it
        //   and then Grandpa died, and here we are.
        //
        //   After getting 4 hearts with George, he sends you the seeder and says it's all good except for the
        //   iron bars which Grandpa never came up with.
        //
        //   If you bring it back to George, he gets all grumpy and says he mailed it to you on-purpose and he's
        //   too old and broke down to fix it.  Quest directs you to talk to his "old friends", and everybody
        //   points you to Lewis.
        //
        //   Lewis says something like "Evelyn can be a big help if you earn her trust.  Talking to Evelyn gives
        //   you a grumpy response until you get a few hearts with her and then she confides that his hands are
        //   too shaky to do fine work anymore.
        //
        //   If you talk to Alex with less than 3 hearts, he blows you off with a "these hands were made for
        //   gridball, not farm work."
        //
        //   After talking to Alex once, talking to Granny and anybody else about it will say that Alex has a
        //   hard time trusting people and you should build up some friendship with him.
        //
        //   After Alex gets to 3 hearts, he'll confide that he just doesn't feel confident and he'll just screw
        //   up and make his grandpa mad.
        //
        //   If you talk to Lewis, Emily or Penny, they'll tell you to talk to Haley.
        //
        //   Haley will (regardless of your heart level with her), declare that confident men are more attractive
        //   and that she knows for sure his hands are plenty nimble enough to do the job and tells you she'll
        //   fix it for you.  A couple days later, you get a mail from her saying that she thinks she's got
        //   him on-side.
        //
        //   After that, you ask Alex and he says he'll do it.  Give him the item and the iron and he asserts
        //   that they'll take care of it.  After a couple days, you get a mail from him saying that the job's
        //   done and tells you to pick it up from George.
        //
        //   When you pick it up from George, you get a friendship heart for him, Alex, Evelyn and half a heart
        //   for Lewis.

        private const int ironBarCount = 10;

        public SeederQuest(SeederQuestController controller)
            : base(controller)
        {
            this.questTitle = "Fix the seeder";
            this.questDescription = "Turns out George had the seeder attachment, maybe he can be talked into fixing it.";
        }

        public void ReadyToInstall()
        {
            this.State = SeederQuestState.InstallPart;
        }

        public override void CheckIfComplete(NPC n, Item? item)
        {
            if (n.Name == "George" && this.State == SeederQuestState.GotPart && !this.pesteredGeorgeToday)
            {
                this.Spout(n, "Young people just don't listen.  I told you, I'm too old to repair your equipment.$a");
                Game1.player.changeFriendship(-120, n);
                n.doEmote(12); // Grumpiness emote
                this.pesteredGeorgeToday = true;
            }
            else if (n.Name == "Maru" && item?.ItemId == ObjectIds.BustedSeeder && this.State == SeederQuestState.GotPart)
            {
                this.Spout(n, "I'm honored that you're asking me to look at this, but given what you've said, and what I know about George, it'd really be better if George did the work...#$b#But I know it won't be easy for him.$s#$b#But if he actually did it, well, it'd do him a lot of good.#$b#You should talk to Lewis.  He and George go back a long way.  He'll know what to do.#$b#But if it all goes wrong, come back to me and I'll try to fix it.");
            }
            else if (n.Name == "Lewis" && this.State == SeederQuestState.GotPart)
            {
                this.Spout(n, "Oh dear oh dear oh dear...$2#$b#Hm...$2#$b#George isn't wrong -- physically, he's just not in good shape.  Mentally, though, he's still as sharp as ever.  Alas, not like your Grandpa, towards the end.$s#$b#But if he can somehow do this, or at least help in doing it, it'll do him so much good.#$b#We need some help here...  We need Evelyn, and YOU have to get her to cajole George into trying this.  I can't be seen to be involved for, err...  reasons.#$b#You need to build some trust with her before you broach the topic, however.  Tread carefully.");
                this.State = SeederQuestState.GetEvelynOnSide;
            }
            else if (n.Name == "Evelyn" && this.State == SeederQuestState.GetEvelynOnSide)
            {
                if (Game1.player.getFriendshipHeartLevelForNPC("Evelyn") >= evelynWillingToHelpLevel)
                {
                    this.Spout(n, "Ohhh...  I know George *could* repair that thing...  But he could also fail miserably, you see, it's not just his legs that have let him down.  Have you seen his hands, how they shake?  It's not so bad, but it really frustrates him.#$b#Oh deary, I'm old too...  I need time to think about this.");
                    Game1.player.mailForTomorrow.Add(MailKeys.EvelynPointsToAlex);
                    this.State = SeederQuestState.WaitForEvelyn;
                }
                else
                {
                    this.Spout(n, "Sorry, what did you say?  I didn't quite hear...");
                }
            }
            else if (n.Name == "Alex" && this.State == SeederQuestState.TalkToAlex1)
            {
                if (Game1.player.getFriendshipHeartLevelForNPC("Alex") < alexWillingToHelpLevel)
                {
                    this.Spout(n, "Look, I got my life to live, and it doesn't involve fixing farm equipment.");
                }
                else
                {
                    this.Spout(n, "Nah man, these hands were made for gridball.  You're good with this sort of thing, you should talk Grandpa into showing you how.  Or get Maru, she's good at mechanical stuff.$3");
                    this.State = SeederQuestState.GetHaleyOnSide;
                }
            }
            else if (n.Name == "Lewis" && this.State == SeederQuestState.GetHaleyOnSide)
            {
                this.Spout(n, "Heh, isn't easy is it.  Keep it up, maybe you'll learn how to be mayor someday!$1#$b#Evelyn was probably right, in that somebody his own age could do it.  Somebody he hangs out with alot.");
            }
            else if ((n.Name == "Sebastian" || n.Name == "Abigail" || n.Name == "Sam") && this.State == SeederQuestState.GetHaleyOnSide)
            {
                this.Spout(n, "You think *I* have a clue what goes on inside Alex's head??$5#$b#Oh wait, I *DO* know...  Absolutely nothing.#4");
            }
            else if (n.Name == "Emily" && this.State == SeederQuestState.GetHaleyOnSide)
            {
                this.Spout(n, "Oh I hate to admit it, but the solution to your problem is Haley.  She can make any boy do any thing, ESPECIALLY Alex.$2#$b#She might seem vaccuous at times, but trust me, if you need some guy manipulated, she can do it.  I'm not saying it's a good thing all the time, but this sounds like a good cause.$4");
            }
            else if (n.Name == "Haley" && this.State == SeederQuestState.GetHaleyOnSide)
            {
                this.Spout(n, "Hah.  Alex does more with his hands than just play Gridball.$3#$b#Tell me everything. every. little. detail.$7#$b#Ah...  I see...#$b#I'll take care of this for you.  Just give me a day or two.  I'll let you know.$2#$b#*Sigh*  Confidence is so very attractive in a man.$7");
                this.State = SeederQuestState.WaitForHaleyDay1;
                Game1.player.mailForTomorrow.Add(MailKeys.HaleysWorkIsDone);
            }
            else if (n.Name == "Alex" && this.State == SeederQuestState.TalkToAlex2)
            {
                this.Spout(n, $"Okay, I can do this.  Grandpa seems fired up too.  Give me the {ironBarCount} iron bars Grandpa said we need and the broken seeder and we'll get on it.");
                this.State = SeederQuestState.GiveAlexStuff;
            }
            else if (n.Name == "Alex" && this.State == SeederQuestState.GiveAlexStuff)
            {
                if (this.TryTakeItemsFromPlayer("335", ironBarCount, ObjectIds.BustedSeeder, 1))
                {
                    this.State = SeederQuestState.WaitForAlexDay1;
                    this.Spout(n, "Thanks, that's all the stuff.  Well, I'm off the the garage with Gramps.  I'll send mail or something after we get it working.");
                }
                else
                {
                    this.Spout(n, $"We'll need the old seeder and {ironBarCount} iron bars.  Bring 'em by when you can.");
                }
            }
            else if (n.Name == "George" && this.State == SeederQuestState.GetPartFromGeorge)
            {
                this.AddQuestItemToInventory(ObjectIds.WorkingSeeder);
                this.Spout(n, "There you go.  Fixed it myself.  Alex didn't screw it up too much; he's a good kid.#$b#Don't try and sprinkle chicken manure with the thing.  I don't want to see this thing back here again.");
                Game1.player.changeFriendship(240, n);
                n.doEmote(20); //hearts
                var evelyn = Game1.getCharacterFromName("Evelyn");
                Game1.player.changeFriendship(240, evelyn);
                evelyn?.doEmote(20);
                var alex = Game1.getCharacterFromName("Alex");
                Game1.player.changeFriendship(240, alex);
                alex?.doEmote(20);
                var lewis = Game1.getCharacterFromName("Lewis");
                Game1.player.changeFriendship(120, lewis);
                lewis?.doEmote(32); // smiley
                this.State = SeederQuestState.InstallPart;
            }
        }

        protected override void SetObjective()
        {
            switch (this.State)
            {
                case SeederQuestState.GotPart:
                    this.currentObjective = "Hm.  I wonder what I should do.  I certainly can't fix it, and it'd cost a fortune to send it to Zuza city.";
                    break;
                case SeederQuestState.GetEvelynOnSide:
                    this.currentObjective = $"Get Evelyn to help (Talk to her after getting her to {evelynWillingToHelpLevel} hearts)";
                    break;
                case SeederQuestState.WaitForEvelyn:
                    this.currentObjective = "Granny said she'll think about it.  I guess we'll wait to hear from her.";
                    break;
                case SeederQuestState.TalkToAlex1:
                    this.currentObjective = "Granny's mail said I should gain Alex's trust (2 hearts) and try and talk him into it.";
                    break;
                case SeederQuestState.GetHaleyOnSide:
                    this.currentObjective = "Alex still seems resistant to the idea.  Maybe I need to get somebody else to give me another angle.";
                    break;
                case SeederQuestState.WaitForHaleyDay1:
                    this.currentObjective = "Wait for Haley to cajole Alex into helping.";
                    break;
                case SeederQuestState.TalkToAlex2:
                    this.currentObjective = "Talk to Alex";
                    break;
                case SeederQuestState.GiveAlexStuff:
                    this.currentObjective = $"Bring Alex the old seeder and {ironBarCount} iron bars";
                    break;
                case SeederQuestState.WaitForAlexDay1:
                case SeederQuestState.WaitForAlexDay2:
                    this.currentObjective = "Hopefully Alex and George are getting on with it.  Alex said he'd send mail when it's working.";
                    break;
                case SeederQuestState.GetPartFromGeorge:
                    this.currentObjective = "Alex's mail says the seeder is done and I just need to get it from George.";
                    break;
                case SeederQuestState.InstallPart:
                    this.currentObjective = "Bring the fixed seeder to the tractor garage.";
                    break;
            }
        }

        public override void GotWorkingPart(Item workingPart)
        {
            this.State = SeederQuestState.InstallPart;
        }
    }
}
