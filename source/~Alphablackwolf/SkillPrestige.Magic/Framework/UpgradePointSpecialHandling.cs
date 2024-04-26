/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using SkillPrestige.Professions;

namespace SkillPrestige.Magic.Framework
{
    /// <summary>Special handling for adding to the spell points from a Magic profession.</summary>
    internal class UpgradePointSpecialHandling : IProfessionSpecialHandling
    {
        /*********
        ** Fields
        *********/
        /// <summary>The spell points to add.</summary>
        private readonly int Amount;

        /// <summary>Reduce the player's spell points by the given amount (or add spell points if the number is negative).</summary>
        private readonly Action<int> UseSpellPoints;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="amount">The spell points to add.</param>
        /// <param name="useSpellPoints">Reduce the player's spell points by the given amount (or add spell points if the number is negative).</param>
        public UpgradePointSpecialHandling(int amount, Action<int> useSpellPoints)
        {
            this.Amount = amount;
            this.UseSpellPoints = useSpellPoints;
        }

        /// <summary>Apply effects for the profession.</summary>
        public void ApplyEffect()
        {
            this.UseSpellPoints(-this.Amount);
        }

        /// <summary>Remove effects for the profession.</summary>
        public void RemoveEffect()
        {
            this.UseSpellPoints(this.Amount);
        }
    }
}
