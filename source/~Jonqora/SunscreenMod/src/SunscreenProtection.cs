using StardewModdingAPI;
using StardewValley;

namespace SunscreenMod
{
    /// <summary>Stores and manipulates data about a player's sunscreen application.</summary>
    public class SunscreenProtection
    {
        static IModHelper Helper => ModEntry.Instance.Helper;
        static IMonitor Monitor => ModEntry.Instance.Monitor;
        static ModConfig Config => ModConfig.Instance;


        static readonly ITranslationHelper i18n = Helper.Translation;

        /// <summary>If player was swimming when last checked.</summary>
        bool PlayerWasSwimming = false;

        /// <summary>The time that sunscreen was last applied.</summary>
        public SDVTime TimeOfApplication = null;

        /// <summary>The time that the sunscreen will wear off.</summary>
        public SDVTime TimeOfExpiry => GetExpiryTime();

        /// <summary>Returns the time when the applied sunscreen will wear off.</summary>
        SDVTime GetExpiryTime()
        {
            if (TimeOfApplication == null) return null;
            SDVTime expiry = new SDVTime(TimeOfApplication);
            expiry.AddMinutes(SDVTime.MINPERHR * Config.SunscreenDuration);
            return expiry;
        }

        /// <summary>Apply (or reapply) suncreen protection.</summary>
        public void ApplySunscreen(SDVTime time = null)
        {
            if (time == null) time = SDVTime.CurrentTime;
            TimeOfApplication = time;
        }

        /// <summary>Remove all suncreen protection.</summary>
        public void RemoveSunscreen()
        {
            TimeOfApplication = null;
        }

        /// <summary>Check if new suncreen was applied within the last 30 minutes.</summary>
        public bool AppliedSunscreenRecently()
        {
            SDVTime now = SDVTime.CurrentTime;
            SDVTime last30Mins = new SDVTime(now);
            last30Mins.AddMinutes(-30);
            if (IsProtected() &&
                TimeOfApplication > last30Mins)
            {
                return true;
            }
            return false;
        }

        /// <summary>Check if sunscreen protection is active at the current time.</summary>
        public bool IsProtected()
        {
            if (TimeOfApplication != null)
            {
                SDVTime time = SDVTime.CurrentTime;
                if (time >= TimeOfApplication && time < TimeOfExpiry) return true;
            }
            return false;
        }

        /// <summary>Remove protection (and display an HUD message) if sunscreen has recently expired or washed off.</summary>
        public void UpdateStatus()
        {
            if (TimeOfApplication != null)
            {
                if (!IsProtected()) //Sunscreen has worn off
                {
                    RemoveSunscreen();
                    string messagetext = i18n.Get("Sunscreen.WornOff");
                    Game1.addHUDMessage(new HUDMessage(messagetext, 3)); //Red X message type
                    Monitor.Log($"Sunscreen status: {messagetext}", LogLevel.Info);
                }
                else if (!Game1.player.swimming.Value && PlayerWasSwimming) //Sunscreen washed off in the water
                {
                    RemoveSunscreen();
                    string messagetext = i18n.Get("Sunscreen.WashedOff");
                    Game1.addHUDMessage(new HUDMessage(messagetext, 3)); //Red X message type
                    Monitor.Log($"Sunscreen status: {messagetext}", LogLevel.Info);
                }
            }
            PlayerWasSwimming = Game1.player.swimming.Value;
        }
    }
}