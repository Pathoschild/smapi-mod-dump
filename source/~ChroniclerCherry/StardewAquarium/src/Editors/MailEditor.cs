/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace StardewAquarium.Editors
{
    class MailEditor
    {
        private IModHelper _helper;
        private const string AquariumOpenAfterLandslide = "StardewAquarium.Open";
        private const string AquariumOpenLater = "StardewAquarium.OpenLater";

        public MailEditor(IModHelper helper)
        {
            this._helper = helper;
            this._helper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (Game1.player.hasOrWillReceiveMail(AquariumOpenAfterLandslide) || Game1.player.hasOrWillReceiveMail(AquariumOpenLater))
                return;

            if (Game1.Date.TotalDays == 30)
                Game1.player.mailbox.Add(AquariumOpenAfterLandslide);

            if (Game1.Date.TotalDays > 30)
                Game1.player.mailbox.Add(AquariumOpenLater);
        }

        public bool CanEdit(IAssetName asset)
        {
            return asset.IsEquivalentTo("Data/mail");
        }

        public void Edit(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            data[AquariumOpenAfterLandslide] = this._helper.Translation.Get("AquariumOpenLandslide");
            data[AquariumOpenLater] = this._helper.Translation.Get("AquariumOPenLater");
        }
    }
}
