/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using StardewValley.Buffs;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterStardrops {
    internal class BuffMaker {

        internal Buff CreateAttackBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropAttackBuff",
                displayName: "Stardrop Attack Power",
                description: "Additional attack from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    Attack = { amount }
                }
            );

            return newBuff;
        }

        internal Buff CreateDefenseBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropDefenseBuff",
                displayName: "Stardrop Defense Power",
                description: "Additional defense from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    Defense = { amount }
                }
            );

            return newBuff;
        }

        internal Buff CreateImmunityBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropImmunityBuff",
                displayName: "Stardrop Immunity Power",
                description: "Additional immunity from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    Immunity = { amount },
                }
            );

            return newBuff;
        }

        internal Buff CreateStaminaBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropStaminaBuff",
                displayName: "Stardrop Stamina Power",
                description: "Additional stamina from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    MaxStamina = { amount }
                }
            );

            return newBuff;
        }

        internal Buff CreateCombatLevelBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropCombatLevelBuff",
                displayName: "Stardrop Combat Level Power",
                description: "Additional combat skill level from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    CombatLevel = { amount }
                }
            );

            return newBuff;
        }

        internal Buff CreateFarmingLevelBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropFarmingLevelBuff",
                displayName: "Stardrop Farming Level Power",
                description: "Additional farming skill level from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    FarmingLevel = { amount }
                }
            );

            return newBuff;
        }

        internal Buff CreateFishingLevelBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropFishingLevelBuff",
                displayName: "Stardrop Fishing Level Power",
                description: "Additional fishing skill level from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    FishingLevel = { amount }
                }
            );

            return newBuff;
        }

        internal Buff CreateForagingLevelBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropForagingLevelBuff",
                displayName: "Stardrop Foraging Level Power",
                description: "Additional foraging skill level from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    ForagingLevel = { amount }
                }
            );

            return newBuff;
        }

        internal Buff CreateLuckLevelBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropLuckLevelBuff",
                displayName: "Stardrop Luck Level Power",
                description: "Additional luck skill level from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    LuckLevel = { amount }
                }
            );

            return newBuff;
        }

        internal Buff CreateMiningLevelBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropMiningLevelBuff",
                displayName: "Stardrop Mining Level Power",
                description: "Additional mining skill level from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    MiningLevel = { amount }
                }
            );

            return newBuff;
        }

        internal Buff CreateMagneticBuff(int amount) {
            Buff newBuff = new Buff(
                id: "{{ModId}}_StardropMagneticBuff",
                displayName: "Stardrop Magnetic Power",
                description: "Additional magnetic radius from consuming stardrops",
                duration: Buff.ENDLESS,
                effects: new BuffEffects() {
                    MagneticRadius = { amount }
                }
            );

            return newBuff;
        }

    }
}
