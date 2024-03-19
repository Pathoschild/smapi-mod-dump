/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/S1mmyy/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AutoTrash
{
    public class ModEntry : Mod
    {
        internal ITranslationHelper i18n => Helper.Translation;
        internal ModConfig Config;
        public bool MinesOnly { get; private set; }
        public IList<string> DeleteItems { get; private set; }
        public SButton ToggleTrash { get; private set; }
        public bool IsEnabled { get; set; } = false;

        public override void Entry(IModHelper helper)
        {
            string startingMessage = i18n.Get("template.start", new { mod = helper.ModRegistry.ModID, folder = helper.DirectoryPath });
            Monitor.Log(startingMessage, LogLevel.Trace);

            Config = helper.ReadConfig<ModConfig>();
            MinesOnly = Config.MinesOnly;
            ToggleTrash = Config.ToggleTrash;
            if (Config.DeleteItems != null)
            {
                DeleteItems = Config.DeleteItems;
            }

            helper.Events.Player.InventoryChanged += Player_InventoryChanged;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && e.Button == ToggleTrash)
            {
                IsEnabled = !IsEnabled;
                Monitor.Log("AutoTrashing " + (IsEnabled ? "enabled" : "disabled"), LogLevel.Info);
                Game1.addHUDMessage(new HUDMessage("AutoTrashing " + (IsEnabled ? "enabled" : "disabled")) { noIcon = true });
            }
        }

        private void Player_InventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (e.IsLocalPlayer && IsEnabled && (MinesOnly ? Game1.CurrentMineLevel > 0 : Game1.CurrentMineLevel >= 0))
            {
                //Remove any newly picked up items that you have in the list.
                foreach (Item newItem in e.Added)
                {
                    if (CheckItem(newItem))
                    {
                        Utility.trashItem(newItem);
                        e.Player.removeItemFromInventory(newItem);
                    }
                }

                //If you already have the item, only remove new items that you gain.
                foreach (ItemStackSizeChange newStacks in e.QuantityChanged)
                {
                    if (CheckItem(newStacks.Item))
                    {
                        e.Player.Items.ReduceId(newStacks.Item.QualifiedItemId, newStacks.NewSize - newStacks.OldSize);
                    }
                }
            }
        }

        //Compares the item added to the list defined from the config file.
        private bool CheckItem(Item itemCheck)
        {
            foreach (string idNum in DeleteItems)
            {
				if (itemCheck is Object && ItemRegistry.QualifyItemId(idNum) == itemCheck.QualifiedItemId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}