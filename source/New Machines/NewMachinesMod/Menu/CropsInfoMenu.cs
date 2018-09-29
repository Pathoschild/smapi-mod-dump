using System;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod.Menu
{
    public sealed class CropsInfoMenu : GridToolTip
    {
        public override bool NeedDraw()
        {
            if (Game1.activeClickableMenu != null || !IsAltPressed()) return false;
            return GetHoveredCrop() != null;
        }

        public override void Draw()
        {
            Clear();
            var tuple = GetHoveredCrop();
            var dirt = tuple.Item1;
            var crop = tuple.Item2;

            var row = 0;

            if (crop != null)
            {
                var fruit = new Object(crop.indexOfHarvest, 1);
                RegisterCell(new TextMenuCell(row, 0, Aligment.Center, fruit.Name) { ColumnSpan = 3 });
                ++row;

                var daysLeft = GetDaysLeft(crop);
                if (daysLeft > 0)
                {
                    RegisterCell(new IconMenuCell(row, 0, Aligment.Center, Game1.mouseCursors, new Rectangle(434, 475, 9, 9), TileSize / 2));
                    RegisterCell(new TextMenuCell(row, 1, Aligment.VerticalCenter, $"{daysLeft}d"));
                    ++row;
                }
                RegisterCell(new ItemMenuCell(row, 0, Aligment.Center, fruit));
            }

            if (dirt.fertilizer != HoeDirt.noFertilizer)
            {
                RegisterCell(new ItemMenuCell(row, 1, Aligment.Left, new Object(dirt.fertilizer, 1)));
            }
            else
            {
                RegisterCell(new IconMenuCell(row, 1, Aligment.Left, Game1.objectSpriteSheet, TextureInfo.Default[TextureType.Items].GetSourceRect(HoeDirt.fertilizerLowQuality), TileSize, shadow: true));
            }

            RegisterCell(new IconMenuCell(row, 2, Aligment.Left | Aligment.VerticalCenter, Game1.toolSpriteSheet, new Rectangle(32, 225, 16, 16), TileSize, shadow: dirt.state != HoeDirt.watered));

            if (crop != null && crop.dead)
            {
                RegisterCell(new IconMenuCell(row, 3, Aligment.Left | Aligment.VerticalCenter, Game1.mouseCursors, new Rectangle(140, 428, 10, 10), TileSize / 2));
            }

            Recalculate();
            var x0 = MouseX - TileSize;
            var y0 = MouseY + TileSize;
            Draw(x0, y0);
        }

        private int GetDaysLeft(Crop crop)
        {
            var currentPhase = crop.currentPhase;
            var dayOfPhase = crop.dayOfCurrentPhase;
            var phases = crop.phaseDays;
            if (currentPhase == phases.Count - 1)
            {
                return crop.regrowAfterHarvest == -1 ? 0 : crop.dayOfCurrentPhase;
            }
            return phases[currentPhase] - dayOfPhase + phases.Skip(currentPhase).Sum() - Crop.finalPhaseLength;
        }

        private static bool IsAltPressed()
        {
            var state = Keyboard.GetState();
            return state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt);
        }

        private static Tuple<HoeDirt, Crop> GetHoveredCrop()
        {
            var key = new Vector2(NormalizedMouseX / Game1.tileSize, NormalizedMouseY / Game1.tileSize);

            TerrainFeature terrainFeature;
            if (!Game1.currentLocation.terrainFeatures.TryGetValue(key, out terrainFeature)) return null;

            var dirt = terrainFeature as HoeDirt;
            if (dirt == null) return null;
            return Tuple.Create(dirt, dirt.crop);
        }
    }
}
