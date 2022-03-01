/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkillfulClothes.Effects;
using SkillfulClothes.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Configuration
{
     /// <summary>
     /// JsonConverter for parsing nested effects
     /// </summary>
    class EffectJsonConverter : JsonConverter<IEffect>
    {
        EffectLibrary EffectLibrary { get; }

        public EffectJsonConverter(EffectLibrary effectLibrary)
        {
            EffectLibrary = effectLibrary;            
        }

        public override IEffect ReadJson(JsonReader reader, Type objectType, [AllowNull] IEffect existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JToken token = JToken.ReadFrom(reader);            
            return ParseJsonEffectDefinition(token, serializer);
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] IEffect effect, JsonSerializer serializer)
        {
            if (effect is EffectSet effectSet)
            {
                writer.WriteStartArray();
                foreach(var childEffect in effectSet.Effects)
                {
                    WriteJson(writer, childEffect, serializer);
                }
                writer.WriteEndArray();
            } else
            {
                if (effect is ICustomizableEffect customizableEffect && customizableEffect.ParameterObject is not NoEffectParameters)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(GetEffectName(effect));
                    // write the effect's parameters
                    JToken parameterToken = JToken.FromObject(customizableEffect.ParameterObject, serializer);
                    parameterToken.WriteTo(writer);                    
                    writer.WriteEndObject();
                } else
                {
                    writer.WriteValue(GetEffectName(effect));
                }                
            }
        }

        private string GetEffectName(IEffect effect)
        {
            string name = effect.GetType().Name;
            if (name.ToLower().EndsWith("effect"))
            {
                return name.Substring(0, name.Length - "effect".Length);
            }

            return name;
        }

        public IEffect ParseJsonEffectDefinition(JToken token, JsonSerializer serializer)
        {
            switch (token.Type)
            {
                case JTokenType.String:
                    return EffectLibrary.CreateEffectInstance(token.ToObject<String>());
                case JTokenType.Array:
                    return ParseJsonEffectSet(token, serializer);
                case JTokenType.Object:
                    return ParseJsonEffectObject(token.Value<JObject>().First.Value<JProperty>(), serializer);
                case JTokenType.Property:
                    return ParseJsonEffectObject(token.Value<JProperty>(), serializer);
                default:
                    Logger.Error("Unexpected value: " + token.ToString());
                    return new NullEffect();
            }
        }        

        EffectSet ParseJsonEffectSet(JToken arrayToken, JsonSerializer serializer)
        {
            if (arrayToken.Type != JTokenType.Array)
            {
                throw new ArgumentException("The specified token is not a JSON array", nameof(arrayToken));
            }

            List<IEffect> effects = new List<IEffect>();
            foreach (var child in arrayToken.Values())
            {
                effects.Add(ParseJsonEffectDefinition(child, serializer));
            }

            return EffectSet.Of(effects.ToArray());
        }

        IEffect ParseJsonEffectObject(JProperty jproperty, JsonSerializer serializer)
        {
            try
            {
                string effectName = jproperty.Name;

                var effect = EffectLibrary.CreateEffectInstance(effectName);
                if (effect is ICustomizableEffect customizableEffect)
                {
                    var parameterDefinition = jproperty.Value;
                    IEffectParameters parameters = parameterDefinition.ToObject(customizableEffect.ParameterType, serializer) as IEffectParameters;
                    customizableEffect.ParameterObject = parameters;
                }
                return effect;
            }
            catch (Exception e)
            {
                Logger.Error($"Encountered an invalid effect definition at {jproperty.Path}");
            }

            return new NullEffect();
        }
    }
}
