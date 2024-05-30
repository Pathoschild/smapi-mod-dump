/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Spells.Effects;
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using static StardewValley.Menus.CharacterCustomization;
using static StardewValley.Minigames.TargetGame;

namespace ArsVenefici.Framework.Spells.Shape
{
    public class Projectile : AbstractShape
    {
        public Projectile() 
                : base (new SpellPartStats(SpellPartStatType.BOUNCE), new SpellPartStats(SpellPartStatType.PIERCING), new SpellPartStats(SpellPartStatType.SPEED))
        {
        
        }

        public override string GetId()
        {
            return "projectile";
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation level, List<ISpellModifier> modifiers, HitResult hit, int ticksUsed, int index, bool awardXp)
        {
            object entity = caster.entity;

            Character character = null;

            if(entity is Character)
                character = (Character)entity;


            if(character != null)
            {
                var helper = SpellHelper.Instance();
                //HitResult hitResult = helper.traceProjectile(modEntry, caster, level, helper.GetModifiedStat(10.5f, new SpellPartStats(SpellPartStatType.RANGE), modifiers, spell, caster, hit, index), true, helper.GetModifiedStat(0, new SpellPartStats(SpellPartStatType.TARGET_NON_SOLID), modifiers, spell, caster, hit, index) > 0);

                Vector2 toLocation = new Vector2(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y);
                float dir = (float)-Math.Atan2(character.getStandingPosition().Y - toLocation.Y, toLocation.X - character.getStandingPosition().X);

                //var projectile = new SpellProjectile(modEntry, caster);

                float velocity = (int)helper.GetModifiedStat(4f + 3, new SpellPartStats(SpellPartStatType.SPEED), modifiers, spell, caster, hit, index);
                var projectile = new SpellProjectile(modEntry, caster, spell, dir, index, velocity);

                projectile.bouncesLeft.Value = (int)helper.GetModifiedStat(0, new SpellPartStats(SpellPartStatType.BOUNCE), modifiers, spell, caster, hit, index);
                projectile.piercesLeft.Value = (int)helper.GetModifiedStat(0, new SpellPartStats(SpellPartStatType.PIERCING), modifiers, spell, caster, hit, index);

                character.currentLocation.projectiles.Add(projectile);
            }

            return new SpellCastResult(SpellCastResultType.SUCCESS);
        }

        public override int ManaCost()
        {
            return 1;
        }
    }
}
