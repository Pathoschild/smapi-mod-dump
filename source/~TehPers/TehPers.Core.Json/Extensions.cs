using System.Collections.Generic;
using Newtonsoft.Json;
using StardewModdingAPI;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Json {
    public static class Extensions {
        private static readonly Dictionary<ITehCoreApi, IJsonApi> _jsonApis = new Dictionary<ITehCoreApi, IJsonApi>();

        public static IJsonApi GetJsonApi(this ITehCoreApi core) {
            return Extensions._jsonApis.GetOrAdd(core, () => {
                JsonApi json = new JsonApi(core);
                core.Log("Json", "JSON API created", LogLevel.Debug);
                return json;
            });
        }

        internal static JsonSerializerSettings Clone(this JsonSerializerSettings source) {
            return new JsonSerializerSettings {
                CheckAdditionalContent = source.CheckAdditionalContent,
                ConstructorHandling = source.ConstructorHandling,
                Context = source.Context,
                ContractResolver = source.ContractResolver,
                Converters = new List<JsonConverter>(source.Converters),
                Culture = source.Culture,
                DateFormatHandling = source.DateFormatHandling,
                DateFormatString = source.DateFormatString,
                DateParseHandling = source.DateParseHandling,
                DateTimeZoneHandling = source.DateTimeZoneHandling,
                DefaultValueHandling = source.DefaultValueHandling,
                Error = source.Error,
                EqualityComparer = source.EqualityComparer,
                Formatting = source.Formatting,
                FloatFormatHandling = source.FloatFormatHandling,
                FloatParseHandling = source.FloatParseHandling,
                MaxDepth = source.MaxDepth,
                MetadataPropertyHandling = source.MetadataPropertyHandling,
                MissingMemberHandling = source.MissingMemberHandling,
                NullValueHandling = source.NullValueHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                PreserveReferencesHandling = source.PreserveReferencesHandling,
                ReferenceLoopHandling = source.ReferenceLoopHandling,
                ReferenceResolverProvider = source.ReferenceResolverProvider,
                SerializationBinder = source.SerializationBinder,
                StringEscapeHandling = source.StringEscapeHandling,
                TypeNameHandling = source.TypeNameHandling,
                TraceWriter = source.TraceWriter,
                TypeNameAssemblyFormatHandling = source.TypeNameAssemblyFormatHandling
            };
        }
    }
}
