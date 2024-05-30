/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using MoonShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace MoonShared.Config
{
    public class ConfigClassParser
    {
        private readonly IMod Mod;
        private readonly object Config;
        private readonly IGenericModConfigMenuApi Api;
        private readonly ITranslationHelper I18n;
        private readonly ConfigClass ConfigClass;

        public ConfigClassParser(IMod mod, object config)
        {
            this.Mod = mod;
            this.Config = config;
            this.Api = mod.Helper.ModRegistry
                .GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (this.Api == null)
            {
                Log.Trace("Generic Mod Config Menu is not enabled, so will skip parsing");
                return;
            }
            this.I18n = mod.Helper.Translation;
            this.ConfigClass = this.ParseClass();
        }

        private ConfigClass ParseClass()
        {
            // Get the class Attribute
            foreach (object attr in this.Config.GetType().GetCustomAttributes(false))
            {
                if (attr is ConfigClass configClass)
                {
                    return configClass;
                }
            }
            Log.Error("Mod is attempting to parse config file, but is not using ConfigClass attribute.  Please reach out to mod author to enable GenericModConfigMenu integration.");
            return null;
        }

        public bool IsEnabled()
        {
            return this.ConfigClass != null && this.Api != null;
        }

        public bool ParseConfigs()
        {
            if (!this.IsEnabled())
            {
                return false;
            }

            this.Api.Register(
                mod: this.Mod.ModManifest,
                reset: () => this.ReflectiveResetProperties(),
                save: () => this.Mod.Helper.WriteConfig(this.Config),
                titleScreenOnly: this.ConfigClass.TitleScreenOnly
            );

            this.ParseAndRegisterAllPropertyAttributes();

            return true;
        }

        private void ReflectiveResetProperties()
        {
            object copyFrom = Activator.CreateInstance(this.Config.GetType());
            foreach (var property in this.Config.GetType().GetProperties())
            {
                property.SetValue(this.Config, property.GetValue(copyFrom));
            }
        }

        private void ParseAndRegisterAllPropertyAttributes()
        {
            foreach (PropertyInfo propertyInfo in this.Config.GetType().GetProperties())
            {
                foreach (Attribute attr in propertyInfo.GetCustomAttributes(false))
                {
                    if (attr is ConfigBase configBase)
                    {
                        this.RegisterPropertyAttribute(configBase, propertyInfo);
                    }
                }
            }
        }

        private void RegisterPropertyAttribute(ConfigBase attr, PropertyInfo propertyInfo)
        {
            if (attr is ConfigOption configOption)
            {
                if (propertyInfo.PropertyType == typeof(bool))
                {
                    this.AddBoolOption(configOption, propertyInfo);
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    this.AddIntOption(configOption, propertyInfo);
                }
                else if (propertyInfo.PropertyType == typeof(float))
                {
                    this.AddFloatOption(configOption, propertyInfo);
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    this.AddTextOption(configOption, propertyInfo);
                }
                else if (propertyInfo.PropertyType == typeof(SButton))
                {
                    this.AddKeybind(configOption, propertyInfo);
                }
                else if (propertyInfo.PropertyType == typeof(KeybindList))
                {
                    this.AddKeybindList(configOption, propertyInfo);
                }
                else
                {
                    throw new Exception($"Config {attr} had invalid property type {propertyInfo.Name}");
                }
            }
            else if (attr is ConfigSectionTitle configSectionTitle)
            {
                this.AddSectionTitle(configSectionTitle);
            }
            else if (attr is ConfigParagraph configParagraph)
            {
                this.AddParagraph(configParagraph);
            }
            else if (attr is ConfigPage configPage)
            {
                this.AddPage(configPage);
            }
            else if (attr is ConfigPageLink configPageLink)
            {
                this.AddPageLink(configPageLink);
            }
            else if (attr is ConfigTitleScreenOnlyForNextOptions configTitleScreenOnly)
            {
                this.SetTitleScreenOnly(configTitleScreenOnly);
            }
        }


        private void AddSectionTitle(ConfigSectionTitle attr)
        {
            this.Api.AddSectionTitle(
                mod: this.Mod.ModManifest,
                text: this.TextOrDefault(attr.Text),
                tooltip: this.TooltipOrDefault(attr.Tooltip)
            );
        }

        private void AddParagraph(ConfigParagraph attr)
        {
            this.Api.AddParagraph(
                mod: this.Mod.ModManifest,
                text: this.TextOrDefault(attr.Text)
            );
        }

        private void AddBoolOption(ConfigOption attr, PropertyInfo propertyInfo)
        {


            this.Api.AddBoolOption(
                mod: this.Mod.ModManifest,
                getValue: () => (bool)propertyInfo.GetValue(this.Config),
                setValue: value => propertyInfo.SetValue(this.Config, value),
                name: this.NameOrDefault(attr.Name ?? propertyInfo.Name),
                tooltip: this.TooltipOrDefault(attr.Tooltip ?? propertyInfo.Name),
                fieldId: attr.FieldId
            );
        }

        private void AddIntOption(ConfigOption attr, PropertyInfo propertyInfo)
        {
            this.Api.AddNumberOption(
                mod: this.Mod.ModManifest,
                getValue: () => (int)propertyInfo.GetValue(this.Config),
                setValue: value => propertyInfo.SetValue(this.Config, Convert.ToInt32(value)),
                name: this.NameOrDefault(attr.Name ?? propertyInfo.Name),
                tooltip: this.TooltipOrDefault(attr.Tooltip ?? propertyInfo.Name),
                min: attr.Min == float.MaxValue ? null : attr.Min,
                max: attr.Max == float.MinValue ? null : attr.Max,
                interval: attr.Interval == float.MinValue ? null : attr.Interval,
                formatValue: attr.FormatValue == null ? null : value => attr.FormatValue.Invoke(value),
                fieldId: attr.FieldId
            );
        }

        private void AddFloatOption(ConfigOption attr, PropertyInfo propertyInfo)
        {
            this.Api.AddNumberOption(
                mod: this.Mod.ModManifest,
                getValue: () => (float)propertyInfo.GetValue(this.Config),
                setValue: value => propertyInfo.SetValue(this.Config, value),
                name: this.NameOrDefault(attr.Name ?? propertyInfo.Name),
                tooltip: this.TooltipOrDefault(attr.Tooltip ?? propertyInfo.Name),
                min: attr.Min == float.MaxValue ? null : attr.Min,
                max: attr.Max == float.MinValue ? null : attr.Max,
                interval: attr.Interval == float.MinValue ? null : attr.Interval,
                formatValue: attr.FormatValue,
                fieldId: attr.FieldId
            );
        }

        private void AddTextOption(ConfigOption attr, PropertyInfo propertyInfo)
        {
            this.Api.AddTextOption(
                mod: this.Mod.ModManifest,
                getValue: () => (string)propertyInfo.GetValue(this.Config),
                setValue: value => propertyInfo.SetValue(this.Config, value),
                name: this.NameOrDefault(attr.Name ?? propertyInfo.Name),
                tooltip: this.TooltipOrDefault(attr.Tooltip ?? propertyInfo.Name),
                allowedValues: attr.AllowedValues,
                formatAllowedValue: attr.FormatAllowedValues,
                fieldId: attr.FieldId
            );
        }

        private void AddKeybind(ConfigOption attr, PropertyInfo propertyInfo)
        {
            this.Api.AddKeybind(
                mod: this.Mod.ModManifest,
                getValue: () => (SButton)propertyInfo.GetValue(this.Config),
                setValue: value => propertyInfo.SetValue(this.Config, value),
                name: this.NameOrDefault(attr.Name ?? propertyInfo.Name),
                tooltip: this.TooltipOrDefault(attr.Tooltip ?? propertyInfo.Name),
                fieldId: attr.FieldId
            );
        }

        private void AddKeybindList(ConfigOption attr, PropertyInfo propertyInfo)
        {
            this.Api.AddKeybindList(
                mod: this.Mod.ModManifest,
                getValue: () => (KeybindList)propertyInfo.GetValue(this.Config),
                setValue: value => propertyInfo.SetValue(this.Config, value),
                name: this.NameOrDefault(attr.Name ?? propertyInfo.Name),
                tooltip: this.TooltipOrDefault(attr.Tooltip ?? propertyInfo.Name),
                fieldId: attr.FieldId
            );
        }

        private void AddPage(ConfigPage attr)
        {
            this.Api.AddPage(
                mod: this.Mod.ModManifest,
                pageId: attr.PageId,
                pageTitle: this.NameOrDefault(attr.PageTitle)
            );
        }

        private void AddPageLink(ConfigPageLink attr)
        {
            this.Api.AddPageLink(
                mod: this.Mod.ModManifest,
                pageId: attr.PageId,
                text: this.TextOrDefault(attr.Text),
                tooltip: this.TooltipOrDefault(attr.Tooltip)
            );
        }

        private void SetTitleScreenOnly(ConfigTitleScreenOnlyForNextOptions attr)
        {
            this.Api.SetTitleScreenOnlyForNextOptions(
                mod: this.Mod.ModManifest,
                titleScreenOnly: attr.TitleScreenOnly
            );
        }


        private Func<string> NameOrDefault(string possibleKey)
        {
            return this.I18nOrDefault(possibleKey, this.ConfigClass.I18NNameSuffix);
        }

        private Func<string> TextOrDefault(string possibleKey)
        {
            return this.I18nOrDefault(possibleKey, this.ConfigClass.I18NTextSuffix);
        }

        private Func<string> TooltipOrDefault(string possibleKey)
        {
            return this.I18nOrDefault(possibleKey, this.ConfigClass.I18NTooltipSuffix, this.ConfigClass.AllowNullOptionTooltips);
        }

        private Func<string> I18nOrDefault(string possibleKey, string suffix, bool nullable = false)
        {
            if (possibleKey == null)
            {
                return null;
            }

            string key = this.ConfigClass.I18NPrefix + TransformUtility.TransformText(possibleKey, this.ConfigClass.I18NInfixTransform) + suffix;

            Translation attemptedTranslation = this.I18n.Get(key);
            if (attemptedTranslation != null && attemptedTranslation.HasValue())
            {
                return () => this.I18n.Get(key);
            }
            else
            {
                if (nullable)
                {
                    return null;
                }
                else
                {
                    return () => possibleKey;
                }
            }
        }
    }

    public class TransformUtility
    {
        private static readonly Dictionary<string, Func<string, string>> TextTransforms = new() {
            { "ToDashCase", ToDashCase },
        };

        private static string ToDashCase(string value)
        {
            return Regex.Replace(value, "(?<!^)([A-Z])", "-$1").ToLower();
        }

        internal static string TransformText(string value, string transform)
        {
            if (transform == null)
            {
                return value;
            }
            if (TextTransforms.TryGetValue(transform, out Func<string, string> transformFunc))
            {
                return transformFunc(value);
            }
            return value;
        }
    }
}
