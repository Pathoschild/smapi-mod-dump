/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System;
using Helpers.ConfigData;
using StardewModdingAPI;

namespace ImJustMatt.Common.Integrations.GenericModConfigMenu
{
    internal class GenericModConfigMenuIntegration : ModIntegration<IGenericModConfigMenuAPI>
    {
        public GenericModConfigMenuIntegration(IModRegistry modRegistry)
            : base(modRegistry, "spacechase0.GenericModConfigMenu")
        {
        }

        public Action RevertToDefault(ConfigHelper configHelper, object instance)
        {
            return delegate
            {
                foreach (var field in configHelper.Fields)
                {
                    configHelper.FieldHandler.SetValue(instance, field, field.DefaultValue);
                }
            };
        }

        public void RegisterConfigOptions(IManifest manifest, ConfigHelper configHelper, object instance)
        {
            if (!IsLoaded)
                return;

            foreach (var property in configHelper.Fields)
            {
                configHelper.FieldHandler.RegisterConfigOption(manifest, this, instance, property);
            }
        }
    }
}