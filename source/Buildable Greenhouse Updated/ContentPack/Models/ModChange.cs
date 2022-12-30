/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/YariazenMods
**
*************************************************/

using KitchenLib.Customs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace KitchenLib.src.ContentPack.Models
{
    public class ModChange
    {
        [JsonProperty("Type", Required = Required.Always)]
        public SerializationContext Type { get; set; }
        [JsonProperty("Location", Required = Required.Always)]
        public List<string> Location { get; set; }

        public void Load()
        {
            string json = File.ReadAllText(Path.Combine(ContentPackManager.CurrentPack.ModDirectory, Path.Combine(Location.ToArray())));
            CustomGameDataObject gdo = DeserializeGDO(Type, json);
            if (gdo != null)
            {
                CustomGDO.RegisterGameDataObject(gdo);
            }
        }

        public CustomGameDataObject DeserializeGDO(SerializationContext context, string json)
        {
            return context switch
            {
                _ => null
            };
        }
    }
}
