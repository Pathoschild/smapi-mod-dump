/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley.Locations;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using StardewValley.Monsters;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using System;
using StardewValley.Objects;
using StardewRoguelike.TerrainFeatures;

namespace StardewRoguelike.ChallengeFloors
{
    internal class Speedrun : ChallengeBase
    {
        public override List<string> MapPaths => new() { "custom-speedrun" };

        public override List<string> MusicTracks
        {
            get
            {
                if (ReachedEnd || !Started)
                    return new() { "none" };

                return new() { "honkytonky" };
            }
        }

        private readonly int BouldersToSpawn = 9;

        private readonly int SlimesToSpawn = 16;

        private readonly int XTileStart = 7;

        private readonly int XTileEnd = 84;

        private readonly List<(Vector2, Vector2)> SpeedPadSpawns = new()
        {
            (new(8, 13), new(10, 10)),
            (new(18, 12), new(21, 15)),
            (new(29, 12), new(31, 15)),
            (new(41, 11), new(41, 9)),
            (new(51, 8), new(51, 13)),
            (new(64, 11), new(62, 15)),
            (new(75, 10), new(77, 16))
        };

        private readonly List<Vector2> BoulderSpawns = new()
        {
            new(12, 11),
            new(16, 15),
            new(26, 12),
            new(32, 13),
            new(37, 9),
            new(49, 10),
            new(58, 13),
            new(69, 13),
            new(86, 10)
        };

        private bool Started = false;

        private bool ReachedEnd = false;

        private double msCounter = 0;

        private int SecondsTaken = 0;

        public Speedrun() : base() { }

        public override void Initialize(MineShaft mine)
        {
            SpawnSpeedpads(mine);
            SpawnBoulders(mine);
            SpawnSlimes(mine);
        }

        public override void Update(MineShaft mine, GameTime time)
        {
            if (Game1.player.currentLocation != mine)
                return;

            if (!Started)
            {
                if (Game1.player.getTileX() >= XTileStart)
                    Started = true;
                else
                    return;
            }

            if (ReachedEnd)
                return;

            if (msCounter >= 1000)
            {
                SecondsTaken++;
                msCounter = 0;
            }
            msCounter += time.ElapsedGameTime.TotalMilliseconds;

            if (Game1.player.getTileX() >= XTileEnd)
                GameOver(mine);
        }

        public void GameOver(MineShaft mine)
        {
            ReachedEnd = true;

            Game1.playSound("discoverMineral");

            int gemReward;
            int goldAmount;

            if (SecondsTaken <= 10 || (SecondsTaken == 11 && msCounter < 100))
            {
                gemReward = 72;
                goldAmount = 40;
            }
            else if (SecondsTaken <= 12 || (SecondsTaken == 13 && msCounter < 100))
            {
                gemReward = 64;
                goldAmount = 35;
            }
            else if (SecondsTaken <= 14 || (SecondsTaken == 15 && msCounter < 100))
            {
                gemReward = 70;
                goldAmount = 30;
            }
            else
            {
                gemReward = 68;
                goldAmount = 20;
            }

            Vector2 chestSpot = new(89, 11);
            List<Item> chestItems = new()
            {
                new StardewValley.Object(gemReward, 1),
                new StardewValley.Object(384, goldAmount)
            };
            Chest chest = new(0, chestItems, chestSpot)
            {
                Tint = Color.White
            };

            mine.overlayObjects.Add(chestSpot, chest);

        }

        public void RenderTimer(object sender, RenderedHudEventArgs e)
        {
            string timeText = $"{SecondsTaken}.{Math.Min((int)(msCounter / 100), 9)}s";
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
        }

        public override void PlayerEntered(MineShaft mine)
        {
            base.PlayerEntered(mine);
            Game1.chatBox.addMessage("Get to the end as fast as you can!", Color.Gold);

            ModEntry.Events.Display.RenderedHud += RenderTimer;
        }

        public override void PlayerLeft(MineShaft mine)
        {
            base.PlayerLeft(mine);
            Dispose();
        }

        private void SpawnSpeedpads(MineShaft mine)
        {
            foreach (var (pad1, pad2) in SpeedPadSpawns)
            {
                bool firstIsBad = Roguelike.FloorRng.NextDouble() < 0.5;
                mine.terrainFeatures.Add(pad1, new SpeedPad(firstIsBad));
                mine.terrainFeatures.Add(pad2, new SpeedPad(!firstIsBad));
            }
        }

        private void SpawnBoulders(MineShaft mine)
        {
            int whichClump = ((Roguelike.FloorRng.NextDouble() < 0.5) ? 752 : 754);
            int mineArea = mine.getMineArea();

            for (int i = 0; i < BouldersToSpawn; i++)
            {
                Vector2 boulderTile = BoulderSpawns[Roguelike.FloorRng.Next(BoulderSpawns.Count)];
                BoulderSpawns.Remove(boulderTile);

                if (mineArea == 40)
                {
                    if (mine.GetAdditionalDifficulty() > 0)
                    {
                        whichClump = 600;
                        if (Roguelike.FloorRng.NextDouble() < 0.1)
                            whichClump = 602;
                    }
                    else
                        whichClump = ((Roguelike.FloorRng.NextDouble() < 0.5) ? 756 : 758);
                }
                mine.resourceClumps.Add(new ResourceClump(whichClump, 2, 2, boulderTile));
            }
        }

        private void SpawnSlimes(MineShaft mine)
        {
            int toSpawn = SlimesToSpawn;
            while (toSpawn > 0)
            {
                Vector2 tile = new(Roguelike.FloorRng.Next(mine.Map.Layers[0].LayerWidth), Roguelike.FloorRng.Next(mine.Map.Layers[0].LayerHeight));
                if (!mine.isTileClearForMineObjects(tile) || mine.getDistanceFromStart((int)tile.X, (int)tile.Y) < 7)
                    continue;

                Monster slime = new GreenSlime(tile * 64f, mine.mineLevel);
                slime = mine.BuffMonsterIfNecessary(slime);
                mine.characters.Add(slime);

                toSpawn--;
            }
        }

        public override void Dispose()
        {
            ModEntry.Events.Display.RenderedHud -= RenderTimer;
        }
    }
}
