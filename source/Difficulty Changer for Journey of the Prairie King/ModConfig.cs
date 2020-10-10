/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/shanks3042/stardewvalleyeasyprairieking
**
*************************************************/

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

        public bool alwaysInvincible{ get; set; } = false;
        public int lives { get; set; } = 98;
        public int coins { get; set; } = 100;
        public int ammoLevel { get; set; } = 0;
        public int bulletDamage { get; set; } = 0;
        public int fireSpeedLevel { get; set; } = 5;
        public int runSpeedLevel { get; set; } = 2;
        public bool useShotgun { get; set; } = false;
        public int waveTimer { get; set; } = 60;
    }
}
