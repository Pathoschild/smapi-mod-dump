/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using StardewValley;
using StarterPack.Framework.Interfaces;
using System;
using System.Collections.Generic;

namespace StarterPack.Framework.Utilities.SpecialAttacks
{
    public class SpeedBuff
    {
        private static int _defaultMovementSpeedBuff = 2;
        private static int _defaultDurationInMilliseconds = 10000;
        private static int _defaultCooldownInMilliseconds = 10000;

        internal static string GetDescription(List<object> arguments)
        {
            return $"Increases your movement speed by {GetMovementSpeed(arguments)} for {GetMovementDuration(arguments) / 1000} second(s).";
        }

        internal static bool HandleSpecialAttack(ISpecialAttack specialAttack)
        {
            if (specialAttack.Farmer is null)
            {
                return false;
            }
            Game1.buffsDisplay.addOtherBuff(GetSpeedBuff(specialAttack.Arguments));

            if (specialAttack.Location is not null)
            {
                specialAttack.Location.playSound("toyPiano");
            }

            return false;
        }

        private static Buff GetSpeedBuff(List<object> arguments)
        {
            int speedBuff = 9;
            var buff = new Buff(null, GetMovementDuration(arguments), "Yoba's Divine Harp", speedBuff) { displaySource = "Yoba's Divine Harp" };

            // Set the speed buff to +2
            buff.buffAttributes[speedBuff] = GetMovementSpeed(arguments);

            return buff;
        }

        private static int GetMovementSpeed(List<object> arguments)
        {
            var speed = _defaultMovementSpeedBuff;
            if (arguments is not null && arguments.Count > 0)
            {
                try
                {
                    speed = Convert.ToInt32(arguments[0]);
                }
                catch (Exception ex)
                {
                    StarterPack.monitor.LogOnce($"Failed to process buff amount argument for PeacefulEnd.Archery.StarterPack/Buff:Speed! See the log for details.", StardewModdingAPI.LogLevel.Error);
                    StarterPack.monitor.LogOnce($"Failed to process buff amount argument for PeacefulEnd.Archery.StarterPack/Buff:Speed:\n{ex}", StardewModdingAPI.LogLevel.Trace);
                }
            }

            return speed;
        }

        private static int GetMovementDuration(List<object> arguments)
        {
            var duration = _defaultDurationInMilliseconds;
            if (arguments is not null && arguments.Count > 1)
            {
                try
                {
                    duration = Convert.ToInt32(arguments[1]);
                }
                catch (Exception ex)
                {
                    StarterPack.monitor.LogOnce($"Failed to process duration argument for PeacefulEnd.Archery.StarterPack/Buff:Speed! See the log for details.", StardewModdingAPI.LogLevel.Error);
                    StarterPack.monitor.LogOnce($"Failed to process duration argument for PeacefulEnd.Archery.StarterPack/Buff:Speed:\n{ex}", StardewModdingAPI.LogLevel.Trace);
                }
            }

            return duration;
        }

        internal static int GetCooldown(List<object> arguments)
        {
            var duration = _defaultCooldownInMilliseconds;
            if (arguments is not null && arguments.Count > 2)
            {
                try
                {
                    duration = Convert.ToInt32(arguments[2]);
                }
                catch (Exception ex)
                {
                    StarterPack.monitor.LogOnce($"Failed to process cooldown argument for PeacefulEnd.Archery.StarterPack/Buff:Speed! See the log for details.", StardewModdingAPI.LogLevel.Error);
                    StarterPack.monitor.LogOnce($"Failed to process cooldown argument for PeacefulEnd.Archery.StarterPack/Buff:Speed:\n{ex}", StardewModdingAPI.LogLevel.Trace);
                }
            }

            return duration;
        }
    }
}
