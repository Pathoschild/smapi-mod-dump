/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.StardustCore.Networking;

namespace Omegasis.Revitalize.Framework.Illuminate
{
    public class NamedColor : NetObject
    {
        public readonly NetString name = new NetString();
        public readonly NetColor color = new NetColor();
        /// <summary>
        /// The color blending mode for this color manager.
        /// </summary>
        public readonly NetEnum<Enums.DyeBlendMode> colorMixMode = new NetEnum<Enums.DyeBlendMode>();

        /// <summary>
        /// The amount of influence the first color has on the mix for the object.
        /// </summary>
        public readonly NetDouble blendInfluence = new NetDouble();

        public NamedColor()
        {
            this.initializeNetFields();
        }

        public NamedColor(string Name, Color Color, Enums.DyeBlendMode ColorMixMode = Enums.DyeBlendMode.Average, double BlendInfluence = 0d)
        {
            this.name.Value = Name;
            this.color.Value = Color;
            this.colorMixMode.Value = ColorMixMode;
            this.blendInfluence.Value = BlendInfluence;
            this.initializeNetFields();
        }

        public NamedColor(Color Color)
        {
            this.name.Value = "";
            this.color.Value = Color;
            this.colorMixMode.Value = Enums.DyeBlendMode.Average;
            this.initializeNetFields();
        }

        public NamedColor(string Name, int r, int g, int b, int a = 255, bool Invert = false, Enums.DyeBlendMode ColorMixMode = Enums.DyeBlendMode.Average, double BlendInfluence = 0d)
        {
            this.name.Value = Name;
            this.color.Value = new Color(r, g, b, a);
            if (Invert)
                this.color.Value = this.color.Value.Invert();
            this.colorMixMode.Value = ColorMixMode;
            this.blendInfluence.Value = BlendInfluence;
            this.initializeNetFields();
        }

        public virtual void setFields(NamedColor other)
        {
            this.blendInfluence.Value = other.blendInfluence.Value;
            this.color.Value = other.color.Value;
            this.colorMixMode.Value = other.colorMixMode.Value;
            this.name.Value = other.name.Value;
        }

        public virtual void clearFields()
        {
            this.blendInfluence.Value = 0.5f;
            this.color.Value = new Color(0, 0, 0, 0);
            this.colorMixMode.Value = Enums.DyeBlendMode.Blend;
            this.name.Value = "";
        }

        public Color getColor()
        {
            return this.color;
        }

        public Color getInvertedColor()
        {
            return this.color.Value.Invert();
        }

        protected override void initializeNetFields()
        {
            this.NetFields.AddFields(

                this.name,
                this.color,
                this.colorMixMode,
                this.blendInfluence

                );
        }

        public Color getBlendedColor(Color other, int Alpha = 255)
        {


            //Used as reference.
            //https://stackoverflow.com/questions/3722307/is-there-an-easy-way-to-blend-two-system-drawing-color-values
            if (this.colorMixMode == Enums.DyeBlendMode.Blend)
            {
                int r = (int)(this.color.R * this.blendInfluence + other.R * (1 - this.blendInfluence));
                int g = (int)(this.color.G * this.blendInfluence + other.G * (1 - this.blendInfluence));
                int b = (int)(this.color.B * this.blendInfluence + other.B * (1 - this.blendInfluence));
                return new Color(r, g, b, Alpha);
            }
            if (this.colorMixMode == Enums.DyeBlendMode.Average)
                return new Color((this.color.R + other.R) / 2, (this.color.G + other.G) / 2, (this.color.B + other.B) / 2, Alpha);

            if (this.colorMixMode == Enums.DyeBlendMode.Multiplier)
                return new Color(this.color.R * other.R, this.color.G * other.G, this.color.B * other.B, Alpha);

            return this.color;
        }

        public NamedColor getCopy()
        {
            return new NamedColor(this.name, this.color, this.colorMixMode, this.blendInfluence);
        }
    }
}
