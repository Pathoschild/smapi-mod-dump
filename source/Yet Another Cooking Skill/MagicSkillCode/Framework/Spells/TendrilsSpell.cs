/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System.Linq;
using MagicSkillCode.Framework.Schools;
using MagicSkillCode.Framework.Spells.Effects;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewValley;
using StardewValley.Monsters;
using MagicSkillCode.Core;

namespace MagicSkillCode.Framework.Spells
{
    // TODO: Change into trap?
    public class TendrilsSpell : Spell
    {
        /*********
        ** Public methods
        *********/
        public TendrilsSpell()
            : base(SchoolId.Nature, "tendrils") { }

        public override int GetManaCost(Farmer player, int level)
        {
            return 10;
        }

        public override int GetMaxCastingLevel()
        {
            return 1;
        }

        public override IActiveEffect OnCast(Farmer player, int level, int targetX, int targetY)
        {
            TendrilGroup tendrils = new TendrilGroup();
            foreach (var npc in player.currentLocation.characters)
            {
                if (npc is Monster mob)
                {
                    float rad = Game1.tileSize;
                    int dur = 11 * 60;
                    if (Vector2.Distance(mob.position.Value, new Vector2(targetX, targetY)) <= rad)
                    {
                        tendrils.Add(new Tendril(mob, new Vector2(targetX, targetY), rad, dur));
                        player.AddCustomSkillExperience(Magic.Skill, 3);
                    }
                }
            }

            return tendrils.Any()
                ? tendrils
                : null;
        }
    }
}
