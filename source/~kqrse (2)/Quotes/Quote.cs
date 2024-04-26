/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quotes
{
    public class Quote
    {
        public string quote;
        public List<string> quoteLines = new List<string>();
        public string author;

        public Quote(string quotestring)
        {
            if (quotestring == null)
                return;
            string[] split = quotestring.Split('^');
            this.quote = split[0];
            if(split.Length > 1)
                this.author = split[1];
            var lineLength = ModEntry.Config.QuoteCharPerLine;
            var words = quote.Split(' ');
            var line = "";
            for(int i = 0; i < words.Length; i++)
            {
                if(words[i].Length > lineLength)
                {
                    quoteLines.Add(words[i].Substring(0, lineLength));
                    words[i] = words[i].Substring(lineLength);
                    i--;
                    continue;
                }
                line += (line.Length == 0 ? "" : " ") + words[i];
                if (line.Length >= lineLength)
                {
                    quoteLines.Add(line);
                    line = "";
                }
            }
            if(line.Length > 0)
                quoteLines.Add(line);
        }
    }
}