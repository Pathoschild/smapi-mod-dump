/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionSense.Framework.Models
{
    public class AppearanceModel
    {
        internal string Owner { get; set; }
        internal string Author { get; set; }
        public string Name { get; set; }
        internal string Id { get; set; }
        internal Texture2D Texture { get; set; }
        public HairModel BackHair { get; set; }
        public HairModel RightHair { get; set; }
        public HairModel FrontHair { get; set; }
        public HairModel LeftHair { get; set; }

        internal HairModel GetHairFromFacingDirection(int facingDirection)
        {
            HairModel hairModel = null;
            switch (facingDirection)
            {
                case 0:
                    hairModel = BackHair;
                    break;
                case 1:
                    hairModel = RightHair;
                    break;
                case 2:
                    hairModel = FrontHair;
                    break;
                case 3:
                    hairModel = LeftHair;
                    break;
            }

            return hairModel;
        }
    }
}
