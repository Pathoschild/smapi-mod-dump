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

namespace DearDiary
{
    class Mod : StardewModdingAPI.Mod
    {

        public override void Entry(IModHelper helper)
        {
           
            helper.Events.Input.ButtonReleased += OnButtonReleased;
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button == SButton.F5)
            {
                if (Game1.activeClickableMenu == null)
                    Game1.activeClickableMenu = new JournalMenu(this.Helper.Data, this.Helper.DirectoryPath);
                else if (Game1.activeClickableMenu is JournalMenu)
                    Game1.exitActiveMenu();
            }
        }
    }
}