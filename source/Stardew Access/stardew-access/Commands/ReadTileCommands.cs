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

public class ReadTileCommands
{
    public static void Flooring(string[] args, bool fromChatBox = false)
    {
        MainClass.Config.ReadFlooring = !MainClass.Config.ReadFlooring;
        MainClass.ModHelper!.WriteConfig(MainClass.Config);

        string text = Translator.Instance.Translate("commands-read_tile-flooring_toggle",
            new { is_enabled = MainClass.Config.ReadFlooring ? 1 : 0 }, TranslationCategory.CustomCommands);

        if (fromChatBox) Game1.chatBox.addInfoMessage(text);
        else Log.Info(text);
    }

    public static void Watered(string[] args, bool fromChatBox = false)
    {
        MainClass.Config.WateredToggle = !MainClass.Config.WateredToggle;
        MainClass.ModHelper!.WriteConfig(MainClass.Config);

        string text = Translator.Instance.Translate("commands-read_tile-watered_toggle",
            new { is_enabled = MainClass.Config.WateredToggle ? 1 : 0 }, TranslationCategory.CustomCommands);

        if (fromChatBox) Game1.chatBox.addInfoMessage(text);
        else Log.Info(text);
    }

    public static void ReadTile(string[] args, bool fromChatBox = false)
    {
        MainClass.Config.ReadTile = !MainClass.Config.ReadTile;
        MainClass.ModHelper!.WriteConfig(MainClass.Config);

        string text = Translator.Instance.Translate("commands-read_tile-read_tile_toggle",
            new { is_enabled = MainClass.Config.ReadTile ? 1 : 0 }, TranslationCategory.CustomCommands);

        if (fromChatBox) Game1.chatBox.addInfoMessage(text);
        else Log.Info(text);
    }
}
