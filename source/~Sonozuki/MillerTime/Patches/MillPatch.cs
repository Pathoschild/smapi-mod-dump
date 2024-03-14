/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MillerTime.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="StardewValley.Buildings.Mill"/> class.</summary>
    internal class MillPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="StardewValley.Buildings.Mill.doAction(Microsoft.Xna.Framework.Vector2, StardewValley.Farmer)"/> method.</summary>
        /// <param name="tileLocation">The tile on the mill the player clicked on.</param>
        /// <param name="who">The player who clicked on the mill.</param>
        /// <param name="__instance">The current <see cref="StardewValley.Buildings.Mill"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to add the custom millables.</remarks>
        internal static bool DoActionPrefix(Vector2 tileLocation, Farmer who, Mill __instance, ref bool __result)
        {
            if (__instance.daysOfConstructionLeft <= 0)
            {
                // check if the player is clicking on the part of the mill to input new items
                if (tileLocation.X == __instance.tileX + 1 && tileLocation.Y == __instance.tileY + 1)
                {
                    // ensure player is holding an item
                    if (who != null && who.ActiveObject != null)
                    {
                        var item = who.ActiveObject;

                        // ensure held item is millable
                        var isItemMillable = false;
                        if (ModEntry.Instance.Recipes.Any(recipe => recipe.InputId == item.ParentSheetIndex))
                            isItemMillable = true;
                        if (!isItemMillable)
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:CantMill"));
                            __result = false;
                            return false;
                        }

                        // place held object in mill
                        who.ActiveObject = null;
                        item = (StardewValley.Object)MillPatch.AddItemToThisInventoryList(item, __instance.input.Value.items, 36);
                        if (item != null)
                        {
                            who.ActiveObject = item;
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:MillFull"));
                        }

                        typeof(Mill).GetField("hasLoadedToday", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, true);
                        Game1.playSound("Ship");
                    }
                }

                // check if the player is clicking on the part of the mill to pick up processed items
                else if (tileLocation.X == __instance.tileX + 3 && tileLocation.Y == __instance.tileY + 1)
                {
                    Utility.CollectSingleItemOrShowChestMenu(__instance.output.Value, __instance);
                    __result = true;
                    return false;
                }
            }

            // call the base doAction method
            // this approach isn't ideal but when using regular reflection and invoking the MethodInfo directly, it would call this patch (instead of the base method) resulting in a stack overflow
            // https://stackoverflow.com/questions/4357729/use-reflection-to-invoke-an-overridden-base-method/14415506#14415506
            var baseMethod = typeof(Building).GetMethod("doAction", BindingFlags.Public | BindingFlags.Instance);
            var functionPointer = baseMethod.MethodHandle.GetFunctionPointer();
            var function = (Func<Vector2, Farmer, bool>)Activator.CreateInstance(typeof(Func<Vector2, Farmer, bool>), __instance, functionPointer);
            __result = function(tileLocation, who);
            return false;
        }

        /// <summary>The prefix for the <see cref="StardewValley.Buildings.Mill.dayUpdate(int)"/> method.</summary>
        /// <param name="dayOfMonth">The current day of month.</param>
        /// <param name="__instance">The current <see cref="StardewValley.Buildings.Mill"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to add the custom millables.</remarks>
        internal static bool DayUpdatePrefix(int dayOfMonth, Mill __instance)
        {
            // reset hasLoadedToday
            typeof(Mill).GetField("hasLoadedToday", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, false);

            // convert all inputs
            for (int i = __instance.input.Value.items.Count - 1; i >= 0; i--)
            {
                // ensure there's an item in the input slot
                var item = __instance.input.Value.items[i];
                if (item == null)
                    continue;

                // get the recipe output
                var recipe = ModEntry.Instance.Recipes.FirstOrDefault(r => r.InputId == item.ParentSheetIndex);
                if (recipe == null)
                {
                    ModEntry.Instance.Monitor.Log($"Failed to retreive recipe that has an input id of {item.ParentSheetIndex}, mill input will be deleted", LogLevel.Error);
                    __instance.input.Value.items[i] = null;
                }
                var outputObject = new StardewValley.Object(recipe.Output.Id, item.Stack * recipe.Output.Amount);

                // try to add the item to the output
                if (Utility.canItemBeAddedToThisInventoryList(outputObject, __instance.output.Value.items, 36))
                    __instance.input.Value.items[i] = MillPatch.AddItemToThisInventoryList(outputObject, __instance.output.Value.items, 36);
            }

            // call the base dayUpdate method
            // this approach isn't ideal but when using regular reflection and invoking the MethodInfo directly, it would call this patch (instead of the base method) resulting in a stack overflow
            // https://stackoverflow.com/questions/4357729/use-reflection-to-invoke-an-overridden-base-method/14415506#14415506
            var baseMethod = typeof(Building).GetMethod("dayUpdate", BindingFlags.Public | BindingFlags.Instance);
            var functionPointer = baseMethod.MethodHandle.GetFunctionPointer();
            var function = (Func<int, bool>)Activator.CreateInstance(typeof(Func<int, bool>), __instance, functionPointer);
            function(dayOfMonth);
            return false;
        }

        /// <summary>The prefix for the <see cref="StardewValley.Buildings.Mill.draw(Microsoft.Xna.Framework.Graphics.SpriteBatch)"/> method.</summary>
        /// <param name="b">The <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> to draw the mill to.</param>
        /// <param name="__instance">The current <see cref="StardewValley.Buildings.Mill"/> instance being patched.</param>
        /// <returns><see lanword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to remove the check when drawing the item overlay on the output chest (so any item in the output chest can get drawn).</remarks>
        internal static bool DrawPrefix(SpriteBatch b, Mill __instance)
        {
            if (__instance.isMoving)
                return false;

            // draw mill differently when under construction
            if (__instance.daysOfConstructionLeft > 0)
            {
                __instance.drawInConstruction(b);
                return false;
            }

            // get private members
            var baseSourceRect = (Rectangle)typeof(Mill).GetField("baseSourceRect", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var alpha = (NetFloat)typeof(Mill).GetField("alpha", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var hasLoadedToday = (bool)typeof(Mill).GetField("hasLoadedToday", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            __instance.drawShadow(b);

            // draw base
            b.Draw(
                texture: __instance.texture.Value,
                position: Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(__instance.tileX * 64, __instance.tileY * 64 + __instance.tilesHigh * 64)),
                sourceRectangle: baseSourceRect,
                color: __instance.color.Value * alpha,
                rotation: 0,
                origin: new Vector2(0, __instance.texture.Value.Bounds.Height),
                scale: 4,
                effects: SpriteEffects.None,
                layerDepth: (__instance.tileY + __instance.tilesHigh - 1) * 64 / 10000f
            );
            
            // draw spinning propeller
            b.Draw(
                texture: __instance.texture.Value,
                position: Game1.GlobalToLocal(
                    viewport: Game1.viewport,
                    globalPosition: new Vector2(__instance.tileX * 64 + 32, __instance.tileY * 64 + __instance.tilesHigh * 64 + 4)),
                sourceRectangle: new Rectangle(
                    x: 64 + (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 800 / 89 * 32 % 160,
                    y: (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 800 / 89 * 32 / 160 * 32,
                    width: 32,
                    height: 32),
                color: __instance.color.Value * alpha,
                rotation: 0,
                origin: new Vector2(0f, __instance.texture.Value.Bounds.Height),
                scale: 4,
                effects: SpriteEffects.None,
                layerDepth: (__instance.tileY + __instance.tilesHigh) * 64 / 10000f
            );
            
            // draw sprinning gear if something has been placing in it
            if (hasLoadedToday)
                b.Draw(
                    texture: __instance.texture.Value,
                    position: Game1.GlobalToLocal(
                        viewport: Game1.viewport,
                        globalPosition: new Vector2(__instance.tileX * 64 + 52, __instance.tileY * 64 + __instance.tilesHigh * 64 + 276)),
                    sourceRectangle: new Rectangle(64 + (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 700 / 100 * 21, 72, 21, 8),
                    color: __instance.color.Value * alpha,
                    rotation: 0f,
                    origin: new Vector2(0f, __instance.texture.Value.Bounds.Height),
                    scale: 4f,
                    effects: SpriteEffects.None,
                    layerDepth: (__instance.tileY + __instance.tilesHigh) * 64 / 10000f
                );

            // draw object overlay when output is collectible
            if (__instance.output.Value.items.Count > 0 && __instance.output.Value.items[0] != null)
            {
                float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                b.Draw(
                    texture: Game1.mouseCursors,
                    position: Game1.GlobalToLocal(
                        viewport: Game1.viewport,
                        globalPosition: new Vector2(__instance.tileX * 64 + 192, __instance.tileY * 64 - 96 + yOffset)),
                    sourceRectangle: new Rectangle(141, 465, 20, 24),
                    color: Color.White * .75f,
                    rotation: 0,
                    origin: Vector2.Zero,
                    scale: 4,
                    effects: SpriteEffects.None,
                    layerDepth: (__instance.tileY + 1 * 64) / 10000f + .0000001f + __instance.tileX / 10000f
                );
                
                b.Draw(
                    texture: Game1.objectSpriteSheet,
                    position: Game1.GlobalToLocal(
                        viewport: Game1.viewport,
                        globalPosition: new Vector2(__instance.tileX * 64 + 192 + 32 + 4, __instance.tileY * 64 - 64 + 8 + yOffset)),
                    sourceRectangle: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, __instance.output.Value.items[0].parentSheetIndex, 16, 16),
                    color: Color.White * .75f,
                    rotation: 0,
                    origin: new Vector2(8),
                    scale: 4,
                    effects: SpriteEffects.None,
                    layerDepth: (__instance.tileY + 1) * 64 / 10000f + .000001f + __instance.tileX / 10000f
                );
            }

            return false;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Adds an item to a list of items representing an inventory.</summary>
        /// <param name="item">The item to add to the inventory list.</param>
        /// <param name="inventory">The list of inventory 'slots' to add the item to.</param>
        /// <param name="maxInventorySize">The most slots that <paramref name="inventory"/> can be expanded to.</param>
        /// <returns>The remaining item that couldn't be fit into <paramref name="inventory"/>.</returns>
        /// <remarks>This reimplements the base game method to fix a bug that was causing mills to produce a max of two stacks of output.<br/>For later reference, the bug report can be seen at: <see href="https://forums.stardewvalley.net/threads/bug-mill-produces-a-max-of-two-stacks-of-output-for-any-one-given-input.4286/"/></remarks>
        private static Item AddItemToThisInventoryList(Item item, IList<Item> inventory, int maxInventorySize = -1)
        {
            // ensure there is atleast one item
            if (item.Stack == 0)
                item.Stack = 1;

            // try to top up any existing stacks first
            foreach (Item it in inventory)
                if (it != null && it.canStackWith(item) && it.getRemainingStackSpace() > 0)
                {
                    item.Stack = it.addToStack(item);
                    if (item.Stack <= 0)
                        return null;
                }

            // next try to fill up any empty slots
            for (int j = inventory.Count - 1; j >= 0; j--)
                if (inventory[j] == null)
                {
                    if (item.Stack <= item.maximumStackSize())
                    {
                        inventory[j] = item;
                        return null;
                    }

                    inventory[j] = item.getOne();
                    inventory[j].Stack = item.maximumStackSize();
                    item.Stack -= item.maximumStackSize();
                }

            // next create new slots to fill up (if slots can be created)
            while (maxInventorySize != -1 && inventory.Count < maxInventorySize)
            {
                if (item.Stack > item.maximumStackSize())
                {
                    Item tmp = item.getOne();
                    tmp.Stack = item.maximumStackSize();
                    (item as StardewValley.Object).stack.Value -= item.maximumStackSize();
                    inventory.Add(tmp);
                    continue;
                }

                inventory.Add(item);
                return null;
            }

            // return the excess
            return item;
        }
    }
}
