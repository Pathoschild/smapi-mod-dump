/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using SpaceCore;
using StardewValley;

namespace RuneMagic.Source
{
    public abstract class SpellEffect
    {
        public string Name { get; set; }
        public Spell Spell { get; set; }
        public int Timer { get; set; }
        public Duration Duration { get; set; }

        public SpellEffect(Spell spell, Duration duration)
        {
            Name = $"Glyph of {spell.Name}";
            Duration = duration;
            Spell = spell;
            switch (Duration)
            {
                case Duration.Instant: Timer = 0; break;
                case Duration.Short: Timer = 5 + spell.Skill.Level * 60; break;
                case Duration.Medium: Timer = 10 * spell.Skill.Level * 60; break;
                case Duration.Long: Timer = 30 * spell.Skill.Level * 60; break;
                case Duration.Permanent: Timer = 999999999; break;
            }
        }

        public virtual void Start()
        {
            RuneMagic.PlayerStats.ActiveEffects.Add(this);
        }

        public virtual void End()
        {
            RuneMagic.PlayerStats.ActiveEffects.Remove(this);
        }

        public virtual void Update()
        {
            if (RuneMagic.PlayerStats.ActiveEffects.Contains(this))
            {
                if (Timer == 0)
                {
                    End();
                    return;
                }
                Timer--;
            }
            //RuneMagic.Instance.Monitor.Log($"{Timer}");
        }
    }
}