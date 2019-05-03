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
        /// The type of the level up menu.
        /// </summary>
        public Type MenuType { get; set; } 

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
