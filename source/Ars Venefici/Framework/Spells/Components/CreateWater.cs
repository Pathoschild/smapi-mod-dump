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
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;
using static ArsVenefici.Framework.GUI.DragNDrop.ShapeGroupArea;
using static System.Reflection.Metadata.BlobBuilder;

namespace ArsVenefici.Framework.Spells.Components
{
    public class CreateWater : AbstractComponent
    {
        public override string GetId()
        {
            return "create_water";
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, CharacterHitResult target, int index, int ticksUsed)
        {
            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override SpellCastResult Invoke(ModEntry modEntry, ISpell spell, IEntity caster, GameLocation gameLocation, List<ISpellModifier> modifiers, TerrainFeatureHitResult target, int index, int ticksUsed)
        {
            //modEntry.Monitor.Log("Invoking Spell Part " + GetId(), StardewModdingAPI.LogLevel.Info);

            TilePos pos = target.GetTilePos();

            Vector2 tile = new Vector2(pos.GetTilePosX(), pos.GetTilePosY());

            gameLocation.terrainFeatures.TryGetValue(tile, out TerrainFeature feature);

            if (feature!= null && feature is HoeDirt dirt)
            {
                dirt.state.Value = HoeDirt.watered;

                gameLocation.temporarySprites.Add(new TemporaryAnimatedSprite(13, new Vector2(pos.GetTilePosX() * (float)Game1.tileSize, pos.GetTilePosY() * (float)Game1.tileSize), Color.White, 10, Game1.random.NextDouble() < 0.5, 70f, 0, Game1.tileSize, (float)((pos.GetTilePosY() * (double)Game1.tileSize + Game1.tileSize / 2) / 10000.0 - 0.00999999977648258))
                {
                    delayBeforeAnimationStart = 10
                });

                gameLocation.localSound("wateringCan", tile);

                return new SpellCastResult(SpellCastResultType.SUCCESS);

            }

            return new SpellCastResult(SpellCastResultType.EFFECT_FAILED);
        }

        public override int ManaCost()
        {
            return 5;
        }
    }
}
