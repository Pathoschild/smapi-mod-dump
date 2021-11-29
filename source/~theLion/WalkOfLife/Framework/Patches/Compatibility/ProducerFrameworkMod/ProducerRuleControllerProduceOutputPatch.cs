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
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class ProducerRuleControllerProduceOutputPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ProducerRuleControllerProduceOutputPatch()
		{
			try
			{
				Original = "ProducerRuleController".ToType().MethodNamed("ProduceOutput");
			}
			catch
			{
				// ignored
			}

			Postfix = new(GetType(), nameof(ProducerRuleControllerProduceOutputPostfix));
		}

		#region harmony patches

		/// <summary>Patch to apply modded Artisan and Gemologist perks to PFM machines.</summary>
		[HarmonyPostfix]
		private static void ProducerRuleControllerProduceOutputPostfix(SObject producer, Farmer who, SObject input,
			bool probe)
		{
			if (input is null || probe) return;

			var output = producer.heldObject.Value;
			if (producer.IsArtisanMachine() && output.IsArtisanGood())
			{
				if (Context.IsMultiplayer && producer.owner.Value != who.UniqueMultiplayerID ||
				    !who.HasProfession("Artisan"))
				{
					output.Quality = SObject.lowQuality;
					return;
				}

				output.Quality = input.Quality;
				if (output.Quality < SObject.bestQuality &&
				    new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
					output.Quality += output.Quality == SObject.highQuality ? 2 : 1;

				producer.MinutesUntilReady -= producer.MinutesUntilReady / 10;
			}
			else if ((output.IsForagedMineral() || output.IsGemOrMineral()) &&
			         who.HasProfession("Gemologist"))
			{
				output.Quality = Utility.Professions.GetGemologistMineralQuality();
				if (who.IsLocalPlayer) ModEntry.Data.Increment<uint>("MineralsCollected");
			}
		}

		#endregion harmony patches
	}
}