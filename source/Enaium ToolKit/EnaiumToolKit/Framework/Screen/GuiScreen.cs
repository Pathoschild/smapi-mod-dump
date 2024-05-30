/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Extensions;
using EnaiumToolKit.Framework.Screen.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace EnaiumToolKit.Framework.Screen;

public class GuiScreen : IClickableMenu
{
    private readonly List<Component> _components = new();
    protected IClickableMenu? PreviousMenu;

    protected GuiScreen()
    {
        _components.Clear();
        Initialization();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        Initialization();
        base.gameWindowSizeChanged(oldBounds, newBounds);
    }

    private bool _isFirstUpdate;

    public override void update(GameTime time)
    {
        if (!_isFirstUpdate)
        {
            _isFirstUpdate = true;
            Initialization();
        }

        base.update(time);
    }

    private void Initialization()
    {
        _components.Clear();
        Init();
    }

    protected virtual void Init()
    {
    }

    public override void draw(SpriteBatch b)
    {
        foreach (var component in _components.Where(component => component.Visibled))
        {
            component.Render(b);
        }

        foreach (var component in _components.Where(component => component.Visibled))
        {
            if (component is { Hovered: true, Description: not null } && !component.Description.Equals(""))
            {
                DrawTooltip(b, component.Description);
            }
        }

        const string text = "EnaiumToolKit By Enaium";
        b.DrawString(text, 0, Game1.graphics.GraphicsDevice.Viewport.Height - b.GetStringHeight(text));

        drawMouse(b);
        base.draw(b);
    }

    protected void DrawTooltip(SpriteBatch b, string text)
    {
        var descriptionWidth = b.GetStringWidth(text) + 50;
        var descriptionHeight = b.GetStringHeight(text) + 50;

        var mouseX = Game1.getMouseX() + 40;
        var mouseY = Game1.getMouseY() + 40;
        var description = text!;
        description = Game1.parseText(description, Game1.dialogueFont, width);
        descriptionWidth = description.Split('\n').Max(s => b.GetStringWidth(s) + 50);
        descriptionHeight += description.Count(c => c == '\n') * b.GetStringHeight(text);
        var offScreen = mouseX + descriptionWidth > Game1.graphics.GraphicsDevice.Viewport.Width &&
                        Game1.getMouseX() - descriptionWidth > 0;
        if (offScreen)
        {
            mouseX = Game1.getMouseX() - descriptionWidth;
        }

        b.DrawWindowTexture(mouseX, mouseY, descriptionWidth, descriptionHeight);
        b.DrawStringCenter(description, mouseX, mouseY, descriptionWidth,
            descriptionHeight);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        foreach (var component in _components.Where(component =>
                     component is { Visibled: true, Enabled: true }))
        {
            if (component is { Hovered: true })
            {
                component.MouseLeftClicked(x, y);
            }
            else
            {
                component.LostFocus(x, y);
            }
        }

        base.receiveLeftClick(x, y, playSound);
    }

    public override void releaseLeftClick(int x, int y)
    {
        foreach (var component in _components.Where(component =>
                     component is { Visibled: true, Enabled: true }))
        {
            if (component is { Hovered: true })
            {
                component.MouseLeftReleased(x, y);
            }
            else
            {
                component.LostFocus(x, y);
            }
        }

        base.releaseLeftClick(x, y);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        foreach (var component in _components.Where(component =>
                     component is { Visibled: true, Enabled: true }))
        {
            if (component is { Hovered: true })
            {
                component.MouseRightClicked(x, y);
            }
            else
            {
                component.LostFocus(x, y);
            }
        }

        base.receiveRightClick(x, y, playSound);
    }

    public override void receiveScrollWheelAction(int direction)
    {
        foreach (var component in _components.Where(component =>
                     component is { Visibled: true, Enabled: true, Hovered: true }))
        {
            component.MouseScrollWheelAction(direction);
        }

        base.receiveScrollWheelAction(direction);
    }

    public override void receiveKeyPress(Keys key)
    {
        if (Game1.options.menuButton[0].key == key)
        {
            return;
        }

        base.receiveKeyPress(key);
    }

    protected void AddComponent(Component component)
    {
        _components.Add(component);
    }

    protected void AddComponentRange(params Component[] component)
    {
        _components.AddRange(component);
    }

    protected void RemoveComponent(Component component)
    {
        _components.Remove(component);
    }

    protected void RemoveComponentRange(params Component[] component)
    {
        foreach (var variable in component)
        {
            _components.Remove(variable);
        }
    }

    protected void OpenScreenGui(IClickableMenu clickableMenu)
    {
        if (clickableMenu is GuiScreen { PreviousMenu: null } guiScreen)
        {
            guiScreen.PreviousMenu = this;
        }

        if (Game1.activeClickableMenu is TitleMenu)
        {
            TitleMenu.subMenu = clickableMenu;
        }
        else
        {
            Game1.activeClickableMenu = clickableMenu;
        }
    }
}