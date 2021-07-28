/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using SkillPrestige.Logging;
using SkillPrestige.SkillTypes;
using StardewValley;

namespace SkillPrestige
{
    /// <summary>
    /// A class to manage aspects of the player.
    /// </summary>
    public static class PlayerManager
    {
        private static int _originalMaxHealth = 100;

        public static void CorrectStats(Skill skillThatIsReset)
        {
            if (skillThatIsReset.Type != SkillType.Combat)
            {
                Logger.LogVerbose("Player Manager - no stats reset.");
            }
            else
            {
                Logger.LogVerbose($"Player Manager- Combat reset. Resetting max health to {_originalMaxHealth}.");
                Game1.player.maxHealth = _originalMaxHealth;
            }
            
        }

    }
}
