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
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewRoguelike.Extensions;
using StardewRoguelike.VirtualProperties;
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
        private readonly int MaxBoxTimer = 15 * 60;

        public override List<string> MapPaths
        {
            get
            {
                return new() { "custom-defend", "custom-defend2", "custom-egg" };
            }
        }

        public override List<string> GetMusicTracks(MineShaft mine)
        {
            string loadedMap = mine.get_MineShaftLoadedMap().Value;
            if (loadedMap == "custom-defend")
                return new() { "VolcanoMines" };
            else
                return new() { "" };
        }

        public override Vector2? GetSpawnLocation(MineShaft mine)
        {
            string loadedMap = mine.get_MineShaftLoadedMap().Value;
            if (loadedMap == "custom-defend")
                return new(44, 38);
            else if (loadedMap == "custom-defend2")
                return new(6, 4);
            else if (loadedMap == "custom-egg")
                return new(37, 8);
            else
                return null;
        }

        private readonly NetString state = new("active");

        private readonly NetInt boxTimer = new(0);

        private int stateTimer = 0;

        private bool ladderSpawned = false;

        private readonly NetRectangle kothBox = new(Rectangle.Empty);

        private Texture2D kothBoxProgressTexture = null!;

        private readonly Texture2D kothBoxTexture;

        private readonly List<Rectangle> possibleBoxes = new() { };

        private int ticksToSpawnEnemies = -1;

        private readonly int enemiesToSpawnMin = 2;

        private readonly int enemiesToSpawnMax = 3;

        public KOTH() : base()
        {
            kothBoxTexture = new(Game1.graphics.GraphicsDevice, 1, 1);
            kothBoxTexture.SetData(new[] { Color.White });
        }

        protected override void InitNetFields()
        {
            base.InitNetFields();

            NetFields.AddFields(state, boxTimer, kothBox);
        }

        public override void Initialize(MineShaft mine)
        {
            string loadedMap = mine.get_MineShaftLoadedMap().Value;
            if (loadedMap == "custom-defend")
            {
                possibleBoxes.Add(new(50 * 64, 21 * 64, 7 * 64, 6 * 64));
                possibleBoxes.Add(new(55 * 64, 45 * 64, 7 * 64, 8 * 64));
                possibleBoxes.Add(new(21 * 64, 34 * 64, 6 * 64, 6 * 64));

                possibleBoxes.Shuffle(Roguelike.FloorRng);
            }
            else if (loadedMap == "custom-defend2")
            {
                possibleBoxes.Add(new(4 * 64, 7 * 64, 7 * 64, 5 * 64));
                possibleBoxes.Add(new(18 * 64, 19 * 64, 7 * 64, 7 * 64));
                possibleBoxes.Add(new(30 * 64, 30 * 64, 5 * 64, 5 * 64));

                int level = Roguelike.GetLevelFromMineshaft(mine);
                bool isDangerous = level % 48 > Roguelike.DangerousThreshold;

                mine.mapImageSource.Value = isDangerous ? "Maps\\Mines\\mine_slime_dangerous" : "Maps\\Mines\\mine_slime";
            }
            else if (loadedMap == "custom-egg")
            {
                possibleBoxes.Add(new(32 * 64, 38 * 64, 6 * 64, 6 * 64));
                possibleBoxes.Add(new(49 * 64, 18 * 64, 7 * 64, 4 * 64));
                possibleBoxes.Add(new(26 * 64, 16 * 64, 3 * 64, 3 * 64));

                possibleBoxes.Shuffle(Roguelike.FloorRng);
            }

            kothBox.Value = possibleBoxes[0];
            possibleBoxes.RemoveAt(0);
        }

        public void OnRenderedWorld(object? sender, RenderedWorldEventArgs e)
        {
            if (kothBox.Value == Rectangle.Empty || stateTimer % 90 > 45 || kothBox.Value.Contains(Game1.player.Position))
                return;

            e.SpriteBatch.Draw(
                kothBoxTexture,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(kothBox.X, kothBox.Y)),
                kothBox,
                Color.DarkGray * 0.4f,
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
                    data[i] = Color.Green;
                else
                    data[i] = Color.Black;
            }

            kothBoxProgressTexture.SetData(data);
        }

        public void RenderProgressBar(object? sender, RenderedHudEventArgs e)
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

        public void SpawnRewardChest(MineShaft mine)
        {
            string loadedMap = mine.get_MineShaftLoadedMap().Value;

            if (loadedMap == "custom-defend")
            {
                mine.createLadderAt(new(44, 38));
                mine.SpawnLocalChest(new(44, 35));
            }
            else if (loadedMap == "custom-defend2")
            {
                mine.createLadderAt(new(35, 33));
                mine.SpawnLocalChest(new(35, 31));
            }
            else if (loadedMap == "custom-egg")
            {
                mine.createLadderAt(new(39, 32));
                mine.SpawnLocalChest(new(39, 30));
            }

            ladderSpawned = true;
        }

        public Monster GetMonsterForKOTH(MineShaft mine, Vector2 spawnTile)
        {
            double monsterChance = Game1.random.NextDouble();
            string loadedMap = mine.get_MineShaftLoadedMap().Value;
            int level = Roguelike.GetLevelFromMineshaft(mine);
            float difficulty = BossFloor.GetLevelDifficulty(level);

            if (loadedMap == "custom-defend")
            {
                Monster monster;
                if (monsterChance < 0.2)
                    monster = new HotHead(spawnTile * 64f);
                else if (monsterChance < 0.4)
                {
                    monster = new GreenSlime(spawnTile * 64f, 0);
                    ((GreenSlime)monster).makeTigerSlime();
                }
                else if (monsterChance < 0.6)
                    monster = new Bat(spawnTile * 64f, -556);
                else if (monsterChance < 0.9)
                    monster = new Bat(spawnTile * 64f, -555);
                else
                    monster = new DwarvishSentry(spawnTile * 64f);

                monster.DamageToFarmer = (int)Math.Round(8 * difficulty);
                monster.MaxHealth = (int)Math.Round(level * 3 * difficulty);
                monster.Health = monster.MaxHealth;
                monster.resilience.Value = 2;

                return monster;
            }
            else if (loadedMap == "custom-defend2")
            {
                Monster monster = mine.BuffMonsterIfNecessary(new GreenSlime(spawnTile * 64f));
                Roguelike.AdjustMonster(mine, ref monster);
                return monster;
            }
            else if (loadedMap == "custom-egg")
            {
                Monster monster = mine.BuffMonsterIfNecessary(mine.getMonsterForThisLevel(mine.mineLevel, (int)spawnTile.X, (int)spawnTile.Y));
                Roguelike.AdjustMonster(mine, ref monster);
                return monster;
            }

            throw new Exception("this path shouldn't be reached");

        }

        public void SpawnEnemies(MineShaft mine)
        {
            if (kothBox.Value == Rectangle.Empty)
                return;

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
                if (!mine.isTileLocationTotallyClearAndPlaceable((int)randomTile.X, (int)randomTile.Y) || kothBox.Value.Contains(randomTile * 64f))
                    continue;

                // spawn enemy
                Monster monster = GetMonsterForKOTH(mine, randomTile);

                mine.characters.Add(monster);

                enemiesToSpawn--;
            }
        }

        public override void PlayerEntered(MineShaft mine)
        {
            base.PlayerEntered(mine);
            Game1.chatBox.addMessage(I18n.ChallengeFloor_KOTH_WelcomeMessage(), Color.Gold);

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

            if (state.Value == "over" && !ladderSpawned)
            {
                kothBox.Value = Rectangle.Empty;
                SpawnRewardChest(mine);
            }
            else if (state.Value == "over" && ladderSpawned)
                return;

            bool contains = false;
            foreach (Farmer farmer in mine.farmers)
            {
                if (kothBox.Value.Contains(farmer.Position))
                {
                    contains = true;
                    break;
                }
            }

            if (contains && ticksToSpawnEnemies == -1)
                ticksToSpawnEnemies = 30;

            if (kothBox.Value != Rectangle.Empty && contains && stateTimer % 15 == 0)
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
                    kothBox.Value = possibleBoxes[0];
                    possibleBoxes.RemoveAt(0);
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
