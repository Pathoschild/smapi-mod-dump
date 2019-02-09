using System;

namespace TehPers.CoreMod.Api.Items {
    public readonly struct BuffDescription {
        public int Duration { get; }
        public TimeSpan TimeDuration => TimeSpan.FromMinutes(this.Duration * 0.7 / 60);

        public int Farming { get; }
        public int Fishing { get; }
        public int Mining { get; }
        [Obsolete("Unimplemented")]
        public int Digging { get; }
        public int Luck { get; }
        public int Foraging { get; }
        [Obsolete("Unimplemented")]
        public int Crafting { get; }
        public int MaxEnergy { get; }
        public int Magnetism { get; }
        public int Speed { get; }
        public int Defense { get; }
        public int Attack { get; }

        public BuffDescription(in TimeSpan duration, int farming = 0, int fishing = 0, int mining = 0, int digging = 0, int luck = 0, int foraging = 0, int crafting = 0, int maxEnergy = 0, int magnetism = 0, int speed = 0, int defense = 0, int attack = 0) : this((int) Math.Ceiling(duration.TotalMinutes / 0.7 * 60), farming, fishing, mining, digging, luck, foraging, crafting, maxEnergy, magnetism, speed, defense, attack) { }
        public BuffDescription(int duration, int farming = 0, int fishing = 0, int mining = 0, int digging = 0, int luck = 0, int foraging = 0, int crafting = 0, int maxEnergy = 0, int magnetism = 0, int speed = 0, int defense = 0, int attack = 0) {
#pragma warning disable 618
            this.Duration = duration;
            this.Farming = farming;
            this.Fishing = fishing;
            this.Mining = mining;
            this.Digging = digging;
            this.Luck = luck;
            this.Foraging = foraging;
            this.Crafting = crafting;
            this.MaxEnergy = maxEnergy;
            this.Magnetism = magnetism;
            this.Speed = speed;
            this.Defense = defense;
            this.Attack = attack;
#pragma warning restore 618
        }

        public string GetRawBuffInformation() {
            return string.Join(" ", new[] {
#pragma warning disable 618
                this.Farming,
                this.Fishing,
                this.Mining,
                this.Digging,
                this.Luck,
                this.Foraging,
                this.Crafting,
                this.MaxEnergy,
                this.Magnetism,
                this.Speed,
                this.Defense,
                this.Attack
#pragma warning restore 618
            });
        }
    }
}