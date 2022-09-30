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
using Omegasis.Revitalize.Framework.Constants.ItemIds.Objects;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;

namespace Omegasis.Revitalize.Framework.Utilities
{
    /// <summary>
    /// Utilities based around machines added by Revitalize and other mods.
    /// </summary>
    public class MachineUtilities
    {

        public static Dictionary<string, List<ResourceInformation>> ResourcesForMachines;


        public static void InitializeResourceList()
        {

            ResourcesForMachines = new Dictionary<string, List<ResourceInformation>>()
            {
                /*
            {"Revitalize.Objects.Machines.BatteryBin" ,new List<ResourceInformation>(){
                new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.BatteryPack,1),1,1,1,1,1,1,0,0,0,0)
            } },
            {"Revitalize.Objects.Machines.Sandbox",new List<ResourceInformation>(){
                new ResourceInformation(ModCore.ObjectManager.GetItem(MiscEarthenResources.Sand,1),1,1,1,1,1,1,0,0,0,0)
            } }
                */
            };
        }

    }
}
