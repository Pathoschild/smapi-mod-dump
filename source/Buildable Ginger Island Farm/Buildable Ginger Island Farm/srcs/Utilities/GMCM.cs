/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/BuildableGingerIslandFarm
**
*************************************************/

namespace BuildableGingerIslandFarm.Utilities
{
	public sealed class ModConfig
	{
		public bool	AllowBuildingInSlimeArea = false;
	}

	internal class GMCMUtility
	{
		internal static void Initialize()
		{
			ReadConfig();
			Register();
		}

		private static void ReadConfig()
		{
			ModEntry.Config = ModEntry.Helper.ReadConfig<ModConfig>();
		}

		private static void Register()
		{
			// Get Generic Mod Config Menu's API
			GenericModConfigMenu.IGenericModConfigMenuApi gmcm = ModEntry.Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (gmcm is null)
				return;

			// Register mod
			gmcm.Register(
				mod: ModEntry.ModManifest,
				reset: () => ModEntry.Config = new ModConfig(),
				save: () => ModEntry.Helper.WriteConfig(ModEntry.Config)
			);

			// Main
			gmcm.AddBoolOption(
				mod: ModEntry.ModManifest,
				name: () => ModEntry.Helper.Translation.Get("GMCM.AllowBuildingInSlimeArea.Title"),
				tooltip: () => ModEntry.Helper.Translation.Get("GMCM.AllowBuildingInSlimeArea.Tooltip"),
				getValue: () => ModEntry.Config.AllowBuildingInSlimeArea,
				setValue: (value) => 
				{
					ModEntry.Config.AllowBuildingInSlimeArea = value;
					GingerIslandFarmUtility.UpdateSlimeArea();
				}
			);
		}
	}
}
