/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sarahvloos/StardewMods
**
*************************************************/

using AlwaysShowBarValues.UIElements;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysShowBarValues.Config
{
    internal class GenericModConfigMenuRegistry
    {
        private readonly ModConfig Config;
        private readonly IGenericModConfigMenuApi ConfigMenu;
        private readonly IManifest ModManifest;
        internal GenericModConfigMenuRegistry(ModConfig config, IManifest modManifest, IGenericModConfigMenuApi configMenu)
        {
            this.Config = config;
            this.ModManifest = modManifest;
            this.ConfigMenu = configMenu;
        }

        internal void RegisterAll(List<StatBox> boxes)
        {
            this.RegisterPage();
            this.AddPageLinks(boxes);
            this.RegisterModPages(boxes);
        }

        internal void RegisterPage(string predicate = "", string topValue = "Health", string bottomValue = "Stamina")
        {
            this.ConfigMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Visibility"
            );
            this.ConfigMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enable box",
                tooltip: () => "This box will not be visible unless this is checked.",
                getValue: () => (bool)this.GetConfig(predicate, "Enabled"),
                setValue: value => this.SetConfig(predicate, "Enabled", value)
            );
            this.ConfigMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Show above HUD",
                tooltip: () => "Check this to show the box above all HUD elements, leave unchecked to show it below everything.",
                getValue: () => (bool)this.GetConfig(predicate, "Above"),
                setValue: value => this.SetConfig(predicate, "Above", value)
            );
            this.ConfigMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Toggle key",
                tooltip: () => "Press the toggle key to show or hide the box with values. The box will not show if the rest of the HUD is hidden.",
                getValue: () => (KeybindList)this.GetConfig(predicate, "ToggleKey"),
                setValue: value => this.SetConfig(predicate, "ToggleKey", value)
            );
            this.ConfigMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Position"
            );
            this.ConfigMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Bar Value Position",
                getValue: () => (string)this.GetConfig(predicate, "Position"),
                setValue: value => this.SetConfig(predicate, "Position", value),
                allowedValues: new string[] { "Bottom Left", "Center Left", "Top Left", "Top Center", "Bottom Right", "Center Right", "Custom" }
            );
            this.ConfigMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Override X Position",
                tooltip: () => "Choose 'Custom' as your bar value position, then change the horizontal position here.",
                getValue: () => (int)this.GetConfig(predicate, "X"),
                setValue: value => this.SetConfig(predicate, "X", value)
            );
            this.ConfigMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Override Y Position",
                tooltip: () => "Choose 'Custom' as your bar value position, then change the horizontal position here.",
                getValue: () => (int)this.GetConfig(predicate, "Y"),
                setValue: value => this.SetConfig(predicate, "Y", value)
            );
            this.ConfigMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Background"
            );
            this.ConfigMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Box Style",
                getValue: () => (string)this.GetConfig(predicate, "BoxStyle"),
                setValue: value => this.SetConfig(predicate, "BoxStyle", value),
                allowedValues: new string[] { "Round", "Toolbar", "None" }
            );
            this.ConfigMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Text"
            );
            this.ConfigMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Place icons to the left of text",
                tooltip: () => "Whether icons should be to the left of text, or to the right of the numbers.",
                getValue: () => (bool)this.GetConfig(predicate, "LeftIcon"),
                setValue: value => this.SetConfig(predicate, "LeftIcon", value)
                ) ;
            this.ConfigMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Add Shadow to Text",
                tooltip: () => "Whether you want the text to have a shadow underneath.",
                getValue: () => (bool)this.GetConfig(predicate, "TextShadow"),
                setValue: value => this.SetConfig(predicate, "TextShadow", value)
            );
            this.ConfigMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => topValue + " Text Color Scheme",
                getValue: () => (string)this.GetConfig(predicate, "HealthColorMode"),
                setValue: value => this.SetConfig(predicate, "HealthColorMode", value),
                allowedValues: new string[] { "Black", "Green/Yellow/Red", "Blue/Yellow/Red", "Blue/Black/Red", "Custom" }
            );
            this.ConfigMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => bottomValue + " Text Color Scheme",
                getValue: () => (string)this.GetConfig(predicate, "StaminaColorMode"),
                setValue: value => this.SetConfig(predicate, "StaminaColorMode", value),
                allowedValues: new string[] { "Black", "Green/Yellow/Red", "Blue/Yellow/Red", "Blue/Black/Red", "Custom" }
            );
            this.ConfigMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Custom Text Colors"
            );
            this.ConfigMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "The following settings only work if you selected Custom as the color scheme for the text you're trying to edit. Colors must be a hex code (like #00ff00). Reshades will affect the color you choose."
            );
            this.ConfigMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Full " + topValue + " Color Override",
                tooltip: () => "What color should the " + topValue + " text be when it's 100%?",
                getValue: () => (string)this.GetConfig(predicate, "MaxHealthHex"),
                setValue: value => this.SetConfig(predicate, "MaxHealthHex", value)
            );
            this.ConfigMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Half " + topValue + " Color Override",
                tooltip: () => "What color should the " + topValue + " text be when it's 50%?",
                getValue: () => (string)this.GetConfig(predicate, "MiddleHealthHex"),
                setValue: value => this.SetConfig(predicate, "MiddleHealthHex", value)
            );
            this.ConfigMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Zero " + topValue + " Color Override",
                tooltip: () => "What color should the " + topValue + " text be when it's 0%?",
                getValue: () => (string)this.GetConfig(predicate, "MinHealthHex"),
                setValue: value => this.SetConfig(predicate, "MinHealthHex", value)
            );
            this.ConfigMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Full " + bottomValue + " Color Override",
                tooltip: () => "What color should the " + bottomValue + " text be when it's 100%?",
                getValue: () => (string)this.GetConfig(predicate, "MaxStaminaHex"),
                setValue: value => this.SetConfig(predicate, "MaxStaminaHex", value)
            );
            this.ConfigMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Half " + bottomValue + " Color Override",
                tooltip: () => "What color should the " + bottomValue + " text be when it's 50%?",
                getValue: () => (string)this.GetConfig(predicate, "MiddleStaminaHex"),
                setValue: value => this.SetConfig(predicate, "MiddleStaminaHex", value)
            );
            this.ConfigMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Zero " + bottomValue + " Color Override",
                tooltip: () => "What color should the " + bottomValue + " text be when it's 0%?",
                getValue: () => (string)this.GetConfig(predicate, "MinStaminaHex"),
                setValue: value => this.SetConfig(predicate, "MinStaminaHex", value)
            );
        }

        private void AddPageLinks(List<StatBox> integrations)
        {
            foreach (StatBox integration in integrations)
            {
                if(integration.BoxName != "main")
                    this.ConfigMenu.AddPageLink(
                        mod: this.ModManifest,
                        pageId: integration.BoxName,
                        text: () => integration.BoxName + " Options"
                     );
            }
        }

        private void RegisterModPages(List<StatBox> integrations)
        {
            foreach (StatBox integration in integrations)
            {
                if (integration.BoxName != "main")
                {
                    this.ConfigMenu.AddPage(
                    mod: this.ModManifest,
                    pageId: integration.BoxName,
                    pageTitle: () => integration.BoxName + " Options"
                    );
                    this.RegisterPage(integration.BoxName, integration.TopValue.StatName, integration.BottomValue.StatName);
                }
            }
        }

        private object GetConfig(string predicate, string key)
        {
            if (GetPropertyInfo(predicate, key) is not PropertyInfo propInfo
                || propInfo.GetValue(Config, null) is not object value) 
                return "ERROR";
            return value;
        }

        private void SetConfig(string predicate, string key, object value)
        {
            if (GetPropertyInfo(predicate, key) is PropertyInfo propInfo) propInfo.SetValue(Config, value);
        }

        private static PropertyInfo? GetPropertyInfo(string predicate, string key)
        {
            var prop = typeof(ModConfig).GetProperty(predicate + key);
            prop ??= typeof(ModConfig).GetProperty(key);
            return prop;
        }
    }
}
