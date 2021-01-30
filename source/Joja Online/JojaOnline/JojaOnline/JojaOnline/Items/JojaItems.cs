/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/JojaOnline
**
*************************************************/

using JojaOnline.JojaOnline.API;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JojaOnline.JojaOnline.Items
{
    public static class JojaItems
    {
        private static string modID;
        private static IJsonAssetApi jsonAssetApi;
        private static IMonitor monitor = JojaResources.GetMonitor();

        public static void HookIntoApi(IModHelper helper)
        {
            // Get modID
            modID = helper.ModRegistry.ModID;

            // Attempt to hook into the IMobileApi interface
            jsonAssetApi = helper.ModRegistry.GetApi<IJsonAssetApi>("spacechase0.JsonAssets");

            if (jsonAssetApi is null)
            {
                monitor.Log("Failed to hook into spacechase0.JsonAssets. Joja Prime Membership will be unavailable for purchase.", LogLevel.Error);
                return;
            }

            monitor.Log("Successfully hooked into spacechase0.JsonAssets.", LogLevel.Debug);
        }

        // TODO: Make this a Dictionary<string, int> and return all custom items from this project
        public static int GetJojaPrimeMembershipID()
        {
            if (jsonAssetApi is null)
            {
                return -1;
            }

            return jsonAssetApi.GetObjectId("Joja Prime");
        }
    }
}
