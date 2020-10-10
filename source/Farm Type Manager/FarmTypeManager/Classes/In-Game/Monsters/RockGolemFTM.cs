/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using Microsoft.Xna.Framework.Graphics;

namespace FarmTypeManager.Monsters
{
    /// <summary>A subclass of Stardew's RockGolem class, adjusted for use by this mod.</summary>
    public class RockGolemFTM : RockGolem
    {
        /// <summary>Creates an instance of Stardew's RockGolem class (Stone Golem subtype), but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        public RockGolemFTM()
            : base()
        {
            
        }

        /// <summary>Creates an instance of Stardew's RockGolem class (Stone Golem subtype), but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        public RockGolemFTM(Vector2 position)
            : base(position)
        {
            //immediately set the golem to its "hiding" state, fixing a bug when spawned near players
            Sprite.currentFrame = 16;
            Sprite.loop = false;
            Sprite.UpdateSourceRect();
        }

        /// <summary>Creates an instance of Stardew's RockGolem class (Wilderness Golem subtype), but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="difficultyMod">A number that affects the stats of this monster. This is normally the value of "Game1.player.CombatLevel".</param>
        public RockGolemFTM(Vector2 position, int difficultyMod)
            : base(position, difficultyMod)
        {
            //immediately set the golem to its "hiding" state, fixing a bug when spawned near players
            Sprite.currentFrame = 16;
            Sprite.loop = false;
            Sprite.UpdateSourceRect();
        }
    }
}
