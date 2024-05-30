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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace EnaiumToolKit.Framework.Screen.Elements;

public class ComboBox<T> : Element
{
    private bool _hovered;
    public bool Expanded;
    public List<T> Options = new();
    public T? Current;
    private T? _hoveredOption;
    public Action<T>? OnCurrentChanged;
    
    private readonly string? _description;

    public ComboBox(string title, string? description = null) : base(title, description)
    {
        _description = description;
    }

    public override void Render(SpriteBatch b, int x, int y)
    {
        Description = _description;

        Current ??= Options.FirstOrDefault();

        var textureWidth = Width / 2;
        var textureHeight = (int)Game1.dialogueFont.MeasureString("Test").Y;
        var textureX = x + Width / 2;
        var textureY = y + Height / 2 - textureHeight / 2;
        _hovered = new Rectangle(textureX, textureY, textureWidth, textureHeight).Contains(Game1.getMouseX(),
            Game1.getMouseY());
        b.DrawStringVCenter(Title!, x, y, Height);
        b.DrawComboBoxBackgroundTexture(textureX, textureY, textureWidth, textureHeight);
        b.Draw(Game1.mouseCursors,
            new Vector2(textureX + textureWidth - OptionsDropDown.dropDownButtonSource.Width * 4,
                textureY + textureHeight / 2 - OptionsDropDown.dropDownButtonSource.Height * 4 / 2),
            OptionsDropDown.dropDownButtonSource, Color.Wheat, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.981f);
        var render = Current?.ToString();
        if (!string.IsNullOrWhiteSpace(render))
        {
            if (_hovered)
            {
                Description = render;
            }

            b.DrawStringVCenter(Game1.dialogueFont.GetEllipsisString(render, textureWidth), textureX + 10, textureY,
                textureHeight);
        }

        var optionsHeight = textureHeight * (Options.Count - 1);
        var optionsBounds = new Rectangle(textureX, textureY + textureHeight, textureWidth, optionsHeight);

        Hovered = (Expanded
            ? optionsBounds
            : new Rectangle(x, y, Width, Height)).Contains(Game1.getMouseX(), Game1.getMouseY());

        if (Expanded)
        {
            b.DrawComboBoxBackgroundTexture(optionsBounds);
            foreach (var (option, index) in Options.Where(option => !option.Equals(Current)).WithIndex())
            {
                var optionRender = option.ToString();
                var optionY = textureY + textureHeight + textureHeight * index;
                var optionBounds = new Rectangle(textureX, optionY, textureWidth, textureHeight);
                if (optionBounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
                {
                    b.Draw(Game1.staminaRect, optionBounds, new Rectangle(0, 0, 1, 1), Color.Wheat, 0.0f, Vector2.Zero,
                        SpriteEffects.None, 0.975f);
                    Description = optionRender;
                    _hoveredOption = option;
                }

                b.DrawStringVCenter(Game1.dialogueFont.GetEllipsisString(optionRender, textureWidth), textureX + 10,
                    optionY,
                    textureHeight);
            }
        }
    }

    public override void MouseLeftClicked(int x, int y)
    {
        if (_hovered)
        {
            Expanded = !Expanded;

            Game1.playSound("shwip");
        }

        base.MouseLeftClicked(x, y);
    }

    public override void MouseLeftReleased(int x, int y)
    {
        if (Expanded)
        {
            Game1.playSound("drumkit6");
        }

        if (_hoveredOption != null)
        {
            Current = _hoveredOption;
            OnCurrentChanged?.Invoke(Current);
        }

        Expanded = false;
        base.MouseLeftReleased(x, y);
    }

    public override void LostFocus(int x, int y)
    {
        Expanded = false;
        base.LostFocus(x, y);
    }
}