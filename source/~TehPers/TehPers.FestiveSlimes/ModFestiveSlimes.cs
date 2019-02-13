using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.Recipes;
using TehPers.CoreMod.Api.Items.Recipes.Parts;
using TehPers.CoreMod.Api.Structs;
using SObject = StardewValley.Object;

namespace TehPers.FestiveSlimes {
    public class ModFestiveSlimes : Mod {
        public static ModFestiveSlimes Instance { get; private set; }
        public static ICoreApi CoreApi;
        private static Texture2D _greenSlimeWinterHat;
        private static Texture2D _bigSlimeWinterHat;
        private Season _lastSeason;

        public override void Entry(IModHelper helper) {
            ModFestiveSlimes.Instance = this;

            this.Helper.Events.GameLoop.GameLaunched += (sender, args) => {
                if (helper.ModRegistry.GetApi<ICoreApiFactory>("TehPers.CoreMod") is ICoreApiFactory coreFactory) {
                    ModFestiveSlimes.CoreApi = coreFactory.GetApi(this);
                    this.InitializeMod(ModFestiveSlimes.CoreApi);
                } else {
                    this.Monitor.Log("Failed to get core API. This mod will be disabled.", LogLevel.Error);
                }
            };
        }

        private void InitializeMod(ICoreApi coreApi) {
            this.Monitor.Log("Rustling Jimmies...");
            this.Monitor.Log("If this mod is being updated from version 1, then remove all candy from your save before loading it!", LogLevel.Warn);
            this.Monitor.Log("Otherwise, unexpected behavior could occur, including possibly crashes.", LogLevel.Warn);

            // Replace slimes
            this.ReplaceSlimes();

            // Add custom items
            this.AddItems(coreApi);

            this.Monitor.Log("Done!");
        }

        private void AddItems(ICoreApi coreApi) {
            // Create a sprite for the candy which points to the main custom item sprite sheet
            ISprite candySprite = new TintedSprite(coreApi.Items.CreateSprite(this.Helper.Content.Load<Texture2D>("assets/items/candy.png")), Color.Red);

            // Set the buffs for the candy
            BuffDescription candyBuffs = new BuffDescription(TimeSpan.FromMinutes(2.5), speed: 1);

            // Create the candy object manager
            ModFood candy = new ModFood(coreApi, candySprite, "candy", 20, 5, Category.Trash, false, candyBuffs);

            // Register the candy with the core API to add it as an object in the game
            coreApi.Items.CommonRegistry.Objects.Register("candy", candy);
        }

        private void ReplaceSlimes() {
            // Add seasonal textures
            this.Helper.Content.AssetLoaders.Add(new SlimeLoader(this, "Green Slime"));
            this.Helper.Content.AssetLoaders.Add(new SlimeLoader(this, "Big Slime"));

            // Make sure to check if the season has changed and invalidate the textures if needed at the start of each day
            this.Helper.Events.GameLoop.Saved += (sender, args) => this.CheckSeason();
            this.Helper.Events.GameLoop.SaveLoaded += (sender, args) => this.CheckSeason();

            // Register texture events for green slimes
            ModFestiveSlimes._greenSlimeWinterHat = this.Helper.Content.Load<Texture2D>(@"assets\Green Slime\hats\winter.png");
            ITrackedTexture trackedTexture = ModFestiveSlimes.CoreApi.Drawing.GetTrackedTexture(new AssetLocation("Characters/Monsters/Green Slime", ContentSource.GameContent));
            trackedTexture.Drawing += this.RemoveSlimeTint;

            // Register texture events for big slimes
            ModFestiveSlimes._bigSlimeWinterHat = this.Helper.Content.Load<Texture2D>(@"assets\Big Slime\hats\winter.png");
            trackedTexture = ModFestiveSlimes.CoreApi.Drawing.GetTrackedTexture(new AssetLocation("Characters/Monsters/Big Slime", ContentSource.GameContent));
            trackedTexture.Drawing += this.RemoveSlimeTint;

            // Add additional drops to slimes
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Green slime - drops
            MethodInfo target = typeof(GreenSlime).GetMethod(nameof(GreenSlime.getExtraDropItems), BindingFlags.Public | BindingFlags.Instance);
            MethodInfo replacement = typeof(ModFestiveSlimes).GetMethod(nameof(ModFestiveSlimes.GreenSlime_GetExtraDropItemsPostfix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, postfix: new HarmonyMethod(replacement));

            // Green slime - drawing
            target = typeof(GreenSlime).GetMethod(nameof(GreenSlime.draw), new[] { typeof(SpriteBatch) });
            replacement = typeof(ModFestiveSlimes).GetMethod(nameof(ModFestiveSlimes.GreenSlime_DrawPostfix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, postfix: new HarmonyMethod(replacement));

            // Big slime - drops
            target = typeof(Monster).GetMethod(nameof(Monster.getExtraDropItems), BindingFlags.Public | BindingFlags.Instance);
            replacement = typeof(ModFestiveSlimes).GetMethod(nameof(ModFestiveSlimes.Monster_GetExtraDropItemsPostfix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, postfix: new HarmonyMethod(replacement));

            // Big slime - drawing
            target = typeof(BigSlime).GetMethod(nameof(Monster.draw), new[] { typeof(SpriteBatch) });
            replacement = typeof(ModFestiveSlimes).GetMethod(nameof(ModFestiveSlimes.BigSlime_DrawPostfix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, postfix: new HarmonyMethod(replacement));
        }

        private void RemoveSlimeTint(object sender, IDrawingInfo info) {
            if (SDateTime.Today.Season == Season.Fall) {
                // Remove tint
                info.SetTint(Color.White);
            }
        }

        private void CheckSeason() {
            // Check if the season changed
            Season season = SDateTime.Today.Season;
            if (this._lastSeason != season) {
                this.Helper.Content.InvalidateCache("Characters/Monsters/Green Slime");
                this.Helper.Content.InvalidateCache("Characters/Monsters/Big Slime");
            }

            // Update the tracked season
            this._lastSeason = season;
        }

        // void draw(SpriteBatch b)
        private static void GreenSlime_DrawPostfix(GreenSlime __instance, SpriteBatch b) {
            if (SDateTime.Today.Season == Season.Winter) {
                b.Draw(ModFestiveSlimes._greenSlimeWinterHat, __instance.getLocalPosition(Game1.viewport) + new Vector2(32f, __instance.GetBoundingBox().Height / 2f + __instance.yOffset), __instance.Sprite.SourceRect, Color.White, 0.0f, new Vector2(8f, 16f), 4f * Math.Max(0.2f, __instance.Scale - (float) (0.400000005960464 * (__instance.ageUntilFullGrown.Value / 120000.0))), SpriteEffects.None, Math.Max(0.0f, __instance.drawOnTop ? 0.991f : __instance.getStandingY() / 10000f + 5f / 10000f));
            }
        }

        // void draw(SpriteBatch b)
        private static void BigSlime_DrawPostfix(BigSlime __instance, SpriteBatch b) {
            if (SDateTime.Today.Season == Season.Winter) {
                b.Draw(ModFestiveSlimes._bigSlimeWinterHat, __instance.getLocalPosition(Game1.viewport) + new Vector2(56f, 16 + __instance.yJumpOffset), __instance.Sprite.SourceRect, Color.White, 0.0f, new Vector2(16f, 16f), 4f * Math.Max(0.2f, __instance.Scale), __instance.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, __instance.drawOnTop ? 0.991f : (float) (__instance.getStandingY() / 10000.0 + 2.0 / 1000.0)));
            }
        }

        // List<Item> GreenSlime.getExtraDropItems()
        private static void GreenSlime_GetExtraDropItemsPostfix(ref List<Item> __result) {
            if (SDateTime.Today.Season == Season.Fall && Game1.random.NextDouble() < 0.25 && ModFestiveSlimes.CoreApi.Items.TryCreate("candy", out Item candy)) {
                //__result.Add(new SObject(Vector2.Zero, index, 1));
                __result.Add(candy);
            }
        }

        // List<Item> BigSlime.getExtraDropItems()
        private static void Monster_GetExtraDropItemsPostfix(Monster __instance, ref List<Item> __result) {
            if (!(__instance is BigSlime)) {
                return;
            }

            // Check if fall
            if (SDateTime.Today.Season != Season.Fall) {
                return;
            }

            // Try to add a candy (50% chance)
            if (Game1.random.NextDouble() < 0.5 && ModFestiveSlimes.CoreApi.Items.TryCreate("candy", out Item candy)) {
                __result.Add(candy);
            }

            // Try to add a candy (25% chance)
            if (Game1.random.NextDouble() < 0.25 && ModFestiveSlimes.CoreApi.Items.TryCreate("candy", out candy)) {
                __result.Add(candy);
            }
        }
    }
}
