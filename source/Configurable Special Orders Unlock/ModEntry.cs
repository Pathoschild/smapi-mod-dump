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
using StardewValley;
using System.Linq;

namespace ConfigurableSpecialOrdersUnlock
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		private AssetEditor modAssetEditor;

		/// <summary>The mod entry point.</summary>
		/// <param name="helper" />
		public override void Entry(IModHelper helper)
		{
			Globals.Config = helper.ReadConfig<ModConfig>();
			Globals.Helper = helper;
			Globals.Monitor = Monitor;
			Globals.Manifest = ModManifest;

			modAssetEditor = new AssetEditor();
			helper.Content.AssetEditors.Add(modAssetEditor);

			helper.Events.GameLoop.GameLaunched += (sender, args) => ModConfigMenuHelper.TryLoadModConfigMenu();
			helper.Events.GameLoop.SaveLoaded += (sender, args) => modAssetEditor.InvalidateCache();
			helper.Events.GameLoop.SaveLoaded += (sender, args) => CleanUpSave();

			if (SpecialOrdersPatch.ApplyHarmonyPatches())
				Monitor.Log($"{ModManifest.Name}: Patches successfully applied");
		}

		private void CleanUpSave()
		{
			if (Game1.player.eventsSeen.Count(id => id == 15389722) > 1)
			{
				Monitor.Log("Duplicate event IDs detected - cleaning up save");

				Game1.player.eventsSeen.Set(
					Game1.player.eventsSeen.Distinct().ToArray()
				);
			}
		}
	}
}
