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
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewRoguelike.Extensions;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using System.Collections.Generic;

namespace StardewRoguelike.ChallengeFloors
{
    internal class Race : ChallengeBase
    {
        int tickCounter = 0;

        bool spawnedFirstWave = false;

        bool gameOver = false;

        private readonly NetInt floorSecondsLeft = new(60);

        private readonly NetInt wavesKilled = new(0);

        public Race() : base() { }

        protected override void initNetFields()
        {
            base.initNetFields();

            NetFields.AddFields(floorSecondsLeft, wavesKilled);
        }

        public override List<string> MapPaths => new() {
            "3", "21", "23", "custom-1",
            "custom-2", "custom-3", "custom-4",
            "custom-5", "custom-6", "custom-7"
        };

        public void GameOver(MineShaft mine)
        {
            gameOver = true;
            mine.createLadderAt(ChallengeFloor.GetSpawnLocation(mine));

            mine.characters.Filter(c => c is not Monster);

            if (wavesKilled.Value == 0)
                return;

            mine.playSound("discoverMineral");

            // find adjacent free tile to spawn the chest
            Vector2 spawnLocation = ChallengeFloor.GetSpawnLocation(mine);
            Vector2 chestSpot = Vector2.Zero;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    if (mine.isTileLocationTotallyClearAndPlaceable((int)spawnLocation.X + i, (int)spawnLocation.Y + j))
                    {
                        chestSpot = new((int)spawnLocation.X + i, (int)spawnLocation.Y + j);
                        break;
                    }
                }
            }

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
                new StardewValley.Object(gemReward, 1),
                new StardewValley.Object(384, goldAmount)
            };

            mine.SpawnLocalChest(chestSpot, chestItems);
        }

        public override bool ShouldSpawnLadder(MineShaft mine)
        {
            return gameOver;
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

            string wavesText = $"Waves Killed: {wavesKilled.Value}";
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
            Game1.chatBox.addMessage("Kill as many monsters as you can! The more you kill, the better your reward.", Color.Gold);

            ModEntry.Events.Display.RenderedHud += RenderHud;
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
            int monstersToSpawn = 4;
            if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                monstersToSpawn++;

            mine.SpawnMonsters(monstersToSpawn * Game1.getOnlineFarmers().Count);
        }

        public override void Update(MineShaft mine, GameTime time)
        {
            if (!Context.IsMainPlayer || gameOver || !Game1.shouldTimePass())
                return;

            if (!spawnedFirstWave)
            {
                SpawnWave(mine);
                spawnedFirstWave = true;
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
}
