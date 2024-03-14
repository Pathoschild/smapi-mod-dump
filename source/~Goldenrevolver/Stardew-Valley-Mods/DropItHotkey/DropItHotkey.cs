/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace DropItHotkey
{
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;
    using StardewValley.Menus;

    public class DropItHotkey : Mod
    {
        private Config config;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<Config>();

            Helper.Events.GameLoop.GameLaunched += delegate { Config.SetUpModConfigMenu(config, this); };

            helper.Events.Input.ButtonsChanged += CheckForHotkey;
        }

        private static void DropItem(Farmer player, Item toDrop)
        {
            // typo by the base game (verified for 1.6.0 alpha)
            Game1.playSound("throwDownITem");
            Game1.createItemDebris(toDrop, player.getStandingPosition(), player.FacingDirection, null, -1).DroppedByPlayerID.Value = player.UniqueMultiplayerID;
        }

        private void CheckForHotkey(object sender, ButtonsChangedEventArgs e)
        {
            if (config.DropKey.JustPressed())
            {
                Helper.Input.SuppressActiveKeybinds(config.DropKey);
                if (Context.IsPlayerFree)
                {
                    Item item = Game1.player.CurrentItem;
                    if (item != null && item.canBeDropped() && item.canBeTrashed())
                    {
                        DropItem(Game1.player, item.getOne());
                        Game1.player.reduceActiveItemByOne();
                    }
                }
                else if (Context.IsWorldReady && Game1.activeClickableMenu is GameMenu menu)
                {
                    if (menu.GetCurrentPage() is InventoryPage)
                    {
                        Item heldItem = Game1.player.CursorSlotItem;
                        if (heldItem != null && heldItem.canBeDropped() && heldItem.canBeTrashed())
                        {
                            Game1.player.CursorSlotItem = null;
                            DropItem(Game1.player, heldItem);
                        }
                    }
                }
            }
        }
    }
}