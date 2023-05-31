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
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Omegasis.Revitalize.Framework.Menus.Items;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using StardewValley;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Buildings;
using System.IO;

namespace Omegasis.Revitalize.Framework.World.Objects.Storage
{
    /// <summary>
    /// A class to handle additional, condensed storage containers.
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Storage.DimensionalStorageBag")]
    public class DimensionalStorageBag : CustomItem
    {

        public DimensionalStorageBag() { }

        public DimensionalStorageBag(BasicItemInformation basicItemInformation) : base(basicItemInformation)
        {
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            return false;
        }

        public override bool performUseAction(GameLocation location)
        {
            return this.openInventoryMenu();
        }

        /// <summary>
        /// Opens the inventory of the item bag to display it's contents.
        /// </summary>
        /// <returns></returns>
        public virtual bool openInventoryMenu()
        {
            if (Game1.activeClickableMenu == null)
            {
                DimensionalStorageUnitBuilding building = DimensionalStorageUnitBuilding.GetDimensionalStorageUnitBuilding();

                if (building != null)
                {
                    Game1.activeClickableMenu = new DimensionalStorageUnitMenu(building);
                    //If false, prevent item from being consumed.
                    return false;
                }
                else
                {
                    Game1.showRedMessage(JsonContentPackUtilities.LoadErrorString(Path.Combine("Objects", "Storage", "DimensionalStorageChest.json"), "DimensionalStorageUnitBuildingNotBuilt"));
                }
            }
            return false;
        }

        public override Item getOne()
        {
            return new DimensionalStorageBag(this.basicItemInformation.Copy());
        }

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {
            //Don't draw placement bounds.
        }
    }
}
