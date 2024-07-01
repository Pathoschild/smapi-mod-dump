/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Caua-Oliveira/StardewValley-AutomateToolSwap
**
*************************************************/


using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Tools;
using xTile.Tiles;


namespace AutomateToolSwap
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance { get; set; } = null!;
        internal static ModConfig Config { get; private set; } = null!;
        internal static Check tileHas { get; private set; } = null!;
        internal static ITranslationHelper i18n;
        internal static bool isTractorModInstalled;
        internal static bool isRangedToolsInstalled;
        internal static bool monsterNearby = false;
        internal static string modsPath;


        public override void Entry(IModHelper helper)
        {
            Instance = this;
            i18n = Helper.Translation;
            Config = Helper.ReadConfig<ModConfig>();
            tileHas = new Check(Instance);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            isTractorModInstalled = Helper.ModRegistry.IsLoaded("Pathoschild.TractorMod");
            isRangedToolsInstalled = Helper.ModRegistry.IsLoaded("vgperson.RangedTools");
            ConfigSetup.SetupConfig(Helper, Instance);
            modsPath = Path.Combine(AppContext.BaseDirectory, "Mods");
        }

        IndexSwitcher indexSwitcher = new IndexSwitcher(0);


        [EventPriority(EventPriority.High)]
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;

            if (Config.ToggleKey.JustPressed())
            {
                Config.Enabled = !Config.Enabled;
                if (Config.Enabled)
                    Game1.addHUDMessage(new HUDMessage("AutomateToolSwap " + i18n.Get("mod.Enabled"), 2));
                else
                    Game1.addHUDMessage(new HUDMessage("AutomateToolSwap " + i18n.Get("mod.Disabled"), 2));
            }

            // swaps to the last used item
            if (Config.LastToolKey.JustPressed() && Game1.player.canMove)
                indexSwitcher.GoToLastIndex();


            if (!ButtonMatched(e) || !Config.Enabled || !(Game1.player.canMove))
                return;

            startMod();
        }

        public void startMod()
        {
            Farmer player = Game1.player;
            GameLocation currentLocation = Game1.currentLocation;
            ICursorPosition cursorPos = Helper.Input.GetCursorPosition();
            Vector2 frontOfPlayerTile = new Vector2((int)Game1.player.GetToolLocation().X / Game1.tileSize, (int)Game1.player.GetToolLocation().Y / Game1.tileSize);

            // compatibility with RangedTools
            string folderPath = Path.Combine(modsPath, "RangedTools");
            string configFilePath = Path.Combine(folderPath, "config.json");
            int toolRange = 1;
            if (File.Exists(configFilePath))
            {
                string jsonString = File.ReadAllText(configFilePath);
                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    JsonElement root = doc.RootElement;
                    if (root.TryGetProperty("AxeRange", out JsonElement toolRangeElement))
                    {
                        toolRange = toolRangeElement.GetInt32();
                    }
                }
            }

            //different methods for detecting tiles
            if (Config.DetectionMethod == "Cursor")
            {
                if (isRangedToolsInstalled)
                {
                    // Calculate Chebyshev distance
                    double distance = Math.Max(Math.Abs(player.Tile.X - cursorPos.Tile.X), Math.Abs(player.Tile.Y - cursorPos.Tile.Y));
                    if (toolRange == -1 || distance <= toolRange)
                    {
                        CheckTile(currentLocation, cursorPos.Tile, player);
                    }
                    else
                    {
                        CheckTile(currentLocation, cursorPos.GrabTile, player);
                    }
                }
                else
                {
                    CheckTile(currentLocation, cursorPos.GrabTile, player);
                }
            }
            else if (Config.DetectionMethod == "Player")
            {
                CheckTile(currentLocation, frontOfPlayerTile, player);
            }

        }

        // detects what is in the tile that the player is trying to interact 
        private void CheckTile(GameLocation location, Vector2 tile, Farmer player)
        {
            // code at OnUpdateTicked()
            if (Config.AlternativeWeaponOnMonsters && player.CurrentItem is MeleeWeapon && !player.CurrentItem.Name.Contains("Scythe") && monsterNearby)
                return;

            // if the player is using slingshot, it will not be swapped because it is a long range weapon
            if (player.CurrentItem is Slingshot)
                return;

            // code at Check.cs
            if (tileHas.Objects(location, tile, player))
                return;

            if (tileHas.ResourceClumps(location, tile, player))
                return;

            if (tileHas.TerrainFeatures(location, tile, player))
                return;

            if (tileHas.Water(location, tile, player))
                return;

            if (Config.WeaponOnMonsters && !Config.AlternativeWeaponOnMonsters)
                if (tileHas.Monsters(location, tile, player))
                    return;

            if (tileHas.Animals(location, tile, player))
                return;

            if (tileHas.DiggableSoil(location, tile, player))
                return;

        }


        //Called when the game updates the tick (60 times per second)
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;

            //Alternative for the option "Weapon for Monsters"
            if (Config.AlternativeWeaponOnMonsters && Config.WeaponOnMonsters)
            {
                monsterNearby = true;
                Vector2 tile = Game1.player.Tile;
                foreach (var monster in Game1.currentLocation.characters)
                {
                    if (monster is RockCrab)
                        break;

                    Vector2 monsterTile = monster.Tile;
                    float distance = Vector2.Distance(tile, monsterTile);

                    if (monster.IsMonster && distance < Config.MonsterRangeDetection && Game1.player.canMove)
                    {
                        if (tileHas.Monsters(Game1.currentLocation, tile, Game1.player))
                            return;
                    }

                }
                monsterNearby = false;
            }

            //Tractor mod compatibility
            if (!isTractorModInstalled || Config.DisableTractorSwap || (!Config.Enabled && !Config.DisableTractorSwap))
                return;

            if (Game1.player.isRidingHorse() && Game1.player.mount.Name.ToLower().Contains("tractor"))
            {
                Farmer player = Game1.player;
                GameLocation currentLocation = Game1.currentLocation;
                ICursorPosition cursorPos = this.Helper.Input.GetCursorPosition();
                Vector2 cursorTile = cursorPos.GrabTile;
                Vector2 toolLocation = new Vector2((int)Game1.player.GetToolLocation().X / Game1.tileSize, (int)Game1.player.GetToolLocation().Y / Game1.tileSize);

                if (Config.DetectionMethod == "Cursor")
                    CheckTile(currentLocation, cursorTile, player);

                else if (Config.DetectionMethod == "Player")
                    CheckTile(currentLocation, toolLocation, player);

            }
        }


        //Looks for the index of the tool necessary for the action
        public void SetTool(Farmer player, Type toolType, string aux = "", bool anyTool = false)
        {
            indexSwitcher.canSwitch = Config.AutoReturnToLastTool;
            var items = player.Items;

            //Melee Weapons (swords and scythes) \/
            if (toolType == typeof(MeleeWeapon))
            {
                if (aux == "Scythe" || aux == "ScytheOnly")
                {
                    for (int i = 0; i < player.maxItems; i++)
                    {
                        if (items[i] != null && items[i].GetType() == toolType && items[i].Name.Contains("Scythe"))
                        {
                            if (player.CurrentToolIndex != i)
                            {
                                indexSwitcher.SwitchIndex(i);
                            }
                            return;
                        }
                    }
                    if (aux == "ScytheOnly")
                        return;
                }

                for (int i = 0; i < player.maxItems; i++)
                {
                    if (items[i] != null && items[i].GetType() == toolType && !(items[i].Name.Contains("Scythe")))
                    {
                        if (player.CurrentToolIndex != i)
                        {
                            indexSwitcher.SwitchIndex(i);
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
                        indexSwitcher.SwitchIndex(i);

                    }

                    return;
                }

            }

        }

        //Looks for the index of the item necessary for the action
        public void SetItem(Farmer player, string categorie, string item = "", string crops = "Both", int aux = 0)
        {
            indexSwitcher.canSwitch = Config.AutoReturnToLastTool;
            var items = player.Items;

            //Handles trash
            if (categorie == "Trash" || categorie == "Fertilizer")
            {
                for (int i = 0; i < player.maxItems; i++)
                {

                    if (items[i] != null && items[i].category == aux && !(items[i].Name.Contains(item)))
                    {
                        if (player.CurrentToolIndex != i)
                        {
                            indexSwitcher.SwitchIndex(i);
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
                    if (items[i] != null && items[i].category == -15 && items[i].Name.Contains(item) && items[i].Stack >= 5)
                    {
                        if (player.CurrentToolIndex != i)
                            indexSwitcher.SwitchIndex(i);

                        return;
                    }
                }
                return;
            }

            //Handles Seeds
            if (categorie == "Seed")
            {
                for (int i = 0; i < player.maxItems; i++)
                {

                    if (items[i] != null && items[i].category == -74 && !items[i].HasContextTag("tree_seed_item"))
                    {
                        if (player.CurrentToolIndex != i)
                            indexSwitcher.SwitchIndex(i);

                        return;
                    }
                }
                return;
            }

            //Handles Crops
            if (categorie == "Crops")
            {
                bool canFruit = crops == "Both" || crops == "Fruit";
                bool canVegetable = crops == "Both" || crops == "Vegetable";

                for (int i = 0; i < player.maxItems; i++)
                {
                    bool isFruit(Item Item) { return Item != null && Item.category == -79; }

                    bool isVegetable(Item Item) { return Item != null && Item.category == -75; }

                    if (items[i] != null && (canFruit && isFruit(items[i]) || canVegetable && isVegetable(items[i])))
                    {
                        if (isFruit(player.CurrentItem) || isVegetable(player.CurrentItem))
                            return;

                        if (player.CurrentToolIndex != i)
                            indexSwitcher.SwitchIndex(i);

                        return;
                    }
                }
                return;
            }


            //Handles any other item
            for (int i = 0; i < player.maxItems; i++)
            {
                if (items[i] != null && items[i].category == aux && items[i].Name.Contains(item))
                {
                    if (player.CurrentItem != null && player.CurrentItem.category.ToString() == categorie)
                        return;

                    if (player.CurrentToolIndex != i)
                        indexSwitcher.SwitchIndex(i);

                    return;
                }
            }
            return;
        }

        //Checks if the button pressed matches the config
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



