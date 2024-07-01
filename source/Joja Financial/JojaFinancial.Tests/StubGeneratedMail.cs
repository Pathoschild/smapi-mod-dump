/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/JojaFinancial
**
*************************************************/

using System;
using StardewValley;
using StardewValleyMods.JojaFinancial;

namespace JojaFinancial.Tests
{
    public class StubGeneratedMail
        : GeneratedMail
    {
        public record MailItem(string IdPrefix, string Synopsis, string Message, WorldDate SentDate, (string qiid, int count)[] attachedItems);

        public List<MailItem> SentMail = new();

        public override void SendMail(string idPrefix, string synopsis, string message, params (string qiid, int count)[] attachedItems)
        {
            this.SentMail.Add(new MailItem(idPrefix, synopsis, message, this.Mod.Game1.Date, attachedItems));
        }

        public MailItem EnsureSingleMatchingItemWasDelivered(Func<MailItem, bool> predicate, string messageDescription)
        {
            var result = this.SentMail.FirstOrDefault(predicate);
            if (result is null)
            {
                var otherMail = this.SentMail.FirstOrDefault();
                if (otherMail is not null)
                {
                    Assert.Fail($"{messageDescription} was not sent, but this was: {otherMail.IdPrefix}|{otherMail.Synopsis}\r\n{otherMail.Message}");
                }
                else
                {
                    Assert.Fail($"{messageDescription} was not sent");
                }
            }
            Assert.IsNotNull(result, $"{messageDescription} was not sent");
            this.SentMail.Remove(result);
            Assert.IsFalse(this.SentMail.Any(predicate), $"More than one {messageDescription} mail was sent");
            return result;
        }

        public void AssertNoMoreMail()
        {
            var example = this.SentMail.FirstOrDefault();
            Assert.IsNull(example, $"Mail was sent when it shouldn't have been: {example?.IdPrefix}|{example?.Synopsis}\r\n{example?.Message}");
        }
    }
}
