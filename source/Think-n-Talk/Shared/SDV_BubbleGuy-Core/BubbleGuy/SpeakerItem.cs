/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/

using System;


namespace SDV_Speaker.Speaker
{
 public   class SpeakerItem : IEquatable<SpeakerItem>
    {
        private int iTileX;
        private int iTileY;

        public SpeakerItem() { }
        public SpeakerItem(string details)
        {

            string[] arSplit = details.Split(']');
            if (arSplit.Length == 2)
            {
                string[] arItems = arSplit[0].Split(',');
                if (arItems.Length == 3)
                {
                    Location = arItems[0].Substring(1);
                    int.TryParse(arItems[1], out iTileX);
                    int.TryParse(arItems[2], out iTileY);
                }
                arItems = arSplit[1].Substring(1).Split(' ');
                if (arItems.Length > 2)
                {
                    Text = arSplit[1].Replace(arItems[0] + " ", "").Trim();
                    IsThink = arItems[0] == "think";
                }
                else if (arItems.Length == 1)
                {
                    IsClear = arItems[0] == "clear";
                }
            }
        }
        public string Location { get; set; }
        public int TileX { get => iTileX; set => iTileX = value; }
        public int TileY { get => iTileY; set=> iTileY = value; }
        public string Text { get; set; }
        public bool IsThink { get; set; }
        public bool IsClear { get; set; }
        public bool MarkHit { get; set; }
        public override int GetHashCode()
        {
            return ($"{Location}{TileX}{TileY}{Text}{IsThink}{IsClear}").GetHashCode();
        }

        public override string ToString()
        {
            return $"[{Location},{TileX},{TileY}] {(IsThink ? "think" : IsClear ? "clear" : "talk")} {(IsClear ? "" : Text)}";
        }
        public override bool Equals(object obj) => this.Equals(obj as SpeakerItem);

        public bool Equals(SpeakerItem other)
        {
            if (other is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != other.GetType())
            {
                return false;
            }
            return Text == other.Text && TileX == other.TileX && TileY == other.TileY && IsThink == other.IsThink && Location == other.Location;
        }

    }
}
