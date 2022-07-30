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
using ItemPipes.Framework.APIs;

namespace ItemPipes
{
    public class ModConfig
    {
        public bool DebugMode { get; set; }
        public bool ItemSending { get; set; }
        public bool IOPipeStatePopup { get; set; }
        public bool IOPipeStateSignals { get; set; }

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
                name: () => "DebugMode",
                tooltip: () => "Adds extra debug commands for ItemPipes.",
                getValue: () => DebugMode,
                setValue: value => DebugMode = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "ItemSending",
                tooltip: () => "Enables/Disables pipe to pipe item sending.",
                getValue: () => ItemSending,
                setValue: value => ItemSending = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "IOPipeStatePopup",
                tooltip: () => "Enables/Disables popup icons in Input/Output pipes.",
                getValue: () => IOPipeStatePopup,
                setValue: value => IOPipeStatePopup = value
            );
            configMenu.AddBoolOption(
                mod: manifest,
                name: () => "IOPipeStateSignals",
                tooltip: () => "Enables/Disables signal textures in Input/Output pipes.",
                getValue: () => IOPipeStateSignals,
                setValue: value => IOPipeStateSignals = value
            );
        }

        public void ResetToDefault()
        {
            DebugMode = false;
            ItemSending = true;
            IOPipeStatePopup = true;
            IOPipeStateSignals = true;
        }


    }
}
