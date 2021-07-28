/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Linq;

namespace SkillPrestige.Bonuses
{
    /// <summary>
    /// Represents a bonus in this mod, which are post-all-professions-prestiged effects the player can purchase.
    /// </summary>
    public class Bonus
    {
        public string BonusTypeCode { get; set; }

        public BonusType Type
        {
            get { return BonusType.AllBonusTypes.Single(x => x.Code == BonusTypeCode); }
        }

        public int Level { get; set; }

        public void ApplyEffect()
        {
            Type.ApplyEffect.Invoke(Level);
        }
    }
}
