/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Caua-Oliveira/StardewValley-AutomateToolSwap
**
*************************************************/


using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;
using xTile.Tiles;



namespace AutomateToolSwap
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance { get; set; } = null!;
        internal static ModConfig Config { get; private set; } = null!; // Declare static instance of ModConfig
        internal static Check check { get; private set; } = null!;
        internal static bool isTractorModInstalled;
        public override void Entry(IModHelper helper)
        {
            isTractorModInstalled = Helper.ModRegistry.IsLoaded("Pathoschild.TractorMod");
            Instance = this;
            Config = Helper.ReadConfig<ModConfig>();
            check = new Check(this);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config.Enabled = true;
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            // Add the general settings
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Detection Settings"
            );

            //If you should use the custumizable SwapKey or the game default
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Use Custom Swap Key",
                tooltip: () => "If you should use the custumizable Tool Swap Keybind or the game default.",
                getValue: () => Config.UseDifferentSwapKey,
                setValue: isEnabled => Config.UseDifferentSwapKey = isEnabled
            );

            // Keybind for swapping tools
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Tool Swap Keybind",
                tooltip: () => "The keybind to switch between tools (Only if you check the option above). Otherwise, it will use the default game keybind for Using Tools.",
                getValue: () => Config.SwapKey,
                setValue: keybinds => Config.SwapKey = keybinds
            );

            // Keybind for toggling mod on/off
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Toggle Mod Keybind",
                tooltip: () => "The keybind to toggle the mod on or off.",
                getValue: () => Config.ToggleKey,
                setValue: keybinds => Config.ToggleKey = keybinds
            );

            // Keybind to switch back to last used tool
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Last Tool Keybind",
                tooltip: () => "The keybind to switch back to the last used tool.",
                getValue: () => Config.LastToolKey,
                setValue: keybinds => Config.LastToolKey = keybinds
            );

            // Detection method for tool switching
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Tool Selection Mode",
                tooltip: () => "Choose how tools are switched: 'Cursor' uses the mouse pointer, 'Player' uses the player's orientation. (USE THIS FOR CONTROLLER)",
                allowedValues: new string[] { "Cursor", "Player" },
                getValue: () => Config.DetectionMethod,
                setValue: method => Config.DetectionMethod = method
            );

            // Auto-return to last tool after switching
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Auto-Return to Last Tool",
                tooltip: () => "Automatically return to the previously used tool after swapping.",
                getValue: () => Config.AutoReturnToLastTool,
                setValue: isEnabled => Config.AutoReturnToLastTool = isEnabled
            );

            // Add the general settings
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Custom Swaps Settings"
            );
            // Switch to Weapon when clicking monsters
            configMenu.AddBoolOption(
               mod: this.ModManifest,
               name: () => "Weapon for Monsters",
               tooltip: () => "Automatically switch to Melee Weapons when clicking on monsters.",
               getValue: () => Config.WeaponOnMonsters,
               setValue: isEnabled => Config.WeaponOnMonsters = isEnabled
           );

            // Alternative method to swapping on Monsters
            configMenu.AddBoolOption(
               mod: this.ModManifest,
               name: () => "Alternative \"Weapon for Monsters\"",
               tooltip: () => "Alternative method to swapping on Monsters",
               getValue: () => Config.AlternativeWeaponOnMonsters,
               setValue: isEnabled => Config.AlternativeWeaponOnMonsters = isEnabled
            );

            // Add a NumberOption for MonsterRangeDetections 
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Monster Range Detection",
                tooltip: () => "The range in which the mod detects monsters (in tiles).",
                getValue: () => Config.MonsterRangeDetection,
                setValue: value => Config.MonsterRangeDetection = value,
                min: 1,
                max: 10
            );

            // Switch to hoe when clicking on empty soil
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Hoe for Empty Soil",
                tooltip: () => "Automatically switch to the hoe when clicking on empty soil.",
                getValue: () => Config.HoeForEmptySoil,
                setValue: isEnabled => Config.HoeForEmptySoil = isEnabled
            );

            // Switch to scythe when clicking on grass
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Scythe for Grass",
                tooltip: () => "Automatically switch to the scythe when clicking on grass.",
                getValue: () => Config.ScytheForGrass,
                setValue: isEnabled => Config.ScytheForGrass = isEnabled
            );

            // Prioritize pickaxe over watering can
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Pickaxe Over Watering Can",
                tooltip: () => "Prioritize using the pickaxe instead of switching to the watering can on dry soil.",
                getValue: () => Config.PickaxeOverWateringCan,
                setValue: isEnabled => Config.PickaxeOverWateringCan = isEnabled
            );

            // Use pickaxe for weeds instead of scythe
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Pickaxe for Weeds",
                tooltip: () => "Automatically switch to the pickaxe instead of the scythe when clicking on weeds (fibers).",
                getValue: () => Config.AnyToolForWeeds,
                setValue: isEnabled => Config.AnyToolForWeeds = isEnabled
            );

            // Switch to fishing rod when clicking on water
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "FishingRod On Water",
                tooltip: () => "Automatically switch to the Fishing Rod when clicking on water outside of Farm.",
                getValue: () => Config.FishingRodOnWater,
                setValue: isEnabled => Config.FishingRodOnWater = isEnabled
            );

            // Disabled swap on growing trees
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Ignore Growing Trees",
                tooltip: () => "Growing Trees do not swap to Axe.",
                getValue: () => Config.IgnoreGrowingTrees,
                setValue: isEnabled => Config.IgnoreGrowingTrees = isEnabled
            );

            // Add the Tractor settings
            if (isTractorModInstalled)
            {
                configMenu.AddSectionTitle(
                    mod: this.ModManifest,
                    text: () => "Tractor Settings"
                );

                // Prioritize pickaxe over watering can
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Disable Auto Swap in Tractor",
                    tooltip: () => "Disables Auto Swap in Tractor ALWAYS, otherwise you can use the Toggle Keybind to disable it",
                    getValue: () => Config.DisableTractorSwap,
                    setValue: isEnabled => Config.DisableTractorSwap = isEnabled
                );

            }

        }

        IndexSwitcher switcher = new IndexSwitcher(0);

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Config.AlternativeWeaponOnMonsters && Config.WeaponOnMonsters)
            {
                Vector2 tile = Game1.player.Tile;
                foreach (var monster in Game1.currentLocation.characters)
                {
                    Vector2 monsterTile = monster.Tile;
                    float distance = Vector2.Distance(tile, monsterTile);

                    if (monster.IsMonster && distance < Config.MonsterRangeDetection && Game1.player.canMove)
                    {
                        if (check.Monsters(Game1.currentLocation, tile, Game1.player))
                            return;
                    }

                }

            }

            if (!isTractorModInstalled || Config.DisableTractorSwap || (!Config.Enabled && !Config.DisableTractorSwap))
                return;

            //Code for Tractor Mod
            if (Game1.player.isRidingHorse() && Game1.player.mount.Name.Contains("tractor"))
            {
                Farmer player = Game1.player;
                GameLocation currentLocation = Game1.currentLocation;
                ICursorPosition cursorPos = this.Helper.Input.GetCursorPosition();
                Vector2 cursorTile = cursorPos.GrabTile;
                Vector2 toolLocation = new Vector2((int)Game1.player.GetToolLocation().X / Game1.tileSize, (int)Game1.player.GetToolLocation().Y / Game1.tileSize);

                switch (Config.DetectionMethod)
                {
                    case "Cursor":
                        CheckTile(currentLocation, cursorTile, player);
                        break;
                    case "Player":
                        CheckTile(currentLocation, toolLocation, player);
                        break;
                }
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // turns mod on/off
            if (Config.ToggleKey.JustPressed())
            {
                Config.Enabled = !Config.Enabled;
                if (Config.Enabled)
                {
                    Game1.addHUDMessage(new HUDMessage("AutomateToolSwap ENABLED", 2));
                }
                Game1.addHUDMessage(new HUDMessage("AutomateToolSwap DISABLED", 2));
                Game1.hudMessages.First().timeLeft = 1200;
            }

            // swap to the last item
            if (Config.LastToolKey.JustPressed() && Game1.player.canMove)
            {
                switcher.GoToLastIndex();
            }

            // check if the mod should try to swap
            if (!ButtonMatched(e) || !Config.Enabled || !(Game1.player.canMove))
                return;


            Farmer player = Game1.player;
            GameLocation currentLocation = Game1.currentLocation;
            ICursorPosition cursorPos = this.Helper.Input.GetCursorPosition();
            Vector2 cursorTile = cursorPos.GrabTile;
            Vector2 toolLocation = new Vector2((int)Game1.player.GetToolLocation().X / Game1.tileSize, (int)Game1.player.GetToolLocation().Y / Game1.tileSize);

            switch (Config.DetectionMethod)
            {
                case "Cursor":
                    CheckTile(currentLocation, cursorTile, player);
                    break;
                case "Player":
                    CheckTile(currentLocation, toolLocation, player);
                    break;
            }

        }

        // detects what is in the tile that the player is looking at and calls the function to swap tools
        private void CheckTile(GameLocation location, Vector2 tile, Farmer player)
        {
            if (Config.AlternativeWeaponOnMonsters && player.CurrentItem is MeleeWeapon && !player.CurrentItem.Name.Contains("Scythe"))
                return;

            if (player.CurrentItem is Slingshot)
                return;

            if (check.Objects(location, tile, player))
                return;

            if (check.TerrainFeatures(location, tile, player))
                return;

            if (check.ResourceClumps(location, tile, player))
                return;

            if (check.Water(location, tile, player))
                return;

            if (Config.WeaponOnMonsters && !Config.AlternativeWeaponOnMonsters)
                if (check.Monsters(location, tile, player))
                    return;

            if (check.Animals(location, tile, player))
                return;

            if (check.ShouldSwapToHoe(location, tile, player))
                return;

        }

        //Looks for the tool necessary for the action
        public void SetTool(Farmer player, Type toolType, string aux = "Scythe", bool anyTool = false)
        {
            switcher.canSwitch = Config.AutoReturnToLastTool;
            var items = player.Items;
            //Melee Weapons \/
            if (toolType == typeof(MeleeWeapon))
            {
                if (aux == "Scythe")
                {
                    for (int i = 0; i < player.maxItems; i++)
                    {
                        if (items[i] != null && items[i].GetType() == toolType && items[i].Name.Contains(aux))
                        {
                            if (player.CurrentToolIndex != i)
                            {
                                switcher.SwitchIndex(i);
                            }
                            return;
                        }
                    }
                }

                for (int i = 0; i < player.maxItems; i++)
                {
                    if (items[i] != null && items[i].GetType() == toolType && !(items[i].Name.Contains("Scythe")))
                    {
                        if (player.CurrentToolIndex != i)
                        {
                            switcher.SwitchIndex(i);
                        }
                        return;
                    }
                }
                return;
            }

            //Any other tool \/

            for (int i = 0; i < player.maxItems; i++)
            {
                if ((items[i] != null && items[i].GetType() == toolType) || (anyTool && items[i] is Axe or Pickaxe or Hoe))
                {
                    if (player.CurrentToolIndex != i)
                    {
                        switcher.SwitchIndex(i);
                    }
                    return;
                }
            }

        }



        //Any item \/
        public void SetItem(Farmer player, string categorie, string item)
        {
            switcher.canSwitch = Config.AutoReturnToLastTool;

            var items = player.Items;
            //Handles trash
            if (categorie == "Trash")
            {
                for (int i = 0; i < player.maxItems; i++)
                {

                    if (items[i] != null && items[i].getCategoryName() == categorie && !(items[i].Name.Contains(item)))
                    {
                        if (player.CurrentToolIndex != i)
                        {
                            switcher.SwitchIndex(i);
                        }
                        return;
                    }
                }
                return;
            }

            //Handles resources
            if (categorie == "Resource")
            {
                for (int i = 0; i < player.maxItems; i++)
                {

                    if (items[i] != null && items[i].getCategoryName() == categorie && items[i].Name.Contains(item) && items[i].Stack >= 5)
                    {
                        if (player.CurrentToolIndex != i)
                        {
                            switcher.SwitchIndex(i);
                        }
                        return;
                    }
                }
                return;
            }

            //Handles any other item
            for (int i = 0; i < player.maxItems; i++)
            {

                if (items[i] != null && items[i].getCategoryName() == categorie && items[i].Name.Contains(item))
                {
                    if (player.CurrentToolIndex != i)
                    {
                        switcher.SwitchIndex(i);
                    }
                    return;
                }
            }
            return;
        }

        public bool ButtonMatched(ButtonPressedEventArgs e)
        {
            if (Config.UseDifferentSwapKey)
            {
                if (Config.SwapKey.JustPressed())
                    return true;
                return false;

            }
            else
            {
                foreach (var button in Game1.options.useToolButton)
                {
                    if (e.Button == button.ToSButton() || e.Button == SButton.ControllerX)
                        return true;

                }
                return false;
            }
        }

    }
}



