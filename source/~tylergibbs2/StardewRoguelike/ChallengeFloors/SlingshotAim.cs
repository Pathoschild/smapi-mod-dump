/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewRoguelike.Extensions;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Collections.Generic;

namespace StardewRoguelike.ChallengeFloors
{
    internal class SlingshotAim : ChallengeBase
    {
        private readonly Vector2 upperLeftCorner = new(8, 12);

        private readonly Vector2 bottomRightCorner = new(28, 16);

        private readonly NetInt floorSecondsLeft = new(30);

        private readonly NetInt wavesKilled = new(0);

        private int tickCounter = 0;

        private bool gameOver = false;

        private bool spawnedFirstWave = false;

        private bool shotProjectile = false;

        public override List<string> MapPaths => new() { "custom-slingshot" };

        protected override void InitNetFields()
        {
            base.InitNetFields();

            NetFields.AddFields(floorSecondsLeft, wavesKilled);
        }

        public override bool ShouldSpawnLadder(MineShaft mine)
        {
            return false;
        }

        public void RenderHud(object? sender, RenderedHudEventArgs e)
        {
            string timeText = I18n.ChallengeFloor_Shared_TimeLeft(seconds: floorSecondsLeft.Value);
            Vector2 textSize = Game1.smallFont.MeasureString(timeText);

            Point timerDrawPos = new(100, 16);

            IClickableMenu.drawTextureBox(
                e.SpriteBatch,
                timerDrawPos.X - 15,
                timerDrawPos.Y - 12,
                (int)textSize.X + 33,
                (int)textSize.Y + 20,
                Color.White
            );

            Utility.drawTextWithShadow(
                e.SpriteBatch,
                timeText,
                Game1.smallFont,
                new Vector2(timerDrawPos.X, timerDrawPos.Y),
                Color.Black
            );

            string wavesText = I18n.ChallengeFloor_Shared_WavesKilled(amount: wavesKilled.Value);
            Vector2 wavesSize = Game1.smallFont.MeasureString(wavesText);

            Point wavesDrawPos = new(100, 40 + (int)textSize.Y);

            IClickableMenu.drawTextureBox(
                e.SpriteBatch,
                wavesDrawPos.X - 15,
                wavesDrawPos.Y - 12,
                (int)wavesSize.X + 33,
                (int)wavesSize.Y + 20,
                Color.White
            );

            Utility.drawTextWithShadow(
                e.SpriteBatch,
                wavesText,
                Game1.smallFont,
                new Vector2(wavesDrawPos.X, wavesDrawPos.Y),
                Color.Black
            );
        }

        public override void PlayerEntered(MineShaft mine)
        {
            base.PlayerEntered(mine);
            Game1.chatBox.addMessage(I18n.ChallengeFloor_SlingshotAim_WelcomeMessage(), Color.Gold);

            ModEntry.Events.Display.RenderedHud += RenderHud;

            Slingshot slingshot = new();
            slingshot.attachments[0] = new(388, 50);

            Vector2 chestSpot = new(6, 23);
            List<Item> chestItems = new()
            {
                slingshot
            };
            Chest chest = new(0, chestItems, chestSpot)
            {
                Tint = Color.White
            };
            mine.overlayObjects.Add(chestSpot, chest);
        }

        public override void PlayerLeft(MineShaft mine)
        {
            base.PlayerLeft(mine);
            Dispose();
        }

        public override void Dispose()
        {
            ModEntry.Events.Display.RenderedHud -= RenderHud;
        }

        public void SpawnWave(MineShaft mine)
        {
            int monstersToSpawn = 6;
            if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                monstersToSpawn++;

            monstersToSpawn *= Game1.getOnlineFarmers().Count;

            for (int i = 0; i < monstersToSpawn; i++)
                SpawnEnemy(mine);
        }

        public void SpawnEnemy(MineShaft mine)
        {
            Vector2 randomTile;
            do
            {
                randomTile = new(
                    Game1.random.Next((int)upperLeftCorner.X, (int)bottomRightCorner.X + 1),
                    Game1.random.Next((int)upperLeftCorner.Y, (int)bottomRightCorner.Y + 1)
                );
            } while (!mine.isTileLocationTotallyClearAndPlaceable(randomTile));

            SlingshotTargetMinion target = new(randomTile * 64f);

            DelayedAction.functionAfterDelay(() =>
            {
                mine.characters.Add(target);
            }, Game1.random.Next(0, 1000));
        }

        public void GameOver(MineShaft mine)
        {
            gameOver = true;
            mine.characters.Filter(c => c is not Monster);

            if (wavesKilled.Value == 0)
                return;

            mine.playSound("discoverMineral");

            // find adjacent free tile to spawn the chest
            Vector2 chestSpot = new(28, 24);

            Color tint = Color.White;

            int gemReward = wavesKilled.Value switch
            {
                1 => 68,
                2 => 68,
                3 => 70,
                4 => 64,
                _ => 72,
            };

            int goldAmount = wavesKilled.Value switch
            {
                1 => 20,
                2 => 25,
                3 => 30,
                4 => 35,
                _ => 40
            };

            List<Item> chestItems = new()
            {
                new SObject(gemReward, 1),
                new SObject(384, goldAmount)
            };

            mine.SpawnLocalChest(new(28, 24), chestItems);
        }

        public override void Update(MineShaft mine, GameTime time)
        {
            if (!Context.IsMainPlayer || !Game1.shouldTimePass() || gameOver)
                return;

            if (!spawnedFirstWave)
            {
                SpawnWave(mine);
                spawnedFirstWave = true;
            }

            if (!shotProjectile)
            {
                if (mine.projectiles.Count > 0)
                    shotProjectile = true;

                return;
            }

            tickCounter++;
            if (tickCounter >= 60)
            {
                if (mine.EnemyCount == 0)
                {
                    wavesKilled.Value++;
                    SpawnWave(mine);
                    mine.playSound("hoeHit");
                }
                else if (floorSecondsLeft.Value == 0)
                    GameOver(mine);
                else
                    floorSecondsLeft.Value--;

                tickCounter = 0;
            }
        }
    }

    internal class SlingshotTargetMinion : Monster
    {
        private static readonly int initialMovingTicks = 120;

        private readonly NetBool seenPlayer = new();

        private bool isMovingRight;

        private int movingTicksLeft = initialMovingTicks;

        private bool fullySpawned = false;

        public SlingshotTargetMinion()
        {
        }

        public SlingshotTargetMinion(Vector2 position)
            : base("Wilderness Golem", position)
        {
            isMovingRight = Game1.random.Next() < 0.5;
            IsWalkingTowardPlayer = false;
            Slipperiness = 3;
            HideShadow = true;
            jitteriness.Value = 0.0;

            Sprite.currentFrame = 16;
            Sprite.loop = false;
            Sprite.UpdateSourceRect();

            Speed = 1;
            Health = 1;
            MaxHealth = 1;
            resilience.Value = 0;
        }


        protected override void initNetFields()
        {
            base.initNetFields();

            NetFields.AddFields(seenPlayer);
            position.Field.AxisAlignedMovement = true;
        }

        protected override void localDeathAnimation()
        {
            currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(46, Position, Color.DarkGray, 10));
            currentLocation.localSound("rockGolemDie");
        }

        protected override void sharedDeathAnimation()
        {
            Game1.createRadialDebris(currentLocation, Sprite.textureName.Value, new Rectangle(0, 576, 64, 64), 32, GetBoundingBox().Center.X, GetBoundingBox().Center.Y, Game1.random.Next(4, 9), (int)getTileLocation().Y);
        }

        public override void noMovementProgressNearPlayerBehavior()
        {
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            if (!seenPlayer.Value)
            {
                if (withinPlayerThreshold(64))
                {
                    currentLocation.playSound("rockGolemSpawn");
                    seenPlayer.Value = true;
                }
                else
                {
                    Sprite.currentFrame = 16;
                    Sprite.loop = false;
                    Sprite.UpdateSourceRect();
                }
            }
            else if (Sprite.currentFrame >= 16)
            {
                Sprite.Animate(time, 16, 8, 75f);
                if (Sprite.currentFrame >= 24)
                {
                    Sprite.loop = true;
                    moveTowardPlayerThreshold.Value = 1;
                    IsWalkingTowardPlayer = true;
                    HideShadow = false;
                    fullySpawned = true;
                }
            }

            if (seenPlayer.Value && fullySpawned)
            {
                if (movingTicksLeft > 0)
                {
                    movingTicksLeft--;

                    if (movingTicksLeft == 0)
                    {
                        isMovingRight = !isMovingRight;
                        movingTicksLeft = initialMovingTicks;
                    }
                }

                if (isMovingRight)
                {
                    moveRight = true;
                    moveLeft = false;
                    moveDown = false;
                    moveUp = false;
                }
                else
                {
                    moveRight = false;
                    moveLeft = true;
                    moveDown = false;
                    moveUp = false;
                }
            }
        }

        protected override void updateMonsterSlaveAnimation(GameTime time)
        {
            if (IsWalkingTowardPlayer)
            {
                if (FacingDirection == 0)
                    Sprite.AnimateUp(time);
                else if (FacingDirection == 3)
                    Sprite.AnimateLeft(time);
                else if (FacingDirection == 1)
                    Sprite.AnimateRight(time);
                else if (FacingDirection == 2)
                    Sprite.AnimateDown(time);
            }
            if (!seenPlayer.Value)
            {
                Sprite.currentFrame = 16;
                Sprite.loop = false;
                Sprite.UpdateSourceRect();
            }
            else if (Sprite.currentFrame >= 16)
            {
                Sprite.Animate(time, 16, 8, 75f);
                if (Sprite.currentFrame >= 24)
                {
                    Sprite.loop = true;
                    Sprite.currentFrame = 0;
                    Sprite.UpdateSourceRect();
                }
            }
        }
    }
}
