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
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.TargetGame;

namespace ArsVenefici.Framework.Spells.Shape
{
    public class AoE : AbstractShape
    {
        public AoE() : base(new SpellPartStats(SpellPartStatType.RANGE))
        {

        }

        public override string GetId()
        {
            return "aoe";
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, HitResult hit, int ticksUsed, int index, bool awardXp)
        {
            //Game1.showRedMessage("Invoking Spell Part " + GetId());
            //modEntry.Monitor.Log("Invoking Spell Part " + GetId(), StardewModdingAPI.LogLevel.Info);

            if (hit == null)
                return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);

            var helper = SpellHelper.Instance();
            float radius = helper.GetModifiedStat(1, new SpellPartStats(SpellPartStatType.RANGE), modifiers, spell, caster, hit, index);
            bool appliedToAtLeastOneEntity = false;

            Rectangle rectangle = new Rectangle((int)hit.GetLocation().X, (int)hit.GetLocation().Y, (int)(radius * 2), (int)(radius * 2));

            foreach (Character e in GameLocationUtils.GetCharacters(caster, rectangle, null))
            {
                if (helper.Invoke(modEntry, spell, caster, gameLocation, new CharacterHitResult(e), ticksUsed, index, awardXp) == new SpellCastResult(SpellCastResultType.SUCCESS))
                {
                    appliedToAtLeastOneEntity = true;
                }
            }

            if (appliedToAtLeastOneEntity) 
                return new SpellCastResult(SpellCastResultType.SUCCESS);

            TilePos pos  = new TilePos(hit.GetLocation());

            //TilePos tilePos = new TilePos(pos); 

            //int rad = (int)Math.Round(radius);
            int rad = (int)radius;
            //int rad = 2;

            //modEntry.Monitor.Log("Tool Location Tile Position: " + pos.ToString(), StardewModdingAPI.LogLevel.Info);
            //modEntry.Monitor.Log("rad: " + rad.ToString(), StardewModdingAPI.LogLevel.Info);

            //for (int x = -rad; x <= rad; x++)
            //{
            //    for (int y = -rad; y <= rad; y++)
            //    {
            //        if (hit.GetHitResultType() == HitResult.HitResultType.TERRAIN_FEATURE)
            //        {
            //            //int offset = ((TerrainFeatureHitResult)hit).GetDirection().getAxisDirection() == Direction.AxisDirection.NEGATIVE ? 0 : -1;

            //            //BlockPos lookPos = switch (((BlockHitResult) hit).getDirection().getAxis()) {
            //            //    case X -> pos.offset(offset, y, z);
            //            //    case Y -> pos.offset(x, offset, z);
            //            //    case Z -> pos.offset(x, y, offset);
            //            //};

            //            //TilePos newTilePos = new TilePos(pos.GetTilePosX() + x, pos.GetTilePosY() + y);
            //            //TilePos newTilePos = new TilePos(Vector2.Add(pos.GetVector(), new Vector2(x,y)));
            //            TilePos newTilePos = new TilePos(Vector2.Add(pos, new Vector2(x, y)));

            //            modEntry.Monitor.Log(newTilePos.GetVector().ToString(), StardewModdingAPI.LogLevel.Info);

            //            //modEntry.Monitor.Log(x.ToString(), StardewModdingAPI.LogLevel.Info);

            //            helper.Invoke(modEntry, spell, caster, gameLocation, new TerrainFeatureHitResult(hit.GetLocation(), ((TerrainFeatureHitResult)hit).GetDirection(), newTilePos, ((TerrainFeatureHitResult)hit).IsInside()), ticksUsed, index, awardXp);
            //        }
            //    }
            //}

            for (int x = (int)(pos.GetVector().X - rad); x <= pos.GetVector().X + rad; ++x)
            {
                for (int y = (int)(pos.GetVector().Y - rad); y <= pos.GetVector().Y + rad; ++y)
                {
                    if (hit.GetHitResultType() == HitResult.HitResultType.TERRAIN_FEATURE)
                    {
                        TilePos newTilePos = new TilePos(x, y);

                        helper.Invoke(modEntry, spell, caster, gameLocation, new TerrainFeatureHitResult(hit.GetLocation(), ((TerrainFeatureHitResult)hit).GetDirection(), newTilePos, ((TerrainFeatureHitResult)hit).IsInside()), ticksUsed, index, awardXp);
                    }
                }
            }

            return new SpellCastResult(SpellCastResultType.SUCCESS);
        }

        public override bool IsEndShape()
        {
            return true;
        }

        public override bool NeedsPrecedingShape()
        {
            return true;
        }

        public override int ManaCost()
        {
            return 2;
        }
    }
}
