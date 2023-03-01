/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JhonnieRandler/TVBrasileira
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TVBrasileira.channels
{
    public class EdnaldoPereira : Channel
    {
        private static readonly Rectangle WeatherReportArea = new(413, 305, 126, 28);
        private static readonly Rectangle IslandReportArea = new(148, 62, 42, 28);

        private static IRawTextureData _ednaldoPereiraTexture;
        private static IRawTextureData _ednaldoIslandTexture;

        private string _farmerName;
        
        public EdnaldoPereira(IModHelper helper, IMonitor monitor) : base(helper, monitor)
        {
            _ednaldoPereiraTexture = Helper.ModContent.Load<IRawTextureData>("assets/ednaldoPereira.png");
            _ednaldoIslandTexture = Helper.ModContent.Load<IRawTextureData>("assets/ednaldoIsland.png");
            
            TargetDialogueAssets = new List<string> { "Strings/StringsFromCSFiles" };
            TargetImageAssets = new List<string> { "LooseSprites/Cursors", "LooseSprites/Cursors2" };
            
            Helper.Events.Content.AssetRequested += CheckTargetDialogues;
            Helper.Events.Content.AssetRequested += CheckTargetImages;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoad;
            Helper.Events.GameLoop.UpdateTicked += UpdateFarmerName;
        }

        protected override void SetCustomDialogues(IAssetDataForDictionary<string, string> editor, IAssetName assetName)
        {
            editor.Data["TV.cs.13105"] = I18n.TitleEdnaldo();
            editor.Data["TV.cs.13136"] = IsChannelEnabled() ? 
                I18n.IntroEdnaldo() : I18n.DisabledEdnaldo(_farmerName);
            editor.Data["TV.cs.13175"] = I18n.FestivalEdnaldo();
            editor.Data["TV.cs.13180"] = I18n.SnowEdnaldo();
            editor.Data["TV.cs.13181"] = I18n.AltSnowEdnaldo();
            editor.Data["TV.cs.13182"] = I18n.SunnyEdnaldo();
            editor.Data["TV.cs.13183"] = I18n.AltSunnyEdnaldo();
            editor.Data["TV.cs.13184"] = I18n.RainEdnaldo();
            editor.Data["TV.cs.13185"] = I18n.StormEdnaldo();
            editor.Data["TV.cs.13187"] = I18n.CloudyEdnaldo();
            editor.Data["TV.cs.13189"] = I18n.WindCloudyEdnaldo();
            editor.Data["TV.cs.13190"] = I18n.BlizzardEdnaldo();
            editor.Data["TV_IslandWeatherIntro"] = I18n.IslandEdnaldo();
        }

        protected override void SetCustomImages(IAssetDataForImage editor, IAssetName assetName)
        {
            switch (assetName.ToString())
            {
                case "LooseSprites/Cursors":
                    editor.PatchImage(_ednaldoPereiraTexture, targetArea: WeatherReportArea);
                    return;
                case "LooseSprites/Cursors2":
                    editor.PatchImage(_ednaldoIslandTexture, targetArea: IslandReportArea);
                    return;
            }
        }

        private void OnSaveLoad(object sender, SaveLoadedEventArgs e)
        {
            _farmerName = Game1.player.Name;
            InvalidateDialogues();
        }

        private void UpdateFarmerName(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (_farmerName == Game1.player.Name) return;
            _farmerName = Game1.player.Name;
            InvalidateDialogues();
        }
    }
}