/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BananaFruit1492/BulkStaircases
**
*************************************************/

using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace BulkStaircases.Framework
{
    internal class ModConfig
    {
        private int _numberOfStaircasesToLeaveInStack = 0;
        /// <summary>Number of staircases left in the held stack of staircases.</summary>
        public int NumberOfStaircasesToLeaveInStack
        {
            get
            {
                return this._numberOfStaircasesToLeaveInStack;
            }
            set
            {
                if (value < 0)
                    this._numberOfStaircasesToLeaveInStack = 0;
                else
                    this._numberOfStaircasesToLeaveInStack = value;
            }
        }

        /// <summary>Whether to skip level 100 in skull cavern.</summary>
        public bool SkipLevel100SkullCavern { get; set; } = false;

        /// <summary>Whether to skip prehistoric floors.</summary>
        public bool SkipDinosaurLevels { get; set; } = false;

        /// <summary>Whether to skip levels with a treasure.</summary>
        public bool SkipTreasureLevels { get; set; } = false;

        /// <summary>Whether to skip quarry dungeon levels that may appear after having been to the quarry mine.</summary>
        public bool SkipQuarryDungeonLevels { get; set; } = false;

        /// <summary>Whether to skip slime infested levels.</summary>
        public bool SkipSlimeLevels { get; set; } = false;

        /// <summary>Whether to skip monster infested levels.</summary>
        public bool SkipMonsterLevels { get; set; } = false;

        /// <summary>Whether to skip mushroom levels.</summary>
        public bool SkipMushroomLevels { get; set; } = false;

        /// <summaryDon't skip level with the monsters given here if there are at least the given number of them.</summary>
        public Dictionary<string, int> MonsterFilters { get; set; } = new ();
        
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("LeftShift + C");
    }
}
