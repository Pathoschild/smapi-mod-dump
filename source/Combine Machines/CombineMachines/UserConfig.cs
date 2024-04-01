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
using StardewModdingAPI;
using SObject = StardewValley.Object;
using CombineMachines.Helpers;
using StardewValley.Objects;
using Newtonsoft.Json;

namespace CombineMachines
{
    [XmlRoot(ElementName = "ProcessingMode", Namespace = "")]
    public enum ProcessingMode
    {
        /// <summary>Indicates that combined machines should have their inputs and outputs increased, to process multiple items in a single processing cycle.</summary>
        [XmlEnum("Output")]
        MultiplyItems,
        /// <summary>Indicates that combined machines should have their <see cref="SObject.MinutesUntilReady"/> reduced, to speed up their processing cycles whilst still only processing 1 item per cycle.</summary>
        [XmlEnum("Output")]
        IncreaseSpeed
    }

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

        /// <summary>True if the tooltip that appears when hovering over a combined machine that has been placed on a tile should display the duration remaining for the current processing cycle.</summary>
        [XmlElement("ToolTipShowDuration")]
        public bool ToolTipShowDuration { get; set; }

        /// <summary>True if the tooltip that appears when hovering over a combined machine that has been placed on a tile should display the quantity that will be produced at the end of the current processing cycle.</summary>
        [XmlElement("ToolTipShowQuantity")]
        public bool ToolTipShowQuantity { get; set; }

        /// <summary>The minimum boost to processing power that will be applied when combined machines. See also: <see cref="CombinePenalty"/></summary>
        [XmlElement("MinimumEffect")]
        public double MinimumEffect { get; set; }

        /// <summary>A penalty that is applied to the processing power of a combined machine. This penalty stacks additively.<para/>
        /// For example, if penalty = 0.05 (5%), then the first machine you add to another will give +95% processing power, the second machine gives +90%, then +85% etc.</summary>
        [XmlElement("CombinePenalty")]
        public double CombinePenalty { get; set; }

        /// <summary>The names of keys that can be held down to combine two or more machines in your inventory. Default = LeftControl or RightControl</summary>
        [XmlElement("CombineKeyNames")]
        public List<string> CombineKeyNames { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public List<SButton> CombineKeys { get; set; }

        [XmlElement("ProcessingMode")]
        public ProcessingMode ProcessingMode { get; set; }
        /// <summary>The names of machines that should use the opposite Processing Mode than the one specified by <see cref="ProcessingMode"/>. Default = Empty list</summary>
        [XmlElement("ProcessingModeExclusions")]
        public List<string> ProcessingModeExclusions { get; set; }

        /// <summary>If true, you will be able to combine scarecrows to increase their effective radius by a factor of Sqrt(CombinedPower)<para/>
        /// (It scales with Sqrt because the multiplier is intended to multiply the # of covered tiles rather than the radius.
        /// For example: doubling the covered tiles only increases the radius by Sqrt(2) (CoveredTiles=Pi*r^2, solve for r)</summary>
        [XmlElement("AllowCombiningScarecrows")]
        public bool AllowCombiningScarecrows { get; set; }

        /// <summary>If true, then combined furnaces will consume 1 coal per bar produced. If false, furnaces will consume 1 coal per processing cycle, regardless of how many bars are produced.</summary>
        [XmlElement("FurnaceMultiplyCoalInputs")]
        public bool FurnaceMultiplyCoalInputs { get; set; }

        public UserConfig()
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            this.NumberOpacity = 1.0f;
            this.DrawToolTip = true;
            this.ToolTipOffset = new Point(0, 0);
            this.ToolTipShowDuration = true;
            this.ToolTipShowQuantity = true;

            this.MinimumEffect = 0.25;
            this.CombinePenalty = 0.03;
            this.CombineKeyNames = new List<string>() {
                SButton.LeftControl.ToString(), SButton.RightControl.ToString(),
                SButton.LeftShoulder.ToString(), SButton.RightShoulder.ToString()
            };

            this.ProcessingMode = ModEntry.IsAutomateModInstalled ? ProcessingMode.IncreaseSpeed : ProcessingMode.MultiplyItems;
            if (ProcessingMode == ProcessingMode.IncreaseSpeed)
                this.ProcessingModeExclusions = new List<string>() { "Lightning Rod" };
            else if (ProcessingMode == ProcessingMode.MultiplyItems)
                this.ProcessingModeExclusions = new List<string>() { };
            else
                this.ProcessingModeExclusions = new List<string>();

            this.AllowCombiningScarecrows = true;

            this.FurnaceMultiplyCoalInputs = true;
        }

        public double ComputeProcessingPower(int CombinedQuantity)
        {
            //  This is just a summation converted to 2 linear formulas:
            //  Sum(1 to n)[Max(MinEffect, 1.0 - n * CombinePenalty)]
            //  If it were just: Sum(1 to n)[n * CombinePenalty] then we'd just do "(n * (n + 1) / 2) * CombinePenalty"
            //  But to account for the minimum effect, split it into two sums, the ones before the min effect and the ones after
            //  And obviously the 1.0 can just be factored out, since Sum(1 to n)[1.0] = n

            int MaxQuantityBeforeMinPenalty = (int)((1.0 - MinimumEffect) / CombinePenalty); // After this many machines are combined, you start getting the MinimumEffect for each successive machine that is added to the stack
            int QuantityWithMinPenalty = Math.Max(0, CombinedQuantity - MaxQuantityBeforeMinPenalty);
            int QuantityBeforeMinPenalty = Math.Min(CombinedQuantity, MaxQuantityBeforeMinPenalty);
            double TotalPenalty = QuantityBeforeMinPenalty * (QuantityBeforeMinPenalty - 1) / 2.0 * CombinePenalty + QuantityWithMinPenalty * (1.0 - MinimumEffect);
            return CombinedQuantity - TotalPenalty;
        }

        public bool ShouldModifyInputsAndOutputs(SObject Machine)
        {
            if (Machine == null || !Machine.IsCombinableObject() || !Machine.IsCombinedMachine() || Machine is Cask || Machine.IsScarecrow())
                return false;
            //  CrabPots always use IncreaseSpeed mode because MultiplyItems mode is currently bugged for CrabPots in Stardew Valley version 1.6
            //  (Something seems to be resetting the held object quantity after I've modified it)
            else if (Machine is CrabPot)
                return false;
            else
            {
                switch (ProcessingMode)
                {
                    case ProcessingMode.MultiplyItems:
                        return !ProcessingModeExclusions.Contains(Machine.Name);
                    case ProcessingMode.IncreaseSpeed:
                        return ProcessingModeExclusions.Contains(Machine.Name);
                    default: throw new NotImplementedException(string.Format("Unrecognized {0}: {1}", nameof(ProcessingMode), this.ProcessingMode.ToString()));
                }
            }
        }

        public bool ShouldModifyProcessingSpeed(SObject Machine)
        {
            if (Machine == null || !Machine.IsCombinableObject() || !Machine.IsCombinedMachine() || Machine.IsScarecrow())
                return false;
            else if (Machine is Cask)
                return true;
            //  CrabPots always use IncreaseSpeed mode because MultiplyItems mode is currently bugged for CrabPots in Stardew Valley version 1.6
            //  (Something seems to be resetting the held object quantity after I've modified it)
            else if (Machine is CrabPot)
                return true;
            else
            {
                switch (ProcessingMode)
                {
                    case ProcessingMode.MultiplyItems:
                        return ProcessingModeExclusions.Contains(Machine.Name);
                    case ProcessingMode.IncreaseSpeed:
                        return !ProcessingModeExclusions.Contains(Machine.Name);
                    default: throw new NotImplementedException(string.Format("Unrecognized {0}: {1}", nameof(ProcessingMode), this.ProcessingMode.ToString()));
                }
            }
        }

        internal const string DefaultFilename = "config.json";

        internal static UserConfig Load(IDataHelper DataHelper)
        {
            UserConfig Config = DataHelper.ReadJsonFile<UserConfig>(DefaultFilename);

            if (Config != null)
            {
                if (Config.CombineKeyNames == null || !Config.CombineKeyNames.Any())
                    Config.CombineKeyNames = new List<string>() { SButton.LeftControl.ToString(), SButton.RightControl.ToString() };
                if (Config.ProcessingModeExclusions == null)
                    Config.ProcessingModeExclusions = new List<string>();
            }

            return Config;
        }
    }
}
