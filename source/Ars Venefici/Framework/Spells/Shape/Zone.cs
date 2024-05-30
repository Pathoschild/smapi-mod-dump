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
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells.Shape
{
    public class Zone : AbstractShape
    {

        public Zone() : base(new SpellPartStats(SpellPartStatType.DURATION), new SpellPartStats(SpellPartStatType.RANGE))
        {

        }

        public override string GetId()
        {
            return "zone";
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, HitResult hit, int ticksUsed, int index, bool awardXp)
        {
            var helper = SpellHelper.Instance();

            Vector2 position = Vector2.One; 

            if (hit != null)
            {
                //position = Utils.ConvertToTilePos(Utility.clampToTile(hit.GetLocation()));
                position = hit.GetLocation();
            }
            else
            {
                position = Utils.AbsolutePosToTilePos(Utility.clampToTile(caster.GetPosition()));
            }

            float radius = helper.GetModifiedStat(1, new SpellPartStats(SpellPartStatType.RANGE), modifiers, spell, caster, hit, index);
            int duration = (int)(200 + helper.GetModifiedStat(100, new SpellPartStats(SpellPartStatType.DURATION), modifiers, spell, caster, hit, index));

            ZoneEffect zoneEffect = new ZoneEffect(modEntry, spell, position, radius, duration);
            zoneEffect.SetIndex(index);
            zoneEffect.SetOwner(caster);

            modEntry.ActiveEffects.Add(zoneEffect);

            return new SpellCastResult(SpellCastResultType.SUCCESS);
        }

        public override int ManaCost()
        {
            return 3;
        }

        public override bool IsEndShape()
        {
            return true;
        }

        public override bool NeedsPrecedingShape()
        {
            return true;
        }
    }
}
