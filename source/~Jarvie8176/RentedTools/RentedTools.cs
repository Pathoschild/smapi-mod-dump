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
