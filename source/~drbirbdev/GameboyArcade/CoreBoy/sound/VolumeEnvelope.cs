/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace CoreBoy.sound
{
    public class VolumeEnvelope
    {
        private int _initialVolume;
        private int _envelopeDirection;
        private int _sweep;
        private int _volume;
        private bool _finished;
        private int _i;

        public void SetNr2(int register)
        {
            this._initialVolume = register >> 4;
            this._envelopeDirection = (register & (1 << 3)) == 0 ? -1 : 1;
            this._sweep = register & 0b111;
        }

        public bool IsEnabled() => this._sweep > 0;

        public void Start()
        {
            this._finished = true;
            this._i = 8192;
        }

        public void Trigger()
        {
            this._i = 0;
            this._volume = this._initialVolume;
            this._finished = false;
        }

        public void Tick()
        {
            if (this._finished)
            {
                return;
            }

            if ((this._volume == 0 && this._envelopeDirection == -1) || (this._volume == 15 && this._envelopeDirection == 1))
            {
                this._finished = true;
                return;
            }

            if (++this._i == this._sweep * Gameboy.TicksPerSec / 64)
            {
                this._i = 0;
                this._volume += this._envelopeDirection;
            }
        }

        public int GetVolume() => this.IsEnabled() ? this._volume : this._initialVolume;
    }

}