/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.Locations;

namespace mouahrarasModuleCollection.Other.BetterPorchRepair.Utilities
{
	internal class BetterPorchRepairUtility
	{
		internal static void RepairPorch(AssetRequestedEventArgs e)
		{
			if (ModEntry.Config is null || !ModEntry.Config.OtherBetterPorchRepair)
				return;

			if (e.Name.IsEquivalentTo("Data/Buildings"))
			{
				if (Context.IsWorldReady)
				{
					Farm farm = Game1.getFarm();

					if (farm is not null)
					{
						Building farmhouse = farm.GetMainFarmHouse();

						if (farmhouse is not null)
						{
							FarmHouse farmhouseIndoors = farmhouse.GetIndoors() as FarmHouse;

							if (farmhouseIndoors is not null && farmhouseIndoors.upgradeLevel > 0)
							{
								e.Edit(asset =>
								{
									IDictionary<string, BuildingData> data = asset.AsDictionary<string, BuildingData>().Data;
									string[] array = data["Farmhouse"].CollisionMap.Trim().Split('\n', StringSplitOptions.TrimEntries);

									array[^2] = string.Concat("O", array[^2].AsSpan(1));
									data["Farmhouse"].CollisionMap = data["Farmhouse"].CollisionMap = string.Join('\n', array);
								});
							}
						}
					}
				}
			}
		}

		internal static void InvalidateCache()
		{
			ModEntry.Helper.GameContent.InvalidateCache("Data/Buildings");
		}
	}
}
