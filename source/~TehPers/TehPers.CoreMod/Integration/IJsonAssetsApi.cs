using System;

namespace TehPers.CoreMod.Integration {
    public interface IJsonAssetsApi {
        int GetObjectId(string name);
        int GetCropId(string name);
        int GetFruitTreeId(string name);
        int GetBigCraftableId(string name);
        int GetHatId(string name);
        int GetWeaponId(string name);

        event EventHandler IdsAssigned;
        event EventHandler AddedItemsToShop;
    }
}