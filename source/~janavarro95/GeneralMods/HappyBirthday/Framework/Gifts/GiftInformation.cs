/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework.Gifts
{
    /// <summary>
    /// Information regarding gifts given to the player.
    /// </summary>
    public class GiftInformation
    {


        public int minRequiredHearts;
        public int maxRequiredHearts;
        public string objectID;
        public int minAmount;
        public int maxAmount;

        public GiftInformation()
        {

        }

        public GiftInformation(GiftIDS.SDVObject ObjectID, int RequiredHearts, int MinAmount, int MaxAmount)
        {
            this.objectID = "StardewValley.Object." + Enum.GetName(typeof(GiftIDS.SDVObject), (int)ObjectID);
            this.minRequiredHearts = RequiredHearts;
            this.maxRequiredHearts = 20;
            this.minAmount = MinAmount;
            this.maxAmount = MaxAmount;
        }

        public GiftInformation(string ObjectID, int RequiredHearts, int MinAmount, int MaxAmount)
        {
            this.objectID = ObjectID;
            this.minRequiredHearts = RequiredHearts;
            this.maxRequiredHearts = 20;
            this.minAmount = MinAmount;
            this.maxAmount = MaxAmount;
        }

        public GiftInformation(string ObjectID, int RequiredHearts,int MaxRequiredHearts ,int MinAmount, int MaxAmount)
        {
            this.objectID = ObjectID;
            this.minRequiredHearts = RequiredHearts;
            this.maxRequiredHearts = MaxRequiredHearts;
            this.minAmount = MinAmount;
            this.maxAmount = MaxAmount;
        }


        public Item getOne()
        {
            Item I = HappyBirthdayModCore.Instance.giftManager.registeredGifts[this.objectID].getOne();
            if (this.minAmount != this.maxAmount)
            {
                I.Stack = StardewValley.Game1.random.Next(this.minAmount, this.maxAmount);
            }
            else
            {
                I.Stack = this.minAmount;
            }
            return I;
        }

        public virtual bool Equals(GiftInformation other)
        {
            return
                other.minRequiredHearts.Equals(this.minRequiredHearts) &&
                other.maxRequiredHearts.Equals(this.maxRequiredHearts) &&
                other.objectID.Equals(this.objectID) &&
                other.minAmount.Equals(this.minAmount) &&
                other.maxAmount.Equals(this.maxAmount);
        }
    }
}
