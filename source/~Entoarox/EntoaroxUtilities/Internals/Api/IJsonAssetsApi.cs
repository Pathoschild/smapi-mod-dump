/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Entoarox.Utilities.Internals.Api
{
    public interface IJsonAssetsApi
    {
        int GetObjectId(string name);
        int GetBigCraftableId(string name);
        int GetHatId(string name);
        int GetWeaponId(string name);
        int GetClothingId(string name);

        IDictionary<string, int> GetAllObjectIds();
        IDictionary<string, int> GetAllBigCraftableIds();
        IDictionary<string, int> GetAllHatIds();
        IDictionary<string, int> GetAllWeaponIds();
        IDictionary<string, int> GetAllClothingIds();
    }
}
