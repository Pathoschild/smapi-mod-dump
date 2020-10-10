/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Dear-Diary
**
*************************************************/

using StardewJournal.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Newtonsoft.Json;

namespace DearDiary
{
    class Mod : StardewModdingAPI.Mod
    {
        private configModel Config;
        internal static IMonitor TempMonitor;
       
        

        public override void Entry(IModHelper helper)
        {
            TempMonitor = this.Monitor;
            helper.Events.Input.ButtonReleased += OnButtonReleased;
            this.Config = helper.ReadConfig<configModel>();
        }
        public void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            
            if (e.Button == this.Config.journalKey)
            {
                if (Game1.activeClickableMenu == null)
                    Game1.activeClickableMenu = new JournalMenu(this.Helper.Data, this.Helper.DirectoryPath);
               /* else if (Game1.activeClickableMenu is JournalMenu)
                    Game1.exitActiveMenu();*/
            }
        }
    }
}