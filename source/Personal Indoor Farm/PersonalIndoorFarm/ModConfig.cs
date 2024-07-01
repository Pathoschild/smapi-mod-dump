/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using Microsoft.Xna.Framework;
using PersonalIndoorFarm.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalIndoorFarm
{
    public class ModConfig
    {
        public string OwnerFarmhouse { get; set; }
        public string OwnerOutside { get; set; }
        public Color LockedDoorColor { get; set; }
        public Color LockedWhenOfflineDoorColor { get; set; }
        public Color UnlockedDoorColor { get; set; }
        public ModConfig()
        {
            OwnerFarmhouse = DoorOwnerEnum.Owner.ToString();
            OwnerOutside = DoorOwnerEnum.PlacedBy.ToString();
            LockedDoorColor = Color.Red * 0.6f;
            LockedWhenOfflineDoorColor = Color.Yellow * 0.6f;
            UnlockedDoorColor = Color.ForestGreen * 0.6f;
        }
    }
}
