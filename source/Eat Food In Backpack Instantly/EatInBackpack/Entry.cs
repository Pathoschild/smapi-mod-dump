/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/philosquare/EatInBackpack
**
*************************************************/

using System;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace EatInBackpack
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!e.Button.Equals(SButton.X))
            {
                return;
            }
            if (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.GetCurrentPage() is InventoryPage inventory && inventory.hoveredItem != null)
            {
                var hoveredItem = inventory.hoveredItem;
                Monitor.Log($"hoveredItem: {hoveredItem}, QualifiedItemId: {hoveredItem.QualifiedItemId}", LogLevel.Info);
                var who = Game1.player;
                if (hoveredItem is StardewValley.Object o && o.Edibility != -300 && !who.isEating)
                {
                    who.eatObject(o);
                    if (who.isEating)
                    {
                        Monitor.Log($"eat food: {o}", LogLevel.Info);
                        if (--o.Stack <= 0)
                        {
                            who.removeItemFromInventory(o);
                            Monitor.Log($"remove food: {o}", LogLevel.Info);
                        }
                    }
                }
            }
        }
    }
}