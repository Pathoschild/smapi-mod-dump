/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using StardewModdingAPI;

namespace CombineMachines
{
    [XmlRoot(ElementName = "Config", Namespace = "")]
    public class UserConfig
    {
        /// <summary>This property is only public for serialization purposes. Use <see cref="CreatedByVersion"/> instead.</summary>
        [XmlElement("CreatedByVersion")]
        public string CreatedByVersionString { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public Version CreatedByVersion {
            get { return string.IsNullOrEmpty(CreatedByVersionString) ? null : Version.Parse(CreatedByVersionString); }
            set { CreatedByVersionString = value == null ? null : value.ToString(); }
        }

        /// <summary>Opacity to use when drawing the combined quantity number on top of placed Objects. 0.0 = Not visible, 1.0 = Fully opaque.</summary>
        [XmlElement("NumberOpacity")]
        public float NumberOpacity { get; set; }

        /// <summary>Whether or not to draw a tooltip when hovering over a combined machine that has been placed on a tile.</summary>
        [XmlElement("DrawToolTip")]
        public bool DrawToolTip { get; set; }

        /// <summary>An positioning offset from the mouse cursor to use when drawing the tooltip that appears when hovering over a combined machine that has been placed on a tile.<para/>
        /// Mainly intended for players with other mods that display tooltips, so that the tooltips won't overlap.</summary>
        [XmlElement("ToolTipOffset")]
        public Point ToolTipOffset { get; set; }

        /// <summary>The minimum boost to processing power that will be applied when combined machines. See also: <see cref="CombinePenalty"/></summary>
        [XmlElement("MinimumEffect")]
        public double MinimumEffect { get; set; }

        /// <summary>A penalty that is applied to the processing power of a combined machine. This penalty stacks additively.<para/>
        /// For example, if penalty = 0.05 (5%), then the first machine you add to another will give +95% processing power, the second machine gives +90%, then +85% etc.</summary>
        [XmlElement("CombinePenalty")]
        public double CombinePenalty { get; set; }

        [XmlElement("CombineKeyNames")]
        public List<string> CombineKeyNames { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public List<SButton> CombineKeys { get; set; }

        public UserConfig()
        {
            InitializeDefaults();
        }

        public UserConfig(double d1, double d2)
        {
            InitializeDefaults();
            this.CombinePenalty = d1;
            this.MinimumEffect = d2;
        }

        private void InitializeDefaults()
        {
            this.NumberOpacity = 1.0f;
            this.DrawToolTip = true;
            this.ToolTipOffset = new Point(0, 0);

            this.MinimumEffect = 0.25;
            this.CombinePenalty = 0.03;
            this.CombineKeyNames = new List<string>() { SButton.LeftControl.ToString(), SButton.RightControl.ToString() };
        }

        public double ComputeProcessingPower(int CombinedQuantity)
        {
            //  This is just a summation converted to 2 linear formulas:
            //  Sum(1 to n)[Max(MinEffect, 1.0 - n * CombinePenalty)]
            //  If it were just: Sum(0 to n)[n * CombinePenalty] then we'd just do "n * (n + 1) / 2"
            //  But to account for the minimum effect, split it into two sums, the ones before the min effect and the ones after
            //  And obviously the 1.0 can just be factored out, since Sum(1 to n)[1.0] = n

            int MaxQuantityBeforeMinPenalty = (int)((1.0 - MinimumEffect) / CombinePenalty); // After this many machines are combined, you start getting the MinimumEffect for each successive machine that is added to the stack
            int QuantityWithMinPenalty = Math.Max(0, CombinedQuantity - MaxQuantityBeforeMinPenalty);
            int QuantityBeforeMinPenalty = Math.Min(CombinedQuantity, MaxQuantityBeforeMinPenalty);
            double TotalPenalty = QuantityBeforeMinPenalty * (QuantityBeforeMinPenalty - 1) / 2.0 * CombinePenalty + QuantityWithMinPenalty * (1.0 - MinimumEffect);
            return CombinedQuantity - TotalPenalty;
        }
    }
}
