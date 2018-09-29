using StardewValley.Tools;
using TehPers.Stardew.Framework;

namespace TehPers.Stardew.CombatOverhaul {
    public class ModWeapon : MeleeWeapon {

        public ModWeapon(int id, string name) {
            this.name = name;
            this.category = -98;
            this.name = name;
            this.description = "";
            this.minDamage = 0;
            this.maxDamage = 0;
            this.knockback = 0f;
            this.speed = 1;
            this.addedPrecision = 0;
            this.addedDefense = 0;
            this.type = 1;
            this.addedAreaOfEffect = 0;
            this.critChance = 0;
            this.critMultiplier = 1;
            this.Stack = 1;
            this.initialParentTileIndex = id;
            this.currentParentTileIndex = this.initialParentTileIndex;
            this.indexOfMenuItemView = this.currentParentTileIndex;
        }

        /// <summary>Converts a MeleeWeapon into a ModWeapon by copying all the fields</summary>
        /// <param name="original">The original (preferably vanilla) weapon</param>
        public ModWeapon(MeleeWeapon original) {
            Helpers.CopyFields<MeleeWeapon>(original, this);
            this.name = "Modded: " + this.name;
            ModEntry.INSTANCE.Monitor.Log(name + " | Damage: " + minDamage + " - " + maxDamage);
        }

        #region Setters
        public ModWeapon setDescription(string desc) {
            this.description = desc;
            return this;
        }

        public ModWeapon setType(int type) {
            this.type = type;
            if (this.type == 0)
                this.type = 3;
            return this;
        }

        public ModWeapon setMinDamage(int n) {
            this.minDamage = n;
            return this;
        }

        public ModWeapon setMaxDamage(int n) {
            this.maxDamage = n;
            return this;
        }

        public ModWeapon setDamage(int min, int max) {
            return this.setMinDamage(min).setMaxDamage(max);
        }

        public ModWeapon setCritChance(float n) {
            this.critChance = n;
            return this;
        }

        public ModWeapon setCritMult(float n) {
            this.critMultiplier = n;
            return this;
        }

        public ModWeapon setKnockback(float n) {
            this.knockback = n;
            return this;
        }

        public ModWeapon setSpeed(int n) {
            this.speed = n;
            return this;
        }

        public ModWeapon setAddedPrecision(int n) {
            this.addedPrecision = n;
            return this;
        }

        public ModWeapon setAddedDefense(int n) {
            this.addedDefense = n;
            return this;
        }

        public ModWeapon setAddedAOE(int n) {
            this.addedAreaOfEffect = n;
            return this;
        }

        public ModWeapon setStats(int minDamage, int maxDamage, float knockback = -1, int speed = -1, int addedPrecision = -1, int addedDefense = -1, int addedAOE = -1, float critChance = -1, float critMult = -1) {
            this.minDamage = minDamage;
            this.maxDamage = maxDamage;
            if (knockback >= 0) this.knockback = knockback;
            if (speed >= 0) this.speed = speed;
            if (addedPrecision >= 0) this.addedPrecision = addedPrecision;
            if (addedDefense >= 0) this.addedDefense = addedDefense;
            if (addedAOE >= 0) this.addedAreaOfEffect = addedAOE;
            if (critChance >= 0) this.critChance = critChance;
            if (critMult >= 0) this.critMultiplier = critMult;
            return this;
        }
        #endregion
    }
}
