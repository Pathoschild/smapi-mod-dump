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

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public class RestoreTractorQuest
        : BaseQuest<RestorationState>
    {
        private bool hasDoneStatusCheckToday = false;

        public RestoreTractorQuest()
            : base(ModEntry.Instance.RestoreTractorQuestController)
        {
            this.questTitle = L("Investigate the tractor");
            this.questDescription = L("There's a rusty old tractor in the fields; it sure would be nice if it could be restored.  Perhaps the townspeople can help.");
        }

        public override bool IsConversationPiece(Item? item)
        {
            return item?.ItemId == ObjectIds.BustedEngine || item?.ItemId == ObjectIds.WorkingEngine;
        }

        protected override void SetObjective()
        {
            switch (this.State)
            {
                case RestorationState.TalkToLewis:
                    this.currentObjective = L("Talk to mayor Lewis");
                    break;

                case RestorationState.TalkToSebastian:
                    this.currentObjective = L("Ask Sebastian to help restore the tractor");
                    break;

                case RestorationState.TalkToLewisAgain:
                    this.currentObjective = L("Welp, Sebastian was a bust.  Maybe Mayor Lewis knows somebody else who could be more helpful.");
                    break;

                case RestorationState.WaitingForMailFromRobinDay1:
                case RestorationState.WaitingForMailFromRobinDay2:
                    this.currentObjective = L("Wait for Lewis to work his magic");
                    break;

                case RestorationState.BuildTractorGarage:
                    this.currentObjective = L("Get Robin to build you a garage to get the tractor out of the weather.");
                    break;

                case RestorationState.WaitingForSebastianDay1:
                case RestorationState.WaitingForSebastianDay2:
                    this.currentObjective = L("Sebastian promised to get on the job right after the barn got built.  Hopefully he's actually on the case.");
                    break;

                case RestorationState.TalkToWizard:
                    this.currentObjective = L("Ask for help with the strange tractor motor.");
                    break;

                case RestorationState.BringStuffToForest:
                    this.currentObjective = LF($"Put the engine, 20 sap, 5 mixed seeds and an aquamarine in a chest in the secret woods.");
                    break;

                case RestorationState.BringEngineToSebastian:
                    this.currentObjective = L("Hopefully the Junimo magic worked!  Get the engine out of the secret woods and bring it to Sebastian.");
                    break;

                case RestorationState.BringEngineToMaru:
                    this.currentObjective = L("Bring the engine to Maru to install.");
                    break;

                case RestorationState.WaitForEngineInstall:
                    this.currentObjective = L("Maru says that after the engine is installed, it should actually run!  Just have to wait a little bit longer...");
                    break;
            }
        }

        public override void CheckIfComplete(NPC n, Item? item)
        {
            if (n.Name == "Lewis" && this.State == RestorationState.TalkToLewis)
            {
                this.Spout(n, L("An old tractor you say?#$b#I know your Grandfather had one - I thought he had sold it off before he died.  He never could keep it on the furrows.$h#$b#If you want to get it fixed, I suggest you talk to Robin's son, Sebastian; he's actually quite the gearhead.  Maybe he can help."));
                this.State = RestorationState.TalkToSebastian;
            }
            else if (n.Name == "Sebastian" && this.State == RestorationState.TalkToSebastian)
            {
                this.Spout(n, L("Let me get this straight - I barely know who you are and I'm supposed to fix your rusty old tractor?$a#$b#Sorry, but I've got a lot of stuff going on and can't really spare the time."));
                Game1.drawDialogue(n);
                this.State = RestorationState.TalkToLewisAgain;
            }
            else if (n.Name == "Lewis" && this.State == RestorationState.TalkToLewisAgain)
            {
                this.Spout(n, L("He said that?$a#$b#Well, I can't say I'm really surprised...  Just a bit disappointed.$s#$b#Hm. . .$u#$b#Welp, I guess this is why they pay me the big money, eh?  I'll see if I can make this happen for you, but it might take a couple days."));
                Game1.drawDialogue(n);
                this.State = RestorationState.WaitingForMailFromRobinDay1;
            }
            // Maybe make an "if there's coffee involved it goes faster?" option?
            else if (n.Name == "Sebastian" && this.State == RestorationState.WaitingForSebastianDay1 && !this.hasDoneStatusCheckToday)
            {
                this.Spout(n, L("Trust me, I'm working on it, but I also have my day-gig to worry about.  I work odd hours, so you might not be around when I'm working on it.  Oh and thanks for the coffee."));
                Game1.drawDialogue(n);
                this.hasDoneStatusCheckToday = true;
            }
            else if (n.Name == "Sebastian" && this.State == RestorationState.WaitingForSebastianDay2 && !this.hasDoneStatusCheckToday)
            {
                this.Spout(n, L("I made a lot of progress last night.  Most of it is cleaning up okay, but the engine itself is, well, it seems a little out of the ordinary. . ."));
                Game1.drawDialogue(n);
                this.hasDoneStatusCheckToday = true;
            }
            else if (n.Name == "Wizard" && this.State == RestorationState.TalkToWizard)
            {
                this.Spout(n, LF($"Oh...  Now where did you get that??!$l#$b#Ooooh... Ah.  Yes.  I see...  Mmm...$s#$b#Yes.  Your grandfather dabbled a bit in Forest Magic.  He was nowhere near as adept as myself, of course...#$b#He lacked the mechanical ability to restore the mundane engine, so he enlisted some forest magic to make one.#$b#As you can see, the Junimos that he recruited to keep the motor running have gotten bored and wandered away.  You'll need to coax them back.$s#$b#Now, pay attention!  This will require your utmost concentration!$a#$b#You must place the engine, 20 sap, 5 mixed seeds and an aquamarine in a chest in the secret woods in front of the statue...#$b#Then, you must run around the chest, six times, clockwise very, very quickly.  Overnight, your engine will be restored.#$b#Now GO!  I have concerns much greater than yours right now.$a"));
                this.State = RestorationState.BringStuffToForest;
            }
            else if (n.Name == "Sebastian" && item?.ItemId == ObjectIds.BustedEngine)
            {
                this.Spout(n, L("That is the craziest engine I've ever seen.  Have  you shown it to Clint?  I mean, he knows something about metalworking.  Maybe it's some kinda weird alloy?^ ^Or...^Maybe aliens."));
            }
            else if (n.Name == "Clint" && item?.ItemId == ObjectIds.BustedEngine)
            {
                this.Spout(n, L("Uh...#$b#What is it?  you say it's an Engine?$s#$b#I say it's weird. . .  Hey, is that thing moving?$a#$b#I don't know.  Maybe the Wizard would know what it is, and even if he doesn't, he'll sure pretend like he does if you show it to him."));
            }
            else if ((n.Name == "Abigail" || n.Name == "Vincent") && item?.ItemId == ObjectIds.BustedEngine)
            {
                this.Spout(n, L("Oh wow...#$b#Can I have it?"));
            }
            else if (n.Name == "Marnie" && item?.ItemId == ObjectIds.BustedEngine)
            {
                this.Spout(n, L("AAAAHHH!!!  IT'S MOVING!  TAKE IT AWAY!$4"));
                // TODO: Remember that Marnie saw it and have gossip later about it.
            }
            else if (n.Name == "Maru" && item?.ItemId == ObjectIds.BustedEngine)
            {
                this.Spout(n, L("I've never seen anything like that before...#$b#It gives me this uncanny feeling like...  it's missing something.#$b#Weird."));
            }
            else if (n.Name == "Sebastian" && item?.ItemId == ObjectIds.WorkingEngine)
            {
                this.Spout(n, L("Whoah....$s#$b#I mean, if you say it's fixed, I can believe it.  Definitely has a look of workiness about it!$l#$b#But seriously...  I shouldn't be installing this thing.  It's, yaknow, out of my area but...$s#$b#I hate to say it, my sister would be able to figure it out, no matter how weird it is."));
                this.State = RestorationState.BringEngineToMaru;
            }
            else if (n.Name == "Maru" && this.State == RestorationState.BringEngineToMaru && this.TryTakeItemsFromPlayer(ObjectIds.WorkingEngine))
            {
                this.Spout(n, L("Wow!  I mean I have no idea what it does, but I'm sure it'll look cool doing it!$h#$b#You want me to install it in the tractor?  Sure, I'll do it.  I helped Seb haul it out of the mud.  He really did a great job polishing it up.#$b#Just give me a day or so, k?  And be sure to drive it up here sometime, I want to ride it around!$l"));
                this.State = RestorationState.WaitForEngineInstall;
            }
        }
    }
}
