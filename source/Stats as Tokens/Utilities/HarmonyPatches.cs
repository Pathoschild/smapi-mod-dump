/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/StatsAsTokens
**
*************************************************/

// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace StatsAsTokens
{
	internal class HarmonyPatches
	{
		/*********
		** FoodEatenToken patch
		*********/

		private static Harmony harmony;
		private static readonly MethodInfo set_RabbitWoolProduced = typeof(Stats).GetMethod("set_RabbitWoolProduced");
		private static readonly MethodInfo incrementAnimalProduceStat = typeof(StatFixes).GetMethod("IncrementAnimalProduceStat");

		public static void PerformHarmonyPatches()
		{
			harmony = new(Globals.Manifest.UniqueID);
			Farmer_eatObject_Patch();
			Tree_performTreeFall_Patch();
			Object_performObjectDropInAction_Patch();
			Object_checkForAction_Patch();
			ResourceClump_performToolAction_Patch();
			FarmAnimal_dayUpdate_Patch();
		}

		private static void Farmer_eatObject_Patch()
		{
			try
			{
				MethodBase eatObject = typeof(Farmer).GetMethod("eatObject");

				harmony.Patch(
					original: eatObject,
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Farmer_eatObject_Prefix))
				);

				Globals.Monitor.Log($"Patched {eatObject.Name} successfully");
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Exception encountered while patching method {nameof(Farmer_eatObject_Prefix)}: {ex}", LogLevel.Error);
			}
		}

		public static void Farmer_eatObject_Prefix(Farmer __instance, StardewValley.Object o)
		{
			string foodID = o.ParentSheetIndex.ToString();
			Farmer f = __instance;

			string pType;
			if (f.IsMainPlayer && f.IsLocalPlayer)
			{
				pType = "hostPlayer";
			}
			else if (f.IsLocalPlayer)
			{
				pType = "localPlayer";
			}
			else
			{
				return;
			}

			Dictionary<string, Dictionary<string, int>> foodDict = FoodEatenToken.foodEatenDict.Value;
			foodDict[pType][foodID] = foodDict[pType].ContainsKey(foodID) ? foodDict[pType][foodID] + 1 : 1;
		}

		private static void Tree_performTreeFall_Patch()
		{
			try
			{
				MethodBase treeFall = AccessTools.Method(typeof(Tree), "performTreeFall");

				harmony.Patch(
					original: treeFall,
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Tree_performTreeFall_Prefix))
				);

				Globals.Monitor.Log($"Patched {treeFall.Name} successfully");
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Exception encountered while patching method {nameof(Tree_performTreeFall_Prefix)}: {ex}", LogLevel.Error);
			}
		}

		public static void Tree_performTreeFall_Prefix(Tree __instance, Tool t)
		{
			Farmer owner = t?.getLastFarmerToUse();

			if (owner == null)
			{
				return;
			}

			string pType;
			if (owner.IsMainPlayer)
			{
				pType = "hostPlayer";
			}
			else if (owner.IsLocalPlayer)
			{
				pType = "localPlayer";
			}
			else
			{
				return;
			}

			string treeType = __instance.treeType.ToString();

			// condense palm trees (palm and palm2) into one entry
			if (treeType == "9")
			{
				treeType = "6";
			}

			Dictionary<string, Dictionary<string, int>> treeDict = TreesFelledToken.treesFelledDict.Value;
			treeDict[pType][treeType] = treeDict[pType].ContainsKey(treeType) ? treeDict[pType][treeType] + 1 : 1;
		}

		private static void Object_performObjectDropInAction_Patch()
		{
			try
			{
				MethodBase dropIn = typeof(StardewValley.Object).GetMethod("performObjectDropInAction");

				harmony.Patch(
					original: dropIn,
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Object_performObjectDropInAction_Prefix))
				);

				harmony.Patch(
					original: dropIn,
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Object_performObjectDropInAction_Postfix))
				);

				Globals.Monitor.Log($"Patched {dropIn.Name} successfully");
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Exception encountered while patching methods: {nameof(Object_performObjectDropInAction_Prefix)}, {nameof(Object_performObjectDropInAction_Postfix)}: {ex}", LogLevel.Error);
			}
		}

		public static void Object_performObjectDropInAction_Prefix(StardewValley.Object __instance, Item dropInItem, out int? __state)
		{
			int? minsReady = null;
			bool isValidInput = false;

			if (__instance.Name.Equals("Furnace"))
			{
				minsReady = __instance.MinutesUntilReady;
			}
			if (dropInItem is StardewValley.Object)
			{
				StardewValley.Object dropIn = dropInItem as StardewValley.Object;

				if (dropIn.Stack >= 5 && dropIn.ParentSheetIndex is 378 or 380 or 384 or 386)
				{
					isValidInput = true;
				}
			}

			__state = (minsReady is not null && isValidInput) ? minsReady : null;
		}

		public static void Object_performObjectDropInAction_Postfix(StardewValley.Object __instance, Farmer who, int? __state)
		{
			if (__instance.Name.Equals("Furnace"))
			{
				if (__state is not null && __state != __instance.MinutesUntilReady)
				{
					who.stats.BarsSmelted++;
				}
			}
		}

		private static void Object_checkForAction_Patch()
		{
			try
			{
				MethodBase dropIn = typeof(StardewValley.Object).GetMethod("checkForAction");

				harmony.Patch(
					original: dropIn,
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Object_checkForAction_Prefix)),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Object_checkForAction_Postfix))
				);

				Globals.Monitor.Log($"Patched {dropIn.Name} successfully");
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Exception encountered while patching methods: {nameof(Object_checkForAction_Prefix)}, {nameof(Object_checkForAction_Postfix)}: {ex}", LogLevel.Error);
			}
		}

		public static void Object_checkForAction_Prefix(StardewValley.Object __instance, bool justCheckingForActivity, out StardewValley.Object __state)
		{
			if (justCheckingForActivity)
			{
				__state = null;
				return;
			}

			__state = __instance.Name.Contains("Mayonnaise") ? __instance.heldObject.Value : null;
		}

		public static void Object_checkForAction_Postfix(StardewValley.Object __instance, bool justCheckingForActivity, Farmer who, StardewValley.Object __state)
		{
			if (justCheckingForActivity || !who.IsLocalPlayer)
			{
				return;
			}

			if (__instance.Name.Contains("Mayonnaise"))
			{
				if (__state is not null && __instance.heldObject.Value is null)
				{
					uint addQuantity = (uint)__state.Stack;

					switch (__state.ParentSheetIndex)
					{
						case 306:
							if (Game1.stats.stat_dictionary.ContainsKey("mayonnaiseMade"))
							{
								Game1.stats.stat_dictionary["mayonnaiseMade"] += addQuantity;
							}
							else
							{
								Game1.stats.stat_dictionary["mayonnaiseMade"] = addQuantity;
							}
							break;

						case 307:
							if (Game1.stats.stat_dictionary.ContainsKey("duckMayonnaiseMade"))
							{
								Game1.stats.stat_dictionary["duckMayonnaiseMade"] += addQuantity;
							}
							else
							{
								Game1.stats.stat_dictionary["duckMayonnaiseMade"] = addQuantity;
							}
							break;

						case 308:
							if (Game1.stats.stat_dictionary.ContainsKey("voidMayonnaiseMade"))
							{
								Game1.stats.stat_dictionary["voidMayonnaiseMade"] += addQuantity;
							}
							else
							{
								Game1.stats.stat_dictionary["voidMayonnaiseMade"] = addQuantity;
							}
							break;

						case 807:
							if (Game1.stats.stat_dictionary.ContainsKey("dinosaurMayonnaiseMade"))
							{
								Game1.stats.stat_dictionary["dinosaurMayonnaiseMade"] += addQuantity;
							}
							else
							{
								Game1.stats.stat_dictionary["dinosaurMayonnaiseMade"] = addQuantity;
							}
							break;

						default:
							return;
					}
				}
			}
		}

		private static void ResourceClump_performToolAction_Patch()
		{
			try
			{
				MethodBase performToolAction = typeof(ResourceClump).GetMethod("performToolAction");

				harmony.Patch(
					original: performToolAction,
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(ResourceClump_performToolAction_Postfix))
				);

				Globals.Monitor.Log($"Patched {performToolAction.Name} successfully");
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Exception encountered while patching method {nameof(ResourceClump_performToolAction_Postfix)}: {ex}", LogLevel.Error);
			}
		}

		public static void ResourceClump_performToolAction_Postfix(ResourceClump __instance, Tool t)
		{
			if (__instance.health.Value <= 0f)
			{
				if (__instance.parentSheetIndex.Value is 672 or 752 or 754 or 756 or 758)
				{
					if (t is not null && t.getLastFarmerToUse() is not null)
					{
						t.getLastFarmerToUse().stats.BouldersCracked++;
					}
				}
			}
		}


		private static void FarmAnimal_dayUpdate_Patch()
		{
			try
			{
				MethodBase dayUpdate = typeof(FarmAnimal).GetMethod("dayUpdate");

				harmony.Patch(
					original: dayUpdate,
					transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(FarmAnimal_dayUpdate_Transpiler))
				);

				Globals.Monitor.Log($"Patched {dayUpdate.Name} successfully");
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Exception encountered while patching method {nameof(FarmAnimal_dayUpdate_Transpiler)}: {ex}", LogLevel.Error);
			}
		}

		public static IEnumerable<CodeInstruction> FarmAnimal_dayUpdate_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> instructionsList = instructions.ToList();

			// remove everything from immediately after Ldloc_3 (whichProduce is loaded onto the stack) to RabbitWoolProduced is called
			int startIndex = instructionsList.FindIndex(il => il.opcode.Equals(OpCodes.Ldloc_3)) + 1;
			int endIndex = instructionsList.FindIndex(il => il.Calls(set_RabbitWoolProduced)) + 1;

			instructionsList.RemoveRange(startIndex, endIndex - startIndex);

			// add in call to IncrementAnimalProduceStat() just after ldloc_3
			instructionsList.Insert(startIndex, new CodeInstruction(OpCodes.Call, incrementAnimalProduceStat));

			// between ldloc_3 and incrementAnimalProduceStat(), add in ldarg_0 to pass `this` to IncrementAnimalProduceStat()
			instructionsList.Insert(startIndex, new CodeInstruction(OpCodes.Ldarg_0));

			return instructionsList.AsEnumerable();
		}
	}
}
