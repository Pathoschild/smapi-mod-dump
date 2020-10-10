/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/captncraig/StardewMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace HardcoreBundles
{
    public class FestivalRewards : IAssetEditor
    {
        private IModHelper Helper;

        public FestivalRewards(IModHelper helper)
        {
            Helper = helper;
            Helper.Content.AssetEditors.Add(this);
            Helper.Events.GameLoop.DayEnding += DayEnding;
            
        }

        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            var n = SDate.Now();
            if (n.Day == 24 && n.Season == "spring")
            {
                var x = 0;
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
        }
    }
}
