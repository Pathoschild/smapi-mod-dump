//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;

using StardewModdingAPI;
using StardewValley.Tools;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using System;

namespace InteractionTweaks
{
    public class ModEntry : Mod, IAssetEditor
    {
        //private bool eating_changes = true;
        private InteractionTweaksConfig config;

        public override void Entry(IModHelper helper)
        {
            //initializes mod features and reads config
            ModFeature.Init(this);

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;



            //TODO:
            //ReturnMuseumRewardsFeature.Enable();

            //if (config.ToolsFeature)
            //     DontUseToolsFeature.Enable();

            Monitor.Log("Configuration:", LogLevel.Trace);
            Monitor.Log($"config.EatingFeature: {config.EatingFeature}", LogLevel.Trace);
            Monitor.Log($"config.AdventurersGuildShopFeature: {config.AdventurersGuildShopFeature}", LogLevel.Trace);
            Monitor.Log($"config.CarpenterMenuFeature: {config.CarpenterMenuFeature}", LogLevel.Trace);
            Monitor.Log($"config.SellableItemsFeature: {config.SellableItemsFeature}", LogLevel.Trace);
            //Monitor.Log($"config.SlingshotFeature: {config.SlingshotFeature}", LogLevel.Trace);
            //Monitor.Log($"config.FishingRodFeature: {config.FishingRodFeature}", LogLevel.Trace);

            /*
            Helper.ConsoleCommands.Add("player_inventory", "",(string arg1, string[] arg2) => {
                Monitor.Log("Player inventory:", LogLevel.Info);
                foreach (StardewValley.Item item in StardewValley.Game1.player.Items)
                    Monitor.Log("Item is type " + item?.GetType().ToString(), LogLevel.Info);
            });
            Helper.ConsoleCommands.Add("reset_marnie", "", (string arg1, string[] arg2) => {
                MarniesItemShopFeature.ResetItems(new StardewValley.Menus.ShopMenu(StardewValley.Utility.getAnimalShopStock()));
            });
            */

            //Helper.ConsoleCommands.Add("carpenter_menu", "", (arg1, arg2) => Game1.activeClickableMenu = new StardewValley.Menus.CarpenterMenu());

        }

        void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (config.EatingFeature || false/*config.WeaponBlockingFeature*/)
                EatingBlockingFeature.Enable();
            if (config.AdventurersGuildShopFeature || config.SellableItemsFeature)
                AdventurersGuildFeature.Enable();
            if (config.CarpenterMenuFeature)
                CarpenterMenuFeature.Enable();
            if (config.SellableItemsFeature)
                FishingRodFeature.Enable();
            if (config.SellableItemsFeature)
                MarniesItemShopFeature.Enable();
        }


        public InteractionTweaksConfig GetConfig() {
            if (config == null)
            {
                config = Helper.ReadConfig<InteractionTweaksConfig>();
            }
            return config;        
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/weapons");          
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/weapons"))
            {
                string[] dataArray = asset.AsDictionary<int, string>().Data[Slingshot.basicSlingshot].Split('/');
                dataArray[1] = Helper.Translation.Get("item.slingshotdescr");
                asset.AsDictionary<int, string>().Data[Slingshot.basicSlingshot] = string.Join("/", dataArray);

                dataArray = asset.AsDictionary<int, string>().Data[Slingshot.masterSlingshot].Split('/');
                dataArray[1] = Helper.Translation.Get("item.slingshotdescr");
                asset.AsDictionary<int, string>().Data[Slingshot.masterSlingshot] = string.Join("/", dataArray);
            }
        }

    }
}
