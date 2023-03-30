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
using PlatoUI.UI.Components;
using System;
using System.Collections.Generic;

namespace PlatoUI.UI.Styles
{
    public interface IStyle : IDisposable
    {
        int Priority { get; set; }
        string Option { get; }
        string[] PropertyNames { get; }

        void Parse(string property, string value, IComponent component);
        bool ShouldApply(IComponent component);
        void Apply(IComponent component);
        void Update(IComponent component);

        IStyle New(IPlatoUIHelper helper, string option = "");
    }

    public interface IDrawStyle : IStyle
    {
        IEnumerable<Color> Render(IComponent component, IComponent parent, IEnumerable<Color> render);
    }
}
