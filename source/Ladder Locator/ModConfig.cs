/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/yuri-moens/LadderLocator
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace LadderLocator
{
    class ModConfig
    {
        public KeybindList ToggleShaftsKey { get; set; } = KeybindList.Parse("OemTilde");
        public bool ForceShafts { get; set; } = false;
        public KeybindList ToggleHighlightTypeKey { get; set; } = KeybindList.Parse("LeftShift + OemTilde");
        public HashSet<HighlightType> HighlightTypes { get; set; } = new HashSet<HighlightType>() { HighlightType.Rectangle };
        public Color HighlightRectangleRGBA { get; set; } = Color.Lime;
        public string HighlightImageFilename { get; set; } = "cracked.png";
        public decimal HighlightAlpha { get; set; } = 0.45M;
        public KeybindList CycleAlpha { get; set; } = KeybindList.Parse("LeftAlt + OemTilde");
        public bool HighlightUsesStoneTint { get; set; } = false;
        public KeybindList ToggleTint { get; set; } = KeybindList.Parse("LeftControl + OemTilde");
        public bool NodeRadar { get; set; } = false;
        public KeybindList ToggleNodeRadar { get; set; } = KeybindList.Parse("LeftControl + LeftShift + OemTilde");
        public decimal RadarScale { get; set; } = 1.0M;
        public HashSet<Node> NodeTypes { get; set; } = new HashSet<Node>((Node[])System.Enum.GetValues(typeof(Node)));
    }

    enum HighlightType
    {
        Rectangle,
        Image,
        Sprite
    }

    enum Node
    {
        Topaz,
        Amethyst,
        Aquamarine,
        Jade,
        Emerald,
        Ruby,
        Diamond,
        Gem,
        Mystic,
        Geode,
        FrozenGeode,
        MagmaGeode,
        OmniGeode,
        CinderShard,
        Copper,
        Iron,
        Gold,
        Iridium,
        Radioactive
    }

    class RadarNode
    {
        public static List<RadarNode> All { get; } = new List<RadarNode>() {
            new RadarNode(Node.Topaz, 8, 80),
            new RadarNode(Node.Amethyst, 10, 100),
            new RadarNode(Node.Aquamarine, 14, 180),
            new RadarNode(Node.Jade, 6, 200),
            new RadarNode(Node.Emerald, 12, 250),
            new RadarNode(Node.Ruby, 4, 250),
            new RadarNode(Node.Diamond, 2, 750),
            new RadarNode(Node.Gem, 44, 1000),
            new RadarNode(Node.Mystic, 26, 1500),
            // Give these an offset value to maintain grouping, hence -1000
            new RadarNode(Node.Geode, 75, 50 - 1000),
            new RadarNode(Node.FrozenGeode, 76, 100 - 1000),
            new RadarNode(Node.MagmaGeode, 77, 150 - 1000),
            new RadarNode(Node.OmniGeode, 819, 200 - 1000),
            // These are somewhere between rare gem and rare ore, hence +1500
            new RadarNode(Node.CinderShard, 843, 50 + 1500),
            new RadarNode(Node.CinderShard, 844, 50 + 1500),
            // Give these an offset value to maintain grouping, hence +2000
            new RadarNode(Node.Copper, 751, 5 + 2000),
            new RadarNode(Node.Copper, 849, 5 + 2000),
            new RadarNode(Node.Iron, 290, 10 + 2000),
            new RadarNode(Node.Iron, 850, 10 + 2000),
            new RadarNode(Node.Gold, 764, 25 + 2000),
            new RadarNode(Node.Iridium, 765, 100 + 2000),
            new RadarNode(Node.Radioactive, 95, 300 + 2000)
        };

        public RadarNode(Node type, int index, int value)
        {
            this.type = type;
            spriteIndex = index;
            this.value = value;
        }

        public Node type { get; }
        public int spriteIndex { get; }
        public int value { get; }
    }
}
