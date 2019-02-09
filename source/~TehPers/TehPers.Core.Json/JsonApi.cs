using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json;
using StardewModdingAPI;
using TehPers.Core.Json.Serialization;

namespace TehPers.Core.Json {
    internal class JsonApi : IJsonApi {
        private readonly ITehCoreApi _coreApi;

        /// <summary>The JSON settings to use when serialising and deserialising files.</summary>
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Replace, // avoid issue where default ICollection<T> values are duplicated each time the config is loaded
            Converters = new List<JsonConverter>
            {
                // Properly converts Net* objects
                new NetConverter(),

                // Provides descriptions
                new DescriptiveJsonConverter()
            }
        };

        internal JsonApi(ITehCoreApi coreApi) {
            this._coreApi = coreApi;
            this.AddSmapiConverters(coreApi.Owner.Helper);
        }

        private void AddSmapiConverters(IModHelper helper) {
            object smapiJsonHelper = helper.Data.GetType().GetField("JsonHelper", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(helper.Data);
            object jsonSettings = smapiJsonHelper?.GetType().GetProperty("JsonSettings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(smapiJsonHelper);
            if (jsonSettings is JsonSerializerSettings smapiSettings) {
                // Add all the converters SMAPI uses to this API's serializer settings
                foreach (JsonConverter converter in smapiSettings.Converters) {
                    this._jsonSettings.Converters.Add(converter);
                }
            } else {
                this._coreApi.Log("Json", "Unable to add SMAPI's JSON converters. Some config settings might be confusing!", LogLevel.Error);
            }
        }

        public void WriteJson<TModel>(string path, TModel model, bool minify = false) where TModel : class => this.WriteJson(path, model, null, minify);
        public void WriteJson<TModel>(string path, TModel model, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class {
            string fullPath = Path.Combine(this._coreApi.Owner.Helper.DirectoryPath, path);

            // Validate path
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("The file path is empty or invalid.", nameof(fullPath));

            // Create directory if needed
            string dir = Path.GetDirectoryName(fullPath);
            if (dir == null)
                throw new ArgumentException("The file path is invalid.", nameof(fullPath));
            Directory.CreateDirectory(dir);

            // Write to file stream directly
            using (FileStream stream = new FileStream(fullPath, FileMode.Create)) {
                this.Serialize(model, stream, settings, minify);
            }
        }

        public void Serialize<TModel>(TModel model, Stream outputStream, bool minify = false) where TModel : class => this.Serialize(model, outputStream, null, minify);
        public void Serialize<TModel>(TModel model, Stream outputStream, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class {
            // Write to stream directly using the custom JSON writer without closing the stream
            TextWriter textWriter = new StreamWriter(outputStream);
            using (DescriptiveJsonWriter writer = new DescriptiveJsonWriter(textWriter)) {
                writer.Minify = minify;

                // Setup JSON Settings
                JsonSerializerSettings jsonSettings = this._jsonSettings.Clone();
                settings?.Invoke(jsonSettings);

                // Serialize
                JsonSerializer.CreateDefault(jsonSettings).Serialize(writer, model);
                writer.Flush();
            }
        }

        public TModel Deserialze<TModel>(Stream inputStream) where TModel : class => this.Deserialze<TModel>(inputStream, null);
        public TModel Deserialze<TModel>(Stream inputStream, Action<JsonSerializerSettings> settings) where TModel : class {
            // Read from stream directly without closing the stream
            StreamReader streamReader = new StreamReader(inputStream);
            using (JsonTextReader jsonReader = new JsonTextReader(streamReader)) {
                // Setup JSON settings
                JsonSerializerSettings jsonSettings = this._jsonSettings.Clone();
                settings?.Invoke(jsonSettings);

                // Deserialize
                return JsonSerializer.CreateDefault(jsonSettings).Deserialize<TModel>(jsonReader);
            }
        }

        public TModel ReadJson<TModel>(string path) where TModel : class => this.ReadJson<TModel>(path, null);
        public TModel ReadJson<TModel>(string path, Action<JsonSerializerSettings> settings) where TModel : class {
            string fullPath = Path.Combine(this._coreApi.Owner.Helper.DirectoryPath, path);

            // Validate path
            if (string.IsNullOrWhiteSpace(fullPath))
                throw new ArgumentException("The file path is empty or invalid.", nameof(fullPath));
            if (!File.Exists(fullPath))
                return null;

            // Read from file stream directly
            using (FileStream stream = File.OpenRead(fullPath)) {
                return this.Deserialze<TModel>(stream, settings);
            }
        }

        public TModel ReadOrCreate<TModel>(string path, bool minify = false) where TModel : class, new() => this.ReadOrCreate(path, null, Activator.CreateInstance<TModel>, minify);
        public TModel ReadOrCreate<TModel>(string path, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class, new() => this.ReadOrCreate(path, settings, Activator.CreateInstance<TModel>, minify);
        public TModel ReadOrCreate<TModel>(string path, Func<TModel> modelFactory, bool minify = false) where TModel : class => this.ReadOrCreate(path, null, modelFactory, minify);
        public TModel ReadOrCreate<TModel>(string path, Action<JsonSerializerSettings> settings, Func<TModel> modelFactory, bool minify = false) where TModel : class {
            TModel model = this.ReadJson<TModel>(path, settings);
            if (model == null) {
                model = modelFactory();
                this.WriteJson(path, model, settings, minify);
            }
            return model;
        }
    }
}
