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
using StardewValley;

namespace SkillPrestige.Professions
{
    /// <summary>Special handling for the defender profession, which adds 25 to the player's maximum health.</summary>
    internal class DefenderSpecialHandling : IProfessionSpecialHandling
    {
        /*********
        ** Public methods
        *********/
        public void ApplyEffect()
        {
            Logger.LogInformation("Applying defender effect.");
            Game1.player.maxHealth += 25;
        }

        public void RemoveEffect()
        {
            Logger.LogInformation("Removing defender effect.");
            Game1.player.maxHealth -= 25;
        }
    }
}
