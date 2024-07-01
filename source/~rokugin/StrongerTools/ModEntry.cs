/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley.Tools;
using StardewValley;
using GenericModConfigMenu;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using HarmonyLib;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using StardewValley.Triggers;
using StardewValley.Delegates;

namespace StrongerTools {
    internal class ModEntry : Mod {

        Item? item;
        ModConfig config = new();
        bool hardwareCursor = false;

        public override void Entry(IModHelper helper) {
            config = helper.ReadConfig<ModConfig>();

            helper.ConsoleCommands.Add("rokugin.set_health", "Sets player health to specified amount.\n\nUsage: rokugin.set_health <amount>\n- amount: integer amount", SetHealth);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(Trinket), nameof(Trinket.canBeShipped)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Trinket_CanBeShipped_Postfix))
            );
        }

        private void OnReturnedToTitle(object? sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e) {
            hardwareCursor = false;
        }

        private void OnUpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e) {
            if (hardwareCursor) return;
            EnableHardwareCursor();
        }

        static void Trinket_CanBeShipped_Postfix(ref bool __result) {
            __result = true;
        }

        private void OnDayStarted(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e) {
            //GameLocation location = Game1.getLocationFromName("Backwoods");

            //AddTerrainFeature(location, new Vector2(34, 10), Tree.pineTree);
            //AddTerrainFeature(location, new Vector2(41, 16), Tree.bushyTree);
            //AddTerrainFeature(location, new Vector2(12, 19), Tree.bushyTree);
            //AddTerrainFeature(location, new Vector2(15, 17), Tree.bushyTree);
            //AddTerrainFeature(location, new Vector2(20, 20), Tree.bushyTree);
            //AddTerrainFeature(location, new Vector2(25, 16), Tree.bushyTree);
            //AddTerrainFeature(location, new Vector2(17, 14), Tree.leafyTree);
            //AddTerrainFeature(location, new Vector2(19, 12), Tree.leafyTree);
            //AddTerrainFeature(location, new Vector2(21, 11), Tree.leafyTree);
            //AddTerrainFeature(location, new Vector2(28, 9), Tree.leafyTree);
            //AddTerrainFeature(location, new Vector2(46, 12), Tree.leafyTree);
            Game1.netWorldState.Value.canDriveYourselfToday.Value = true;
        }

        void AddTerrainFeature(GameLocation location, Vector2 tilePosition, string treeType) {
            if (!location.IsTileOccupiedBy(tilePosition)) {
                location.terrainFeatures.Add(tilePosition, new Tree(treeType, Tree.treeStage));
                if (config.ShowLogs) Monitor.Log(
                    $"Placing stage 5 {GetTreeNameByID(treeType)} tree at {location.Name} (X:{tilePosition.X}, Y:{tilePosition.Y})", LogLevel.Info);
            }
        }

        string GetTreeNameByID(string id) {
            string treeName;
            switch (id) {
                case "1":
                    treeName = "Oak";
                    break;
                case "2":
                    treeName = "Maple";
                    break;
                case "3":
                    treeName = "Pine";
                    break;
                default:
                    treeName = "";
                    break;
            }
            return treeName;
        }

        private void OnGameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e) {
            SetupGMCM();
            EnableHardwareCursor();

            TriggerActionManager.RegisterAction("rokugin.SetPlayerHealth", SetPlayerHealth);
        }

        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e) {
            if (!Context.IsPlayerFree) return;
            if (Game1.activeClickableMenu != null) return;

            item = Game1.player.CurrentTool;

            if (e.Button.IsUseToolButton()) {
                if (item is Pickaxe pickaxe) {
                    switch (pickaxe.ItemId) {
                        case "GoldPickaxe":
                            if (pickaxe.additionalPower.Value < 3) pickaxe.additionalPower.Value = 3;
                            break;
                        case "IridiumPickaxe":
                            if (pickaxe.additionalPower.Value < 10) pickaxe.additionalPower.Value = 10;
                            break;
                    }

                    if (config.ShowLogs) Monitor.Log($"{pickaxe.Name} current additional power: +{pickaxe.additionalPower.Value}", LogLevel.Info);
                }
            }
        }

        public static bool SetPlayerHealth(string[] args, TriggerActionContext context, out string error) {
            if (ArgUtility.TryGet(args, 1, out string amount, out error, allowBlank: false)) {
                if (amount == "full") {
                    Game1.player.health = Game1.player.maxHealth;
                    return true;
                }

                if (!int.TryParse(amount, out int healthAmount)) {
                    return false;
                }

                if (healthAmount <= 0) healthAmount = 1;

                Game1.player.health = healthAmount;
                return true;
            }

            return false;
        }

        void SetHealth(string command, string[] args) {
            if (!Context.IsWorldReady) {
                Monitor.Log("Load a save first.", LogLevel.Error);
                return;
            }
            if (!int.TryParse(args[0], out int healthAmount)) {
                Monitor.Log("Could not parse amount to set player health.", LogLevel.Error);
                return;
            }

            if (healthAmount <= 0) healthAmount = 1;
            Game1.player.health = healthAmount;
            Monitor.Log($"Set player health to {healthAmount}.", LogLevel.Info);
        }

        void EnableHardwareCursor() {
            Game1.options.hardwareCursor = true;
            hardwareCursor = true;
        }

        void SetupGMCM() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () => Helper.WriteConfig(config)
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Debug"
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => config.ShowLogs,
                setValue: value => config.ShowLogs = value,
                name: () => "Enabled"
            );
        }

    }
}
