/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThongUICore.Framework.Element
{
    internal class ScrollView : BaseElement
    {

        public List<BaseElement> Items = new();

        public override int Height
        {
            get
            {
                return this.Items.Sum(p => p.Height);
            }
            protected set
            {
                throw new NotImplementedException();
            }
        }
        public override int Width
        {
            get
            {
                return this.Items.Sum(p => p.Width);
            }
            protected set
            {
                throw new NotImplementedException();
            }
        }
    }
}
