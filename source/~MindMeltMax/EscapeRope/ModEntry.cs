/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.IO;

namespace EscapeRope
{
    class ModEntry : Mod
    {
        public IJsonAssetsApi JA;
        public Warp warp;
        public int RopeID;
        
        public override void Entry(IModHelper helper) //Load helpers 
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.GameLoop.SaveLoaded += SaveLoaded;
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e) //Get JsonAssets Api and directory on Game launch
        {
            JA = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            JA.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets"));
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (JA != null) //Check if JsonAssets Api is present
            {
                RopeID = JA.GetObjectId("Escape Rope"); //Get the id for the Escape Rope 

                if (RopeID == -1) //If the Escape Rope doesn't have an id the console alerts the player
                {
                    Monitor.Log($"Failed loading Object JA.Escape_Rope", LogLevel.Warn);
                }
            }
            else //Alert the player if JsonAssets Api isn't present
            {
                Monitor.Log($"Failed Loading Json Assets API", LogLevel.Warn);
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            warp = new Warp(Game1.player.getTileX(), Game1.player.getTileY(), "Mine", 18, 5, false); //Create a warp to the mine entrance
            if (!Context.IsWorldReady) //if the world isn't loaded, do nothing
                return;



            if (Game1.player.currentLocation is MineShaft && Game1.player.CurrentItem.Name is "Escape Rope" && e.Button.IsUseToolButton()) // if the player is in the mine below level 0 and clicks with the escape rope it removes the item once and teleports the player
            {
                Game1.player.removeItemsFromInventory(RopeID, 1);
                Game1.player.warpFarmer(warp);
            }
        }
    }
    public interface IJsonAssetsApi //Get The JsonAssets Api functions
    {
        int GetObjectId(string name);
        void LoadAssets(string path);
    }
}
