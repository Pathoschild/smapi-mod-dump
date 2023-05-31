/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces.Internal;
using StardewValley;
using StardewValley.Tools;

namespace Archery.Framework.Utilities.SpecialAttacks
{
    public class Snapshot
    {
        private static int _internalTimer = 0;

        internal static bool HandleSpecialAttack(ISpecialAttack specialAttack)
        {
            var slingshot = specialAttack.Slingshot;

            var weaponData = Archery.internalApi.GetWeaponData(Archery.manifest, slingshot);
            if (weaponData is null)
            {
                return false;
            }

            if (weaponData.WeaponType is WeaponType.Bow)
            {
                return HandleSpecialAttackForBow(specialAttack, weaponData, slingshot);
            }
            else if (weaponData.WeaponType is WeaponType.Crossbow)
            {
                return HandleSpecialAttackForCrossbow(specialAttack, weaponData, slingshot);
            }

            return false;
        }

        internal static bool HandleSpecialAttackForBow(ISpecialAttack specialAttack, IWeaponData weaponData, Slingshot slingshot)
        {
            var currentChargeTime = slingshot.GetSlingshotChargeTime();
            if (currentChargeTime < 0.5f)
            {
                Archery.internalApi.SetChargePercentage(Archery.manifest, slingshot, 0.5f);
            }
            else if (currentChargeTime >= 1f)
            {
                HandlePerformFire(specialAttack, slingshot);
            }

            if (GetShotsFired(slingshot) >= 2)
            {
                SetShotsFired(slingshot, 0);

                return false;
            }

            return true;
        }

        internal static bool HandleSpecialAttackForCrossbow(ISpecialAttack specialAttack, IWeaponData weaponData, Slingshot slingshot)
        {
            if (weaponData.AmmoInMagazine == 0)
            {
                Game1.showRedMessage("The weapon must be loaded before using the special attack!");
                Archery.internalApi.SetCooldownRemaining(Archery.manifest, 0);
                return false;
            }

            _internalTimer += (int)specialAttack.Time.ElapsedGameTime.TotalMilliseconds;
            if (_internalTimer > 250)
            {
                _internalTimer = 0;
                HandlePerformFire(specialAttack, slingshot);
            }

            if (GetShotsFired(slingshot) >= 2)
            {
                SetShotsFired(slingshot, 0);

                return false;
            }

            return true;
        }

        internal static int GetShotsFired(Slingshot slingshot)
        {
            int shotsFired = 0;
            if (slingshot.modData.TryGetValue(ModDataKeys.SPECIAL_ATTACK_SNAPSHOT_COUNT, out string rawShotsFired) is false || int.TryParse(rawShotsFired, out shotsFired) is false)
            {
                slingshot.modData[ModDataKeys.SPECIAL_ATTACK_SNAPSHOT_COUNT] = shotsFired.ToString();
            }

            return shotsFired;
        }

        internal static void SetShotsFired(Slingshot slingshot, int count)
        {
            slingshot.modData[ModDataKeys.SPECIAL_ATTACK_SNAPSHOT_COUNT] = count.ToString();
        }

        internal static void HandlePerformFire(ISpecialAttack specialAttack, Slingshot slingshot)
        {
            int shotsFired = GetShotsFired(slingshot);

            Archery.internalApi.PerformFire(Archery.manifest, slingshot, specialAttack.Location, specialAttack.Farmer);

            SetShotsFired(slingshot, shotsFired + 1);
        }
    }
}
