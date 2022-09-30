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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using StardewRoguelike.Extensions;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewRoguelike.UI;

namespace StardewRoguelike.Bosses
{
    public class BossDeathMessage
    {
        public string BossName { get; set; }

        public int KillSeconds { get; set; }

        public BossDeathMessage() { }

        public BossDeathMessage(string bossName, int killSeconds)
        {
            BossName = bossName;
            KillSeconds = killSeconds;
        }
    }

    public static class BossManager
    {
        private static Texture2D healthBarTexture;

        private static int previousHealth;

        public static readonly List<Type> mainBossTypes = new()
        {
            typeof(TutorialSlime),
            typeof(Skelly),
            typeof(ThunderKid),
            typeof(Dragon),
            typeof(BigBug),
            typeof(QueenBee),
            typeof(ShadowKing),
            typeof(Modulosaurus)
        };

        public static readonly List<Type> miscBossTypes = new()
        {
            typeof(HiddenLurker),
            typeof(LoopedSlime)
        };

        public static Vector2 VectorFromDegree(int degrees)
        {
            double radians = DegreesToRadians(degrees);
            return new((float)Math.Cos(radians), (float)Math.Sin(radians));
        }

        public static float VectorToRadians(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static int VectorToDegrees(Vector2 vector)
        {
            return (int)(VectorToRadians(vector) * (180f / Math.PI));
        }

        public static float DegreesToRadians(float degrees)
        {
            return (float)(degrees * (Math.PI / 180));
        }

        public static void MakeBossHealthBar(int Health, int MaxHealth)
        {
            healthBarTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1000.ToUIScale(), 30.ToUIScale());
            Color[] data = new Color[healthBarTexture.Width * healthBarTexture.Height];
            healthBarTexture.GetData(data);
            for (int i = 0; i < data.Length; i++)
            {
                if (i <= healthBarTexture.Width || i % healthBarTexture.Width == healthBarTexture.Width - 1)
                    data[i] = new Color(1f, 0.5f, 0.5f);
                else if (data.Length - i < healthBarTexture.Width || i % healthBarTexture.Width == 0)
                    data[i] = new Color(0.5f, 0, 0);
                else if (i % healthBarTexture.Width / (float)healthBarTexture.Width < (float)Health / MaxHealth)
                    data[i] = Color.DarkRed;
                else
                    data[i] = Color.Black;
            }

            healthBarTexture.SetData(data);
        }

        public static void Death(GameLocation location, Farmer killer, string displayName, Vector2? chestLocation = null)
        {
            TimeSpan span = (TimeSpan)(DateTime.UtcNow - ModEntry.Stats.StartTime);
            int killSeconds = (int)span.TotalSeconds;
            ModEntry.MultiplayerHelper.SendMessage(new BossDeathMessage(displayName, killSeconds), "BossDeath");
            Game1.onScreenMenus.Add(
                new BossKillAnnounceMenu(displayName, killSeconds)
            );

            ModEntry.Stats.BossesDefeated++;

            StopRenderHealthBar();

            location.playSound("Cowboy_Secret");
            Game1.changeMusicTrack("none", true, Game1.MusicContext.Default);
            location.checkForMusic(new GameTime());

            if (chestLocation.HasValue)
            {
                MineShaft mine = location as MineShaft;
                int level = Roguelike.GetLevelFromMineshaft(mine);
                int additionalGold = BossFloor.GetBossIndexForFloor(level) * 5;
                mine.SpawnLocalChest(chestLocation.Value, new StardewValley.Object(384, 20 + additionalGold));
            }
        }

        public static void PlayerWarped(object sender, WarpedEventArgs e)
        {
            IBossMonster boss = null;
            foreach (NPC character in Game1.player.currentLocation.characters)
            {
                if (mainBossTypes.Contains(character.GetType()) || miscBossTypes.Contains(character.GetType()))
                {
                    boss = (IBossMonster)character;
                    break;
                }
            }

            if (boss is not null)
            {
                if (boss.InitializeWithHealthbar)
                    StartRenderHealthBar();
            }
            else
            {
                StopRenderHealthBar();
                if (e.NewLocation is not null)
                    e.NewLocation.checkForMusic(new GameTime());
            }
        }

        public static void StartRenderHealthBar()
        {
            ModEntry.Events.Display.RenderedHud += RenderHealthBar;
            ModEntry.Events.Display.WindowResized += WindowResized;
        }

        public static void StopRenderHealthBar()
        {
            ModEntry.Events.Display.RenderedHud -= RenderHealthBar;
            ModEntry.Events.Display.WindowResized -= WindowResized;
        }

        public static void WindowResized(object sender, WindowResizedEventArgs e)
        {
            Monster boss = null;
            foreach (NPC character in Game1.player.currentLocation.characters)
            {
                if (mainBossTypes.Contains(character.GetType()) || miscBossTypes.Contains(character.GetType()))
                {
                    boss = (Monster)character;
                    break;
                }
            }

            if (boss is null)
                return;

            MakeBossHealthBar(boss.Health, boss.MaxHealth);
        }

        public static void RenderHealthBar(object sender, RenderedHudEventArgs e)
        {
            Monster boss = null;
            foreach (NPC character in Game1.player.currentLocation.characters)
            {
                if (mainBossTypes.Contains(character.GetType()) || miscBossTypes.Contains(character.GetType()))
                {
                    boss = (Monster)character;
                    break;
                }
            }
            if (boss is null)
            {
                StopRenderHealthBar();
                return;
            }

            if (boss.Health != previousHealth)
            {
                previousHealth = boss.Health;
                MakeBossHealthBar(boss.Health, boss.MaxHealth);
            }

            int drawHeight = Game1.uiViewport.Height - 150;

            IClickableMenu.drawTextureBox(
                e.SpriteBatch,
                Game1.uiViewport.Width / 2 - healthBarTexture.Width / 2 - 10,
                drawHeight - 10,
                healthBarTexture.Width + 20,
                healthBarTexture.Height + 20,
                Color.White
            );

            e.SpriteBatch.Draw(healthBarTexture, new Vector2(Game1.uiViewport.Width / 2 - healthBarTexture.Width / 2, drawHeight), null, Color.White);

            e.SpriteBatch.Draw(
                Game1.mouseCursors,
                new Vector2(
                    Game1.uiViewport.Width / 2 - healthBarTexture.Width / 2 - 6,
                    drawHeight + healthBarTexture.Height / 2
                ),
                new Rectangle(192, 324, 7, 10),
                Color.White,
                0f,
                new Vector2(3f, 5f),
                4f + Game1.dialogueButtonScale / 25f,
                SpriteEffects.None,
                -1f
            );

            e.SpriteBatch.Draw(
                Game1.mouseCursors,
                new Vector2(
                    Game1.uiViewport.Width / 2 + healthBarTexture.Width / 2 + 6,
                    drawHeight + healthBarTexture.Height / 2
                ),
                new Rectangle(192, 324, 7, 10),
                Color.White,
                0f,
                new Vector2(3f, 5f),
                4f + Game1.dialogueButtonScale / 25f,
                SpriteEffects.None,
                -1f
            );

            Vector2 nameTextSize = Game1.dialogueFont.MeasureString((boss as IBossMonster).DisplayName);

            Utility.drawTextWithColoredShadow(
                e.SpriteBatch,
                (boss as IBossMonster).DisplayName,
                Game1.dialogueFont,
                new Vector2(
                    Game1.uiViewport.Width / 2 - nameTextSize.X / 2,
                    drawHeight - nameTextSize.Y - 8
                ),
                Color.White,
                Color.Black
            );
        }
    }
}
