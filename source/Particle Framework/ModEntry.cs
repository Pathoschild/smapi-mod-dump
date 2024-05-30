/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/ParticleFramework
**
*************************************************/

global using SObject = StardewValley.Object;
using HarmonyLib;
using ParticleFramework.Framework.Api;
using ParticleFramework.Framework.Data;
using ParticleFramework.Framework.Interfaces;
using ParticleFramework.Framework.Managers;
using ParticleFramework.Framework.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ParticleFramework
{
    public class ModEntry : Mod
    {
        // Shared static helpers
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static Multiplayer multiplayer;
        internal static ModConfig modConfig;

        // Managers
        internal static ApiManager apiManager;
        public override void Entry(IModHelper helper)
        {
            // Setup i18n
            I18n.Init(helper.Translation);

            // Setup the monitor, helper and multiplayer
            monitor = Monitor;
            modHelper = helper;
            multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

            // Setup the config
            modConfig = Helper.ReadConfig<ModConfig>();

            // Setup the manager
            apiManager = new ApiManager();

            // Apply the patches
            var harmony = new Harmony(this.ModManifest.UniqueID);

            new CodePatches(harmony).Apply();

            // Hook into GameLoop events
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configApi = apiManager.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu", false);
            if (Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu") && configApi != null)
            {
                configApi.Register(ModManifest, () => modConfig = new ModConfig(), () => Helper.WriteConfig(modConfig));

                AddOption(configApi, nameof(modConfig.EnableMod));
            }
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (modConfig.EnableMod)
            {
                foreach (var path in ParticleEffectManager.dictPaths)
                {
                    if (e.NameWithoutLocale.IsEquivalentTo(path))
                    {
                        e.LoadFrom(() => new Dictionary<string, ParticleEffectData>(), AssetLoadPriority.Exclusive);
                    }
                }
            }
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (!modConfig.EnableMod)
                return;
            foreach (var key in ParticleEffectManager.effectDict.Keys)
            {
                var ped = ParticleEffectManager.effectDict[key];
                switch (ped.type.ToLower())
                {
                    case "screen":
                        ParticleEffectManager.ShowScreenParticleEffect(e.SpriteBatch, ped);
                        break;
                }
            }
        }
        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!modConfig.EnableMod)
                return;
            foreach (var key in ParticleEffectManager.effectDict.Keys)
            {
                var ped = ParticleEffectManager.effectDict[key];
                switch (ped.type.ToLower())
                {
                    case "location":
                        if (Game1.currentLocation.Name == ped.name)
                            ParticleEffectManager.ShowLocationParticleEffect(e.SpriteBatch, Game1.currentLocation, ped);
                        break;
                }
            }
        }

        public override object GetApi()
        {
            return new ParticleFrameworkApi();
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            ParticleEffectManager.LoadEffects();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            ParticleEffectManager.LoadEffects();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            ParticleEffectManager.LoadEffects();
        }
        private void AddOption(IGenericModConfigMenuApi configApi, string name)
        {
            PropertyInfo propertyInfo = typeof(ModConfig).GetProperty(name);
            if (propertyInfo == null)
            {
                Monitor.Log($"Error: Property '{name}' not found in ModConfig.", LogLevel.Error);
                return;
            }

            Func<string> getName = () => I18n.GetByKey($"Config.{typeof(ModEntry).Namespace}.{name}.Name");
            Func<string> getDescription = () => I18n.GetByKey($"Config.{typeof(ModEntry).Namespace}.{name}.Description");

            if (getName == null || getDescription == null)
            {
                Monitor.Log($"Error: Localization keys for '{name}' not found.", LogLevel.Error);
                return;
            }

            var getterMethod = propertyInfo.GetGetMethod();
            var setterMethod = propertyInfo.GetSetMethod();

            if (getterMethod == null || setterMethod == null)
            {
                Monitor.Log($"Error: The get/set methods are null for property '{name}'.", LogLevel.Error);
                return;
            }

            var getter = Delegate.CreateDelegate(typeof(Func<>).MakeGenericType(propertyInfo.PropertyType), modConfig, getterMethod);
            var setter = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(propertyInfo.PropertyType), modConfig, setterMethod);

            switch (propertyInfo.PropertyType.Name)
            {
                case nameof(Boolean):
                    configApi.AddBoolOption(ModManifest, (Func<bool>)getter, (Action<bool>)setter, getName, getDescription);
                    break;
                case nameof(Int32):
                    configApi.AddNumberOption(ModManifest, (Func<int>)getter, (Action<int>)setter, getName, getDescription);
                    break;
                case nameof(Single):
                    configApi.AddNumberOption(ModManifest, (Func<float>)getter, (Action<float>)setter, getName, getDescription);
                    break;
                case nameof(String):
                    configApi.AddTextOption(ModManifest, (Func<string>)getter, (Action<string>)setter, getName, getDescription);
                    break;
                case nameof(SButton):
                    configApi.AddKeybind(ModManifest, (Func<SButton>)getter, (Action<SButton>)setter, getName, getDescription);
                    break;
                case nameof(KeybindList):
                    configApi.AddKeybindList(ModManifest, (Func<KeybindList>)getter, (Action<KeybindList>)setter, getName, getDescription);
                    break;
                default:
                    Monitor.Log($"Error: Unsupported property type '{propertyInfo.PropertyType.Name}' for '{name}'.", LogLevel.Error);
                    break;
            }
        }
    }
}