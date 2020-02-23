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