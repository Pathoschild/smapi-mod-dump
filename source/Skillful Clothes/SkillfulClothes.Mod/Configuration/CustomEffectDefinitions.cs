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
using SkillfulClothes.Effects;
using SkillfulClothes.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Configuration
{
    class CustomEffectDefinitions
    {
        public static void LoadCustomEffectDefinitions()
        {
            if (EffectHelper.Config.EnableShirtEffects)
            {
                HandleCustomEffectDefinitions("custom_shirts.json", ItemDefinitions.ShirtEffects);
            }

            if (EffectHelper.Config.EnablePantsEffects)
            {
                HandleCustomEffectDefinitions("custom_pants.json", ItemDefinitions.PantsEffects);
            }

            if (EffectHelper.Config.EnableHatEffects)
            {
                HandleCustomEffectDefinitions("custom_hats.json", ItemDefinitions.HatEffects);
            }
        }

        private static void HandleCustomEffectDefinitions<TItem>(string filename, Dictionary<TItem, ExtItemInfo> target)
             where TItem : AlphanumericItemId
        {
            string filepath = Path.Combine(EffectHelper.ModHelper.DirectoryPath, filename);

            if (File.Exists(filepath))
            {
                Logger.Info($"Loading custom effect definitions from {filename}");

                target.Clear();
                ReadItemDefinitions(filepath, target);
            }
            else
            {
                // export the current definitions
                WriteItemDefinitions(filepath, target);
            }
        }

        private static void ReadItemDefinitions<TItem>(string filepath, Dictionary<TItem, ExtItemInfo> target)
            where TItem: AlphanumericItemId
        {
            List<CustomEffectItemDefinition> definitions;

            CustomEffectConfigurationParser parser = new CustomEffectConfigurationParser();
            using (FileStream fStream = new FileStream(filepath, FileMode.Open))
            {
                try
                {
                    definitions = parser.Parse(fStream);
                } catch (Exception e)
                {
                    Logger.Error($"Unable to read {filepath}: {e.Message}");
                    return;
                }
            }

            foreach (var def in definitions)
            {
                TItem itemId = ItemDefinitions.GetKnownItemById<TItem>(def.ItemIdentifier);
                target.Add(itemId, ExtendItem.With.Effect(def.Effect));
            }
        }

        private static void WriteItemDefinitions<TItem>(string filepath, Dictionary<TItem, ExtItemInfo> source)
            where TItem: AlphanumericItemId
        {
            Dictionary<string, IEffect> exportData = new Dictionary<string, IEffect>();
            foreach(var itemDef in source)
            {
                string itemId = itemDef.Key.ItemId;
                if (!String.IsNullOrEmpty(itemDef.Key.ItemName))
                {
                    itemId = itemDef.Key.ItemName;
                }

                exportData.Add(itemId, itemDef.Value.Effect);
            }

            JsonSerializer jsonSerializer = new JsonSerializer();
            jsonSerializer.Formatting = Formatting.Indented;
            jsonSerializer.Converters.Add(new EnumJsonConverter());
            jsonSerializer.Converters.Add(new EffectJsonConverter(null));            

            using (var fStream = new FileStream(filepath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(fStream))
            {
                jsonSerializer.Serialize(writer, exportData);
            }
        }
    }
}
