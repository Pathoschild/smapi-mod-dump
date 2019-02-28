using System;
using System.Management;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BatteryWarningMod
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private bool messageShown = false;
        private bool noBattery = false;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            // check battery state
            if (!this.noBattery)
            {
                ObjectQuery query = new ObjectQuery("Select EstimatedChargeRemaining FROM Win32_Battery");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection col = searcher.Get();
                if (col.Count == 0)
                    this.noBattery = true;
                else if (!this.messageShown) //checking no Battery twice, cause of first run...
                {
                    foreach (ManagementBaseObject mo in col)
                    {
                        foreach (PropertyData property in mo.Properties)
                        {
                            if (Convert.ToInt16(property.Value) < 25)
                            {
                                Game1.drawObjectDialogue(this.Helper.Translation.Get("message"));
                                this.messageShown = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
