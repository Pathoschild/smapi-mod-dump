/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;

namespace MultiplayerEmotes
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

        /// <summary>Get a translation equivalent to "Emotes".</summary>
        public static string Menu_EmotesButton_HoverText()
        {
            return I18n.GetByKey("menu.emotes-button.hover-text");
        }

        /// <summary>Get a translation equivalent to "Play the emote animation with the passed id.".</summary>
        public static string Command_PlayEmote()
        {
            return I18n.GetByKey("command.play_emote");
        }

        /// <summary>Get a translation equivalent to "Usage: play_emote &lt;value&gt;\n- value: a integer representing the animation id.".</summary>
        public static string Command_PlayEmote_Usage()
        {
            return I18n.GetByKey("command.play_emote.usage");
        }

        /// <summary>Get a translation equivalent to "Force a npc to play the emote animation with the given id.".</summary>
        public static string Command_PlayEmoteNpc()
        {
            return I18n.GetByKey("command.play_emote_npc");
        }

        /// <summary>Get a translation equivalent to "Usage: play_emote_npc &lt;name&gt; &lt;value&gt;\n- name: a string representing the npc name.\n- value: a integer representing the animation id.".</summary>
        public static string Command_PlayEmoteNpc_Usage()
        {
            return I18n.GetByKey("command.play_emote_npc.usage");
        }

        /// <summary>Get a translation equivalent to "Force a farm animal to play the emote animation with the given id.".</summary>
        public static string Command_PlayEmoteAnimal()
        {
            return I18n.GetByKey("command.play_emote_animal");
        }

        /// <summary>Get a translation equivalent to "Usage: play_emote_animal &lt;name&gt; &lt;value&gt;\n- name: a string representing the farm animal name.\n- value: a integer representing the animation id.".</summary>
        public static string Command_PlayEmoteAnimal_Usage()
        {
            return I18n.GetByKey("command.play_emote_animal.usage");
        }

        /// <summary>Get a translation equivalent to "Stop any emote being played by you.".</summary>
        public static string Command_StopEmote()
        {
            return I18n.GetByKey("command.stop_emote");
        }

        /// <summary>Get a translation equivalent to "Usage: stop_emote".</summary>
        public static string Command_StopEmote_Usage()
        {
            return I18n.GetByKey("command.stop_emote.usage");
        }

        /// <summary>Get a translation equivalent to "Stop any emote being played by an NPC.".</summary>
        public static string Command_StopEmoteNpc()
        {
            return I18n.GetByKey("command.stop_emote_npc");
        }

        /// <summary>Get a translation equivalent to "Usage: stop_emote_npc".</summary>
        public static string Command_StopEmoteNpc_Usage()
        {
            return I18n.GetByKey("command.stop_emote_npc.usage");
        }

        /// <summary>Get a translation equivalent to "Stop any emote being played by a farm animal.".</summary>
        public static string Command_StopEmoteAnimal()
        {
            return I18n.GetByKey("command.stop_emote_animal");
        }

        /// <summary>Get a translation equivalent to "Usage: stop_emote_animal".</summary>
        public static string Command_StopEmoteAnimal_Usage()
        {
            return I18n.GetByKey("command.stop_emote_animal.usage");
        }

        /// <summary>Get a translation equivalent to "Stop any emote being played.".</summary>
        public static string Command_StopAllEmotes()
        {
            return I18n.GetByKey("command.stop_all_emotes");
        }

        /// <summary>Get a translation equivalent to "Usage: stop_all_emotes".</summary>
        public static string Command_StopAllEmotes_Usage()
        {
            return I18n.GetByKey("command.stop_all_emotes.usage");
        }

        /// <summary>Get a translation equivalent to "List all the players that have this mod and can send and receive emotes.".</summary>
        public static string Command_MultiplayerEmotes()
        {
            return I18n.GetByKey("command.multiplayer_emotes");
        }

        /// <summary>Get a translation equivalent to "Usage: multiplayer_emotes".</summary>
        public static string Command_MultiplayerEmotes_Usage()
        {
            return I18n.GetByKey("command.multiplayer_emotes.usage");
        }

        /// <summary>Get a translation equivalent to "Missing parameters.".</summary>
        public static string Command_MissingParameters()
        {
            return I18n.GetByKey("command.missing_parameters");
        }

        /// <summary>Get a translation equivalent to "Permission denied. You dont have enough permissions to run this command.".</summary>
        public static string Command_PermissionsDenied()
        {
            return I18n.GetByKey("command.permissions_denied");
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
