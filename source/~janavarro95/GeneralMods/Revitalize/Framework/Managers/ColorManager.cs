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

namespace Revitalize.Framework.Managers
{
    public class ColorManager
    {
        /// <summary>
        /// The color blending mode for this color manager.
        /// </summary>
        private Enums.DyeBlendMode _colorMixMode;

        public Enums.DyeBlendMode ColorMixMode
        {
            get
            {
                return this._colorMixMode;
            }
            set
            {
                this._colorMixMode = value;
                this.requiresUpdate = true;
            }
        }

        /// <summary>
        /// The amount of influence the first color has on the mix for the object.
        /// </summary>
        private double _blendInfluence;
        public double BlendInfluence
        {
            get
            {
                return this._blendInfluence;
            }
            set
            {
                this._blendInfluence = value;
                this.requiresUpdate = true;
            }
        }

        /// <summary>
        /// Does this ColorManager require a sync update.
        /// </summary>
        public bool requiresUpdate;

        public ColorManager()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="DyeBlendMode"></param>
        public ColorManager(Enums.DyeBlendMode DyeBlendMode,double BlendInfluence=0d)
        {
            this._colorMixMode = DyeBlendMode;
            this._blendInfluence = BlendInfluence;
        }

        /// <summary>
        /// Constructor which loosely enforeces the color blend mode to be a mixed blend but only requires a influence parameter.
        /// </summary>
        /// <param name="BlendInfluence"></param>
        /// <param name="DyeBlendMode"></param>
        public ColorManager(double BlendInfluence, Enums.DyeBlendMode DyeBlendMode=Enums.DyeBlendMode.Blend)
        {
            this._colorMixMode = DyeBlendMode;
            this._blendInfluence = BlendInfluence;
        }

        public Color getBlendedColor(Color self, Color other,int Alpha=255)
        {
            //Used as reference.
            //https://stackoverflow.com/questions/3722307/is-there-an-easy-way-to-blend-two-system-drawing-color-values
            if (this._colorMixMode== Enums.DyeBlendMode.Blend)
            {
                int r =(int)(self.R * this._blendInfluence + other.R * (1 - this._blendInfluence));
                int g = (int)(self.G * this._blendInfluence + other.G * (1 - this._blendInfluence));
                int b = (int)(self.B * this._blendInfluence + other.B * (1 - this._blendInfluence));
                return new Color(r, g, b, Alpha);
            }
            if(this._colorMixMode== Enums.DyeBlendMode.Average)
            {
                return new Color((self.R + other.R) / 2, (self.G + other.G) / 2, (self.B + other.B) / 2, Alpha);
            }

            if(this._colorMixMode== Enums.DyeBlendMode.Multiplier)
            {
                return new Color(self.R * other.R, self.G * other.G,self.B * other.B, Alpha);
            }

            return self;
        }

        public ColorManager Copy()
        {
            return new ColorManager(this._blendInfluence, this._colorMixMode);
        }
    }
}
