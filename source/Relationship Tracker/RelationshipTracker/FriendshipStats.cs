using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
//using SFarmer = StardewValley.Farmer;
using StardewValley.Characters;
//using SGame = StardewValley.Game1;
using DatableType = RelationshipTracker.ModConfig.DatableType;

namespace RelationshipTracker
{
    internal enum Eligibility
    {
        Ineligible,
        Bachelor,
        Bechelorette
    }

    internal class FriendshipStats
    {
        // Contants
        private const int PointsPerLvl = 250;
        private const int MaxPoints = 2500;

        // Instance Variables
        public FriendshipStatus Status;
        public string Name;
        public int Level;
        public int ToNextLevel;
        public int GiftsThisWeek;
        public Icons.Portrait Portrait;
        //private ModConfig.DatableType DatingType;

        // Methods
        public FriendshipStats(Farmer player, NPC npc, Friendship friendship, DatableType datableType)
        {
        if (npc.datable.Value && npc.Gender == (int)datableType)
            {
                Name = npc.displayName;
                Status = friendship.Status;
                int points = friendship.Points;
                if (points < 250)
                {
                    Level = 0;
                }
                else if (points >= MaxPoints)
                {
                    Level = 10;
                }
                else
                {
                    Level = points / PointsPerLvl;
                }

                ToNextLevel = 250 - (points % PointsPerLvl);
                GiftsThisWeek = friendship.GiftsThisWeek;
                this.Portrait = new Icons.Portrait(npc);

            }
        }
    }
}
