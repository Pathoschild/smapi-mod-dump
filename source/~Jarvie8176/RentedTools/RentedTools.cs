/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jarvie8176/StardewMods
**
*************************************************/

using StardewValley.Tools;

namespace RentedTools
{
    public interface IRentedTool
    {

    }

    public class RentedAxe : Axe, IRentedTool
    {
        public RentedAxe()
        {
            this.Name = "RentedAxe";
        }
    }

    public class RentedPickaxe : Pickaxe, IRentedTool
    {
        public RentedPickaxe()
        {
            this.Name = "RentedPickaxe";
        }
    }

    public class RentedHoe : Hoe, IRentedTool
    {
        public RentedHoe() : base()
        {
            this.Name = "RentedHoe";
        }
    }

    public class RentedWateringCan : WateringCan, IRentedTool
    {
        public RentedWateringCan()
        {
            // this.Name = "RentedWateringCan";
        }
    }
}
