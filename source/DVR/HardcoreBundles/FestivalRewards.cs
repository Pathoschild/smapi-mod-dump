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
