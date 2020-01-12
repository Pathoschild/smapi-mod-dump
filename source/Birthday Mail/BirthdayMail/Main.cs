using System;
using StardewModdingAPI;
using StardewValley;

namespace BirthdayMail
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            // subscribe to events in StardewModdingAPI
            helper.Events.GameLoop.DayStarted += BirthdayMail;
        }

        // checks for birthdays and sends mail if needed
        private void BirthdayMail(object sender, EventArgs e)
        {
            // test if today is someone's birthday...
            if (Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth) != null)
            {
                // ... if so, set birthday NPC and their mail item
                NPC birthdayNPC = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
                string birthdayMail = birthdayNPC.Name + "Birth";

                if (!Game1.mailbox.Contains(birthdayMail)) 
                    Game1.mailbox.Add(birthdayMail);
            }
        }
    }
}
