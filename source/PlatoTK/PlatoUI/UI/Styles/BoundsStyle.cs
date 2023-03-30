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

namespace PlatoUI.UI.Styles
{
    public class BoundsStyle : Style
    {
        public Func<IComponent,int> Width { get; set; } = (c) => c.Bounds.Width;

        public Func<IComponent, int> Height { get; set; } = (c) => c.Bounds.Height;

        public Func<IComponent, int> X { get; set; } = (c) => c.Bounds.X;

        public Func<IComponent, int> Y { get; set; } = (c) => c.Bounds.Y;

        public BoundsStyle(IPlatoUIHelper helper, string option = "")
            : base(helper, option)
        {
            Priority = -int.MaxValue;
        }

        public override IStyle New(IPlatoUIHelper helper, string option = "")
        {
            return new BoundsStyle(helper, option);
        }

        public override string[] PropertyNames => new string[] { "X", "Y", "Width", "Height" };


        public override void Apply(IComponent component)
        {
            component.Bounds = new Rectangle(X(component), Y(component), Width(component), Height(component));
            base.Apply(component);
        }

        public override void Parse(string property, string value, IComponent component)
        {
            switch (property.ToLower())
            {
                case "width": Width = (c) => component.GetWrapper().TryParseIntValue(value, c.Parent.Bounds.Width, c, out int width) ? width : Width(c); break;
                case "height": Height = (c) => component.GetWrapper().TryParseIntValue(value, component.Parent.Bounds.Height, c, out int height) ? height : Height(c); break;
                case "x": X = (c) => component.GetWrapper().TryParseIntValue(value, component.Parent.Bounds.Width, c, out int xvalue) ? xvalue : X(c); break;
                case "y": Y = (c) => component.GetWrapper().TryParseIntValue(value, component.Parent.Bounds.Height, c, out int yvalue) ? yvalue : Y(c); break;
            }
        }
    }
}
