/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

using System;
using System.Collections.Generic;

namespace CustomCompanions.Framework.Interfaces
{
    public interface IJsonAssetsApi
    {
        void LoadAssets(string path);
        IDictionary<string, int> GetAllObjectIds();
        int GetObjectId(string name);

        event EventHandler IdsAssigned;
    }
}
