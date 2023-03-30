/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework;
using RuneMagic.Source.Effects;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneMagic.Source.Spells
{
    public class Summoning : Spell
    {
        public Summoning() : base(School.Conjuration)
        {
            Description += "Summons a creature to fight for you."; Level = 3;
        }

        public override bool Cast()
        {
            //if (!RuneMagic.PlayerStats.ActiveEffects.OfType<Summon>().Any())
            //{
            //get the vector 2 of the tile the cursor is on and create a monster

            Effect = new Summon(this, new Monster("Green Slime", new Vector2(Game1.currentCursorTile.X * Game1.tileSize, Game1.currentCursorTile.Y * Game1.tileSize)));
            return base.Cast();
            //}
            //else
            //    return false;
        }
    }
}