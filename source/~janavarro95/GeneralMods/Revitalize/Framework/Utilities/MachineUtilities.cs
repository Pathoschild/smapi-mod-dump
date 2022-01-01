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
using Revitalize;
using Revitalize.Framework;
using Revitalize.Framework.Objects.InformationFiles;

namespace Revitalize.Framework.Utilities
{
    public class MachineUtilities
    {

        public static Dictionary<string, List<ResourceInformation>> ResourcesForMachines;


        public static List<ResourceInformation> GetResourcesProducedByThisMachine(string ID)
        {

            if (ResourcesForMachines == null) InitializeResourceList();

            if (ResourcesForMachines.ContainsKey(ID))
            {
                return ResourcesForMachines[ID];
            }
            else if (ID.Equals("Revitalize.Objects.Machines.MiningDrillV1"))
            {
                return ModCore.ObjectManager.resources.miningDrillResources.Values.ToList();
            }

            return new List<ResourceInformation>();
        }

        public static void InitializeResourceList()
        {
            
            ResourcesForMachines = new Dictionary<string, List<ResourceInformation>>()
        {
            {"Revitalize.Objects.Machines.BatteryBin" ,new List<ResourceInformation>(){
                new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.BatteryPack,1),1,1,1,1,1,1,0,0,0,0)
            } },
            {"Revitalize.Objects.Machines.Sandbox",new List<ResourceInformation>(){
                new ResourceInformation(ModCore.ObjectManager.resources.getResource("Sand",1),1,1,1,1,1,1,0,0,0,0)
            } }
        };
        }

    }
}
