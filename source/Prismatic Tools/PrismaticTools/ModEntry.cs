using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

using Harmony;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using PrismaticTools.Framework;
using System.Collections.Generic;
using StardewValley.Objects;
using StardewValley.Locations;
using StardewValley.Tools;

namespace PrismaticTools {

    public class ModEntry : Mod {

        public static IMonitor mon;
        public static IModHelper ModHelper;
        public static ModConfig Config;

        public static Texture2D toolsTexture;
        private int colorCycleIndex = 0;
        private List<Color> colors = new List<Color>();

        public override void Entry(IModHelper helper) {
            mon = Monitor;
            ModHelper = helper;

            Config = Helper.ReadConfig<ModConfig>();

            toolsTexture = ModHelper.Content.Load<Texture2D>("Assets/tools.png", ContentSource.ModFolder);

            helper.ConsoleCommands.Add("ptools", "Upgrade all tools to prismatic", UpgradeTools);

            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            PlayerEvents.InventoryChanged += PlayerEvents_InventoryChanged;
            GameEvents.EighthUpdateTick += GameEvents_EighthUpdateTick;

            helper.Content.AssetEditors.Add(new AssetEditor());
            SprinklerInitializer.Init();
            BlacksmithInitializer.Init();

            InitColors();

            var harmony = HarmonyInstance.Create("stokastic.PrismaticTools");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void GameEvents_EighthUpdateTick(object sender, System.EventArgs e) {
            Farmer farmer = Game1.player;
            Item item;
            try {
                item = farmer.Items[farmer.CurrentToolIndex];
            }  catch (System.ArgumentOutOfRangeException) {
                return;
            }

            if (item == null || !(item is Object) || !((item as Object).ParentSheetIndex == PrismaticBarItem.INDEX)) {
                return;
            }

            for (int i=0; i<farmer.currentLocation.sharedLights.Count; i++) {
                if (farmer.currentLocation.sharedLights[i].Identifier == (int)farmer.UniqueMultiplayerID) {
                    farmer.currentLocation.sharedLights[i].color.Value = colors[colorCycleIndex];
                }
            }
            colorCycleIndex = (colorCycleIndex + 1) % colors.Count;
        }

        public override object GetApi() {
            return new PrismaticAPI();
        }

        private void UpgradeTools(string command, string[] args) {
            foreach (Item item in Game1.player.Items) {
                if (item is Axe || item is WateringCan || item is Pickaxe || item is Hoe) {
                    (item as Tool).UpgradeLevel = 5;
                }
            }
        }

        // adds lightsources to prismatic bar and sprinkler items in inventory
        private void AddLightsToInventoryItems() {
            if (!Config.UseSprinklersAsLamps) {
                return;
            }
            foreach (Item item in Game1.player.Items) {
                if (item is Object) {
                    if (item.ParentSheetIndex == PrismaticSprinklerItem.INDEX) {
                        (item as Object).lightSource = new LightSource(LightSource.cauldronLight, new Vector2(0, 0), 2.0f, new Color(0.0f, 0.0f, 0.0f));
                    } else if (item.ParentSheetIndex == PrismaticBarItem.INDEX) {
                        (item as Object).lightSource = new LightSource(LightSource.cauldronLight, new Vector2(0, 0), 1.0f, colors[colorCycleIndex]);
                    }
                }
            }
        }

        private void PlayerEvents_InventoryChanged(object sender, EventArgsInventoryChanged e) {
            AddLightsToInventoryItems();
        }

        private void SaveEvents_AfterLoad(object sender, System.EventArgs e) {
            // force add sprinkler recipe for people who were level 10 before installing mod
            if (Game1.player.FarmingLevel >= PrismaticSprinklerItem.CRAFTING_LEVEL) {
                try {
                    Game1.player.craftingRecipes.Add("Prismatic Sprinkler", 0);
                } catch { }
            }

            IndexCompatibilityFix();
            AddLightsToInventoryItems();
        }

        // used to resolve asset conflicts with other mods
        private void IndexCompatibilityFix() {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (GameLocation location in Game1.locations) {
                if (location is FarmHouse) {
                    // check fridge
                    if ((location as FarmHouse).fridge.Value != null) {
                        foreach (Item item in (location as FarmHouse).fridge.Value.items) {
                            if (item == null) {
                                continue;
                            }
                            if (item.Name.Contains("Prismatic")) {
                                SwapIndex(item);
                            }
                        }
                    }
                }
                foreach (Object obj in location.Objects.Values) {
                    // check chests, signposts, furnaces, and placed sprinklers
                    if (obj == null) {
                        continue;
                    }

                    if (obj is Chest) {
                        foreach (Item item in (obj as Chest).items) {
                            if (item.Name.Contains("Prismatic")) {
                                SwapIndex(item);
                            }
                        }
                    } else if (obj is Sign) {
                        SwapIndex((obj as Sign).displayItem.Value);
                    } else if (obj.bigCraftable.Value && obj.name.Equals("Furnace")) {
                        if (obj.heldObject.Value != null) {
                            SwapIndex((obj.heldObject.Value));
                        }
                    } else if (obj.ParentSheetIndex == PrismaticBarItem.OLD_INDEX || obj.ParentSheetIndex == PrismaticSprinklerItem.OLD_INDEX) {
                        SwapIndex(obj);
                    }
                }
            }

            foreach (Item item in Game1.player.Items) {
                if (item != null && item.Name.Contains("Prismatic")) {
                    SwapIndex(item);
                }
            }
            watch.Stop();
            Monitor.Log($"IndexCompatibility exec time: {watch.ElapsedMilliseconds} ms", LogLevel.Trace);
        }

        private void SwapIndex(Item item) {
            if (item.ParentSheetIndex == PrismaticBarItem.OLD_INDEX) {
                item.ParentSheetIndex = PrismaticBarItem.INDEX;
            }
            if (item.ParentSheetIndex == PrismaticSprinklerItem.OLD_INDEX) {
                item.ParentSheetIndex = PrismaticSprinklerItem.INDEX;
            }
        }

        private void InitColors() {
            int n = 24;
            for(int i=0; i<n; i++) {
                colors.Add(ColorFromHSV(360.0 * i / n, 1.0, 1.0));
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

            if (hi == 0)
                return new Color(v, t, p);
            else if (hi == 1)
                return new Color(q, v, p);
            else if (hi == 2)
                return new Color(p, v, t);
            else if (hi == 3)
                return new Color(p, q, v);
            else if (hi == 4)
                return new Color(t, p, v);
            else
                return new Color(v, p, q);
        }
    }
}