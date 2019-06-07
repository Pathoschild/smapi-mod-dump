using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;
using xTile;

namespace TheChestDimension
{
    public class ModEntry : Mod
    {
        private ModConfig config;

        // player's location and position before the warp
        GameLocation OldLocation;
        Vector2 OldPosition;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            config = Helper.ReadConfig<ModConfig>();
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == config.TCDKey)
            {
                // if already in TCD, go back
                if (PlayerInTCD)
                {
                    Game1.warpFarmer(OldLocation.Name, (int)OldPosition.X, (int)OldPosition.Y, false);
                    return;
                }

                // shorten string names, for the upcoming monstrosity of an if statement
                string coef = config.CanOnlyEnterFrom;
                string curloc = Game1.player.currentLocation.Name;

                if
                (
                    // no specific entry location, or if there is any, it's the same as the current location
                    (coef == null || coef == "" || coef == curloc)
                    &&
                    // player is not in a cave, when CanEnterFromCave is false
                    !(!config.CanEnterFromCave && curloc.StartsWith("UndergroundMine"))
                )
                {
                    OldLocation = Game1.player.currentLocation;
                    OldPosition = Game1.player.getTileLocation();

                    // get custom spawn location from save
                    string spawnX = Helper.Data.ReadSaveData<string>("TCD_spawn_location_X");
                    string spawnY = Helper.Data.ReadSaveData<string>("TCD_spawn_location_Y");

                    // if custom spawn location is null, warp to default spawn location, else warp to custom spawn location 
                    if (spawnX == null)
                    {
                        Game1.warpFarmer("ChestDimension", 55, 37, false);
                    }
                    else
                    {
                        Game1.warpFarmer("ChestDimension", int.Parse(spawnX), int.Parse(spawnY), false);
                    }

                    // if entry message is enabled, replace variables and show entry message in chat
                    if (config.ShowEntryMessage)
                    {
                        string msg = config.EntryMessage;
                        msg.Replace("{TCDkey}", config.TCDKey.ToString());
                        Game1.chatBox.addInfoMessage(msg);
                    }
                }
                else // cannot enter TCD
                {
                    if (config.ShowCannotEnterMessage)
                    {
                        Game1.chatBox.addInfoMessage(config.CannotEnterMessage);
                    }
                }
            }
            else if (e.Button == config.SetSpawnKey)
            {
                if (PlayerInTCD)
                {
                    // save current location as spawn location

                    string xPos = Game1.player.getTileX().ToString();
                    string yPos = Game1.player.getTileY().ToString();

                    Helper.Data.WriteSaveData("TCD_spawn_location_X", xPos);
                    Helper.Data.WriteSaveData("TCD_spawn_location_Y", yPos);

                    Game1.chatBox.addInfoMessage("TCD spawn location set to " + xPos + "," + yPos + ".");
                }
                else
                {
                    Game1.chatBox.addErrorMessage("You have to be inside TCD to set its spawn location.");
                }
            }
        }

        private bool PlayerInTCD => Game1.player.currentLocation.Name == "ChestDimension";
    }
}