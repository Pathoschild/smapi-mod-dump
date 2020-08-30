using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunscreenMod
{
    public class SunscreenProtection
    {
        protected static IModHelper Helper => ModEntry.Instance.Helper;
        protected static IMonitor Monitor => ModEntry.Instance.Monitor;
        private static ModConfig Config => ModConfig.Instance;


        protected static ITranslationHelper i18n = Helper.Translation;

        bool PlayerWasSwimming = false;

        private SDVTime TimeOfApplication = null;

        private SDVTime TimeOfExpiry => GetExpiryTime();

        SDVTime GetExpiryTime()
        {
            if (TimeOfApplication == null) return null;
            SDVTime expiry = new SDVTime(TimeOfApplication);
            expiry.AddMinutes(SDVTime.MINPERHR * Config.SunscreenDuration);
            return expiry;
        }

        public void ApplySunscreen(SDVTime time = null)
        {
            if (time == null) time = SDVTime.CurrentTime;
            TimeOfApplication = time;
        }

        public void RemoveSunscreen()
        {
            TimeOfApplication = null;
        }

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

        public bool IsProtected()
        {
            if (TimeOfApplication != null)
            {
                SDVTime time = SDVTime.CurrentTime;
                if (time >= TimeOfApplication && time < TimeOfExpiry) return true;
            }
            return false;
        }

        public void UpdateStatus()
        {
            if (TimeOfApplication != null)
            {
                if (!IsProtected()) //Sunscreen has worn off
                {
                    Game1.addHUDMessage(new HUDMessage(i18n.Get("Sunscreen.WornOff"), 3)); //Exclamation mark message type
                    RemoveSunscreen();
                }
                else if (!Game1.player.swimming.Value && PlayerWasSwimming) //Sunscreen washed off in the water
                {
                    Game1.addHUDMessage(new HUDMessage(i18n.Get("Sunscreen.WashedOff"), 3)); //Red X message type
                    RemoveSunscreen();
                }
            }
            PlayerWasSwimming = Game1.player.swimming.Value;
        }
    }
}
