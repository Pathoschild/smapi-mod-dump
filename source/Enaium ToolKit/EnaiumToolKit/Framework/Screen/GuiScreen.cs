/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Screen.Components;
using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace EnaiumToolKit.Framework.Screen;

public class GuiScreen : IClickableMenu
{
    private readonly List<Component> _components = new();

    protected GuiScreen()
    {
        _components.Clear();
        Initialization();
        ModEntry.GetInstance().Helper.Events.Display.WindowResized += (_, _) => { Initialization(); };
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
            if (component.Hovered && !component.Description.Equals(""))
            {
                var descriptionWidth = FontUtils.GetWidth(component.Description) + 50;
                var descriptionHeight = FontUtils.GetHeight(component.Description) + 50;

                drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), 0, 0, descriptionWidth,
                    descriptionHeight, Color.Wheat, 4f, false);
                FontUtils.DrawHvCentered(b, component.Description, 0, 0, descriptionWidth, descriptionHeight);
            }
        }

        const string text = "EnaiumToolKit By Enaium";
        FontUtils.Draw(b, text, 0, Game1.viewport.Height - FontUtils.GetHeight(text));

        drawMouse(b);
        base.draw(b);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        foreach (var component in _components.Where(component =>
                     component is { Visibled: true, Enabled: true, Hovered: true }))
        {
            component.MouseLeftClicked(x, y);
            Game1.playSound("drumkit6");
        }

        base.receiveLeftClick(x, y, playSound);
    }

    public override void releaseLeftClick(int x, int y)
    {
        foreach (var component in _components.Where(component =>
                     component is { Visibled: true, Enabled: true, Hovered: true }))
        {
            component.MouseLeftReleased(x, y);
        }

        base.releaseLeftClick(x, y);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        foreach (var component in _components.Where(component =>
                     component is { Visibled: true, Enabled: true, Hovered: true }))
        {
            component.MouseRightClicked(x, y);
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