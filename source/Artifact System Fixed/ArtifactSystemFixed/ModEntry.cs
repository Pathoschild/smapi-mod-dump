/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/ArtifactSystemFixed
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ArtifactSystemFixed
{
	class ModEntry : Mod
	{
		public override void Entry(IModHelper helper)
		{
			LoadConfig();

			var harmony = HarmonyInstance.Create("me.ilyaki.ArtifactSystemFixed");
			Patch.PatchAll(harmony);
			
			//Debugging commands
			/*helper.ConsoleCommands.Add("asf_addspot", "Add an artifact dig spot where you are standing", this.AddArtifactDigSpot);
			helper.ConsoleCommands.Add("asf_addspots", "Add lots of dig spots (Do not use this!)", this.AddArtifactDigSpots);*/
			

			helper.ConsoleCommands.Add("asf_reloadconfig", "Reloads the config", 
				(cmd, args) => { LoadConfig(); Monitor.Log("Reloaded config", LogLevel.Info); } );

		}

		private void LoadConfig()
		{
			var config = this.Helper.ReadConfig<ModConfig>();
			GameLocation_digUpArtifactSpot_Patcher.Config = config;
			Utility_getTreasureFromGeode_Patcher.Config = config;
		}

		private void AddArtifactDigSpot(string command, string[] args)
		{
			var loc = Game1.player.getTileLocation();
			var obj = new StardewValley.Object(loc, 590, 1);
			Game1.currentLocation.objects.Add(loc, obj);
		}

		private void AddArtifactDigSpots(string command, string[] args)
		{
			int width = Game1.currentLocation.Map.Layers[0].LayerWidth;
			int height = Game1.currentLocation.Map.Layers[0].LayerHeight;

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					var loc = new Vector2(i,j);
					if (!Game1.currentLocation.isTileOccupiedForPlacement(loc))
					{
						var obj = new StardewValley.Object(loc, 590, 1);
						Game1.currentLocation.objects.Add(loc, obj);
					}
				}
			}
		}
	}
}
