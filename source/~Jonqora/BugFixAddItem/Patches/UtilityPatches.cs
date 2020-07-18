using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;
using Netcode;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BugFixAddItem
{
    class UtilityPatches
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;

		private static HarmonyInstance Harmony => ModEntry.Instance.Harmony;

		public static void Apply()
		{
			Harmony.Patch(
				original: AccessTools.Method(typeof(Utility), nameof(Utility.addItemToInventory)),
                //prefix: new HarmonyMethod(AccessTools.Method(typeof(UtilityPatches), nameof(UtilityPatches.addItemToInventory_Prefix))),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(UtilityPatches), nameof(UtilityPatches.addItemToInventory_Transpiler)))
			);
		}

        // No longer used after switching to transpiler
        public static bool addItemToInventory_Prefix(
            Item item,
            int position,
            IList<Item> items, 
            ItemGrabMenu.behaviorOnItemSelect onAddFunction,
            ref Item __result)
        {
            try
            {
                Monitor.Log($"Invoked Utility.addItemToInventory with onAddFunction: {onAddFunction}", LogLevel.Debug);

                // Reimplemented game code
                if (items.Equals((object)Game1.player.items) && item is Object && (item as Object).specialItem)
                {
                    if ((bool)(NetFieldBase<bool, NetBool>)(item as Object).bigCraftable)
                    {
                        if (!Game1.player.specialBigCraftables.Contains((bool)(NetFieldBase<bool, NetBool>)(item as Object).isRecipe ? -(int)(NetFieldBase<int, NetInt>)(item as Object).parentSheetIndex : (int)(NetFieldBase<int, NetInt>)(item as Object).parentSheetIndex))
                            Game1.player.specialBigCraftables.Add((bool)(NetFieldBase<bool, NetBool>)(item as Object).isRecipe ? -(int)(NetFieldBase<int, NetInt>)(item as Object).parentSheetIndex : (int)(NetFieldBase<int, NetInt>)(item as Object).parentSheetIndex);
                    }
                    else if (!Game1.player.specialItems.Contains((bool)(NetFieldBase<bool, NetBool>)(item as Object).isRecipe ? -(int)(NetFieldBase<int, NetInt>)(item as Object).parentSheetIndex : (int)(NetFieldBase<int, NetInt>)(item as Object).parentSheetIndex))
                        Game1.player.specialItems.Add((bool)(NetFieldBase<bool, NetBool>)(item as Object).isRecipe ? -(int)(NetFieldBase<int, NetInt>)(item as Object).parentSheetIndex : (int)(NetFieldBase<int, NetInt>)(item as Object).parentSheetIndex);
                }
                if (position < 0 || position >= items.Count)
                { 
                    __result = item; 
                    return false;
                }
                if (items[position] == null)
                {
                    items[position] = item;
                    Utility.checkItemFirstInventoryAdd(item);
                    if (onAddFunction != null)
                        onAddFunction(item, Game1.player);
                    __result = (Item)null;
                    return false;
                }
                if (items[position].maximumStackSize() != -1 && items[position].Name.Equals(item.Name) && (!(item is Object) || !(items[position] is Object) || (NetFieldBase<int, NetInt>)(item as Object).quality == (items[position] as Object).quality && (NetFieldBase<int, NetInt>)(item as Object).parentSheetIndex == (items[position] as Object).parentSheetIndex) && item.canStackWith((ISalable)items[position]))
                {
                    Utility.checkItemFirstInventoryAdd(item);
                    int stack = items[position].addToStack(item);
                    if (stack <= 0)
                    {
                        __result = (Item)null;
                        return false;
                    }
                    item.Stack = stack;
                    if (onAddFunction != null)
                        onAddFunction(item, Game1.player);
                    __result = item;
                    return false;
                }
                Item obj = items[position];
                if (position == Game1.player.CurrentToolIndex && items.Equals((object)Game1.player.items) && obj != null)
                {
                    obj.actionWhenStopBeingHeld(Game1.player);
                    item.actionWhenBeingHeld(Game1.player);
                }
                Utility.checkItemFirstInventoryAdd(item);
                items[position] = item;
                if (onAddFunction != null)
                    onAddFunction(item, Game1.player);
                __result = obj;
                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(addItemToInventory_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // Run original code
            }
        }
        
        public static IEnumerable<CodeInstruction> addItemToInventory_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            try
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count - 3; i++)
                {
                    // Find any null value appearing as the last argument of a ItemGrabMenu.behaviorOnItemSelect delegate method call
                    if (codes[i].opcode == OpCodes.Ldarg_3 &&
                        codes[i + 1].opcode == OpCodes.Ldarg_0 && 
                        codes[i + 2].opcode == OpCodes.Ldnull && // The (Farmer) null value we want to change
                        codes[i + 3].opcode == OpCodes.Callvirt &&
                        codes[i + 3].operand.ToString() == "Void Invoke(StardewValley.Item, StardewValley.Farmer)")
                    {                    
                        // change (Farmer) null to Game1.player
                        codes[i + 2] = new CodeInstruction(OpCodes.Call, typeof(Game1).GetProperty("player").GetGetMethod());

                        Monitor.Log($"Edited OpCode: {codes[i + 2]}", LogLevel.Debug);
                    }
                }
                return codes.AsEnumerable();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(addItemToInventory_Transpiler)}:\n{ex}", LogLevel.Error);
                return instructions; // use original code
            }
        }
    }
}
