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
    internal partial class BorrowHarpoonQuest
        : BaseQuest<BorrowHarpoonQuestState>
    {
        public BorrowHarpoonQuest(BorrowHarpoonQuestController controller)
            : base(controller)
        {
            this.questTitle = "We need a bigger pole";
            this.questDescription = "There's something big down at the bottom of the farm pond.  Maybe Willy can loan me something to help get it out.";
        }

        protected override void SetObjective()
        {
            switch (this.State)
            {
                case BorrowHarpoonQuestState.GetThePole:
                    this.currentObjective = "Go find Willy in his shop";
                    break;
                case BorrowHarpoonQuestState.CatchTheBigOne:
                    this.currentObjective = "Use Willy's harpoon to haul in whatever's at the bottom of the pond";
                    break;
                case BorrowHarpoonQuestState.ReturnThePole:
                    this.currentObjective = "Give the Harpoon back to Willy";
                    break;
            }
        }

        public override bool IsConversationPiece(Item item) => item.QualifiedItemId == BorrowHarpoonQuestController.HarpoonToolQiid;

        public void LandedTheWaterer() => this.State = BorrowHarpoonQuestState.ReturnThePole;

        public override void CheckIfComplete(NPC n, Item? item)
        {
            if (n.Name == "Willy" && this.State == BorrowHarpoonQuestState.ReturnThePole && this.TryTakeItemsFromPlayer(BorrowHarpoonQuestController.HarpoonToolQiid))
            {
                this.Spout(n, "Ya reeled that ol water tank on wheels in, did ya laddy!$3#$b#Aye I do believe this'll be the talk of the Stardrop for many Fridays to come!$1");
                Game1.player.changeFriendship(240, n);
                n.doEmote(20); // hearts
                this.questComplete();
            }
            else if (n.Name == "Willy" && this.State == BorrowHarpoonQuestState.GetThePole && Game1.player.currentLocation.Name == "FishShop")
            {
                this.Spout(n, "Ah laddy...  I do think I know what you mighta hooked into and yer right that ya need a lot more pole than what you got.#$b#Here's a wee bit o' fishin' kit that my great great grandpappy used to land whales back before we knew better.#$b#I think ya will find it fit for tha purpose.");

                var harpoon = ItemRegistry.Create(BorrowHarpoonQuestController.HarpoonToolQiid);
                harpoon.specialItem = true;
                Game1.player.addItemByMenuIfNecessary(harpoon);
                this.State = BorrowHarpoonQuestState.CatchTheBigOne;
            }
            else if (n.Name == "Willy" && this.State == BorrowHarpoonQuestState.GetThePole && Game1.player.currentLocation.Name != "FishShop")
            {
                this.Spout(n, "Ah laddy...  I do think I know what you mighta hooked into and yer right that ya need a lot more pole than what you got.#$b#Come visit me in my shop and I'll set you up with something that might work");
            }
        }
    }
}
