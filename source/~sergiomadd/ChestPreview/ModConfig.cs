/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using MaddUtil;
using ChestPreview.Framework;
using ChestPreview.Framework.APIs;

namespace ChestPreview
{
    public class ModConfig
    {
        public bool Enabled { get; set; }
        public int Range { get; set; }
        public int Size { get; set; }
        public bool Connector { get; set; }
        public bool EnableKey { get; set; }
        public SButton Key { get; set; }
        public bool EnableMouse { get; set; }
        public string Mouse { get; set; }


        public ModConfig()
        {
            ResetToDefault();         
        }

        public void RegisterModConfigMenu(IModHelper helper, IManifest manifest)
        {
            var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: manifest,
                reset: () => ResetToDefault(),
                save: () => helper.WriteConfig(this)
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => Helpers.GetTranslationHelper().Get("config.enabled.name"),
                getValue: () => Enabled,
                setValue: value => Enabled = value
                );
            configMenu.AddNumberOption(
                mod: manifest,
                name: () => Helpers.GetTranslationHelper().Get("config.range.name"),
                tooltip: () => Helpers.GetTranslationHelper().Get("config.range.tooltip"),
                getValue: () => Range,
                setValue: value => Range = value
            );
            
            configMenu.AddNumberOption(
                 mod: manifest,
                 name: () => Helpers.GetTranslationHelper().Get("config.size.name"),
                 getValue: () => Size,
                 setValue: value => Size = ModEntry.UpdateSize(value),
                 min: 1,
                 max: 4,
                 interval: 1,
                 formatValue: value => Conversor.GetSizeTrasnlationName(value)
             );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => Helpers.GetTranslationHelper().Get("config.connector.name"),
                tooltip: () => Helpers.GetTranslationHelper().Get("config.connector.tooltip"),
                getValue: () => Connector,
                setValue: value => Connector = value
                );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => Helpers.GetTranslationHelper().Get("config.enablekey.name"),
                getValue: () => EnableKey,
                setValue: value => EnableKey = value
                );
            configMenu.AddKeybind(
                mod: manifest,
                name: () => Helpers.GetTranslationHelper().Get("config.key.name"),
                tooltip: () => Helpers.GetTranslationHelper().Get("config.key.tooltip"),
                getValue: () => Key,
                setValue: value => Key = value
                );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => Helpers.GetTranslationHelper().Get("config.enablemouse.name"),
                getValue: () => EnableMouse,
                setValue: value => EnableMouse = value
                );
            configMenu.AddTextOption(
                mod: manifest,
                name: () => Helpers.GetTranslationHelper().Get("config.mouse.name"),
                tooltip: () => Helpers.GetTranslationHelper().Get("config.mouse.tooltip"),
                getValue: () => Mouse,
                setValue: value => Mouse = value,
                allowedValues: new string[] { "MouseLeft", "MouseRight", "MouseMiddle", "MouseX1", "MouseX2" },
                formatAllowedValue: value => Conversor.GetTranslationMouse(value)
            );
        }

        public void ResetToDefault()
        {
            Enabled = true;
            Range = -1;
            Size = 3;
            Connector = true;
            EnableKey = false;
            Key = SButton.J;
            EnableMouse = false;
            Mouse = "MouseLeft";
        }
    }
}
