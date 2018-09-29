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