using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;


namespace HotbarHotswap
{
    public class ModEntry : Mod
    {
        private static IReflectionHelper _reflection;

        private readonly Dictionary<int, InputButton[]> _keyBindings = new Dictionary<int, InputButton[]>();

        public override void Entry(IModHelper helper)
        {
            _keyBindings[0] = Game1.options.inventorySlot1;
            _keyBindings[1] = Game1.options.inventorySlot2;
            _keyBindings[2] = Game1.options.inventorySlot3;
            _keyBindings[3] = Game1.options.inventorySlot4;
            _keyBindings[4] = Game1.options.inventorySlot5;
            _keyBindings[5] = Game1.options.inventorySlot6;
            _keyBindings[6] = Game1.options.inventorySlot7;
            _keyBindings[7] = Game1.options.inventorySlot8;
            _keyBindings[8] = Game1.options.inventorySlot9;
            _keyBindings[9] = Game1.options.inventorySlot10;
            _keyBindings[10] = Game1.options.inventorySlot11;
            _keyBindings[11] = Game1.options.inventorySlot12;

            _reflection = helper.Reflection;
            InputEvents.ButtonPressed += Update;
        }


        private void Update(object sender, EventArgsInput e)
        {
            if (!(Game1.activeClickableMenu is GameMenu)) return;
            var menuList = _reflection.GetField<List<IClickableMenu>>(Game1.activeClickableMenu, "pages").GetValue();

            foreach (var menu in menuList)
            {
                if (!(menu is InventoryPage)) continue;
                var item = _reflection.GetField<Item>(menu, "hoveredItem").GetValue();
                if (item == null) return;

                var newSlot = -1;
                var oldSlot = Game1.player.Items.IndexOf(item);

                foreach (var ent in _keyBindings)
                {
                    foreach (var inputButton in ent.Value)
                    {
                        if (inputButton.key.ToString().Equals(e.Button.ToString()))
                        {
                            newSlot = ent.Key;
                        }
                    }
                }

                if (newSlot == -1) return;

                var currentItem = Game1.player.Items[newSlot];
                Game1.player.items[newSlot] = item;
                Game1.player.items[oldSlot] = currentItem;
            }
        }
    }
}