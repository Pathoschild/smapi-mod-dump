/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;
using StardewValley.Menus;

namespace SkillPrestige
{
    /// <summary>
    /// Manages the necessary methods to handle a level up menu.
    /// </summary>
    public class LevelUpManager
    {
        /// <summary>
        /// Returns whether a given menu is the relevant level up menu.
        /// </summary>
        public Func<IClickableMenu, bool> IsMenu { get; set; } 

        /// <summary>
        /// Returns the skill being levelled up.
        /// </summary>
        public Func<Skill> GetSkill { get; set; }

        /// <summary>
        /// Returns the level that has been reached.
        /// </summary>
        public Func<int> GetLevel { get; set; }

        /// <summary>
        /// Returns the menu that will replace the original level up menu.
        /// </summary>
        public Func<Skill, int, IClickableMenu> CreateNewLevelUpMenu { get; set; }
    }
}
