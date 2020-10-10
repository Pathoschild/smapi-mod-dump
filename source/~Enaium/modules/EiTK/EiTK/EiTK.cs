/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using System.Collections.Generic;
using EiTK.Gui;
using EiTK.Update;
using EiTK.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace EiTK
{
    public class EiTK : Mod
    {
        public static UpdateData updateData;

        public override void Entry(IModHelper helper)
        {
            updateData = helper.Data.ReadJsonFile<UpdateData>("manifest.json");
            if (UpdateUtils.isNewVersion(updateData))
                Monitor.Log("You can update mod:" + updateData.contactDatas[0].websiteLink, LogLevel.Warn);
        }
        
    }
}