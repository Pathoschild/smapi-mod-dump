using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace DishOfTheDayDisplay
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
		{	
			this.Monitor.Log($"Dish of the Day: {Game1.dishOfTheDay.Name} ({Game1.dishOfTheDay.DisplayName})", LogLevel.Debug);
			if (this.Config.Show_Chat_Message) {
				string message = this.Helper.Translation.Get("dod-message", new { dishName = Game1.dishOfTheDay.DisplayName, dateString = StardewValley.Utility.getDateString() } );
                Game1.chatBox.addInfoMessage(message);
				this.Monitor.Log($"Sending message to chat: {message}", LogLevel.Trace);
			}
			
			if (this.Config.Place_Sign) {
				GameLocation theMap = Game1.getLocationFromName(this.Config.Sign_Location_Map_Name);
				if (theMap == null) {
					this.Monitor.Log($"Unknown map name: {this.Config.Sign_Location_Map_Name}. No sign will be used.", LogLevel.Warn);
					return;
				}
				
				// Hardcoding object IDs in the finest Stardew Valley tradition
				const int woodSignID = 37;
				const int stoneSignID = 38;
				
				int objectID = woodSignID;
				if (this.Config.Sign_Type.ToLower() != "wood") {
					if (this.Config.Sign_Type.ToLower() == "stone") {
						objectID = stoneSignID;
					} else {
						this.Monitor.Log($"Unknown sign type: {this.Config.Sign_Type}. Expected \"wood\" or \"stone\". Will use wood instead.", LogLevel.Warn);
					}
				}
				
				int signX = this.Config.Sign_Location_Tile_X;
                int signY = this.Config.Sign_Location_Tile_Y;
				Vector2 theSpot = new Vector2(signX, signY);
                
				Sign theSign = null;
				if (theMap.objects.TryGetValue(theSpot, out StardewValley.Object existingObject)) {
					if (existingObject.bigCraftable.Value && (existingObject.ParentSheetIndex == woodSignID || existingObject.ParentSheetIndex == stoneSignID)) {
						// use existing sign
						theSign = (Sign)existingObject;
					} else {
						this.Monitor.Log($"Found and removed one {existingObject.Name} at {signX}, {signY} on map {theMap.Name}.", LogLevel.Info);
						theMap.objects.Remove(theSpot);
					}
				}
				
				// Creating a new sign at the specified spot
				if (theSign == null) {
					theSign = new Sign(theSpot, objectID);
					theMap.objects.Add(theSpot, theSign);
				}
				theSign.displayItem.Value = Game1.dishOfTheDay.getOne();
				theSign.displayType.Value = 1;
				this.Monitor.Log($"{theSign.name} updated at {signX}, {signY} on map {theMap.Name}.", LogLevel.Trace);
			}
		}
    }
}