/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace ItemPipes.Framework.API
{

    public static class ApiManager
    {
        private static IDynamicGameAssetsApi dynamicGameAssets;
        private static IJsonAssetsApi3 jsonAssets;

        public static void HookIntoDynamicGameAssets()
        {
            // Attempt to hook into the IMobileApi interface
            dynamicGameAssets = Helper.GetHelper().ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");

            if (dynamicGameAssets is null)
            {
                Printer.Info("Failed to hook into spacechase0.DynamicGameAssets.");
                return;
            }
            Printer.Info("Successfully hooked into spacechase0.DynamicGameAssets.");
        }

        public static void HookIntoJsonAssets()
        {
            // Attempt to hook into the IMobileApi interface
            jsonAssets = Helper.GetHelper().ModRegistry.GetApi<IJsonAssetsApi3>("spacechase0.JsonAssets");
            if (jsonAssets is null)
            {
                Printer.Info("Failed to hook into spacechase0.JsonAssets.");
                return;
            }
            Printer.Info("Successfully hooked into spacechase0.JsonAssets.");
        }

        public static IDynamicGameAssetsApi GetDynamicGameAssetsInterface()
        {
            return dynamicGameAssets;
        }
        public static IJsonAssetsApi3 GetJsonAssetsInterface()
        {
            return jsonAssets;
        }
    }
}
