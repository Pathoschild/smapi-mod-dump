using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.API
{
    public class Player
    {
        public int GetFriendshipPoints(string NPC)
        {
            Farmer f2 = Game1.player;
            if (f2.friendshipData.ContainsKey(NPC)) return f2.friendshipData[NPC].Points;
            else return 0;
        }

        public FriendshipStatus GetFriendshipStatus(string NPC)
        {
            if (!Game1.player.friendshipData.ContainsKey(NPC)) Game1.player.friendshipData[NPC] = new Friendship(0);
            return Game1.player.friendshipData[NPC].Status;
        }

        public void SetFriendshipStatus(string NPC, FriendshipStatus status)
        {
            if (!Game1.player.friendshipData.ContainsKey(NPC)) Game1.player.friendshipData[NPC] = new Friendship(0);
            Game1.player.friendshipData[NPC].Status = status;
        }

        public void SetFriendshipPoints(string NPC, int points)
        {
            Farmer f2 = Game1.player;
            if (!f2.friendshipData.ContainsKey(NPC)) f2.friendshipData[NPC] = new Friendship(points);
            else f2.friendshipData[NPC].Points = points;
        }

        public void GiveItem(int id, int count = 1)
        {
            var i = Modworks.Items.CreateItemstack(id, count);
            Game1.player.addItemByMenuIfNecessary(i);
        }

        public void RemoveItem(Item which, int count = 1)
        {
            int currentQuantity = which.getStack();
            if (count > currentQuantity)
            {
                Game1.player.removeItemsFromInventory(which.ParentSheetIndex, count);
            }
            else if (count == currentQuantity)
            {
                Game1.player.removeItemFromInventory(which);
            }
            else
            {
                which.Stack = which.Stack - count;
            }
        }

        public int[] GetStandingTileCoordinate()
        {
            var f = Game1.player;
            return new int[] { f.getTileX(), f.getTileY() };
        }

        public void ForceOfferEatHeldItem()
        {
            //should work even for non-edibles
            Game1.player.faceDirection(2);
            Game1.player.isEating = true;
            Game1.player.itemToEat = Game1.player.ActiveObject;
            Game1.player.FarmerSprite.setCurrentSingleAnimation(304);
            Game1.currentLocation.createQuestionDialogue((Game1.objectInformation[Game1.player.ActiveObject.ParentSheetIndex].Split('/').Length > 6 && Game1.objectInformation[Game1.player.ActiveObject.ParentSheetIndex].Split('/')[6].Equals("drink")) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3159", Game1.player.ActiveObject.DisplayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3160", Game1.player.ActiveObject.DisplayName), Game1.currentLocation.createYesNoResponses(), "Eat");
        }


        public int[] GetFacingTileCoordinate()
        {
            var f = Game1.player;
            int target_x = f.getTileX();
            int target_y = f.getTileY();
            int d = f.FacingDirection;
            switch (d)
            {
                case 0: target_y -= 1; break; //up
                case 1: target_x += 1; break; //right
                case 2: target_y += 1; break; //down
                case 3: target_x -= 1; break; //left
            }
            return new int[] { target_x, target_y };
        }
    }
}
