/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/Nightshade
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ichortower
{
    public sealed class ModConfig
    {
        public KeybindList MenuKeybind = new(SButton.H);
        public bool ColorizeWorld = true;
        public bool ColorizeUI = true;
        public bool ColorizeBySeason = true;
        public int ColorizerActiveProfile = 0;
        public ColorizerPreset[] ColorizerProfiles = new ColorizerPreset[4] {
            new(), new(), new(), new(),
        };

        public bool DepthOfFieldEnabled = false;
        public DepthOfFieldPreset DepthOfFieldSettings = new();
    }

    public sealed class ColorizerPreset
    {
        public float Saturation = 0f;
        public float Lightness = 0f;
        public float Contrast = 0f;

        public LumaType Luma = LumaType.BT709;

        public float ShadowR = 0f;
        public float ShadowG = 0f;
        public float ShadowB = 0f;
        public float MidtoneR = 0f;
        public float MidtoneG = 0f;
        public float MidtoneB = 0f;
        public float HighlightR = 0f;
        public float HighlightG = 0f;
        public float HighlightB = 0f;

        public ColorizerPreset Clone() {
            return (ColorizerPreset) this.MemberwiseClone();
        }
    }

    public sealed class DepthOfFieldPreset
    {
        public float Field = 0.6f;
        public float Intensity = 6.0f;
    }

    public enum LumaType {
        BT709 = 0,
        BT601 = 1,
    }
}
