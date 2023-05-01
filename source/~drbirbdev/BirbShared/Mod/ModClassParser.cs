/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Reflection;
using BirbShared.Asset;
using BirbShared.Command;
using BirbShared.Config;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BirbShared.Mod
{
    class ModClass
    {
        public event EventHandler<OneSecondUpdateTickedEventArgs> ApisLoaded;

        private IMod Mod;

        public void Parse(IMod mod, bool harmony = true)
        {
            this.Mod = mod;

            Log.Init(mod.Monitor);

            mod.Helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched_ApisLoaded;

            Log.Debug("" + mod.GetType());
            foreach(FieldInfo fieldInfo in mod.GetType().GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach(Attribute attr in fieldInfo.GetCustomAttributes(false))
                {
                    if (attr is SmapiApi apiAttr)
                    {
                        mod.Helper.Events.GameLoop.GameLaunched += (object sender, GameLaunchedEventArgs e) =>
                        {
                            object api = mod.Helper.ModRegistry.GetType().GetMethod("GetApi", 1, new Type[] { typeof(string) })
                                .MakeGenericMethod(fieldInfo.FieldType)
                                .Invoke(mod.Helper.ModRegistry, new object[] { apiAttr.UniqueID });
                            if (api is null && apiAttr.IsRequired)
                            {
                                Log.Error($"[{fieldInfo.Name}] Can't access required API");
                            }
                            fieldInfo.SetValue(mod, api);
                        };
                    }
                    else if (attr is SmapiAsset assetAttr)
                    {
                        object asset = fieldInfo.FieldType.GetConstructor(Array.Empty<Type>())?.Invoke(Array.Empty<object>());
                        if (asset is null)
                        {
                            Log.Error($"[{fieldInfo.Name}] Asset class must have zero-argument constructor");
                            continue;
                        }
                        fieldInfo.SetValue(mod, asset);
                        new AssetClassParser(mod, asset).ParseAssets();
                    }
                    else if (attr is SmapiCommand commandAttr)
                    {
                        object command = fieldInfo.FieldType.GetConstructor(Array.Empty<Type>())?.Invoke(Array.Empty<object>());
                        if (command is null)
                        {
                            Log.Error($"[{fieldInfo.Name}] Command class must have zero-argument constructor");
                            continue;
                        }
                        fieldInfo.SetValue(mod, command);
                        mod.Helper.Events.GameLoop.GameLaunched += (object sender, GameLaunchedEventArgs e) =>
                        {
                            new CommandClassParser(mod.Helper.ConsoleCommands, command).ParseCommands();
                        };
                    }
                    else if (attr is SmapiConfig configAttr)
                    {
                        object config = mod.Helper.GetType().GetMethod("ReadConfig")
                            .MakeGenericMethod(fieldInfo.FieldType)
                            .Invoke(mod.Helper, Array.Empty<object>());
                        if (config is null)
                        {
                            Log.Error($"[{fieldInfo.Name}] Config class could not be read");
                            continue;
                        }
                        fieldInfo.SetValue(mod, config);
                        mod.Helper.Events.GameLoop.GameLaunched += (object sender, GameLaunchedEventArgs e) =>
                        {
                            new ConfigClassParser(mod, config).ParseConfigs();
                        };
                    }
                    else if (attr is SmapiContent contentAttr)
                    {

                    }
                    else if (attr is SmapiData dataAttr)
                    {

                    }
                    else if (attr is SmapiInstance instanceAttr)
                    {
                        fieldInfo.SetValue(mod, mod);
                    }
                }
            }

            if (harmony)
            {
                mod.Helper.Events.GameLoop.GameLaunched += (object sender, GameLaunchedEventArgs e) =>
                {
                    new Harmony(mod.ModManifest.UniqueID).PatchAll(Assembly.GetExecutingAssembly());
                };
            }
        }

        private void GameLoop_GameLaunched_ApisLoaded(object sender, GameLaunchedEventArgs e)
        {
            this.Mod.Helper.Events.GameLoop.OneSecondUpdateTicked += this.GameLoop_OneSecondUpdateTicked_ApisLoaded;
        }

        private void GameLoop_OneSecondUpdateTicked_ApisLoaded(object sender, OneSecondUpdateTickedEventArgs e)
        {
            this.Mod.Helper.Events.GameLoop.OneSecondUpdateTicked -= GameLoop_OneSecondUpdateTicked_ApisLoaded;
            ApisLoaded?.Invoke(sender, e);
        }
    }
}
