/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/WeatherTotems
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;
using StardewValley.Logging;
using StardewValley.Network;

namespace WeatherTotems
{
	public class ModEntry
		: Mod
	{
		private static bool success = false;

		private static List<string> totems = new List<string>() { "TheMightyAmondee.WeatherTotemsCP_SunTotem", "TheMightyAmondee.WeatherTotemsCP_WindTotem", "TheMightyAmondee.WeatherTotemsCP_SnowTotem", "TheMightyAmondee.WeatherTotemsCP_ThunderTotem", "TheMightyAmondee.WeatherTotemsCP_GreenRainTotem" };
		public override void Entry(IModHelper helper)
		{
			WeatherTotem.Initialise(this.Helper, this.ModManifest);
			i18n.gethelpers(helper.Translation);

			helper.Events.Input.ButtonPressed += this.ButtonPressed;
			helper.Events.Specialized.LoadStageChanged += this.LoadStageChanged;
			helper.Events.GameLoop.DayStarted += this.DayStarted;
		}

        private void LoadStageChanged(object sender, LoadStageChangedEventArgs e)
		{
			foreach(var farmer in Game1.getAllFarmers())
			{
                var modData = farmer.modData;

                if (e.NewStage == StardewModdingAPI.Enums.LoadStage.Loaded && modData.ContainsKey($"{this.ModManifest.UniqueID}/GreenTotemUse") && modData[$"{this.ModManifest.UniqueID}/GreenTotemUse"] == "true")
                {
                    StartGreenRain();
                }
            }			
		}

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                var modData = farmer.modData;
                if (modData.ContainsKey($"{this.ModManifest.UniqueID}/GreenTotemUse") == false)
                {
                    modData.Add($"{this.ModManifest.UniqueID}/GreenTotemUse", "false");

                }
                else if (modData[$"{this.ModManifest.UniqueID}/GreenTotemUse"] == "true")
                {
                    modData[$"{this.ModManifest.UniqueID}/GreenTotemUse"] = "false";
                }
            }
           
        }

        public static void StartGreenRain()
        {
            LocationWeather weather = Game1.netWorldState.Value.GetWeatherForLocation("Default");
            weather.IsGreenRain = true;
            weather.IsDebrisWeather = false;
            Game1.isRaining = weather.IsRaining;
            Game1.isGreenRain = weather.IsGreenRain;
            Game1.isDebrisWeather = false;
        }
        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			if (true 
				&& e.Button.IsActionButton() == true 
				&& Context.CanPlayerMove == true 
				&& Game1.player.CurrentItem != null 
				&& totems.Contains(Game1.player.CurrentItem.ItemId) == true)
			{
				// Get whether totem can change weather
				bool normal_gameplay = true 
					&& Game1.eventUp == false 
					&& Game1.isFestival() == false 
					&& Game1.fadeToBlack == false 
					&& Game1.player.swimming.Value == false 
					&& Game1.player.bathingClothes.Value == false 
					&& Game1.player.onBridge.Value == false;

				// Is the item used one of the weather totems?
				if (Game1.player.CurrentItem.Name != null && normal_gameplay == true)
				{
					// Yes, can the totem update tomorrows weather?

					if (Game1.currentLocation.GetLocationContextId() == "Default" && Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season) == true)
					{
                        var hudmessage = new HUDMessage(i18n.string_ErrorFestival(), 3);
                        this.Monitor.Log("Failed to set weather", LogLevel.Trace);
                        Game1.addHUDMessage(hudmessage);
						return;
                    }

					if (normal_gameplay == true)
					{
						success = false;
						// Yes, which totem is it?
						// Execute method with arguments based on the totem type
						switch (Game1.player.CurrentItem.ItemId)
						{
							case "TheMightyAmondee.WeatherTotemsCP_SunTotem":
								success = WeatherTotem.UseWeatherTotem(Game1.player, 0);
								this.Monitor.Log("Try set to sunny weather tomorrow", LogLevel.Trace);
								break;
							case "TheMightyAmondee.WeatherTotemsCP_WindTotem":
                                success = WeatherTotem.UseWeatherTotem(Game1.player, 1);
								this.Monitor.Log("Try set to windy weather tomorrow", LogLevel.Trace);
								break;
							case "TheMightyAmondee.WeatherTotemsCP_SnowTotem":
								success = WeatherTotem.UseWeatherTotem(Game1.player, 2);
								this.Monitor.Log("Try set to snowy weather tomorrow", LogLevel.Trace);
								break;
							case "TheMightyAmondee.WeatherTotemsCP_ThunderTotem":
								success = WeatherTotem.UseWeatherTotem(Game1.player, 3);
								this.Monitor.Log("Try set to stormy weather tomorrow", LogLevel.Trace);
								break;
                            case "TheMightyAmondee.WeatherTotemsCP_GreenRainTotem":
                                success = WeatherTotem.UseWeatherTotem(Game1.player, 4);
                                this.Monitor.Log("Try set to green rain weather tomorrow", LogLevel.Trace);
                                break;
                            default:
								break;
						}

						if (success == true)
						{
                            Game1.player.removeFirstOfThisItemFromInventory(Game1.player.CurrentItem.ItemId);
                            this.Monitor.Log("Weather set successfully", LogLevel.Trace);
                        }
						else
						{
							var hudmessage = new HUDMessage(i18n.string_Error(), 3);
                            this.Monitor.Log("Failed to set weather", LogLevel.Trace);
                            Game1.addHUDMessage(hudmessage);
						}

					}
				}
			}
		}
	}	
}
