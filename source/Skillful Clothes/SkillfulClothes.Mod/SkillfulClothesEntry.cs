/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SkillfulClothes.Configuration;
using SkillfulClothes.Effects;
using SkillfulClothes.Patches;
using SkillfulClothes.Types;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace SkillfulClothes
{
    /** Effect Ideas
     * 
     * [x] Increase Max Health
     * [x] Increase Max Energy (= Stamina)
     * [x] Attack + 
     * [x] Defense +  (= Resilience)
     * [x] Skill + 
     * [x] Immunity +
     * [x] Move faster (Speed +)
     * Weapon Speed + 
     * [x] Experience Multiplicator
     * [x] Save from death (consume shirt)
     * [x] Health regen when not moving
     * [x] Energy regen when not moving     
     * No Chain Damage
     * [x] Increase size of fishing bar based on number of fish caught
     * 
     **/

    public class SkillfulClothesEntry : Mod
    {

        ClothingObserver clothingObserver;

        public override void Entry(IModHelper helper)
        {
            var config = helper.ReadConfig<SkillfulClothesConfig>();
            Logger.Init(this.Monitor, config.verboseLogging);
            EffectHelper.Init(helper, config);

            if (EffectHelper.Config.LoadCustomEffectDefinitions)
            {
                CustomEffectDefinitions.LoadCustomEffectDefinitions();
            }

            HarmonyPatches.Apply(this.ModManifest.UniqueID);
            ShopPatches.Apply(helper);
            TailoringPatches.Apply(helper);
            ClothingTextPatches.Apply(helper);            


            clothingObserver = EffectHelper.ClothingObserver;

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;

            helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;            
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {             
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            clothingObserver.Reset(Game1.player);            
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            clothingObserver.Restore(Game1.player, EffectChangeReason.DayStart);
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            clothingObserver.Suspend(Game1.player, EffectChangeReason.DayEnd);
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;            
            
            clothingObserver.Update(Game1.player);            
        }
    }
}
