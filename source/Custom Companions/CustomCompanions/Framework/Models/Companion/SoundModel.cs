/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

namespace CustomCompanions.Framework.Models.Companion
{
    public class SoundModel
    {
        public string SoundName { get; set; }
        public string WhenToPlay { get; set; }
        public int Pitch { get; set; } = -1;
        public int MinPitchRandomness { get; set; }
        public int MaxPitchRandomness { get; set; }
        public int TimeBetweenSound { get; set; }
        public float ChanceOfPlaying { get; set; } = 1f;

        public override string ToString()
        {
            return $"[SoundName: {SoundName} | WhenToPlay: {WhenToPlay} | Pitch: {Pitch} | MinPitchRandomness: {MinPitchRandomness} | MaxPitchRandomness: {MaxPitchRandomness} | TimeBetweenSound: {TimeBetweenSound} | ChanceOfPlaying: {ChanceOfPlaying}]";
        }
    }
}
