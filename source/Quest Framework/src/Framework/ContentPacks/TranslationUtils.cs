/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Newtonsoft.Json.Linq;
using QuestFramework.Offers;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.ContentPacks
{
    internal static class TranslationUtils
    {
        public static string Translate(string text, ITranslationHelper translation)
        {
            if (text != null && text.StartsWith("%i18n:"))
            {
                return translation.Get(text.Substring(6));
            }

            return text;
        }

        public static QuestOffer<JObject> TranslateOffer(ITranslationHelper translation, QuestOffer<JObject> offer)
        {
            return TranslateJObject(translation, JObject.FromObject(offer))
                .ToObject<QuestOffer<JObject>>();
        }

        public static JObject TranslateJObject(ITranslationHelper translation, JObject jObject)
        {
            var transJObject = new JObject(jObject);

            foreach (var token in jObject)
            {
                transJObject[token.Key] = TranslateToken(translation, token.Value);
            }

            return transJObject;
        }

        public static T Translate<T>(ITranslationHelper translation, T toTranslate)
        {
            JObject _toTranslate = JObject.FromObject(toTranslate);

            return TranslateJObject(translation, _toTranslate)
                .ToObject<T>();
        }

        private static JToken TranslateToken(ITranslationHelper translation, JToken token)
        {
            if (token.Type == JTokenType.String)
                return Translate(token.Value<string>(), translation);
            if (token.Type == JTokenType.Object)
                return TranslateJObject(translation, token.Value<JObject>());
            if (token.Type == JTokenType.Array)
            {
                var source = (JArray)token;
                var jArray = new JArray();

                foreach (JToken toTranslate in source)
                    jArray.Add(TranslateToken(translation, toTranslate));

                return jArray;
            }
                

            return token;
        }
    }
}
