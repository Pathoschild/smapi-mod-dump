using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using WorkbenchAnywhere.Framework;
using WorkbenchAnywhere.Utils;

namespace WorkbenchAnywhere
{
    public class ModEntry : Mod
    {
        private ModConfig _config;
        private UIHandler _uiHandler;
        private MaterialStorage _materialStorage;
        private SButton _refreshConfigKey;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _materialStorage = new MaterialStorage(Monitor, _config);
            _materialStorage.RegisterEvents(helper);
            var chestSelected = helper.Content.Load<Texture2D>("assets/workbench-1.png");
            var chestDeselected = helper.Content.Load<Texture2D>("assets/workbench-2.png");
            _uiHandler = new UIHandler(chestSelected, chestDeselected, _config, _materialStorage, Helper, Monitor);
            _uiHandler.RegisterUIEvents(helper);

            _refreshConfigKey = InputUtils.ParseButton(_config.ConfigReloadKey, Monitor);
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            BluePrintPatch.Initialize(Monitor, _config, _materialStorage);
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);
            harmony.Patch(
                AccessTools.Method(typeof(StardewValley.BluePrint), nameof(StardewValley.BluePrint.doesFarmerHaveEnoughResourcesToBuild)),
                new HarmonyMethod(typeof(BluePrintPatch), nameof(BluePrintPatch.doesFarmerHaveEnoughResourcesToBuild_Prefix)),
                new HarmonyMethod(typeof(BluePrintPatch), nameof(BluePrintPatch.doesFarmerHaveEnoughResourcesToBuild_Postfix)));
            harmony.Patch(
                AccessTools.Method(typeof(StardewValley.BluePrint), nameof(StardewValley.BluePrint.consumeResources)),
                new HarmonyMethod(typeof(BluePrintPatch), nameof(BluePrintPatch.consumeResources_Prefix)));
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (e.Button == _refreshConfigKey)
            {
                try
                {
                    _config = Helper.ReadConfig<ModConfig>();
                    _uiHandler.LoadConfig(_config);
                    _materialStorage.LoadConfig(_config);

                    Monitor.Log("Config file reloaded", LogLevel.Warn);
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Error while reloading config {ex}");
                }
            }
        }
    }
}
