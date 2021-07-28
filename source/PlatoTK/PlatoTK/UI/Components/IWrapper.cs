/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PlatoTK.UI.Components
{
    public interface IWrapper : IComponent
    {
        bool TryGet<T>(string key, out T value);
        bool TryCall<T>(string key, out T value);
        bool TryCall(string key, IComponent component);
        bool TryCall<T>(string key, out T value, IComponent component);

        void Set(string key, object value);

        bool WasLeftMouseDown { get;}
        bool IsLeftMouseDown { get;}

        bool WasRightMouseDown { get; }
        bool IsRightMouseDown { get;}

        Point LastMousePosition { get;}
        Point CurrentMousePosition { get;}

        int LastMouseWheelState { get; }
        int CurrentMouseWheelState { get; }

        IEnumerable<StyleDefinition> GetStyleDefinitions(string[] tags);

        bool TryLoadTexture(string value, IComponent component, out Texture2D texture);

        bool TryLoadText(string value, IComponent component, out string text);

        bool TryParseIntValue(string value, int relative, IComponent component, out int intValue);
    }
}
