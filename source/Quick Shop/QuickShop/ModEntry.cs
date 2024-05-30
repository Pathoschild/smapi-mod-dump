/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/QuickShop
**
*************************************************/

using QuickShop.Framework;
using QuickShop.Framework.Gui;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace QuickShop;

public class ModEntry : Mod
{
    public Config Config;
    private static ModEntry _instance;

    public ModEntry()
    {
        _instance = this;
    }

    public override void Entry(IModHelper helper)
    {
        Config = helper.ReadConfig<Config>();
        helper.Events.Input.ButtonsChanged += ButtonsChanged;
    }

    private void ButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;
        if (!Context.IsPlayerFree)
            return;
        if (!Config.OpenQuickShop.JustPressed())
            return;
        Game1.activeClickableMenu = new QuickShopScreen();
    }

    public static ModEntry GetInstance()
    {
        return _instance;
    }

    public string GetButtonTranslation(string key)
    {
        return _instance.Helper.Translation.Get("quickShop.button." + key);
    }
    
    public string GetLabelTranslation(string key)
    {
        return _instance.Helper.Translation.Get("quickShop.label." + key);
    }
    
    public string GetSettingTranslation(string key)
    {
        return _instance.Helper.Translation.Get("quickShop.screen.setting." + key);
    }

    public string GetTranslation(string key)
    {
        return _instance.Helper.Translation.Get(key);
    }
}