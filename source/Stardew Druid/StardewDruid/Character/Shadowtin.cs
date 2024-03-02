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
using StardewDruid.Event.World;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
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

        public NetBool netSweepActive = new(false);
        public NetBool netForageActive = new(false);

        public Dictionary<int, Rectangle> sweepFrames;

        public Vector2 forageVector;
        public Dictionary<int, Rectangle> forageFrames;

        public int sweepTimer;

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
            NetFields.AddFields(new INetSerializable[2]
            {
                 netSweepActive,
                 netForageActive,
            });
        }

        public override void LoadOut()
        {

            characterTexture = CharacterData.CharacterTexture(Name);

            barrages = new();

            roamVectors = new List<Vector2>();

            eventVectors = new List<Vector2>();

            targetVectors = new();

            opponentThreshold = 640;

            gait = 2f;

            modeActive = mode.random;

            behaviourActive = behaviour.idle;

            idleInterval = 90;

            moveLength = 4;
            moveInterval = 12;

            specialInterval = 30;

            walkFrames = WalkFrames(32, 32);

            dashFrames = WalkFrames(32, 32);

            dashFrames[0][2] = new Rectangle(64, 192, 32, 32);
            dashFrames[1][2] = new Rectangle(32, 192, 32, 32);
            dashFrames[2][2] = new Rectangle(0, 192, 32, 32);
            dashFrames[3][2] = new Rectangle(96, 192, 32, 32);

            sweepFrames = new()
            {

                [0] = new Rectangle(0, 160, 64, 32),
                [1] = new Rectangle(64, 160, 64, 32),
                [2] = new Rectangle(0, 128, 64, 32),
                [3] = new Rectangle(64, 128, 64, 32),

            };

            haltFrames = sweepFrames;

            specialFrames = sweepFrames;

            forageFrames = new()
            {

                [0] = new Rectangle(0, 224, 32, 32),
                [1] = new Rectangle(32, 224, 32, 32),

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

            if (IsEmoting && !Game1.eventUp)
            {
                Vector2 localPosition2 = getLocalPosition(Game1.viewport);
                localPosition2.Y -= 160;
                b.Draw(Game1.emoteSpriteSheet, localPosition2, new Microsoft.Xna.Framework.Rectangle(base.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, base.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, getStandingY() / 10000f);
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            b.Draw(
                Game1.shadowTexture,
                localPosition + new Vector2(6, 44f),
                Game1.shadowTexture.Bounds,
                Color.White * alpha, 0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                Math.Max(0.0f, getStandingY() / 10000f) - 0.0001f
                );

            if (netHaltActive.Value)
            {

                int chooseFrame = idleFrame.Value % 6;

                if (chooseFrame <2)
                {
                    b.Draw(
                        characterTexture,
                        localPosition - new Vector2(32, 64f),
                        walkFrames[netDirection.Value][0],
                        Color.White,
                        0f,
                        Vector2.Zero,
                        4f,
                        flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                    );

                    return;

                }

                b.Draw(
                     characterTexture,
                     localPosition - new Vector2(96, 64f),
                     haltFrames[chooseFrame - 2],
                     Color.White,
                     0f,
                     Vector2.Zero,
                     4f,
                     flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                 );

            }
            else if( netSweepActive.Value)
            {

                b.Draw(
                     characterTexture,
                     localPosition - new Vector2(96, 64f),
                     sweepFrames[specialFrame.Value],
                     Color.White,
                     0f,
                     Vector2.Zero,
                     4f,
                     flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                 );

            }
            else if (netForageActive.Value)
            {
                
                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(64, 64f),
                    forageFrames[specialFrame.Value],
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                );

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(96, 64f),
                    specialFrames[netDirection.Value],
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0,
                    Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                );

            }
            else if (netDashActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    dashFrames[netDirection.Value][moveFrame.Value],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                );

            }
            else
            {
                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    walkFrames[netDirection.Value][moveFrame.Value],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                );

            }

        }

        public override Rectangle GetBoundingBox()
        {

            return new Rectangle ((int)Position.X, (int)Position.Y, 64, 64);

        }

        public override Rectangle GetHitBox()
        {
            return new Rectangle((int)Position.X-32, (int)Position.Y -32, 128, 128);
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            if (!base.checkAction(who, l))
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
            
            Mod.instance.dialogue[nameof(Shadowtin)].DialogueApproach();
            
            return true;
        
        }

        public override void ResetActives()
        {
            base.ResetActives();

            netSweepActive.Set(false);

            sweepTimer = 0;

            netForageActive.Set(false);
        
        }

        public override void UpdateMove()
        {

            base.UpdateMove();

            if (netDashActive.Value)
            {
                
                float distance = Vector2.Distance(Position, targetVectors.First());

                if (distance < 320 && moveFrame.Value > 2)
                {

                    moveFrame.Set(2);

                }

            }

        }

        public override void UpdateSpecial()
        {

            if(netForageActive.Value)
            {

                if(specialTimer == 60)
                {

                    Position = (forageVector * 64);

                    ModUtility.AnimateQuickWarp(currentLocation, Position, "Solar");

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
                        else if (targetObject.isForage(currentLocation))
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

                                StardewValley.Item objectInstance = new StardewValley.Object(targetObject.ParentSheetIndex, 1, false, -1, 4);

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

                    specialFrame.Set(0);

                    behaviourActive = behaviour.idle;

                    cooldownTimer = 120;

                }

            }

            if (netSweepActive.Value)
            {

                sweepTimer--;

                if (sweepTimer % 15 == 0)
                {

                    int nextFrame = specialFrame.Value + 1;

                    if (nextFrame > 3) { nextFrame = 0; }

                    specialFrame.Set(nextFrame);

                }

                if (sweepTimer <= 0)
                {

                    netSweepActive.Set(false);

                    specialFrame.Set(0);

                }

            }

            if (netForageActive.Value)
            {

                if (specialTimer % 30== 0)
                {

                    int nextFrame = specialFrame.Value + 1;

                    if (nextFrame > 1) { nextFrame = 0; }

                    specialFrame.Set(nextFrame);

                }

            }

            if (barrages.Count > 0)
            {

                UpdateBarrages();

            }

        }

        public override bool MonsterAttack(StardewValley.Monsters.Monster targetMonster)
        {

            float distance = Vector2.Distance(Position, targetMonster.Position);

            if (distance >= 128f && distance <= 640f)
            {

                Vector2 vector2 = new(targetMonster.Position.X - Position.X - 32f, targetMonster.Position.Y - Position.Y);

                if ((double)Math.Abs(vector2.Y) <= 128.0)
                {

                    netSpecialActive.Set(true);

                    behaviourActive = behaviour.special;

                    specialTimer = 60;

                    NextTarget(targetMonster.Position, -1);

                    ResetAll();

                    BarrageHandle beam = new(currentLocation, targetMonster.getTileLocation(), getTileLocation(), 2, 1, "Blue", -1, Mod.instance.DamageLevel());

                    beam.type = BarrageHandle.barrageType.beam;

                    barrages.Add(beam);

                }
                else
                {

                    behaviourActive = behaviour.dash;

                    moveTimer = (int)(distance / gait * 5);

                    netDashActive.Set(true);

                    NextTarget(targetMonster.Position, -1);

                }

                return true;

            }

            return false;

        }

        public override void HitMonster(StardewValley.Monsters.Monster monsterCharacter)
        {

            DealDamageToMonster(monsterCharacter,true);

            targetVectors.Clear();

            netSweepActive.Set(true);

            int nextFrame = netDirection.Value + 1;

            if (nextFrame > 3) { nextFrame = 0; }

            specialFrame.Set(nextFrame);

            sweepTimer = 60;

            cooldownTimer = 120;

        }

        public override void TargetRandom()
        {

            if (netFollowActive)
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

                            if (targetObject.Name.Contains("Artifact Spot") || targetObject.isForage(currentLocation))
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
