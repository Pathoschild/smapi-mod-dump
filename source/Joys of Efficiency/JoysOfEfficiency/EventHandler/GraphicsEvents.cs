using JoysOfEfficiency.Automation;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Huds;
using JoysOfEfficiency.Misc;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;

namespace JoysOfEfficiency.EventHandler
{
    internal class GraphicsEvents
    {
        private static Config Conf => InstanceHolder.Config;

        public void OnRenderHud(object sender, RenderingHudEventArgs args)
        {
            if (Game1.currentLocation is MineShaft shaft && Conf.MineInfoGui)
            {
                MineHud.DrawMineGui(shaft);
            }
            if (Conf.GiftInformation)
            {
                GiftInformationTooltip.DrawTooltip();
            }
            if (Conf.FishingProbabilitiesInfo && Game1.player.CurrentTool is FishingRod rod && rod.isFishing)
            {
                FishingProbabilitiesBox.PrintFishingInfo();
            }
            if (Conf.PauseWhenIdle)
            {
                IdlePause.DrawHud();
            }
        }

        public void OnPostRenderGui(object sender, RenderedActiveMenuEventArgs args)
        {
            if (Game1.activeClickableMenu is BobberBar bar)
            {
                if (Conf.FishingInfo)
                {
                    FishInformationHud.DrawFishingInfoBox(Game1.spriteBatch, bar, Game1.dialogueFont);
                }
                if (Conf.AutoFishing)
                {
                    AutoFisher.AutoFishing(bar);
                }
            }
            if (Conf.EstimateShippingPrice && Game1.activeClickableMenu is ItemGrabMenu menu)
            {
                ShippingEstimationInfoBox.DrawShippingPrice(menu, Game1.dialogueFont);
            }
        }
    }
}
