/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tesla1889tv/ControlValleyMod
**
*************************************************/

/*
 * ControlValley
 * Stardew Valley Support for Twitch Crowd Control
 * Copyright (C) 2021 TheTexanTesla
 * LGPL v2.1
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Tools;

namespace ControlValley
{
    public class WeaponClass : List<IWeaponTier>
    {
        public static readonly WeaponClass Club = new WeaponClass()
        {
            new SingleWeaponTier("Wood Club", new MeleeWeapon(24)),
            new SingleWeaponTier("Wood Mallet", new MeleeWeapon(27)),
            new SingleWeaponTier("Lead Rod", new MeleeWeapon(26)),
            new SingleWeaponTier("Kudgel", new MeleeWeapon(46)),
            new SingleWeaponTier("The Slammer", new MeleeWeapon(28)),
            new SingleWeaponTier("Galaxy Hammer", new MeleeWeapon(29)),
            new SingleWeaponTier("Dwarf Hammer", new MeleeWeapon(55)),
            new SingleWeaponTier("Dragontooth Club", new MeleeWeapon(58)),
            new SingleWeaponTier("Infinity Gavel", new MeleeWeapon(63))
        };

        public static readonly WeaponClass Dagger = new WeaponClass()
        {
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Carving Knife", new MeleeWeapon(16)),
                new KeyValuePair<string, Tool>("Iron Dirk", new MeleeWeapon(17)),
                new KeyValuePair<string, Tool>("Wind Spire", new MeleeWeapon(22))
            },
            new SingleWeaponTier("Elf Blade", new MeleeWeapon(20)),
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Burglar's Shank", new MeleeWeapon(18)),
                new KeyValuePair<string, Tool>("Crystal Dagger", new MeleeWeapon(21)),
                new KeyValuePair<string, Tool>("Wooden Blade", new MeleeWeapon(19))
            },
            new SingleWeaponTier("Broken Trident", new MeleeWeapon(51)),
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Wicked Kris", new MeleeWeapon(45)),
                new KeyValuePair<string, Tool>("Galaxy Dagger", new MeleeWeapon(23))
            },
            new SingleWeaponTier("Dwarf Dagger", new MeleeWeapon(56)),
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Dragontooth Shiv", new MeleeWeapon(59)),
                new KeyValuePair<string, Tool>("Iridium Needle", new MeleeWeapon(61))
            },
            new SingleWeaponTier("Infinity Dagger", new MeleeWeapon(64))
        };

        public static readonly WeaponClass Sword = new WeaponClass()
        {
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Rusty Sword", new MeleeWeapon(0)),
                new KeyValuePair<string, Tool>("Steel Smallsword", new MeleeWeapon(11)),
                new KeyValuePair<string, Tool>("Wooden Blade", new MeleeWeapon(12))
            },
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Pirate's Sword", new MeleeWeapon(43)),
                new KeyValuePair<string, Tool>("Silver Saber", new MeleeWeapon(1))
            },
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Cutlass", new MeleeWeapon(44)),
                new KeyValuePair<string, Tool>("Forest Sword", new MeleeWeapon(15)),
                new KeyValuePair<string, Tool>("Iron Edge", new MeleeWeapon(6))
            },
            new SingleWeaponTier("Insect Head", new MeleeWeapon(13)),
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Bone Sword", new MeleeWeapon(5)),
                new KeyValuePair<string, Tool>("Claymore", new MeleeWeapon(10)),
                new KeyValuePair<string, Tool>("Neptune's Glaive", new MeleeWeapon(14)),
                new KeyValuePair<string, Tool>("Templar's Blade", new MeleeWeapon(7))
            },
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Obsidian Edge", new MeleeWeapon(8)),
                new KeyValuePair<string, Tool>("Ossified Blade", new MeleeWeapon(60))
            },
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Tempered Broadsword", new MeleeWeapon(52)),
                new KeyValuePair<string, Tool>("Yeti Tooth", new MeleeWeapon(48))
            },
            new SingleWeaponTier("Steel Falchion", new MeleeWeapon(50)),
            new SingleWeaponTier("Dark Sword", new MeleeWeapon(2)),
            new SingleWeaponTier("Lava Katana", new MeleeWeapon(9)),
            new MultiWeaponTier()
            {
                new KeyValuePair<string, Tool>("Dragontooth Cutlass", new MeleeWeapon(57)),
                new KeyValuePair<string, Tool>("Dwarf Sword", new MeleeWeapon(54)),
                new KeyValuePair<string, Tool>("Galaxy Sword", new MeleeWeapon(4))
            },
            new SingleWeaponTier("Infinity Blade", new MeleeWeapon(62))
        };

        public bool DoDowngrade()
        {
            for (int i = this.Count - 1; i > 0; --i)
            {
                if (this[i].RemoveWeapon())
                {
                    this[i - 1].AddWeapon();
                    return true;
                }
            }

            return false;
        }

        public bool DoUpgrade()
        {
            for (int i = 0; i < this.Count - 1; ++i)
            {
                if (this[i].RemoveWeapon())
                {
                    this[i + 1].AddWeapon();
                    return true;
                }
            }

            return false;
        }
    }

    public interface IWeaponTier
    {
        void AddWeapon();
        bool RemoveWeapon();
    }

    class MultiWeaponTier : List<KeyValuePair<string, Tool>>, IWeaponTier
    {
        private Random Random { get; set; }

        public MultiWeaponTier() : base()
        {
            Random = new Random();
        }

        public void AddWeapon()
        {
            Game1.player.addItemByMenuIfNecessary(this[Random.Next(Count)].Value);
        }

        public bool RemoveWeapon()
        {
            foreach (KeyValuePair<string, Tool> weapon in this)
            {
                Tool tool = Game1.player.getToolFromName(weapon.Key);
                if (tool != null)
                {
                    Game1.player.removeItemFromInventory(tool);
                    return true;
                }
            }

            return false;
        }
    }

    class SingleWeaponTier : IWeaponTier
    {
        private readonly string name;
        private readonly Tool tool;

        public SingleWeaponTier(string name, Tool tool)
        {
            this.name = name;
            this.tool = tool;
        }

        public void AddWeapon()
        {
            Game1.player.addItemByMenuIfNecessary(tool);
        }

        public bool RemoveWeapon()
        {
            Tool tool = Game1.player.getToolFromName(name);
            if (tool != null)
            {
                Game1.player.removeItemFromInventory(tool);
                return true;
            }

            return false;
        }
    }
}
