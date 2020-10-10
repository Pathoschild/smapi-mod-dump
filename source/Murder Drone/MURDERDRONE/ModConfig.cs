/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/camcamcamcam/murderdrone
**
*************************************************/

using System;
namespace MURDERDRONE
{
    public class ModConfig
    {
        public bool Active { get; set; }
        public string KeyboardShortcut { get; set; }
        public int RotationSpeed { get; set; }
        public int Damage { get; set; }
        public int ProjectileVelocity { get; set; }

        public ModConfig()
        {
            this.Active = true;
            this.KeyboardShortcut = "F7";
            this.RotationSpeed = 2;
            this.Damage = -1;
            this.ProjectileVelocity = 16;
        }
    }
}
