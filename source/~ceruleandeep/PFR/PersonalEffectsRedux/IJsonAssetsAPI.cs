/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace PersonalEffects
{
    public interface IJsonAssetsApi
    {
        int GetObjectId(string name);
        IDictionary<string, int> GetAllObjectIds();
    }
}
