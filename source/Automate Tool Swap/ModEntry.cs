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
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Tools;


namespace AutomateToolSwap
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance { get; set; } = null!;
        internal static ModConfig Config { get; private set; } = null!;
        internal static Check check { get; private set; } = null!;
        internal static ITranslationHelper i18n;
        internal static bool isTractorModInstalled;
        internal static bool monsterNearby = false;


        //When the mods are loading
        //Quando os mods estão carregando
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            i18n = Helper.Translation;
            Config = Helper.ReadConfig<ModConfig>();
            check = new Check(Instance);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

        }

        //When the game opes
        //Quando o jogo abre
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            isTractorModInstalled = Helper.ModRegistry.IsLoaded("Pathoschild.TractorMod");
            ConfigSetup.SetupConfig(Helper, Instance);
        }

        IndexSwitcher switcher = new IndexSwitcher(0);

        //Special cases
        //Casos especiais
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;

            //Alternative "Weapon for Monsters"
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
                        if (check.Monsters(Game1.currentLocation, tile, Game1.player))
                            return;
                    }

                }
                monsterNearby = false;
            }

            //Code for Tractor Mod
            //Codigo para o Mod de Trator
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

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet or is in a menu
            // ignora se o jogador não tiver carregado um save ou estiver em um menu
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;

            // turns mod on/off
            // desliga e liga o mod
            if (Config.ToggleKey.JustPressed())
            {
                Config.Enabled = !Config.Enabled;
                if (Config.Enabled)
                    Game1.addHUDMessage(new HUDMessage("AutomateToolSwap " + i18n.Get("mod.Enabled"), 2));
                else
                    Game1.addHUDMessage(new HUDMessage("AutomateToolSwap " + i18n.Get("mod.Disabled"), 2));
            }

            // swap to the last item
            // troca para o item mais recente
            if (Config.LastToolKey.JustPressed() && Game1.player.canMove)
                switcher.GoToLastIndex();

            // check if the mod should try to swap items
            // verifica se o mod deve tentar trocar itens
            if (!ButtonMatched(e) || !Config.Enabled || !(Game1.player.canMove))
                return;

            // variables for the main method
            // variáveis para o método principal
            Farmer player = Game1.player;
            GameLocation currentLocation = Game1.currentLocation;
            ICursorPosition cursorPos = this.Helper.Input.GetCursorPosition();
            Vector2 cursorTile = cursorPos.GrabTile;
            Vector2 frontOfPlayerTile = new Vector2((int)Game1.player.GetToolLocation().X / Game1.tileSize, (int)Game1.player.GetToolLocation().Y / Game1.tileSize);
            if (Config.DetectionMethod == "Cursor")
                CheckTile(currentLocation, cursorTile, player);

            else if (Config.DetectionMethod == "Player")
                CheckTile(currentLocation, frontOfPlayerTile, player);

        }

        // detects what is in the tile that the player is looking at and calls the function to swap items
        // detecta o que está no bloco que o jogador está olhando e chama a função para trocar items
        private void CheckTile(GameLocation location, Vector2 tile, Farmer player)
        {
            if (Config.AlternativeWeaponOnMonsters && player.CurrentItem is MeleeWeapon && !player.CurrentItem.Name.Contains("Scythe") && monsterNearby)
                return;

            if (player.CurrentItem is Slingshot)
                return;

            if (check.Objects(location, tile, player))
                return;

            if (check.ResourceClumps(location, tile, player))
                return;

            if (check.TerrainFeatures(location, tile, player))
                return;

            if (check.Water(location, tile, player))
                return;

            if (Config.WeaponOnMonsters && !Config.AlternativeWeaponOnMonsters)
                if (check.Monsters(location, tile, player))
                    return;

            if (check.Animals(location, tile, player))
                return;

            if (check.DiggableSoil(location, tile, player))
                return;

        }

        //Looks for the tool necessary for the action
        //Procura pelo ferramenta necessária para a ação
        public void SetTool(Farmer player, Type toolType, string aux = "", bool anyTool = false)
        {
            switcher.canSwitch = Config.AutoReturnToLastTool;
            var items = player.Items;
            //Melee Weapons (swords and scythes) \/
            //Armas de curta distância (espadas e foice) \/
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
                                switcher.SwitchIndex(i);
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
                            switcher.SwitchIndex(i);
                        }
                        return;
                    }
                }
                return;
            }

            //Any other tool \/
            //Qualquer outra ferramenta

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

        //Looks for the item necessary for the action
        //Procura pelo item necessário para a ação
        public void SetItem(Farmer player, string categorie, string item = "", string crops = "Both", int aux = 0)
        {
            switcher.canSwitch = Config.AutoReturnToLastTool;

            var items = player.Items;
            //Handles trash
            //Trata da categoria lixo
            if (categorie == "Trash" || categorie == "Fertilizer")
            {
                for (int i = 0; i < player.maxItems; i++)
                {

                    if (items[i] != null && items[i].category == aux && !(items[i].Name.Contains(item)))
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
            //Trata da categoria recursos
            if (categorie == "Resource")
            {
                for (int i = 0; i < player.maxItems; i++)
                {
                    if (items[i] != null && items[i].category == -15 && items[i].Name.Contains(item) && items[i].Stack >= 5)
                    {
                        if (player.CurrentToolIndex != i)
                            switcher.SwitchIndex(i);

                        return;
                    }
                }
                return;
            }

            //Handles Seeds
            //Trata da categoria sementes
            if (categorie == "Seed")
            {
                for (int i = 0; i < player.maxItems; i++)
                {

                    if (items[i] != null && items[i].category == -74 && !items[i].HasContextTag("tree_seed_item"))
                    {
                        if (player.CurrentToolIndex != i)
                            switcher.SwitchIndex(i);

                        return;
                    }
                }
                return;
            }

            //Handles Crops
            //Trata da categoria plantações
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
                            switcher.SwitchIndex(i);

                        return;
                    }
                }
                return;
            }


            //Handles any other item
            //Trata de qualquer outro item
            for (int i = 0; i < player.maxItems; i++)
            {
                if (items[i] != null && items[i].category == aux && items[i].Name.Contains(item))
                {
                    if (player.CurrentItem != null && player.CurrentItem.category.ToString() == categorie)
                        return;

                    if (player.CurrentToolIndex != i)
                        switcher.SwitchIndex(i);

                    return;
                }
            }
            return;
        }

        //Checks if the button pressed matches the config
        //Verifica se o botão pressionado corresponde a config
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



