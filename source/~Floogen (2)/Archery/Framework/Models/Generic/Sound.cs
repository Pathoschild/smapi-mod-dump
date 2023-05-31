/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using StardewValley;
using System;

namespace Archery.Framework.Models.Generic
{
    public class Sound
    {
        public string Name { get; set; }
        public int Pitch { get; set; } = -1;
        public RandomRange PitchRandomness { get; set; }
        private float _volume { get; set; } = 1f;
        public float Volume { get { return _volume; } set { _volume = Utility.Clamp(value, 0f, 1f); } }
        public float MaxDistance { get; set; } = 1024f;

        internal bool IsValid()
        {
            try
            {
                Game1.soundBank.GetCue(Name);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
