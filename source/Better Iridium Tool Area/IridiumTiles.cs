using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Graphics;

namespace BetterIridiumFarmTools
{
    public class IridiumTiles
    {
        public static List<Vector2> AFTiles(Vector2 tileLocation, int power, StardewValley.Farmer who)
        {
            ++power;
            List<Vector2> vector2List = new List<Vector2>();
            vector2List.Add(tileLocation);
            if (who.facingDirection == 0)
            {
                if (power >= 2)
                {
                    vector2List.Add(tileLocation + new Vector2(0.0f, -1f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, -2f));
                }
                if (power >= 3)
                {
                    vector2List.Add(tileLocation + new Vector2(0.0f, -3f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, -4f));
                }
                if (power >= 4)
                {
                    vector2List.RemoveAt(vector2List.Count - 1);
                    vector2List.RemoveAt(vector2List.Count - 1);
                    vector2List.Add(tileLocation + new Vector2(1f, -2f));
                    vector2List.Add(tileLocation + new Vector2(1f, -1f));
                    vector2List.Add(tileLocation + new Vector2(1f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(-1f, -2f));
                    vector2List.Add(tileLocation + new Vector2(-1f, -1f));
                    vector2List.Add(tileLocation + new Vector2(-1f, 0.0f));
                }
                if (power >= 5)
                {
                    vector2List.Add(tileLocation + new Vector2(2f, -4f));
                    vector2List.Add(tileLocation + new Vector2(2f, -3f));
                    vector2List.Add(tileLocation + new Vector2(2f, -2f));
                    vector2List.Add(tileLocation + new Vector2(2f, -1f));
                    vector2List.Add(tileLocation + new Vector2(2f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(1f, -4f));
                    vector2List.Add(tileLocation + new Vector2(1f, -3f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, -4f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, -3f));
                    vector2List.Add(tileLocation + new Vector2(-1f, -4f));
                    vector2List.Add(tileLocation + new Vector2(-1f, -3f));
                    vector2List.Add(tileLocation + new Vector2(-2f, -4f));
                    vector2List.Add(tileLocation + new Vector2(-2f, -3f));
                    vector2List.Add(tileLocation + new Vector2(-2f, -2f));
                    vector2List.Add(tileLocation + new Vector2(-2f, -1f));
                    vector2List.Add(tileLocation + new Vector2(-2f, 0.0f));
                    vector2List.Remove(tileLocation + new Vector2(-1f, -5f));
                    vector2List.Remove(tileLocation + new Vector2(0.0f, -5f));
                    vector2List.Remove(tileLocation + new Vector2(1f, -5f));
                }
            }
            else if (who.facingDirection == 1)
            {
                if (power >= 2)
                {
                    vector2List.Add(tileLocation + new Vector2(1f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(2f, 0.0f));
                }
                if (power >= 3)
                {
                    vector2List.Add(tileLocation + new Vector2(3f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(4f, 0.0f));
                }
                if (power >= 4)
                {
                    vector2List.RemoveAt(vector2List.Count - 1);
                    vector2List.RemoveAt(vector2List.Count - 1);
                    vector2List.Add(tileLocation + new Vector2(0.0f, -1f));
                    vector2List.Add(tileLocation + new Vector2(1f, -1f));
                    vector2List.Add(tileLocation + new Vector2(2f, -1f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, 1f));
                    vector2List.Add(tileLocation + new Vector2(1f, 1f));
                    vector2List.Add(tileLocation + new Vector2(2f, 1f));
                }
                if (power >= 5)
                {
                    vector2List.Add(tileLocation + new Vector2(0.0f, -2f));
                    vector2List.Add(tileLocation + new Vector2(1f, -2f));
                    vector2List.Add(tileLocation + new Vector2(2f, -2f));
                    vector2List.Add(tileLocation + new Vector2(3f, -2f));
                    vector2List.Add(tileLocation + new Vector2(4f, -2f));
                    vector2List.Add(tileLocation + new Vector2(3f, -1f));
                    vector2List.Add(tileLocation + new Vector2(4f, -1f));
                    vector2List.Add(tileLocation + new Vector2(3f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(4f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(3f, 1f));
                    vector2List.Add(tileLocation + new Vector2(4f, 1f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, 2f));
                    vector2List.Add(tileLocation + new Vector2(1f, 2f));
                    vector2List.Add(tileLocation + new Vector2(2f, 2f));
                    vector2List.Add(tileLocation + new Vector2(3f, 2f));
                    vector2List.Add(tileLocation + new Vector2(4f, 2f));
                    vector2List.Remove(tileLocation + new Vector2(5f, 1f));
                    vector2List.Remove(tileLocation + new Vector2(5f, 0.0f));
                    vector2List.Remove(tileLocation + new Vector2(5f, -1f));
                }
            }
            else if (who.facingDirection == 2)
            {
                if (power >= 2)
                {
                    vector2List.Add(tileLocation + new Vector2(0.0f, 1f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, 2f));
                }
                if (power >= 3)
                {
                    vector2List.Add(tileLocation + new Vector2(0.0f, 3f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, 4f));
                }
                if (power >= 4)
                {
                    vector2List.RemoveAt(vector2List.Count - 1);
                    vector2List.RemoveAt(vector2List.Count - 1);
                    vector2List.Add(tileLocation + new Vector2(1f, 2f));
                    vector2List.Add(tileLocation + new Vector2(1f, 1f));
                    vector2List.Add(tileLocation + new Vector2(1f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(-1f, 2f));
                    vector2List.Add(tileLocation + new Vector2(-1f, 1f));
                    vector2List.Add(tileLocation + new Vector2(-1f, 0.0f));
                }
                if (power >= 5)
                {
                    vector2List.Add(tileLocation + new Vector2(2f, 4f));
                    vector2List.Add(tileLocation + new Vector2(2f, 3f));
                    vector2List.Add(tileLocation + new Vector2(2f, 2f));
                    vector2List.Add(tileLocation + new Vector2(2f, 1f));
                    vector2List.Add(tileLocation + new Vector2(2f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(1f, 4f));
                    vector2List.Add(tileLocation + new Vector2(1f, 3f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, 4f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, 3f));
                    vector2List.Add(tileLocation + new Vector2(-1f, 4f));
                    vector2List.Add(tileLocation + new Vector2(-1f, 3f));
                    vector2List.Add(tileLocation + new Vector2(-2f, 4f));
                    vector2List.Add(tileLocation + new Vector2(-2f, 4f));
                    vector2List.Add(tileLocation + new Vector2(-2f, 3f));
                    vector2List.Add(tileLocation + new Vector2(-2f, 2f));
                    vector2List.Add(tileLocation + new Vector2(-2f, 1f));
                    vector2List.Add(tileLocation + new Vector2(-2f, 0.0f));
                    vector2List.Remove(tileLocation + new Vector2(1f, 5f));
                    vector2List.Remove(tileLocation + new Vector2(0.0f, 5f));
                    vector2List.Remove(tileLocation + new Vector2(-1f, 5f));
                }
            }
            else if (who.facingDirection == 3)
            {
                if (power >= 2)
                {
                    vector2List.Add(tileLocation + new Vector2(-1f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(-2f, 0.0f));
                }
                if (power >= 3)
                {
                    vector2List.Add(tileLocation + new Vector2(-3f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(-4f, 0.0f));
                }
                if (power >= 4)
                {
                    vector2List.RemoveAt(vector2List.Count - 1);
                    vector2List.RemoveAt(vector2List.Count - 1);
                    vector2List.Add(tileLocation + new Vector2(0.0f, -1f));
                    vector2List.Add(tileLocation + new Vector2(-1f, -1f));
                    vector2List.Add(tileLocation + new Vector2(-2f, -1f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, 1f));
                    vector2List.Add(tileLocation + new Vector2(-1f, 1f));
                    vector2List.Add(tileLocation + new Vector2(-2f, 1f));
                }
                if (power >= 5)
                {
                    vector2List.Add(tileLocation + new Vector2(0.0f, -2f));
                    vector2List.Add(tileLocation + new Vector2(-1f, -2f));
                    vector2List.Add(tileLocation + new Vector2(-2f, -2f));
                    vector2List.Add(tileLocation + new Vector2(-3f, -2f));
                    vector2List.Add(tileLocation + new Vector2(-4f, -2f));
                    vector2List.Add(tileLocation + new Vector2(-3f, -1f));
                    vector2List.Add(tileLocation + new Vector2(-4f, -1f));
                    vector2List.Add(tileLocation + new Vector2(-3f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(-4f, 0.0f));
                    vector2List.Add(tileLocation + new Vector2(-3f, 1f));
                    vector2List.Add(tileLocation + new Vector2(-4f, 1f));
                    vector2List.Add(tileLocation + new Vector2(0.0f, 2f));
                    vector2List.Add(tileLocation + new Vector2(-1f, 2f));
                    vector2List.Add(tileLocation + new Vector2(-2f, 2f));
                    vector2List.Add(tileLocation + new Vector2(-3f, 2f));
                    vector2List.Add(tileLocation + new Vector2(-4f, 2f));
                    vector2List.Remove(tileLocation + new Vector2(-5f, -1f));
                    vector2List.Remove(tileLocation + new Vector2(-5f, 0.0f));
                    vector2List.Remove(tileLocation + new Vector2(-5f, 1f));
                }
            }
            return vector2List;
        }

        public static bool perfWaterAction(IridiumWaterCan w, int damage, Vector2 tileLocation, GameLocation location, HoeDirt joeDirte)
        {
            if (w != null)
            {
                joeDirte.state = 1;
            }
            return false;
        }

        public static bool perfHoeAtion(IridiumHoe h, StardewValley.Object obj)
        {
            if (h == null)
            {
                if (Game1.currentLocation.objects.ContainsKey(obj.tileLocation) && Game1.currentLocation.objects[obj.tileLocation].Equals((object)obj))
                {
                    Game1.createRadialDebris(Game1.currentLocation, 12, (int)obj.tileLocation.X, (int)obj.tileLocation.Y, Game1.random.Next(4, 10), false, -1, false, -1);
                    Game1.currentLocation.objects.Remove(obj.tileLocation);
                }
                return false;
            }

            GameLocation currentLocation = h.getLastFarmerToUse().currentLocation;

            if (obj.name.Contains("Twig") || obj.name.Equals("Boulder") || obj.name.Equals("Stone"))
                return false;

            if (obj.name.Contains("Weeds") && h.isHeavyHitter())
            {
                cutWeed(h.getLastFarmerToUse(), obj);
                return true;
            }
     
            if (obj.parentSheetIndex == 590)
            {
                currentLocation.digUpArtifactSpot((int) obj.tileLocation.X, (int) obj.tileLocation.Y, h.getLastFarmerToUse());
                if (!currentLocation.terrainFeatures.ContainsKey(obj.tileLocation))
                    currentLocation.terrainFeatures.Add(obj.tileLocation, (TerrainFeature)new HoeDirt());
                Game1.playSound("hoeHit");
                if (currentLocation.objects.ContainsKey(obj.tileLocation))
                    currentLocation.objects.Remove(obj.tileLocation);
                return false;
            }

            if (obj.fragility == 2 || obj.type == null || (!obj.type.Equals("crafting") || !(h.GetType() != typeof(MeleeWeapon))) || !h.isHeavyHitter())
                return false;

            Game1.playSound("hammer");
            if (obj.fragility == 1)
            {
                Game1.createRadialDebris(currentLocation, 12, (int)obj.tileLocation.X, (int)obj.tileLocation.Y, Game1.random.Next(4, 10), false, -1, false, -1);
                if (currentLocation.objects.ContainsKey(obj.tileLocation))
                    currentLocation.objects.Remove(obj.tileLocation);
                return false;
            }
            if (obj.name.Equals("Tapper") && h.getLastFarmerToUse().currentLocation.terrainFeatures.ContainsKey(obj.tileLocation) && h.getLastFarmerToUse().currentLocation.terrainFeatures[obj.tileLocation] is Tree)
                (h.getLastFarmerToUse().currentLocation.terrainFeatures[obj.tileLocation] as Tree).tapped = false;
            if (obj.heldObject != null && obj.readyForHarvest)
                h.getLastFarmerToUse().currentLocation.debris.Add(new Debris((Item)obj.heldObject, obj.tileLocation * (float)Game1.tileSize + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2))));
            if (obj.parentSheetIndex == 157)
            {
                obj.parentSheetIndex = 156;
                obj.heldObject = (StardewValley.Object)null;
                obj.minutesUntilReady = -1;
            }

            return true;
        }

        private static void cutWeed(StardewValley.Farmer who, StardewValley.Object obj)
        {
            Color color = Color.Green;
            string cueName = "cut";
            int rowInAnimationTexture = 50;
            obj.fragility = 2;
            int parentSheetIndex = -1;
            if (Game1.random.NextDouble() < 0.5)
                parentSheetIndex = 771;
            else if (Game1.random.NextDouble() < 0.05)
                parentSheetIndex = 770;
            switch (obj.parentSheetIndex)
            {
                case 313:
                case 314:
                case 315:
                    color = new Color(84, 101, 27);
                    break;
                case 316:
                case 317:
                case 318:
                    color = new Color(109, 49, 196);
                    break;
                case 319:
                    color = new Color(30, 216, (int)byte.MaxValue);
                    cueName = "breakingGlass";
                    rowInAnimationTexture = 47;
                    Game1.playSound("drumkit2");
                    parentSheetIndex = -1;
                    break;
                case 320:
                    color = new Color(175, 143, (int)byte.MaxValue);
                    cueName = "breakingGlass";
                    rowInAnimationTexture = 47;
                    Game1.playSound("drumkit2");
                    parentSheetIndex = -1;
                    break;
                case 321:
                    color = new Color(73, (int)byte.MaxValue, 158);
                    cueName = "breakingGlass";
                    rowInAnimationTexture = 47;
                    Game1.playSound("drumkit2");
                    parentSheetIndex = -1;
                    break;
                case 678:
                    color = new Color(228, 109, 159);
                    break;
                case 679:
                    color = new Color(253, 191, 46);
                    break;
                case 792:
                case 793:
                case 794:
                    parentSheetIndex = 770;
                    break;
            }
            if (cueName.Equals("breakingGlass") && Game1.random.NextDouble() < 1.0 / 400.0)
                parentSheetIndex = 338;
            Game1.playSound(cueName);
            who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(rowInAnimationTexture, obj.tileLocation * (float)Game1.tileSize, color, 8, false, 100f, 0, -1, -1f, -1, 0));
            who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(rowInAnimationTexture, obj.tileLocation * (float)Game1.tileSize + new Vector2((float)Game1.random.Next(-Game1.tileSize / 4, Game1.tileSize / 4), (float)Game1.random.Next(-Game1.tileSize * 3 / 4, Game1.tileSize * 3 / 4)), color * 0.75f, 8, false, 100f, 0, -1, -1f, -1, 0)
            {
                scale = 0.75f,
                flipped = true
            });
            who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(rowInAnimationTexture, obj.tileLocation * (float)Game1.tileSize + new Vector2((float)Game1.random.Next(-Game1.tileSize / 4, Game1.tileSize / 4), (float)Game1.random.Next(-Game1.tileSize * 3 / 4, Game1.tileSize * 3 / 4)), color * 0.75f, 8, false, 100f, 0, -1, -1f, -1, 0)
            {
                scale = 0.75f,
                delayBeforeAnimationStart = 50
            });
            who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(rowInAnimationTexture, obj.tileLocation * (float)Game1.tileSize + new Vector2((float)Game1.random.Next(-Game1.tileSize / 4, Game1.tileSize / 4), (float)Game1.random.Next(-Game1.tileSize * 3 / 4, Game1.tileSize * 3 / 4)), color * 0.75f, 8, false, 100f, 0, -1, -1f, -1, 0)
            {
                scale = 0.75f,
                flipped = true,
                delayBeforeAnimationStart = 100
            });
            if (parentSheetIndex != -1)
                who.currentLocation.debris.Add(new Debris((Item)new StardewValley.Object(parentSheetIndex, 1, false, -1, 0), obj.tileLocation * (float)Game1.tileSize + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2))));
            if (Game1.random.NextDouble() >= 0.02)
                return;
            who.currentLocation.addJumperFrog(obj.tileLocation);
        }

        public static void toolDrawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, IridiumWaterCan w)
        {
            spriteBatch.Draw(Game1.toolSpriteSheet, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(Game1.toolSpriteSheet, Game1.tileSize / 4, Game1.tileSize / 4, w.indexOfMenuItemView)), Color.White * transparency, 0.0f, new Vector2((float)(Game1.tileSize / 4 / 2), (float)(Game1.tileSize / 4 / 2)), (float)Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
            if (!w.stackable)
                return;
            Game1.drawWithBorder(string.Concat((object)((Stackable)(Tool)w).NumberInStack), Color.Black, Color.White, location + new Vector2((float)Game1.tileSize - Game1.dialogueFont.MeasureString(string.Concat((object)((Stackable)(Tool)w).NumberInStack)).X, (float)Game1.tileSize - (float)((double)Game1.dialogueFont.MeasureString(string.Concat((object)((Stackable)(Tool)w).NumberInStack)).Y * 3.0 / 4.0)), 0.0f, 0.5f, 1f);
        }

        /*----IN CLASS---------------------------------------------------------------------*/
    }
}
