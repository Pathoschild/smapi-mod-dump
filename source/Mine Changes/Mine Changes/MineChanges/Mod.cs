using System;
using System.Reflection;
using Harmony;
using Mine_Changes.MineChanges.Config;
using Mine_Changes.MineChanges.Hook;
using StardewModdingAPI;

namespace Mine_Changes.MineChanges
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static HarmonyInstance hInstance;
        public static MineConfig config;
        public static Mod instance;
        public static bool jsonAssetsLoaded = false;
        public static JsonAssetsAPI jsonAssets = null;
        public override void Entry(IModHelper helper)
        {
            instance = this;
            config = helper.ReadConfig<MineConfig>() ?? new MineConfig();
            tryLoadJsonAssets(helper);
            try
            {
                hInstance = HarmonyInstance.Create("jpan.mine_changes");
                MineHooks.addTrans(this, hInstance);
                LocationHooks.addBreak(this, hInstance);
            }
            catch(Exception ex)
            {
                Monitor.Log("Could not patch one of the Hooks: " + ex, LogLevel.Error);
            }
        }

        private void tryLoadJsonAssets(IModHelper helper)
        {
            if (!helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                Monitor.Log("JsonAssets is not loaded. Will not be able to add defined objects to stone drops.", LogLevel.Info);
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
