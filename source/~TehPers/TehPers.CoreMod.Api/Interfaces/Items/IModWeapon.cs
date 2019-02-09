namespace TehPers.CoreMod.Api.Items {
    public interface IModWeapon : IModItem {
        /// <summary>Gets the raw information that should be added to "Data/weapons".</summary>
        /// <returns>The raw information string.</returns>
        string GetRawWeaponInformation();
    }
}