/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;
using STree = StardewValley.TerrainFeatures.Tree;

namespace CropTransplantMod
{
    public class HeldIndoorPot : IndoorPot
    {
        private readonly NetRef<TerrainFeature> _tree = new NetRef<TerrainFeature>();

        public TerrainFeature Tree
        {
            get => _tree.Value;
            set => _tree.Value = value;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddFields(this._tree);
        }

        public HeldIndoorPot():base()
        {
        }

        public HeldIndoorPot(Vector2 tileLocation) : base(tileLocation)
        {
        }

        public bool IsHoldingSomething()
        {
            return this.hoeDirt.Value.crop != null || this.Tree != null || this.bush.Value != null;
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            if ((this.hoeDirt.Value.crop != null && !DataLoader.ModConfig.EnablePlacementOfCropsOutsideOutOfTheFarm && !location.IsFarm && !location.CanPlantSeedsHere(this.hoeDirt.Value.crop.netSeedIndex.Value, x / 64, y / 64) && location.IsOutdoors))
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13919"));
                return false;
            }
            bool result = base.placementAction(location, x, y, who);
            Vector2 placedPotLocation = new Vector2((float)(x / 64), (float)(y / 64));
            if (location.objects[placedPotLocation] is IndoorPot pot)
            {
                if (this.hoeDirt.Value.crop != null)
                {
                    pot.hoeDirt.Value.crop = this.hoeDirt.Value.crop;
                    pot.hoeDirt.Value.fertilizer.Value = this.hoeDirt.Value.fertilizer.Value;
                    TransplantController.ShakeCrop(pot.hoeDirt.Value, placedPotLocation);
                }
                else if (this.bush.Value != null)
                {
                    DataLoader.Helper.Reflection.GetField<NetBool>(pot, "bushLoadDirty").GetValue().Value = false;
                    pot.bush.Value = TransplantController.PrepareBushForPlacement(this.bush.Value, placedPotLocation);
                }
                TransplantOverrides.CleanHeldIndoorPot();
            }
            return result;
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            Vector2 vector2 = this.getScale() * 4f;
            Vector2 local = objectPosition;
            var x = local.X / 64;
            var y = (local.Y + 64) / 64;
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, objectPosition, new Rectangle?(Object.getSourceRectForBigCraftable(f.ActiveObject.ParentSheetIndex)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f));
            if (this.hoeDirt.Value.fertilizer.Value != 0)
            {
                int num = 0;
                switch (this.hoeDirt.Value.fertilizer.Value)
                {
                    case 369:
                        num = 1;
                        break;
                    case 370:
                        num = 2;
                        break;
                    case 371:
                        num = 3;
                        break;
                    case 465:
                        num = 4;
                        break;
                    case 466:
                        num = 5;
                        break;
                }
                spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)((double)objectPosition.X + 4.0), (float)(objectPosition.Y + 52)), new Rectangle?(new Rectangle(173 + num / 2 * 16, 466 + num % 2 * 16, 13, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f)+0.0001f);
            }

            if (this.Tree is Tree tree)
            {
                drawTree(tree, spriteBatch, objectPosition, f);
            }
            else if (this.Tree is FruitTree fruitTreetree)
            {
                drawFruitTree(fruitTreetree, spriteBatch, objectPosition, f);
            }

            if (this.hoeDirt.Value.crop != null)
            {
                DrawWithOffset(this.hoeDirt.Value.crop, spriteBatch, objectPosition, this.hoeDirt.Value.state.Value != 1 || this.hoeDirt.Value.crop.currentPhase.Value != 0 || this.hoeDirt.Value.crop.raisedSeeds.Value ? Color.White : new Color(180, 100, 200) * 1f, this.hoeDirt.Value.getShakeRotation(), new Vector2(32f, 72), f);
            }

            if (base.heldObject.Value != null)
            {
                base.heldObject.Value.draw(spriteBatch, (int)x * 64, (int)(y * 64 - 48), (objectPosition.Y + 0.66f) * 64f / 10000f + (float)x * 1E-05f, 1f);
            }

            if (this.bush.Value != null)
            {
                drawTeaBush(this.bush.Value, spriteBatch, objectPosition, f);
            }
        }

        private void DrawWithOffset(Crop crop, SpriteBatch b, Vector2 tileLocation, Color toTint, float rotation, Vector2 offset, Farmer f)
        {
            if (crop.forageCrop.Value)
            {
                b.Draw(Game1.mouseCursors,  offset + tileLocation, new Rectangle?(new Rectangle((int)((double)tileLocation.X * 51.0 + (double)tileLocation.Y * 77.0) % 3 * 16, 128 + crop.whichForageCrop.Value * 16, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0002f);
            }
            else
            {
                Rectangle? sourceRect = null;
                if (crop.currentPhase.Value > 0)
                {
                    sourceRect = GetSourceRect(crop, (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
                }
                else
                {
                    sourceRect = GetSourceRect(crop, (int)base.TileLocation.X * 7 + (int)base.TileLocation.Y * 11);
                }
                b.Draw(Game1.cropSpriteSheet,  offset + tileLocation, sourceRect, toTint, rotation, new Vector2(8f, 24f), 4f, crop.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0002f);
                if (crop.tintColor.Value.Equals((object)Color.White) || crop.currentPhase.Value != crop.phaseDays.Count - 1 || crop.dead.Value)
                    return;
                b.Draw(Game1.cropSpriteSheet, offset + tileLocation, new Rectangle?(new Rectangle((crop.fullyGrown.Value ? (crop.dayOfCurrentPhase.Value <= 0 ? 6 : 7) : crop.currentPhase.Value + 1 + 1) * 16 + (crop.rowInSpriteSheet.Value % 2 != 0 ? 128 : 0), crop.rowInSpriteSheet.Value / 2 * 16 * 2, 16, 32)), crop.tintColor.Value, rotation, new Vector2(8f, 24f), 4f, crop.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0003f);
            }
        }

        private Rectangle GetSourceRect(Crop crop, int number)
        {
            if (crop.dead.Value)
                return new Rectangle(192 + number % 4 * 16, 384, 16, 32);
            return new Rectangle(Math.Min(240, (crop.fullyGrown.Value ? (crop.dayOfCurrentPhase.Value <= 0 ? 6 : 7) : (int)(crop.phaseToShow.Value != -1 ? crop.phaseToShow.Value : crop.currentPhase.Value) + ((int)(crop.phaseToShow.Value != -1 ? crop.phaseToShow.Value : crop.currentPhase.Value) != 0 || number % 2 != 0 ? 0 : -1) + 1) * 16 + (crop.rowInSpriteSheet.Value % 2 != 0 ? 128 : 0)), crop.rowInSpriteSheet.Value / 2 * 16 * 2, 16, 32);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            //Empty to avoid drawing the HoeDirty out of place.
        }

        public void drawTree(Tree treeToDraw, SpriteBatch spriteBatch, Vector2 tileLocation, Farmer f)
        {
            Microsoft.Xna.Framework.Rectangle boundingBox;
            Texture2D texture = DataLoader.Helper.Reflection.GetField<Lazy<Texture2D>>(treeToDraw, "texture").GetValue().Value;
            List<Leaf> leaves = DataLoader.Helper.Reflection.GetField<List<Leaf>>(treeToDraw, "leaves").GetValue();
            float shakeRotation = DataLoader.Helper.Reflection.GetField<float>(treeToDraw, "shakeRotation").GetValue();
            if (treeToDraw.growthStage.Value < 5)
            {
                Microsoft.Xna.Framework.Rectangle rectangle = Microsoft.Xna.Framework.Rectangle.Empty;
                switch (treeToDraw.growthStage.Value)
                {
                    case 0:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(32, 128, 16, 16);
                        break;
                    case 1:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(0, 128, 16, 16);
                        break;
                    case 2:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(16, 128, 16, 16);
                        break;
                    default:
                        rectangle = new Microsoft.Xna.Framework.Rectangle(0, 96, 16, 32);
                        break;
                }
                SpriteBatch spriteBatch1 = spriteBatch;
                Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(rectangle);
                Color white = Color.White;
                Vector2 origin = new Vector2(8f, treeToDraw.growthStage.Value >= 3 ? 32f : 16f);
                double num1 = 4.0;
                int num2 = treeToDraw.flipped.Value ? 1 : 0;
                spriteBatch1.Draw(texture, tileLocation + new Vector2(32,96), sourceRectangle, white, (float)shakeRotation, origin, (float)num1, (SpriteEffects)num2, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0002f);
            }
            else
            {
                if (!treeToDraw.stump.Value)
                {
                    spriteBatch.Draw(Game1.mouseCursors, tileLocation + new Vector2(-51, 150), new Microsoft.Xna.Framework.Rectangle?(STree.shadowSourceRect), Color.White * (1.570796f - Math.Abs(shakeRotation)), 0.0f, Vector2.Zero, 4f, treeToDraw.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
                    SpriteBatch spriteBatch1 = spriteBatch;
                    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(treeToDraw.treeTopSourceRect);
                    Color color = Color.White;
                    Vector2 origin = new Vector2(24f, 96f);
                    double num1 = 4.0;
                    int num2 = treeToDraw.flipped.Value ? 1 : 0;
                    boundingBox = this.getBoundingBox(tileLocation);
                    double num3 = Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0003f;
                    spriteBatch1.Draw(texture, tileLocation + new Vector2(32, 96), sourceRectangle, color, (float)shakeRotation, origin, (float)num1, (SpriteEffects)num2, (float)num3);
                }
                if (treeToDraw.health.Value > -99.0)
                {
                    SpriteBatch spriteBatch1 = spriteBatch;
                    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(STree.stumpSourceRect);
                    Color color = Color.White;
                    double num1 = 0.0;
                    Vector2 zero = Vector2.Zero;
                    double num2 = 4.0;
                    int num3 = treeToDraw.flipped.Value ? 1 : 0;
                    boundingBox = this.getBoundingBox(tileLocation);
                    double num4 = Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0002f;
                    spriteBatch1.Draw(texture, tileLocation + new Vector2(0, -32), sourceRectangle, color, (float)num1, zero, (float)num2, (SpriteEffects)num3, (float)num4);
                }
                if (treeToDraw.stump.Value && treeToDraw.health.Value < 4.0 && treeToDraw.health.Value > -99.0)
                {
                    SpriteBatch spriteBatch1 = spriteBatch;
                    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Math.Min(2, (int)(3.0 - treeToDraw.health.Value)) * 16, 144, 16, 16));
                    Color color = Color.White;
                    double num1 = 0.0;
                    Vector2 zero = Vector2.Zero;
                    double num2 = 4.0;
                    int num3 = treeToDraw.flipped.Value ? 1 : 0;
                    boundingBox = this.getBoundingBox(tileLocation);
                    double num4 = Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0004f;
                    spriteBatch1.Draw(texture, tileLocation + new Vector2(0, 32), sourceRectangle, color, (float)num1, zero, (float)num2, (SpriteEffects)num3, (float)num4);
                }
            }
            foreach (Leaf leaf in leaves)
            {
                SpriteBatch spriteBatch1 = spriteBatch;
                Vector2 local = Game1.GlobalToLocal(Game1.viewport, leaf.position);
                Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16 + leaf.type % 2 * 8, 112 + leaf.type / 2 * 8, 8, 8));
                Color white = Color.White;
                double rotation = (double)leaf.rotation;
                Vector2 zero = Vector2.Zero;
                double num1 = 4.0;
                int num2 = 0;
                boundingBox = this.getBoundingBox(tileLocation);
                double num3 = Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0005f;
                spriteBatch1.Draw(texture, local, sourceRectangle, white, (float)rotation, zero, (float)num1, (SpriteEffects)num2, (float)num3);
            }
        }

        public void drawFruitTree(FruitTree treeToDraw, SpriteBatch spriteBatch, Vector2 tileLocation, Farmer f)
        {
            List<Leaf> leaves = DataLoader.Helper.Reflection.GetField<List<Leaf>>(treeToDraw, "leaves").GetValue();
            float shakeRotation = DataLoader.Helper.Reflection.GetField<float>(treeToDraw, "shakeRotation").GetValue();
            Rectangle boundingBox;
            if (treeToDraw.growthStage.Value < 4)
            {
                //Vector2 vector2 = new Vector2((float)Math.Max(-8.0, Math.Min(64.0, Math.Sin((double)tileLocation.X * 200.0 / (2.0 * Math.PI)) * -16.0)), (float)Math.Max(-8.0, Math.Min(64.0, Math.Sin((double)tileLocation.X * 200.0 / (2.0 * Math.PI)) * -16.0))) / 2f;
                Rectangle rectangle = Rectangle.Empty;
                switch (treeToDraw.growthStage.Value)
                {
                    case 0:
                        rectangle = new Rectangle(0, treeToDraw.treeType.Value * 5 * 16, 48, 80);
                        break;
                    case 1:
                        rectangle = new Rectangle(48, treeToDraw.treeType.Value * 5 * 16, 48, 80);
                        break;
                    case 2:
                        rectangle = new Rectangle(96, treeToDraw.treeType.Value * 5 * 16, 48, 80);
                        break;
                    default:
                        rectangle = new Rectangle(144, treeToDraw.treeType.Value * 5 * 16, 48, 80);
                        break;
                }
                SpriteBatch spriteBatch1 = spriteBatch;
                Texture2D texture = FruitTree.texture;
                //Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * 64.0 + 32.0) + vector2.X, (float)((double)tileLocation.Y * 64.0 - (double)rectangle.Height + 128.0) + vector2.Y));
                Rectangle? sourceRectangle = new Rectangle?(rectangle);
                Color white = Color.White;
                Vector2 origin = new Vector2(24f, 80f);
                double num1 = 4.0;
                int num2 = treeToDraw.flipped.Value  ? 1 : 0;
                boundingBox = this.getBoundingBox(tileLocation);
                double num3 = Math.Max(0.0f, (float) (f.getStandingY() + 2) / 10000f) + 0.0002f;
                spriteBatch1.Draw(texture, tileLocation + new Vector2(32, 96), sourceRectangle, white, (float)shakeRotation, origin, (float)num1, (SpriteEffects)num2, (float)num3);
            }
            else
            {
                if (!treeToDraw.stump.Value)
                {
                    spriteBatch.Draw(Game1.mouseCursors, tileLocation + new Vector2(-51, 150), new Rectangle?(STree.shadowSourceRect), Color.White * (1.570796f - Math.Abs(shakeRotation)), 0.0f, Vector2.Zero, 4f, treeToDraw.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
                    SpriteBatch spriteBatch1 = spriteBatch;
                    Texture2D texture = FruitTree.texture;
                    Rectangle? sourceRectangle = new Rectangle?(new Rectangle((12 + (treeToDraw.GreenHouseTree ? 1 : Utility.getSeasonNumber(Game1.currentSeason)) * 3) * 16, treeToDraw.treeType.Value * 5 * 16, 48, 64));
                    Color color = treeToDraw.struckByLightningCountdown.Value > 0 ? Color.Gray : Color.White;
                    Vector2 origin = new Vector2(24f, 80f);
                    double num1 = 4.0;
                    int num2 = treeToDraw.flipped.Value  ? 1 : 0;
                    boundingBox = this.getBoundingBox(tileLocation);
                    double num3 = Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0003f;
                    spriteBatch1.Draw(texture, tileLocation + new Vector2(32, 96), sourceRectangle, color, (float)shakeRotation, origin, (float)num1, (SpriteEffects)num2, (float)num3);
                }
                if (treeToDraw.health.Value > -99.0)
                {
                    SpriteBatch spriteBatch1 = spriteBatch;
                    Texture2D texture = FruitTree.texture;
                    Rectangle? sourceRectangle = new Rectangle?(new Rectangle(384, treeToDraw.treeType.Value * 5 * 16 + 48, 48, 32));
                    Color color = treeToDraw.struckByLightningCountdown.Value > 0 ? Color.Gray : Color.White;
                    double num1 = 0.0;
                    Vector2 origin = new Vector2(24f, 32f);
                    double num2 = 4.0;
                    int num3 = treeToDraw.flipped.Value  ? 1 : 0;
                    double num4;
                    boundingBox = this.getBoundingBox(tileLocation);
                    num4 = Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0002f;
                    spriteBatch1.Draw(texture, tileLocation + new Vector2(32, 96), sourceRectangle, color, (float)num1, origin, (float)num2, (SpriteEffects)num3, (float)num4);
                }
            }
            foreach (Leaf leaf in leaves)
            {
                SpriteBatch spriteBatch1 = spriteBatch;
                Texture2D texture = FruitTree.texture;
                Vector2 local = Game1.GlobalToLocal(Game1.viewport, leaf.position);
                Rectangle? sourceRectangle = new Rectangle?(new Rectangle((24 + Utility.getSeasonNumber(Game1.currentSeason)) * 16, treeToDraw.treeType.Value * 5 * 16, 8, 8));
                Color white = Color.White;
                double rotation = (double)leaf.rotation;
                Vector2 zero = Vector2.Zero;
                double num1 = 4.0;
                int num2 = 0;
                boundingBox = this.getBoundingBox(tileLocation);
                double num3 = Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0004f;
                spriteBatch1.Draw(texture, local, sourceRectangle, white, (float)rotation, zero, (float)num1, (SpriteEffects)num2, (float)num3);
            }
        }

        public void drawTeaBush(Bush bushToDraw, SpriteBatch spriteBatch, Vector2 tileLocation, Farmer f)
        {
            float shakeRotation = DataLoader.Helper.Reflection.GetField<float>(bushToDraw, "shakeRotation").GetValue();
            Rectangle sourceRect = DataLoader.Helper.Reflection.GetField<NetRectangle>(bushToDraw, "sourceRect").GetValue().Value;
            Vector2 plantLocation = tileLocation + new Vector2(32f, 104f);

            Color color = Color.White * 1f;
            SpriteEffects spriteEffects = bushToDraw.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float layerDepth = Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0002f;
            spriteBatch.Draw(Bush.texture.Value, plantLocation, sourceRect, color, shakeRotation, new Vector2(8f, 32f), 4f, spriteEffects, layerDepth);
        }
    }
}
