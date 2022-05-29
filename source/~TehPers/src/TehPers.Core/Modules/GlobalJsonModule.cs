/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Setup;
using TehPers.Core.Json;
using TehPers.Core.Setup;

namespace TehPers.Core.Modules
{
    public class GlobalJsonModule : ModModule
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;

        public GlobalJsonModule(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper;
            this.monitor = monitor;
        }

        public override void Load()
        {
            // SMAPI's default converters
            foreach (var converter in this.GetSmapiConverters())
            {
                this.GlobalProxyRoot.Bind<JsonConverter>().ToConstant(converter).InSingletonScope();
            }

            // Patches
            this.Bind<ISetup>().ToMethod(NewtonsoftPatcher.Create).InSingletonScope();

            // Custom converters
            this.GlobalProxyRoot.Bind<JsonConverter>().To<NetConverter>().InSingletonScope();
            this.GlobalProxyRoot.Bind<JsonConverter>()
                .To<NamespacedKeyJsonConverter>()
                .InSingletonScope();
        }

        private IEnumerable<JsonConverter> GetSmapiConverters()
        {
            var smapiJsonHelper = this.helper.Data.GetType()
                .GetField("JsonHelper", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(this.helper.Data);
            var smapiJsonSettings = smapiJsonHelper?.GetType()
                .GetProperty(
                    "JsonSettings",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                )
                ?.GetValue(smapiJsonHelper);
            if (smapiJsonSettings is JsonSerializerSettings {Converters: { } smapiConverters})
            {
                // Add all the converters SMAPI uses to this API's serializer settings
                foreach (var converter in smapiConverters)
                {
                    yield return converter;
                }
            }
            else
            {
                this.monitor.Log(
                    "Unable to get SMAPI's JSON converters. Some config settings might be confusing!",
                    LogLevel.Error
                );
            }
        }
    }
}
