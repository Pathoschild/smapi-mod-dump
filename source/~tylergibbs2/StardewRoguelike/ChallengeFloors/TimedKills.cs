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

namespace StardewRoguelike.ChallengeFloors
{
    public class TimedKills : ChallengeBase
    {
        private readonly int monstersToSpawn = 15;

        private bool spawnedMonsters = false;

        private bool gameOver = false;

        private int tickCounter = 0;

        private readonly NetInt floorSecondsLeft = new(60);

        public TimedKills() : base() { }

        protected override void InitNetFields()
        {
            base.InitNetFields();

            NetFields.AddFields(floorSecondsLeft);
        }

        public void SpawnMonsters(MineShaft mine)
        {
            // spawn monsters

            int toSpawn = monstersToSpawn;
            if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                toSpawn += Game1.random.Next(3, 5);

            mine.SpawnMonsters(toSpawn);

            spawnedMonsters = true;
        }

        public void DespawnMonsters(MineShaft mine)
        {
            mine.characters.Filter(c => c is not Monster);
        }

        public void Win(MineShaft mine)
        {
            mine.playSound("Cowboy_Secret");
            mine.createLadderAt(ChallengeFloor.GetSpawnLocation(mine));

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

            mine.SpawnLocalChest(chestSpot);
        }

        public void Lose(MineShaft mine)
        {
            mine.playSound("cowboy_dead");
            DespawnMonsters(mine);
            mine.createLadderAt(ChallengeFloor.GetSpawnLocation(mine));

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

            mine.SpawnLocalChest(chestSpot, new SObject(384, 20));
        }

        public override bool ShouldSpawnLadder(MineShaft mine)
        {
            return gameOver;
        }

        public void RenderTimer(object? sender, RenderedHudEventArgs e)
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
        }

        public override void PlayerEntered(MineShaft mine)
        {
            base.PlayerEntered(mine);
            Game1.chatBox.addMessage(I18n.ChallengeFloor_TimedKills_WelcomeMessage(), Color.Gold);

            ModEntry.Events.Display.RenderedHud += RenderTimer;
        }

        public override void PlayerLeft(MineShaft mine)
        {
            base.PlayerLeft(mine);
            Dispose();
        }

        public override void Update(MineShaft mine, GameTime time)
        {
            if (!Context.IsMainPlayer || gameOver)
                return;

            if (!spawnedMonsters)
                SpawnMonsters(mine);

            tickCounter++;
            if (tickCounter >= 60)
            {
                if (mine.EnemyCount == 0)
                {
                    gameOver = true;
                    Win(mine);
                }
                else if (floorSecondsLeft.Value == 0)
                {
                    gameOver = true;
                    Lose(mine);
                }
                else
                    floorSecondsLeft.Value--;

                tickCounter = 0;
            }
        }

        public override void Dispose()
        {
            ModEntry.Events.Display.RenderedHud -= RenderTimer;
        }
    }
}
