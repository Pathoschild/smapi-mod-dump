/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using System.Collections.Generic;
using SplitscreenImproved.Layout;
using SplitscreenImproved.ShowName;

namespace SplitscreenImproved
{
    public class ModConfig
    {
        public bool IsModEnabled { get; set; } = true;

        public LayoutFeatureConfig LayoutFeature { get; set; } = new();

        public MusicFixFeatureConfig MusicFixFeature { get; set; } = new();

        public HudTweaksFeatureConfig HudTweaksFeature { get; set; } = new();

        public ShowNameFeatureConfig ShowNameFeature { get; set; } = new();

        public class LayoutFeatureConfig
        {
            public bool IsFeatureEnabled { get; set; } = true;

            public LayoutPreset PresetChoice { get; set; } = LayoutPreset.Default;

            public SplitscreenLayout DefaultSplitscreenLayout { get; set; } = new(LayoutPreset.Default);

            public SplitscreenLayout SwapSidesSplitscreenLayout { get; set; } = new(LayoutPreset.SwapSides);

            public SplitscreenLayout CustomSplitscreenLayout { get; set; } = new(LayoutPreset.Custom);

            public SplitscreenLayout GetSplitscreenLayoutByPreset(LayoutPreset preset) => preset switch
            {
                LayoutPreset.Default => DefaultSplitscreenLayout,
                LayoutPreset.SwapSides => SwapSidesSplitscreenLayout,
                _ => CustomSplitscreenLayout,
            };
        }

        public class ShowNameFeatureConfig
        {
            public bool IsFeatureEnabled { get; set; } = true;

            public bool IsSplitscreenOnly { get; set; } = true;

            public ShowNamePosition Position { get; set; } = ShowNamePosition.Top;
        }

        public class MusicFixFeatureConfig
        {
            /* DEBUG
            public bool IsDebugMode { get; set; } = false;
            */

            public bool IsFeatureEnabled { get; set; } = true;
        }

        public class HudTweaksFeatureConfig
        {
            public bool IsFeatureEnabled { get; set; } = true;

            public bool IsSplitscreenOnly { get; set; } = false;

            public bool IsToolbarHudOffsetEnabled { get; set; } = true;
        }
    }
}
