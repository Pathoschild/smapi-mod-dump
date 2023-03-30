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
    public class AlignementStyle : Style
    {
        public Func<IComponent, int> AllignX { get; set; } = (c) => 0;

        public Func<IComponent, int> AllignY { get; set; } = (c) => 0;

        public AlignementStyle(IPlatoUIHelper helper, string option = "")
            : base(helper, option)
        {
        }

        public override IStyle New(IPlatoUIHelper helper, string option = "")
        {
            return new AlignementStyle(helper, option);
        }

        public override string[] PropertyNames => new string[] { "Align", "AlignX","AlignY"};


        public override void Apply(IComponent component)
        {
            component.Bounds = new Rectangle(component.Bounds.X + AllignX(component), component.Bounds.Y + AllignY(component), component.Bounds.Width, component.Bounds.Height);
            base.Apply(component);
        }

        public override void Parse(string property, string value, IComponent component)
        {
            switch (property.ToLower())
            {
                case "align":
                    {
                        string[] parts = value.Split(' ');
                        string aX = parts[0].ToLower();
                        string aY = parts.Length > 1 ? parts[1].ToLower() : aX;

                        if (aX == "right")
                            AllignX = (c) => c.Parent.Bounds.Width - c.Bounds.Width;
                        else if (aX == "center")
                            AllignX = (c) => (c.Parent.Bounds.Width - c.Bounds.Width) / 2;
                        
                        if (aY == "bottom")
                            AllignY = (c) => c.Parent.Bounds.Height - c.Bounds.Height;
                        else if (aY == "center")
                            AllignY = (c) => (c.Parent.Bounds.Height - c.Bounds.Height) / 2;

                        break;
                    }
                case "alignx":
                    {
                        string aX = value.ToLower().Trim();
                        if (aX == "right")
                            AllignX = (c) => c.Parent.Bounds.Width - c.Bounds.Width;
                        else if (aX == "center")
                            AllignX = (c) => (c.Parent.Bounds.Width - c.Bounds.Width) / 2;
                        break;
                    }
                case "aligny":
                    {
                        string aY = value.ToLower().Trim();
                        if (aY == "bottom")
                            AllignY = (c) => c.Parent.Bounds.Height - c.Bounds.Height;
                        else if (aY == "center")
                            AllignY = (c) => (c.Parent.Bounds.Height - c.Bounds.Height) / 2;
                        break;
                    }
            }
        }
    }
}
