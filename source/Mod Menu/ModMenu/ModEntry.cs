/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/ModMenu
**
*************************************************/

using EnaiumToolKit.Framework.Screen.Components;
using ModMenu.Framework.Screen;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ModMenu;

public class ModEntry : Mod
{
    private static ModEntry _instance;

    public ModEntry()
    {
        _instance = this;
    }

    private Button _modMenuButton = new Button("", "", 20, 80, 200, 80)
    {
        OnLeftClicked = () =>
        {
            if (Game1.activeClickableMenu is TitleMenu)
            {
                TitleMenu.subMenu = new ModMenuScreen();
            }
            else
            {
                Game1.activeClickableMenu = new ModMenuScreen();
            }

            Game1.playSound("drumkit6");
        }
    };

    public override void Entry(IModHelper helper)
    {
        helper.Events.Display.Rendered += OnRendered;
        helper.Events.Input.ButtonPressed += OnButton;
    }

    private void OnRendered(object? sender, RenderedEventArgs args)
    {
        if (Game1.activeClickableMenu is TitleMenu titleMenu && TitleMenu.subMenu == null &&
            !GetBool(titleMenu, "isTransitioningButtons") &&
            GetBool(titleMenu, "titleInPosition") &&
            !GetBool(titleMenu, "transitioningCharacterCreationMenu") || Game1.activeClickableMenu is GameMenu)
        {
            _modMenuButton.Title = Helper.Translation.Get("modMenu.button.modMenu");
            _modMenuButton.Render(args.SpriteBatch);
        }
    }

    private void OnButton(object? sender, ButtonPressedEventArgs args)
    {
        if (args.Button != SButton.MouseLeft || !_modMenuButton.Hovered) return;
        _modMenuButton.Hovered = false;
        _modMenuButton.MouseLeftClicked(Game1.getMouseX(), Game1.getMouseY());
    }

    private bool GetBool(object obj, string name)
    {
        return Helper.Reflection.GetField<bool>(obj, name).GetValue();
    }

    public static ModEntry GetInstance()
    {
        return _instance;
    }
}