using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using StardewModdingAPI;
using TreeChanges.TreeChanges.Config;
using TreeChanges.TreeChanges.Hooks;

namespace TreeChanges.TreeChanges
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static HarmonyInstance hInstance;
        public static TreeConfig config;
        public static Mod instance;
        public static bool jsonAssetsLoaded = false;
        public static JsonAssetsAPI jsonAssets = null;
        public override void Entry(IModHelper helper)
        {
            instance = this;
            config = helper.ReadConfig<TreeConfig>() ?? new TreeConfig();
            tryLoadJsonAssets(helper);
            try
            {
                hInstance = HarmonyInstance.Create("jpan.mine_changes");
                TreeHooks.addTrans(this, hInstance);
            }
            catch (Exception ex)
            {
                Monitor.Log("Could not patch one of the Hooks: " + ex, LogLevel.Error);
            }
        }

        private void tryLoadJsonAssets(IModHelper helper)
        {
            if (!helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                Monitor.Log("JsonAssets is not loaded. Will not be able to add defined objects to seed drops.", LogLevel.Info);
                return;
            }
            jsonAssetsLoaded = true;
            return;
        }

        public static int getObjectIDFromAsset(string oreName)
        {
            if (jsonAssetsLoaded)
            {
                if (jsonAssets == null)
                {
                    jsonAssets = Mod.instance.Helper.ModRegistry.GetApi<JsonAssetsAPI>("spacechase0.JsonAssets");
                    if (jsonAssets == null)
                    {
                        Mod.instance.Monitor.Log("Tried to load object " + oreName + " but could not find JsonAssets.", StardewModdingAPI.LogLevel.Error);
                        //jsonAssetsLoaded = false;
                    }
                    int ans = Mod.jsonAssets.GetObjectId(oreName);
                    if (ans < 0)
                    {
                        Mod.instance.Monitor.Log("Tried to load object " + oreName + " but JsonAssets could not find it.", StardewModdingAPI.LogLevel.Error);
                        return -1000;
                    }
                    return ans;
                }
            }
            return -1000;
        }
    }
}

