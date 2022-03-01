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

namespace FashionSense.Framework.Models.Shoes
{
    public class ShoesContentPack : AppearanceContentPack
    {
        public ShoesModel BackShoes { get; set; }
        public ShoesModel RightShoes { get; set; }
        public ShoesModel FrontShoes { get; set; }
        public ShoesModel LeftShoes { get; set; }

        internal ShoesModel GetShoesFromFacingDirection(int facingDirection)
        {
            ShoesModel ShoesModel = null;
            switch (facingDirection)
            {
                case 0:
                    ShoesModel = BackShoes;
                    break;
                case 1:
                    ShoesModel = RightShoes;
                    break;
                case 2:
                    ShoesModel = FrontShoes;
                    break;
                case 3:
                    ShoesModel = LeftShoes;
                    break;
            }

            return ShoesModel;
        }
    }
}
