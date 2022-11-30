/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

using System;
using System.Collections.Generic;

namespace SailorStyles
{
	public interface IJsonAssetsApi
	{
		void LoadAssets(string path);

		int GetHatId(string name);
		int GetClothingId(string name);
		IDictionary<string, int> GetAllHatIds();
		IDictionary<string, int> GetAllClothingIds();
		List<string> GetAllHatsFromContentPack(string cp);
		List<string> GetAllClothingFromContentPack(string cp);

        /// <summary>
        /// Raised when JA tries to fix IDs.
        /// </summary>
        event EventHandler IdsFixed;
    }
}
