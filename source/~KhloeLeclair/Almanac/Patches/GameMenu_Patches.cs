/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HarmonyLib;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Almanac.Patches;

public static class GameMenu_Patches {

	private static IMonitor? Monitor;

	public static void Patch(ModEntry mod) {
		Monitor = mod.Monitor;

		try {
			mod.Harmony!.Patch(
				original: AccessTools.Constructor(typeof(GameMenu), new Type[] { typeof(bool) }),
				postfix: new HarmonyMethod(typeof(GameMenu_Patches), nameof(Ctor_Postfix))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(GameMenu), nameof(GameMenu.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(GameMenu_Patches), nameof(Draw_Transpiler))
			);

			mod.Harmony!.Patch(
				original: AccessTools.Method(typeof(GameMenu), nameof(GameMenu.getTabNumberFromName)),
				prefix: new HarmonyMethod(typeof(GameMenu_Patches), nameof(getTabNumberFromName_Prefix))
			);

		} catch (Exception ex) {
			mod.Log("An error occurred while registering a harmony patch for GameMenu.", LogLevel.Warn, ex);
		}
	}

	public static void Ctor_Postfix(GameMenu __instance) {
		try {
			int xPos = -1;
			int lastId = -1;
			ClickableComponent? rightTab = null;
			for(int i = 0; i < __instance.tabs.Count; i++) {
				ClickableComponent tab = __instance.tabs[i];
				if (tab.bounds.X > xPos) {
					xPos = tab.bounds.X;
					rightTab = tab;
				}

				if (tab.myID > lastId)
					lastId = tab.myID;
			}

			var cmp = new ClickableComponent(
				new Rectangle(xPos + 80, __instance.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64),
				"leclair.almanac:books",
				"Book Collection"
			) {
				myID = lastId + 1,
				leftNeighborID = rightTab?.myID ?? ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = rightTab?.rightNeighborID ?? ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = 8,
				tryDefaultIfNoDownNeighborExists = true,
				fullyImmutable = true
			};

			if (rightTab != null)
				rightTab.rightNeighborID = lastId + 1;

			__instance.tabs.Add(cmp);
			__instance.pages[__instance.currentTab]?.allClickableComponents?.Add(cmp);

			__instance.pages.Add(new Menus.BookCollectionMenu(ModEntry.Instance, __instance.xPositionOnScreen, __instance.yPositionOnScreen, __instance.width, __instance.height, false));

		} catch (Exception ex) {
			Monitor?.Log($"An error occurred within GameMenu Constructor Postfix.", LogLevel.Warn);
			Monitor?.Log($"Details:\n{ex}", LogLevel.Warn);
		}
	}

	public static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions) {
		bool did_draw_farmer = false;
		bool did_draw_button = false;

		CodeInstruction[] instrs = instructions.ToArray();

		for(int i =0; i < instrs.Length; i++) {
			CodeInstruction instr = instrs[i];
			CodeInstruction? next = i + 1 < instrs.Length ? instrs[i + 1] : null;

			if (instr.opcode == OpCodes.Callvirt && instr.operand is MethodInfo method && method == AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.drawMiniPortrat)))
				did_draw_farmer = true;

			yield return instr;

			if (did_draw_farmer && !did_draw_button && instr.opcode == OpCodes.Callvirt && instr.operand is MethodInfo meth && meth == AccessTools.Method(typeof(SpriteBatch), nameof(SpriteBatch.Begin))) {
				did_draw_button = true;

				Monitor?.Log("Adding draw instruction to GameMenu.Draw", LogLevel.Trace);

				yield return new CodeInstruction(
					opcode: OpCodes.Ldarg_0 // this
				);
				yield return new CodeInstruction(
					opcode: OpCodes.Ldarg_1 // b
				);

				yield return new CodeInstruction(
					opcode: OpCodes.Call,
					operand: AccessTools.Method(typeof(GameMenu_Patches), nameof(GameMenu_Patches.Draw_Button))
				);
			}
		}

		if (!did_draw_button) {
			Monitor?.Log("Did not find correct draw instruction. Our book menu button will look wrong.", LogLevel.Warn);
		}

	}

	public static void Draw_Button(GameMenu menu, SpriteBatch batch) {
		try {
			int tab = menu.tabs.Count - 1;
			while (tab >= 0 && menu.tabs[tab].name != "leclair.almanac:books")
				tab--;

			if (tab < 0)
				return;

			if (menu.pages[tab] is Menus.BookCollectionMenu bcm)
				bcm.DrawButton(batch, menu.tabs[tab], menu.currentTab == tab);

		} catch (Exception ex) {
			Monitor?.LogOnce("An error occurred within Draw_Button.", LogLevel.Warn);
			Monitor?.LogOnce($"Details:\n{ex}", LogLevel.Warn);
		}
	}

	public static bool getTabNumberFromName_Prefix(GameMenu __instance, string name, ref int __result) {
		try {
			if (name.Equals("leclair.almanac:books")) {
				for (int i = 0; i < __instance.tabs.Count; i++) {
					if (__instance.tabs[i].name == name) {
						__result = i;
						return false;
					}
				}
			}
		} catch (Exception ex) {
			Monitor?.Log($"An error occurred within getTabNumberFromName.", LogLevel.Warn);
			Monitor?.Log($"Details:\n{ex}", LogLevel.Warn);
		}

		return true;
	}

}
