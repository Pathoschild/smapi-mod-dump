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
using EnaiumToolKit.Framework.Screen.Elements;
using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using Button = EnaiumToolKit.Framework.Screen.Components.Button;

namespace EnaiumToolKit.Framework.Screen;

public class ScreenGui : GuiScreen
{
    private readonly List<Element> _elements = new();
    private List<Element> _searchElements;
    private int _index;
    private int _maxElement;
    private TextField _searchTextField;

    private string? Title { get; }

    protected override void Init()
    {
        _index = 0;
        _maxElement = 7;
        width = 832;
        height = 578;
        var centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
        xPositionOnScreen = (int)centeringOnScreen.X;
        yPositionOnScreen = (int)centeringOnScreen.Y + 32;
        const int buttonSize = 60;
        AddComponent(new Button("U", GetTranslation("screenGui.component.textField.flipUp"),
            xPositionOnScreen + width + buttonSize, yPositionOnScreen, buttonSize,
            buttonSize)
        {
            OnLeftClicked = () =>
            {
                if (_index >= _maxElement)
                {
                    _index -= _maxElement;
                }
                else
                {
                    _index = 0;
                }
            }
        });
        AddComponent(new Button("D", GetTranslation("screenGui.component.textField.flipDown"),
            xPositionOnScreen + width + buttonSize,
            yPositionOnScreen + height - buttonSize,
            buttonSize, buttonSize)
        {
            OnLeftClicked = () =>
            {
                if (_index + (_searchElements.Count >= _maxElement ? _maxElement : _searchElements.Count) <
                    _searchElements.Count)
                {
                    if (_index + _maxElement <= _searchElements.Count - _maxElement)
                    {
                        _index += _maxElement;
                    }
                    else
                    {
                        _index += _searchElements.Count - _index - _maxElement;
                    }
                }
            }
        });

        if (!(Game1.activeClickableMenu is TitleMenu))
        {
            AddComponent(new Button("C", GetTranslation("screenGui.component.textField.closeScreen"),
                Game1.viewport.Width - buttonSize, 0, buttonSize, buttonSize)
            {
                OnLeftClicked = () => { Game1.activeClickableMenu = null; }
            });
        }

        _searchTextField = new TextField("", GetTranslation("screenGui.component.textField.Search"),
            xPositionOnScreen,
            yPositionOnScreen - 100, width, 50);
        AddComponent(_searchTextField);

        base.Init();
    }

    protected ScreenGui()
    {
    }

    protected ScreenGui(string title)
    {
        Title = title;
    }


    private string GetTranslation(string key)
    {
        return ModEntry.GetInstance().Helper.Translation.Get(key);
    }

    public override void draw(SpriteBatch b)
    {
        Render2DUtils.DrawBound(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);
        var y = yPositionOnScreen + 20;
        _searchElements = new List<Element>();
        _searchElements.AddRange(GetSearchElements());
        var i = 0;
        foreach (var element in GetElements())
        {
            if (element.Visibled)
            {
                element.Render(b, xPositionOnScreen + 15, y + i * 78);
                if (element.Hovered && !element.Description.Equals(""))
                {
                    var descriptionWidth = FontUtils.GetWidth(element.Description) + 50;
                    var descriptionHeight = FontUtils.GetHeight(element.Description) + 50;

                    drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), 0, 0,
                        descriptionWidth,
                        descriptionHeight, Color.Wheat, 4f, false);
                    FontUtils.DrawHvCentered(b, element.Description, descriptionWidth / 2,
                        descriptionHeight / 2);
                }
            }
            i++;
        }

        if (Title != null)
        {
            SpriteText.drawStringWithScrollCenteredAt(b, Title, Game1.viewport.Width / 2,
                Game1.viewport.Height - 100, Title);
        }

        const string text = "EnaiumToolKit By Enaium";
        FontUtils.Draw(b, text, 0, Game1.viewport.Height - FontUtils.GetHeight(text));

        drawMouse(b);
        base.draw(b);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        foreach (var variable in GetElements().Where(variable =>
                     variable is { Visibled: true, Enabled: true, Hovered: true }))
        {
            variable.MouseLeftClicked(x, y);
            Game1.playSound("drumkit6");
        }

        base.receiveLeftClick(x, y, playSound);
    }

    public override void releaseLeftClick(int x, int y)
    {
        foreach (var variable in GetElements().Where(variable =>
                     variable is { Visibled: true, Enabled: true, Hovered: true }))
        {
            variable.MouseLeftReleased(x, y);
        }

        base.releaseLeftClick(x, y);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        foreach (var variable in GetElements().Where(variable =>
                     variable is { Visibled: true, Enabled: true, Hovered: true }))
        {
            variable.MouseRightClicked(x, y);
        }

        base.receiveRightClick(x, y);
    }

    public override void receiveScrollWheelAction(int direction)
    {
        if (direction > 0 && _index > 0)
        {
            _index--;
        }
        else if (direction < 0 &&
                 _index + (_searchElements.Count >= _maxElement ? _maxElement : _searchElements.Count) <
                 _searchElements.Count)
        {
            _index++;
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
    
    private IEnumerable<Element> GetElements()
    {
        List<Element> elements = new List<Element>();
        for (int i = _index, j = 0;
             j < (_searchElements.Count >= _maxElement ? _maxElement : _searchElements.Count);
             i++, j++)
        {
            elements.Add(_elements[i]);
        }

        return elements;
    }

    private IEnumerable<Element> GetSearchElements()
    {
        IEnumerable<Element> elements = _elements;
        if (!_searchTextField.Text.Equals(""))
        {
            elements = elements.Where(element =>
                element.Title.IndexOf(_searchTextField.Text, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        return elements;
    }

    protected void AddElement(Element element)
    {
        _elements.Add(element);
    }

    protected void AddElementRange(params Element[] element)
    {
        _elements.AddRange(element);
    }

    protected void RemoveElement(Element element)
    {
        _elements.Remove(element);
    }

    protected void RemoveElementRange(params Element[] element)
    {
        foreach (var variable in element)
        {
            _elements.Remove(variable);
        }
    }
}