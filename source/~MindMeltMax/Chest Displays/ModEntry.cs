/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

global using Object = StardewValley.Object;
global using SUtils = StardewValley.Utility;
using ChestDisplays.Patches;
using ChestDisplays.Utility;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace ChestDisplays
{
    public class ModEntry : Mod
    {
        public static IModHelper IHelper;
        public static IMonitor IMonitor;
        public static Config IConfig;

        private bool hadSavedData = false;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;

            helper.Events.Input.ButtonPressed += onButtonPressed;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.GameLaunched += onGameLaunch;

            CheckConfig();
        }


        private void CheckConfig()
        {
            IConfig = Helper.ReadConfig<Config>();
            if (!IConfig.ChangeItemButtons.Any())
            { 
                Monitor.Log($"No valid entry button found, button has been reset to defaults ('\"' on keyboard and 'LeftStick' on controller)", LogLevel.Warn);
                IConfig.ChangeItemKey = "OemQuotes, LeftStick";
                Helper.WriteConfig(IConfig);
            }
            Monitor.Log($"Loaded Chest Displays with input keys {IConfig.ChangeItemKey}");
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            if (!Context.IsMainPlayer) 
                return;

            if (hadSavedData)
                Helper.Data.WriteSaveData<object>($"MindMeltMax.ChestDisplay", null);
            hadSavedData = false;
        }

        private void GameLoop_SaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            foreach (var l in Game1.locations)
                foreach (var o in l.Objects.Values)
                    if (o is Chest c && c.modData.ContainsKey(Helper.ModRegistry.ModID))
                        Utils.updateCache(c);

            if (!Context.IsMainPlayer) 
                return;

            #region BackWardsCompat
            var data = Helper.Data.ReadSaveData<List<SaveData>>($"MindMeltMax.ChestDisplay");
            if (data == null) return;
            hadSavedData = true;
            Monitor.LogOnce($"Old save data was found, The mod will now attempt to update to the new version");
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var loc = Game1.getLocationFromName(item.Location);
                var key = new Vector2(item.X, item.Y);

                if (loc.Objects.ContainsKey(key) && loc.Objects[key] is Chest c)
                {
                    Item obj = SUtils.getItemFromStandardTextDescription(item.ItemDescription, Game1.player);
                    if (obj is null) 
                        continue;
                    ModData modData = new()
                    {
                        ItemId = obj.QualifiedItemId,
                        ItemQuality = obj is Object o ? o.Quality : -1, 
                        Color = null, 
                        UpgradeLevel = -1 
                    };

                    c.modData[Helper.ModRegistry.ModID] = JsonConvert.SerializeObject(modData);
                }
            }
            #endregion
        }

        private void onGameLaunch(object? sender, GameLaunchedEventArgs e) => Patcher.Init(Helper);

        private void onButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.CanPlayerMove)
                return;

            if (IConfig.ChangeItemButtons.Any(x => x == e.Button))
            {
                var tile = Game1.player.GetGrabTile();
                var OatT = Game1.player.currentLocation.getObjectAtTile((int)tile.X, (int)tile.Y);

                if (OatT is Chest c)
                    Game1.activeClickableMenu = new ChangeDisplayMenu(c);
            }
        }
    }
}
