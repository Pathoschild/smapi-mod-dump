/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace NPCMapLocations.Framework
{
    // Class for map markers
    public class NpcMarker
    {
        public string DisplayName { get; set; }
        public string LocationName { get; set; }
        public Texture2D Sprite { get; set; }
        public int CropOffset { get; set; }
        public int MapX { get; set; }
        public int MapY { get; set; }
        public bool IsBirthday { get; set; }
        public CharacterType Type { get; set; }
        public bool HasQuest { get; set; }
        public bool IsHidden { get; set; }
        public int Layer { get; set; }

        public NpcMarker()
        {
            this.DisplayName = null;
            this.LocationName = null;
            this.Sprite = null;
            this.CropOffset = 0;
            this.MapX = -9999;
            this.MapY = -9999;
            this.IsBirthday = false;
            this.HasQuest = false;
            this.IsHidden = false;
            this.Layer = 4;
            this.Type = CharacterType.Villager;
        }
    }
}
