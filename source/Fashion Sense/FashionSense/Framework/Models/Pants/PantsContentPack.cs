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

namespace FashionSense.Framework.Models.Pants
{
    public class PantsContentPack : AppearanceContentPack
    {
        public PantsModel BackPants { get; set; }
        public PantsModel RightPants { get; set; }
        public PantsModel FrontPants { get; set; }
        public PantsModel LeftPants { get; set; }

        internal PantsModel GetPantsFromFacingDirection(int facingDirection)
        {
            PantsModel PantsModel = null;
            switch (facingDirection)
            {
                case 0:
                    PantsModel = BackPants;
                    break;
                case 1:
                    PantsModel = RightPants;
                    break;
                case 2:
                    PantsModel = FrontPants;
                    break;
                case 3:
                    PantsModel = LeftPants;
                    break;
            }

            return PantsModel;
        }
    }
}
