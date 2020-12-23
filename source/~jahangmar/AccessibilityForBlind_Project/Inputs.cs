/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using StardewModdingAPI;
namespace AccessibilityForBlind
{
    public static class Inputs
    {
        public static bool IsMenuNextButton(SButton b) => ModEntry.GetConfig().key_menu_next.Contains(b);
        public static bool IsMenuPrevButton(SButton b) => ModEntry.GetConfig().key_menu_prev.Contains(b);
        public static bool IsMenuActivateButton(SButton b) => ModEntry.GetConfig().key_menu_activate.Contains(b) || b.IsActionButton();
        public static bool IsMenuDeleteButton(SButton b) => ModEntry.GetConfig().key_menu_delete.Contains(b);
        public static bool IsMenuEscapeButton(SButton b) => ModEntry.GetConfig().key_menu_escape.Contains(b);
        public static bool IsMenuTitleMuteButton(SButton b) => ModEntry.GetConfig().key_menu_title_mute.Contains(b);

        public static bool IsTTSMapCheckButton(SButton b) => ModEntry.GetConfig().key_tts_map_check.Contains(b);
        public static bool IsTTSMapCheckUnderneathButton(SButton b, System.Func<SButton, bool> extra) => IsTTSMapCheckButton(b) && (extra(SButton.LeftShift) || extra(SButton.RightShift));
        public static bool IsTTSInfoButton(SButton b) => ModEntry.GetConfig().key_tts_info.Contains(b);
        public static bool IsTTSRepeatButton(SButton b) => ModEntry.GetConfig().key_tts_repeat.Contains(b);
        public static bool IsTTSStopButton(SButton b) => ModEntry.GetConfig().key_tts_stop.Contains(b);

        public static bool IsTTSTimeButton(SButton b) => ModEntry.GetConfig().key_tts_time.Contains(b);
        public static bool IsTTSHealthButton(SButton b) => ModEntry.GetConfig().key_tts_health.Contains(b);

        public static bool IsGameMenuInventoryButton(SButton b) => ModEntry.GetConfig().key_gamemenu_inv.Contains(b);
        public static bool IsGameMenuSkillsButton(SButton b) => ModEntry.GetConfig().key_gamemenu_skills.Contains(b);
        public static bool IsGameMenuSocialButton(SButton b) => ModEntry.GetConfig().key_gamemenu_social.Contains(b);
        public static bool IsGameMenuMapButton(SButton b) => ModEntry.GetConfig().key_gamemenu_map.Contains(b);
        public static bool IsGameMenuCraftingButton(SButton b) => ModEntry.GetConfig().key_gamemenu_crafting.Contains(b);
        public static bool IsGameMenuCollectionsButton(SButton b) => ModEntry.GetConfig().key_gamemenu_collections.Contains(b);
        public static bool IsGameMenuOptionsButton(SButton b) => ModEntry.GetConfig().key_gamemenu_options.Contains(b);
        public static bool IsGameMenuExitButton(SButton b) => ModEntry.GetConfig().key_gamemenu_exit.Contains(b);

        public static bool IsGameMenuButton(SButton b) => IsGameMenuMapButton(b) || IsGameMenuSkillsButton(b) || IsGameMenuSocialButton(b)
            || IsGameMenuInventoryButton(b) || IsGameMenuCraftingButton(b) || IsGameMenuCollectionsButton(b) || IsGameMenuOptionsButton(b)
            || IsGameMenuExitButton(b);
    }
}
