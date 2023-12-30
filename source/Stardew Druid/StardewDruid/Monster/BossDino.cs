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
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewDruid.Monster
{
    public class BossDino : DinoMonster
    {
        public List<string> ouchList;
        public List<string> dialogueList;
        public Queue<Vector2> burningQueue;
        public int burnDamage;
        public Texture2D hatsTexture;
        public Rectangle hatSourceRect;
        public Vector2 hatOffset;
        public int hatIndex;
        public Dictionary<int, Rectangle> hatSourceRects;
        public Dictionary<int, Vector2> hatOffsets;
        public Dictionary<int, float> hatRotates;
        public Dictionary<int, Vector2> hatRotateOffsets;
        public bool firingAction;
        public bool defeated;
        public bool hardMode;
        public int dialogueTimer;

        public BossDino(Vector2 vector, int combatModifier)
          : base(vector* 64f)
        {
            Health = combatModifier * 12;
            MaxHealth = Health;
            focusedOnFarmers = true;
            DamageToFarmer = (int)(combatModifier * 0.15);
            objectsToDrop.Clear();
            burnDamage = (int)(combatModifier * 0.1);
            burningQueue = new Queue<Vector2>();
            hideShadow.Value = true;
            ouchList = new List<string>()
            {
                "ouch",
                "croak",
                "can't you aim for the helmet?"
            };
            dialogueList = new List<string>()
            {
                "Why am I here",
                "The power of the Stars has seeped into the land",
                "I should be at rest, I should be...",
                "Surrender, and I'll give you a pony ride",
                "STOP MOVING. JUST BURN.",
                "My helmet provides +3 Intelligence!"
            };
            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");
            hatIndex = 345;
            hatSourceRects = new Dictionary<int, Rectangle>()
            {
                [2] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex, 20, 20),
                [1] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 12, 20, 20),
                [3] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 24, 20, 20),
                [0] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 36, 20, 20)
            };
            hatOffsets = new Dictionary<int, Vector2>()
            {
                [2] = new Vector2(-16f, 0.0f),
                [1] = new Vector2(36f, 2f),
                [3] = new Vector2(-68f, 4f),
                [0] = new Vector2(-16f, -32f)
            };
            hatRotates = new Dictionary<int, float>()
            {
                [1] = 6f,
                [3] = 0.4f
            };
            hatRotateOffsets = new Dictionary<int, Vector2>()
            {
                [1] = new Vector2(-4f, -2f),
                [3] = new Vector2(8f, -12f)
            };
        }

        public void HardMode()
        {
            Health = Health * 3;
            Health = Health / 2;
            MaxHealth = Health;
            DamageToFarmer = DamageToFarmer * 3;
            DamageToFarmer = DamageToFarmer / 2;
            hardMode = true;
        }

        public override Rectangle GetBoundingBox()
        {
            Vector2 position = Position;
            return new Rectangle((int)position.X - 48, (int)position.Y - 32, 160, 128);
        }

        public override List<Item> getExtraDropItems() => new List<Item>();

        public override int takeDamage(
          int damage,
          int xTrajectory,
          int yTrajectory,
          bool isBomb,
          double addedPrecision,
          Farmer who)
        {
            int damage1 = Math.Max(1, damage - resilience.Value);
            Health = Health - damage1;
            if (Game1.random.Next(5) == 0)
                setTrajectory(xTrajectory, yTrajectory);
            if (Health <= 0)
            {
                deathAnimation();
                defeated = true;
            }
            if (dialogueTimer <= 0 && Game1.random.Next(4) == 0)
            {
                showTextAboveHead(ouchList[Game1.random.Next(ouchList.Count)], -1, 2, 3000, 0);
                dialogueTimer = 300;
            }
            return damage1;
        }

        public override void draw(SpriteBatch b)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
                return;
            b.Draw(Sprite.Texture, getLocalPosition(Game1.viewport)+new Vector2(56f, (float)(16 + yJumpOffset)), new Rectangle?(Sprite.SourceRect), Color.White* 0.7f, rotation, new Vector2(16f, 16f), 7f, flip ? (SpriteEffects)1 : 0, 0.99f);
            hatSourceRect = hatSourceRects[((NetFieldBase<int, NetInt>)facingDirection).Value];
            hatOffset = hatOffsets[((NetFieldBase<int, NetInt>)facingDirection).Value];
            float num1 = 0.0f;
            if (firingAction)
            {
                if (hatRotates.ContainsKey(((NetFieldBase<int, NetInt>)facingDirection).Value))
                {
                    num1 = hatRotates[FacingDirection];
                    hatOffset = hatOffset + hatRotateOffsets[FacingDirection];
                }
                else
                    hatOffset = hatOffset- new Vector2(0.0f, 4f);
            }
            if (((NetFieldBase<int, NetInt>)facingDirection).Value % 2 == 0)
            {
                switch (Sprite.currentFrame % 4)
                {
                    case 1:
                        hatOffset = hatOffset+new Vector2(4f, 0.0f);
                        break;
                    case 3:
                        hatOffset = hatOffset-new Vector2(4f, 0.0f);
                        break;
                }
            }
            else
            {
                switch (Sprite.currentFrame % 4)
                {
                    case 1:
                        hatOffset = hatOffset+new Vector2(0.0f, 4f);
                        break;
                    case 3:
                        hatOffset = hatOffset+new Vector2(0.0f, 4f);
                        break;
                }
            }
            float num2 = 0.991f;
            if (((NetFieldBase<int, NetInt>)facingDirection).Value == 0)
                num2 = 0.989f;
            Vector2 vector2 = getLocalPosition(Game1.viewport)+ new Vector2(56f, (float)(16 + yJumpOffset)) +  hatOffset;
            b.Draw(hatsTexture, vector2, new Rectangle?(hatSourceRect), Color.White*0.6f, num1, new Vector2(8f, 12f), 8f, flip ? (SpriteEffects)1 : 0, num2);
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            --dialogueTimer;
            TimeSpan elapsedGameTime;
            if (attackState.Value == 1)
            {
                IsWalkingTowardPlayer = false;
                Halt();
            }
            else if (withinPlayerThreshold())
            {
                IsWalkingTowardPlayer = true;
                firingAction = false;
            }
            else
            {
                IsWalkingTowardPlayer = false;
                nextChangeDirectionTime -= time.ElapsedGameTime.Milliseconds;
                elapsedGameTime = time.ElapsedGameTime;
                int milliseconds = elapsedGameTime.Milliseconds;
                nextWanderTime = nextWanderTime - milliseconds;
                if (nextChangeDirectionTime < 0)
                {
                    nextChangeDirectionTime = Game1.random.Next(500, 1000);
                    FacingDirection = FacingDirection + ((Game1.random.Next(0, 3) - 1) + 4) % 4;
                }
                if (nextWanderTime < 0)
                {
                    if (wanderState)
                        nextWanderTime = Game1.random.Next(1000, 2000);
                    else
                        nextWanderTime = Game1.random.Next(1000, 3000);
                    wanderState = !wanderState;
                }
                if (wanderState)
                {
                    moveLeft = moveUp = moveRight = moveDown = false;
                    tryToMoveInDirection(((NetFieldBase<int, NetInt>)facingDirection).Value, false, DamageToFarmer, isGlider);
                }
                firingAction = false;
            }
            elapsedGameTime = time.ElapsedGameTime;
            int milliseconds1 = elapsedGameTime.Milliseconds;
            timeUntilNextAttack = timeUntilNextAttack - milliseconds1;
            if (attackState.Value == 0 && (double)Vector2.Distance(Position, Game1.player.Position) <= 560.0)
            {
                firing.Set(false);
                if (timeUntilNextAttack >= 0)
                    return;
                timeUntilNextAttack = 0;
                attackState.Set(1);
                totalFireTime = 1500;
                currentLocation.playSound("croak", (NetAudio.SoundContext)0);
                if (dialogueTimer <= 0 && Game1.random.Next(3) == 0)
                {
                    showTextAboveHead(dialogueList[Game1.random.Next(dialogueList.Count)], -1, 2, 3000, 0);
                    dialogueTimer = 300;
                }
            }
            else
            {
                if (totalFireTime <= 0)
                    return;
                if (!firing)
                {
                    Farmer player = Player;
                    if (player != null)
                        faceGeneralDirection(player.Position, 0, false);
                }

                elapsedGameTime = time.ElapsedGameTime;

                int milliseconds2 = elapsedGameTime.Milliseconds;

                totalFireTime = totalFireTime - milliseconds2;

                if (!firingAction)
                {
                    if (!firing.Value)
                    {
                        firing.Set(true);
                        currentLocation.playSound("furnace", (NetAudio.SoundContext)0);
                    }

                    Vector2 position = Position;

                    Vector2 zero1 = Vector2.Zero;

                    Vector2 zero2 = Vector2.Zero;

                    float num1 = 0.0f;

                    float num2 = 998f;

                    bool flag = false;

                    Vector2 vector2_1;

                    Vector2 vector2_2;

                    Vector2 vector2_3;

                    switch (FacingDirection)
                    {
                        case 0:
                            Sprite.AnimateUp(Game1.currentGameTime, 0, "");
                            num1 = -1.57079637f;
                            num2 = 0.0001f;
                            vector2_1 = Position + new Vector2(0.0f, -300f);
                            vector2_2 = Position + new Vector2(0.0f, -500f);
                            vector2_3 = Position + new Vector2(-200f, -300f);
                            break;
                        case 1:
                            vector2_1 = Position + new Vector2(300f, 0.0f);
                            vector2_2 = Position + new Vector2(500f, 0.0f);
                            vector2_3 = Position + new Vector2(100f, 0.0f);
                            break;
                        case 2:
                            num1 = 1.57079637f;
                            vector2_1 = Position + new Vector2(0.0f, 300f);
                            vector2_2 = Position + new Vector2(0.0f, 500f);
                            vector2_3 = Position + new Vector2(-200f, 300f);
                            break;
                        default:
                            vector2_1 = Position + new Vector2(-300f, 0.0f);
                            vector2_2 = Position + new Vector2(-500f, 0.0f);
                            vector2_3 = Position + new Vector2(-500f, 0.0f);
                            flag = true;
                            break;
                    }

                    burningQueue.Clear();

                    burningQueue.Enqueue(vector2_1);

                    burningQueue.Enqueue(vector2_2);

                    DelayedAction.functionAfterDelay(burningDesert, 800);

                    DelayedAction.functionAfterDelay(burningDesert, 1000);

                    currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(0, 120f, 4, 1, vector2_3, false, flag)
                    {
                        sourceRect = new Rectangle(0, 0, 160, 32),
                        sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "EnergyBeam.png")),
                        scale = 3f,
                        timeBasedMotion = true,
                        layerDepth = num2,
                        rotation = num1
                    });

                    currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(0, 400f, 1, 1, vector2_3, false, flag)
                    {
                        sourceRect = new Rectangle(0, 128, 160, 32),
                        sourceRectStartingPos = new Vector2(0.0f, 128f),
                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "EnergyBeam.png")),
                        scale = 3f,
                        timeBasedMotion = true,
                        layerDepth = num2,
                        alphaFade = 1f / 1000f,
                        rotation = num1,
                        delayBeforeAnimationStart = 500
                    });

                    currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(0, 125f, 4, 1, vector2_2-new Vector2(96f, 96f), false, false)
                    {
                        sourceRect = new Rectangle(0, 0, 64, 64),
                        sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                        texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "EnergyBomb.png")),
                        scale = 4f,
                        timeBasedMotion = true,
                        layerDepth = num2 + 1f,
                        rotationChange = 0.00628f,
                        delayBeforeAnimationStart = 500
                    });

                    nextFireTime = hardMode ? 30 : 50;

                    firingAction = true;

                }
                if (totalFireTime <= 0)
                {
                    totalFireTime = 0;
                    attackState.Set(0);
                    timeUntilNextAttack = Game1.random.Next(1000, 2000);
                    burningQueue.Clear();
                    firingAction = false;
                }
            }
        }

        public void burningDesert()
        {
            if (burningQueue.Count <= 0)
                return;
            currentLocation.explode(burningQueue.Dequeue()/64f, 2, Game1.player, true, 20 + Game1.player.CombatLevel * 2);
        }
    }
}
