/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalEffects
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
            if (!Game1.player.friendshipData.ContainsKey(NPC)) return FriendshipStatus.Friendly;
            return Game1.player.friendshipData[NPC].Status;
        }

        public void SetFriendshipStatus(string NPC, FriendshipStatus status)
        {
            if (status == GetFriendshipStatus(NPC)) return;
            if (!Game1.player.friendshipData.ContainsKey(NPC)) Game1.player.friendshipData[NPC] = new Friendship(0);
            Game1.player.friendshipData[NPC].Status = status;
        }

        public void SetFriendshipPoints(string NPC, int points)
        {
            Farmer f2 = Game1.player;
            if (!f2.friendshipData.ContainsKey(NPC)) f2.friendshipData[NPC] = new Friendship(points);
            else f2.friendshipData[NPC].Points = points;
        }

        public bool HasItem(int id, int count = 1)
        {
            return Game1.player.hasItemInInventory(id, count);
        }

        public void RemoveItem(int itemId, int count = 1)
        {
            Game1.player.removeItemsFromInventory(itemId, count);
        }

        public int[] GetStandingTileCoordinate()
        {
            var f = Game1.player;
            return new int[] { f.getTileX(), f.getTileY() };
        }

        public void ForceOfferEatInedibleHeldItem()
        {
            //should work even for non-edibles, bypassing the 'sickness'. still triggers the events.
            Game1.player.faceDirection(2);
            Game1.player.isEating = true;
            Game1.player.itemToEat = Game1.player.ActiveObject;
            Game1.player.FarmerSprite.setCurrentSingleAnimation(304);
            Game1.currentLocation.createQuestionDialogue((Game1.objectInformation[Game1.player.ActiveObject.ParentSheetIndex].Split('/').Length > 6 && Game1.objectInformation[Game1.player.ActiveObject.ParentSheetIndex].Split('/')[6].Equals("drink")) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3159", Game1.player.ActiveObject.DisplayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3160", Game1.player.ActiveObject.DisplayName), Game1.currentLocation.createYesNoResponses(), ForceFeed);
        }

        private void ForceFeed(Farmer who, string answer)
        {
            if (answer != "No")
            {
                Game1.player.isEating = false;
                var o = Game1.player.ActiveObject;
                Game1.player.itemToEat = o;
                Game1.player.mostRecentlyGrabbedItem = o;
                Game1.player.forceCanMove();
                Game1.player.completelyStopAnimatingOrDoingAction();
                Game1.player.FarmerSprite.animateOnce(216, 80f, 8);
                Game1.player.freezePause = 20000;
                Game1.player.CanMove = false;
                Game1.player.reduceActiveItemByOne();
            }
        }


        //scaled, minimum possible to maximum possible, 0f to 1f.
        public float GetLuckFactorFloat()
        {
            //Sasha Valeria Edits
            Random r = new Random();
            //-0.1 to 0.1
            float l = (float)(((r.NextDouble() * 2) - 1) * 0.1f);
            l += 0.1f; //0.0 to 0.2
            l *= 5f; //0.0 to 1.0
            l = (l / 2f) + (l / 2f * (Game1.player.LuckLevel / 3f)); //lucklevel is 0-3, this applies it as a float multiplier to 50% of your daily luck.
            l = Math.Max(0f, Math.Min(1f, l));
            return l;
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

