/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.IO;
using Newtonsoft.Json;
using TehPers.Core.Api.Content;

namespace TehPers.Core.Api.Json
{
    /// <summary>
    /// API for reading and writing JSON files.
    /// </summary>
    public interface IJsonProvider
    {
        /// <summary>
        /// Serializes JSON to a <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of object being written.</typeparam>
        /// <param name="data">The object being written.</param>
        /// <param name="outputStream">The <see cref="StreamWriter"/> to write to.</param>
        /// <param name="minify">Whether to minify the output. Minifying the output removes all comments and extra whitespace.</param>
        void Serialize<TModel>(TModel data, StreamWriter outputStream, bool minify = false)
            where TModel : class;

        /// <summary>
        /// Serializes JSON to a <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of object being written.</typeparam>
        /// <param name="data">The object being written.</param>
        /// <param name="outputStream">The <see cref="StreamWriter"/> to write to.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="minify">Whether to minify the output. Minifying the output removes all comments and extra whitespace.</param>
        void Serialize<TModel>(
            TModel data,
            StreamWriter outputStream,
            Action<JsonSerializerSettings>? settings,
            bool minify = false
        )
            where TModel : class;

        /// <summary>
        /// Deserializes JSON from a <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="inputStream">The <see cref="StreamReader"/> to read from.</param>
        /// <returns>The deserialized model.</returns>
        TModel? Deserialize<TModel>(StreamReader inputStream)
            where TModel : class;

        /// <summary>
        /// Deserializes JSON from a <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="inputStream">The <see cref="StreamReader"/> to read from.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>The deserialized model.</returns>
        TModel? Deserialize<TModel>(StreamReader inputStream, Action<JsonSerializerSettings>? settings)
            where TModel : class;


        /// <summary>
        /// Writes a JSON file to a specified path.
        /// </summary>
        /// <typeparam name="TModel">The type of object being written.</typeparam>
        /// <param name="data">The object being written.</param>
        /// <param name="path">The path to the output file.</param>
        /// <param name="minify">Whether to minify the output. Minifying the output removes all comments and extra whitespace.</param>
        void WriteJson<TModel>(TModel data, string path, bool minify = false)
            where TModel : class;

        /// <summary>
        /// Writes a JSON file to a specified path.
        /// </summary>
        /// <typeparam name="TModel">The type of object being written.</typeparam>
        /// <param name="data">The object being written.</param>
        /// <param name="path">The path to the output file.</param>
        /// <param name="assetProvider">The <see cref="IAssetProvider"/> to write the file to.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="minify">Whether to minify the output. Minifying the output removes all comments and extra whitespace.</param>
        void WriteJson<TModel>(
            TModel data,
            string path,
            IAssetProvider assetProvider,
            Action<JsonSerializerSettings>? settings,
            bool minify = false
        )
            where TModel : class;

        /// <summary>
        /// Reads JSON from a file.
        /// </summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <returns>The deserialized model.</returns>
        TModel? ReadJson<TModel>(string path)
            where TModel : class;

        /// <summary>
        /// Reads JSON from a file.
        /// </summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <param name="assetProvider">The <see cref="IAssetProvider"/> to read the file from.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>The deserialized model.</returns>
        TModel? ReadJson<TModel>(string path, IAssetProvider assetProvider, Action<JsonSerializerSettings>? settings)
            where TModel : class;

        /// <summary>
        /// Reads JSON from a file, creating the file if it doesn't exist.
        /// </summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <param name="minify">Whether to minify the output if the file is created. Minifying the output removes all comments and extra whitespace.</param>
        /// <returns>The deserialized model.</returns>
        TModel ReadOrCreate<TModel>(string path, bool minify = false)
            where TModel : class, new();

        /// <summary>
        /// Reads JSON from a file, creating the file if it doesn't exist.
        /// </summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <param name="assetProvider">The <see cref="IAssetProvider"/> to read the file from.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="minify">Whether to minify the output if the file is created. Minifying the output removes all comments and extra whitespace.</param>
        /// <returns>The deserialized model.</returns>
        TModel ReadOrCreate<TModel>(
            string path,
            IAssetProvider assetProvider,
            Action<JsonSerializerSettings>? settings,
            bool minify = false
        )
            where TModel : class, new();

        /// <summary>
        /// Reads JSON from a file, creating the file if it doesn't exist.
        /// </summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <param name="dataFactory">Factory method which creates the data to write.</param>
        /// <param name="minify">Whether to minify the output if the file is created. Minifying the output removes all comments and extra whitespace.</param>
        /// <returns>The deserialized model.</returns>
        TModel ReadOrCreate<TModel>(string path, Func<TModel> dataFactory, bool minify = false)
            where TModel : class;

        /// <summary>
        /// Reads JSON from a file, creating the file if it doesn't exist.
        /// </summary>
        /// <typeparam name="TModel">The type of object being read.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <param name="assetProvider">The <see cref="IAssetProvider"/> to read the file from.</param>
        /// <param name="settings">Callback for configuring the <see cref="JsonSerializerSettings"/>.</param>
        /// <param name="dataFactory">Factory method which creates the data to write.</param>
        /// <param name="minify">Whether to minify the output if the file is created. Minifying the output removes all comments and extra whitespace.</param>
        /// <returns>The deserialized model.</returns>
        TModel ReadOrCreate<TModel>(
            string path,
            IAssetProvider assetProvider,
            Action<JsonSerializerSettings>? settings,
            Func<TModel> dataFactory,
            bool minify = false
        )
            where TModel : class;
    }
}
