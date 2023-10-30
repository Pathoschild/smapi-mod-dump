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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using StardewRoguelike.Extensions;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewRoguelike.UI;
using StardewRoguelike.VirtualProperties;
using StardewRoguelike.Patches;
using StardewRoguelike.Netcode;

namespace StardewRoguelike.Bosses
{
    public static class BossManager
    {
        private static Texture2D? healthBarTexture = null;

        private static int PreviousHealth;

        public static readonly List<List<Type>> mainBossTypes = new()
        {
            new() { typeof(TutorialSlime) },
            new() { typeof(Skelly) },
            new() { typeof(ThunderKid) },
            new() { typeof(Dragon) },
            new() { typeof(BigBug) },
            new() { typeof(QueenBee) },
            new() { typeof(ShadowKing) },
            new() { typeof(Modulosaurus) }
        };

        public static readonly List<Type> miscBossTypes = new()
        {
            typeof(HiddenLurker),
            typeof(LoopedSlime)
        };

        public static List<Type> GetFlattenedBosses()
        {
            var flattened = new List<Type>();
            foreach (var bossList in mainBossTypes)
                flattened.AddRange(bossList);

            return flattened;
        }

        internal static int GetBaseDamageToFarmer(MineShaft mine, Type boss)
        {
            int level = Roguelike.GetLevelFromMineshaft(mine);
            if (level > Constants.ScalingOrder[^1] + 6)
                return 27;

            if (boss == typeof(TutorialSlime))
                return 6;
            else if (boss == typeof(Skelly))
                return 10;
            else if (boss == typeof(ThunderKid))
                return 10;
            else if (boss == typeof(Dragon))
                return 25;
            else if (boss == typeof(BigBug))
                return 25;
            else if (boss == typeof(QueenBee))
                return 25;
            else if (boss == typeof(ShadowKing))
                return 30;
            else if (boss == typeof(Modulosaurus))
                return 25;
            else if (boss == typeof(HiddenLurker))
                return 17;
            else if (boss == typeof(LoopedSlime))
                return 20;
            else
                throw new Exception("Invalid boss passed.");
        }

        internal static int GetBaseHealth(MineShaft mine, Type boss)
        {
            int level = Roguelike.GetLevelFromMineshaft(mine);
            if (level > Constants.ScalingOrder[^1] + 6)
                return 3000;

            if (boss == typeof(TutorialSlime))
                return 400;
            else if (boss == typeof(Skelly))
                return 900;
            else if (boss == typeof(ThunderKid))
                return 1150;
            else if (boss == typeof(Dragon))
                return 900;
            else if (boss == typeof(BigBug))
                return 1680;
            else if (boss == typeof(QueenBee))
                return 1700;
            else if (boss == typeof(ShadowKing))
                return 5000;
            else if (boss == typeof(Modulosaurus))
                return 2850;
            else if (boss == typeof(HiddenLurker))
                return GetBaseDamageToFarmer(mine, boss) * (Roguelike.HardMode ? 10 : 7);
            else if (boss == typeof(LoopedSlime))
                return 3000;
            else
                throw new Exception("Invalid boss passed.");
        }

        private static Color GetColorGradient(Color from, Color to, float gradient)
        {
            return new(
                (int)(from.R > to.R ? from.R - (from.R - to.R) * gradient : from.R + (to.R - from.R) * gradient),
                (int)(from.G > to.G ? from.G - (from.G - to.G) * gradient : from.G + (to.G - from.G) * gradient),
                (int)(from.B > to.B ? from.B - (from.B - to.B) * gradient : from.B + (to.B - from.B) * gradient)
            );
        }

        public static void MakeBossHealthBar(int Health, int MaxHealth)
        {
            healthBarTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1000.ToUIScale(), 30.ToUIScale());
            Color[] data = new Color[healthBarTexture.Width * healthBarTexture.Height];
            healthBarTexture.GetData(data);

            Color healthColor;
            if (Health > MaxHealth * 0.33f)
            {
                float gradient = 1 - (Health - MaxHealth * 0.33f) / (MaxHealth * 0.66f);
                healthColor = GetColorGradient(Color.Green, Color.Gold, gradient);
            }
            else
            {
                float gradient = 1 - (Health / (MaxHealth * 0.33f));
                healthColor = GetColorGradient(Color.Gold, Color.DarkRed, gradient);
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (i % healthBarTexture.Width / (float)healthBarTexture.Width < (float)Health / MaxHealth)
                    data[i] = healthColor;
                else
                    data[i] = Color.Black;
            }

            healthBarTexture.SetData(data);
        }

        public static void Death(GameLocation location, Farmer killer, string displayName, Vector2? chestLocation = null)
        {
            if (ModEntry.ActiveStats.StartTime is not null)
            {
                TimeSpan span = (TimeSpan)(DateTime.UtcNow - ModEntry.ActiveStats.StartTime);
                int killSeconds = (int)span.TotalSeconds;
                ModEntry.MultiplayerHelper.SendMessage(new BossDeathMessage(displayName, killSeconds), "BossDeath");
                Game1.onScreenMenus.Add(
                    new BossKillAnnounceMenu(displayName, killSeconds)
                );
            }

            ModEntry.ActiveStats.BossesDefeated++;

            StopRenderHealthBar();

            bool allFarmersNoHit = true;
            foreach (Farmer farmer in location.farmers)
            {
                if (farmer.get_FarmerWasDamagedOnThisLevel().Value)
                {
                    allFarmersNoHit = false;
                    break;
                }
            }

            string sound = allFarmersNoHit ? "getNewSpecialItem" : "Cowboy_Secret";
            location.playSound(sound);
            Game1.changeMusicTrack("none", true, Game1.MusicContext.Default);
            location.checkForMusic(new GameTime());

            if (chestLocation.HasValue)
            {
                MineShaft mine = (MineShaft)location;
                int level = Roguelike.GetLevelFromMineshaft(mine);
                int additionalGold = BossFloor.GetBossIndexForFloor(level) * 5;

                List<Item> chestItems = new()
                {
                    new SObject(384, 20 + additionalGold),
                };
                if (allFarmersNoHit)
                    chestItems.Add(new SObject(896, 1));

                mine.SpawnLocalChest(chestLocation.Value, chestItems);
            }
        }

        public static void PlayerWarped(object? sender, WarpedEventArgs e)
        {
            bool foundBoss = false;
            foreach (NPC character in Game1.player.currentLocation.characters)
            {
                if (character is IBossMonster boss)
                {
                    foundBoss = true;
                    if (boss.InitializeWithHealthbar)
                        StartRenderHealthBar();

                    break;
                }
            }

            if (!foundBoss)
            {
                StopRenderHealthBar();
                if (e.NewLocation is not null)
                    e.NewLocation.checkForMusic(new GameTime());
            }
        }

        public static void StartRenderHealthBar()
        {
            MineShaftCheckForMusicPatch.ShouldAnnounceMusic = true;
            ModEntry.Events.Display.RenderedHud += RenderHealthBar;
            ModEntry.Events.Display.WindowResized += WindowResized;
            MakeBossHealthBar(100, 100);
        }

        public static void StopRenderHealthBar()
        {
            ModEntry.Events.Display.RenderedHud -= RenderHealthBar;
            ModEntry.Events.Display.WindowResized -= WindowResized;
        }

        public static void WindowResized(object? sender, WindowResizedEventArgs e)
        {
            foreach (NPC character in Game1.player.currentLocation.characters)
            {
                if (character is IBossMonster boss)
                {
                    MakeBossHealthBar(((Monster)boss).Health, ((Monster)boss).MaxHealth);
                    break;
                }
            }

        }

        public static void RenderHealthBar(object? sender, RenderedHudEventArgs e)
        {
            if (healthBarTexture is null)
                return;

            Monster? boss = null;
            foreach (NPC character in Game1.player.currentLocation.characters)
            {
                if (character is IBossMonster)
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

            if (boss.Health != PreviousHealth)
            {
                PreviousHealth = boss.Health;
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

            Vector2 nameTextSize = Game1.dialogueFont.MeasureString(((IBossMonster)boss).DisplayName);

            Utility.drawTextWithColoredShadow(
                e.SpriteBatch,
                ((IBossMonster)boss).DisplayName,
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
