/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kakashigr/stardew-radioactivetools
**
*************************************************/

using System.Collections.Generic;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadioactiveTools.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace RadioactiveTools {
    public class ModEntry : Mod {
        public static IModHelper ModHelper;
        public static ModConfig Config;
        public static Texture2D ToolsTexture;
        public static Texture2D WeaponsTexture;

        private int colorCycleIndex;
        private readonly List<Color> colors = new List<Color>();

        public override void Entry(IModHelper helper) {
            ModHelper = helper;

            Config = this.Helper.ReadConfig<ModConfig>();

            ToolsTexture = ModHelper.Content.Load<Texture2D>("assets/tools.png");

            helper.ConsoleCommands.Add("ptools", "Upgrade all tools to radioactive", this.UpgradeTools);

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Player.InventoryChanged += this.OnInventoryChanged;

            helper.Content.AssetEditors.Add(new AssetEditor());
            BlacksmithInitializer.Init(helper.Events);

            this.InitColors();

            var harmony = HarmonyInstance.Create("kakashigr.RadioactiveTools");
            this.ApplyPatches(harmony);
        }

        private void ApplyPatches(HarmonyInstance harmony) {

            // sprinklers
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.addCrows)),
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Farm_AddCrows))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.IsSprinkler)),
                postfix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.After_Object_IsSprinkler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.GetBaseRadiusForSprinkler)),
                postfix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.After_Object_GetBaseRadiusForSprinkler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.updateWhenCurrentLocation)),
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Object_UpdatingWhenCurrentLocation))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Object), nameof(Object.placementAction)),
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Object_OnPlacing))
            );

            // tools
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.performToolAction)),
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Tree_PerformToolAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.performToolAction)),
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.FruitTree_PerformToolAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Pickaxe), nameof(Pickaxe.DoFunction)),
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Pickaxe_DoFunction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ResourceClump), nameof(ResourceClump.performToolAction)),
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.ResourceClump_PerformToolAction))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Tool), "tilesAffected"),
                postfix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Tool_TilesAffected_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Property(typeof(Tool), nameof(Tool.Name)).GetMethod,
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Tool_Name))
            );
            harmony.Patch(
                original: AccessTools.Property(typeof(Tool), nameof(Tool.DisplayName)).GetMethod,
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Tool_DisplayName))
            );
            harmony.Patch(
                original: AccessTools.Property(typeof(FishingRod), nameof(FishingRod.Name)).GetMethod,
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Fishrod_Name))
            );
            harmony.Patch(
                original: AccessTools.Property(typeof(FishingRod), nameof(FishingRod.DisplayName)).GetMethod,
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Fishrod_DisplayName))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.getColor)),
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.FishrodColor))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.isScythe)),
                prefix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.Radioactive_isScythe))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), "performToolAction"),
                postfix: new HarmonyMethod(typeof(RadioactivePatches), nameof(RadioactivePatches.RScythe_performToolAction))
            );
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {
            if (!e.IsMultipleOf(8))
                return;

            Farmer farmer = Game1.player;
            Item item;
            try {
                item = farmer.Items[farmer.CurrentToolIndex];
            } catch (System.ArgumentOutOfRangeException) {
                return;
            }

            if (!(item is Object obj) || obj.ParentSheetIndex != 910) {
                return;
            }

            foreach (var light in farmer.currentLocation.sharedLights.Values) {
                if (light.Identifier == (int)farmer.UniqueMultiplayerID) {
                    light.color.Value = this.colors[this.colorCycleIndex];
                }
            }
            this.colorCycleIndex = (this.colorCycleIndex + 1) % this.colors.Count;
        }

        public override object GetApi() {
            return new RadioactiveAPI();
        }

        private void UpgradeTools(string command, string[] args) {
            foreach (Item item in Game1.player.Items) {
                if (item is Axe || item is WateringCan || item is Pickaxe || item is Hoe) {
                    (item as Tool).UpgradeLevel = 5;
                }
            }
        }

        /// <summary>Adds light sources to radioactive bar and sprinkler items in inventory.</summary>
        private void AddLightsToInventoryItems() {
            if (!Config.UseSprinklersAsLamps) {
                return;
            }
            foreach (Item item in Game1.player.Items) {
                if (item is Object obj) {
                    if (obj.ParentSheetIndex == RadioactiveSprinklerItem.INDEX) {
                        obj.lightSource = new LightSource(LightSource.cauldronLight, Vector2.Zero, 2.0f, new Color(0.0f, 0.0f, 0.0f));
                    } else if (obj.ParentSheetIndex == 910) {
                        obj.lightSource = new LightSource(LightSource.cauldronLight, Vector2.Zero, 1.0f, this.colors[this.colorCycleIndex]);
                    }
                }
            }
        }

        /// <summary>Set scarecrow mode for sprinkler items.</summary>
        private void SetScarecrowModeForAllSprinklers() {
            foreach (GameLocation location in Game1.locations) {
                foreach (Object obj in location.Objects.Values) {
                    if (obj.ParentSheetIndex == RadioactiveSprinklerItem.INDEX) {
                        obj.Name = Config.UseSprinklersAsScarecrows
                            ? "Radioactive Scarecrow Sprinkler"
                            : "Radioactive Sprinkler";
                    }
                }
            }
        }

        /// <summary>Raised after items are added or removed to a player's inventory.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e) {
            if (e.IsLocalPlayer)
                this.AddLightsToInventoryItems();
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
            // force add sprinkler recipe for people who were level 10 before installing mod
            if (Game1.player.FarmingLevel >= RadioactiveSprinklerItem.CRAFTING_LEVEL) {
                try {
                    Game1.player.craftingRecipes.Add("Radioactive Sprinkler", 0);
                    Game1.player.craftingRecipes.Add("Radioactive Fertilizer", 0);
                } catch { }
            }

            this.AddLightsToInventoryItems();
            this.SetScarecrowModeForAllSprinklers();
        }

        private void InitColors() {
            int n = 24;
            for (int i = 0; i < n; i++) {
                this.colors.Add(this.ColorFromHSV(360.0 * i / n, 1.0, 1.0));
            }
        }

        private Color ColorFromHSV(double hue, double saturation, double value) {
            int hi = System.Convert.ToInt32(System.Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - System.Math.Floor(hue / 60);

            value = value * 255;
            int v = System.Convert.ToInt32(value);
            int p = System.Convert.ToInt32(value * (1 - saturation));
            int q = System.Convert.ToInt32(value * (1 - f * saturation));
            int t = System.Convert.ToInt32(value * (1 - (1 - f) * saturation));

            v = 255 - v;
            p = 255 - v;
            q = 255 - q;
            t = 255 - t;

            switch (hi) {
                case 0:
                    return new Color(v, t, p);
                case 1:
                    return new Color(q, v, p);
                case 2:
                    return new Color(p, v, t);
                case 3:
                    return new Color(p, q, v);
                case 4:
                    return new Color(t, p, v);
                default:
                    return new Color(v, p, q);
            }
        }
    }
}
