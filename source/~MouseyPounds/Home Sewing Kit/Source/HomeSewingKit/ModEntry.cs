using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using SVObject = StardewValley.Objects;
using Microsoft.Xna.Framework;



namespace HomeSewingKit
{
    public interface IJsonAssetsApi
    {
        int GetBigCraftableId(string name);
        void LoadAssets(string path);
    }

    public class ModEntry : Mod, IAssetEditor
    {
        private IJsonAssetsApi JsonAssets;
        private int SewingMachineID = -1;
        private int DyeCabinetID = -1;
        private bool HasSentLetter = false;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            JsonAssets = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (JsonAssets == null)
            {
                Monitor.Log("Can't load Json Assets API, which is needed for Home Sewing Kit to function", LogLevel.Error);
            }
            else
            {
                JsonAssets.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets"));
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (JsonAssets != null)
            {
                SewingMachineID = JsonAssets.GetBigCraftableId("Sewing Machine");
                DyeCabinetID = JsonAssets.GetBigCraftableId("Dye Cabinet");
                if (SewingMachineID == -1)
                {
                    Monitor.Log("Can't get ID for Sewing Machine. Some functionality will be lost.", LogLevel.Warn);
                }
                else
                {
                    Monitor.Log($"Sewing Machine ID is {SewingMachineID}.", LogLevel.Info);
                }
                if (DyeCabinetID == -1)
                {
                    Monitor.Log("Can't get ID for Dye Cabinet. Some functionality will be lost.", LogLevel.Warn);
                }
                else
                {
                    Monitor.Log($"Dye Cabinet ID is {DyeCabinetID}.", LogLevel.Info);
                }
            }
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!HasSentLetter && 
                !Game1.player.mailReceived.Contains("HomeSewingKit_Letter") && 
                !Game1.player.mailForTomorrow.Contains("HomeSewingKit_Letter") &&
                Game1.player.eventsSeen.Contains(992559))
            {
                Monitor.Log("Player does not have our mail. Queueing for tomorrow.");
                Game1.player.mailForTomorrow.Add("HomeSewingKit_Letter");
                HasSentLetter = true;
            }
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady &&
                Game1.currentLocation != null &&
                Game1.activeClickableMenu == null &&
                e.Button.IsActionButton())
            {
                //this.Monitor.Log("Passed basic checks", LogLevel.Trace);
                GameLocation loc = Game1.currentLocation;
                Vector2 tile = e.Cursor.GrabTile;
                loc.Objects.TryGetValue(tile, out StardewValley.Object obj);
                if (obj != null && obj.bigCraftable.Value)
                {
                    if (obj.ParentSheetIndex.Equals(SewingMachineID))
                    {
                        Game1.activeClickableMenu = new TailoringMenu();
                        Helper.Input.Suppress(e.Button);
                    }
                    else if (obj.ParentSheetIndex.Equals(DyeCabinetID))
                    {
                        // Duplicating logic from GameLocation.performAction()
                        if (!DyeMenu.IsWearingDyeable())
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:DyePot_NoDyeable"));
                        }
                        else
                        {
                            Game1.activeClickableMenu = new DyeMenu();
                        }
                        Helper.Input.Suppress(e.Button);
                    }
                }
            }
        }

        // Asset Editors
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return (asset.AssetNameEquals("Data/mail"));
        }
        public void Edit<T>(IAssetData asset)
        {
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
            if (asset.AssetNameEquals("Data/mail"))
            {
                data["HomeSewingKit_Letter"] = Helper.Translation.Get("HomeSewingKit_Letter");
            }
        }

    }
}
