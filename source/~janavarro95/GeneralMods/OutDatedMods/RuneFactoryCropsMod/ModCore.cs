using AdditionalCropsFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneFactoryCropsMod
{
    public class ModCore : Mod
    {
        public override void Entry(IModHelper helper)
        {
            StardewModdingAPI.Events.ControlEvents.KeyPressed += ControlEvents_KeyPressed;
        }


        private void ControlEvents_KeyPressed(object sender, StardewModdingAPI.Events.EventArgsKeyPressed e)
        {
            if (e.KeyPressed == Keys.U)
            {
                List<Item> shopInventory = new List<Item>();
                string ModdedCropsFolderName = "RuneFactoryCropsMod";
                shopInventory.Add((Item)new ModularSeeds(1, Path.Combine(ModdedCropsFolderName, "SeedsGraphics.xnb"), Path.Combine(ModdedCropsFolderName, "SeedsData.xnb"), Path.Combine(ModdedCropsFolderName, "CropsGraphics.xnb"), Path.Combine(ModdedCropsFolderName, "CropsData.xnb"), Path.Combine(ModdedCropsFolderName, "CropsObjectTexture.xnb"), Path.Combine(ModdedCropsFolderName, "CropsObjectData.xnb")){
                    stack = 5
                });
                shopInventory.Add((Item)new PlanterBox(1, Vector2.Zero, Path.Combine(ModdedCropsFolderName, "PlanterBox.png"), Path.Combine(ModdedCropsFolderName, "PlanterBox.xnb")));
                shopInventory.Add((Item)new ModularCropObject(816, 1, Path.Combine(ModdedCropsFolderName, "CropsObjectTexture.xnb"), Path.Combine(ModdedCropsFolderName, "CropsObjectData.xnb")));
                Game1.activeClickableMenu = new StardewValley.Menus.ShopMenu(shopInventory);
            }
        }
    }
}
