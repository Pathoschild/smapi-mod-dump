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

namespace BirbCore.Attributes;


/// <summary>
/// Specifies a class as a config class.
/// </summary>
public class SConfig : ClassHandler
{
    public bool TitleScreenOnly = false;

    private static IGenericModConfigMenuApi? Api;

    public SConfig(bool titleScreenOnly = false) : base(1)
    {
        this.TitleScreenOnly = titleScreenOnly;
    }

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        if (this.Priority < 1)
        {
            Log.Error("Config cannot be loaded with priority < 1");
            return;
        }
        MemberInfo configField = mod.GetType().GetMemberOfType(type);
        if (configField == null)
        {
            Log.Error("Mod must define a Config property");
            return;
        }

        var getter = configField.GetGetter();
        var setter = configField.GetSetter();
        instance = Activator.CreateInstance(type);
        setter(mod, instance);

        Api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (Api is null)
        {
            Log.Error("Generic Mod Config Menu is not enabled, so will skip parsing");
            return;
        }

        Api.Register(
            mod: mod.ModManifest,
            reset: () =>
            {
                object? copyFrom = Activator.CreateInstance(type);
                object? copyTo = getter(mod);
                foreach (PropertyInfo property in type.GetProperties(ReflectionExtensions.AllDeclared))
                {
                    property.SetValue(copyTo, property.GetValue(copyFrom));
                }
                foreach (FieldInfo field in type.GetFields(ReflectionExtensions.AllDeclared))
                {
                    field.SetValue(copyTo, field.GetValue(copyFrom));
                }
            },
            save: () => mod.Helper.WriteConfig(getter(mod)),
            titleScreenOnly: this.TitleScreenOnly
        );

        base.Handle(type, instance, mod);

        return;
    }


    /// <summary>
    /// Specifies a property as a config.
    /// </summary>
    public class Option : FieldHandler
    {
        public string? FieldId;
        public float Min = float.MaxValue;
        public float Max = float.MinValue;
        public float Interval = float.MinValue;
        public string[]? AllowedValues;

        public Option(string? fieldId = null)
        {
            this.FieldId = fieldId;
        }

        public Option(int min, int max, int interval = 1, string? fieldId = null)
        {
            this.FieldId = fieldId;
            this.Min = min;
            this.Max = max;
            this.Interval = interval;
        }

        public Option(float min, float max, float interval = 1.0f, string? fieldId = null)
        {
            this.FieldId = fieldId;
            this.Min = min;
            this.Max = max;
            this.Interval = interval;
        }

        public Option(string[] allowedValues, string? fieldId = null)
        {
            this.FieldId = fieldId;
            this.AllowedValues = allowedValues;
        }



        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (Api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            if (fieldType == typeof(bool))
            {
                Api.AddBoolOption(
                    mod: mod.ModManifest,
                    getValue: () => (bool)(getter(instance) ?? false),
                    setValue: value => setter(instance, value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this.FieldId
                );
            }
            else if (fieldType == typeof(int))
            {
                Api.AddNumberOption(
                    mod: mod.ModManifest,
                    getValue: () => (int)(getter(instance) ?? 0),
                    setValue: value => setter(instance, (int)value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this.FieldId,
                    min: this.Min == float.MaxValue ? null : this.Min,
                    max: this.Max == float.MinValue ? null : this.Max,
                    interval: this.Interval == float.MinValue ? null : this.Interval,
                    formatValue: null
                );
            }
            else if (fieldType == typeof(float))
            {
                Api.AddNumberOption(
                    mod: mod.ModManifest,
                    getValue: () => (float)(getter(instance) ?? 0f),
                    setValue: value => setter(instance, value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this.FieldId,
                    min: this.Min == float.MaxValue ? null : this.Min,
                    max: this.Max == float.MinValue ? null : this.Max,
                    interval: this.Interval == float.MinValue ? null : this.Interval,
                    formatValue: null
                );
            }
            else if (fieldType == typeof(string))
            {
                Api.AddTextOption(
                    mod: mod.ModManifest,
                    getValue: () => (string)(getter(instance) ?? ""),
                    setValue: value => setter(instance, value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this.FieldId,
                    allowedValues: this.AllowedValues,
                    formatAllowedValue: null
                );
            }
            else if (fieldType == typeof(SButton))
            {
                Api.AddKeybind(
                    mod: mod.ModManifest,
                    getValue: () => (SButton)(getter(instance) ?? SButton.None),
                    setValue: value => setter(instance, value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this.FieldId
                );
            }
            else if (fieldType == typeof(KeybindList))
            {
                Api.AddKeybindList(
                    mod: mod.ModManifest,
                    getValue: () => (KeybindList)(getter(instance) ?? new KeybindList()),
                    setValue: value => setter(instance, value),
                    name: () => mod.Helper.Translation.Get($"config.{name}").Default(name),
                    tooltip: () => mod.Helper.Translation.Get($"config.{name}.tooltip").UsePlaceholder(false),
                    fieldId: this.FieldId
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
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SectionTitle : FieldHandler
    {
        public string Key;

        public SectionTitle(string key)
        {
            this.Key = key;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (Api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            Api.AddSectionTitle(
                mod: mod.ModManifest,
                text: () => mod.Helper.Translation.Get($"config.{this.Key}").Default(this.Key),
                tooltip: () => mod.Helper.Translation.Get($"config.{this.Key}.tooltip").UsePlaceholder(false)
            );
        }
    }

    /// <summary>
    /// Adds a paragraph to the config menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Paragraph : FieldHandler
    {
        public string Key;

        public Paragraph(string key)
        {
            this.Key = key;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (Api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            Api.AddParagraph(
                mod: mod.ModManifest,
                text: () => mod.Helper.Translation.Get($"config.{this.Key}").Default(this.Key)
            );
        }
    }

    /// <summary>
    /// Starts a page block.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PageBlock : FieldHandler
    {
        public string PageId;

        public PageBlock(string pageId)
        {
            this.PageId = pageId;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (Api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            Api.AddPage(
                mod: mod.ModManifest,
                pageId: this.PageId,
                pageTitle: () => mod.Helper.Translation.Get($"config.{this.PageId}").Default(this.PageId)
            );
        }
    }

    /// <summary>
    /// Adds a link to a config page to the config menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PageLink : FieldHandler
    {
        public string PageId;

        public PageLink(string pageId)
        {
            this.PageId = pageId;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (Api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            Api.AddPageLink(
                mod: mod.ModManifest,
                pageId: this.PageId,
                text: () => mod.Helper.Translation.Get($"config.{this.PageId}").Default(this.PageId),
                tooltip: () => mod.Helper.Translation.Get($"config.{this.PageId}.tooltip").UsePlaceholder(false)
            );
        }
    }

    /// <summary>
    /// Adds an image to the config menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Image : FieldHandler
    {
        public string Texture;
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Image(string texture)
        {
            this.Texture = texture;
        }

        public Image(string texture, int x, int y, int width, int height)
        {
            this.Texture = texture;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (Api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            Api.AddImage(
                mod: mod.ModManifest,
                texture: () => mod.Helper.GameContent.Load<Texture2D>(this.Texture),
                texturePixelArea: this.Width != 0 ? new Rectangle(this.X, this.Y, this.Width, this.Height) : null
            );
        }
    }

    /// <summary>
    /// Starts or ends a block of title-screen exclusive configs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class StartTitleOnlyBlock : FieldHandler
    {
        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (Api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            Api.SetTitleScreenOnlyForNextOptions(
                mod: mod.ModManifest,
                titleScreenOnly: true
            );
        }
    }

    /// <summary>
    /// Starts or ends a block of title-screen exclusive configs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class EndTitleOnlyBlock : FieldHandler
    {
        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            if (Api is null)
            {
                Log.Error("Attempting to use GMCM API before it is initialized");
                return;
            }
            Api.SetTitleScreenOnlyForNextOptions(
                mod: mod.ModManifest,
                titleScreenOnly: false
            );
        }
    }

}
