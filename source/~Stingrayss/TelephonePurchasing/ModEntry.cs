/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stingrayss/StardewValley
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;

namespace TelephonePurchasing
{
    public class ModEntry : Mod
    {
        public static ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            if (!Config.EnableMod)
                return;
            helper.Events.Display.MenuChanged += UpdatePhone;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Mod Enabled?",
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value
            );
        }

    private Vector2 playerTile;
        private GameLocation playerLocation;
        private void UpdatePhone(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            //if our last menu was building or animal purchase, warp the farmer back to their original location upon exit
            if (e.OldMenu is CarpenterMenu || e.OldMenu is PurchaseAnimalsMenu)
            {
                //if the player uses the shop from it's actual location, don't warp
                if (playerLocation.name.Value is "ScienceHouse" || playerLocation.name.Value is "AnimalShop")
                    return;

                Game1.warpFarmer(playerLocation.name, (int)playerTile.X, (int)playerTile.Y, Game1.player.facingDirection);
            }

            //return if we are not in any of the telephone menus
            if (!(e.NewMenu is ShopMenu) && !(e.NewMenu is CarpenterMenu) && !(e.NewMenu is PurchaseAnimalsMenu))
                return;
            
            if (e.NewMenu is ShopMenu menu)
            {
                menu.readOnly = false;
                return;
            }

            playerTile = Game1.player.getTileLocation();
            playerLocation = Game1.player.currentLocation;

            if (e.NewMenu is PurchaseAnimalsMenu aMenu)
            {
                aMenu.readOnly = false;
                return;
            }

            if (e.NewMenu is CarpenterMenu cMenu)
            {
                cMenu.readOnly = false;
                cMenu.upgradeIcon.visible = true;
                cMenu.demolishButton.visible = true;
                cMenu.moveButton.visible = true;
                cMenu.okButton.visible = true;
                cMenu.paintButton.visible = true;
                return;
            }
        }
    }
}