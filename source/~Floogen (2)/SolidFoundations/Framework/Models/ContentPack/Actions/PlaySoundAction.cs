/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using SolidFoundations.Framework.Models.Data;
using StardewValley;
using System;
using static SolidFoundations.Framework.Models.ContentPack.Actions.SpecialAction;

namespace SolidFoundations.Framework.Models.ContentPack.Actions
{
    public class PlaySoundAction
    {
        public string Sound { get; set; }
        public int Pitch { get; set; } = -1;
        public int MinPitchRandomness { get; set; }
        public int MaxPitchRandomness { get; set; }
        private float _volume { get; set; } = 1f;
        public float Volume { get { return _volume; } set { _volume = value > 1f ? 1f : value < 0f ? 0f : value; } }
        public AmbientSoundSettings AmbientSettings { get; set; }

        internal bool IsValid()
        {
            try
            {
                Game1.soundBank.GetCue(Sound);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        internal int GetPitchRandomized()
        {
            if (MinPitchRandomness > MaxPitchRandomness)
            {
                return Game1.random.Next(MinPitchRandomness < 0 ? 0 : MinPitchRandomness);
            }

            return Game1.random.Next(MinPitchRandomness, MaxPitchRandomness);
        }
    }
}
