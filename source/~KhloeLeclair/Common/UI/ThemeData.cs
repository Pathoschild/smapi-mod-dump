/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;

using StardewModdingAPI;

namespace Leclair.Stardew.Common.UI
{
    public class ThemeData {

		public string[] For { get; set; }


		public bool HasMatchingMod(IModRegistry registry) {
			if (For != null)
				foreach (string mod in For) {
					if (!string.IsNullOrEmpty(mod) && registry.IsLoaded(mod))
						return true;
				}

			return false;
		}

    }
}
