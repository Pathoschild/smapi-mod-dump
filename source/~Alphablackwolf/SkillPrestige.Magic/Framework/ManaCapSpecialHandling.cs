/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using Magic;
using SkillPrestige.Professions;
using StardewValley;

namespace SkillPrestige.Magic.Framework
{
    /// <summary>Special handling for adding to the mana cap from a Magic profession.</summary>
    internal class ManaCapSpecialHandling : IProfessionSpecialHandling
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mana points to add.</summary>
        public readonly int Amount;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="amount">The mana points to add.</param>
        public ManaCapSpecialHandling(int amount)
        {
            this.Amount = amount;
        }

        /// <summary>Apply effects for the profession.</summary>
        public void ApplyEffect()
        {
            Game1.player.setMaxMana(Game1.player.getMaxMana() + this.Amount);
        }

        /// <summary>Remove effects for the profession.</summary>
        public void RemoveEffect()
        {
            Game1.player.setMaxMana(Game1.player.getMaxMana() - this.Amount);
        }
    }
}
