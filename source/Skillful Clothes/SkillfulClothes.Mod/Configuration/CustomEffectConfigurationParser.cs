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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Configuration
{
    public class CustomEffectConfigurationParser
    {

        JsonSerializer defaultSerializer = new JsonSerializer();        

        public CustomEffectConfigurationParser()
        {
            defaultSerializer.Converters.Add(new EffectJsonConverter(EffectLibrary.Default));            
        }

        /// <summary>
        /// Parse a json description of custom item effect definitions
        /// </summary>
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <returns></returns>
        public List<CustomEffectItemDefinition> Parse(Stream jsonStream)
        {            
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new EffectJsonConverter(EffectLibrary.Default));

            using (StreamReader reader = new StreamReader(jsonStream))
            using(JsonTextReader jsonReader = new JsonTextReader(reader))
            {                
                var lst = serializer.Deserialize<Dictionary<String, IEffect>>(jsonReader);
                return lst.Select(x => new CustomEffectItemDefinition(x.Key, x.Value)).ToList();
            }
        }        
    }

    public class CustomEffectItemDefinition
    {
        /// <summary>
        /// Numerical id of the item or a well-known name
        /// </summary>        
        public string ItemIdentifier { get; }
        public IEffect Effect { get; }

        public CustomEffectItemDefinition(string id, IEffect effect)
        {
            ItemIdentifier = id;
            Effect = effect;
        }
    }
}
