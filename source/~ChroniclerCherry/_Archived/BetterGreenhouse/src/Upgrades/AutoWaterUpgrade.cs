/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace GreenhouseUpgrades.Upgrades
{
    public class AutoWaterUpgrade : Upgrade
    {
        public override UpgradeTypes Type => UpgradeTypes.AutoWaterUpgrade;
        public override bool Active { get; set; }
        public override bool Unlocked { get; set; } = false;
        public override bool DisableOnFarmhand { get; } = true;

        public override void Start()
        {
            base.Start();
            Helper.Events.GameLoop.DayStarted += WaterAllGreenhouseDayStart;
            Helper.Events.GameLoop.DayEnding += WaterAllGreenhouseDayEnd;
        }

        public override void Stop()
        {
            Active = false;
            Helper.Events.GameLoop.DayStarted -= WaterAllGreenhouseDayStart;
            Helper.Events.GameLoop.DayEnding -= WaterAllGreenhouseDayEnd;
        }

        private void WaterAllGreenhouseDayStart(object sender, DayStartedEventArgs e)
        {
            WaterGreenhouse();
        }

        private void WaterAllGreenhouseDayEnd(object sender, DayEndingEventArgs e)
        {
            WaterGreenhouse();
        }

        private void WaterGreenhouse()
        {
            if (!Unlocked || !Active) return;
            Monitor.Log($"{TranslatedName} : Watering the greenhouse");

            foreach (var loc in Game1.locations)
            {
                if (!loc.IsGreenhouse) continue;

                foreach (var feature in loc.terrainFeatures.Values)
                {
                    if (feature is HoeDirt dirt)
                    {
                        dirt.state.Value = HoeDirt.watered;
                    }
                }

                //stole this from CJB cheats lol
                foreach (var pot in loc.objects.Values.OfType<IndoorPot>())
                {
                    var dirt = pot.hoeDirt.Value;
                    if (dirt?.crop == null) continue;

                    dirt.state.Value = HoeDirt.watered;
                    pot.showNextIndex.Value = true;
                }

            }
        }
    }
}
