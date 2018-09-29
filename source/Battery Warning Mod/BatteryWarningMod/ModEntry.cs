using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Management;
using System.IO;

namespace BatteryWarningMod
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private bool messageShown = false;
        private bool noBattery = false;
        private ITranslationHelper translations = null;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {

            TimeEvents.TimeOfDayChanged += this.CheckBatteryState;
            this.translations = this.Helper.Translation;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        private void CheckBatteryState(object sender, EventArgs e)
        {
            if (this.noBattery == false) { 
                ObjectQuery query = new ObjectQuery("Select EstimatedChargeRemaining FROM Win32_Battery");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection col = searcher.Get();
                if(col.Count == 0)
                {
                    this.noBattery = true;
                }
                if(this.noBattery == false && this.messageShown == false) //checking no Battery twice, cause of first run...
                {
                    foreach (ManagementObject mo in col)
                    {
                        foreach (PropertyData property in mo.Properties)
                        {
                            if(Convert.ToInt16(property.Value) < 25)
                            {
                                Game1.drawObjectDialogue(this.translations.Get("message"));
                                this.messageShown = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
