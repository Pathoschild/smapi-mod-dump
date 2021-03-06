/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;

namespace ToDew
{
    /// <summary>Get translations from the mod's <c>i18n</c> folder.</summary>
    /// <remarks>This is auto-generated from the <c>i18n/default.json</c> file when the T4 template is saved.</remarks>
    [GeneratedCode("TextTemplatingFileGenerator", "1.0.0")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Deliberately named for consistency and to match translation conventions.")]
    internal static class I18n
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod's translation helper.</summary>
        private static ITranslationHelper Translations;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="translations">The mod's translation helper.</param>
        public static void Init(ITranslationHelper translations)
        {
            I18n.Translations = translations;
        }

        /// <summary>Get a translation equivalent to "Hotkey".</summary>
        public static string Config_Hotkey()
        {
            return I18n.GetByKey("config.hotkey");
        }

        /// <summary>Get a translation equivalent to "The key to bring up the to-do list".</summary>
        public static string Config_Hotkey_Desc()
        {
            return I18n.GetByKey("config.hotkey.desc");
        }

        /// <summary>Get a translation equivalent to "Secondary Close Button".</summary>
        public static string Config_SecondaryCloseButton()
        {
            return I18n.GetByKey("config.secondary-close-button");
        }

        /// <summary>Get a translation equivalent to "An alternate key (besides ESC) to close the to-do list".</summary>
        public static string Config_SecondaryCloseButton_Desc()
        {
            return I18n.GetByKey("config.secondary-close-button.desc");
        }

        /// <summary>Get a translation equivalent to "Debug".</summary>
        public static string Config_Debug()
        {
            return I18n.GetByKey("config.debug");
        }

        /// <summary>Get a translation equivalent to "Enable debugging output in the log".</summary>
        public static string Config_Debug_Desc()
        {
            return I18n.GetByKey("config.debug.desc");
        }

        /// <summary>Get a translation equivalent to "Overlay".</summary>
        public static string Config_Overlay()
        {
            return I18n.GetByKey("config.overlay");
        }

        /// <summary>Get a translation equivalent to "Configure the always-on overlay showing the list".</summary>
        public static string Config_Overlay_Desc()
        {
            return I18n.GetByKey("config.overlay.desc");
        }

        /// <summary>Get a translation equivalent to "Enabled".</summary>
        public static string Config_Overlay_Enabled()
        {
            return I18n.GetByKey("config.overlay.enabled");
        }

        /// <summary>Get a translation equivalent to "Is the overlay enabled?".</summary>
        public static string Config_Overlay_Enabled_Desc()
        {
            return I18n.GetByKey("config.overlay.enabled.desc");
        }

        /// <summary>Get a translation equivalent to "Hotkey".</summary>
        public static string Config_Overlay_Hotkey()
        {
            return I18n.GetByKey("config.overlay.hotkey");
        }

        /// <summary>Get a translation equivalent to "Hotkey to show or hide".</summary>
        public static string Config_Overlay_Hotkey_Desc()
        {
            return I18n.GetByKey("config.overlay.hotkey.desc");
        }

        /// <summary>Get a translation equivalent to "Hide at festivals".</summary>
        public static string Config_Overlay_HideAtFestivals()
        {
            return I18n.GetByKey("config.overlay.hide-at-festivals");
        }

        /// <summary>Get a translation equivalent to "Hide the overlay during festivals?".</summary>
        public static string Config_Overlay_HideAtFestivals_Desc()
        {
            return I18n.GetByKey("config.overlay.hide-at-festivals.desc");
        }

        /// <summary>Get a translation equivalent to "Max Width".</summary>
        public static string Config_Overlay_MaxWidth()
        {
            return I18n.GetByKey("config.overlay.max-width");
        }

        /// <summary>Get a translation equivalent to "Maximum width of the overlay in pixels".</summary>
        public static string Config_Overlay_MaxWidth_Desc()
        {
            return I18n.GetByKey("config.overlay.max-width.desc");
        }

        /// <summary>Get a translation equivalent to "Max Items".</summary>
        public static string Config_Overlay_MaxItems()
        {
            return I18n.GetByKey("config.overlay.max-items");
        }

        /// <summary>Get a translation equivalent to "Maximum number of items to show in the overlay".</summary>
        public static string Config_Overlay_MaxItems_Desc()
        {
            return I18n.GetByKey("config.overlay.max-items.desc");
        }

        /// <summary>Get a translation equivalent to "To-Dew List".</summary>
        public static string Overlay_Header()
        {
            return I18n.GetByKey("overlay.header");
        }

        /// <summary>Get a translation equivalent to "List information in this save is in a newer format version ({{saveFormatVersion}}) than this version of {{modName}} uses ({{modFormatVersion}}).  The next save will use this older version, which will result in losing any attributes added in the newer version.".</summary>
        /// <param name="saveFormatVersion">The value to inject for the <c>{{saveFormatVersion}}</c> token.</param>
        /// <param name="modName">The value to inject for the <c>{{modName}}</c> token.</param>
        /// <param name="modFormatVersion">The value to inject for the <c>{{modFormatVersion}}</c> token.</param>
        public static string Message_SaveVersionNewer(object saveFormatVersion, object modName, object modFormatVersion)
        {
            return I18n.GetByKey("message.save-version-newer", new { saveFormatVersion, modName, modFormatVersion });
        }

        /// <summary>Get a translation equivalent to "Read list information in format version {{saveFormatVersion}}.  It will be updated to the current format version ({{modFormatVersion}}) on next save.".</summary>
        /// <param name="saveFormatVersion">The value to inject for the <c>{{saveFormatVersion}}</c> token.</param>
        /// <param name="modFormatVersion">The value to inject for the <c>{{modFormatVersion}}</c> token.</param>
        public static string Message_SaveVersionOlder(object saveFormatVersion, object modFormatVersion)
        {
            return I18n.GetByKey("message.save-version-older", new { saveFormatVersion, modFormatVersion });
        }

        /// <summary>Get a translation equivalent to "Host does not have {{modName}} installed; list disabled.".</summary>
        /// <param name="modName">The value to inject for the <c>{{modName}}</c> token.</param>
        public static string Message_HostNoMod(object modName)
        {
            return I18n.GetByKey("message.host-no-mod", new { modName });
        }

        /// <summary>Get a translation equivalent to "Host has version {{hostVersion}} of {{modName}} installed, but you have version {{myVersion}}.  Attempting to proceed anyway.".</summary>
        /// <param name="hostVersion">The value to inject for the <c>{{hostVersion}}</c> token.</param>
        /// <param name="modName">The value to inject for the <c>{{modName}}</c> token.</param>
        /// <param name="myVersion">The value to inject for the <c>{{myVersion}}</c> token.</param>
        public static string Message_HostDifferentModVersion(object hostVersion, object modName, object myVersion)
        {
            return I18n.GetByKey("message.host-different-mod-version", new { hostVersion, modName, myVersion });
        }

        /// <summary>Get a translation equivalent to "Ignoring unexpected message type {{messageType}} from player {{fromId}} ({{fromName}})".</summary>
        /// <param name="messageType">The value to inject for the <c>{{messageType}}</c> token.</param>
        /// <param name="fromId">The value to inject for the <c>{{fromId}}</c> token.</param>
        /// <param name="fromName">The value to inject for the <c>{{fromName}}</c> token.</param>
        public static string Message_IgnoringUnexpectedMessageType(object messageType, object fromId, object fromName)
        {
            return I18n.GetByKey("message.ignoring-unexpected-message-type", new { messageType, fromId, fromName });
        }

        /// <summary>Get a translation equivalent to "Host is using a newer data format version ({{hostVersion}}) than this version of {{modName}} uses ({{myVersion}}), but we'll do our best to deal with it.".</summary>
        /// <param name="hostVersion">The value to inject for the <c>{{hostVersion}}</c> token.</param>
        /// <param name="modName">The value to inject for the <c>{{modName}}</c> token.</param>
        /// <param name="myVersion">The value to inject for the <c>{{myVersion}}</c> token.</param>
        public static string Message_HostNewerDataFormatMinor(object hostVersion, object modName, object myVersion)
        {
            return I18n.GetByKey("message.host-newer-data-format-minor", new { hostVersion, modName, myVersion });
        }

        /// <summary>Get a translation equivalent to "Host is using a newer data format version ({{hostVersion}}) than this version of {{modName}} uses ({{myVersion}}).  Since it has a different major version number, we're not going to try to handle it.".</summary>
        /// <param name="hostVersion">The value to inject for the <c>{{hostVersion}}</c> token.</param>
        /// <param name="modName">The value to inject for the <c>{{modName}}</c> token.</param>
        /// <param name="myVersion">The value to inject for the <c>{{myVersion}}</c> token.</param>
        public static string Message_HostNewerDataFormatMajor(object hostVersion, object modName, object myVersion)
        {
            return I18n.GetByKey("message.host-newer-data-format-major", new { hostVersion, modName, myVersion });
        }

        /// <summary>Get a translation equivalent to "Host is using data format version {{hostVersion}}, which is older than the current format version ({{myVersion}}).  Some features may be unavailable.".</summary>
        /// <param name="hostVersion">The value to inject for the <c>{{hostVersion}}</c> token.</param>
        /// <param name="myVersion">The value to inject for the <c>{{myVersion}}</c> token.</param>
        public static string Message_HostOlderDataFormat(object hostVersion, object myVersion)
        {
            return I18n.GetByKey("message.host-older-data-format", new { hostVersion, myVersion });
        }

        /// <summary>Get a translation equivalent to "Item {{itemId}} was deleted by another player".</summary>
        /// <param name="itemId">The value to inject for the <c>{{itemId}}</c> token.</param>
        public static string Message_CurrentItemDeleted(object itemId)
        {
            return I18n.GetByKey("message.current-item-deleted", new { itemId });
        }

        /// <summary>Get a translation equivalent to "To-Dew List".</summary>
        public static string Menu_List_TitleBoldPart()
        {
            return I18n.GetByKey("menu.list.title-bold-part");
        }

        /// <summary>Get a translation equivalent to "for {{farmName}} farm".</summary>
        /// <param name="farmName">The value to inject for the <c>{{farmName}}</c> token.</param>
        public static string Menu_List_TitleRest(object farmName)
        {
            return I18n.GetByKey("menu.list.title-rest", new { farmName });
        }

        /// <summary>Get a translation equivalent to "Add to-do item".</summary>
        public static string Menu_Textbox_Title()
        {
            return I18n.GetByKey("menu.textbox.title");
        }

        /// <summary>Get a translation equivalent to "Editing item".</summary>
        public static string Menu_Edit_Title()
        {
            return I18n.GetByKey("menu.edit.title");
        }

        /// <summary>Get a translation equivalent to "Header".</summary>
        public static string Menu_Edit_Header()
        {
            return I18n.GetByKey("menu.edit.header");
        }

        /// <summary>Get a translation equivalent to "Bold".</summary>
        public static string Menu_Edit_Bold()
        {
            return I18n.GetByKey("menu.edit.bold");
        }

        /// <summary>Get a translation equivalent to "Repeating".</summary>
        public static string Menu_Edit_Repeating()
        {
            return I18n.GetByKey("menu.edit.repeating");
        }

        /// <summary>Get a translation equivalent to "Hide in Overlay".</summary>
        public static string Menu_Edit_HideInOverlay()
        {
            return I18n.GetByKey("menu.edit.hide-in-overlay");
        }

        /// <summary>Get a translation equivalent to "Show Item When:".</summary>
        public static string Menu_Edit_ShowWhen()
        {
            return I18n.GetByKey("menu.edit.show-when");
        }

        /// <summary>Get a translation equivalent to "Raining on Farm".</summary>
        public static string Menu_Edit_RainingOnFarm()
        {
            return I18n.GetByKey("menu.edit.raining-on-farm");
        }

        /// <summary>Get a translation equivalent to "Not Raining on Farm".</summary>
        public static string Menu_Edit_NotRainingOnFarm()
        {
            return I18n.GetByKey("menu.edit.not-raining-on-farm");
        }

        /// <summary>Get a translation equivalent to "Raining on Island".</summary>
        public static string Menu_Edit_RainingOnIsland()
        {
            return I18n.GetByKey("menu.edit.raining-on-island");
        }

        /// <summary>Get a translation equivalent to "Not Raining on Island".</summary>
        public static string Menu_Edit_NotRainingOnIsland()
        {
            return I18n.GetByKey("menu.edit.not-raining-on-island");
        }

        /// <summary>Get a translation equivalent to "Show Only on These Days:".</summary>
        public static string Menu_Edit_OnlyDays()
        {
            return I18n.GetByKey("menu.edit.only-days");
        }

        /// <summary>Get a translation equivalent to "Week:".</summary>
        public static string Menu_Edit_Week()
        {
            return I18n.GetByKey("menu.edit.week");
        }

        /// <summary>Get a translation equivalent to "Show only in:".</summary>
        public static string Menu_Edit_OnlySeason()
        {
            return I18n.GetByKey("menu.edit.only-season");
        }

        /// <summary>Get a translation equivalent to "Sunday".</summary>
        public static string Sunday()
        {
            return I18n.GetByKey("sunday");
        }

        /// <summary>Get a translation equivalent to "Monday".</summary>
        public static string Monday()
        {
            return I18n.GetByKey("monday");
        }

        /// <summary>Get a translation equivalent to "Tuesday".</summary>
        public static string Tuesday()
        {
            return I18n.GetByKey("tuesday");
        }

        /// <summary>Get a translation equivalent to "Wednesday".</summary>
        public static string Wednesday()
        {
            return I18n.GetByKey("wednesday");
        }

        /// <summary>Get a translation equivalent to "Thursday".</summary>
        public static string Thursday()
        {
            return I18n.GetByKey("thursday");
        }

        /// <summary>Get a translation equivalent to "Friday".</summary>
        public static string Friday()
        {
            return I18n.GetByKey("friday");
        }

        /// <summary>Get a translation equivalent to "Saturday".</summary>
        public static string Saturday()
        {
            return I18n.GetByKey("saturday");
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get a translation by its key.</summary>
        /// <param name="key">The translation key.</param>
        /// <param name="tokens">An object containing token key/value pairs. This can be an anonymous object (like <c>new { value = 42, name = "Cranberries" }</c>), a dictionary, or a class instance.</param>
        private static Translation GetByKey(string key, object tokens = null)
        {
            if (I18n.Translations == null)
                throw new InvalidOperationException($"You must call {nameof(I18n)}.{nameof(I18n.Init)} from the mod's entry method before reading translations.");
            return I18n.Translations.Get(key, tokens);
        }
    }
}

