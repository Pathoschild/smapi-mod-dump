/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using xTile.Dimensions;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using StardewRoguelike.Extensions;

namespace StardewRoguelike.ChallengeFloors
{
    internal class EggHunt : ChallengeBase
    {
        public readonly static Texture2D festivalTexture = Game1.content.Load<Texture2D>("Maps/Festivals");

        private readonly static Vector2 GoldenEggTile = new(39, 32);

        private bool gameOver = false;

        private int tickCounter = 0;

        private bool didSpawnEggs = false;

        private bool spawnedGoldEgg = false;

        private readonly NetInt eggsFound = new();

        private readonly NetInt floorSecondsLeft = new(45);

        private readonly NetCollection<Egg> eggsOnMap = new();

        public override List<string> MapPaths => new() { "custom-egg" };

        public override List<string> MusicTracks
        {
            get
            {
                if (eggsFound.Value == 0 || floorSecondsLeft.Value == 0)
                    return new() { "none" };

                return new() { "event1" };
            }
        }

        public override Vector2? SpawnLocation => new(37, 8);

        public EggHunt() : base() { }

        protected override void initNetFields()
        {
            base.initNetFields();

            NetFields.AddFields(eggsFound, eggsOnMap, floorSecondsLeft);
        }

        public override bool ShouldSpawnLadder(MineShaft mine) => gameOver;

        public void SpawnEggs(int amount)
        {
            // 67 - 70. 66 is golden egg

            EggTiles.Shuffle();

            int baseIndex = 67;
            int offset = 0;
            for (int i = 0; i < amount; i++)
            {
                Vector2 eggTile = EggTiles[i];
                eggsOnMap.Add(new(eggTile, baseIndex + offset));

                offset++;
                if (offset > 3)
                    offset = 0;
            }

            foreach (Egg goldenEgg in MerchantFloor.PickNFromList(eggsOnMap, 3))
                goldenEgg.Variant = 66;
        }

        public override bool CheckAction(MineShaft mine, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            Vector2 tileVector = new(tileLocation.X, tileLocation.Y);
            Egg toRemove = null;
            foreach (Egg egg in eggsOnMap)
            {
                if (egg.TileLocation == tileVector)
                {
                    toRemove = egg;
                    break;
                }
            }

            if (toRemove is not null)
            {
                eggsOnMap.Remove(toRemove);
                Game1.playSound("coin");
                eggsFound.Value += toRemove.PointValue;
            }

            return false;
        }

        public override bool CheckForCollision(MineShaft mine, Microsoft.Xna.Framework.Rectangle position, Farmer who)
        {
            foreach (Egg egg in eggsOnMap)
            {
                if (egg.IsColliding(position))
                    return true;
            }

            return false;
        }

        public override void DrawBeforeLocation(MineShaft mine, SpriteBatch b)
        {
            foreach (Egg egg in eggsOnMap)
                egg.Draw(b);
        }

        public void RenderHud(object sender, RenderedHudEventArgs e)
        {
            string timeText = $"Time Left: {floorSecondsLeft.Value}";
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

            int totalEggsFound = eggsFound.Value <= 3 ? 0 : eggsFound.Value - 3;  // don't count the first gold egg
            string eggsText = $"Eggs Found: {totalEggsFound}";
            Vector2 eggsSize = Game1.smallFont.MeasureString(eggsText);

            Point wavesDrawPos = new(100, 40 + (int)textSize.Y);

            IClickableMenu.drawTextureBox(
                e.SpriteBatch,
                wavesDrawPos.X - 15,
                wavesDrawPos.Y - 12,
                (int)eggsSize.X + 33,
                (int)eggsSize.Y + 20,
                Color.White
            );

            Utility.drawTextWithShadow(
                e.SpriteBatch,
                eggsText,
                Game1.smallFont,
                new Vector2(wavesDrawPos.X, wavesDrawPos.Y),
                Color.Black
            );
        }

        public override void PlayerEntered(MineShaft mine)
        {
            base.PlayerEntered(mine);
            mine.forceViewportPlayerFollow = false;

            Game1.chatBox.addMessage("Pick up the golden egg in the middle to start the egg hunt!", Color.Gold);

            ModEntry.Events.Display.RenderedHud += RenderHud;
        }

        public override void PlayerLeft(MineShaft mine)
        {
            base.PlayerLeft(mine);
            Dispose();
        }

        public void GameOver(MineShaft mine)
        {
            eggsOnMap.Clear();

            gameOver = true;
            mine.createLadderAt(GoldenEggTile);

            int totalEggsFound = eggsFound.Value <= 3 ? 0 : eggsFound.Value - 3;  // don't count the first gold egg

            mine.playSound("discoverMineral");

            int gemReward;
            int goldAmount;

            if (totalEggsFound <= 10)
            {
                gemReward = 68;
                goldAmount = 25;
            }
            else if (totalEggsFound <= 15)
            {
                gemReward = 70;
                goldAmount = 30;
            }
            else if (totalEggsFound <= 19)
            {
                gemReward = 64;
                goldAmount = 35;
            }
            else
            {
                gemReward = 72;
                goldAmount = 40;
            }

            List<Item> chestItems = new()
            {
                new StardewValley.Object(gemReward, 1),
                new StardewValley.Object(384, goldAmount)
            };

            mine.SpawnLocalChest(new(39, 30), chestItems);
        }

        public override void Update(MineShaft mine, GameTime time)
        {
            if (!Context.IsMainPlayer || !Game1.shouldTimePass() || gameOver)
                return;

            if (!spawnedGoldEgg)
            {
                // Add the golden egg
                eggsOnMap.Add(new(GoldenEggTile, 66));
                spawnedGoldEgg = true;
                return;
            }
            else if (!didSpawnEggs && eggsOnMap.Count == 0)
            {
                double eggPercent = 0.5 + Math.Min(0.5, 0.1 * Game1.getOnlineFarmers().Count);
                SpawnEggs((int)(EggTiles.Count * eggPercent));
                didSpawnEggs = true;
                return;
            }
            else if (!didSpawnEggs)
                return;

            tickCounter++;
            if (tickCounter >= 60)
            {
                if (floorSecondsLeft.Value == 0)
                    GameOver(mine);
                else
                    floorSecondsLeft.Value--;

                tickCounter = 0;
            }
        }

        public override void Dispose()
        {
            ModEntry.Events.Display.RenderedHud -= RenderHud;
        }

        private readonly static List<Vector2> EggTiles = new()
        {
            new(32, 10),
            new(40, 9),
            new(47, 12),
            new(59, 15),
            new(53, 12),
            new(50, 22),
            new(57, 20),
            new(53, 15),
            new(45, 18),
            new(46, 27),
            new(35, 16),
            new(41, 24),
            new(24, 11),
            new(26, 18),
            new(17, 25),
            new(23, 32),
            new(16, 36),
            new(24, 53),
            new(14, 40),
            new(28, 39),
            new(26, 35),
            new(35, 47),
            new(27, 41),
            new(39, 56),
            new(31, 56),
            new(39, 48),
            new(41, 39),
            new(49, 35),
            new(43, 35),
            new(32, 32),
            new(30, 25),
            new(58, 46),
            new(58, 44),
            new(54, 35),
            new(52, 41),
            new(48, 51),
            new(51, 47),
            new(59, 27)
        };
    }

    public class Egg : INetObject<NetFields>
    {
        private readonly NetVector2 _tileLocation = new(Vector2.Zero);

        private readonly NetInt _variant = new(0);

        public Vector2 TileLocation
        {
            get
            {
                return _tileLocation.Value;
            }
            set
            {
                _tileLocation.Value = value;
            }
        }

        public int Variant
        {
            get
            {
                return _variant.Value;
            }
            set
            {
                _variant.Value = value;
            }
        }

        public int PointValue
        {
            get { return Variant == 66 ? 3 : 1; }
        }

        private bool initialized = false;

        private Microsoft.Xna.Framework.Rectangle sourceRect;

        private Microsoft.Xna.Framework.Rectangle drawRect;

        private Microsoft.Xna.Framework.Rectangle boundingRect;

        public NetFields NetFields { get; } = new();

        public Egg()
        {
            InitializeNetFields();
        }

        public Egg(Vector2 tileLocation, int variant) : this()
        {
            TileLocation = tileLocation;
            Variant = variant;
        }

        private void InitializeNetFields()
        {
            this.NetFields.AddFields(_tileLocation, _variant);
        }

        private void Initialize()
        {
            if (TileLocation == Vector2.Zero || Variant == 0)
                return;

            sourceRect = Game1.getSourceRectForStandardTileSheet(EggHunt.festivalTexture, Variant, 16, 16);
            drawRect = new((int)(TileLocation.X * 64), (int)(TileLocation.Y * 64), 64, 64);
            boundingRect = drawRect;

            initialized = true;
        }

        public bool IsColliding(Microsoft.Xna.Framework.Rectangle r)
        {
            if (!initialized)
            {
                Initialize();
                return false;
            }

            return r.Intersects(boundingRect);
        }

        public void Draw(SpriteBatch b)
        {
            if (!initialized)
            {
                Initialize();
                return;
            }

            drawRect.X = boundingRect.X - Game1.viewport.X;
            drawRect.Y = boundingRect.Y + (boundingRect.Height - drawRect.Height) - Game1.viewport.Y;
            b.Draw(EggHunt.festivalTexture, drawRect, sourceRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, drawRect.Y / 10_000f);
        }
    }
}
