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
    /// <summary>Special handling for adding to the mana cap from a Magic profession.</summary>
    internal class ManaCapSpecialHandling : IProfessionSpecialHandling
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mana points to add.</summary>
        private readonly int Amount;

        /// <summary>Adjust the player's max mana points by the given amount.</summary>
        private readonly Action<int> AddMaxMana;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="amount">The mana points to add.</param>
        /// <param name="addMaxMana">Adjust the player's max mana points by the given amount.</param>
        public ManaCapSpecialHandling(int amount, Action<int> addMaxMana)
        {
            this.Amount = amount;
            this.AddMaxMana = addMaxMana;
        }

        /// <summary>Apply effects for the profession.</summary>
        public void ApplyEffect()
        {
            this.AddMaxMana(this.Amount);
        }

        /// <summary>Remove effects for the profession.</summary>
        public void RemoveEffect()
        {
            this.AddMaxMana(-this.Amount);
        }
    }
}
