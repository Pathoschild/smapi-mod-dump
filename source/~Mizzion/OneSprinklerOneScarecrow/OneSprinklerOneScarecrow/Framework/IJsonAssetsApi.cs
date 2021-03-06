/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System.Collections.Generic;

namespace OneSprinklerOneScarecrow.Framework
{
    public interface IJsonAssetsApi
    {
        IDictionary<string, int> GetAllObjectIds();
        IDictionary<string, int> GetAllBigCraftableIds();
    }
}