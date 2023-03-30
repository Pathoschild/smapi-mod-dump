/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using BmFont;
using Microsoft.Xna.Framework.Graphics;
using PlatoUI.UI.Components;
using PlatoUI.UI.Styles;
using StardewValley.Menus;
using System.Collections.Generic;

namespace PlatoUI.UI
{
    public interface IUIHelper
    {
        void RegisterStyle(IStyle style);

        void RegisterComponent(IComponent component);

        IWrapper LoadFromFile(string layoutPath, string id = "");

        bool TryGetStyle(string propertyName, out IStyle style, string option = "");

        bool TryGetComponent(string componentName, out IComponent component);

        IClickableMenu OpenMenu(IWrapper wrapper);

        List<Texture2D> LoadFontPages(FontFile fontFile, string assetName);

        FontFile LoadFontFile(string assetName);

        Dictionary<char, FontChar> ParseCharacterMap(FontFile fontFile);

        SpriteFont LoadSpriteFont(string assetName);
    }
}
