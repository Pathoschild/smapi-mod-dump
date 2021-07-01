/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/WhiteMinds/mod-sv-autofish
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace AutoFish
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private bool catching = false;

        public override void Entry(IModHelper helper)
        {
            Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            Farmer player = Game1.player;
            if (!Context.IsWorldReady || player == null)
                return;

            if (player.CurrentTool is FishingRod currentTool)
            {
                if (Config.fastBite && currentTool.timeUntilFishingBite > 0)
                    currentTool.timeUntilFishingBite /= 2; // 快速咬钩

                if (Config.autoHit && currentTool.isNibbling && !currentTool.isReeling && !currentTool.hit && !currentTool.pullingOutOfWater && !currentTool.fishCaught)
                    currentTool.DoFunction(player.currentLocation, 1, 1, 1, player); // 自动咬钩

                if (Config.maxCastPower)
                    currentTool.castingPower = 1;
            }

            if (Game1.activeClickableMenu is BobberBar bar) // 自动小游戏
            {
                float barPos = Helper.Reflection.GetField<float>(bar, "bobberBarPos").GetValue();
                float barHeight = Helper.Reflection.GetField<int>(bar, "bobberBarHeight").GetValue();
                float fishPos = Helper.Reflection.GetField<float>(bar, "bobberPosition").GetValue();
                float treasurePos = Helper.Reflection.GetField<float>(bar, "treasurePosition").GetValue();
                float distanceFromCatching = Helper.Reflection.GetField<float>(bar, "distanceFromCatching").GetValue();

                bool treasureCaught = Helper.Reflection.GetField<bool>(bar, "treasureCaught").GetValue();
                bool hasTreasure = Helper.Reflection.GetField<bool>(bar, "treasure").GetValue();
                float bobberBarSpeed = Helper.Reflection.GetField<float>(bar, "bobberBarSpeed").GetValue();
                float barPosMax = 568 - barHeight;

                float min = barPos + barHeight / 4,
                    max = barPos + barHeight / 1.5f;

                if (Config.catchTreasure && hasTreasure && !treasureCaught && (distanceFromCatching > 0.75 || catching))
                {
                    catching = true;
                    fishPos = treasurePos;
                }
                if (catching && distanceFromCatching < 0.15)
                {
                    catching = false;
                    fishPos = Helper.Reflection.GetField<float>(bar, "bobberPosition").GetValue();
                }

                if (fishPos < min)
                {
                    bobberBarSpeed -= 0.35f + (min - fishPos) / 20;
                    Helper.Reflection.GetField<float>(bar, "bobberBarSpeed").SetValue(bobberBarSpeed);
                }
                else if (fishPos > max)
                {
                    bobberBarSpeed += 0.35f + (fishPos - max) / 20;
                    Helper.Reflection.GetField<float>(bar, "bobberBarSpeed").SetValue(bobberBarSpeed);
                }
                else
                {
                    float target = 0.1f;
                    if (bobberBarSpeed > target)
                    {
                        bobberBarSpeed -= 0.1f + (bobberBarSpeed - target) / 25;
                        if (barPos + bobberBarSpeed > barPosMax)
                            bobberBarSpeed /= 2; // 减小触底反弹
                        if (bobberBarSpeed < target)
                            bobberBarSpeed = target;
                    }
                    else
                    {
                        bobberBarSpeed += 0.1f + (target - bobberBarSpeed) / 25;
                        if (barPos + bobberBarSpeed < 0)
                            bobberBarSpeed /= 2; // 减小触顶反弹
                        if (bobberBarSpeed > target)
                            bobberBarSpeed = target;
                    }
                    Helper.Reflection.GetField<float>(bar, "bobberBarSpeed").SetValue(bobberBarSpeed);
                }
            }
            else
            {
                catching = false;
            }
        }
    }
}
