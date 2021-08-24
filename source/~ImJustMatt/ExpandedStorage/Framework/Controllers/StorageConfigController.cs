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
using System.Collections.Generic;
using ExpandedStorage.API;
using ExpandedStorage.Framework.Models;
using Helpers.ConfigData;
using Common.Integrations.GenericModConfigMenu;
using Newtonsoft.Json;
using StardewModdingAPI;

namespace ExpandedStorage.Framework.Controllers
{
    public class StorageConfigController : StorageConfigModel
    {
        public enum Choice
        {
            Unspecified,
            Enable,
            Disable
        }

        /// <summary>Default storage config for unspecified options</summary>
        private static StorageConfigController _defaultConfig;

        internal static readonly ConfigHelper ConfigHelper = new(new FieldHandler(), new StorageConfigController(), new List<KeyValuePair<string, string>>
        {
            new("Capacity", "Number of item slots the storage will contain"),
            new("AccessCarried", "Allow storage to be access while carried"),
            new("CanCarry", "Allow storage to be picked up"),
            new("Indestructible", "Cannot be broken by tools even while empty"),
            new("ShowColorPicker", "Show color toggle and bars for colorable storages"),
            new("ShowSearchBar", "Show search bar above chest inventory"),
            new("ShowTabs", "Show tabs below chest inventory"),
            new("VacuumItems", "Allow storage to automatically collect dropped items")
        });

        private StorageConfigController _parent;

        [JsonConstructor]
        internal StorageConfigController(IStorageConfig config = null)
        {
            if (config == null)
                return;
            Capacity = config.Capacity;
            EnabledFeatures = new HashSet<string>(config.EnabledFeatures);
            DisabledFeatures = new HashSet<string>(config.DisabledFeatures);
        }

        internal static IList<string> DefaultTabs => _defaultConfig?.Tabs;

        /// <summary>Parent storage config for unspecified options</summary>
        internal StorageConfigController ParentConfig
        {
            get => _parent ?? _defaultConfig;
            set => _parent = value;
        }

        internal StorageMenuController Menu => Capacity == 0 && !ReferenceEquals(ParentConfig, this)
            ? ParentConfig.Menu
            : new StorageMenuController(this);

        internal int ActualCapacity =>
            Capacity switch
            {
                0 => ReferenceEquals(ParentConfig, this) ? 0 : ParentConfig?.ActualCapacity ?? 0,
                -1 => int.MaxValue,
                _ => Capacity
            };

        internal void SetDefault()
        {
            _defaultConfig = this;
        }

        internal Choice Option(string option, bool globalOverride = false)
        {
            if (DisabledFeatures.Contains(option))
                return Choice.Disable;
            if (EnabledFeatures.Contains(option))
                return Choice.Enable;
            return globalOverride && !ReferenceEquals(ParentConfig, this)
                ? ParentConfig?.Option(option, true) ?? Choice.Unspecified
                : Choice.Unspecified;
        }

        internal void RegisterModConfig(string storageName, IManifest manifest, GenericModConfigMenuIntegration modConfigMenu)
        {
            if (!modConfigMenu.IsLoaded)
                return;

            // Add Expanded Storage to Generic Mod Config Menu
            modConfigMenu.API.StartNewPage(manifest, storageName);
            modConfigMenu.API.RegisterLabel(manifest, storageName, "");
            modConfigMenu.RegisterConfigOptions(manifest, ConfigHelper, this);
            modConfigMenu.API.RegisterPageLabel(manifest, "Go Back", "", "");
        }

        private class FieldHandler : BaseFieldHandler
        {
            private static readonly string[] Choices = Enum.GetNames(typeof(Choice));

            public override bool CanHandle(IField field)
            {
                return !field.Name.Equals("Capacity");
            }

            public override object GetValue(object instance, IField field)
            {
                return ((StorageConfigController) instance).Option(field.Name);
            }

            public override void SetValue(object instance, IField field, object value)
            {
                var storageConfig = (StorageConfigController) instance;
                storageConfig.EnabledFeatures.Remove(field.Name);
                storageConfig.DisabledFeatures.Remove(field.Name);
                if (value is not string stringValue || !Enum.TryParse(stringValue, out Choice choice))
                    return;
                switch (choice)
                {
                    case Choice.Disable:
                        storageConfig.DisabledFeatures.Add(field.Name);
                        break;
                    case Choice.Enable:
                        storageConfig.EnabledFeatures.Add(field.Name);
                        break;
                    case Choice.Unspecified:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public override void RegisterConfigOption(IManifest manifest, GenericModConfigMenuIntegration modConfigMenu, object instance, IField field)
            {
                modConfigMenu.API.RegisterChoiceOption(
                    manifest,
                    field.Name,
                    field.Description,
                    () => GetValue(instance, field).ToString(),
                    value => SetValue(instance, field, value),
                    Choices
                );
            }
        }
    }
}