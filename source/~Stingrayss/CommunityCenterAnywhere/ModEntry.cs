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

namespace CommunityCenterAnywhere
{
    public class ModEntry : Mod
    {

        public static ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            if (!Config.EnableMod)
                return;
            helper.Events.Display.MenuChanged += OnMenuUpdate;
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

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Warping Enabled?",
                tooltip: () => "Disables warping to the Community Center; this allows your game to softlock if you are not careful with bundle completion",
                getValue: () => Config.PlayerWarping,
                setValue: value => Config.PlayerWarping = value
            );
        }

        private Vector2 playerTile;
        private GameLocation playerLocation;
        private void OnMenuUpdate(object sender, MenuChangedEventArgs e)
        {

            if (!Context.IsWorldReady)
                return;

            //warp the player back to their original location when they are done
            if (e.OldMenu is JunimoNoteMenu && (!(e.NewMenu is JunimoNoteMenu)) && (!(playerLocation.name.Value is "CommunityCenter")) && Config.PlayerWarping)
                Game1.warpFarmer(playerLocation.name, (int)playerTile.X, (int)playerTile.Y, Game1.player.facingDirection);

            if (e.NewMenu is JunimoNoteMenu menu)
            {
                //update player location only if they have not previously been in the Community Center menu
                if (!(e.OldMenu is JunimoNoteMenu))
                {
                    playerTile = Game1.player.getTileLocation();
                    playerLocation = Game1.player.currentLocation;
                }

                //warp the player to the Community Center so that bundle completion will be counted
                if (!(Game1.player.currentLocation.name.Value is "CommunityCenter") && Config.PlayerWarping)
                    Game1.warpFarmer(Game1.getLocationFromName("CommunityCenter").name, 32, 18, Game1.player.facingDirection);

                foreach (Bundle bundle in menu.bundles)
                {
                    if (bundle.name is "2,500g" || bundle.name is "5,000g" || bundle.name is "10,000g" || bundle.name is "25,000g")
                        menu.purchaseButton = new ClickableTextureComponent(new Rectangle(menu.xPositionOnScreen + 800, menu.yPositionOnScreen + 504, 260, 72), menu.noteTexture, new Rectangle(517, 286, 65, 20), 4f);

                    bundle.depositsAllowed = true;
                }

            }
        }
    }
}