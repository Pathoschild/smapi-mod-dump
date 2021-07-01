/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-silo-size/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;

namespace SiloSize
    {
    public class ModConfig
        {
        public int SiloSize = 1000;
        public bool MorningReport = true;
        public int LowWarning = 3;
        public HashSet<string> AutoHayBuildings = new HashSet<string>() { "Deluxe Barn", "Deluxe Coop" };
        }
    }
