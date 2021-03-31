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
using System.Threading;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewBoots = StardewValley.Objects.Boots;
using StardewChest = StardewValley.Objects.Chest;

namespace ControlValley
{
    public delegate CrowdResponse CrowdDelegate(ControlClient client, CrowdRequest req);

    public class CrowdDelegates
    {
        private static readonly List<KeyValuePair<string, int>> downgradeFishingRods = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("Iridium Rod", 2),
            new KeyValuePair<string, int>("Fiberglass Rod", 0),
            new KeyValuePair<string, int>("Bamboo Pole", 1)
        };

        private static readonly List<KeyValuePair<string, int>> upgradeFishingRods = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("Training Rod", 0),
            new KeyValuePair<string, int>("Bamboo Pole", 2),
            new KeyValuePair<string, int>("Fiberglass Rod", 3)
        };

        public static CrowdResponse DowngradeAxe(ControlClient client, CrowdRequest req)
        {
            return DoDowngrade(req, "Axe");
        }

        public static CrowdResponse DowngradeBoots(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            StardewBoots boots = Game1.player.boots.Get();
            if (boots == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is not currently wearing Boots";
            }
            else
            {
                boots = Boots.GetDowngrade(boots.getStatsIndex());
                if (boots == null)
                {
                    status = CrowdResponse.Status.STATUS_FAILURE;
                    message = Game1.player.Name + "'s Boots are already at the lowest upgrade level";
                }
                else
                {
                    Game1.player.boots.Value = boots;
                    Game1.player.changeShoeColor(boots.indexInColorSheet);
                    UI.ShowInfo($"{req.GetReqViewer()} downgraded {Game1.player.Name}'s Boots");
                }
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse DowngradeFishingRod(ControlClient client, CrowdRequest req)
        {
            int id = req.GetReqID();

            foreach (KeyValuePair<string, int> downgrade in downgradeFishingRods)
            {
                Tool tool = Game1.player.getToolFromName(downgrade.Key);
                if (tool != null)
                {
                    tool.UpgradeLevel = downgrade.Value;
                    UI.ShowInfo($"{req.GetReqViewer()} downgraded {Game1.player.Name}'s Fishing Rod");

                    return new CrowdResponse(id);
                }
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Fishing Rod is already at the lowest upgrade level");
        }

        public static CrowdResponse DowngradeHoe(ControlClient client, CrowdRequest req)
        {
            return DoDowngrade(req, "Hoe");
        }

        public static CrowdResponse DowngradePickaxe(ControlClient client, CrowdRequest req)
        {
            return DoDowngrade(req, "Pickaxe");
        }

        public static CrowdResponse DowngradeTrashCan(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Game1.player.trashCanLevel > 0)
            {
                Interlocked.Decrement(ref Game1.player.trashCanLevel);
                UI.ShowInfo($"{req.GetReqViewer()} downgraded {Game1.player.Name}'s Trash Can");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + "'s Trash Can is already at the lowest upgrade level";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse DowngradeWateringCan(ControlClient client, CrowdRequest req)
        {
            return DoDowngrade(req, "Watering Can");
        }

        public static CrowdResponse DowngradeWeapon(ControlClient client, CrowdRequest req)
        {
            int id = req.GetReqID();

            if (WeaponClass.Club.DoDowngrade() || WeaponClass.Sword.DoDowngrade() || WeaponClass.Dagger.DoDowngrade())
            {
                UI.ShowInfo($"{req.GetReqViewer()} downgraded {Game1.player.Name}'s Weapon");
                return new CrowdResponse(id);
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Weapon is already at the lowest upgrade level");
        }

        public static CrowdResponse Energize10(ControlClient client, CrowdRequest req)
        {
            return DoEnergizeBy(req, 0.1f);
        }

        public static CrowdResponse Energize25(ControlClient client, CrowdRequest req)
        {
            return DoEnergizeBy(req, 0.25f);
        }

        public static CrowdResponse Energize50(ControlClient client, CrowdRequest req)
        {
            return DoEnergizeBy(req, 0.5f);
        }

        public static CrowdResponse EnergizeFull(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int max = Game1.player.MaxStamina;
            float stamina = Game1.player.Stamina;
            if (stamina < max)
            {
                Game1.player.Stamina = max;
                UI.ShowInfo($"{req.GetReqViewer()} fully energized {Game1.player.Name}");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at maximum energy";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse GiveBuffAdrenaline(ControlClient client, CrowdRequest req)
        {
            return DoGiveBuff(req, Buff.adrenalineRush, 30, "Adrenaline Rush");
        }

        public static CrowdResponse GiveBuffDarkness(ControlClient client, CrowdRequest req)
        {
            return DoGiveBuff(req, Buff.darkness, 30, "Darkness");
        }

        public static CrowdResponse GiveBuffFrozen(ControlClient client, CrowdRequest req)
        {
            return DoGiveBuff(req, Buff.frozen, 10, "Frozen");
        }

        public static CrowdResponse GiveBuffInvincibility(ControlClient client, CrowdRequest req)
        {
            return DoGiveBuff(req, Buff.yobaBlessing, 30, "Invincibility");
        }

        public static CrowdResponse GiveBuffNauseous(ControlClient client, CrowdRequest req)
        {
            return DoGiveBuff(req, Buff.nauseous, 60, "Nauseous");
        }

        public static CrowdResponse GiveBuffSlime(ControlClient client, CrowdRequest req)
        {
            return DoGiveBuff(req, Buff.slimed, 10, "Slimed");
        }

        public static CrowdResponse GiveBuffSpeed(ControlClient client, CrowdRequest req)
        {
            return DoGiveBuff(req, Buff.speed, 120, "Speed Buff");
        }

        public static CrowdResponse GiveBuffTipsy(ControlClient client, CrowdRequest req)
        {
            return DoGiveBuff(req, Buff.tipsy, 120, "Tipsy");
        }

        public static CrowdResponse GiveBuffWarrior(ControlClient client, CrowdRequest req)
        {
            return DoGiveBuff(req, Buff.warriorEnergy, 30, "Warrior Energy");
        }

        public static CrowdResponse GiveMoney100(ControlClient client, CrowdRequest req)
        {
            return DoGiveMoney(req, 100);
        }

        public static CrowdResponse GiveMoney1000(ControlClient client, CrowdRequest req)
        {
            return DoGiveMoney(req, 1000);
        }

        public static CrowdResponse GiveMoney10000(ControlClient client, CrowdRequest req)
        {
            return DoGiveMoney(req, 10000);
        }

        public static CrowdResponse GiveStardrop(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int stamina = Game1.player.MaxStamina;
            if (stamina == 508)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at the highest energy maximum";
            }
            else
            {
                stamina += 34;
                Game1.player.MaxStamina = stamina;
                Game1.player.Stamina = stamina;
                UI.ShowInfo($"{req.GetReqViewer()} gave {Game1.player.Name} a Stardrop");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Heal10(ControlClient client, CrowdRequest req)
        {
            return DoHealBy(req, 0.1f);
        }

        public static CrowdResponse Heal25(ControlClient client, CrowdRequest req)
        {
            return DoHealBy(req, 0.25f);
        }

        public static CrowdResponse Heal50(ControlClient client, CrowdRequest req)
        {
            return DoHealBy(req, 0.5f);
        }

        public static CrowdResponse HealFull(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Interlocked.Exchange(ref Game1.player.health, Game1.player.maxHealth) == 0)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo($"{req.GetReqViewer()} fully healed {Game1.player.Name}");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Hurt10(ControlClient client, CrowdRequest req)
        {
            return DoHurtBy(req, 0.1f);
        }

        public static CrowdResponse Hurt25(ControlClient client, CrowdRequest req)
        {
            return DoHurtBy(req, 0.25f);
        }

        public static CrowdResponse Hurt50(ControlClient client, CrowdRequest req)
        {
            return DoHurtBy(req, 0.5f);
        }

        public static CrowdResponse Kill(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Interlocked.Exchange(ref Game1.player.health, 0) == 0)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo($"{req.GetReqViewer()} killed {Game1.player.Name}");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse PassOut(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            float stamina = Game1.player.Stamina;
            if (stamina > -16)
            {
                Game1.player.Stamina = -16;
                UI.ShowInfo($"{req.GetReqViewer()} made {Game1.player.Name} pass out");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently passed out";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse RemoveMoney100(ControlClient client, CrowdRequest req)
        {
            return DoRemoveMoney(req, 100);
        }

        public static CrowdResponse RemoveMoney1000(ControlClient client, CrowdRequest req)
        {
            return DoRemoveMoney(req, 1000);
        }

        public static CrowdResponse RemoveMoney10000(ControlClient client, CrowdRequest req)
        {
            return DoRemoveMoney(req, 10000);
        }

        public static CrowdResponse RemoveStardrop(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int stamina = Game1.player.MaxStamina;
            if (stamina == 270)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at lowest energy maximum";
            }
            else
            {
                stamina -= 34;
                Game1.player.MaxStamina = stamina;
                if (Game1.player.Stamina > stamina)
                    Game1.player.Stamina = stamina;
                UI.ShowInfo($"{req.GetReqViewer()} removed a Stardrop from {Game1.player.Name}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse SpawnBat(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Bat(GetRandomNear(), 20));
        }

        public static CrowdResponse SpawnFly(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Fly(GetRandomNear()));
        }

        public static CrowdResponse SpawnGhost(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Ghost(GetRandomNear()));
        }

        public static CrowdResponse SpawnLavaBat(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Bat(GetRandomNear(), 100));
        }

        public static CrowdResponse SpawnFrostBat(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Bat(GetRandomNear(), 60));
        }

        public static CrowdResponse SpawnSerpent(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Serpent(GetRandomNear()));
        }

        public static CrowdResponse Tire10(ControlClient client, CrowdRequest req)
        {
            return DoTireBy(req, 0.1f);
        }

        public static CrowdResponse Tire25(ControlClient client, CrowdRequest req)
        {
            return DoTireBy(req, 0.25f);
        }

        public static CrowdResponse Tire50(ControlClient client, CrowdRequest req)
        {
            return DoTireBy(req, 0.5f);
        }

        public static CrowdResponse UpgradeAxe(ControlClient client, CrowdRequest req)
        {
            return DoUpgrade(req, "Axe");
        }

        public static CrowdResponse UpgradeBackpack(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Game1.player.items.Capacity == 36)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + "'s Backpack is already at maximum capacity";
            }
            else
            {
                Game1.player.increaseBackpackSize(12);
                UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s Backpack");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse UpgradeBoots(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            StardewBoots boots = Game1.player.boots.Get();
            if (boots == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is not currently wearing Boots";
            }
            else
            {
                boots = Boots.GetUpgrade(boots.getStatsIndex());
                if (boots == null)
                {
                    status = CrowdResponse.Status.STATUS_FAILURE;
                    message = Game1.player.Name + "'s Boots are already at the highest upgrade level";
                }
                else
                {
                    Game1.player.boots.Value = boots;
                    Game1.player.changeShoeColor(boots.indexInColorSheet);
                    UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s Boots");
                }
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse UpgradeFishingRod(ControlClient client, CrowdRequest req)
        {
            int id = req.GetReqID();

            foreach (KeyValuePair<string, int> upgrade in upgradeFishingRods)
            {
                Tool tool = Game1.player.getToolFromName(upgrade.Key);
                if (tool != null)
                {
                    tool.UpgradeLevel = upgrade.Value;
                    UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s Fishing Rod");

                    return new CrowdResponse(id);
                }
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Fishing Rod is already at the highest upgrade level");
        }

        public static CrowdResponse UpgradeHoe(ControlClient client, CrowdRequest req)
        {
            return DoUpgrade(req, "Hoe");
        }

        public static CrowdResponse UpgradePickaxe(ControlClient client, CrowdRequest req)
        {
            return DoUpgrade(req, "Pickaxe");
        }

        public static CrowdResponse UpgradeTrashCan(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Game1.player.trashCanLevel < 4)
            {
                Interlocked.Increment(ref Game1.player.trashCanLevel);
                UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s Trash Can");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + "'s Trash Can is already at the highest upgrade level";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse UpgradeWeapon(ControlClient client, CrowdRequest req)
        {
            int id = req.GetReqID();

            if (WeaponClass.Club.DoUpgrade() || WeaponClass.Sword.DoUpgrade() || WeaponClass.Dagger.DoUpgrade())
            {
                UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s Weapon");
                return new CrowdResponse(id);
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Weapon is already at the highest upgrade level");
        }

        public static CrowdResponse UpgradeWateringCan(ControlClient client, CrowdRequest req)
        {
            return DoUpgrade(req, "Watering Can");
        }

        public static CrowdResponse WarpBeach(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Beach", 20, 4);
        }

        public static CrowdResponse WarpDesert(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Desert", 35, 43);
        }

        public static CrowdResponse WarpFarm(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Farm", 48, 7);
        }

        public static CrowdResponse WarpIsland(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "IslandSouth", 11, 11);
        }

        public static CrowdResponse WarpMountain(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Mountain", 31, 20);
        }

        public static CrowdResponse WarpRailroad(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Railroad", 35, 52);
        }

        public static CrowdResponse WarpSewer(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Sewer", 16, 13);
        }

        public static CrowdResponse WarpTower(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Forest", 5, 29);
        }

        public static CrowdResponse WarpTown(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Town", 29, 67);
        }

        public static CrowdResponse WarpWoods(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Woods", 55, 15);
        }

        private static CrowdResponse DoDowngrade(CrowdRequest req, string toolName)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            Tool tool = Game1.player.getToolFromName(toolName);
            if (tool == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = $"{Game1.player.Name}'s {toolName} is already at the lowest upgrade level";
            }
            else
            {
                int level = tool.UpgradeLevel;
                if (level == 0)
                    status = CrowdResponse.Status.STATUS_FAILURE;
                else
                {
                    tool.UpgradeLevel = level - 1;
                    UI.ShowInfo($"{req.GetReqViewer()} downgraded {Game1.player.Name}'s {toolName}");
                }
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoEnergizeBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int max = Game1.player.MaxStamina;
            float stamina = Game1.player.Stamina;
            if (stamina < max)
            {
                stamina += percent * max;
                Game1.player.Stamina = (stamina > max) ? max : stamina;
                UI.ShowInfo($"{req.GetReqViewer()} energized {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at maximum energy";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoGiveBuff(CrowdRequest req, int buff, int duration, string name)
        {
            new Thread(new BuffThread(buff, duration * 1000).Run).Start();
            UI.ShowInfo($"{req.GetReqViewer()} gave {Game1.player.Name} the {name} effect for {duration} seconds");
            return new CrowdResponse(req.GetReqID());
        }

        private static CrowdResponse DoGiveMoney(CrowdRequest req, int amount)
        {
            Game1.player.addUnearnedMoney(amount);
            UI.ShowInfo($"{req.GetReqViewer()} gave {Game1.player.Name} {amount} coins");
            return new CrowdResponse(req.GetReqID());
        }

        private static CrowdResponse DoHealBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int max = Game1.player.maxHealth;
            int health = (int)Math.Floor(percent * max) + Game1.player.health;
            if (Interlocked.Exchange(ref Game1.player.health, (health > max) ? max : health) == 0)
            {
                Game1.player.health = 0;
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo($"{req.GetReqViewer()} healed {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoHurtBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int health = Game1.player.health - (int)Math.Floor(percent * Game1.player.maxHealth);
            if (Interlocked.Exchange(ref Game1.player.health, (health < 0) ? 0 : health) == 0)
            {
                Game1.player.health = 0;
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already dead";
            }
            else
                UI.ShowInfo($"{req.GetReqViewer()} hurt {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoRemoveMoney(CrowdRequest req, int amount)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int money = Game1.player.Money;
            if (money > 0)
            {
                money -= amount;
                Game1.player.Money = (money < 0) ? 0 : money;
                UI.ShowInfo($"{req.GetReqViewer()} removed {amount} coins from {Game1.player.Name}");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " currently has no money";
            }
            
            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoSpawn(ControlClient client, CrowdRequest req, Monster monster)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (client.CanSpawn())
            {
                Game1.player.currentLocation.addCharacter(monster);
                client.TrackMonster(monster);
                UI.ShowInfo($"{req.GetReqViewer()} spawned a {monster.Name} near {Game1.player.Name}");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = $"Cannot spawn {monster.Name} because {Game1.player.Name} is at {Game1.player.currentLocation.Name}";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoTireBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            float stamina = Game1.player.Stamina;
            if (stamina > 0)
            {
                stamina -= percent * Game1.player.MaxStamina;
                Game1.player.Stamina = (stamina < 0) ? 0 : stamina;
                UI.ShowInfo($"{req.GetReqViewer()} tired {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already passed out";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoUpgrade(CrowdRequest req, string toolName, int max = 4)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            Tool tool = Game1.player.getToolFromName(toolName);
            if (tool == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = $"{Game1.player.Name}'s {toolName} is already at the highest upgrade level";
            }
            else
            {
                int level = tool.UpgradeLevel;
                if (level == max)
                    status = CrowdResponse.Status.STATUS_FAILURE;
                else
                {
                    tool.UpgradeLevel = level + 1;
                    UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s {toolName}");
                }
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoWarp(CrowdRequest req, string name, int targetX, int targetY)
        {
            try
            {
                Game1.player.resetState();
                Game1.warpFarmer(name, targetX, targetY, false);
            }
            catch (Exception e)
            {
                UI.ShowError(e.Message);
            }
            if (name == "Forest")
                name = "Wizard's Tower";
            else if (name == "IslandSouth")
                name = "Island";
            UI.ShowInfo($"{req.GetReqViewer()} warped {Game1.player.Name} to the {name}");
            return new CrowdResponse(req.GetReqID());
        }

        private static readonly float MAX_RADIUS = 400;

        private static Vector2 GetRandomNear()
        {
            Random random = new Random();
            return Game1.player.Position + new Vector2(
                (float)((random.NextDouble() * 2 * MAX_RADIUS) - MAX_RADIUS),
                (float)((random.NextDouble() * 2 * MAX_RADIUS) - MAX_RADIUS));
        }
    }
}
