/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/YariazenMods
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Semver;
using System;

namespace KitchenLib.src.ContentPack.JsonConverters
{
    public class SemVersionConverter : CustomCreationConverter<SemVersion>
    {
        private string semver;

        public override SemVersion Create(Type objectType)
        {
            return SemVersion.Parse(semver, SemVersionStyles.Any);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            semver = (string)reader.Value;
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}
