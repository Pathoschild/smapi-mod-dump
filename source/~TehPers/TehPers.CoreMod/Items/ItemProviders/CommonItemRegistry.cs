using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.ItemProviders;

namespace TehPers.CoreMod.Items.ItemProviders {
    internal class CommonItemRegistry : ICommonItemRegistry {
        public CommonItemRegistry(IApiHelper apiHelper, ItemDelegator itemDelegator) {
            // Object registry
            SObjectRegistry objectRegistry = new SObjectRegistry(apiHelper, itemDelegator);
            this.Objects = objectRegistry;
            itemDelegator.AddProvider(_ => this.Objects);
            apiHelper.Owner.Helper.Content.AssetEditors.Add(objectRegistry);

            // Weapon registry
            WeaponRegistry weaponRegistry = new WeaponRegistry(apiHelper, itemDelegator);
            this.Weapons = weaponRegistry;
            itemDelegator.AddProvider(_ => this.Weapons);
            apiHelper.Owner.Helper.Content.AssetEditors.Add(weaponRegistry);
        }

        public IItemRegistry<IModObject> Objects { get; }
        public IItemRegistry<IModWeapon> Weapons { get; }
    }
}