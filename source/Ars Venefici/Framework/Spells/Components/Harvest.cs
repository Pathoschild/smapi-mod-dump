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
using StardewValley.GameData.Crops;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells.Components
{
    public class Harvest : AbstractComponent
    {
        public override string GetId()
        {
            return "harvest";
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, CharacterHitResult target, int index, int ticksUsed)
        {
            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, TerrainFeatureHitResult target, int index, int ticksUsed)
        {
            TilePos blockPos = target.GetTilePos();
            Vector2 tile = blockPos.GetVector();

            if (gameLocation.terrainFeatures.TryGetValue(tile, out TerrainFeature terrainFeature))
            {
                switch (terrainFeature)
                {
                    case HoeDirt dirt:
                        this.HarvestHoeDirt(dirt, tile);
                        break;
                }

                return new SpellCastResult(SpellCastResultType.SUCCESS);
            }

            if (gameLocation.objects.TryGetValue(tile, out StardewValley.Object obj))
            {
                if (obj is IndoorPot pot)
                    this.HarvestHoeDirt(pot.hoeDirt.Value, tile);

                return new SpellCastResult(SpellCastResultType.SUCCESS);
            }

            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        private void HarvestHoeDirt(HoeDirt dirt, Vector2 tile)
        {
            if (dirt?.crop is not null)
            {
                //dirt.performUseAction(tile);

                if (dirt.readyForHarvest())
                {
                    dirt.crop.harvest((int)tile.X, (int)tile.Y, dirt, null, true);

                    if(!dirt.crop.RegrowsAfterHarvest())
                        dirt.destroyCrop(true);
                }
            }
        }

        public override int ManaCost()
        {
            return 5;
        }
    }
}
