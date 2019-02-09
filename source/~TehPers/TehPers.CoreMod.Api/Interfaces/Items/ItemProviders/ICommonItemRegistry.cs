namespace TehPers.CoreMod.Api.Items.ItemProviders {
    public interface ICommonItemRegistry {
        /// <summary>Item provider for simple objects, like the ones that can be found in "Maps/springobjects".</summary>
        IItemRegistry<IModObject> Objects { get; }

        /// <summary>Item provider for weaopns, like the ones that can be found in "TileSheets/weapons".</summary>
        IItemRegistry<IModWeapon> Weapons { get; }
    }
}