/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miketweaver/DailyNews
**
*************************************************/

using System;

namespace DailyNews
{
    public class Headline
    {
        public string HeadlineText { get; }
        public string Texture { get; }
        public string Source { get; }

        public Headline(string headlineText, string texture, string source)
        {
            this.HeadlineText = headlineText;
            this.Texture = texture;
            this.Source = source;
        }
    }
}