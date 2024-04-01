/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/RelocateFarmAnimals
**
*************************************************/

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Buildings;

namespace RelocateFarmAnimals.Patches
{
	internal class AnimalQueryMenuPatch
	{
		private static GameLocation TargetLocation = null;

		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(ReceiveLeftClickPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.receiveLeftClick)),
				transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(Transpiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.receiveLeftClick)),
				transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(ReceiveLeftClickTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.prepareForAnimalPlacement)),
				transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(Transpiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.performHoverAction)),
				transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(Transpiler))
			);
		}

		private static bool ReceiveLeftClickPrefix(AnimalQueryMenu __instance, int x, int y)
		{
			if (Game1.globalFade)
				return true;

			bool movingAnimal = (bool)typeof(AnimalQueryMenu).GetField("movingAnimal", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

			if (movingAnimal)
				return true;

			bool confirmingSell = (bool)typeof(AnimalQueryMenu).GetField("confirmingSell", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

			if (confirmingSell)
				return true;

			if (__instance.moveHomeButton.containsPoint(x, y))
			{
				List<KeyValuePair<string, string>> list = new();

				Game1.playSound("smallSelect");
				foreach (GameLocation location in Game1.locations)
				{
					if (location.buildings.Any((Building p) => p.GetIndoors() is AnimalHouse) && (!Game1.IsClient || location.CanBeRemotedlyViewed()))
					{
						list.Add(new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName));
					}
				}
				if (!list.Any())
				{
					Farm farm = Game1.getFarm();

					list.Add(new KeyValuePair<string, string>(farm.NameOrUniqueName, farm.DisplayName));
				}
				Game1.currentLocation.ShowPagedResponses(ModEntry.Helper.Translation.Get("RelocateAnimalMenu.ChooseLocation"), list, delegate(string value)
				{
					GameLocation locationFromName = Game1.getLocationFromName(value);

					if (locationFromName != null)
					{
						TargetLocation = locationFromName;
						if (Game1.activeClickableMenu is not null && Game1.activeClickableMenu is DialogueBox)
						{
							(Game1.activeClickableMenu as DialogueBox).closeDialogue();
						}
						Game1.activeClickableMenu = __instance;
						Game1.globalFadeToBlack(__instance.prepareForAnimalPlacement);
					}
					else
					{
						ModEntry.Monitor.Log("Can't find location '" + value + "' for animal relocate menu.", LogLevel.Error);
					}
				}, auto_select_single_choice: true);
				return false;
			}
			return true;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				foreach (CodeInstruction instruction in instructions)
				{
					if (instruction.opcode.Equals(OpCodes.Call) && instruction.operand.Equals(typeof(Game1).GetMethod(nameof(Game1.getFarm))))
					{
						instruction.operand = typeof(AnimalQueryMenuPatch).GetMethod(nameof(GetLocation), BindingFlags.NonPublic | BindingFlags.Static);
					}
				}
				return instructions;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(AnimalQueryMenu)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}

		private static GameLocation GetLocation()
		{
			return TargetLocation;
		}

		private static IEnumerable<CodeInstruction> ReceiveLeftClickTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 1; i < list.Count; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Ldloc_1) && list[i - 1].opcode.Equals(OpCodes.Brtrue_S))
					{
						CodeInstruction[] replacementInstructions = new CodeInstruction[]
						{
							new(OpCodes.Ldarg_0),
							new(OpCodes.Ldfld, typeof(AnimalQueryMenu).GetField("animal", BindingFlags.NonPublic | BindingFlags.Instance)),
							new(OpCodes.Callvirt, typeof(FarmAnimal).GetProperty("currentLocation").GetGetMethod())
						};
						list.InsertRange(i, replacementInstructions);
						i += replacementInstructions.Length;
						list.RemoveAt(i);
					}
				}
				return list;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(AnimalQueryMenu)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}
	}
}
