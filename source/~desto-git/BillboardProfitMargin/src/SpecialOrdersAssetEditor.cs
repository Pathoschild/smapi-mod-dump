/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/desto-git/sdv-mods
**
*************************************************/

namespace BillboardProfitMargin
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using StardewModdingAPI;
	using StardewValley;
	using StardewValley.GameData;

	/// <summary>Main class.</summary>
	internal class SpecialOrdersAssetEditor : IAssetEditor
	{
		private readonly float configMultiplier;

		/// <summary>Initializes a new instance of the <see cref="SpecialOrdersAssetEditor"/> class.</summary>
		/// <param name="config">Mod configuration object.</param>
		public SpecialOrdersAssetEditor(ModConfig config)
		{
			this.configMultiplier = config.UseProfitMarginForSpecialOrders
				? Game1.player.difficultyModifier
				: config.CustomProfitMarginForSpecialOrders;
		}

		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		/// <typeparam name="T">.</typeparam>
		/// <returns>Wether or not the asset may be edited by this mod.</returns>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals("Data/SpecialOrders");
		}

		/// <summary>Edit a matched asset.</summary>
		/// <typeparam name="T">.</typeparam>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
		public void Edit<T>(IAssetData asset)
		{
			// update monetary rewards for special order quests
			IDictionary<string, SpecialOrderData> assetData = asset.AsDictionary<string, SpecialOrderData>().Data;

			// https://stackoverflow.com/a/31767807
			// .ToList is part of System.Linq
			// Without it, the loop would error after an assignment to a dictionary element
			foreach (string npcName in assetData.Keys.ToList())
			{
				foreach (SpecialOrderRewardData reward in assetData[npcName].Rewards)
				{
					if (reward.Type != "Money") continue;

					Dictionary<string, string> data = reward.Data;

					if (!data.ContainsKey("Amount")) throw new Exception("Could not get 'Amount' for special order quest.");
					string amount = data["Amount"];

					// amount is dictated by the requested resource with a multiplier
					if (amount.StartsWith("{"))
					{
						// There is actually nothing to do here.
						// The base price is already taking the profit margin into account.

						// string multiplier = data.ContainsKey("Multiplier") ? data["Multiplier"] : "100";
						// int newMultiplier = (int)Math.Ceiling(int.Parse(multiplier) * this.configMultiplier);
						// data["Multiplier"] = newMultiplier.ToString();
					}

					// reward is a fixed gold amount
					else
					{
						int newAmount = (int)Math.Ceiling(int.Parse(amount) * this.configMultiplier);
						data["Amount"] = newAmount.ToString();
					}
				}
			}
		}
	}
}