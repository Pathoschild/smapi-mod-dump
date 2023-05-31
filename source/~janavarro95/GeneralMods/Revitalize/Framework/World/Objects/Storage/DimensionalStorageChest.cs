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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Netcode;
using Newtonsoft.Json;
using Omegasis.Revitalize.Framework.Menus.Items;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Buildings;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using StardewValley;

namespace Omegasis.Revitalize.Framework.World.Objects.Storage
{
    /// <summary>
    /// A class to handle additional, condensed storage containers.
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Storage.DimensionalStorageChest")]
    public class DimensionalStorageChest : CustomObject
    {

        public DimensionalStorageChest() { }

        public DimensionalStorageChest(BasicItemInformation basicItemInformation) : base(basicItemInformation)
        {
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (Game1.activeClickableMenu == null)
            {
                DimensionalStorageUnitBuilding building= DimensionalStorageUnitBuilding.GetDimensionalStorageUnitBuilding();

                if (building != null)
                {
                    Game1.activeClickableMenu = new DimensionalStorageUnitMenu(building);
                    return true;
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
            return new DimensionalStorageChest(this.basicItemInformation.Copy());
        }
    }
}
