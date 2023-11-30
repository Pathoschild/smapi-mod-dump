/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace CoreBoy.memory.cart.rtc
{
    public class RealTimeClock
    {
        private readonly IClock _clock;
        private long _offsetSec;
        private long _clockStart;
        private bool _halt;
        private long _latchStart;
        private int _haltSeconds;
        private int _haltMinutes;
        private int _haltHours;
        private int _haltDays;

        public RealTimeClock(IClock clock)
        {
            this._clock = clock;
            this._clockStart = clock.CurrentTimeMillis();
        }

        public void Latch()
        {
            this._latchStart = this._clock.CurrentTimeMillis();
        }

        public void Unlatch()
        {
            this._latchStart = 0;
        }

        public int GetSeconds()
        {
            return (int)(this.ClockTimeInSec() % 60);
        }

        public int GetMinutes()
        {
            return (int)(this.ClockTimeInSec() % (60 * 60) / 60);
        }

        public int GetHours()
        {
            return (int)(this.ClockTimeInSec() % (60 * 60 * 24) / (60 * 60));
        }

        public int GetDayCounter()
        {
            return (int)(this.ClockTimeInSec() % (60 * 60 * 24 * 512) / (60 * 60 * 24));
        }

        public bool IsHalt()
        {
            return this._halt;
        }

        public bool IsCounterOverflow()
        {
            return this.ClockTimeInSec() >= 60 * 60 * 24 * 512;
        }

        public void SetSeconds(int seconds)
        {
            if (!this._halt)
            {
                return;
            }

            this._haltSeconds = seconds;
        }

        public void SetMinutes(int minutes)
        {
            if (!this._halt)
            {
                return;
            }

            this._haltMinutes = minutes;
        }

        public void SetHours(int hours)
        {
            if (!this._halt)
            {
                return;
            }

            this._haltHours = hours;
        }

        public void SetDayCounter(int dayCounter)
        {
            if (!this._halt)
            {
                return;
            }

            this._haltDays = dayCounter;
        }

        public void SetHalt(bool halt)
        {
            if (halt && !this._halt)
            {
                this.Latch();
                this._haltSeconds = this.GetSeconds();
                this._haltMinutes = this.GetMinutes();
                this._haltHours = this.GetHours();
                this._haltDays = this.GetDayCounter();
                this.Unlatch();
            }
            else if (!halt && this._halt)
            {
                this._offsetSec = this._haltSeconds + (this._haltMinutes * 60) + (this._haltHours * 60 * 60) + (this._haltDays * 60 * 60 * 24);
                this._clockStart = this._clock.CurrentTimeMillis();
            }

            this._halt = halt;
        }

        public void ClearCounterOverflow()
        {
            while (this.IsCounterOverflow())
            {
                this._offsetSec -= 60 * 60 * 24 * 512;
            }
        }

        private long ClockTimeInSec()
        {
            var now = this._latchStart == 0 ? this._clock.CurrentTimeMillis() : this._latchStart;
            return ((now - this._clockStart) / 1000) + this._offsetSec;
        }

        public void Deserialize(long[] clockData)
        {
            var seconds = clockData[0];
            var minutes = clockData[1];
            var hours = clockData[2];
            var days = clockData[3];
            var daysHigh = clockData[4];
            var timestamp = clockData[10];

            this._clockStart = timestamp * 1000;
            this._offsetSec = seconds + (minutes * 60) + (hours * 60 * 60) + (days * 24 * 60 * 60) +
                             (daysHigh * 256 * 24 * 60 * 60);
        }

        public long[] Serialize()
        {
            var clockData = new long[11];
            this.Latch();
            clockData[0] = clockData[5] = this.GetSeconds();
            clockData[1] = clockData[6] = this.GetMinutes();
            clockData[2] = clockData[7] = this.GetHours();
            clockData[3] = clockData[8] = this.GetDayCounter() % 256;
            clockData[4] = clockData[9] = this.GetDayCounter() / 256;
            clockData[10] = this._latchStart / 1000;
            this.Unlatch();
            return clockData;
        }
    }
}