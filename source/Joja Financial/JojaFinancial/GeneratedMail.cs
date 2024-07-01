/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/JojaFinancial
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

using static NermNermNerm.Stardew.LocalizeFromSource.SdvLocalize;

namespace StardewValleyMods.JojaFinancial
{
    public class GeneratedMail : ISimpleLog
    {
        public ModEntry Mod { get; private set; } = null!;

        private const string MailModDataPrefix = "JojaFinancial.Mail.";

        public void Entry(ModEntry mod)
        {
            this.Mod = mod;

            this.Mod.Helper.Events.Content.AssetRequested += this.Content_AssetRequested;
        }

        private void Content_AssetRequested(object? sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit(editor =>
                {
                    IDictionary<string, string> data = editor.AsDictionary<string, string>().Data;
                    this.AddMailKeys(data);
                });
            }
        }

        private void AddMailKeys(IDictionary<string, string> data)
        {
            foreach (var pair in Game1.player.modData.Pairs)
            {
                if (pair.Key.StartsWith(MailModDataPrefix))
                {
                    data[pair.Key.Substring(MailModDataPrefix.Length+1)] = pair.Value;
                }
            }
        }

        public virtual void SendMail(string idPrefix, string synopsis, string message, params (string qiid, int count)[] attachedItems)
        {
            string mailKey = $"{idPrefix}.{Game1.Date.Year}.{Game1.Date.SeasonIndex}.{Game1.Date.DayOfMonth}";
            // [letterbg 4] adds the joja letterhead
            string value = I("[letterbg 4]") + message.Replace("\r", "").Replace("\n", "^");

            foreach (var pair in attachedItems)
            {
                value += IF($"%item id {pair.qiid} {pair.count}%%");
            }
            value += "[#]" + synopsis;
            Game1.player.modData[IF($"{MailModDataPrefix}.{mailKey}")] = value;
            this.Mod.Helper.GameContent.InvalidateCache("Data/Mail");
            Game1.player.mailForTomorrow.Add(mailKey);
        }

        public void SendMail(string idPrefix, string synopsis, string message, params string[] attachedItemQiids)
        {
            this.SendMail(idPrefix, synopsis, message, attachedItemQiids.Select(id => (id, 1)).ToArray());
        }

        public void WriteToLog(string message, LogLevel level, bool isOnceOnly)
        {
            this.Mod.WriteToLog(message, level, isOnceOnly);
        }
    }
}
