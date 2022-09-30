/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Graphics;
using Omegasis.StardustCore.Animations;
using Omegasis.StardustCore.UIUtilities;

namespace Omegasis.Revitalize.Framework.Managers
{
    public class TextureManagers
    {
        private static bool HasLoadedTextureManagers;

        public static TextureManager HUD;

        public static TextureManager Items_Resources_Ore;
        public static TextureManager Items_Crafting;
        public static TextureManager Items_Farming;
        public static TextureManager Items_Misc;

        public static TextureManager Objects_Crafting;
        public static TextureManager Objects_Farming;
        public static TextureManager Objects_Furniture;
        public static TextureManager Objects_Machines;
        public static TextureManager Objects_Resources_ResourcePlants;

        public static TextureManager Menus_Misc;
        public static TextureManager Menus_CraftingMenu;
        public static TextureManager Menus_EnergyMenu;
        public static TextureManager Menus_InventoryMenu;

        public static TextureManager Resources_Misc;
        public static TextureManager Resources_Ore;


        public static TextureManager Tools;

        /// <summary>
        /// Loads in textures to be used by the mod.
        /// </summary>
        public static void loadInTextures()
        {

            if (HasLoadedTextureManagers) return;

            //HUD
            HUD = InitializeTextureManager("Revitalize.HUD", HudGraphicsPaths.HUD);

            //Items
            Items_Resources_Ore = InitializeTextureManager("Revitalize.Items.Resources.Ore", ItemsGraphicsPaths.Resources_Ore);
            Items_Crafting = InitializeTextureManager("Revitalize.Items.Crafting", ItemsGraphicsPaths.Crafting);
            Items_Farming = InitializeTextureManager("Revitalize.Items.Farming", ItemsGraphicsPaths.Farming);
            Items_Misc = InitializeTextureManager("Revitalize.Items.Misc", ItemsGraphicsPaths.Misc);

            //World Objects
            Objects_Crafting = InitializeTextureManager("Revitalize.Objects.Crafting", ObjectsGraphicsPaths.Crafting);
            Objects_Farming = InitializeTextureManager("Revitalize.Objects.Farming", ObjectsGraphicsPaths.Farming);
            Objects_Furniture = InitializeTextureManager("Revitalize.Objects.Furniture", ObjectsGraphicsPaths.Furniture);
            Objects_Machines = InitializeTextureManager("Revitalize.Objects.Machines", ObjectsGraphicsPaths.Machines);
            Objects_Resources_ResourcePlants = InitializeTextureManager("Revitalize.Objects.Resources.ResourcePlants", ObjectsGraphicsPaths.Resources_ResourcePlants);

            //Menus
            Menus_Misc = InitializeTextureManager("Revitalize.Menus", MenusGraphicPaths.Menus);
            Menus_CraftingMenu = InitializeTextureManager("Revitalize.Menus.CraftingMenu", MenusGraphicPaths.CraftingMenu);
            Menus_EnergyMenu = InitializeTextureManager("Revitalize.Menus.EnergyMenu", MenusGraphicPaths.EnergyMenu);
            Menus_InventoryMenu = InitializeTextureManager("Revitalize.Menus.InventoryMenu", MenusGraphicPaths.InventoryMenu);

            //Resources
            Resources_Ore = InitializeTextureManager("Revitalize.Resources.Ore", ObjectsGraphicsPaths.Resources_Ore);
            Resources_Misc = InitializeTextureManager("Revitalize.Items.Resources.Misc", ItemsGraphicsPaths.Resources_Misc);

            //Tools
            Tools = InitializeTextureManager("Revitalize.Tools", ItemsGraphicsPaths.Tools);

            HasLoadedTextureManagers = true;
        }

        private static TextureManager InitializeTextureManager(string TextureManagerId, string TextureManagerPathToSearch)
        {
            TextureManager.AddTextureManager(RevitalizeModCore.Instance.Helper.DirectoryPath, RevitalizeModCore.Instance.ModManifest, TextureManagerId);
            TextureManager textureManager = TextureManager.GetTextureManager(RevitalizeModCore.Instance.ModManifest, TextureManagerId);
            textureManager.searchForTextures(RevitalizeModCore.ModHelper, RevitalizeModCore.Manifest, TextureManagerPathToSearch);
            return textureManager;
        }

        public static TextureManager GetTextureManager(string TextureManagerId)
        {
            return TextureManager.GetTextureManager(RevitalizeModCore.Manifest, TextureManagerId) ;
        }

        public static AnimationManager CreateAnimationManager(string TextureManagerId, string TextureName ,Animation DefaultAnimation)
        {
            return TextureManager.GetTextureManager(RevitalizeModCore.Manifest, TextureManagerId).createAnimationManager(TextureName,DefaultAnimation);
        }

        public static AnimationManager CreateAnimationManager(string ModId,string TextureManagerId, string TextureName, Dictionary<string,Animation> Animations, string DefaultAnimationKey, string StartingAnimationKey)
        {
            return TextureManager.GetTextureManager(ModId, TextureManagerId).createAnimationManager(TextureName, Animations,DefaultAnimationKey,StartingAnimationKey);
        }


        public static AnimationManager createOreResourceAnimationManager(string TextureName)
        {
            return createOreResourceAnimationManager(TextureName, new Animation(0, 0, 16, 16));
        }

        public static AnimationManager createOreResourceAnimationManager(string TextureName, Animation DefaultAnimation)
        {
            return TextureManagers.Items_Resources_Ore.createAnimationManager(TextureName, DefaultAnimation);
        }

        public static AnimationManager createMiscResourceAnimationManager(string TextureName)
        {
            return createMiscResourceAnimationManager(TextureName, new Animation(0, 0, 16, 16));
        }

        public static AnimationManager createMiscResourceAnimationManager(string TextureName, Animation DefaultAnimation)
        {
            return TextureManagers.Resources_Misc.createAnimationManager(TextureName, DefaultAnimation);
        }

    }
}
