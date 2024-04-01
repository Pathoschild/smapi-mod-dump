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
using StardewDruid.Dialogue;
using StardewDruid.Event;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StardewDruid.Character
{
    public class Shadowtin : StardewDruid.Character.Character
    {

        public NetBool netForageActive = new(false);
        public Vector2 forageVector;
        public Dictionary<int, Rectangle> forageFrames;

        public Shadowtin()
        {
        }

        public Shadowtin(Vector2 position, string map)
          : base(position, map, nameof(Shadowtin))
        {

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddField(netForageActive, "netForageActive");

        }

        public override void LoadOut()
        {

            LoadBase();

            characterTexture = CharacterData.CharacterTexture(Name);

            walkFrames = FrameSeries(32, 32, 0, 128, 6);

            haltFrames = new()
            {
                [0] = new(0, 64, 32, 32),
                [1] = new(0, 32, 32, 32),
                [2] = new(0, 0, 32, 32),
                [3] = new(0, 96, 32, 32),

            };

            idleFrames = new()
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
                    new Rectangle(192, 320, 64, 64),
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                    new Rectangle(128, 320, 64, 64),
                },
                [2] = new()
                {
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(192, 320, 64, 64),
                },
                [3] = new()
                {
                    new Rectangle(64, 320, 64, 64),
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(192, 320, 64, 64),
                    new Rectangle(0, 320, 64, 64),
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

            dashCeiling = 2;
            dashFloor = 1;

            dashFrames = new()
            {
                [0] = new()
                {
                    new(0, 192, 32, 32),
                    new(32, 64, 32, 32),
                    new(32, 64, 32, 32),
                },
                [1] = new()
                {
                    new(0, 160, 32, 32),
                    new(32, 32, 32, 32),
                    new(32, 32, 32, 32),
                },
                [2] = new()
                {
                    new(0, 128, 32, 32),
                    new(32, 0, 32, 32),
                    new(32, 0, 32, 32),
                },
                [3] = new()
                {
                    new(0, 224, 32, 32),
                    new(32, 96, 32, 32),
                    new(32, 96, 32, 32),
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

            forageFrames = new()
            {

                [0] = new Rectangle(128, 0, 32, 32),
                [1] = new Rectangle(160, 0, 32, 32),

            };

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

            if (IsEmoting && !Game1.eventUp)
            {
                b.Draw(Game1.emoteSpriteSheet, localPosition - new Vector2(0,160), new Microsoft.Xna.Framework.Rectangle(base.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, base.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, drawLayer);
            }

            b.Draw(
                Game1.shadowTexture,
                localPosition + new Vector2(6, 44f),
                Game1.shadowTexture.Bounds,
                Color.White * alpha, 0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                drawLayer - 0.0001f
                );

            if (netHaltActive.Value)
            {

                int chooseFrame = idleFrame % 8;

                if (chooseFrame <5)
                {
                    b.Draw(
                        characterTexture,
                        localPosition - new Vector2(32, 64f),
                        haltFrames[netDirection.Value],
                        Color.White,
                        0f,
                        Vector2.Zero,
                        4f,
                        flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        drawLayer
                    );

                    return;

                }

                b.Draw(
                     characterTexture,
                     localPosition - new Vector2(96, 96f),
                     idleFrames[netDirection.Value][chooseFrame-4],
                     Color.White,
                     0f,
                     Vector2.Zero,
                     4f,
                     flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     drawLayer
                 );

            }
            else if( netSweepActive.Value)
            {

                b.Draw(
                     characterTexture,
                     localPosition - new Vector2(96, 96f),
                     sweepFrames[netDirection.Value][sweepFrame],
                     Color.White,
                     0f,
                     Vector2.Zero,
                     4f,
                     flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     drawLayer
                 );

            }
            else if (netForageActive.Value)
            {
                
                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(64, 64f),
                    forageFrames[specialFrame],
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    drawLayer
                );

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(96, 64f),
                    specialFrames[netDirection.Value][specialFrame],
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0,
                    drawLayer
                );

            }
            else if (netDashActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    dashFrames[netDirection.Value][dashFrame],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else
            {
                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    walkFrames[netDirection.Value][moveFrame],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }

        }

        public override Rectangle GetBoundingBox()
        {

            return new Rectangle ((int)Position.X+ 8, (int)Position.Y + 8, 48, 48);

        }

        public override Rectangle GetHitBox()
        {
            return new Rectangle((int)Position.X-32, (int)Position.Y -32, 128, 128);
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {

            if (Mod.instance.eventRegister.ContainsKey("transform"))
            {

                Mod.instance.CastMessage("Unable to converse while transformed");

                return false;

            }

            foreach (NPC character in currentLocation.characters)
            {

                if (character is StardewValley.Monsters.Monster monster && (double)Vector2.Distance(Position, monster.Position) <= 1280.0)
                {

                    return false;

                }

            }

            if (netDashActive.Value || netSpecialActive.Value)
            {

                return false;

            }

            if (!Mod.instance.dialogue.ContainsKey(nameof(Shadowtin)))
            {
                
                Dictionary<string, StardewDruid.Dialogue.Dialogue> dialogue = Mod.instance.dialogue;
                
                StardewDruid.Dialogue.Shadowtin shadowtin = new StardewDruid.Dialogue.Shadowtin();

                shadowtin.npc = this;
                
                dialogue[nameof(Shadowtin)] = shadowtin;
            
            }

            if (netSceneActive.Value && Mod.instance.dialogue[nameof(Shadowtin)].specialDialogue.Count == 0)
            {

                return false;

            }

            Mod.instance.dialogue[nameof(Shadowtin)].DialogueApproach();

            Halt();

            NextTarget(who.Position);

            return true;
        
        }

        public override void ResetActives()
        {
            base.ResetActives();

            netForageActive.Set(false);
        
        }

        public override void UpdateSpecial()
        {

            if(netForageActive.Value)
            {

                if(specialTimer == 60)
                {

                    Position = (forageVector * 64);

                    ModUtility.AnimateQuickWarp(currentLocation, Position);

                }

                if(specialTimer == 30)
                {

                    if (currentLocation.objects.ContainsKey(forageVector))
                    {

                        StardewValley.Object targetObject = currentLocation.objects[forageVector];

                        if (targetObject.Name.Contains("Artifact Spot"))
                        {

                            currentLocation.digUpArtifactSpot((int)forageVector.X, (int)forageVector.Y, Mod.instance.trackRegister["Shadowtin"].followPlayer);

                            currentLocation.objects.Remove(forageVector);
                        
                        }
                        else if (targetObject.isForage())
                        {


                            List<Chest> chests = new();

                            GameLocation farmcave = Game1.getLocationFromName("FarmCave");

                            int chestCount = 0;

                            foreach (Dictionary<Vector2, StardewValley.Object> dictionary in farmcave.Objects)
                            {

                                foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in dictionary)
                                {

                                    if (keyValuePair.Value is Chest)
                                    {

                                        chests.Add((Chest)keyValuePair.Value);

                                        if (chestCount == 2)
                                        {

                                            break;

                                        }

                                        chestCount++;

                                    }

                                }

                            }

                            if (chests.Count != 0)
                            {

                                Chest chest = chests.Last();

                                StardewValley.Item objectInstance = new StardewValley.Object(targetObject.ParentSheetIndex.ToString(), 1, false, -1, 4);

                                chest.addItem(objectInstance);

                                currentLocation.objects.Remove(forageVector);

                            }
                            else
                            {

                                new Throw(Mod.instance.trackRegister["Shadowtin"].followPlayer, Position, targetObject.ParentSheetIndex, 4).ThrowObject();

                                currentLocation.objects.Remove(forageVector);

                            }

                        }
                    
                    }

                }

            }

            if (netSpecialActive.Value)
            {

                specialTimer--;

                if (specialTimer <= 0)
                {

                    netSpecialActive.Set(false);

                    netForageActive.Set(false);

                    specialFrame=(0);

                    cooldownTimer = 120;

                }

            }

            if (netForageActive.Value)
            {

                if (specialTimer % 30== 0)
                {

                    int nextFrame = specialFrame+ 1;

                    if (nextFrame > 1) { nextFrame = 0; }

                    specialFrame=(nextFrame);

                }

            }

        }

        public override void TargetRandom(int level = 8)
        {

            if (netFollowActive.Value)
            {

                targetVectors.Clear();

                Vector2 currentVector = new((int)(Position.X / 64), (int)(Position.Y / 64));

                List<Vector2> objectVectors = new List<Vector2>();

                for (int i = 0; i < 6; i++)
                {

                    if (currentLocation.objects.Count() == 0)
                    {
                        break;

                    }

                    objectVectors = ModUtility.GetTilesWithinRadius(currentLocation, currentVector, i); ;

                    foreach (Vector2 objectVector in objectVectors)
                    {

                        if (currentLocation.objects.ContainsKey(objectVector))
                        {

                            StardewValley.Object targetObject = currentLocation.objects[objectVector];

                            if (targetObject.Name.Contains("Artifact Spot") || targetObject.isForage())
                            {

                                forageVector = objectVector;

                                netForageActive.Set(true);

                                netSpecialActive.Set(true);

                                specialTimer = 60;

                                return;

                            }

                        }

                    }

                }

            }

            base.TargetRandom();

        }

    }

}
