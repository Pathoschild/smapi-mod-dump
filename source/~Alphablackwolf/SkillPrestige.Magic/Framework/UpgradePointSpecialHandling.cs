using Magic;
using SkillPrestige.Professions;
using StardewValley;

namespace SkillPrestige.Magic.Framework
{
    /// <summary>Special handling for adding to the spell points from a Magic profession.</summary>
    internal class UpgradePointSpecialHandling : IProfessionSpecialHandling
    {
        /*********
        ** Fields
        *********/
        /// <summary>The spell points to add.</summary>
        public readonly int Amount;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="amount">The mana points to add.</param>
        public UpgradePointSpecialHandling(int amount)
        {
            this.Amount = amount;
        }

        /// <summary>Apply effects for the profession.</summary>
        public void ApplyEffect()
        {
            Game1.player.useSpellPoints(-this.Amount, true);
        }

        /// <summary>Remove effects for the profession.</summary>
        public void RemoveEffect()
        {
            Game1.player.useSpellPoints(this.Amount, true);
        }
    }
}
