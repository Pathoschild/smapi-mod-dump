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
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Player;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Debris
{
    public class CustomObjectDebris:StardewValley.Debris
    {

        public CustomObjectDebris()
        {

        }

        public CustomObjectDebris(Item item, Vector2 DebrisOrigin) : base(item, DebrisOrigin)
        {

        }

        public CustomObjectDebris(Item item, Vector2 DebrisOrigin, Vector2 DebrisTargetLocation) :base(item,DebrisOrigin,DebrisTargetLocation)
        {

        }

        public override bool collect(Farmer farmer, Chunk chunk = null)
        {
            if (this.item != null)
            {
                Item tmpItem = this.item;
                this.item = null;
                if (!PlayerUtilities.AddItemToInventory(farmer, tmpItem))
                {
                    this.item = tmpItem;
                    return false;
                }
            }
            return true;
        }

    }
}
