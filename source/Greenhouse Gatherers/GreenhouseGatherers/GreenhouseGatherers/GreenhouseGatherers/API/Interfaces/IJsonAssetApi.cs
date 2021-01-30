/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using System;
using System.Collections.Generic;

namespace GreenhouseGatherers.GreenhouseGatherers.API.Interfaces
{
    public interface IJsonAssetApi
    {
        List<string> GetAllBigCraftablesFromContentPack(string cp);
        IDictionary<string, int> GetAllBigCraftableIds();
        int GetBigCraftableId(string name);

        event EventHandler IdsAssigned;
    }
}
