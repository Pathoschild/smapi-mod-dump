/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;

namespace BarleyCore
{
    public class BarleyCoreApi : IBarleyCoreApi
    {
        private IManifest Manifest;
        private IHaveModData? ModDataTarget;
        private JsonSerializerSettings JsonSettings;

        public BarleyCoreApi(IModInfo mod)
        {
            Manifest = mod.Manifest;
            ModDataTarget = null;
            JsonSettings = new()
            {
                Formatting = Formatting.None,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter()
                },
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public TModel? ModData_Read<TModel>(string key, bool local = true) where TModel : class
        {
            if (ModDataTarget == null) throw new NoModDataTargetException();

            string jsonString = ModDataTarget.modData[getModDataKey(key, local)];
            return JsonConvert.DeserializeObject<TModel>(jsonString);
        }

        public void ModData_SetTarget(IHaveModData target)
        {
            this.ModDataTarget = target;
        }

        public void ModData_Write<TModel>(string key, TModel value, bool local = true) where TModel : class
        {
            if (ModDataTarget == null) throw new NoModDataTargetException();

            string jsonString = JsonConvert.SerializeObject(value, Formatting.None, JsonSettings);
            ModDataTarget.modData[getModDataKey(key, local)] = jsonString;
        }

        private string getModDataKey(string key, bool local)
        {
            if (local) return key;

            return Manifest.UniqueID + "_" + key;
        }
    }

    public class NoModDataTargetException : Exception
    {
        public NoModDataTargetException() : base("No ModData target set for this instance.")
        {
        }
    }
}
