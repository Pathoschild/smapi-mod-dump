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
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace RuneMagic.Source.Spells
{
    public class Breaking : Spell
    {
        public Breaking() : base(School.Conjuration)
        {
            Description += "Conjures a Pickaxe at the target location."; Level = 2;
        }

        public override bool Cast()
        {
            var target = Game1.currentCursorTile;
            var tool = new Pickaxe();
            var potency = 1 + (School.Level - Level) / 4;
            tool.DoFunction(Game1.currentLocation, (int)Game1.currentCursorTile.X * Game1.tileSize, (int)Game1.currentCursorTile.Y * Game1.tileSize, potency, Game1.player);
            return base.Cast();
        }
    }
}