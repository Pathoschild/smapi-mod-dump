/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GameLocationPerformActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GameLocationPerformActionPatch()
		{
			Original = RequireMethod<GameLocation>(nameof(GameLocation.performAction));
			Prefix = new(GetType(), nameof(GameLocationPerformActionPrefix));
		}

		#region harmony patches

		/// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
		[HarmonyPrefix]
		private static bool GameLocationPerformActionPrefix(GameLocation __instance, string action, Farmer who)
		{
			if (!ModEntry.Config.EnablePrestige || action is null || action.Split(' ')[0] != "DogStatue" || !who.IsLocalPlayer)
				return true; // run original logic

			try
			{
				if (who.CanPrestigeAny())
				{
					__instance.createQuestionDialogue(ModEntry.ModHelper.Translation.Get("prestige.dogstatue.first"),
						__instance.createYesNoResponses(), "dogStatue");
					return false; // don't run original logic
				}

				string message = ModEntry.ModHelper.Translation.Get("prestige.dogstatue.first");
				var tokens = message.Split('^');
				message = tokens[0] + '^' + tokens[1];
				Game1.drawObjectDialogue(message);
				return false; // don't run original logic
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patches
	}
}