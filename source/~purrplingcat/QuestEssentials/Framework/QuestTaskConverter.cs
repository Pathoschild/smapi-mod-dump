/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestEssentials.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Framework
{
    internal class QuestTaskConverter : JsonConverter
    {
        private bool _inside = false;

        public override bool CanWrite => false;
        public override bool CanRead => !this._inside;

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(QuestTask));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonException();

            this._inside = true;
            JObject jObject = JObject.Load(reader);

            if (!jObject.ContainsKey("Type"))
            {
                throw new JsonException("Attribute `Type` is required for `StoryQuestTalk`!");
            }

            QuestTask task;
            string type = jObject["Type"].ToString();
            if (!QuestTask.IsKnownTaskType(type))
            {
                throw new JsonException($"Unknown story quest type `{type}`");
            }

            QuestEssentialsMod.ModMonitor.Log($"StoryQuestTaskConverter: Using class type <{QuestTask.GetTaskType(type).FullName}> for `{type}`");
            task = (QuestTask)jObject.ToObject(QuestTask.GetTaskType(type), serializer);
            this._inside = false;

            return task;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
