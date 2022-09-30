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
using StardewModdingAPI.Events;
using StardewRoguelike.Extensions;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.ChallengeFloors
{
    public class KOTH : ChallengeBase
    {
        private readonly int MaxBoxTimer = 20 * 60;

        public override List<string> MapPaths => new() { "custom-defend" };

        public override List<string> MusicTracks => new() { "VolcanoMines" };

        public override Vector2? SpawnLocation => new(44, 38);

        private readonly NetString state = new("initialize");

        private readonly NetInt boxTimer = new(0);

        private string oldState;

        private int stateTimer = 0;

        private bool ladderSpawned = false;

        private Rectangle kothBox = Rectangle.Empty;

        private Texture2D kothBoxProgressTexture;

        private readonly Texture2D kothBoxTexture;

        private readonly List<string> possibleBoxes = new() { "box1", "box2", "box3" };

        private int ticksToSpawnEnemies = -1;

        private readonly int enemiesToSpawnMin = 2;

        private readonly int enemiesToSpawnMax = 3;

        public KOTH() : base()
        {
            kothBoxTexture = new(Game1.graphics.GraphicsDevice, 1, 1);
            kothBoxTexture.SetData(new[] { Color.White });
        }

        protected override void initNetFields()
        {
            base.initNetFields();

            NetFields.AddFields(state, boxTimer);
        }

        public void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (kothBox == Rectangle.Empty || stateTimer % 90 > 45 || kothBox.Contains(Game1.player.Position))
                return;

            e.SpriteBatch.Draw(
                kothBoxTexture,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(kothBox.X, kothBox.Y)),
                kothBox,
                Color.Aqua * 0.2f,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
        }

        public void MakeBoxProgressBar(int Progress, int MaxProgress)
        {
            kothBoxProgressTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1000.ToUIScale(), 30.ToUIScale());
            Color[] data = new Color[kothBoxProgressTexture.Width * kothBoxProgressTexture.Height];
            kothBoxProgressTexture.GetData(data);
            for (int i = 0; i < data.Length; i++)
            {
                if (i <= kothBoxProgressTexture.Width || i % kothBoxProgressTexture.Width == kothBoxProgressTexture.Width - 1)
                    data[i] = new Color(1f, 0.5f, 0.5f);
                else if (data.Length - i < kothBoxProgressTexture.Width || i % kothBoxProgressTexture.Width == 0)
                    data[i] = new Color(0.5f, 0, 0);
                else if (i % kothBoxProgressTexture.Width / (float)kothBoxProgressTexture.Width < (float)Progress / MaxProgress)
                    data[i] = Color.LimeGreen;
                else
                    data[i] = Color.Black;
            }

            kothBoxProgressTexture.SetData(data);
        }

        public void RenderProgressBar(object sender, RenderedHudEventArgs e)
        {
            if (state.Value == "over")
                ModEntry.Events.Display.RenderedHud -= RenderProgressBar;

            MakeBoxProgressBar(boxTimer.Value, MaxBoxTimer);

            int drawHeight = Game1.uiViewport.Height - 150;

            IClickableMenu.drawTextureBox(
                e.SpriteBatch,
                (Game1.uiViewport.Width / 2) - kothBoxProgressTexture.Width / 2 - 10,
                drawHeight - 10,
                kothBoxProgressTexture.Width + 20,
                kothBoxProgressTexture.Height + 20,
                Color.White
            );

            e.SpriteBatch.Draw(kothBoxProgressTexture, new Vector2((Game1.uiViewport.Width / 2) - kothBoxProgressTexture.Width / 2, drawHeight), null, Color.White);
        }

        public static void SpawnRewardChest(MineShaft mine)
        {
            mine.SpawnLocalChest(new(44, 35));
        }

        public void SpawnEnemies(MineShaft mine)
        {
            if (kothBox == Rectangle.Empty)
                return;

            float difficulty = BossFloor.GetLevelDifficulty(mine);
            int level = Roguelike.GetLevelFromMineshaft(mine);

            int enemiesToSpawn = Game1.random.Next(enemiesToSpawnMin * Game1.getOnlineFarmers().Count, enemiesToSpawnMax * Game1.getOnlineFarmers().Count);
            enemiesToSpawn += (level / 6) / 2;
            if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                enemiesToSpawn += 2;

            int padding = 4;
            Rectangle spawnRect = new((kothBox.X / 64) - padding, (kothBox.Y / 64) - padding, (kothBox.Width / 64) + (padding * 2), (kothBox.Height / 64) + (padding * 2));

            while (enemiesToSpawn > 0)
            {
                Vector2 randomTile = new(
                    Game1.random.Next(spawnRect.X, spawnRect.X + spawnRect.Width),
                    Game1.random.Next(spawnRect.Y, spawnRect.Y + spawnRect.Height)
                );
                if (!mine.isTileLocationTotallyClearAndPlaceable((int)randomTile.X, (int)randomTile.Y) || kothBox.Contains(randomTile * 64f))
                    continue;

                // spawn enemy
                Monster monster;
                double monsterChance = Game1.random.NextDouble();

                if (monsterChance < 0.2)
                    monster = new HotHead(randomTile * 64f);
                else if (monsterChance < 0.4)
                {
                    monster = new GreenSlime(randomTile * 64f, 0);
                    (monster as GreenSlime).makeTigerSlime();
                }
                else if (monsterChance < 0.6)
                    monster = new Bat(randomTile * 64f, -556);
                else if (monsterChance < 0.9)
                    monster = new Bat(randomTile * 64f, -555);
                else
                    monster = new DwarvishSentry(randomTile * 64f);

                monster.DamageToFarmer = (int)Math.Round(8 * difficulty);
                monster.MaxHealth = (int)Math.Round(level * 3 * difficulty);
                monster.Health = monster.MaxHealth;
                monster.resilience.Value = 2;

                Roguelike.AdjustMonster(mine, ref monster);

                mine.characters.Add(monster);

                enemiesToSpawn--;
            }
        }

        public override void PlayerEntered(MineShaft mine)
        {
            base.PlayerEntered(mine);
            Game1.chatBox.addMessage("Reclaim the areas that the monsters have taken over!", Color.Gold);

            if (state.Value != "over")
            {
                ModEntry.Events.Display.RenderedWorld += OnRenderedWorld;
                ModEntry.Events.Display.RenderedHud += RenderProgressBar;
            }
        }

        public override void PlayerLeft(MineShaft mine)
        {
            base.PlayerLeft(mine);
            Dispose();
        }

        public override bool ShouldSpawnLadder(MineShaft mine)
        {
            return state.Value == "over";
        }

        public override void Update(MineShaft mine, GameTime time)
        {
            if (!Game1.shouldTimePass())
                return;

            stateTimer++;

            if (state.Value == "box1" && kothBox == Rectangle.Empty)
                kothBox = new(50 * 64, 21 * 64, 7 * 64, 6 * 64);
            else if (state.Value == "box2" && kothBox == Rectangle.Empty)
                kothBox = new(55 * 64, 45 * 64, 7 * 64, 8 * 64);
            else if (state.Value == "box3" && kothBox == Rectangle.Empty)
                kothBox = new(21 * 64, 34 * 64, 6 * 64, 6 * 64);

            if (state != oldState)
            {
                kothBox = Rectangle.Empty;
                oldState = state;
                stateTimer = 0;
            }

            if (!Context.IsMainPlayer)
                return;

            if (ticksToSpawnEnemies > 0)
            {
                ticksToSpawnEnemies--;

                if (ticksToSpawnEnemies == 0)
                {
                    SpawnEnemies(mine);
                    ticksToSpawnEnemies = Game1.random.Next(60 * 9, 60 * 12);
                }
            }

            if (state.Value == "initialize")
            {
                int randomBoxIdx = Game1.random.Next(possibleBoxes.Count);
                state.Value = possibleBoxes[randomBoxIdx];
                possibleBoxes.RemoveAt(randomBoxIdx);
            }

            if (state.Value == "over" && !ladderSpawned)
            {
                mine.createLadderAt((Vector2)SpawnLocation);
                SpawnRewardChest(mine);
                ladderSpawned = true;
            }
            else if (state.Value == "over" && ladderSpawned)
                return;

            bool contains = false;
            foreach (Farmer farmer in mine.farmers)
            {
                if (kothBox.Contains(farmer.Position))
                {
                    contains = true;
                    break;
                }
            }

            if (contains && ticksToSpawnEnemies == -1)
                ticksToSpawnEnemies = 30;

            if (kothBox != Rectangle.Empty && contains && stateTimer % 15 == 0)
            {
                boxTimer.Value += 15;

                if (stateTimer % 60 == 0)
                    mine.playSound("sell");
            }

            if (boxTimer.Value >= MaxBoxTimer)
            {
                boxTimer.Value = 0;
                mine.playSound("discoverMineral");
                if (possibleBoxes.Count == 0)
                    state.Value = "over";
                else
                {
                    int randomBoxIdx = Game1.random.Next(possibleBoxes.Count);
                    state.Value = possibleBoxes[randomBoxIdx];
                    possibleBoxes.RemoveAt(randomBoxIdx);
                }
            }
        }

        public override void Dispose()
        {
            ModEntry.Events.Display.RenderedWorld -= OnRenderedWorld;
            ModEntry.Events.Display.RenderedHud -= RenderProgressBar;
        }
    }
}
