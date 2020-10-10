/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GilarF/SVM
**
*************************************************/

using System.Collections.Generic;
using ModSettingsTab.Framework;
using ModSettingsTabApi.Framework.Interfaces;

namespace ModSettingsTab
{
    public class Api : IModTabSettingsApi
    {
        internal readonly Dictionary<string,SettingsTabApi> ApiList = new Dictionary<string, SettingsTabApi>();

        public ISettingsTabApi GetMod(string uniqueId)
        {
            if (ApiList.ContainsKey(uniqueId)) return ApiList[uniqueId];
            ApiList.Add(uniqueId,new SettingsTabApi());
            return ApiList[uniqueId];
        }
    }
}