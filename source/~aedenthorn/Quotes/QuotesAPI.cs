/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace Quotes
{
    public class QuotesAPI
    {
        public string[] GetRandomQuoteAndAuthor(bool makeLines = false)
        {
            var quote = ModEntry.GetAQuote(true);
            if (!makeLines)
            {
                return new string[] { quote.quote, quote.author };
            }
            List<string> result = new List<string>(quote.quoteLines);
            result.Add(quote.author);
            return result.ToArray();
        }
    }
}