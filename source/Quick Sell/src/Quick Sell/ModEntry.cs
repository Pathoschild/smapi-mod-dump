/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AlejandroAkbal/Stardew-Valley-Quick-Sell-Mod
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Quick_Sell
{
    public class ModEntry : Mod
    {
        public static Mod Instance;

        public static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Get Generic Mod Config Menu's API (if it's installed)
            var genericModConfigMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (genericModConfigMenu is null)
                return;

            // Register mod
            genericModConfigMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // Add some config options
            genericModConfigMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("config.qs_SellKey_name"),
                tooltip: () => Helper.Translation.Get("config.qs_SellKey_tooltip"),
                getValue: () => Config.SellKey,
                setValue: value => Config.SellKey = value
            );

            genericModConfigMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("config.qs_CheckIfItemsCanBeShipped_name"),
                tooltip: () => Helper.Translation.Get("config.qs_CheckIfItemsCanBeShipped_tooltip"),
                getValue: () => Config.CheckIfItemsCanBeShipped,
                setValue: value => Config.CheckIfItemsCanBeShipped = value
            );

            genericModConfigMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("config.qs_EnableHUDMessages_name"),
                tooltip: () => Helper.Translation.Get("config.qs_EnableHUDMessages_tooltip"),
                getValue: () => Config.EnableHUDMessages,
                setValue: value => Config.EnableHUDMessages = value
            );
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            //if (!Context.IsPlayerFree)
            //    return;

            //if (!Game1.displayHUD)
            //    return;

            if (Config.SellKey.JustPressed())
                OnSellButtonPressed(sender, e);
        }

        private void OnSellButtonPressed(object sender, ButtonsChangedEventArgs e)
        {
            Item item = ModPlayer.GetHoveredItem();

            if (item == null)
            {
                ModLogger.Trace("Item was null.");
                return;
            }

            ModLogger.Trace($"{Game1.player.Name} pressed {e.ToString()} and has selected {item}.");

            if (Config.CheckIfItemsCanBeShipped == true && ModPlayer.CheckIfItemCanBeShipped(item) == false)
            {
                ModLogger.Info("Item can't be shipped.");
                return;
            }

            // Ship item
            Game1.getFarm().shipItem(item, Game1.player);

            ModUtils.SendHUDMessageRespectingConfig(Helper.Translation.Get("messages.qs_ItemShipped", new { itemStack = item.Stack, itemName = item.DisplayName }));

            Game1.playSound("Ship");
        }
    }
}
