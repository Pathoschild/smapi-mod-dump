using StardewValley.Buildings;


namespace IndustrialFurnace
{
	public interface IIndustrialFurnaceAPI
	{
		bool IsBuildingIndustrialFurnace(Building building);
		IndustrialFurnaceController GetController(int ID);
	}


	public class IndustrialFurnaceAPI : IIndustrialFurnaceAPI
	{
		// TODO: Tyr not to give the entire ModEntry...
		private ModEntry mod;


		public IndustrialFurnaceAPI(ModEntry mod)
		{
			this.mod = mod;
		}


		/// <summary>Checks if the provided building is an Industrial Furnace.</summary>
		public bool IsBuildingIndustrialFurnace(Building building)
		{
			return mod.IsBuildingIndustrialFurnace(building);
		}


		/// <summary>Returns the controller that matches the provided ID.</summary>
		public IndustrialFurnaceController GetController(int ID)
		{
			return mod.GetController(ID);
		}
	}
}
