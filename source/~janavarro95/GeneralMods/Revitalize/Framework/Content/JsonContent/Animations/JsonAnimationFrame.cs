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
using Omegasis.StardustCore.Animations;

namespace Omegasis.Revitalize.Framework.Content.JsonContent.Animations
{
    public class JsonAnimationFrame
    {

        public Rectangle animationSourceRectangle = new Rectangle();
        public int drawForXFrames = 0;

        public JsonAnimationFrame()
        {

        }

        public virtual AnimationFrame toAnimationFrame()
        {
            return new AnimationFrame(this.animationSourceRectangle, this.drawForXFrames);
        }
    }
}
