/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System.Collections.Generic;
using ImJustMatt.Common.Integrations.GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace Helpers.ConfigData
{
    internal class BaseFieldHandler : IFieldHandler
    {
        public virtual bool CanHandle(IField field) => true;

        public virtual object GetValue(object instance, IField field)
        {
            if (field.Info == null)
                return null;
            var value = field.Info.GetValue(instance, null);
            return value switch
            {
                IList<string> listValues => string.Join(", ", listValues),
                HashSet<string> listValues => string.Join(", ", listValues),
                _ => value
            };
        }

        public virtual void SetValue(object instance, IField field, object value)
        {
            field.Info?.SetValue(instance, value);
        }

        public virtual void CopyValue(IField field, object source, object target)
        {
            field.Info?.SetValue(target, field.Info.GetValue(source, null));
        }

        public virtual void RegisterConfigOption(IManifest manifest, GenericModConfigMenuIntegration modConfigMenu, object instance, IField field)
        {
            if (field.Info?.PropertyType == null)
            {
                return;
            }

            if (field.Info.PropertyType == typeof(KeybindList))
            {
                modConfigMenu.API.RegisterSimpleOption(manifest,
                    field.DisplayName,
                    field.Description,
                    () => (KeybindList) field.Info.GetValue(instance, null),
                    value => field.Info.SetValue(instance, value));
            }
            else if (field.Info.PropertyType == typeof(bool))
            {
                modConfigMenu.API.RegisterSimpleOption(
                    manifest,
                    field.DisplayName,
                    field.Description,
                    () => (bool) field.Info.GetValue(instance, null),
                    value => field.Info.SetValue(instance, value)
                );
            }
            else if (field.Info.PropertyType == typeof(int))
            {
                modConfigMenu.API.RegisterSimpleOption(
                    manifest,
                    field.DisplayName,
                    field.Description,
                    () => (int) field.Info.GetValue(instance, null),
                    value => field.Info.SetValue(instance, value)
                );
            }
            else if (field.Info.PropertyType == typeof(string))
            {
                modConfigMenu.API.RegisterSimpleOption(
                    manifest,
                    field.DisplayName,
                    field.Description,
                    () => (string) field.Info.GetValue(instance, null),
                    value => field.Info.SetValue(instance, value)
                );
            }
        }
    }
}