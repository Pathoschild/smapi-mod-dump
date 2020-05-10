/*

Provides a few options to make Journey of the Prairie King minigame in Stardew
Valley easier

Copyright(C) 2020  shanks3042

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<https://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StardewValley;
using StardewValley.Locations;

namespace StardewValleyEasyPrairieKing
{
    public class ModConfig
    {
        public bool always_invincible_ { get; set; } = false;
        public int lives_ { get; set; } = 98;
        public int coins_ { get; set; } = 100;
        public int ammo_level_ { get; set; } = 0;
        public int bullet_damage_ { get; set; } = 0;
        public int fire_speed_level_ { get; set; } = 5;
        public int run_speed_level_ { get; set; } = 2;
        public bool spread_pistol_ { get; set; } = false;
    }
}
