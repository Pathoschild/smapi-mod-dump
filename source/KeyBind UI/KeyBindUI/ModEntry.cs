/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/KeyBindUI
**
*************************************************/

using EnaiumToolKit.Framework.Screen.Components;
using KeyBindUI.Framework.Gui;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace KeyBindUI;

public class ModEntry : Mod
{
    private static ModEntry _instance;

    public ModEntry()
    {
        _instance = this;
    }

    private readonly Button _keyBindUi = new("KeyBindUI", "", 20, 0, 200, 80);

    public override void Entry(IModHelper helper)
    {
        helper.Events.Display.Rendered += (sender, args) =>
        {
            if (Game1.activeClickableMenu is GameMenu)
            {
                _keyBindUi.Render(args.SpriteBatch);
            }
        };

        helper.Events.Input.ButtonPressed += (sender, args) =>
        {
            if (args.Button != SButton.MouseLeft || !_keyBindUi.Hovered) return;
            _keyBindUi.Hovered = false;
            Game1.activeClickableMenu = new KeyBindScreen();
            Game1.playSound("drumkit6");
        };
    }

    public static ModEntry GetInstance()
    {
        return _instance;
    }
}