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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.GarbageCans;

namespace NermNermNerm.Stardew.QuestableTractor
{
    /// <summary>
    ///  Implements the quest line for the Axe and Pick tools, which we're treating as a single thing.
    /// </summary>
    internal class LoaderQuest
        : TractorPartQuest<LoaderQuestState>
    {
        private static readonly Vector2 placeInMineForShoes = new Vector2(48, 8);

        public LoaderQuest(LoaderQuestController controller)
            : base(controller)
        {
            this.questTitle = "Fix the loader";
            this.questDescription = "I found the front end loader attachment for the tractor, but it's all bent up and rusted through in spots.";
        }

        protected override void SetObjective()
        {
            switch (this.State)
            {
                case LoaderQuestState.TalkToClint:
                    this.currentObjective = "This looks like a the sort of thing a blacksmith could fix.";
                    break;
                case LoaderQuestState.FindSomeShoes:
                    this.currentObjective = "Somehow I've been railroaded into finding Clint some new shoes.  Maybe some of the younger townspeople know where I could get some for cheap.";
                    break;
                case LoaderQuestState.SnagAlexsOldShoes:
                    this.currentObjective = "Somehow I've been maneuvered into finding Clint some new shoes.  Alex wears the same shoe size and tosses out shoes regularly...  Hm...  Ew...  Hm...";
                    break;
                case LoaderQuestState.LinusSniffing1:
                case LoaderQuestState.LinusSniffing2:
                case LoaderQuestState.LinusSniffing3:
                case LoaderQuestState.LinusSniffing4:
                case LoaderQuestState.LinusSniffing5:
                    this.currentObjective = "I've got Linus recruited to look out for shoes in Alex's trash can, but he warns me that there's something else that might be looking as well...  Who could that be?";
                    break;
                case LoaderQuestState.DisguiseTheShoes:
                    this.currentObjective = "Somehow I need to disguise these shoes so that Alex doesn't recognize them...  And maybe do something about that smudge.  Perhaps Sam or Sebastian would loan me some shoe polish.";
                    break;
                case LoaderQuestState.GiveShoesToClint:
                    this.currentObjective = "Give the shoes and the old loader to Clint";
                    break;
                case LoaderQuestState.InstallTheLoader:
                    this.currentObjective = "Take the fixed loader attachment to the tractor garage.";
                    break;
                case LoaderQuestState.WaitForClint1:
                case LoaderQuestState.WaitForClint2:
                    this.currentObjective = "Wait for Clint to finish repairing the loader.";
                    break;
                case LoaderQuestState.PickUpLoader:
                    this.currentObjective = "Clint should be done repairing the loader by now.";
                    break;
            }
        }

        public override bool IsItemForThisQuest(Item item) =>
            item.ItemId == ObjectIds.AlexesOldShoe || item.ItemId == ObjectIds.DisguisedShoe || base.IsItemForThisQuest(item);

        public override void GotWorkingPart(Item workingPart)
        {
            this.State = LoaderQuestState.InstallTheLoader;
        }

        public override void CheckIfComplete(NPC? n, Item? item)
        {
            if (n?.Name == "Clint" && this.State == LoaderQuestState.TalkToClint)
            {
                this.Spout(n, "Shoes.  That's my problem.  I wear these scuffed up old work boots all over the place.$2#$b#What??  Oh.  Sorry.  Just been a bit distracted because I saw on TV that women judge a man by their shoes and look at these...  No wonder I've got no luck with the ladies.$3#$b#What?  You want me to fix that thing?  Sure, looks like it'd be just a bit of reforging, some welds here and there...#$b#Wait!  You're from the city, you know all about shoes!  Tell you what, you get me a nice pair of shoes and I'll fix your loader.  Deal??#$b#GREAT!  I wear 14EEE.");
                this.State = LoaderQuestState.FindSomeShoes;
                this.InvalidateGarbageCanData();
            }
            else if (n?.Name == "Sam" && this.State < LoaderQuestState.SnagAlexsOldShoes && this.State > LoaderQuestState.TalkToClint)
            {
                this.Spout(n, "Shoes, yeah man, nice shoes cost a fortune.  My gig at the library barely pays, so I roll around in these supercheapies from Joja.  I color mine every once in a while so they look fresh.");
            }
            else if (n?.Name == "Abigail" && this.State < LoaderQuestState.SnagAlexsOldShoes && this.State > LoaderQuestState.TalkToClint)
            {
                this.Spout(n, "Cheap shoes!  And you somehow deduce that *I* am authority on such matters?$5#$b$Heh.  Perhaps I am!$1#$b#Back before the Jojamart we'd order them online, but now, I've learned the art of Thrift Stores.  I'm actually kindof glad it happened, I really like shopping at thrift stores.#$b#Cheaper than that?  Welp, you could try dumpster-diving!$1");
            }
            else if (n?.Name == "Haley" && this.State < LoaderQuestState.SnagAlexsOldShoes && this.State > LoaderQuestState.TalkToClint)
            {
                this.Spout(n, "Ladies' shoes I know.  Men's shoes I don't.  'Cheap' shoes is definitely not something I spend time thinking about.$3");
            }
            else if (n?.Name == "Emily" && this.State < LoaderQuestState.SnagAlexsOldShoes && this.State > LoaderQuestState.TalkToClint)
            {
                this.Spout(n, "Well, I mostly get my shoes from secondhand stores, but I don't really know about men's shoes.  Have you asked Sam, or Alex?");
            }
            else if (n?.Name == "Sebastian" && this.State < LoaderQuestState.SnagAlexsOldShoes)
            {
                this.Spout(n, "Shoes..  Yeah man, decent shoes cost a fortune.  I always wear black, though, which makes it easy.  Blacken anything and it looks cool.");
            }
            else if (n?.Name == "Alex" && this.State < LoaderQuestState.SnagAlexsOldShoes && this.State > LoaderQuestState.TalkToClint)
            {
                this.PlantShoesNextToDwarf();
                this.Spout(n, "I got these new shoes yesterday 'cuz my old pair had a brown smudge.#$b#I just threw them into the garbage. I would've donated them but I don't like the idea of some weirdo wearing my shoes, ya know?#$b#What size do I wear?  14EEE. . . .  Wait, why do you ask?");
                this.State = LoaderQuestState.SnagAlexsOldShoes;
            }
            else if (n?.Name == "Linus" && Game1.player.getFriendshipHeartLevelForNPC("Linus") >= 2 && this.State == LoaderQuestState.SnagAlexsOldShoes)
            {
                // The event where you catch linus dumpster diving is at ~.25 hearts, so at a level of 2, we can assume the player knows the sort of things Linus gets up to during the night...
                this.Spout(n, ". . . So...  what I think I'm hearing you say is you want me to scout the Mullner's trash can for shoes...#$b#You promise this is for a good cause?  Hm...  Okay.  I'll let you know if I come across them.#$b#I don't want to disturb you, but I'm not the only one nosing around town late at night.  I might not find them first.");
                this.State = LoaderQuestState.LinusSniffing1;
            }
            else if ((n?.Name == "Sam" || n?.Name == "Sebastian") && this.TryTakeItemsFromPlayer(ObjectIds.AlexesOldShoe))
            {
                string treat = (n.Name == "Sam" ? "Pizza" : "Sashimi");
                this.Spout(n, $"You want to borrow my shoe polish?  That's kindof an odd request but, you know what?  Sure.  Knock yourself out.#$b#There better be some {treat} in this for me somewhere down the road.");
                this.AddQuestItemToInventory(ObjectIds.DisguisedShoe);
            }
            else if (n?.Name == "Clint" && this.TryTakeItemsFromPlayer(ObjectIds.BustedLoader, 1, ObjectIds.DisguisedShoe, 1))
            {
                this.Spout(n, "Ah, these shoes look great!  Fit good too.  But somehow I still don't quite feel like a ladykiller.#$b#Welp.  A deal's a deal.  I'll fix the loader.  You can pick it up in a couple days.");
                this.State = LoaderQuestState.WaitForClint1;
            }
            else if (n?.Name == "Clint" && this.State == LoaderQuestState.PickUpLoader)
            {
                this.Spout(n, "Here's your front-end loader, all fixed up.  Stick to small rocks, right?#$b#If you need to move big ones, get some explosives for the job.  Oh, and let me know when you're doing it.  I'll bring beer.");
                this.AddQuestItemToInventory(ObjectIds.WorkingLoader);
            }
        }

        private void PlantShoesNextToDwarf()
        {
            var mines = Game1.locations.FirstOrDefault(l => l.Name == "Mine");
            if (mines is null)
            {
                this.LogWarning("Couldn't find the Mine?!");
                return;
            }

            var alreadyPlaced = mines.getObjectAtTile((int)placeInMineForShoes.X, (int)placeInMineForShoes.Y);
            if (alreadyPlaced is null)
            {
                var o = ItemRegistry.Create<StardewValley.Object>(ObjectIds.AlexesOldShoe);
                o.questItem.Value = true;
                o.Location = mines;
                o.TileLocation = placeInMineForShoes;
                this.LogInfo($"{ObjectIds.AlexesOldShoe} placed at {placeInMineForShoes.X},{placeInMineForShoes.Y}");
                o.IsSpawnedObject = true;
                mines.objects[o.TileLocation] = o;
            }
        }

        private void RemoveShoesNearDwarf()
        {
            var mines = Game1.locations.FirstOrDefault(l => l.Name == "Mine");
            var removedShoes = mines?.removeObject(placeInMineForShoes, showDestroyedObject: false);
            if (removedShoes is not null)
            {
                this.LogTrace("Shoes removed");
            }
            else
            {
                this.LogTrace("No shoes near dwarf to remove");
            }
        }

        private void InvalidateGarbageCanData()
        {
            this.Controller.Mod.Helper.GameContent.InvalidateCache("Data/GarbageCans");
        }

        internal void OnPlayerGotOldShoes(Item oldShoes)
        {
            Game1.player.holdUpItemThenMessage(oldShoes);
            if (this.State < LoaderQuestState.DisguiseTheShoes)
            {
                this.State = LoaderQuestState.DisguiseTheShoes;
            }

            if (Game1.player.currentLocation.Name == "Mine")
            {
                // crazy long duration since the player could take a while getting hold of the language scrolls.
                // Note that if the player talks to the dwarf, it'll probably eat this event anyway.  Such is life.
                Game1.player.activeDialogueEvents.Add(ConversationKeys.DwarfShoesTaken, 100);
            }

            this.InvalidateGarbageCanData();
            this.RemoveShoesNearDwarf();
        }

        internal void OnPlayerGotDisguisedShoes(Item dyedShoes)
        {
            Game1.player.holdUpItemThenMessage(dyedShoes);
            if (this.State < LoaderQuestState.GiveShoesToClint)
            {
                this.State = LoaderQuestState.GiveShoesToClint;
            }
        }
    }
}
