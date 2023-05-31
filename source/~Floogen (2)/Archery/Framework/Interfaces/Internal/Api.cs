/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Models.Generic;
using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects.Items;
using Archery.Framework.Objects.Projectiles;
using Archery.Framework.Objects.Weapons;
using Archery.Framework.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;

namespace Archery.Framework.Interfaces.Internal
{
    public class Api : IApi
    {
        private IMonitor _monitor;
        internal static IModHelper _helper;
        private Dictionary<string, Func<ISpecialAttack, bool>> _registeredSpecialAttackMethods;
        private Dictionary<string, SpecialAttack> _registeredSpecialAttackData;
        private Dictionary<string, Func<IEnchantment, bool>> _registeredEnchantmentMethods;
        private Dictionary<string, Enchantment> _registeredEnchantmentData;

        public event EventHandler<IWeaponFiredEventArgs> OnWeaponFired;
        public event EventHandler<IWeaponChargeEventArgs> OnWeaponCharging;
        public event EventHandler<IWeaponChargeEventArgs> OnWeaponCharged;
        public event EventHandler<ICrossbowLoadedEventArgs> OnCrossbowLoaded;
        public event EventHandler<IAmmoChangedEventArgs> OnAmmoChanged;
        public event EventHandler<IAmmoHitMonsterEventArgs> OnAmmoHitMonster;


        internal Api(IMonitor monitor, IModHelper helper)
        {
            _monitor = monitor;
            _helper = helper;
            _registeredSpecialAttackMethods = new Dictionary<string, Func<ISpecialAttack, bool>>();
            _registeredSpecialAttackData = new Dictionary<string, SpecialAttack>();
            _registeredEnchantmentMethods = new Dictionary<string, Func<IEnchantment, bool>>();
            _registeredEnchantmentData = new Dictionary<string, Enchantment>();
        }

        private void GenerateApiMessage(IManifest callerManifest, string message, bool logOnce = true, LogLevel logLevel = LogLevel.Trace)
        {
            if (logOnce)
            {
                _monitor.LogOnce($"[API][{callerManifest.UniqueID}]: {message}", logLevel);
            }
            else
            {
                _monitor.Log($"[API][{callerManifest.UniqueID}]: {message}", logLevel);
            }
        }

        #region Events
        internal void TriggerOnWeaponFired(IWeaponFiredEventArgs weaponFiredEventArgs)
        {
            var handler = OnWeaponFired;
            if (handler is not null)
            {
                handler(this, weaponFiredEventArgs);
            }
        }

        internal void TriggerOnWeaponCharging(IWeaponChargeEventArgs weaponChargeEventArgs)
        {
            var handler = OnWeaponCharging;
            if (handler is not null)
            {
                handler(this, weaponChargeEventArgs);
            }
        }

        internal void TriggerOnWeaponCharged(IWeaponChargeEventArgs weaponChargeEventArgs)
        {
            var handler = OnWeaponCharged;
            if (handler is not null)
            {
                handler(this, weaponChargeEventArgs);
            }
        }

        internal void TriggerOnCrossbowLoaded(ICrossbowLoadedEventArgs crossbowLoadedEventArgs)
        {
            var handler = OnCrossbowLoaded;
            if (handler is not null)
            {
                handler(this, crossbowLoadedEventArgs);
            }
        }

        internal void TriggerOnAmmoChanged(IAmmoChangedEventArgs ammoChangedEventArgs)
        {
            var handler = OnAmmoChanged;
            if (handler is not null)
            {
                handler(this, ammoChangedEventArgs);
            }
        }

        internal void TriggerOnAmmoHitMonster(IAmmoHitMonsterEventArgs ammoHitMonsterEventArgs)
        {
            var handler = OnAmmoHitMonster;
            if (handler is not null)
            {
                handler(this, ammoHitMonsterEventArgs);
            }
        }
        #endregion

        #region Special attacks internals
        internal bool HandleSpecialAttack(WeaponType weaponType, string specialAttackId, ISpecialAttack specialAttack)
        {
            if (_registeredSpecialAttackMethods.ContainsKey(specialAttackId) is false)
            {
                return false;
            }

            var registeredWeaponTypeFilter = _registeredSpecialAttackData[specialAttackId].WeaponType;
            if (registeredWeaponTypeFilter != weaponType && registeredWeaponTypeFilter != WeaponType.Any)
            {
                return false;
            }

            var specialAttackMethod = _registeredSpecialAttackMethods[specialAttackId];
            if (specialAttackMethod(specialAttack) is true)
            {
                _monitor.LogOnce($"Using special attack {specialAttackId}", LogLevel.Trace);
                return true;
            }

            return false;
        }

        internal string GetSpecialAttackName(string specialAttackId, List<object> arguments)
        {
            if (_registeredSpecialAttackData.ContainsKey(specialAttackId) is false)
            {
                return null;
            }

            return _registeredSpecialAttackData[specialAttackId].GetName(arguments);
        }

        internal string GetSpecialAttackDescription(string specialAttackId, List<object> arguments)
        {
            if (_registeredSpecialAttackData.ContainsKey(specialAttackId) is false)
            {
                return null;
            }

            return _registeredSpecialAttackData[specialAttackId].GetDescription(arguments);
        }

        internal int GetSpecialAttackCooldown(string specialAttackId, List<object> arguments)
        {
            if (_registeredSpecialAttackData.ContainsKey(specialAttackId) is false)
            {
                return 0;
            }

            return _registeredSpecialAttackData[specialAttackId].GetCooldownInMilliseconds(arguments);
        }

        private string GetSpecialAttackId(IManifest callerManifest, string name)
        {
            return $"{callerManifest.UniqueID}/{name}";
        }
        #endregion

        #region Enchantment internals
        internal bool HandleEnchantment(AmmoType ammoType, string enchantmentId, IEnchantment enchantment)
        {
            if (_registeredEnchantmentMethods.ContainsKey(enchantmentId) is false)
            {
                return false;
            }

            var registeredAmmoTypeFilter = _registeredEnchantmentData[enchantmentId].AmmoType;
            if (registeredAmmoTypeFilter != ammoType && registeredAmmoTypeFilter != AmmoType.Any)
            {
                return false;
            }

            var specialAttackMethod = _registeredEnchantmentMethods[enchantmentId];
            if (specialAttackMethod(enchantment) is true)
            {
                _monitor.LogOnce($"Using special attack {enchantmentId}", LogLevel.Trace);
                return true;
            }

            return false;
        }

        internal string GetEnchantmentName(string enchantmentId, List<object> arguments)
        {
            if (_registeredEnchantmentData.ContainsKey(enchantmentId) is false)
            {
                return null;
            }

            return _registeredEnchantmentData[enchantmentId].GetName(arguments);
        }

        internal string GetEnchantmentDescription(string enchantmentId, List<object> arguments)
        {
            if (_registeredEnchantmentData.ContainsKey(enchantmentId) is false)
            {
                return null;
            }

            return _registeredEnchantmentData[enchantmentId].GetDescription(arguments);
        }

        internal TriggerType GetEnchantmentTriggerType(string enchantmentId)
        {
            if (_registeredEnchantmentData.ContainsKey(enchantmentId) is false)
            {
                return TriggerType.Unknown;
            }

            return _registeredEnchantmentData[enchantmentId].TriggerType;
        }

        private string GetEnchantmentId(IManifest callerManifest, string name)
        {
            return $"{callerManifest.UniqueID}/{name}";
        }
        #endregion

        public Item CreateWeapon(IManifest callerManifest, string weaponModelId)
        {
            if (Archery.modelManager.DoesModelExist<WeaponModel>(weaponModelId) is false)
            {
                GenerateApiMessage(callerManifest, $"Given weaponModelId {weaponModelId} does not exist");
                return null;
            }
            GenerateApiMessage(callerManifest, $"Creating weapon instance with weaponModelId {weaponModelId}");

            return Bow.CreateInstance(Archery.modelManager.GetSpecificModel<WeaponModel>(weaponModelId));
        }

        public Item CreateAmmo(IManifest callerManifest, string ammoModelId)
        {
            if (Archery.modelManager.DoesModelExist<AmmoModel>(ammoModelId) is false)
            {
                GenerateApiMessage(callerManifest, $"Given ammoModelId {ammoModelId} does not exist");
                return null;
            }
            GenerateApiMessage(callerManifest, $"Creating ammo instance with ammoModelId {ammoModelId}");

            return Arrow.CreateInstance(Archery.modelManager.GetSpecificModel<AmmoModel>(ammoModelId));
        }

        public bool PlaySound(IManifest callerManifest, ISound sound, Vector2 position)
        {
            if (sound is null)
            {
                GenerateApiMessage(callerManifest, "Given ISound is not a valid");
                return false;
            }

            if (Toolkit.PlaySound((Sound)sound, callerManifest.UniqueID, position) is false)
            {
                GenerateApiMessage(callerManifest, "Failed to play ISound, see log for details");
                return false;
            }

            GenerateApiMessage(callerManifest, $"Played sound {sound.Name} at {position}");
            return true;
        }

        public int? GetSpecialAttackCooldown(IManifest callerManifest, Slingshot slingshot)
        {
            if (slingshot is null)
            {
                GenerateApiMessage(callerManifest, "Given slingshot is null");
                return null;
            }

            if (Bow.IsValid(slingshot) is false)
            {
                GenerateApiMessage(callerManifest, "Given slingshot is not valid Archery object");
                return null;
            }

            if (Bow.GetModel<WeaponModel>(slingshot) is WeaponModel weaponModel && weaponModel.SpecialAttack is not null)
            {
                return GetSpecialAttackCooldown(weaponModel.SpecialAttack.Id, weaponModel.SpecialAttack.Arguments);
            }

            GenerateApiMessage(callerManifest, "Given slingshot does not have a SpecialAttack associated to it");
            return null;
        }

        public int GetCooldownRemaining(IManifest callerManifest)
        {
            return Bow.ActiveCooldown;
        }

        public void SetCooldownRemaining(IManifest callerManifest, int cooldownInMilliseconds)
        {
            GenerateApiMessage(callerManifest, $"Set the weapon cooldown to {cooldownInMilliseconds}");
            Bow.ActiveCooldown = cooldownInMilliseconds;
        }

        public IWeaponData GetWeaponData(IManifest callerManifest, Slingshot slingshot)
        {
            if (slingshot is null)
            {
                GenerateApiMessage(callerManifest, "Given slingshot is null");
                return null;
            }

            if (Bow.IsValid(slingshot) is false)
            {
                GenerateApiMessage(callerManifest, "Given slingshot is not valid Archery object");
                return null;
            }

            return Bow.GetData(slingshot);
        }

        public IProjectileData GetProjectileData(IManifest callerManifest, BasicProjectile projectile)
        {
            if (projectile is null)
            {
                GenerateApiMessage(callerManifest, "Given projectile is null");
                return null;
            }

            if (projectile is not ArrowProjectile arrowProjectile)
            {
                GenerateApiMessage(callerManifest, "Given projectile is not valid ArrowProjectile");
                return null;
            }

            return arrowProjectile.GetData();
        }

        public bool SetProjectileData(IManifest callerManifest, BasicProjectile projectile, IProjectileData data)
        {
            if (projectile is null)
            {
                GenerateApiMessage(callerManifest, "Given projectile is null");
                return false;
            }

            if (projectile is not ArrowProjectile arrowProjectile)
            {
                GenerateApiMessage(callerManifest, "Given projectile is not valid ArrowProjectile");
                return false;
            }

            arrowProjectile.Override(data);
            GenerateApiMessage(callerManifest, "Overrode the projectile with the given IProjectile");

            return true;
        }

        public bool SetChargePercentage(IManifest callerManifest, Slingshot slingshot, float percentage)
        {
            if (Bow.IsValid(slingshot) is false)
            {
                GenerateApiMessage(callerManifest, "Given slingshot object is not a valid Archery.WeaponModel");
                return false;
            }

            Bow.SetSlingshotChargeTime(slingshot, percentage);
            GenerateApiMessage(callerManifest, $"Set Archery.WeaponModel's charge percentage to {percentage}");

            return true;
        }

        public BasicProjectile PerformFire(IManifest callerManifest, BasicProjectile projectile, Slingshot slingshot, GameLocation location, Farmer who, bool suppressFiringSound = false)
        {
            if (Bow.IsValid(slingshot) is false)
            {
                GenerateApiMessage(callerManifest, "Given slingshot object is not a valid Archery.WeaponModel");
                return null;
            }

            var arrow = Bow.PerformFire(projectile, null, slingshot, location, who, suppressFiringSound);
            if (arrow is null)
            {
                GenerateApiMessage(callerManifest, "Bow.PerformFire returned null");
                return null;
            }

            return arrow;
        }

        public BasicProjectile PerformFire(IManifest callerManifest, string ammoId, Slingshot slingshot, GameLocation location, Farmer who, bool suppressFiringSound = false)
        {
            if (Bow.IsValid(slingshot) is false)
            {
                GenerateApiMessage(callerManifest, "Given slingshot object is not a valid Archery.WeaponModel");
                return null;
            }

            var arrow = Bow.PerformFire(Archery.modelManager.GetSpecificModel<AmmoModel>(ammoId), slingshot, location, who, suppressFiringSound);
            if (arrow is null)
            {
                arrow = Bow.PerformFire(slingshot, location, who, suppressFiringSound);

                if (arrow is null)
                {
                    GenerateApiMessage(callerManifest, "Bow.PerformFire returned null");
                    return null;
                }
            }

            return arrow;
        }

        public BasicProjectile PerformFire(IManifest callerManifest, Slingshot slingshot, GameLocation location, Farmer who, bool suppressFiringSound = false)
        {
            return PerformFire(callerManifest, ammoId: null, slingshot, location, who, suppressFiringSound);
        }

        public bool RegisterSpecialAttack(IManifest callerManifest, string name, WeaponType whichWeaponTypeCanUse, Func<List<object>, string> getDisplayName, Func<List<object>, string> getDescription, Func<List<object>, int> getCooldownMilliseconds, Func<ISpecialAttack, bool> specialAttackHandler)
        {
            string id = GetSpecialAttackId(callerManifest, name);
            _registeredSpecialAttackMethods[id] = specialAttackHandler;
            _registeredSpecialAttackData[id] = new SpecialAttack()
            {
                WeaponType = whichWeaponTypeCanUse,
                GetName = getDisplayName,
                GetDescription = getDescription,
                GetCooldownInMilliseconds = getCooldownMilliseconds
            };

            GenerateApiMessage(callerManifest, $"The mod {callerManifest.Name} registered a special attack {id} with the type {whichWeaponTypeCanUse}");
            return true;
        }

        public bool DeregisterSpecialAttack(IManifest callerManifest, string name)
        {
            string id = GetSpecialAttackId(callerManifest, name);
            if (_registeredSpecialAttackMethods.ContainsKey(id) is false)
            {
                GenerateApiMessage(callerManifest, $"There were no registered special attack methods under {id}.");
                return false;
            }

            _registeredSpecialAttackMethods.Remove(id);
            _registeredSpecialAttackData.Remove(id);

            GenerateApiMessage(callerManifest, $"Unregistered the special attack method {id}.");
            return true;
        }

        public bool RegisterEnchantment(IManifest callerManifest, string name, AmmoType whichAmmoTypeCanUse, TriggerType triggerType, Func<List<object>, string> getDisplayName, Func<List<object>, string> getDescription, Func<IEnchantment, bool> enchantmentHandler)
        {
            string id = GetSpecialAttackId(callerManifest, name);
            _registeredEnchantmentMethods[id] = enchantmentHandler;
            _registeredEnchantmentData[id] = new Enchantment()
            {
                AmmoType = whichAmmoTypeCanUse,
                TriggerType = triggerType,
                GetName = getDisplayName,
                GetDescription = getDescription
            };

            GenerateApiMessage(callerManifest, $"The mod {callerManifest.Name} registered an enchantment {id} with the type {whichAmmoTypeCanUse}");
            return true;
        }

        public bool DeregisterEnchantment(IManifest callerManifest, string name)
        {
            string id = GetSpecialAttackId(callerManifest, name);
            if (_registeredEnchantmentMethods.ContainsKey(id) is false)
            {
                GenerateApiMessage(callerManifest, $"There were no registered enchantments methods under {id}.");
                return false;
            }

            _registeredEnchantmentMethods.Remove(id);
            _registeredEnchantmentData.Remove(id);

            GenerateApiMessage(callerManifest, $"Unregistered the enchantment method {id}.");
            return true;
        }
    }
}
