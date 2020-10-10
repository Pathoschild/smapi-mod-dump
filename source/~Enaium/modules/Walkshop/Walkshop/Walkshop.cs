/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using EiTK.Update;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using WalkSho;
using Object = StardewValley.Object;

namespace Walkshop
{
    public class Walkshop : Mod
    {
        private Config Config;
        public static UpdateData updateData;

        public override void Entry(IModHelper helper)
        {
            updateData = helper.Data.ReadJsonFile<UpdateData>("manifest.json");
            if(UpdateUtils.isNewVersion(updateData)) Monitor.Log("You can update mod:" + updateData.contactDatas[0].websiteLink,LogLevel.Warn);
            Config = helper.ReadConfig<Config>();
            helper.Events.Input.ButtonPressed += onButton;
        }
        
        private void onButton(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (!Context.IsPlayerFree)
                return;
            if (e.Button != Config.Walkshop)
                return;
            Game1.activeClickableMenu = new Menu(Helper);
        }


    }
}