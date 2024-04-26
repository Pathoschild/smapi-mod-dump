/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Cast.Weald;
using StardewDruid.Data;
using StardewDruid.Event;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.FruitTrees;
using StardewValley.Internal;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using static StardewDruid.Data.CharacterData;

namespace StardewDruid.Character
{
    public class Effigy : StardewDruid.Character.Character
    {
        new public CharacterData.characters characterType = CharacterData.characters.effigy;

        public List<Vector2> ritesDone;
        public int riteIcon;
        public bool showIcon;

        public Texture2D iconsTexture;

        public Texture2D swipeTexture;
        public Dictionary<int, List<Rectangle>> swipeFrames;

        public Effigy()
        {
        }

        public Effigy(characters type)
          : base(type)
        {

            
        }

        protected override void initNetFields()
        {
            base.initNetFields();

        }

        public override void LoadOut()
        {

            LoadBase();

            characterTexture = CharacterData.CharacterTexture(characterType);

            haltFrames = FrameSeries(32, 32, 0, 0, 1);

            walkFrames = FrameSeries(32, 32, 0, 128, 6, haltFrames);

            walkLeft = 1;

            walkRight = 4;

            specialScheme = SpellHandle.schemes.stars;

            idleFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(128, 0, 32, 32),
                    new Rectangle(160, 0, 32, 32),
                },
                [1] = new()
                {
                    new Rectangle(128, 0, 32, 32),
                    new Rectangle(160, 0, 32, 32),
                },
                [2] = new()
                {
                    new Rectangle(128, 0, 32, 32),
                    new Rectangle(160, 0, 32, 32),
                },
                [3] = new()
                {
                    new Rectangle(128, 0, 32, 32),
                    new Rectangle(160, 0, 32, 32),
                },
            };

            specialFrames = new()
            {
                [0] = new()
                {

                    new(64, 64, 32, 32),
                    new(96, 64, 32, 32),

                },
                [1] = new()
                {

                    new(64, 32, 32, 32),
                    new(96, 32, 32, 32),

                },
                [2] = new()
                {

                    new(64, 0, 32, 32),
                    new(96, 0, 32, 32),

                },
                [3] = new()
                {

                    new(64, 96, 32, 32),
                    new(96, 96, 32, 32),

                },

            };

            workFrames = specialFrames;

            specialInterval = 30;
            specialCeiling = 1;
            specialFloor = 1;

            dashCeiling = 10;
            dashFloor = 0;

            dashFrames = new()
            {
                [0] = new()
                {
                    new(0, 192, 32, 32),
                    new(32, 64, 32, 32),
                    new(32, 64, 32, 32),
                    new(32, 64, 32, 32),
                    new(32, 64, 32, 32),
                    new(32, 64, 32, 32),
                    new(32, 64, 32, 32),
                    new(32, 64, 32, 32),
                    new(96,192,32,32),
                    new(128,192,32,32),
                    new(160,192,32,32),
                },
                [1] = new()
                {
                    new(0, 160, 32, 32),
                    new(32, 32, 32, 32),
                    new(32, 32, 32, 32),
                    new(32, 32, 32, 32),
                    new(32, 32, 32, 32),
                    new(32, 32, 32, 32),
                    new(32, 32, 32, 32),
                    new(32, 32, 32, 32),
                    new(96,160,32,32),
                    new(128,160,32,32),
                    new(160,160,32,32),
                },
                [2] = new()
                {
                    new(0, 128, 32, 32),
                    new(32, 0, 32, 32),
                    new(32, 0, 32, 32),
                    new(32, 0, 32, 32),
                    new(32, 0, 32, 32),
                    new(32, 0, 32, 32),
                    new(32, 0, 32, 32),
                    new(32, 0, 32, 32),
                    new(96,128,32,32),
                    new(128,128,32,32),
                    new(160,128,32,32),
                },
                [3] = new()
                {
                    new(0, 224, 32, 32),
                    new(32, 96, 32, 32),
                    new(32, 96, 32, 32),
                    new(32, 96, 32, 32),
                    new(32, 96, 32, 32),
                    new(32, 96, 32, 32),
                    new(32, 96, 32, 32),
                    new(32, 96, 32, 32),
                    new(96,224,32,32),
                    new(128,224,32,32),
                    new(160,224,32,32),
                },
            };

            sweepFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(192, 320, 64, 64),
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                },
                [1] = new()
                {
                    new Rectangle(0, 384, 64, 64),
                    new Rectangle(64, 384, 64, 64),
                    new Rectangle(128, 384, 64, 64),
                    new Rectangle(192, 384, 64, 64),
                },
                [2] = new()
                {
                    new Rectangle(0, 256, 64, 64),
                    new Rectangle(64, 256, 64, 64),
                    new Rectangle(128, 256, 64, 64),
                    new Rectangle(192, 256, 64, 64),
                },
                [3] = new()
                {
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(192, 320, 64, 64),
                },
            };

            swipeTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Swipe.png"));

            swipeFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(128, 64, 64, 64),
                    new Rectangle(192, 64, 64, 64),
                    new Rectangle(0, 64, 64, 64),
                    new Rectangle(64, 64, 64, 64),
                },
                [1] = new()
                {
                    new Rectangle(0, 128, 64, 64),
                    new Rectangle(64, 128, 64, 64),
                    new Rectangle(128, 128, 64, 64),
                    new Rectangle(192, 128, 64, 64),
                },
                [2] = new()
                {
                    new Rectangle(0, 0, 64, 64),
                    new Rectangle(64, 0, 64, 64),
                    new Rectangle(128, 0, 64, 64),
                    new Rectangle(192, 0, 64, 64),
                },
                [3] = new()
                {
                    new Rectangle(0, 64, 64, 64),
                    new Rectangle(64, 64, 64, 64),
                    new Rectangle(128, 64, 64, 64),
                    new Rectangle(192, 64, 64, 64),
                },
            };

            ritesDone = new List<Vector2>();

            //iconsTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Icons.png"));

            iconsTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "EffigyIcon.png"));

            loadedOut = true;

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {

            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if (characterTexture == null)
            {

                return;

            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            DrawEmote(b);

            if (netStandbyActive.Value)
            {

                DrawStandby(b, localPosition, drawLayer);

                return;

            } else if (netHaltActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    haltFrames[netDirection.Value][0],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else if (netSweepActive.Value)
            {

                b.Draw(
                     characterTexture,
                     localPosition - new Vector2(96, 128f),
                     sweepFrames[netDirection.Value][sweepFrame],
                     Color.White,
                     0f,
                     Vector2.Zero,
                     4f,
                     (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     drawLayer
                 );

                b.Draw(
                     swipeTexture,
                     localPosition - new Vector2(96, 128f),
                     swipeFrames[netDirection.Value][sweepFrame],
                     Color.White * 0.5f,
                     0f,
                     Vector2.Zero,
                     4f,
                     (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     drawLayer
                 );

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    specialFrames[netDirection.Value][specialFrame],
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0,
                    drawLayer
                );

            }
            else if (netDashActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f + dashHeight) ,
                    dashFrames[netDirection.Value][dashFrame],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else
            {

                if (TightPosition() && currentLocation.IsOutdoors && (idleTimer > 0) && !netSceneActive.Value)
                {

                    DrawStandby(b, localPosition, drawLayer);

                    return;

                }

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    walkFrames[netDirection.Value][moveFrame],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }

            DrawShadow(b, localPosition, drawLayer);

            DrawIcon(b, localPosition, drawLayer);

        }

        public override Rectangle GetBoundingBox()
        {

            return new Rectangle((int)Position.X + 8, (int)Position.Y + 8, 48, 48);

        }

        public override Rectangle GetHitBox()
        {
            return new Rectangle((int)Position.X - 32, (int)Position.Y - 32, 128, 128);
        }

        public void DrawIcon(SpriteBatch b, Vector2 localPosition, float drawLayer)
        {
            
            if (netDirection.Value != 0)
            {
            
                return;
            
            }

            /*int riteIcon;

            switch (Mod.instance.CurrentBlessing())
            {

                case "mists":
                    riteIcon = 2;
                    break;
                case "stars":
                    riteIcon = 3;
                    break;
                case "fates":
                    riteIcon = 4;
                    break;
                case "ether":
                    riteIcon = 6;
                    break;
                default: // weald
                    riteIcon = 1;
                    break;

            }*/

            Vector2 offset = new(21, -16);

            if (netDashActive.Value)
            {

                offset.Y -= dashHeight;
            }

            b.Draw(
                iconsTexture,
                localPosition + offset,
                //new Rectangle((riteIcon % 4) * 8, (riteIcon / 4) * 8, 8, 8),
                new(0,0,12,12),
                Color.White*0.65f,
                0f,
                new Vector2(0, 0),
                2f,
                SpriteEffects.None,
                drawLayer + 0.0001f
            );

        }

        public void DrawStandby(SpriteBatch b, Vector2 localPosition, float drawLayer)
        {

            int chooseFrame = IdleFrame();

            b.Draw(
                characterTexture,
                localPosition - new Vector2(32f, 64f),
                idleFrames[0][chooseFrame],
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                netDirection.Value == 1 || netAlternative.Value == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer
            );

            b.Draw(
                characterTexture,
                localPosition - new Vector2(30f, 60f),
                idleFrames[0][chooseFrame],
                Color.Black * 0.25f,
                0f,
                Vector2.Zero,
                4f,
                netDirection.Value == 1 || netAlternative.Value == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer - 0.001f
            );

            return;

        }

        public override List<Vector2> RoamAnalysis()
        {

            List<Vector2> collection = base.RoamAnalysis();

            if (Game1.currentSeason == "winter")
            {
                
                return collection;

            }

            List<Vector2> scarelist = new List<Vector2>();

            int takeABreak = 0;

            foreach (Dictionary<Vector2, StardewValley.Object> dictionary in currentLocation.Objects)
            {
                
                foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in dictionary)
                {
                    
                    if (keyValuePair.Value.IsScarecrow())
                    {

                        Vector2 scareVector = new(keyValuePair.Key.X * 64f, keyValuePair.Key.Y * 64f);

                        scarelist.Add(scareVector);

                        takeABreak++;

                    }

                    if (takeABreak >= 4)
                    {

                        scarelist.Add(new Vector2(-1f));

                        takeABreak = 0;

                    }

                }

                if (scarelist.Count >= 30)
                {

                    break;

                }
                    
            }

            scarelist.AddRange(collection);
            
            return scarelist;
        
        }

        public override bool TargetWork()
        {
            
            //Vector2.op_Division(roamVectors[roamIndex], 64f);

            string location = currentLocation.Name;

            if (Game1.currentSeason == "winter")
            {
                return false;
            }

            if (currentLocation.objects.Count() < 0)
            {
                return false;
            }
            
            List<Vector2> tileVectors;

            for (int i = 0; i < 4; i++)
            {

                tileVectors = ModUtility.GetTilesWithinRadius(currentLocation, occupied, i);

                foreach (Vector2 scarevector in tileVectors)
                {

                    if (ritesDone.Contains(scarevector))
                    {

                        continue;

                    }

                    if (currentLocation.objects.ContainsKey(scarevector))
                    {

                        if (currentLocation.Objects[scarevector].IsScarecrow())
                        {

                            ResetActives();

                            netSpecialActive.Set(true);

                            netWorkActive.Set(true);

                            specialTimer = 90;

                            workVector = scarevector;

                            return true;

                        }

                    }
                
                }
            
            }

            return false;

        }

        public override void PerformWork()
        {

            if(specialTimer == 80)
            {

                if (currentLocation.Name == Game1.player.currentLocation.Name && Utility.isOnScreen(Position, 128))
                {

                    Mod.instance.iconData.DecorativeIndicator(currentLocation, Position, IconData.decorations.weald, 1f);

                    Game1.player.currentLocation.playSound("discoverMineral", null, 1000);

                }

            }

            if(specialTimer != 50)
            {

                return;

            }

            Vector2 vector2 = workVector;

            if (!Mod.instance.targetCasts.ContainsKey(currentLocation.Name))
            {

                Mod.instance.targetCasts[currentLocation.Name] = new();

            }

            bool Reseed = !Mod.instance.EffectDisabled("Seeds");

            List<CastHandle> casts = new();

            for (int level = 1; level < (Mod.instance.PowerLevel + 2); level++)
            {

                foreach (Vector2 tileWithinRadius in ModUtility.GetTilesWithinRadius(currentLocation, vector2, level))
                {

                    if (Mod.instance.targetCasts[currentLocation.Name].ContainsKey(tileWithinRadius))
                    {

                        continue;

                    }

                    if (currentLocation.terrainFeatures.ContainsKey(tileWithinRadius))
                    {

                        if (currentLocation.terrainFeatures[tileWithinRadius] is StardewValley.TerrainFeatures.HoeDirt hoeDirtFeature)
                        {

                            if (hoeDirtFeature.crop != null)
                            {

                                StardewDruid.Cast.Weald.Crop cropHustle = new(tileWithinRadius, Reseed, true);

                                cropHustle.targetLocation = currentLocation;

                                casts.Add(cropHustle);

                                Mod.instance.targetCasts[currentLocation.Name][tileWithinRadius] = "Crop" + hoeDirtFeature.crop.indexOfHarvest.Value.ToString();

                            }

                        }


                    }

                }

            }

            string location = currentLocation.Name;

            for (int radius = 0; radius < 9; radius++)
            {

                List<Vector2> radii = ModUtility.GetTilesWithinRadius(currentLocation, vector2, radius);

                foreach (Vector2 radiiVector in radii)
                {

                    if (!Mod.instance.targetCasts.ContainsKey(location))
                    {

                        Mod.instance.targetCasts[location] = ModUtility.LocationTargets(currentLocation);
                    }

                    if (Mod.instance.targetCasts[location].ContainsKey(radiiVector))
                    {
                        continue;
                    }

                    if (currentLocation.terrainFeatures.ContainsKey(radiiVector))
                    {

                        if (currentLocation.terrainFeatures[radiiVector] is StardewValley.TerrainFeatures.FruitTree fruitTree)
                        {

                            if (fruitTree.stump.Value)
                            {

                                continue;

                            }

                            if (fruitTree.growthStage.Value < 4)
                            {

                                fruitTree.dayUpdate();

                            }
                            else if (!Game1.IsWinter)
                            {
                                Debris debris;

                                if ((int)fruitTree.struckByLightningCountdown.Value > 0)
                                {
                                    debris = new Debris("382", new Vector2(radiiVector.X * 64f + 32f, (radiiVector.Y - 3f) * 64f + 32f), Game1.player.Position)
                                    {
                                        itemQuality = 0,
                                    };
                                }
                                else
                                {

                                    FruitTreeData data = fruitTree.GetData();

                                    Item item = null;

                                    if (data?.Fruit != null)
                                    {

                                        foreach (FruitTreeFruitData item2 in data.Fruit)
                                        {

                                            item = ItemQueryResolver.TryResolveRandomItem(item2, new ItemQueryContext(currentLocation, null, null), avoidRepeat: false, null, null, null, delegate (string query, string error) { });

                                        }

                                    }

                                    if (item == null)
                                    {

                                        continue;

                                    }

                                    int itemQuality = 0;

                                    if ((int)fruitTree.daysUntilMature.Value <= -112)
                                    {
                                        itemQuality = 1;
                                    }

                                    if ((int)fruitTree.daysUntilMature.Value <= -224)
                                    {
                                        itemQuality = 2;
                                    }

                                    if ((int)fruitTree.daysUntilMature.Value <= -336)
                                    {
                                        itemQuality = 4;
                                    }

                                    if ((int)fruitTree.struckByLightningCountdown.Value > 0)
                                    {
                                        itemQuality = 0;
                                    }

                                    debris = new Debris(item, new Vector2(radiiVector.X * 64f + 32f, (radiiVector.Y - 3f) * 64f + 32f), Game1.player.Position)
                                    {
                                        itemQuality = itemQuality,
                                    };

                                }

                                debris.Chunks[0].xVelocity.Value += (float)Game1.random.Next(-10, 11) / 10f;

                                debris.chunkFinalYLevel = (int)(radiiVector.Y * 64f + 64f);

                                currentLocation.debris.Add(debris);
                            }

                            Mod.instance.targetCasts[location][radiiVector] = "Tree";

                        }

                    }

                }

            }

            if (casts.Count != 0)
            {

                foreach (Cast.CastHandle effect in casts)
                {

                    effect.CastEffect();
                }
            }

            ritesDone.Add(vector2);

        }


    }

}
