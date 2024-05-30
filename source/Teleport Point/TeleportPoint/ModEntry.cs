/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/TeleportPoint
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using TeleportPoint.Framework;
using TeleportPoint.Framework.Gui;

namespace TeleportPoint;

public class ModEntry : Mod
{
    public static Config Config;
    private static ModEntry _instance;

    public ModEntry()
    {
        _instance = this;
    }

    public override void Entry(IModHelper helper)
    {
        Config = helper.ReadConfig<Config>();
        helper.Events.Input.ButtonsChanged += OnButtonsChanged;
    }

    public static void ConfigReload()
    {
        GetInstance().Helper.WriteConfig(Config);
        GetInstance().Helper.ReadConfig<Config>();
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;
        if (!Context.IsPlayerFree)
            return;
        if (!Config.OpenTeleport.JustPressed())
            return;
        Game1.activeClickableMenu = new TeleportPointScreen();
    }

    public static string GetTranslation(string key)
    {
        return GetInstance().Helper.Translation.Get(key);
    }

    public static ModEntry GetInstance()
    {
        return _instance;
    }
}