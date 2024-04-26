/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sandman534/Abilities-Experience-Bars
**
*************************************************/

using StardewModdingAPI;

namespace AbilitiesExperienceBars
{
    public class ModConfig
    {
        public SButton ToggleKey { get; set; }

        public string UITheme { get; set; }
        public bool SmallIcons { get; set; }

        public bool ShowUI { get; set; }
        public bool ShowExperienceInfo { get; set; }
        public bool ShowExperienceGain { get; set; }
        public bool ShowBoxBackground { get; set; }
        public bool HideExperienceMax { get; set; }
        public int mainPosX { get; set; }
        public int mainPosY { get; set; }
        public int mainScale { get; set; }

        public bool ShowExpPopup { get; set; }
        public bool ShowExperiencePopupInfo { get; set; }
        public bool ShowExperiencePopupGain { get; set; }
        public bool HideExperiencePopupMax { get; set; }
        public bool ShowExpPopupTest { get; set; }
        public float PopupMessageDuration { get; set; }
        public int popupPosX { get; set; }
        public int popupPosY { get; set; }
        public int popupScale { get; set; }

        public bool ShowLevelUp { get; set; }
        public bool LevelUpSound { get; set; }
        public float LevelUpMessageDuration { get; set; }
    }
}
