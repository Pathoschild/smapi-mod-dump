/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;
using TheLion.Common.TileGeometry;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class GameLocationExplodePatch : BasePatch
	{
		private static ILHelper _helper;
		private static ITranslationHelper _i18n;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		/// <param name="i18n">Provides localized text.</param>
		internal GameLocationExplodePatch(ModConfig config, IMonitor monitor, ITranslationHelper i18n)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
			_i18n = i18n;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(GameLocation), nameof(GameLocation.explode)),
				transpiler: new HarmonyMethod(GetType(), nameof(GameLocationExplodeTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(GameLocationExplodePostfix))
			);
		}

		/// <summary>Patch for Demolitionist explosion resistance.</summary>
		protected static IEnumerable<CodeInstruction> GameLocationExplodeTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(GameLocation)}::{nameof(GameLocation.explode)}.");

			/// From: damagePlayers(areaOfEffect, damage_amount)
			/// To: damagePlayers(areaOfEffect, who.professions.Contains(<demolitionist_id>) ? 1 : damage_amount)

			Label isNotDemolitionist = iLGenerator.DefineLabel();
			Label resumeExecution = iLGenerator.DefineLabel();
			try
			{
				_helper
					.Find(                                      // find index of damagePlayers
						new CodeInstruction(OpCodes.Call, operand: AccessTools.Method(typeof(GameLocation), name: "damagePlayers"))
					)
					.AddLabel(resumeExecution)					// branch here to resume execution
					.Retreat()
					.AddLabel(isNotDemolitionist)				// branch here if player is not demolitionist
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_3)	// arg 3 = Farmer
					)
					.InsertProfessionCheckForWho(Utils.ProfessionsMap.Forward["demolitionist"], isNotDemolitionist)
					.Insert(
						new CodeInstruction(OpCodes.Ldc_I4_1),	// replace damage_amount with 1
						new CodeInstruction(OpCodes.Br_S, operand: resumeExecution)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Demolitionist explosion resistance.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// From: damagePlayers(areaOfEffect, radius * 3)
			/// To: damagePlayers(areaOfEffect, who.professions.Contains(<demolitionist_id>) ? 1 : radius * 3)

			isNotDemolitionist = iLGenerator.DefineLabel();
			resumeExecution = iLGenerator.DefineLabel();
			try
			{
				_helper
					.Find(                                      // find index of damagePlayers
						new CodeInstruction(OpCodes.Call, operand: AccessTools.Method(typeof(GameLocation), name: "damagePlayers"))
					)
					.AddLabel(resumeExecution)					// branch here to resume execution
					.Retreat(3)
					.AddLabel(isNotDemolitionist)				// branch here if player is not demolitionist
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_3)	// arg 3 = Farmer who
					)
					.InsertProfessionCheckForWho(Utils.ProfessionsMap.Forward["demolitionist"], isNotDemolitionist)
					.Insert(
						new CodeInstruction(OpCodes.Ldc_I4_1),	// replace radius * 3 with 1
						new CodeInstruction(OpCodes.Br_S, operand: resumeExecution)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Demolitionist explosion resistance.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Patch for Blaster double coal chance + Demolitionist speed burst.</summary>
		protected static void GameLocationExplodePostfix(ref GameLocation __instance, Vector2 tileLocation, int radius, Farmer who, bool damageFarmers = true)
		{
			if (Utils.PlayerHasProfession("blaster", who))
			{
				double chanceModifier = who.DailyLuck / 2.0 + who.LuckLevel * 0.001 + who.MiningLevel * 0.005;
				CircleTileGrid grid = new CircleTileGrid(tileLocation, radius);
				foreach (Vector2 tile in grid)
				{
					if (__instance.objects.TryGetValue(tile, out SObject tileObj) && IsStone(tileObj))
					{
						Random r = new Random(tile.GetHashCode());
						if (tileObj.ParentSheetIndex == 343 || tileObj.ParentSheetIndex == 450)
						{
							if (r.NextDouble() < 0.035 && Game1.stats.DaysPlayed > 1)
							{
								Game1.createObjectDebris(SObject.coal, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
							}
						}
						else if (r.NextDouble() < 0.05 * chanceModifier)
						{
							Game1.createObjectDebris(SObject.coal, (int)tile.X, (int)tile.Y, who.UniqueMultiplayerID, __instance);
						}
					}
				}
			}

			if (Utils.PlayerHasProfession("demolitionist") && damageFarmers)
			{
				int distanceFromEpicenter = (int)(tileLocation - who.getTileLocation()).Length();
				if (distanceFromEpicenter < radius * 2 + 1)
				{
					ModEntry.DemolitionistBuffMagnitude = 4;
				}
				if (distanceFromEpicenter < radius + 1)
				{
					ModEntry.DemolitionistBuffMagnitude += 2;
				}
			}
		}

		/// <summary>Whether a given object is a stone.</summary>
		/// <param name="obj">The world object.</param>
		protected static bool IsStone(SObject obj)
		{
			return obj?.Name == "Stone";
		}
	}
}
