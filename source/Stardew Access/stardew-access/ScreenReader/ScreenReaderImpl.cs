/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using CrossSpeak;
using stardew_access.Translation;

namespace stardew_access.ScreenReader;

public class ScreenReaderImpl : IScreenReader
{
    public string prevText = "", prevTextTile = "", prevChatText = "", prevMenuText = "";
    private string menuPrefixText = "";
    private string prevMenuPrefixText = "";
    private string menuSuffixText = "";
    private string prevMenuSuffixText = "";
    private string menuPrefixNoQueryText = "";
    private string menuSuffixNoQueryText = "";

    public string PrevTextTile
    {
        get => prevTextTile;
        set => prevTextTile = value;
    }

    public string PrevMenuQueryText
    {
        get => prevMenuText;
        set => prevMenuText = value;
    }

    public string MenuPrefixText
    {
        get => menuPrefixText;
        set => menuPrefixText = value;
    }

    public string MenuSuffixText
    {
        get => menuSuffixText;
        set => menuSuffixText = value;
    }

    public string MenuPrefixNoQueryText
    {
        get => menuPrefixNoQueryText;
        set => menuPrefixNoQueryText = value;
    }

    public string MenuSuffixNoQueryText
    {
        get => menuSuffixNoQueryText;
        set => menuSuffixNoQueryText = value;
    }

    public void InitializeScreenReader()
    {
        Log.Info("Initializing CrossSpeak...");
        CrossSpeakManager.Instance.TrySAPI(true);
        if (!CrossSpeakManager.Instance.Initialize())
        {
            Log.Error("None of the supported screen readers is running");
            return;
        }

        string name = CrossSpeakManager.Instance.DetectScreenReader();
        if (name != null)
        {
            Log.Info($"The active screen reader driver is: {name}");
        }
        else
        {
            Log.Info("Screen reader successfully initialized.");
        }
    }

    public void CloseScreenReader() => CrossSpeakManager.Instance.Close();

    public bool Say(string text, bool interrupt)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        if (!MainClass.Config.TTS) return false;
        if (text.Contains('^')) text = text.Replace('^', '\n');

        if (!CrossSpeakManager.Instance.Speak(text, interrupt))
        {
            Log.Error($"Failed to output text: {text}");
            return false;
        }

#if DEBUG
        Log.Verbose($"Speaking(interrupt: {interrupt}) = {text}");
#endif
        return true;
    }

    public bool TranslateAndSay(string translationKey, bool interrupt, object? translationTokens = null, TranslationCategory translationCategory = TranslationCategory.Default, bool disableTranslationWarnings = false)
    {
        if (string.IsNullOrWhiteSpace(translationKey))
            return false;

        return Say(Translator.Instance.Translate(translationKey, translationTokens, translationCategory, disableTranslationWarnings), interrupt);
    }

    public bool SayWithChecker(string text, bool interrupt, string? customQuery = null)
    {
        customQuery ??= text;

        if (string.IsNullOrWhiteSpace(customQuery))
            return false;

        if (prevText == customQuery)
            return false;

        prevText = customQuery;
        return Say(text, interrupt);
    }

    public bool TranslateAndSayWithChecker(string translationKey, bool interrupt, object? translationTokens = null, TranslationCategory translationCategory = TranslationCategory.Default, string? customQuery = null, bool disableTranslationWarnings = false)
    {
        if (string.IsNullOrWhiteSpace(translationKey))
            return false;

        return SayWithChecker(Translator.Instance.Translate(translationKey, translationTokens, translationCategory, disableTranslationWarnings), interrupt, customQuery);
    }

    public bool SayWithMenuChecker(string text, bool interrupt, string? customQuery = null)
    {
        customQuery ??= text;

        if (string.IsNullOrWhiteSpace(customQuery))
            return false;

        if (prevMenuText == customQuery && prevMenuSuffixText == MenuSuffixText && prevMenuPrefixText == MenuPrefixText)
            return false;

        prevMenuText = customQuery;
        prevMenuSuffixText = MenuSuffixText;
        prevMenuPrefixText = MenuPrefixText;
        bool re = Say($"{MenuPrefixNoQueryText}{MenuPrefixText}{text}{MenuSuffixText}{MenuSuffixNoQueryText}", interrupt);
        MenuPrefixNoQueryText = "";
        MenuSuffixNoQueryText = "";

        return re;
    }

    public bool TranslateAndSayWithMenuChecker(string translationKey, bool interrupt, object? translationTokens = null, TranslationCategory translationCategory = TranslationCategory.Menu, string? customQuery = null, bool disableTranslationWarnings = false)
    {
        if (string.IsNullOrWhiteSpace(translationKey))
            return false;

        return SayWithMenuChecker(Translator.Instance.Translate(translationKey, translationTokens, translationCategory, disableTranslationWarnings), interrupt, customQuery);
    }

    public bool SayWithChatChecker(string text, bool interrupt)
    {
        if (prevChatText == text)
            return false;

        prevChatText = text;
        return Say(text, interrupt);
    }

    public bool SayWithTileQuery(string text, int x, int y, bool interrupt)
    {
        string query = $"{text} x:{x} y:{y}";

        if (prevTextTile == query)
            return false;

        prevTextTile = query;
        return Say(text, interrupt);
    }

    public void Cleanup()
    {
        MainClass.ScreenReader.PrevMenuQueryText = "";
        MainClass.ScreenReader.MenuPrefixText = "";
        MainClass.ScreenReader.MenuSuffixText = "";
    }
}
