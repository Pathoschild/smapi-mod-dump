/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;

namespace Outerwear
{
    /// <summary>Responsible for applying all the effects the passed outerwear data has.</summary>
    public static class OuterwearEffectsApplier
    {
        /*********
        ** Fields
        *********/
        /// <summary>The object id of the outerwear whose buff is currently active.</summary>
        /// <remarks>This is used to tell when an outerwear has been changed if it hasn't been set to <see langword="null"/> first, so the buff can be reset correctly.</remarks>
        private static int CurrentBuffObjectId;


        /*********
        ** Accessors
        *********/
        /// <summary>The id of the buff created from an equipped outerwear.</summary>
        private static int BuffId => 234554345;

        /// <summary>The id of the light source created from an equipped outerwear.</summary>
        private static int LightSourceId => 736576353 + (int)Game1.player.UniqueMultiplayerID;


        /*********
        ** Public Methods
        *********/
        /// <summary>Updates the outerwear effects.</summary>
        /// <param name="isOneSecond">Whether <see cref="StardewModdingAPI.Events.UpdateTickedEventArgs.Ticks"/> is a multiple of 60, which happens approximately once per second.</param>
        /// <remarks>This is invoked once per tick.</remarks>
        public static void Update(bool isOneSecond)
        {
            // ignore in cutscenes
            if (Game1.eventUp || !Context.IsWorldReady)
                return;

            // get the equipped outerwear
            var equippedOuterwear = ModEntry.Instance.Api.GetEquippedOuterwear();
            if (equippedOuterwear == null)
            {
                RemoveLightSource();
                return;
            }
            var equippedOuterwearData = ModEntry.Instance.Api.GetOuterwearData(equippedOuterwear.ParentSheetIndex);
            if (equippedOuterwearData == null) // this will be the case if the player has uninstalled a content pack while wearing one of it's outerwear
                return;

            // add or update buff
            var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(buff => buff.which == BuffId);

            if (equippedOuterwearData.ObjectId != CurrentBuffObjectId)
            {
                Game1.buffsDisplay.removeOtherBuff(BuffId);
                buff = null;
            }

            if (buff == null)
            {
                buff = new Buff(
                    farming: equippedOuterwearData.Effects.FarmingIncrease,
                    fishing: equippedOuterwearData.Effects.FishingIncrease,
                    mining: equippedOuterwearData.Effects.MiningIncrease, 0, 0,
                    foraging: equippedOuterwearData.Effects.ForagingIncrease, 0,
                    maxStamina: equippedOuterwearData.Effects.MaxStaminaIncrease,
                    magneticRadius: equippedOuterwearData.Effects.MagneticRadiusIncrease,
                    speed: equippedOuterwearData.Effects.SpeedIncrease,
                    defense: equippedOuterwearData.Effects.DefenceIncrease,
                    attack: equippedOuterwearData.Effects.AttackIncrease,
                    minutesDuration: 1, source: "Outerwear", displaySource: "Outerwear")
                { which = BuffId };
                buff.buffAttributes[(int)CustomBuff.CombatSkill] = equippedOuterwearData.Effects.CombatIncrease;
                buff.buffAttributes[(int)CustomBuff.CriticalChance] = equippedOuterwearData.Effects.CriticalChanceIncrease;
                buff.buffAttributes[(int)CustomBuff.CriticalPower] = equippedOuterwearData.Effects.CriticalPowerIncrease;
                buff.buffAttributes[(int)CustomBuff.MaxHealth] = equippedOuterwearData.Effects.MaxHealthIncrease;
                buff.buffAttributes[(int)CustomBuff.HealthRegeneration] = equippedOuterwearData.Effects.HealthRegeneration;
                buff.buffAttributes[(int)CustomBuff.StaminaRegeneration] = equippedOuterwearData.Effects.StaminaRegeneration;

                Game1.buffsDisplay.addOtherBuff(buff);
            }

            buff.millisecondsDuration = 50;
            CurrentBuffObjectId = equippedOuterwearData.ObjectId;

            // apply regeneration
            if (isOneSecond)
            {
                Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + equippedOuterwearData.Effects.HealthRegeneration);
                Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.stamina + equippedOuterwearData.Effects.StaminaRegeneration);
            }

            // add or update light emission
            if (equippedOuterwearData.Effects.EmitsLight)
            {
                // add light if it doesn't exist
                if (!Game1.currentLocation.sharedLights.ContainsKey(LightSourceId))
                    Game1.currentLocation.sharedLights[LightSourceId] = new LightSource(1, Game1.player.Position, equippedOuterwearData.Effects.LightRadius, new Color(0, 80, 0), playerID: Game1.player.UniqueMultiplayerID);

                // update light position to follow player
                var offset = new Vector2(21, 0);
                if (Game1.player.shouldShadowBeOffset)
                    offset += Game1.player.drawOffset.Value;
                Game1.currentLocation.repositionLightSource(LightSourceId, Game1.player.Position + offset);
            }
            else
                RemoveLightSource();
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Removes the light source.</summary>
        private static void RemoveLightSource()
        {
            if (Game1.currentLocation.sharedLights.ContainsKey(LightSourceId))
                Game1.currentLocation.sharedLights.Remove(LightSourceId);
        }
    }
}
