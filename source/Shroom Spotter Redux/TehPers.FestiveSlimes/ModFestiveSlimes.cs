/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

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
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Structs;
using TehPers.FestiveSlimes.AssetLoaders;
using SObject = StardewValley.Object;

namespace TehPers.FestiveSlimes {
    public class ModFestiveSlimes : Mod {
        public static ModFestiveSlimes Instance { get; private set; }
        public static ICoreApi CoreApi;
        private static GreenSlimeManager _greenSlimeManager;
        private static BigSlimeManager _bigSlimeManager;

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
            this.ReplaceSlimes(coreApi);

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
            ModFood candy = new ModFood(coreApi.TranslationHelper, candySprite, "candy", 20, 5, Category.Trash, false, candyBuffs);

            // Register the candy with the core API to add it as an object in the game
            coreApi.Items.CommonRegistry.Objects.Register("candy", candy);
        }

        private void ReplaceSlimes(ICoreApi coreApi) {
            // Create slime managers
            ModFestiveSlimes._greenSlimeManager = new GreenSlimeManager(coreApi);
            ModFestiveSlimes._bigSlimeManager = new BigSlimeManager(coreApi);

            // Add seasonal textures
            this.Helper.Content.AssetLoaders.Add(ModFestiveSlimes._greenSlimeManager);
            this.Helper.Content.AssetLoaders.Add(ModFestiveSlimes._bigSlimeManager);

            // Make sure to check if the season has changed and invalidate the textures if needed at the start of each day
            this.Helper.Events.GameLoop.Saved += (sender, args) => ModFestiveSlimes.CheckSeason();
            this.Helper.Events.GameLoop.SaveLoaded += (sender, args) => ModFestiveSlimes.CheckSeason();

            // Create harmony instance
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

        private static void CheckSeason() {
            ModFestiveSlimes._greenSlimeManager.InvalidateIfNeeded(SDateTime.Today);
            ModFestiveSlimes._bigSlimeManager.InvalidateIfNeeded(SDateTime.Today);
        }

        // void draw(SpriteBatch b)
        private static void GreenSlime_DrawPostfix(GreenSlime __instance, SpriteBatch b) {
            ModFestiveSlimes._greenSlimeManager.DrawHat(SDateTime.Today, b, __instance);
        }

        // void draw(SpriteBatch b)
        private static void BigSlime_DrawPostfix(BigSlime __instance, SpriteBatch b) {
            ModFestiveSlimes._bigSlimeManager.DrawHat(SDateTime.Today, b, __instance);
        }

        // List<Item> GreenSlime.getExtraDropItems()
        private static void GreenSlime_GetExtraDropItemsPostfix(GreenSlime __instance, ref List<Item> __result) {
            // Add extra drops
            __result.AddRange(ModFestiveSlimes._greenSlimeManager.GetExtraDrops(SDateTime.Today, __instance));
        }

        // List<Item> BigSlime.getExtraDropItems()
        private static void Monster_GetExtraDropItemsPostfix(Monster __instance, ref List<Item> __result) {
            if (!(__instance is BigSlime)) {
                return;
            }

            // Add extra drops
            __result.AddRange(ModFestiveSlimes._bigSlimeManager.GetExtraDrops(SDateTime.Today, __instance));
        }
    }
}
