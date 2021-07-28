/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlatoTK.Utils
{
    internal class Serialization
    {
        internal const char KeySeperator = '|';
        internal const char ValueSeperator = '=';
        internal const string Prefix = "Plato:";
        internal const char innerValueSeperator = '/';


        internal static T DeserializeValue<T>(string base64String)
        {
            using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(base64String)))
            using (BsonDataReader reader = new BsonDataReader(buffer))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        internal static string SerializeValue<T>(T value)
        {
            using (MemoryStream buffer = new MemoryStream())
            using (BsonDataWriter writer = new BsonDataWriter(buffer))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, value);
                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        internal static Dictionary<string, string> ParseDataString(string dataString)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(dataString) || !dataString.StartsWith(Prefix))
                return data;

            foreach (string[] p in dataString.Substring(Prefix.Length).Split(KeySeperator).Select(s => s.Split(ValueSeperator)))
                if(p.Length == 2)
                    data.Add(p[0], p[1]);

            return data;
        }

        internal static string DataToString(Dictionary<string,string> data)
        {
            return Prefix + string.Join($"{KeySeperator}", data.Select(k => $"{k.Key}{ValueSeperator}{k.Value}"));
        }
    }
}
