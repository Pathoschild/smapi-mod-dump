/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jasisco5/UncannyValleyMod
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Enchantments;
using StardewValley.GameData.Weapons;
using StardewValley.ItemTypeDefinitions;

namespace UncannyValleyMod
{
    /// <summary>
    /// Class for generating the mod's custom weapon. 
    /// Seperated to have cleaner code
    /// </summary>
    internal class ModWeapon
    {
        public ModSaveData saveModel { get; set; }
        public WeaponToken token { get; set; }

        IModHelper helper;

        public ModWeapon(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            this.helper = helper;
        }

        /// <summary>
        /// Load the Spectral Sabre into the game
        /// </summary>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // Load Custom Weapon
            if (e.Name.IsEquivalentTo("TileSheets/Weapons"))
            {
                e.LoadFromModFile<Texture2D>("assets/weapons/weapons.png", AssetLoadPriority.Medium);
            }
            // Edit in Custom Weapon
            if (e.Name.IsEquivalentTo("Data/Weapons"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, WeaponData>().Data;
                    WeaponData weaponData = new WeaponData();
                    weaponData.Name = "SpectralSabre";
                    weaponData.DisplayName = "Spectral Sabre";
                    weaponData.Description = "A blade to reap the life and energy from monsters.";
                    weaponData.MinDamage = 40;
                    weaponData.MaxDamage = 60;
                    weaponData.Type = 3;
                    weaponData.Texture = "Tilesheets/weapons";
                    weaponData.SpriteIndex = 67;

                    data["2051901"] = weaponData;

                }, AssetEditPriority.Default);
            }
        }

        public void AddWeaponToInv()
        {
            MeleeWeapon weapon = new MeleeWeapon("2051901");
            BaseWeaponEnchantment reaping = new ReapingEnchantment();
            weapon.AddEnchantment(reaping);
            weapon.ParentSheetIndex = 65;
            if(Game1.player.addItemToInventory(weapon) != null)
            {
                Game1.createItemDebris(weapon, Game1.player.position.Value, 0);
            }
            saveModel.weaponObtained = true;
            // If the state has changed, update the token and reload the map
            if (token != null && !token.weaponObtained)
            {
                token.UpdateContext();
                // Attempt to force reload the maps content patch
                //helper.GameContent.InvalidateCache("Maps/Custom_Mansion_Exterior");
            }
        }

    }
}
