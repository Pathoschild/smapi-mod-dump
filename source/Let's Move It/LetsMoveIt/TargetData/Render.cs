/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Exblosis/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace LetsMoveIt.TargetData
{
    internal partial class Target
    {
        public void Render(SpriteBatch spriteBatch, GameLocation location, Vector2 tile)
        {
            try
            {
                BoundingBoxTile.Clear();
                if (TargetObject is ResourceClump resourceClump)
                {
                    if (TargetObject is GiantCrop giantCrop)
                    {
                        var data = giantCrop.GetData();
                        //Monitor.Log("Data: " + data.TileSize, LogLevel.Debug); // <<< debug >>>
                        for (int x_offset = 0; x_offset < data.TileSize.X; x_offset++)
                        {
                            for (int y_offset = 0; y_offset < data.TileSize.Y; y_offset++)
                            {
                                spriteBatch.Draw(Game1.mouseCursors, Mod1.LocalTile(tile, x_offset * 64, y_offset * 64), new Rectangle?(new Rectangle(194, 388, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                            }
                        }
                        Texture2D texture = Game1.content.Load<Texture2D>(data.Texture);
                        spriteBatch.Draw(texture, Mod1.LocalTile(tile, y: -64), new Rectangle(data.TexturePosition.X, data.TexturePosition.Y, 16 * data.TileSize.X, 16 * (data.TileSize.Y + 1)), Color.White * 0.6f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                    }
                    else
                    {
                        string textureName = resourceClump.textureName.Value;
                        Texture2D texture = (textureName != null) ? Game1.content.Load<Texture2D>(textureName) : Game1.objectSpriteSheet;
                        Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(texture, resourceClump.parentSheetIndex.Value, 16, 16);
                        sourceRect.Width = resourceClump.width.Value * 16;
                        sourceRect.Height = resourceClump.height.Value * 16;
                        spriteBatch.Draw(texture, Mod1.LocalTile(tile), sourceRect, Color.White * 0.6f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                    }
                    var rc = resourceClump.getBoundingBox();
                    for (int x_offset = 0; x_offset < rc.Width / 64; x_offset++)
                    {
                        for (int y_offset = 0; y_offset < rc.Height / 64; y_offset++)
                        {
                            BoundingBoxTile.Add(tile + new Vector2(x_offset, y_offset));
                        }
                    }
                }
                else if (TargetObject is TerrainFeature terrainFeature)
                {
                    var tf = terrainFeature.getBoundingBox();
                    for (int x_offset = 0; x_offset < tf.Width / 64; x_offset++)
                    {
                        BoundingBoxTile.Add(tile + new Vector2(x_offset, 0));
                        spriteBatch.Draw(Game1.mouseCursors, Mod1.LocalTile(tile, x_offset * 64), new Rectangle?(new Rectangle(194, 388, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                    }
                    if (TargetObject is Bush bush)
                    {
                        Texture2D texture = Game1.content.Load<Texture2D>("TileSheets\\bushes");
                        SpriteEffects flipped = bush.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                        int tileOffset = (bush.sourceRect.Height / 16 - 1) * -64;
                        spriteBatch.Draw(texture, Mod1.LocalTile(tile, y: tileOffset), bush.sourceRect.Value, Color.White * 0.6f, 0f, Vector2.Zero, 4f, flipped, 1);
                    }
                    else if (TargetObject is Flooring flooring)
                    {
                        Texture2D texture = flooring.GetTexture();
                        Point textureCorner = flooring.GetTextureCorner();
                        spriteBatch.Draw(texture, Mod1.LocalTile(tile), new Rectangle(textureCorner.X, textureCorner.Y, 16, 16), Color.White * 0.5f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                    }
                    else if (TargetObject is HoeDirt)
                    {
                        Texture2D texture = ((location.Name.Equals("Mountain") || location.Name.Equals("Mine") || (location is MineShaft mineShaft && mineShaft.shouldShowDarkHoeDirt()) || location is VolcanoDungeon) ? Game1.content.Load<Texture2D>("TerrainFeatures\\hoeDirtDark") : Game1.content.Load<Texture2D>("TerrainFeatures\\hoeDirt"));
                        if ((location.GetSeason() == Season.Winter && !location.SeedsIgnoreSeasonsHere() && location is not MineShaft) || (location is MineShaft mineShaft2 && mineShaft2.shouldUseSnowTextureHoeDirt()))
                        {
                            texture = Game1.content.Load<Texture2D>("TerrainFeatures\\hoeDirtSnow");
                        }
                        spriteBatch.Draw(texture, Mod1.LocalTile(tile), new Rectangle(0, 0, 16, 16), Color.White * 0.5f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                    }
                    else if (TargetObject is Grass grass)
                    {
                        Texture2D texture = grass.texture.Value;
                        int grassSourceOffset = grass.grassSourceOffset.Value;
                        spriteBatch.Draw(texture, Mod1.LocalTile(tile, y: -16), new Rectangle(0, grassSourceOffset, 15, 20), Color.White * 0.5f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1);
                    }
                    else if (TargetObject is FruitTree fruitTree)
                    {
                        Texture2D texture = fruitTree.texture;
                        SpriteEffects flipped = fruitTree.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                        int growthStage = fruitTree.growthStage.Value;
                        int spriteRowNumber = fruitTree.GetSpriteRowNumber();
                        int seasonIndexForLocation = Game1.GetSeasonIndexForLocation(location);
                        bool flag = fruitTree.IgnoresSeasonsHere();
                        if (fruitTree.stump.Value)
                        {
                            spriteBatch.Draw(texture, Mod1.LocalTile(tile, -64, -64), new Rectangle(8 * 48, spriteRowNumber * 5 * 16 + 48, 48, 32), Color.White * 0.5f, 0f, Vector2.Zero, 4f, flipped, 1);
                        }
                        else
                        {
                            spriteBatch.Draw(texture, Mod1.LocalTile(tile, -64, -256), new Rectangle(((flag ? 1 : seasonIndexForLocation) + System.Math.Min(growthStage, 4)) * 48, spriteRowNumber * 5 * 16, 48, 80), Color.White * 0.5f, 0f, Vector2.Zero, 4f, flipped, 1);
                        }
                    }
                    else if (TargetObject is Tree tree)
                    {
                        Texture2D texture = tree.texture.Value;
                        Rectangle treeTopSourceRect = new(0, 0, 48, 96);
                        Rectangle stumpSourceRect = new(32, 96, 16, 32);
                        SpriteEffects flipped = tree.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                        int growthStage = tree.growthStage.Value;
                        int seasonIndexForLocation = Game1.GetSeasonIndexForLocation(location);
                        if (tree.hasMoss.Value)
                        {
                            treeTopSourceRect.X += 96;
                            stumpSourceRect.X += 96;
                        }
                        if (tree.stump.Value)
                        {
                            spriteBatch.Draw(texture, Mod1.LocalTile(tile, y: -64), stumpSourceRect, Color.White * 0.5f, 0f, Vector2.Zero, 4f, flipped, 1);
                        }
                        else if (growthStage < 5)
                        {
                            Rectangle value = growthStage switch
                            {
                                0 => new Rectangle(32, 128, 16, 16),
                                1 => new Rectangle(0, 128, 16, 16),
                                2 => new Rectangle(16, 128, 16, 16),
                                _ => new Rectangle(0, 96, 16, 32),
                            };
                            spriteBatch.Draw(texture, Mod1.LocalTile(tile, y: growthStage >= 3 ? -64 : 0), value, Color.White * 0.5f, 0f, Vector2.Zero, 4f, flipped, 1);
                        }
                        else
                        {
                            spriteBatch.Draw(texture, Mod1.LocalTile(tile, y: -64), stumpSourceRect, Color.White * 0.5f, 0f, Vector2.Zero, 4f, flipped, 1);
                            spriteBatch.Draw(texture, Mod1.LocalTile(tile, -64, -320), treeTopSourceRect, Color.White * 0.5f, 0f, Vector2.Zero, 4f, flipped, 1);
                        }
                    }
                }
                else if (TargetObject is Crop crop)
                {
                    if (crop.whichForageCrop.Value == "2")
                    {
                        crop.draw(spriteBatch, tile, Color.White * 0.6f, 0f);
                    }
                    else
                    {
                        crop.drawWithOffset(spriteBatch, tile, Color.White * 0.6f, 0f, new Vector2(32));
                    }
                }
                else if (TargetObject is SObject sObject)
                {
                    sObject.draw(spriteBatch, (int)tile.X, (int)tile.Y, 0.6f);
                }
                else if (TargetObject is Character character)
                {
                    Rectangle box = character.GetBoundingBox();
                    if (TargetObject is Farmer farmer)
                    {
                        farmer.FarmerRenderer.draw(spriteBatch, farmer, farmer.FarmerSprite.CurrentFrame, new Vector2(Game1.getMouseX() - 32, Game1.getMouseY() - 128), box.Center.Y / 10000f, farmer.FacingDirection == 3);
                    }
                    else
                    {
                        bool flip = character.flip;
                        if (TargetObject is FarmAnimal)
                            flip = character.FacingDirection == 3;
                        character.Sprite.draw(spriteBatch, new Vector2(Game1.getMouseX() - 32, Game1.getMouseY()) + new Vector2(character.GetSpriteWidthForPositioning() * 4 / 2, box.Height / 2), box.Center.Y / 10000f, 0, character.ySourceRectOffset, Color.White, flip, 4f, 0f, true);
                    }
                }
                else if (TargetObject is Building building)
                {
                    for (int x_offset = 0; x_offset < building.tilesWide.Value; x_offset++)
                    {
                        for (int y_offset = 0; y_offset < building.tilesHigh.Value; y_offset++)
                        {
                            spriteBatch.Draw(Game1.mouseCursors, Mod1.LocalTile(tile, x_offset * 64, y_offset * 64) - TileOffset * 64, new Rectangle?(new Rectangle(194, 388, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
