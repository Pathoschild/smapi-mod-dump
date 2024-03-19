/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

#nullable enable
using System;
using System.Reflection;
using BirbCore.APIs;
using BirbCore.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace BirbCore.Attributes;


/// <summary>
/// Specifies a class as a config class.
/// </summary>
public class SConfig(bool titleScreenOnly = false) : ClassHandler(1)
{
    private static IGenericModConfigMenuApi? _api;

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        if (this.Priority < 1)
        {
            Log.Error("Config cannot be loaded with priority < 1");
            return;
        }

        if (!mod.GetType().TryGetMemberOfType(type, out MemberInfo configField))
        {
            Log.Error("Mod must define a Config property");
            return;
        }

        var getter = configField.GetGetter();
        var setter = configField.GetSetter();
        setter(mod, instance);

        _api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (_api is null)
        {
            Log.Error("Generic Mod Config Menu is not enabled, so will skip parsing");
            return;
        }

        _api.Register(
            mod: mod.ModManifest,
            reset: () =>
            {
                object? copyFrom = Activator.CreateInstance(type);
                object? copyTo = getter(mod);
                foreach (PropertyInfo property in type.GetProperties(ReflectionExtensions.ALL_DECLARED))
                {
                    property.SetValue(copyTo, property.GetValue(copyFrom));
                }
                foreach (FieldInfo field in type.GetFields(ReflectionExtensions.ALL_DECLARED))
                {
                    field.SetValue(copyTo, field.GetValue(copyFrom));
                }
            },
            save: () => mod.Helper.WriteConfig(getter(mod) ?? ""),
            titleScreenOnly: titleScreenOnly
        );

        base.Handle(type, instance, mod);
    }


    /// <summary>
    /// Specifies a property as a config.
    /// </summary>
    public class Option : FieldHandler
    {
        private readonly string? _fieldId;
        private readonly float _min = float.MaxValue;
        private readonly float _max = float.MinValue;
        private readonly float _interval = float.MinValue;
        private readonly string[]? _allowedValues;

        public Option(string? fieldId = null)
        {
            this._fieldId = fieldId;
        }

        public Option(int min, int max, int interval = 1, string? fieldId = null)
        {
            this._fieldId = fieldId;
            this._min = min;
            this._max = max;
            this._interval = interval;
        }

        public Option(float min, float max, float interval = 1.0f, string? fieldId = null)
        {
            this._fieldId = fieldId;
            this._min = min;
            this._max = max;
            this._interval = interval;
        }

        public Option(string[] allowedValues, string? fieldId = null)
        {
            this._fieldId = fieldId;
            this._allowedValues = allowedValues;
        }


        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (_api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            if (fieldType == typeof(bool))
            {
                _api.AddBoolOption(
                    mod: mod.ModManifest,
                    getValue: () => (bool)(getter(instance) ?? false),
                    setValue: value => setter(instance, value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this._fieldId
                );
            }
            else if (fieldType == typeof(int))
            {
                _api.AddNumberOption(
                    mod: mod.ModManifest,
                    getValue: () => (int)(getter(instance) ?? 0),
                    setValue: value => setter(instance, (int)value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this._fieldId,
                    min: this._min == float.MaxValue ? null : this._min,
                    max: this._max == float.MinValue ? null : this._max,
                    interval: this._interval == float.MinValue ? null : this._interval,
                    formatValue: null
                );
            }
            else if (fieldType == typeof(float))
            {
                _api.AddNumberOption(
                    mod: mod.ModManifest,
                    getValue: () => (float)(getter(instance) ?? 0f),
                    setValue: value => setter(instance, value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this._fieldId,
                    min: this._min == float.MaxValue ? null : this._min,
                    max: this._max == float.MinValue ? null : this._max,
                    interval: this._interval == float.MinValue ? null : this._interval,
                    formatValue: null
                );
            }
            else if (fieldType == typeof(string))
            {
                _api.AddTextOption(
                    mod: mod.ModManifest,
                    getValue: () => (string)(getter(instance) ?? ""),
                    setValue: value => setter(instance, value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this._fieldId,
                    allowedValues: this._allowedValues,
                    formatAllowedValue: null
                );
            }
            else if (fieldType == typeof(SButton))
            {
                _api.AddKeybind(
                    mod: mod.ModManifest,
                    getValue: () => (SButton)(getter(instance) ?? SButton.None),
                    setValue: value => setter(instance, value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this._fieldId
                );
            }
            else if (fieldType == typeof(KeybindList))
            {
                _api.AddKeybindList(
                    mod: mod.ModManifest,
                    getValue: () => (KeybindList)(getter(instance) ?? new KeybindList()),
                    setValue: value => setter(instance, value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this._fieldId
                );
            }
            else
            {
                throw new Exception($"Config had invalid property type {name}");
            }
        }
    }

    /// <summary>
    /// Adds a section title to the config menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class SectionTitle(string key) : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (_api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            _api.AddSectionTitle(
                mod: mod.ModManifest,
                text: () => mod.Helper.Translation.Get($"config.{key}").Default(key),
                tooltip: () => mod.Helper.Translation.Get($"config.{key}.tooltip").UsePlaceholder(false)
            );
        }
    }

    /// <summary>
    /// Adds a paragraph to the config menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class Paragraph(string key) : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (_api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            _api.AddParagraph(
                mod: mod.ModManifest,
                text: () => mod.Helper.Translation.Get($"config.{key}").Default(key)
            );
        }
    }

    /// <summary>
    /// Starts a page block.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class PageBlock(string pageId) : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (_api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            _api.AddPage(
                mod: mod.ModManifest,
                pageId: pageId,
                pageTitle: () => mod.Helper.Translation.Get($"config.{pageId}").Default(pageId)
            );
        }
    }

    /// <summary>
    /// Adds a link to a config page to the config menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class PageLink(string pageId) : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (_api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            _api.AddPageLink(
                mod: mod.ModManifest,
                pageId: pageId,
                text: () => mod.Helper.Translation.Get($"config.{pageId}").Default(pageId),
                tooltip: () => mod.Helper.Translation.Get($"config.{pageId}.tooltip").UsePlaceholder(false)
            );
        }
    }

    /// <summary>
    /// Adds an image to the config menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class Image(string texture, int x = 0, int y = 0, int width = 0, int height = 0) : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (_api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            _api.AddImage(
                mod: mod.ModManifest,
                texture: () => mod.Helper.GameContent.Load<Texture2D>(texture),
                texturePixelArea: width != 0 ? new Rectangle(x, y, width, height) : null
            );
        }
    }

    /// <summary>
    /// Starts or ends a block of title-screen exclusive configs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class StartTitleOnlyBlock : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (_api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            _api.SetTitleScreenOnlyForNextOptions(
                mod: mod.ModManifest,
                titleScreenOnly: true
            );
        }
    }

    /// <summary>
    /// Starts or ends a block of title-screen exclusive configs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class EndTitleOnlyBlock : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (_api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            _api.SetTitleScreenOnlyForNextOptions(
                mod: mod.ModManifest,
                titleScreenOnly: false
            );
        }
    }

}
