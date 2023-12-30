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
using StardewDruid.Cast;
using StardewDruid.Event.World;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StardewDruid.Character
{
    public class Jester : StardewDruid.Character.Character
    {
        public int xOffset;
        public Vector2 facePosition;
        public Vector2 beamPosition;
        public Vector2 beamTargetOne;
        public Vector2 beamTargetTwo;
        public float beamRotation;
        public bool flipBeam;
        public bool flipFace;

        public Jester()
        {
        }

        public Jester(Vector2 position, string map)
          : base(position, map, nameof(Jester))
        {
            HideShadow = true;
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (!Context.IsMainPlayer)
            {
                base.draw(b, alpha);

                return;
            }

            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if (base.IsEmoting && !Game1.eventUp)
            {
                Vector2 localPosition2 = getLocalPosition(Game1.viewport);
                localPosition2.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition2, new Microsoft.Xna.Framework.Rectangle(base.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, base.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, getStandingY() / 10000f);
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            b.Draw(
                Game1.shadowTexture,
                localPosition + new Vector2(32f, 40f),
                Game1.shadowTexture.Bounds,
                Color.White * alpha, 0f,
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                4f,
                SpriteEffects.None,
                Math.Max(0.0f, getStandingY() / 10000f) - 0.0001f
                );


            if (timers.ContainsKey("idle"))
            {
                int num3 = timers["idle"] / 80 % 4 * 32;

                b.Draw(
                    Sprite.Texture,
                    localPosition + new Vector2(64, 16f),
                    new Rectangle(num3, 256, 32, 32),
                    Color.White,
                    0f,
                    new Vector2(Sprite.SpriteWidth / 2, Sprite.SpriteHeight * 3f / 4f),
                    4f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f))
                );

            }
            else
            {
                b.Draw(
                    Sprite.Texture,
                    localPosition + new Vector2(64f + xOffset, 16f),
                    Sprite.SourceRect,
                    Color.White,
                    0f,
                    new Vector2(Sprite.SpriteWidth / 2, Sprite.SpriteHeight * 3f / 4f),
                    4f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() / 10000f)));


            }

        }

        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)Position.X + 8, (int)Position.Y + 16, 48, 32);
        }

        public override Rectangle GetHitBox()
        {
            return moveDirection % 2 == 0 ? new Rectangle((int)Position.X + 8, (int)Position.Y - 16, 48, 64) : new Rectangle((int)Position.X - 16, (int)Position.Y + 16, 96, 32);
        }

        public override void AnimateMovement(GameTime time)
        {
            if (timers.ContainsKey("attack") && targetOpponents.Count > 0 && (double)Vector2.Distance(Position, targetOpponents.First().Position) <= 96.0 && Sprite.CurrentFrame % 6 == 2 && Sprite.currentFrame >= 24)
            {
                return;
            }

            flip = false;
            moveDown = false;
            moveLeft = false;
            moveRight = false;
            moveUp = false;
            FacingDirection = moveDirection;
            Sprite.interval = 175f;
            xOffset = 0;
            switch (moveDirection)
            {
                case 0:
                    Sprite.AnimateUp(time, 0, "");
                    moveUp = true;
                    if (altDirection == 3)
                    {
                        flip = true;
                        xOffset = -56;
                        break;
                    }
                    break;
                case 1:
                    Sprite.AnimateRight(time, 0, "");
                    moveRight = true;
                    xOffset = -32;
                    break;
                case 2:
                    Sprite.AnimateDown(time, 0, "");
                    moveDown = true;
                    if (altDirection == 3)
                    {
                        flip = true;
                        xOffset = -56;
                        break;
                    }
                    break;
                default:
                    moveLeft = true;
                    Sprite.AnimateLeft(time, 0, "");
                    xOffset = -32;
                    break;
            }
            if (!timers.ContainsKey("sprint") && !timers.ContainsKey("attack"))
                return;
            if (Sprite.CurrentFrame < 24)
            {
                Sprite.CurrentFrame += 24;
                Sprite.UpdateSourceRect();
            }
            Sprite.interval = 125f;
        }

        public override bool TargetOpponent()
        {
            if (!base.TargetOpponent())
            {

                return false;
            
            }
                
            float distance = Vector2.Distance(Position, targetOpponents.First().Position);

            if (distance >= 64f && distance <= 480f)
            {

                StardewValley.Monsters.Monster targetMonster = targetOpponents.First();

                Vector2 vector2 = new(targetMonster.Position.X - Position.X - 32f, targetMonster.Position.Y - Position.Y);//Vector2.op_Subtraction(((StardewValley.Character)targetOpponents.First<Monster>()).Position, Vector2.op_Addition(Position, new Vector2(32f, 0.0f)));
                
                if ((double)Math.Abs(vector2.Y) <= 32.0 || (double)Math.Abs(vector2.X) <= 32.0)
                {
                    
                    TargetBeam();

                }

            }
            
            return true;

        }

        public void TargetBeam()
        {
            Halt();
            flip = false;
            timers["stop"] = 120;
            timers["cooldown"] = 180;
            timers["busy"] = 600;
            flipFace = false;
            flipBeam = false;
            facePosition = Vector2.Zero;
            string str = "RalphFace";
            beamRotation = 0.0f;
            float num = 999f;
            switch (moveDirection)
            {
                case 0:
                    Sprite.AnimateUp(Game1.currentGameTime, 0, "");
                    beamTargetOne = new(Position.X + 32f, Position.Y - 208f);//Vector2.op_Addition(Position, new Vector2(32f, -208f));
                    beamTargetTwo = new(Position.X + 32f, Position.Y - 408f); //Vector2.op_Addition(Position, new Vector2(32f, -448f));
                    beamPosition = new(Position.X - xOffset - 200, Position.Y - 320f); //Vector2.op_Addition(Position, new Vector2((float)(xOffset - 200), -320f));
                    beamRotation = -1.57079637f;
                    num = 0.0001f;
                    if (altDirection == 3)
                    {
                        beamPosition = new(Position.X - xOffset - 136, Position.Y - 320f); //Vector2.op_Addition(Position, new Vector2((float)(xOffset - 136), -320f));
                        flip = true;
                        break;
                    }
                    break;
                case 1:
                    Sprite.AnimateRight(Game1.currentGameTime, 0, "");
                    beamTargetOne = new(Position.X + 272f, Position.Y + 32f); //Vector2.op_Addition(Position, new Vector2(272f, 32f));
                    beamTargetTwo = new(Position.X + 512f, Position.Y + 32f); //Vector2.op_Addition(Position, new Vector2(512f, 32f));
                    facePosition = new(Position.X + xOffset + 68f, Position.Y - 52f); //Vector2.op_Addition(Position, new Vector2((float)(68 + xOffset), -52f));
                    beamPosition = new(Position.X + xOffset + 96f, Position.Y - 48f); //Vector2.op_Addition(Position, new Vector2((float)(96 + xOffset), -48f));
                    break;
                case 2:
                    Sprite.AnimateDown(Game1.currentGameTime, 0, "");
                    beamTargetOne = new(Position.X + 32f, Position.Y + 272f); //Vector2.op_Addition(Position, new Vector2(32f, 272f));
                    beamTargetTwo = new(Position.X + 32f, Position.Y + 512f);//Vector2.op_Addition(Position, new Vector2(32f, 512f));
                    facePosition = new(Position.X + xOffset + 12, Position.Y - 52f); //Vector2.op_Addition(Position, new Vector2((float)(12 + xOffset), -52f));
                    beamPosition = new(Position.X + xOffset - 200, Position.Y - 186f); //Vector2.op_Addition(Position, new Vector2((float)(xOffset - 200), 186f));
                    beamRotation = 1.57079637f;
                    str = "RalphZero";
                    if (altDirection == 3)
                    {
                        facePosition = new(Position.X + xOffset + 60f, Position.Y - 52f); //Vector2.op_Addition(Position, new Vector2((float)(60 + xOffset), -52f));
                        beamPosition = new(Position.X + xOffset - 136f, Position.Y + 186f); //Vector2.op_Addition(Position, new Vector2((float)(xOffset - 136), 186f));
                        flip = true;
                        break;
                    }
                    break;
                default:
                    Sprite.AnimateLeft(Game1.currentGameTime, 0, "");
                    beamTargetOne = new(Position.X - 208f, Position.Y + 32f); //Vector2.op_Addition(Position, new Vector2(-208f, 32f));
                    beamTargetTwo = new(Position.X - 448f, Position.Y + 32f); //Vector2.op_Addition(Position, new Vector2(-448f, 32f));
                    facePosition = new(Position.X + xOffset, Position.Y - 52f); //Vector2.op_Addition(Position, new Vector2((float)xOffset, -52f));
                    beamPosition = new(Position.X + xOffset - 464, Position.Y - 56f); //Vector2.op_Addition(Position, new Vector2((float)(xOffset - 464), -56f));
                    flipBeam = true;
                    flipFace = true;
                    break;
            }
            if (facePosition != Vector2.Zero)
                currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(0, 600f, 1, 1, facePosition, true, flipFace)
                {
                    sourceRect = new Rectangle(0, 0, 32, 32),
                    sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                    texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", str + ".png")),
                    scale = 2f,
                    timeBasedMotion = true,
                    layerDepth = 998f
                });
            currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(0, 75f, 4, 1, beamPosition, false, flipBeam)
            {
                sourceRect = new Rectangle(0, 0, 160, 32),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "EnergyBeam.png")),
                scale = 3f,
                timeBasedMotion = true,
                layerDepth = num,
                rotation = beamRotation
            });
            currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(0, 75f, 2, 2, beamPosition, false, flipBeam)
            {
                sourceRect = new Rectangle(0, 96, 160, 32),
                sourceRectStartingPos = new Vector2(0.0f, 96f),
                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "EnergyBeam.png")),
                scale = 3f,
                timeBasedMotion = true,
                layerDepth = num,
                alphaFade = 1f / 500f,
                rotation = beamRotation,
                delayBeforeAnimationStart = 300
            });
            ApplyDazeEffect(targetOpponents.First());

            DelayedAction.functionAfterDelay(ApplyBeamEffect, 300);
        }

        public void ApplyBeamEffect()
        {
            if (!ModUtility.MonsterVitals(targetOpponents.First(), currentLocation))
                return;
            for (int index = currentLocation.characters.Count - 1; index >= 0; --index)
            {
                if (currentLocation.characters[index] is StardewValley.Monsters.Monster character && character != targetOpponents.First())
                {
                    if ((double)Vector2.Distance(Position, beamTargetOne) < 128.0)
                        base.DealDamageToMonster(character, true, Mod.instance.DamageLevel(), false);
                    else if ((double)Vector2.Distance(Position, beamTargetTwo) < 128.0)
                        base.DealDamageToMonster(character, true, Mod.instance.DamageLevel(), false);
                }
            }
            base.DealDamageToMonster(targetOpponents.First(), true, Mod.instance.DamageLevel() * 2, false);
            currentLocation.playSoundPitched("flameSpellHit", 1200, 0);
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            if (!base.checkAction(who, l))
                return false;
            if (!Mod.instance.dialogue.ContainsKey(nameof(Jester)))
            {
                Dictionary<string, StardewDruid.Dialogue.Dialogue> dialogue = Mod.instance.dialogue;
                StardewDruid.Dialogue.Jester jester = new StardewDruid.Dialogue.Jester();
                jester.npc = this;
                dialogue[nameof(Jester)] = jester;
            }
            Mod.instance.dialogue[nameof(Jester)].DialogueApproach();
            return true;
        }

        public override void DealDamageToMonster(StardewValley.Monsters.Monster monster, bool kill = false, int damage = -1, bool push = true)
        {
            base.DealDamageToMonster(monster, kill, damage, push);
            if (Mod.instance.CurrentProgress() < 25)
                return;
            ApplyDazeEffect(monster);
        }

        public void ApplyDazeEffect(StardewValley.Monsters.Monster monster)
        {
            if (Mod.instance.eventRegister.ContainsKey("Gravity"))
                return;
            List<int> source = new List<int>();
            for (int index = 0; index < 5; ++index)
            {
                string key = "daze" + index.ToString();
                if (!Mod.instance.eventRegister.ContainsKey(key))
                    source.Add(index);
                else if ((Mod.instance.eventRegister[key] as Daze).victim == monster)
                    return;
            }
            if (source.Count <= 0)
                return;
            Rite rite = Mod.instance.NewRite(false);
            Daze daze = new Daze(getTileLocation(), rite, monster, source.First<int>(), 1);
            if (!MonsterData.CustomMonsters().Contains(monster.GetType()))
            {
                monster.Halt();
                monster.stunTime = 4000;
            }
            daze.EventTrigger();
        }

        public override void SwitchFollowMode()
        {
            base.SwitchFollowMode();
            Buff buff = new Buff("Fortune's Favour", 999999, nameof(Jester), 4);
            buff.buffAttributes[4] = 2;
            buff.which = 184654;
            if (Game1.buffsDisplay.hasBuff(184654))
                return;
            Game1.buffsDisplay.addOtherBuff(buff);
        }

        public override void SwitchDefaultMode()
        {
            base.SwitchDefaultMode();
            foreach (Buff otherBuff in Game1.buffsDisplay.otherBuffs)
            {
                if (otherBuff.which == 184654)
                    otherBuff.removeBuff();
            }
        }
    }
}
