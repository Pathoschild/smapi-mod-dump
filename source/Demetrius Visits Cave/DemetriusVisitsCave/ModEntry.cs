using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace DemetriusVisitsCave
{
    public class ModConfig
    {
        public string day { get; set; } = "Tuesday";
        public bool avoidRain { get; set; } = true;
        public bool modDialog { get; set; } = true;
        public int locx { get; set; } = 3;
        public int locy { get; set; } = 7;
        public string facing { get; set; } = "right";
    }
    public class ModEntry : Mod, IAssetEditor
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }
        // Custom dialogue
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Characters\\Dialogue\\Demetrius");
        }
        public void Edit<T>(IAssetData asset)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            bool modDialog = this.Config.modDialog;
            ITranslationHelper i18n = Helper.Translation;
            if (modDialog == true && asset.AssetNameEquals("Characters\\Dialogue\\Demetrius"))
            {
                Dictionary<int, string> caveTypes = new Dictionary<int, string>();
                caveTypes.Add(0, i18n.Get("cave0"));
                caveTypes.Add(1, i18n.Get("cave1"));
                caveTypes.Add(2, i18n.Get("cave2"));
                string greet = i18n.Get("greet");
                string research = i18n.Get("research", new { playerCave = caveTypes[Game1.player.caveChoice] });
                string thanks = i18n.Get("thanks");
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data.Add("FarmCave", $"{greet}#$b#{research}");
                data.Add("FarmCave8", $"{greet}#$b#$c 0.8#{research}#{thanks}");
            }
        }
        // Check day and send Demetrius to the caves
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            int locx = this.Config.locx;
            int locy = this.Config.locy;
            var directions = new List<string>() { "up", "right", "down", "left" };
            int facing = directions.IndexOf(this.Config.facing);
            bool avoidRain = this.Config.avoidRain;
            string day = this.Config.day;
            var date = SDate.Now();
            NPC Dima = Game1.getCharacterFromName("Demetrius", true);
            bool sched = Dima.hasMasterScheduleEntry(date.Season + "_" + date.Day.ToString());
            Point loc = new Point(locx, locy);
            if (date.DayOfWeek.ToString() == day && sched == false && (Game1.isRaining == false || avoidRain == false) && Game1.player.caveChoice != 0)
            {
                Dima.faceDirection(facing);
                Game1.warpCharacter(Dima, "FarmCave", new Point(locx, locy));
            }
        }
    }
}