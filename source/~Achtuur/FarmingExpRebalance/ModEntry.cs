/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Events;
using AchtuurCore.Patches;
using FarmingExpRebalance.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace FarmingExpRebalance
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance;
        public const int FarmingSkillID = 0;

        internal ModConfig Config;
        private float wateringExpTotal;

        public override void Entry(IModHelper helper)
        {

            HarmonyPatcher.ApplyPatches(this,
                new CropHarvestPatcher()
            );

            I18n.Init(helper.Translation);
            ModEntry.Instance = this;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.wateringExpTotal = 0f;

            AchtuurCore.Events.EventPublisher.onFinishedWateringSoil += OnFinishedWateringSoil;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.Config.createMenu(this);
        }

        private void OnFinishedWateringSoil(object sender, WateringFinishedArgs e)
        {
            // Quit if world is not loaded
            if (!Context.IsWorldReady)
                return;


            Instance.Monitor.Log($"{e.farmer.Name} {e.farmer.IsLocalPlayer}", LogLevel.Debug);

            // Only add exp if farmer who watered is current player
            if (!e.farmer.IsLocalPlayer)
                return;

            Instance.Monitor.Log("hello", LogLevel.Debug);
            // Add exp
            this.wateringExpTotal += ModConfig.ExpforWateringSoil;
            int floored_total = (int) Math.Floor(wateringExpTotal);
            e.farmer.gainExperience(FarmingSkillID, floored_total);
            this.wateringExpTotal -= floored_total;
        }
    }
}
