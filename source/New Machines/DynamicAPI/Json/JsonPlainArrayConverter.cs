/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Json
{
    public sealed class JsonPlainArrayConverter : JsonConverter
    {
        public override bool CanRead { get; } = false;
        private bool IsLocked { get; set; }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IsLocked = true;
            var currentFormating = writer.Formatting;
            writer.Formatting = Formatting.None;
            serializer.Serialize(writer, value, value.GetType());
            writer.Formatting = currentFormating;
            IsLocked = false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return !IsLocked && typeof(IReadOnlyList<int>).IsAssignableFrom(objectType);
        }
    }
}
