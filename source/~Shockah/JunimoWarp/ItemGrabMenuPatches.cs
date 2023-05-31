/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Nanoray.Shrike.Harmony;
using Nanoray.Shrike;
using Shockah.Kokoro;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using StardewValley.Menus;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

namespace Shockah.JunimoWarp
{
	internal class ItemGrabMenuPatches
	{
		private const int WarpButtonID = 1567601; // {NexusID}01

		private static JunimoWarp Instance
			=> JunimoWarp.Instance;

		private static readonly ConditionalWeakTable<ItemGrabMenu, ClickableTextureComponent> WarpButtons = new();

		internal static void Apply(Harmony harmony)
		{
			harmony.TryPatch(
				monitor: Instance.Monitor,
				original: () => AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.RepositionSideButtons)),
				transpiler: new HarmonyMethod(typeof(ItemGrabMenuPatches), nameof(RepositionSideButtons_Transpiler))
			);

			harmony.TryPatch(
				monitor: Instance.Monitor,
				original: () => AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(ItemGrabMenuPatches), nameof(draw_Transpiler))
			);

			harmony.TryPatch(
				monitor: Instance.Monitor,
				original: () => AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.performHoverAction)),
				postfix: new HarmonyMethod(typeof(ItemGrabMenuPatches), nameof(performHoverAction_Postfix)),
				transpiler: new HarmonyMethod(typeof(ItemGrabMenuPatches), nameof(performHoverAction_Transpiler))
			);

			harmony.TryPatch(
				monitor: Instance.Monitor,
				original: () => AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.receiveLeftClick)),
				postfix: new HarmonyMethod(typeof(ItemGrabMenuPatches), nameof(receiveLeftClick_Postfix))
			);

			harmony.TryPatch(
				monitor: Instance.Monitor,
				original: () => AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.populateClickableComponentList)),
				postfix: new HarmonyMethod(typeof(ItemGrabMenuPatches), nameof(IClickableMenu_populateClickableComponentList_Postfix))
			);
		}

		public static ClickableTextureComponent ObtainWarpButton(ItemGrabMenu menu)
		{
			if (!WarpButtons.TryGetValue(menu, out var button))
			{
				button = new ClickableTextureComponent("", new Rectangle(menu.xPositionOnScreen + menu.width, menu.yPositionOnScreen + menu.height / 3 - 64, 64, 64), "", Instance.Helper.Translation.Get("junimoWarp.tooltip"), Game1.mouseCursors, new Rectangle(108, 491, 16, 16), 4f)
				{
					myID = WarpButtonID,
					leftNeighborID = 53912,
					region = 15923
				};
				WarpButtons.AddOrUpdate(menu, button);
			}
			return button;
		}

		private static IEnumerable<CodeInstruction> RepositionSideButtons_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.AsGuidAnchorable()
					.Find(
						ILMatches.Newobj(AccessTools.DeclaredConstructor(typeof(List<ClickableComponent>), Array.Empty<Type>())),
						ILMatches.Stloc<List<ClickableComponent>>(originalMethod.GetMethodBody()!.LocalVariables).WithAutoAnchor(out Guid sideButtonsStlocPointer)
					)
					.PointerMatcher(sideButtonsStlocPointer)
					.CreateLdlocInstruction(out var ldlocSideButtons)
					.Insert(
						SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.JustInsertion,

						new CodeInstruction(OpCodes.Ldarg_0),
						ldlocSideButtons,
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ItemGrabMenuPatches), nameof(RepositionSideButtons_Transpiler_ModifySideButtons)))
					)
					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		public static void RepositionSideButtons_Transpiler_ModifySideButtons(ItemGrabMenu menu, List<ClickableComponent> sideButtons)
		{
			if (menu.context is not Chest chest)
				return;
			if (chest.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
				return;
			sideButtons.Add(ObtainWarpButton(menu));
		}

		private static IEnumerable<CodeInstruction> draw_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.Find(
						ILMatches.Ldarg(0),
						ILMatches.Ldfld(AccessTools.Field(typeof(MenuWithInventory), nameof(MenuWithInventory.hoverText))),
						ILMatches.Brfalse
					)
					.PointerMatcher(SequenceMatcherRelativeElement.First)
					.ExtractLabels(out var labels)
					.Insert(
						SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.JustInsertion,

						new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
						new CodeInstruction(OpCodes.Ldarg_1),
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ItemGrabMenuPatches), nameof(draw_Transpiler_DrawWarpButton)))
					)
					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		public static void draw_Transpiler_DrawWarpButton(ItemGrabMenu menu, SpriteBatch b)
		{
			if (menu.context is not Chest chest)
				return;
			if (chest.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
				return;
			ObtainWarpButton(menu).draw(b);
		}

		private static IEnumerable<CodeInstruction> performHoverAction_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				return new SequenceBlockMatcher<CodeInstruction>(instructions)
					.Find(
						ILMatches.Ldarg(0),
						ILMatches.Ldarg(1),
						ILMatches.Ldarg(2),
						ILMatches.Call("performHoverAction")
					)
					.Insert(
						SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.JustInsertion,

						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldarg_1),
						new CodeInstruction(OpCodes.Ldarg_2),
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ItemGrabMenuPatches), nameof(performHoverAction_Transpiler_TryHoverWarpButton)))
					)
					.AllElements();
			}
			catch (Exception ex)
			{
				Instance.Monitor.Log($"Could not patch methods - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
				return instructions;
			}
		}

		public static void performHoverAction_Transpiler_TryHoverWarpButton(ItemGrabMenu menu, int x, int y)
		{
			if (menu.context is not Chest chest)
				return;
			if (chest.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
				return;
			ObtainWarpButton(menu).tryHover(x, y, 0.25f);
		}

		private static void performHoverAction_Postfix(ItemGrabMenu __instance, int x, int y)
		{
			if (__instance.context is not Chest chest)
				return;
			if (chest.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
				return;

			var button = ObtainWarpButton(__instance);
			button.tryHover(x, y);
			if (button.containsPoint(x, y))
				__instance.hoverText = button.hoverText;
		}

		private static void receiveLeftClick_Postfix(ItemGrabMenu __instance, int x, int y)
		{
			if (__instance.context is not Chest chest)
				return;
			if (chest.SpecialChestType != Chest.SpecialChestTypes.JunimoChest)
				return;

			var button = ObtainWarpButton(__instance);
			if (button.containsPoint(x, y))
			{
				Game1.exitActiveMenu();

				if (Instance.Config.RequiredEmptyChest)
				{
					foreach (var item in __instance.ItemsToGrabMenu.actualInventory)
						if (item is not null && item.Stack > 0)
						{
							Kokoro.Kokoro.Instance.QueueObjectDialogue(Instance.Helper.Translation.Get("junimoWarp.notEmpty.message"));
							return;
						}
				}

				Instance.RequestNextWarp(Game1.player.currentLocation, new((int)chest.TileLocation.X, (int)chest.TileLocation.Y), (warpLocation, warpPoint) =>
				{
					JunimoWarp.AnimatePlayerWarp(warpLocation, warpPoint);
				});
			}
		}

		private static void IClickableMenu_populateClickableComponentList_Postfix(IClickableMenu __instance)
		{
			if (__instance is not ItemGrabMenu menu)
				return;
			menu.allClickableComponents.Add(ObtainWarpButton(menu));
		}
	}
}