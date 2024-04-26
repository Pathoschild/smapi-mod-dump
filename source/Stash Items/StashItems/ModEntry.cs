/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack-hill/stardew-valley-stash-items
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace StashItems
{
    public class ModEntry : Mod
    {
        internal static ModConfig Config { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Stash radius",
                tooltip: () => "The radius in tiles around the player to search for inventories to stash items into",
                getValue: () => Config.Radius,
                setValue: value => Config.Radius = value,
                min: 1,
                max: 50
            );
            configMenu.AddKeybindList(
                ModManifest,
                name: () => "Stash items keybind",
                tooltip: () => "Stashes items in nearby chests",
                getValue: () => Config.StashHotKey,
                setValue: value => Config.StashHotKey = value
            );
        }

        private static void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (Config.StashHotKey.JustPressed())
                {
                    ItemStashing.StashItemsInNearbyChests(Config.Radius);
                }
            }
        }
	}
}
