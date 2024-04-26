/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using stardew_access.Translation;
using StardewValley;

namespace stardew_access.Commands;

public class OtherCommands
{
    // TODO: add Refresh functionality to `AccessibleTileManager and restore this
    /*helper.ConsoleCommands.Add("refst", "Refresh static tiles", (string command, string[] args) =>
    {
        StaticTiles.LoadTilesFiles();
        StaticTiles.SetupTilesDicts();

        Log.Info("Static tiles refreshed!");
    });*/

    public static void RefreshScreenReader_refsr(string[] args, bool fromChatBox = false)
    {
        MainClass.ScreenReader.InitializeScreenReader();

        string text = Translator.Instance.Translate("commands-other-refresh_screen_reader",
            translationCategory: TranslationCategory.CustomCommands);

        if (fromChatBox) Game1.chatBox.addInfoMessage(text);
        else Log.Info(text);
    }

    public static void RefreshModConfig_refmc(string[] args, bool fromChatBox = false)
    {
        MainClass.Config = MainClass.ModHelper!.ReadConfig<ModConfig>();

        string text = Translator.Instance.Translate("commands-other-refresh_mod_config",
            translationCategory: TranslationCategory.CustomCommands);

        if (fromChatBox) Game1.chatBox.addInfoMessage(text);
        else Log.Info(text);
    }

    public static void HnsPercentage_hnspercent(string[] args, bool fromChatBox = false)
    {
        MainClass.Config.HealthNStaminaInPercentage = !MainClass.Config.HealthNStaminaInPercentage;
        MainClass.ModHelper!.WriteConfig(MainClass.Config);

        string text = Translator.Instance.Translate("commands-other-hns_percentage_toggle",
            new { is_enabled = MainClass.Config.HealthNStaminaInPercentage ? 1 : 0 },
            translationCategory: TranslationCategory.CustomCommands);

        if (fromChatBox) Game1.chatBox.addInfoMessage(text);
        else Log.Info(text);
    }

    public static void SnapMouse(string[] args, bool fromChatBox = false)
    {
        MainClass.Config.SnapMouse = !MainClass.Config.SnapMouse;
        MainClass.ModHelper!.WriteConfig(MainClass.Config);

        string text = Translator.Instance.Translate("commands-other-snap_mouse_toggle",
            new { is_enabled = MainClass.Config.SnapMouse ? 1 : 0 },
            translationCategory: TranslationCategory.CustomCommands);

        if (fromChatBox) Game1.chatBox.addInfoMessage(text);
        else Log.Info(text);
    }

    public static void Warning(string[] args, bool fromChatBox = false)
    {
        MainClass.Config.Warning = !MainClass.Config.Warning;
        MainClass.ModHelper!.WriteConfig(MainClass.Config);

        string text = Translator.Instance.Translate("commands-other-warnings_toggle",
            new { is_enabled = MainClass.Config.Warning ? 1 : 0 },
            translationCategory: TranslationCategory.CustomCommands);

        if (fromChatBox) Game1.chatBox.addInfoMessage(text);
        else Log.Info(text);
    }

    public static void Tts(string[] args, bool fromChatBox = false)
    {
        MainClass.Config.TTS = !MainClass.Config.TTS;
        MainClass.ModHelper!.WriteConfig(MainClass.Config);

        string text = Translator.Instance.Translate("commands-other-tts_toggle",
            new { is_enabled = MainClass.Config.TTS ? 1 : 0 },
            translationCategory: TranslationCategory.CustomCommands);

        if (fromChatBox) Game1.chatBox.addInfoMessage(text);
        else Log.Info(text);
    }

    public static void RepeatLastText_rlt(string[] args, bool fromChatBox = false)
    {
        if (int.TryParse(args[0], out int index))
        {
#if DEBUG
            Log.Verbose($"OtherCommands->RepeatLastText: Repeating the {index}th from last");
#endif
            MainClass.ScreenReader.Say(MainClass.ScreenReader.SpokenBuffer[^index], true, excludeFromBuffer: true);
        }
        else
        {
            string text = "Unable to parse the index provided.";
            if (fromChatBox) Game1.chatBox.addInfoMessage(text);
            else Log.Info(text);
        }
    }
}
