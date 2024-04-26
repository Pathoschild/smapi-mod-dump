/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using SkillPrestige.Logging;
using SkillPrestige.SkillTypes;
using StardewValley;

namespace SkillPrestige.Framework
{
    /// <summary>A class to manage aspects of the player.</summary>
    internal static class PlayerManager
    {
        /*********
        ** Fields
        *********/
        private const int OriginalMaxHealth = 100;


        /*********
        ** Public methods
        *********/
        public static void CorrectStats(Skill skillThatIsReset)
        {
            if (skillThatIsReset.Type != SkillType.Combat)
                Logger.LogVerbose("Player Manager - no stats reset.");
            else
            {
                Logger.LogVerbose($"Player Manager- Combat reset. Resetting max health to {OriginalMaxHealth}.");
                Game1.player.maxHealth = OriginalMaxHealth;
            }
        }
    }
}
