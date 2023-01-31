/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Unlockable_Areas.Lib
{
    //The model is here because casting directly to Unlockable caused issues
    public class UnlockableModel
    {
        //Don't forget to implement changes to Unlockable constructor as well
        public string ID = "";
        public string Location = "";
        public string ShopDescription = "";
        public string ShopPosition = "";
        public string ShopTexture = "UnlockableAreas/ShopTextures/Sign";
        public string ShopAnimation = "";
        public Dictionary<string, int> Price = new Dictionary<string, int>();
        public string UpdateMap = "";
        public string UpdateType = "Overlay";
        public string UpdatePosition = "";

        public static explicit operator UnlockableModel(Unlockable v)
        {
            return new UnlockableModel() {
                ID = v.ID,
                Location = v.Location,
                ShopDescription = v.ShopDescription,
                ShopPosition = v.ShopPosition,
                ShopTexture = v.ShopTexture,
                ShopAnimation = v.ShopAnimation,
                Price = v.Price,
                UpdateMap = v.UpdateMap,
                UpdateType = v.UpdateType,
                UpdatePosition = v.UpdatePosition,
            };
        }
    }
}
