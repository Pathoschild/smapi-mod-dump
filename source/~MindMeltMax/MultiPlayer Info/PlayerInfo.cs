/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPInfo
{
    public class PlayerInfo
    {
        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public float Stamina { get; set; }

        public int MaxStamina { get; set; }

        public PlayerInfo() { }

        public PlayerInfo(Farmer player)
        {
            Health = player.health;
            MaxHealth = player.maxHealth;
            Stamina = player.Stamina;
            MaxStamina = player.MaxStamina;
        }

        //Using this instead of $"{Health}|{MaxHealth}|{Stamina}|{MaxStamina}"; takes 18 less il instructions... WTF c#?
        public string Serialize() => new StringBuilder().Append(Health).Append('|').Append(MaxHealth).Append('|').Append(Stamina).Append('|').Append(MaxStamina).ToString();

        public static PlayerInfo Deserialize(string value)
        {
            var items = value.Split('|');
            return new()
            {
                Health = int.Parse(items[0]),
                MaxHealth = int.Parse(items[1]),
                Stamina = float.Parse(items[2]),
                MaxStamina = int.Parse(items[3])
            };
        }

        public bool IsMatch(Farmer player) => player.health == Health && player.maxHealth == MaxHealth && player.Stamina == Stamina && player.MaxStamina == MaxStamina;
    }
}
