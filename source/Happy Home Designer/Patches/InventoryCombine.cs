/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace HappyHomeDesigner.Patches
{
	internal class InventoryCombine
	{
		public static void Apply(Harmony harmony)
		{
			harmony.Patch(
				typeof(InventoryMenu).GetMethod(nameof(InventoryMenu.rightClick)),
				transpiler: new(typeof(InventoryCombine), nameof(InsertCombineCheck))
			);
		}

		private static IEnumerable<CodeInstruction> InsertCombineCheck(IEnumerable<CodeInstruction> source, ILGenerator gen)
		{
			var il = new CodeMatcher(source, gen);
			var slot = gen.DeclareLocal(typeof(Item));
			var held = gen.DeclareLocal(typeof(Item));

			il.MatchStartForward(
				new CodeMatch(OpCodes.Callvirt, typeof(Tool).GetMethod(nameof(Tool.attach)))
			).MatchStartForward(
				new CodeMatch(OpCodes.Leave)
			);

			if (!il.AssertValid("Fusion patch failed. Could not find match point 1."))
				return null;

			var leaveTarget = il.Instruction.operand;

			il.MatchEndBackwards(
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldfld, typeof(InventoryMenu).GetField(nameof(InventoryMenu.actualInventory))),
				new(OpCodes.Ldloc_1),
				new(OpCodes.Callvirt, typeof(IList<Item>).GetMethod("get_Item")),
				new(OpCodes.Brfalse)
			);

			if (!il.AssertValid("Fusion patch failed. Could not find match point 2."))
				return null;

			il.Advance(1)
			.CreateLabel(out var jump)
			.InsertAndAdvance(

				// slot = actualInventory[i];
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldfld, typeof(InventoryMenu).GetField(nameof(InventoryMenu.actualInventory))),
				new(OpCodes.Ldloc_1),
				new(OpCodes.Callvirt, typeof(IList<Item>).GetMethod("get_Item")),
				new(OpCodes.Stloc, slot),

				// held = toAddTo;
				new(OpCodes.Ldarg_3),
				new(OpCodes.Stloc, held),
				
				// if (TryCombine(ref slot, ref held, playSound))
				new(OpCodes.Ldloca, slot),
				new(OpCodes.Ldloca, held),
				new(OpCodes.Ldarg_S, 4),
				new(OpCodes.Call, typeof(InventoryCombine).GetMethod(nameof(TryCombine))),
				new(OpCodes.Brfalse, jump),

				// actualInventory[i] = slot;
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldfld, typeof(InventoryMenu).GetField(nameof(InventoryMenu.actualInventory))),
				new(OpCodes.Ldloc_1),
				new(OpCodes.Ldloc, slot),
				new(OpCodes.Callvirt, typeof(IList<Item>).GetMethod("set_Item")),

				// return held;
				new(OpCodes.Ldloc, held),
				new(OpCodes.Stloc_3),
				new(OpCodes.Leave, leaveTarget)
			);

			//var d = il.InstructionEnumeration().ToList();

			return il.InstructionEnumeration();
		}

		public static bool TryCombine(ref Item slot, ref Item held, bool playSound)
		{
			// furniture in slot
			if(slot is Furniture furn)
			{
				// furniture catalog
				if (furn.ItemId is "1226")
				{
					// wall catalog
					if (held is Furniture heldFurn && heldFurn.ItemId is "1308")
					{
						held = null;
						slot = ItemRegistry.Create<Furniture>(ModEntry.manifest.UniqueID + "_Catalogue");

						if (playSound)
							Game1.playSound("axe");

						return true;
					}
				} 
				// combined catalog
				else if (furn.ItemId == ModEntry.manifest.UniqueID + "_Catalogue" && held is null)
				{
					held = new Furniture("1308", Vector2.Zero);
					slot = new Furniture("1226", Vector2.Zero);

					if (playSound)
						Game1.playSound("pickUpItem");

					return true;
				}
			}
			return false;
		}
	}
}
