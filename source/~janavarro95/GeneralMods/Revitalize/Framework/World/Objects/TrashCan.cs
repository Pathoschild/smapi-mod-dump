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
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Revitalize.Framework.Menus;
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects
{
    public class TrashCan : CustomObject, IInventoryManagerProvider
    {

        public TrashCan()
        {
        }

        public TrashCan(BasicItemInformation basicItemInformation):base(basicItemInformation)
        {

        }

        public TrashCan(BasicItemInformation basicItemInformation, Vector2 TilePosition) : base(basicItemInformation, TilePosition)
        {

        }

        public ref InventoryManager GetInventoryManager()
        {
            return ref this.basicItemInfo.inventory;
        }

        public void SetInventoryManager(InventoryManager Manager)
        {
            this.basicItemInfo.inventory = Manager;
        }

        public override bool rightClicked(Farmer who)
        {
            if (Game1.player.ActiveObject != null)
            {
                if (this.GetInventoryManager() != null && Game1.activeClickableMenu == null)
                {
                    this.GetInventoryManager().addItem(Game1.player.ActiveObject);
                    Game1.player.Items.Remove(Game1.player.ActiveObject);
                }
            }
            else
            {
                if (this.GetInventoryManager() != null && Game1.activeClickableMenu == null)
                {
                    this.GetInventoryManager().clear();
                    Game1.activeClickableMenu = new InventoryTransferMenu(100, 100, 500, 500, this.GetInventoryManager().items, this.GetInventoryManager().capacity);
                }
            }
            return false;
        }

        public override Item getOne()
        {
            return new TrashCan();
        }
    }
}
