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
