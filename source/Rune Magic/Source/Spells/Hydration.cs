/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using SpaceCore;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace RuneMagic.Source.Spells
{
    public class Hydration : Spell
    {
        public Hydration() : base(School.Conjuration)
        {
            Description += "Water a tile at the cursor.";
            Level = 1;
        }

        public override bool Cast()
        {
            var tool = new WateringCan();
            var potency = 1 + (Skill.Level - Level) / 4;
            tool.DoFunction(Game1.currentLocation, (int)Game1.currentCursorTile.X * Game1.tileSize, (int)Game1.currentCursorTile.Y * Game1.tileSize, potency, Game1.player);
            return base.Cast();
        }
    }
}