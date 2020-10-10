/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using StardewValley;

namespace MTN2.MapData {
    public class Spawn {
        //Fundamentals
        public int ItemId { get; set; } = 0;
        public SpawningSeason Seasons { get; set; } = SpawningSeason.allYear;
        public SpawnType Boundary { get; set; } = SpawnType.noSpawn;

        //Spawn Bindings
        public Area AreaBinding { get; set; }
        public int TileBinding { get; set; } = -1;
        public bool MapWide { get; set; } = false;

        //Probability
        public float Chance { get; set; } = 0.70f;
        public Modifier RainModifier { get; set; }
        public Modifier NewMonthModifier { get; set; }
        public Modifier NewYearModifier { get; set; }

        //Spawn Amount Parameters
        public int AmountMin { get; set; } = 1;
        public int AmountMax { get; set; } = 0;
        public Modifier RainAmount { get; set; }
        public Modifier NewMonthAmount { get; set; }
        public Modifier NewYearAmount { get; set; }

        //Cooldown Parameters
        public int CooldownMin { get; set; } = 0;
        public int CooldownMax { get; set; } = 0;
        public int DaysLeft { get; set; } = 1;

        //Profession Requirement
        public int SkillLevel { get; set; } = 1;

        public Spawn() {
            RainModifier = new Modifier(0, 1);
            NewMonthModifier = new Modifier(0, 1);
            NewYearModifier = new Modifier(0, 1);
            RainAmount = new Modifier(0, 1);
            NewMonthAmount = new Modifier(0, 1);
            NewYearAmount = new Modifier(0, 1);
        }

        public void Initialize() {
            SetCooldown();
        }

        public int GenerateAmount() {
            if (AmountMax == 0) {
                return AmountMin;
            }

            return Game1.random.Next(AmountMin - 1, AmountMax);
        }

        public bool Roll() {
            float dice = Chance;

            if (dice >= Game1.random.NextDouble()) return true;

            return false;
        }

        public bool TriggerCooldown() {
            if (CooldownMin == 0) return true;
            DaysLeft--;
            if (DaysLeft <= 0) {
                SetCooldown();
                return true;
            }
            return false;
        }

        public bool IsCorrectSeason(string CurrentSeason) {
            if (SpawningSeason.allYear == Seasons) return true;
            if (CurrentSeason == "spring") {
                switch (Seasons) {
                    case SpawningSeason.springOnly:
                    case SpawningSeason.notWinter:
                    case SpawningSeason.notFall:
                    case SpawningSeason.notSummer:
                    case SpawningSeason.firstHalf:
                    case SpawningSeason.springFall:
                    case SpawningSeason.springWinter:
                        return true;
                    default:
                        return false;
                }
            } else if (CurrentSeason == "summer") {
                switch (Seasons) {
                    case SpawningSeason.summerOnly:
                    case SpawningSeason.notWinter:
                    case SpawningSeason.notFall:
                    case SpawningSeason.notSpring:
                    case SpawningSeason.firstHalf:
                    case SpawningSeason.summerWinter:
                    case SpawningSeason.summerFall:
                        return true;
                    default:
                        return false;
                }
            } else if (CurrentSeason == "fall") {
                switch (Seasons) {
                    case SpawningSeason.fallOnly:
                    case SpawningSeason.notWinter:
                    case SpawningSeason.notSummer:
                    case SpawningSeason.notSpring:
                    case SpawningSeason.secondHalf:
                    case SpawningSeason.springFall:
                    case SpawningSeason.summerFall:
                        return true;
                    default:
                        return false;
                }
            } else if (CurrentSeason == "winter") {
                switch (Seasons) {
                    case SpawningSeason.winterOnly:
                    case SpawningSeason.notFall:
                    case SpawningSeason.notSummer:
                    case SpawningSeason.notSpring:
                    case SpawningSeason.secondHalf:
                    case SpawningSeason.summerWinter:
                    case SpawningSeason.springWinter:
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        protected void SetCooldown() {
            if (CooldownMax <= 0) {
                DaysLeft = CooldownMin;
            } else {
                DaysLeft = Game1.random.Next(CooldownMin - 1, CooldownMax);
            }
            return;
        }
    }
}
