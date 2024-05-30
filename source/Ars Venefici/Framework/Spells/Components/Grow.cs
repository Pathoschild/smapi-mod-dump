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
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;

namespace ArsVenefici.Framework.Spells.Components
{
    public class Grow : AbstractComponent
    {
        public override string GetId()
        {
            return "grow";
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, CharacterHitResult target, int index, int ticksUsed)
        {
            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, TerrainFeatureHitResult target, int index, int ticksUsed)
        {
            TilePos tilePos = target.GetTilePos();
            Vector2 tile = tilePos.GetVector();

            if (gameLocation.terrainFeatures.TryGetValue(tile, out TerrainFeature terrainFeature))
            {
                switch (terrainFeature)
                {
                    case HoeDirt dirt:
                        this.GrowHoeDirt(dirt);
                        break;
                    case Bush bush:
                        if (bush.size.Value == Bush.greenTeaBush)
                        {
                            if (bush.getAge() < Bush.daysToMatureGreenTeaBush)
                            {
                                bush.datePlanted.Value = (int)(Game1.stats.DaysPlayed - Bush.daysToMatureGreenTeaBush);
                                bush.dayUpdate(); // update sprite, etc
                            }

                            if (bush.inBloom() && bush.tileSheetOffset.Value == 0)
                                bush.dayUpdate(); // grow tea leaves
                        }
                        break;
                    case FruitTree fruitTree:
                        if (fruitTree.daysUntilMature.Value > 0)
                        {
                            fruitTree.daysUntilMature.Value = Math.Max(0, fruitTree.daysUntilMature.Value - 7);
                            fruitTree.growthStage.Value = fruitTree.daysUntilMature.Value > 0 ? (fruitTree.daysUntilMature.Value > 7 ? (fruitTree.daysUntilMature.Value > 14 ? (fruitTree.daysUntilMature.Value > 21 ? 0 : 1) : 2) : 3) : 4;
                        }
                        else if (!fruitTree.stump.Value && fruitTree.growthStage.Value == 4 && (fruitTree.IsInSeasonHere() || gameLocation.Name == "Greenhouse"))
                            fruitTree.TryAddFruit();
                        //else
                        //{
                        //    for (int i = 0; i < 3; i++)
                        //    {
                        //        fruitTree.TryAddFruit();
                        //    }
                        //}

                        //if (!fruitTree.stump.Value)
                        //{
                        //    if (fruitTree.growthStage.Value < FruitTree.treeStage)
                        //    {
                        //        fruitTree.growthStage.Value = Tree.treeStage;
                        //        fruitTree.daysUntilMature.Value = 0;
                        //    }

                        //    if (fruitTree.IsInSeasonHere())
                        //        fruitTree.TryAddFruit();
                        //}

                        break;

                    case Tree tree:
                        if (tree.growthStage.Value < 5)
                            tree.growthStage.Value++;
                        break;
                }

                return new SpellCastResult(SpellCastResultType.SUCCESS);
            }

            if (gameLocation.objects.TryGetValue(tile, out StardewValley.Object obj))
            {
                if (obj is IndoorPot pot)
                    this.GrowHoeDirt(pot.hoeDirt.Value);

                return new SpellCastResult(SpellCastResultType.SUCCESS);
            }

            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override int ManaCost()
        {
            return 5;
        }

        private void GrowHoeDirt(HoeDirt dirt)
        {
            if (dirt?.crop is not null)
            {
                Crop crop = dirt.crop;
                crop.newDay(HoeDirt.watered);
            }
        }
    }
}
