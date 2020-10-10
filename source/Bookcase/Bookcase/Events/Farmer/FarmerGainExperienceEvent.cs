/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using StardewValley;

namespace Bookcase.Events {

    /// <summary>
    /// This event is fired when a farmer gains experience for a skill. 
    /// 
    /// This event CAN be canceled!
    /// </summary>
    public class FarmerGainExperienceEvent : FarmerEvent {

        /// <summary>
        /// The type of experience gained. This is the id of the skill gaining EXP.
        /// </summary>
        public int SkillType { get; set; }

        /// <summary>
        /// The amount of experience being gained.
        /// </summary>
        public int Amount { get; set; }

        public FarmerGainExperienceEvent(Farmer farmer, int type, int amount) : base(farmer) {

            this.SkillType = type;
            this.Amount = amount;
        }

        public override bool CanCancel() {

            return true;
        }
    }
}