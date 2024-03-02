/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nrobinson12/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace StaticInventory
{
    internal class StaticInventory
    {
        private int toolbarOffset = 0;
        private bool isShifted  = false;
        private enum IsShifting { None, Left, Right }

        public StaticInventory(ModData modData)
        {
            modData ??= new ModData();
            toolbarOffset = modData.ToolbarOffset;
            isShifted = modData.IsShifted;
            if (isShifted) ShiftInventory(true);
        }

        public ModData GetModData()
        {
            return new ModData(toolbarOffset, isShifted);
        }

        public void OnButtonPressed()
        {
            IsShifting isShifting = CheckIfIsShifting();
            if (isShifting == IsShifting.Left) ShiftToolbarOffset(false);
            else if (isShifting == IsShifting.Right) ShiftToolbarOffset(true);
        }

        public void OnMenuChange()
        {
            if (toolbarOffset == 0) return;

            // If we are opening any menu
            if (Game1.activeClickableMenu != null) ShiftInventory(false);

            // If we are closing any menu
            else if (Game1.activeClickableMenu == null) ShiftInventory(true);
        }

        public void SetFirstRow()
        {
            if (!isShifted) ShiftInventory(true);
            toolbarOffset = 0;
        }

        private static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        /// <summary>
        /// Update the toolbarOffset based on whether the user shifted left/right.
        /// </summary>
        /// <param name="right"></param>
        private void ShiftToolbarOffset(bool right)
        {
            IList<Item> items = Game1.player.Items;
            bool usingTool = Game1.player.UsingTool;

            // Same logic as Game1.player.shiftToolbar
            if (items == null || items.Count <= 12 || usingTool || Game1.dialogueUp || (!Game1.pickingTool && !Game1.player.CanMove) || Game1.player.areAllItemsNull() || Game1.eventUp || Game1.farmEvent != null)
            {
                return;
            }

            int numRows = items.Count / 12;
            if (right) toolbarOffset = Mod(toolbarOffset + 1, numRows);
            else toolbarOffset = Mod(toolbarOffset - 1, numRows);
        }

        /// <summary>
        /// Shift the inventory left/right based on param, toolbarOffset amount of times.
        /// </summary>
        /// <param name="right"></param>
        private void ShiftInventory(bool right)
        {
            NetObjectList<Item> items = new(Game1.player.Items);
            int offsetAmount = toolbarOffset * 12;

            // Shift for toolbar view
            if (right)
            {
                List<Item> range = items.GetRange(0, offsetAmount);
                items.RemoveRange(0, offsetAmount);
                items.AddRange(range);
                isShifted = true;
            }

            // Shift for menu view
            else
            {
                List<Item> range2 = items.GetRange(items.Count - offsetAmount, offsetAmount);
                for (int i = 0; i < items.Count - offsetAmount; i++)
                {
                    range2.Add(items[i]);
                }

                items.Set(range2);
                isShifted = false;
            }

            Game1.player.Items = items;
        }

        /// <summary>
        /// Check if the user pressed a shift toolbar button.
        /// </summary>
        /// <returns>IsShifting enum for which way the user is shifting.</returns>
        private static IsShifting CheckIfIsShifting()
        {
            KeyboardState currentKBState = Game1.GetKeyboardState();
            GamePadState currentPadState = Game1.input.GetGamePadState();

            // Clicking the toolbar button that was set
            if (Game1.isOneOfTheseKeysDown(currentKBState, Game1.options.toolbarSwap) && Game1.areAllOfTheseKeysUp(Game1.oldKBState, Game1.options.toolbarSwap))
            {
                return currentKBState.IsKeyDown(Keys.LeftControl) ? IsShifting.Left : IsShifting.Right;
            }

            if (Game1.options.gamepadControls)
            {
                // Clicking the right shoulder button on controller
                if (currentPadState.IsButtonDown(Buttons.RightShoulder) && !Game1.oldPadState.IsButtonDown(Buttons.RightShoulder)) return IsShifting.Right;
                if (currentPadState.IsButtonDown(Buttons.LeftShoulder) && !Game1.oldPadState.IsButtonDown(Buttons.LeftShoulder)) return IsShifting.Left;
            }

            // Debug mode keybindings
            if (!Game1.IsChatting && Game1.player.freezePause <= 0 && Game1.debugMode)
            {
                if (currentKBState.IsKeyDown(Keys.B) && !Game1.oldKBState.IsKeyDown(Keys.B)) return IsShifting.Left;
                if (currentKBState.IsKeyDown(Keys.N) && !Game1.oldKBState.IsKeyDown(Keys.N)) return IsShifting.Right;
            }

            // No shifting
            return IsShifting.None;
        }
    }
}
