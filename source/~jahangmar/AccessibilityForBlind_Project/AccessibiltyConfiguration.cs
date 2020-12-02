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

using System.Collections.Generic;
using StardewModdingAPI;

namespace AccessibilityForBlind
{
    public class AccessibilityConfiguration
    {
        public string tts_path = "";
        public int tts_speed = 120;
        public int tts_pitch = 120;

        public List<SButton> key_menu_escape = new List<SButton>() { SButton.Escape, SButton.ControllerBack};
        public List<SButton> key_menu_next = new List<SButton>() { SButton.Down, SButton.Tab };
        public List<SButton> key_menu_prev = new List<SButton>() { SButton.Up };
        public List<SButton> key_menu_activate = new List<SButton>() { SButton.Enter, SButton.ControllerA };
        public List<SButton> key_menu_delete = new List<SButton>() { SButton.Delete };
        public List<SButton> key_tts_info = new List<SButton>() { SButton.F1, SButton.ControllerX };
        public List<SButton> key_tts_repeat = new List<SButton>() { SButton.F5 };
        public List<SButton> key_tts_stop = new List<SButton>() { SButton.Escape };
        public List<SButton> key_tts_map_check = new List<SButton>() { SButton.Enter, SButton.ControllerA };
        public List<SButton> key_menu_title_mute = new List<SButton>() { SButton.M };
        public List<SButton> key_tts_time = new List<SButton>() { SButton.F2 };
        public List<SButton> key_tts_health = new List<SButton>() { SButton.F3 };
        public List<SButton> key_gamemenu_inv = new List<SButton>() { SButton.I };
        public List<SButton> key_gamemenu_skills = new List<SButton>() { SButton.K };
        public List<SButton> key_gamemenu_social = new List<SButton>() { SButton.S };
        public List<SButton> key_gamemenu_map = new List<SButton>() { SButton.M };
        public List<SButton> key_gamemenu_crafting = new List<SButton>() { SButton.C };
        public List<SButton> key_gamemenu_collections = new List<SButton>() { SButton.A };
        public List<SButton> key_gamemenu_options = new List<SButton>() { SButton.O };
        public List<SButton> key_gamemenu_exit = new List<SButton>() { SButton.Q };
    }
}
