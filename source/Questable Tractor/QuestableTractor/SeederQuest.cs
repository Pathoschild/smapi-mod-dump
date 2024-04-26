/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using System.Linq;
using StardewValley;
using StardewValley.Network;

namespace NermNermNerm.Stardew.QuestableTractor
{
    internal class SeederQuest
        : TractorPartQuest<SeederQuestState>
    {
        public const int GeorgeSendsBrokenPartHeartLevel = 3;
        private const int evelynWillingToHelpLevel = 3;
        private const int alexWillingToHelpLevel = 2;

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
            bool isHoldingSeeder = item?.ItemId == ObjectIds.BustedSeeder;
            if (n.Name == "George" && isHoldingSeeder && this.State == SeederQuestState.GotPart)
            {
                this.Spout(n, "Young people just don't listen.  I told you, I'm too old to repair your equipment.$a");
                Game1.player.changeFriendship(-120, n);
                n.doEmote(12); // Grumpiness emote
                this.State = SeederQuestState.GeorgePestered;
            }
            if (new string[] { "Clint", "Pierre", "Gus", "Caroline", "Jodi", "Penny" }.Contains(n.Name) && isHoldingSeeder && this.State < SeederQuestState.GetEvelynOnSide)
            {
                this.Spout(n, "Hm.  George said that did he...  Hm.  This *is* a thorny problem.  I think you need to consult a politician.  Try Lewis.");
            }
            else if (n.Name == "Maru" && isHoldingSeeder && this.State <= SeederQuestState.GeorgePestered)
            {
                this.Spout(n, "I'm honored that you're asking me to look at this, but it'd really be better if George did the work...#$b#I know it won't be easy for him...  Maybe he'll need help.$s#$b#But if he actually did it, well, it'd do him a lot of good.#$b#You should talk to Lewis.  He and George go back a long way.  He'll know what to do.#$b#If it all goes wrong, come back to me and I'll try to fix it.");
            }
            else if (n.Name == "Maru" && isHoldingSeeder && this.State == SeederQuestState.GetHaleyOnSide)
            {
                this.Spout(n, "I'm flattered that Alex thinks I can fix it...  Maybe I can, but we should stick to Evelyn's plan.  Alex and I don't exactly click, but I'm sure there's somebody who knows Alex well enough to help convince him to do it.");
            }
            else if (n.Name == "Lewis" && this.State <= SeederQuestState.GeorgePestered)
            {
                this.Spout(n, "Oh dear oh dear oh dear...$2#$b#Hm...$2#$b#George isn't wrong -- physically, he's just not in good shape.  Mentally, though, he's still as sharp as ever.  Alas, not like your Grandpa, towards the end.$s#$b#But if he can somehow do this, or at least help in doing it, it'll do him so much good.#$b#We need some help here...  We need Evelyn, and YOU have to get her to cajole George into trying this.  I can't be seen to be involved for, err...  reasons.#$b#You need to build some trust with her before you broach the topic, however.  Tread carefully.");
                this.State = SeederQuestState.GetEvelynOnSide;
            }
            else if (n.Name == "Evelyn" && this.State == SeederQuestState.GetEvelynOnSide && Game1.player.getFriendshipHeartLevelForNPC("Evelyn") >= evelynWillingToHelpLevel)
            {
                this.Spout(n, "Ohhh...  I know George *could* repair that thing...  But he could also fail miserably, you see, it's not just his legs that have let him down.  Have you seen his hands, how they shake?  It's not so bad, but it really frustrates him.#$b#Oh deary, I'm old too...  I need time to think about this.");
                Game1.player.mailForTomorrow.Add(MailKeys.EvelynPointsToAlex);
                this.State = SeederQuestState.WaitForEvelyn;
            }
            else if (n.Name == "Evelyn" && isHoldingSeeder && this.State == SeederQuestState.GetEvelynOnSide && Game1.player.getFriendshipHeartLevelForNPC("Evelyn") < evelynWillingToHelpLevel)
            {
                this.Spout(n, "Sorry, what did you say?  I didn't quite hear...");
            }
            else if (n.Name == "Alex" && isHoldingSeeder && this.State == SeederQuestState.TalkToAlex1 && Game1.player.getFriendshipHeartLevelForNPC("Alex") < alexWillingToHelpLevel && isHoldingSeeder)
            {
                this.Spout(n, "Look, I got my life to live, and it doesn't involve fixing farm equipment.");
            }
            else if (n.Name == "Alex" && this.State == SeederQuestState.TalkToAlex1 && Game1.player.getFriendshipHeartLevelForNPC("Alex") >= alexWillingToHelpLevel)
            {
                this.Spout(n, "Nah man, these hands were made for gridball.  You're good with this sort of thing, you should talk Grandpa into showing you how.  Or get Maru, she's good at mechanical stuff.$3");
                this.State = SeederQuestState.GetHaleyOnSide;
            }
            else if (n.Name == "Lewis" && isHoldingSeeder && this.State == SeederQuestState.GetHaleyOnSide)
            {
                this.Spout(n, "Heh, isn't easy is it.  Keep it up, maybe you'll learn how to be mayor someday!$1#$b#Evelyn was probably right, in that somebody his own age could convince him.  Somebody he spends a lot of time with.");
            }
            else if ((n.Name == "Sebastian" || n.Name == "Abigail" || n.Name == "Sam") && isHoldingSeeder && this.State == SeederQuestState.GetHaleyOnSide)
            {
                this.Spout(n, "You think *I* have a clue what goes on inside Alex's head??$5#$b#Oh wait, I *DO* know...  Absolutely nothing.$4");
            }
            else if (n.Name == "Emily" && isHoldingSeeder && this.State == SeederQuestState.GetHaleyOnSide)
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
                if (this.TryTakeItemsFromPlayer("(O)335", ironBarCount, ObjectIds.BustedSeeder, 1))
                {
                    this.State = SeederQuestState.WaitForAlexDay1;
                    this.Spout(n, "Thanks, that's all the stuff.  Well, I'm off the the garage with Gramps.  I'll send mail or something after we get it working.");
                }
                else if (isHoldingSeeder)
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
                case SeederQuestState.GeorgePestered:
                    this.currentObjective = "Talking to George didn't go well.  I'm going to have to find a different angle.";
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
