using System;
using System.ComponentModel;
using Newtonsoft.Json;
using StardewValley;
using TehPers.CoreMod.Api.Json;

namespace TehPers.CoreMod.Api.Items {
    [JsonDescribe]
    public readonly struct BuffDescription {
        [Description("The duration of the buff. To convert from minutes into this value, use the formula `minutes / 0.7 * 60`.")]
        public int Duration { get; }

        [JsonIgnore]
        public TimeSpan TimeDuration => TimeSpan.FromMinutes(this.Duration * 0.7 / 60);

        [Description("The amount to increase farming level.")]
        public int Farming { get; }

        [Description("The amount to increase fishing level.")]
        public int Fishing { get; }

        [Description("The amount to increase mining level.")]
        public int Mining { get; }

        [Description("Unimplemented feature.")]
        [Obsolete("Unimplemented")]
        public int Digging { get; }

        [Description("The amount to increase luck level.")]
        public int Luck { get; }

        [Description("The amount to increase foraging level.")]
        public int Foraging { get; }

        [Description("Unimplemented feature.")]
        [Obsolete("Unimplemented")]
        public int Crafting { get; }

        [Description("The amount to increase max energy.")]
        public int MaxEnergy { get; }

        [Description("The amount to increase magnetism.")]
        public int Magnetism { get; }

        [Description("The amount to increase speed.")]
        public int Speed { get; }

        [Description("The amount to increase defense.")]
        public int Defense { get; }

        [Description("The amount to increase attack.")]
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

        public Buff CreateBuff(string source, string displaySource) {
#pragma warning disable 618
            return new Buff(this.Farming, this.Fishing, this.Mining, this.Digging, this.Luck, this.Foraging, this.Crafting, this.MaxEnergy, this.Magnetism, this.Speed, this.Defense, this.Attack, (int) this.TimeDuration.TotalMinutes, source, displaySource);
#pragma warning restore 618
        }
    }
}