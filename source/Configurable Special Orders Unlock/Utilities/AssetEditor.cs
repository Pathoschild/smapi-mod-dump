/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dtomlinson-ga/ConfigurableSpecialOrdersUnlock
**
*************************************************/

// Configurable Special Orders Unlock mod for Stardew Valley
// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

using StardewModdingAPI;
using System.Collections.Generic;

namespace ConfigurableSpecialOrdersUnlock
{
	class AssetEditor : IAssetEditor
	{
		/// <summary>
		/// Utilized by SMAPI to determine whether an asset should be edited.
		/// </summary>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			if (asset.AssetNameEquals("Data/Events/Town"))
			{
				return !Globals.Config.skipCutscene;
			}
			return false;
		}

		/// <summary>
		/// Utilized by SMAPI to determine what edits should be made to an asset.
		/// </summary>
		public void Edit<T>(IAssetData asset)
		{
			if (asset.AssetNameEquals("Data/Events/Town"))
			{
				IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

				int unlockDays = Globals.Config.GetUnlockDaysPlayed();

				if (unlockDays - 1 != 57)
				{
					data[$"15389722/j {unlockDays - 1}"] = data["15389722/j 57"];
					data.Remove("15389722/j 57");
				}
			}
		}

		/// <summary>
		/// Forces invalidation of Data/Events/Town.
		/// This prevents cached values from being used if the player has changed config options.
		/// </summary>
		public void InvalidateCache()
		{
			Globals.Helper.Content.InvalidateCache("Data/Events/Town");
		}
	}
}
