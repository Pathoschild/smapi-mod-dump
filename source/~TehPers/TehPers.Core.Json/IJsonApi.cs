using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json;

namespace TehPers.Core.Json {
    public interface IJsonApi {
        /// <summary>Deserializes a JSON file if it exists into a <see cref="TModel"/>. If it doesn't exist, returns null.</summary>
        /// <typeparam name="TModel">The type of object to deserialize the file as.</typeparam>
        /// <param name="path">The path to the JSON file relative to the mod folder.</param>
        /// <returns>The deserialized file, or null of the file doesn't exist.</returns>
        TModel ReadJson<TModel>(string path) where TModel : class;

        /// <summary>Deserializes a JSON file if it exists into a <see cref="TModel"/>. If it doesn't exist, returns null.</summary>
        /// <typeparam name="TModel">The type of object to deserialize the file as.</typeparam>
        /// <param name="path">The path to the JSON file relative to the mod folder.</param>
        /// <param name="settings">A function which modifies the <see cref="JsonSerializerSettings"/> used by the deserializer.</param>
        /// <returns>The deserialized file, or null of the file doesn't exist.</returns>
        TModel ReadJson<TModel>(string path, Action<JsonSerializerSettings> settings) where TModel : class;

        /// <summary>Deserializes a JSON file if it exists into a <see cref="TModel"/>. If it doesn't exist, creates a new one using the default constructor.</summary>
        /// <typeparam name="TModel">The type of object to deserialize the file as.</typeparam>
        /// <param name="path">The path to the JSON file relative to the mod folder.</param>
        /// <param name="minify">If a new file is created, whether it should be minified.</param>
        /// <returns>The deserialized file, or a new instance of <see cref="TModel"/> using the default constructor.</returns>
        TModel ReadOrCreate<TModel>(string path, bool minify = false) where TModel : class, new();

        /// <summary>Deserializes a JSON file if it exists into a <see cref="TModel"/>. If it doesn't exist, creates a new one using the default constructor.</summary>
        /// <typeparam name="TModel">The type of object to deserialize the file as.</typeparam>
        /// <param name="path">The path to the JSON file relative to the mod folder.</param>
        /// <param name="settings">A function which modifies the <see cref="JsonSerializerSettings"/> used by the serializer and deserializer.</param>
        /// <param name="minify">If a new file is created, whether it should be minified.</param>
        /// <returns>The deserialized file, or a new instance of <see cref="TModel"/> using the default constructor.</returns>
        TModel ReadOrCreate<TModel>(string path, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class, new();

        /// <summary>Deserializes a JSON file if it exists into a <see cref="TModel"/>. If it doesn't exist, creates a new one using a factory.</summary>
        /// <typeparam name="TModel">The type of object to deserialize the file as.</typeparam>
        /// <param name="path">The path to the JSON file relative to the mod folder.</param>
        /// <param name="modelFactory">A function which returns an instance of <see cref="TModel"/> which is used to create the new file if it doesn't already exist.</param>
        /// <param name="minify">If a new file is created, whether it should be minified.</param>
        /// <returns>The deserialized file, or a new instance of <see cref="TModel"/> using the default constructor.</returns>
        TModel ReadOrCreate<TModel>(string path, Func<TModel> modelFactory, bool minify = false) where TModel : class;

        /// <summary>Deserializes a JSON file if it exists into a <see cref="TModel"/>. If it doesn't exist, creates a new one using a factory.</summary>
        /// <typeparam name="TModel">The type of object to deserialize the file as.</typeparam>
        /// <param name="path">The path to the JSON file relative to the mod folder.</param>
        /// <param name="settings">A function which modifies the <see cref="JsonSerializerSettings"/> used by the serializer and deserializer.</param>
        /// <param name="modelFactory">A function which returns an instance of <see cref="TModel"/> which is used to create the new file if it doesn't already exist.</param>
        /// <param name="minify">If a new file is created, whether it should be minified.</param>
        /// <returns>The deserialized file, or a new instance of <see cref="TModel"/> using the default constructor.</returns>
        TModel ReadOrCreate<TModel>(string path, Action<JsonSerializerSettings> settings, Func<TModel> modelFactory, bool minify = false) where TModel : class;

        /// <summary>Serializes a <see cref="TModel"/> into a JSON file.</summary>
        /// <typeparam name="TModel">The type of object to deserialize the file as.</typeparam>
        /// <param name="path">The path to the JSON file relative to the mod folder.</param>
        /// <param name="model">The object to serialize.</param>
        /// <param name="minify">If a new file is created, whether it should be minified.</param>
        void WriteJson<TModel>(string path, TModel model, bool minify = false) where TModel : class;

        /// <summary>Serializes a <see cref="TModel"/> into a JSON file.</summary>
        /// <typeparam name="TModel">The type of object to deserialize the file as.</typeparam>
        /// <param name="path">The path to the JSON file relative to the mod folder.</param>
        /// <param name="model">The object to serialize.</param>
        /// <param name="settings">A function which modifies the <see cref="JsonSerializerSettings"/> used by the serializer and deserializer.</param>
        /// <param name="minify">If a new file is created, whether it should be minified.</param>
        void WriteJson<TModel>(string path, TModel model, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class;

        /// <summary>Serializes a <see cref="TModel"/> into JSON and writes it to a stream.</summary>
        /// <typeparam name="TModel">The type of object to serialize.</typeparam>
        /// <param name="model">The object to serialize.</param>
        /// <param name="outputStream">The stream to output the serialized JSON to.</param>
        /// <param name="minify">Whether the output should be minified.</param>
        void Serialize<TModel>(TModel model, Stream outputStream, bool minify = false) where TModel : class;

        /// <summary>Serializes a <see cref="TModel"/> into JSON and writes it to a stream.</summary>
        /// <typeparam name="TModel">The type of object to serialize.</typeparam>
        /// <param name="model">The object to serialize.</param>
        /// <param name="outputStream">The stream to output the serialized JSON to.</param>
        /// <param name="settings">A function which modifies the <see cref="JsonSerializerSettings"/> used by the serializer and deserializer.</param>
        /// <param name="minify">Whether the output should be minified.</param>
        void Serialize<TModel>(TModel model, Stream outputStream, Action<JsonSerializerSettings> settings, bool minify = false) where TModel : class;

        /// <summary>Deserializes a stream into a <see cref="TModel"/>.</summary>
        /// <typeparam name="TModel">The type of object to deserialize the data in the stream as.</typeparam>
        /// <param name="inputStream">The stream containing the data being deserialzed.</param>
        /// <returns>The deserialized object.</returns>
        TModel Deserialze<TModel>(Stream inputStream) where TModel : class;

        /// <summary>Deserializes a stream into a <see cref="TModel"/>.</summary>
        /// <typeparam name="TModel">The type of object to deserialize the data in the stream as.</typeparam>
        /// <param name="inputStream">The stream containing the data being deserialzed.</param>
        /// <param name="settings">A function which modifies the <see cref="JsonSerializerSettings"/> used by the deserializer.</param>
        /// <returns>The deserialized object.</returns>
        TModel Deserialze<TModel>(Stream inputStream, Action<JsonSerializerSettings> settings) where TModel : class;
    }
}
