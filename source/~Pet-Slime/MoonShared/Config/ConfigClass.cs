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
using System.Reflection;

namespace MoonShared.Config
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigClass : Attribute
    {
        /// <summary>
        /// Whether null tooltip values are allowed.
        /// If disallowed, tooltip will default to the property name.
        /// </summary>
        public bool AllowNullOptionTooltips = true;

        /// <summary>
        /// Prefix to use with i18n for all configs.
        /// </summary>
        public string I18NPrefix = "config.";

        /// <summary>
        /// Text transform type to perform on property keys.
        /// </summary>
        public string I18NInfixTransform = null;

        /// <summary>
        /// Suffix to use with i18n for all config names and titles.
        /// </summary>
        public string I18NNameSuffix = ".name";

        /// <summary>
        /// Suffix to use with i18n for all config text.
        /// </summary>
        public string I18NTextSuffix = ".text";

        /// <summary>
        /// Suffix to use with i18n for all config tooltips.
        /// </summary>
        public string I18NTooltipSuffix = ".tooltip";

        /// <summary>
        /// Whether this config menu should only appear on the title screen.
        /// </summary>
        public bool TitleScreenOnly = false;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ConfigBase : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConfigOption : ConfigBase
    {
        /// <summary>
        /// The name of Config Option.  Defaults to property name
        /// </summary>
        public string Name;

        /// <summary>
        /// The tooltip of Config Option.  Defaults depending on class config.
        /// </summary>
        public string Tooltip;

        /// <summary>
        /// Optional Field Id.
        /// </summary>
        public string FieldId;

        /// <summary>
        /// For number configs.  The minimum allowed value.  Defaults to null.
        /// </summary>
        public float Min = float.MaxValue;

        /// <summary>
        /// For number configs.  The maximum allowed value.  Defaults to null.
        /// </summary>
        public float Max = float.MinValue;

        /// <summary>
        /// For number configs.  The interval that changes are incremented by.  Defaults to null.
        /// </summary>
        public float Interval = float.MinValue;

        /// <summary>
        /// For text configs.  The allowed drop-down values.  Defaults to null.
        /// </summary>
        public string[] AllowedValues;

        // unimplemented
        public Func<float, string> FormatValue;

        /// <summary>
        /// Text transform type to perform on allowed values to display.
        /// </summary>
        public Func<string, string> FormatAllowedValues;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ConfigSectionTitle : ConfigBase
    {
        /// <summary>
        /// Section Title, required.
        /// </summary>
        public string Text;

        /// <summary>
        /// Section tooltip.  Defaults to null.
        /// </summary>
        public string Tooltip;

        public ConfigSectionTitle(string text, string tooltip = null)
        {
            this.Text = text;
            this.Tooltip = tooltip;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ConfigParagraph : ConfigBase
    {
        /// <summary>
        /// Paragraph Text, required.
        /// </summary>
        public string Text;

        public ConfigParagraph(string text)
        {
            this.Text = text;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ConfigPage : ConfigBase
    {
        /// <summary>
        /// Page Id, required.
        /// </summary>
        public string PageId;

        /// <summary>
        /// Page Title.  Defaults to null.
        /// </summary>
        public string PageTitle;

        public ConfigPage(string pageId, string pageTitle = null)
        {
            this.PageId = pageId;
            this.PageTitle = pageTitle;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ConfigPageLink : ConfigBase
    {
        /// <summary>
        /// Page Id, required.
        /// </summary>
        public string PageId;

        /// <summary>
        /// Link Text, required.
        /// </summary>
        public string Text;

        /// <summary>
        /// Link Tooltip.  Defaults to null.
        /// </summary>
        public string Tooltip;

        public ConfigPageLink(string pageId, string text, string tooltip = null)
        {
            this.PageId = pageId;
            this.Text = text;
            this.Tooltip = tooltip;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ConfigTitleScreenOnlyForNextOptions : ConfigBase
    {
        /// <summary>
        /// Whether following options should be on the title screen only, required.
        /// </summary>
        public bool TitleScreenOnly;

        public ConfigTitleScreenOnlyForNextOptions(bool titleScreenOnly)
        {
            this.TitleScreenOnly = titleScreenOnly;
        }
    }

    // TODO: Complex Options, Images, FormatValues
}
