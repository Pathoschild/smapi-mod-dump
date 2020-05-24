using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Internal
{
    internal class Countdown : IUpdateable
    {
        private class CountdownTimer
        {
            public string name;
            public int interval;

            public CountdownTimer(string name, int interval)
            {
                this.name = name;
                this.interval = interval;
            }
        }
        private List<CountdownTimer> cooldowns = new List<CountdownTimer>();

        public void Set(string name, int interval)
        {
            CountdownTimer timer = this.cooldowns.Find(t => t.name == name);

            if (timer != null)
            {
                timer.interval = interval;
                return;
            }

            this.cooldowns.Add(new CountdownTimer(name, interval));
        }

        public void Increase(string name, int amount)
        {
            CountdownTimer timer = this.cooldowns.Find(t => t.name == name);

            if (timer != null)
            {
                timer.interval += amount;
                return;
            }

            this.Set(name, amount);
        }

        public bool IsRunning(string name)
        {
            CountdownTimer timer = this.cooldowns.Find(t => t.name == name);

            return timer != null && timer.interval > 0;
        }

        public void Reset()
        {
            this.cooldowns.Clear();
        }

        public void Update(UpdateTickedEventArgs e)
        {
            if (this.cooldowns.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < this.cooldowns.Count; i++)
            {
                if (--this.cooldowns[i].interval <= 0)
                {
                    this.cooldowns.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
