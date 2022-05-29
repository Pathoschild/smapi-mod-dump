/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using StardewValley.Buildings;


namespace IndustrialFurnace
{
	public interface IIndustrialFurnaceAPI
	{
		bool IsBuildingIndustrialFurnace(Building building);
		IndustrialFurnaceController? GetController(int ID);
	}


	public class IndustrialFurnaceAPI : IIndustrialFurnaceAPI
	{
		// TODO: Tyr not to give the entire ModEntry...
		private readonly ModEntry mod;


		public IndustrialFurnaceAPI(ModEntry mod)
		{
			this.mod = mod;
		}


		/// <summary>Checks if the provided building is an Industrial Furnace.</summary>
		public bool IsBuildingIndustrialFurnace(Building building)
		{
			return ModEntry.MainIsBuildingIndustrialFurnace(building);
		}


		/// <summary>Returns the controller that matches the provided ID.</summary>
		public IndustrialFurnaceController? GetController(int ID)
		{
			return mod.GetController(ID);
		}
	}
}
