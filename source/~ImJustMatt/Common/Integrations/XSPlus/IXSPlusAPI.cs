/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Integrations.XSPlus
{
    using System;
    using System.Collections.Generic;

    public interface IXSPlusApi
    {
        public void EnableWithModData(string featureName, string key, string value, bool param);

        public void EnableWithModData(string featureName, string key, string value, float param);

        public void EnableWithModData(string featureName, string key, string value, int param);

        public void EnableWithModData(string featureName, string key, string value, string param);

        public void EnableWithModData(string featureName, string key, string value, HashSet<string> param);

        public void EnableWithModData(string featureName, string key, string value, Dictionary<string, bool> param);

        public void EnableWithModData(string featureName, string key, string value, Tuple<int, int, int> param);
    }
}