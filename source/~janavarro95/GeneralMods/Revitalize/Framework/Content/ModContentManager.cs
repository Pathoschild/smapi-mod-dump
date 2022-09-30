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
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.ItemCategoryInformation;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Data;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Graphics;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.Managers;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles.Json;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles.Json.Crafting;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Shops;
using StardewModdingAPI;

namespace Omegasis.Revitalize.Framework.Content
{
    public class ModContentManager
    {
        public CraftingManager craftingManager;
        /// <summary>
        /// Keeps track of custom objects.
        /// </summary>
        public ObjectManager objectManager;
        public MailManager mailManager;


        public BuildingAssetLoader buildingAssetLoader;

        public ModContentManager()
        {

        }


        public virtual void initializeModContent(IManifest modManifest)
        {
            this.createDirectories();

            this.createJsonDataTemplates();

            //Loads in textures to be used by the mod.
            TextureManagers.loadInTextures();

            //Loads in objects to be use by the mod.
            this.objectManager = new ObjectManager(modManifest);


            this.mailManager = new MailManager();

            this.craftingManager = new CraftingManager();

            this.buildingAssetLoader = new BuildingAssetLoader();
            this.buildingAssetLoader.registerEvents();

        }

        public virtual void loadContentOnGameLaunched()
        {
            //Load in all objects from disk.
            this.objectManager.loadItemsFromDisk();

            //Once all objects have been initialized, then we can add references to them for recipes and initialize all of the crafting recipes for the mod.
            this.craftingManager.initializeRecipeBooks();

            this.dumpAllObjectIdsToJsonFile();
        }

        private void createDirectories()
        {
            Directory.CreateDirectory(Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, "Configs"));

            Directory.CreateDirectory(Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, RelativePaths.ModAssetsFolder));
            Directory.CreateDirectory(Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, RelativePaths.Graphics_Folder));
            Directory.CreateDirectory(Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, ObjectsGraphicsPaths.Furniture, "Chairs"));
            Directory.CreateDirectory(Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, ObjectsGraphicsPaths.Furniture, "Lamps"));
            Directory.CreateDirectory(Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, ObjectsGraphicsPaths.Furniture, "Tables"));
        }

        /// <summary>
        /// Creates empty json templates of various mod classes to be able to add/edit json files that are then converted into mod content.
        /// </summary>
        protected virtual void createJsonDataTemplates()
        {
            Content.JsonContent.Animations.JsonAnimationFrame jsonAnimationFrame = new JsonContent.Animations.JsonAnimationFrame();
            JsonUtilities.WriteJsonFile(jsonAnimationFrame, DataPaths.AnimationTemplatesPath, "AnimationFrame.json");

            Content.JsonContent.Animations.JsonAnimation jsonAnimation = new JsonContent.Animations.JsonAnimation();
            JsonUtilities.WriteJsonFile(jsonAnimation, DataPaths.AnimationTemplatesPath, "Animation.json");

            Content.JsonContent.Animations.JsonAnimationManager jsonAnimationManager = new JsonContent.Animations.JsonAnimationManager();
            JsonUtilities.WriteJsonFile(jsonAnimationManager, DataPaths.AnimationTemplatesPath, "AnimationManager.json");

            //Crafting components, recipes. books, and menu tabs.
            Crafting.JsonContent.JsonCraftingComponent jsonCraftingComponent = new Crafting.JsonContent.JsonCraftingComponent();
            JsonUtilities.WriteJsonFile(jsonCraftingComponent, CraftingDataPaths.CraftingStationTemplatesPath, "CraftingComponentTemplate.json");

            Crafting.JsonContent.UnlockableJsonCraftingRecipe jsonUnlockableCraftingRecipe = new Crafting.JsonContent.UnlockableJsonCraftingRecipe();
            JsonUtilities.WriteJsonFile(jsonUnlockableCraftingRecipe, CraftingDataPaths.CraftingStationTemplatesPath, "UnlockableCraftingRecipeTemplate.json");

            Crafting.JsonContent.JsonCraftingMenuTab jsonCraftingTab = new Crafting.JsonContent.JsonCraftingMenuTab();
            JsonUtilities.WriteJsonFile(jsonCraftingTab, CraftingDataPaths.CraftingStationTemplatesPath, "CraftingTabTemplate.json");

            Crafting.JsonContent.JsonCraftingRecipeBookDefinition jsonRecipeBookDefinition = new Crafting.JsonContent.JsonCraftingRecipeBookDefinition();
            JsonUtilities.WriteJsonFile(jsonRecipeBookDefinition, CraftingDataPaths.CraftingStationTemplatesPath, "RecipeBookDefinition.json");


            JsonBasicItemInformation jsonItemInformationFile = new JsonBasicItemInformation();
            JsonUtilities.WriteJsonFile(jsonItemInformationFile, ObjectsDataPaths.ObjectsDataTemplatesPath,"ItemInformationTemplate.json");

            JsonCraftingBlueprint jsonCraftingBlueprint = new JsonCraftingBlueprint();
            JsonUtilities.WriteJsonFile(jsonCraftingBlueprint, ObjectsDataPaths.ObjectsDataTemplatesPath, "JsonCraftingBlueprintItemTemplate.json");
        }

        protected virtual void dumpAllObjectIdsToJsonFile()
        {
            List<string> objectIds = new List<string>();
            foreach (string objectId in this.objectManager.itemsById.Keys)
            {
                objectIds.Add(objectId);
            }
            objectIds.Sort();
            JsonUtilities.WriteJsonFile(objectIds, ObjectsDataPaths.ObjectsDataDumpPath, "RegisteredObjectIds.json");


            List<string> sdvObjectIds = new List<string>();
            foreach (Enums.SDVObject sdvId in Enum.GetValues(typeof(Enums.SDVObject)))
            {
                sdvObjectIds.Add(Enum.GetName(typeof(Enums.SDVObject), sdvId));
            }
            sdvObjectIds.Sort();
            JsonUtilities.WriteJsonFile(sdvObjectIds, ObjectsDataPaths.ObjectsDataDumpPath, "StardewValleyObjectIds.json");

            List<string> sdvBigCraftableIds = new List<string>();
            foreach (Enums.SDVBigCraftable sdvId in Enum.GetValues(typeof(Enums.SDVBigCraftable)))
            {
                sdvBigCraftableIds.Add(Enum.GetName(typeof(Enums.SDVBigCraftable), sdvId));
            }
            sdvBigCraftableIds.Sort();
            JsonUtilities.WriteJsonFile(sdvBigCraftableIds, ObjectsDataPaths.ObjectsDataDumpPath, "StardewValleyBigCraftableIds.json");

            List<string> categoryIds = new List<string>();
            foreach(string id in ItemCategories.CategoriesById.Keys)
            {
                categoryIds.Add(id);
            }
            categoryIds.Sort();
            JsonUtilities.WriteJsonFile(categoryIds, ObjectsDataPaths.ObjectsDataDumpPath, "ItemCategories.json");
        }




    }
}
