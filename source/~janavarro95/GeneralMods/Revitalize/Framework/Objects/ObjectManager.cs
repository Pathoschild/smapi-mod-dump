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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Revitalize.Framework.Factories.Objects;
using Revitalize.Framework.Illuminate;
using Revitalize.Framework.Objects.CraftingTables;
using Revitalize.Framework.Objects.Extras;
using Revitalize.Framework.Objects.Furniture;
using Revitalize.Framework.Objects.Interfaces;
using Revitalize.Framework.Objects.Items.Tools;
using Revitalize.Framework.Objects.Machines;
using Revitalize.Framework.Objects.Machines.EnergyGeneration;
using Revitalize.Framework.Utilities;
using StardewModdingAPI;
using StardewValley;
using StardustCore.Animations;
using StardustCore.UIUtilities;

namespace Revitalize.Framework.Objects
{
    /// <summary>
    /// Deals with handling all objects for the mod.
    /// </summary>
    public class ObjectManager
    {
        /// <summary>
        /// All of the object managers id'd by a mod's or content pack's unique id.
        /// </summary>
        public static Dictionary<string, ObjectManager> ObjectPools;


        /// <summary>
        /// The name of this object manager.
        /// </summary>
        public string name;

        /// <summary>
        /// All of the chairs held by this object pool.
        /// </summary>
        public Dictionary<string, ChairMultiTiledObject> chairs;
        /// <summary>
        /// All of the tables held by this object pool.
        /// </summary>
        public Dictionary<string, TableMultiTiledObject> tables;
        /// <summary>
        /// All of the lamps held by this object pool.
        /// </summary>
        public Dictionary<string, LampMultiTiledObject> lamps;
        /// <summary>
        /// All of the rugs held by this object pool.
        /// </summary>
        public Dictionary<string, RugMultiTiledObject> rugs;
        public Dictionary<string, StorageFurnitureOBJ> furnitureStorage;

        public Dictionary<string, MultiTiledObject> generic;
        /// <summary>
        /// Misc. items for this mod.
        /// </summary>
        public Dictionary<string, MultiTiledObject> miscellaneous;

        public ResourceManager resources;

        public Dictionary<string, CustomObject> ItemsByName;

        public Dictionary<string, StardewValley.Tool> Tools;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectManager()
        {
            this.initialize();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="manifest"></param>
        public ObjectManager(IManifest manifest)
        {
            this.name = manifest.UniqueID;
            this.initialize();
        }

        /// <summary>
        /// Initialize all objects used to manage this class.
        /// </summary>
        private void initialize()
        {
            this.chairs = new Dictionary<string, ChairMultiTiledObject>();
            this.tables = new Dictionary<string, TableMultiTiledObject>();
            this.lamps = new Dictionary<string, LampMultiTiledObject>();
            this.rugs = new Dictionary<string, RugMultiTiledObject>();
            this.furnitureStorage = new Dictionary<string, StorageFurnitureOBJ>();

            this.generic = new Dictionary<string, MultiTiledObject>();
            this.miscellaneous = new Dictionary<string, MultiTiledObject>();

            this.resources = new ResourceManager();
            this.ItemsByName = new Dictionary<string, CustomObject>();

            this.Tools = new Dictionary<string, Tool>();

            //Load in furniture again!
        }

        /// <summary>
        /// Loads in the items for the object and resource managers.
        /// </summary>
        public void loadInItems()
        {
            this.resources.loadInItems(); //Must be first.
            this.loadInCraftingTables();
            this.loadInMachines();
            this.loadInTools();
            this.loadInAestheticsObjects();
            FurnitureFactory.LoadFurnitureFiles();
        }

        private void loadInAestheticsObjects()
        {
            LampMultiTiledObject lighthouse = new LampMultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("LightHouse", "Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(), Color.White, false, null, null));
            LampTileComponent lighthouse_0_0 = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(0, 0, 16, 16)), Color.White, true, null, new Illuminate.LightManager(),null,true));
            LampTileComponent lighthouse_1_0 = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(16, 0, 16, 16)), Color.White, true, null, new Illuminate.LightManager(), null, true));
            LampTileComponent lighthouse_0_1 = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(0, 16, 16, 16)), Color.White, true, null, new Illuminate.LightManager(), null, true));
            LampTileComponent lighthouse_1_1 = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(16, 16, 16, 16)), Color.White, true, null, new Illuminate.LightManager(), null, true));
            LampTileComponent lighthouse_0_2 = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(0, 32, 16, 16)), Color.White, true, null, new Illuminate.LightManager(), null, false));
            LampTileComponent lighthouse_1_2 = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(16, 32, 16, 16)), Color.White, true, null, new Illuminate.LightManager(), null, false));
            LampTileComponent lighthouse_0_3 = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(0, 48, 16, 16)), Color.White, false, null, new Illuminate.LightManager()));
            LampTileComponent lighthouse_1_3 = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Omegasis.Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(16, 48, 16, 16)), Color.White, false, null, new Illuminate.LightManager()));
            lighthouse_0_0.lightManager.addLight(new Vector2(16, 16), LightManager.CreateLightSource(10f, Color.White), lighthouse_0_0);
            lighthouse.addComponent(new Vector2(0,-3),lighthouse_0_0);
            lighthouse.addComponent(new Vector2(1, -3), lighthouse_1_0);
            lighthouse.addComponent(new Vector2(0, -2), lighthouse_0_1);
            lighthouse.addComponent(new Vector2(1, -2), lighthouse_1_1);
            lighthouse.addComponent(new Vector2(0, -1), lighthouse_0_2);
            lighthouse.addComponent(new Vector2(1, -1), lighthouse_1_2);
            lighthouse.addComponent(new Vector2(0, 0), lighthouse_0_3);
            lighthouse.addComponent(new Vector2(1, 0), lighthouse_1_3);

            this.AddItem("Lighthouse", lighthouse);
        }

        private void loadInCraftingTables()
        {
            MultiTiledObject WorkbenchObj = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Crafting.Workbench", TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Workbench", "Omegasis.Revitalize.Objects.Crafting.Workbench", "A workbench that can be used for crafting different objects.", "Crafting", Color.Brown, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new AnimationManager(), Color.White, false, null, null));
            CraftingTableTile workbenchTile_0_0 = new CraftingTableTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Crafting.Workbench", TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), typeof(CraftingTableTile), Color.White, true), new BasicItemInformation("Workbench", "Omegasis.Revitalize.Objects.Crafting.Workbench", "A workbench that can be used for crafting different objects.", "Crafting", Color.Brown, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new Animation(0, 0, 16, 16)), Color.White, false, null, null), "Workbench");
            CraftingTableTile workbenchTile_1_0 = new CraftingTableTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Crafting.Workbench", TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), typeof(CraftingTableTile), Color.White, true), new BasicItemInformation("Workbench", "Omegasis.Revitalize.Objects.Crafting.Workbench", "A workbench that can be used for crafting different objects.", "Crafting", Color.Brown, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new Animation(16, 0, 16, 16)), Color.White, false, null, null), "Workbench");
            CraftingTableTile workbenchTile_0_1 = new CraftingTableTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Crafting.Workbench", TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), typeof(CraftingTableTile), Color.White, true), new BasicItemInformation("Workbench", "Omegasis.Revitalize.Objects.Crafting.Workbench", "A workbench that can be used for crafting different objects.", "Crafting", Color.Brown, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new Animation(0, 16, 16, 16)), Color.White, false, null, null), "Workbench");
            CraftingTableTile workbenchTile_1_1 = new CraftingTableTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Crafting.Workbench", TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), typeof(CraftingTableTile), Color.White, true), new BasicItemInformation("Workbench", "Omegasis.Revitalize.Objects.Crafting.Workbench", "A workbench that can be used for crafting different objects.", "Crafting", Color.Brown, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new Animation(16, 16, 16, 16)), Color.White, false, null, null), "Workbench");
            WorkbenchObj.addComponent(new Vector2(0, 0), workbenchTile_0_0);
            WorkbenchObj.addComponent(new Vector2(1, 0), workbenchTile_1_0);
            WorkbenchObj.addComponent(new Vector2(0, 1), workbenchTile_0_1);
            WorkbenchObj.addComponent(new Vector2(1, 1), workbenchTile_1_1);

            MultiTiledObject AnvilObj = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Crafting.Anvil", TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Anvil", "Omegasis.Revitalize.Objects.Crafting.Anvil", "An anvil that can be used for crafting different machines and other metalic objects.", "Crafting", Color.Brown, -300, 0, false, 2000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new AnimationManager(), Color.White, false, null, null));
            CraftingTableTile anvilTile_0_0 = new CraftingTableTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Crafting.Anvil", TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), typeof(CraftingTableTile), Color.White, true), new BasicItemInformation("Anvil", "Omegasis.Revitalize.Objects.Crafting.Anvil", "An anvil that can be used for crafting different machines and other metalic objects.", "Crafting", Color.Brown, -300, 0, false, 2000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new Animation(0, 0, 16, 16)), Color.White, false, null, null), "Anvil");
            CraftingTableTile anvilTile_1_0 = new CraftingTableTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Crafting.Anvil", TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), typeof(CraftingTableTile), Color.White, true), new BasicItemInformation("Anvil", "Omegasis.Revitalize.Objects.Crafting.Anvil", "An anvil that can be used for crafting different machines and other metalic objects.", "Crafting", Color.Brown, -300, 0, false, 2000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new Animation(16, 0, 16, 16)), Color.White, false, null, null), "Anvil");
            CraftingTableTile anvilTile_0_1 = new CraftingTableTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Crafting.Anvil", TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), typeof(CraftingTableTile), Color.White, true), new BasicItemInformation("Anvil", "Omegasis.Revitalize.Objects.Crafting.Anvil", "An anvil that can be used for crafting different machines and other metalic objects.", "Crafting", Color.Brown, -300, 0, false, 2000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new Animation(0, 16, 16, 16)), Color.White, false, null, null), "Anvil");
            CraftingTableTile anvilTile_1_1 = new CraftingTableTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Crafting.Anvil", TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), typeof(CraftingTableTile), Color.White, true), new BasicItemInformation("Anvil", "Omegasis.Revitalize.Objects.Crafting.Anvil", "An anvil that can be used for crafting different machines and other metalic objects.", "Crafting", Color.Brown, -300, 0, false, 2000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new Animation(16, 16, 16, 16)), Color.White, false, null, null), "Anvil");
            AnvilObj.addComponent(new Vector2(0, 0), anvilTile_0_0);
            AnvilObj.addComponent(new Vector2(1, 0), anvilTile_1_0);
            AnvilObj.addComponent(new Vector2(0, 1), anvilTile_0_1);
            AnvilObj.addComponent(new Vector2(1, 1), anvilTile_1_1);

            this.AddItem("Workbench", WorkbenchObj);
            this.AddItem("Anvil", AnvilObj);
        }

        private void loadInMachines()
        {
            this.loadInConnectionComponents();

            MultiTiledObject trashCan = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Misc.TrashCan", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "TrashCan"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Trash Can", "Omegasis.Revitalize.Furniture.Misc.TrashCan", "A trash can where you can throw away unnecessary objects. It empties out at the beginning of each new day.", "Machine", Color.SteelBlue, -300, 0, false, 650, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "TrashCan"), new AnimationManager(), Color.White, true, new InventoryManager(36), null, null));
            TrashCanTile trash1 = new TrashCanTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Misc.TrashCan", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "TrashCan"), typeof(TrashCanTile), Color.White, true), new BasicItemInformation("Trash Can", "Omegasis.Revitalize.Furniture.Misc.TrashCan", "A trash can where you can throw away unnecessary objects. It empties out at the beginning of each new day.", "Machine", Color.SteelBlue, -300, 0, false, 650, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "TrashCan"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "TrashCan"), new Animation(0, 0, 16, 16)), Color.White, true, new InventoryManager(36), null, null));
            TrashCanTile trash2 = new TrashCanTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Misc.TrashCan", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "TrashCan"), typeof(TrashCanTile), Color.White, true), new BasicItemInformation("Trash Can", "Omegasis.Revitalize.Furniture.Misc.TrashCan", "A trash can where you can throw away unnecessary objects. It empties out at the beginning of each new day.", "Machine", Color.SteelBlue, -300, 0, false, 650, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "TrashCan"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "TrashCan"), new Animation(0, 16, 16, 16)), Color.White, false, new InventoryManager(36), null, null));
            trashCan.addComponent(new Vector2(0, 0), trash1);
            trashCan.addComponent(new Vector2(0, 1), trash2);

            this.AddItem("TrashCan", trashCan);

            MultiTiledObject sandBox = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Sandbox", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Sandbox"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Sandbox", "Omegasis.Revitalize.Objects.Machines.Sandbox", "A sandbox which slowly produces sand. Unfortunately you can't sit in this one.", "Machine", Color.SteelBlue, -300, 0, false, 750, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Sandbox"), new AnimationManager(), Color.White, true, new InventoryManager(36), null, null));
            Machine sandBox_0_0 = new Machine(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Sandbox", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Sandbox"), typeof(Machine), Color.White, true), new BasicItemInformation("Sandbox", "Omegasis.Revitalize.Objects.Machines.Sandbox", "A sandbox which slowly produces sand. Unfortunately you can't sit in this one.", "Machine", Color.SteelBlue, -300, 0, false, 750, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Sandbox"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Sandbox"),new Animation(0,0,16,16)), Color.White, false, new InventoryManager(36), null, null), new List<InformationFiles.ResourceInformation>()
            {
                new InformationFiles.ResourceInformation(this.resources.getResource("Sand",1),1,1,1,1,1,1,0,0,0,0)

            }, 0, TimeUtilities.GetMinutesFromTime(0, 1, 0), true,"Workbench");
            Machine sandBox_1_0 = new Machine(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Sandbox", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Sandbox"), typeof(Machine), Color.White, true), new BasicItemInformation("Sandbox", "Omegasis.Revitalize.Objects.Machines.Sandbox", "A sandbox which slowly produces sand. Unfortunately you can't sit in this one.", "Machine", Color.SteelBlue, -300, 0, false, 750, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Sandbox"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Sandbox"), new Animation(16, 0, 16, 16)), Color.White, false, new InventoryManager(36), null, null), new List<InformationFiles.ResourceInformation>()
            {
                //new InformationFiles.ResourceInformation(this.resources.getResource("Sand",1),1,1,1,1,1,1,0,0,0,0)

            }, 0, TimeUtilities.GetMinutesFromTime(0, 1, 0), false, "Workbench");
            Machine sandBox_0_1 = new Machine(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Sandbox", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Sandbox"), typeof(Machine), Color.White, true), new BasicItemInformation("Sandbox", "Omegasis.Revitalize.Objects.Machines.Sandbox", "A sandbox which slowly produces sand. Unfortunately you can't sit in this one.", "Machine", Color.SteelBlue, -300, 0, false, 750, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Sandbox"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Sandbox"), new Animation(0, 16, 16, 16)), Color.White, false, new InventoryManager(36), null, null), new List<InformationFiles.ResourceInformation>()
            {
                //new InformationFiles.ResourceInformation(this.resources.getResource("Sand",1),1,1,1,1,1,1,0,0,0,0)

            }, 0, TimeUtilities.GetMinutesFromTime(0,1,0), false, "Workbench");
            Machine sandBox_1_1 = new Machine(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Sandbox", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Sandbox"), typeof(Machine), Color.White, true), new BasicItemInformation("Sandbox", "Omegasis.Revitalize.Objects.Machines.Sandbox", "A sandbox which slowly produces sand. Unfortunately you can't sit in this one.", "Machine", Color.SteelBlue, -300, 0, false, 750, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Sandbox"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Sandbox"), new Animation(16, 16, 16, 16)), Color.White, false, new InventoryManager(36), null, null), new List<InformationFiles.ResourceInformation>()
            {
                //new InformationFiles.ResourceInformation(this.resources.getResource("Sand",1),1,1,1,1,1,1,0,0,0,0)

            }, 0, TimeUtilities.GetMinutesFromTime(0, 1, 0), false, "Workbench");

            sandBox.addComponent(new Vector2(0,0),sandBox_0_0);
            sandBox.addComponent(new Vector2(1, 0), sandBox_1_0);
            sandBox.addComponent(new Vector2(0, 1), sandBox_0_1);
            sandBox.addComponent(new Vector2(1, 1), sandBox_1_1);

            this.AddItem("SandBox", sandBox);


            MultiTiledObject solarpanel1Container = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SolarPanelV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarPanelTier1"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Solar Panel", "Omegasis.Revitalize.Objects.Machines.SolarPanel", "Generates energy while the sun is up.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarPanelTier1"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SolarPanelTier1"), new Animation(0, 0, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(100, Enums.EnergyInteractionType.Produces)));
            SolarPanel solarP1 = new SolarPanel(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SolarPanelTier1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarPanelTier1"), typeof(SolarPanel), Color.White, true), new BasicItemInformation("Solar Panel", "Omegasis.Revitalize.Objects.Machines.SolarPanelV1", "Generates energy while the sun is up.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarPanelTier1"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SolarPanelTier1"), new Animation(0, 0, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(100, Enums.EnergyInteractionType.Produces)),2,0,true);
            solarpanel1Container.addComponent(new Vector2(0, 0), solarP1);
            this.AddItem("SolarPanelTier1", solarpanel1Container);


            MultiTiledObject solarArray1Container = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SolarArrayV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Solar Array", "Omegasis.Revitalize.Objects.Machines.SolarArrayV1", "A collection of solar panels that generates even more energy while the sun is up.", "Machine", Color.SteelBlue, -300, 0, false, 4000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new Animation(0, 0, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(100, Enums.EnergyInteractionType.Produces)));
            SolarPanel solarA1V1 = new SolarPanel(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SolarArrayTier1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), typeof(SolarPanel), Color.White, true), new BasicItemInformation("Solar Array", "Omegasis.Revitalize.Objects.Machines.SolarArrayV1", "A collection of solar panels that generates even more energy while the sun is up.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new Animation(0, 0, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(100, Enums.EnergyInteractionType.Produces)), 8, 0, true);
            SolarPanel solarA2V1 = new SolarPanel(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SolarArrayTier1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), typeof(SolarPanel), Color.White, true), new BasicItemInformation("Solar Array", "Omegasis.Revitalize.Objects.Machines.SolarArrayV1", "A collection of solar panels that generates even more energy while the sun is up.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new Animation(16, 0, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(100, Enums.EnergyInteractionType.Produces)), 8, 0, false);
            SolarPanel solarA3V1 = new SolarPanel(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SolarArrayTier1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), typeof(SolarPanel), Color.White, true), new BasicItemInformation("Solar Array", "Omegasis.Revitalize.Objects.Machines.SolarArrayV1", "A collection of solar panels that generates even more energy while the sun is up.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new Animation(0, 16, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(100, Enums.EnergyInteractionType.Produces)), 8, 0, false);
            SolarPanel solarA4V1 = new SolarPanel(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SolarArrayTier1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), typeof(SolarPanel), Color.White, true), new BasicItemInformation("Solar Array", "Omegasis.Revitalize.Objects.Machines.SolarArrayV1", "A collection of solar panels that generates even more energy while the sun is up.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new Animation(16, 16, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(100, Enums.EnergyInteractionType.Produces)), 8, 0, false);

            solarArray1Container.addComponent(new Vector2(0, 0), solarA1V1);
            solarArray1Container.addComponent(new Vector2(1, 0), solarA2V1);
            solarArray1Container.addComponent(new Vector2(0, 1), solarA3V1);
            solarArray1Container.addComponent(new Vector2(1, 1), solarA4V1);
            this.AddItem("SolarArrayTier1", solarArray1Container);


            ///Consumes energy. Produces batteries.
            MultiTiledObject batteryBin = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.BatteryBin", TextureManager.GetTexture(ModCore.Manifest, "Machines", "BatteryBin"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Battery Bin", "Omegasis.Revitalize.Objects.Machines.BatteryBin", "Consumes energy over time to produce battery packs.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "BatteryBin"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "BatteryBin"), new Animation(0, 0, 16, 16)), Color.White, false, new InventoryManager(9,3,3), null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Consumes)));
            Machine batteryBin_0_0 = new Machine(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.BatteryBin", TextureManager.GetTexture(ModCore.Manifest, "Machines", "BatteryBin"), typeof(Machine), Color.White, true), new BasicItemInformation("Battery Bin", "Omegasis.Revitalize.Objects.Machines.BatteryBin", "Consumes energy over time to produce battery packs.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "BatteryBin"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "BatteryBin"), new Animation(0, 0, 16, 16)), Color.White, false, new InventoryManager(9,3,3), null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Consumes)), new List<InformationFiles.ResourceInformation>()
            {
                new InformationFiles.ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.BatteryPack,1),1,1,1,1,1,1,0,0,0,0)

            }, 1, TimeUtilities.GetMinutesFromTime(0, 1, 0), true, "");
            batteryBin.addComponent(new Vector2(0, 0), batteryBin_0_0);
            this.AddItem("BatteryBin", batteryBin);

            MultiTiledObject capacitor= new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Capacitor", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Capacitor"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Capacitor", "Omegasis.Revitalize.Objects.Machines.Capacitor", "A box which stores energy for use over time.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Capacitor"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Capacitor"), new Animation(0, 0, 16, 16)), Color.White, false,null, null, new Energy.EnergyManager(2000, Enums.EnergyInteractionType.Storage)));
            Machine capacitor_0_0 = new Machine(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Capacitor", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Capacitor"), typeof(Machine), Color.White, true), new BasicItemInformation("Capacitor", "Omegasis.Revitalize.Objects.Machines.Capacitor", "A box which stores energy for use over time.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Capacitor"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Capacitor"), new Animation(0, 0, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(2000, Enums.EnergyInteractionType.Storage)),null,0,0,true,"");
            capacitor.addComponent(new Vector2(0, 0), capacitor_0_0);
            this.AddItem("Capacitor", capacitor);


            MultiTiledObject chargingStation = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.ChargingStation", TextureManager.GetTexture(ModCore.Manifest, "Machines", "ChargingStation"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Charging Station", "Omegasis.Revitalize.Objects.Machines.ChargingStation", "A place to charge your tools and other electrical components.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "ChargingStation"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "ChargingStation"), new Animation(0, 0, 16, 16)), Color.White, false,new InventoryManager(4,4,1), null, new Energy.EnergyManager(2000, Enums.EnergyInteractionType.Storage)));
            ChargingStation chargingStation_0_0 = new ChargingStation(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.ChargingStation", TextureManager.GetTexture(ModCore.Manifest, "Machines", "ChargingStation"), typeof(ChargingStation), Color.White, true), new BasicItemInformation("Charging Station", "Omegasis.Revitalize.Objects.Machines.ChargingStation", "A place to charge your tools and other electrical components.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "ChargingStation"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "ChargingStation"), new Animation(0, 0, 16, 16)), Color.White, true, null, null, new Energy.EnergyManager(2000, Enums.EnergyInteractionType.Storage)), null, 0, 0, true, "");
            ChargingStation chargingStation_1_0 = new ChargingStation(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.ChargingStation", TextureManager.GetTexture(ModCore.Manifest, "Machines", "ChargingStation"), typeof(ChargingStation), Color.White, true), new BasicItemInformation("Charging Station", "Omegasis.Revitalize.Objects.Machines.ChargingStation", "A place to charge your tools and other electrical components.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "ChargingStation"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "ChargingStation"), new Animation(16, 0, 16, 16)), Color.White, true, null, null, new Energy.EnergyManager(2000, Enums.EnergyInteractionType.Storage)), null, 0, 0, false, "");
            ChargingStation chargingStation_0_1 = new ChargingStation(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.ChargingStation", TextureManager.GetTexture(ModCore.Manifest, "Machines", "ChargingStation"), typeof(ChargingStation), Color.White, true), new BasicItemInformation("Charging Station", "Omegasis.Revitalize.Objects.Machines.ChargingStation", "A place to charge your tools and other electrical components.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "ChargingStation"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "ChargingStation"), new Animation(0, 16, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(2000, Enums.EnergyInteractionType.Storage)), null, 0, 0, false, "");
            ChargingStation chargingStation_1_1 = new ChargingStation(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.ChargingStation", TextureManager.GetTexture(ModCore.Manifest, "Machines", "ChargingStation"), typeof(ChargingStation), Color.White, true), new BasicItemInformation("Charging Station", "Omegasis.Revitalize.Objects.Machines.ChargingStation", "A place to charge your tools and other electrical components.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "ChargingStation"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "ChargingStation"), new Animation(16, 16, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(2000, Enums.EnergyInteractionType.Storage)), null, 0, 0, false, "");
            chargingStation.addComponent(new Vector2(0, 0), chargingStation_0_0);
            chargingStation.addComponent(new Vector2(1, 0), chargingStation_1_0);
            chargingStation.addComponent(new Vector2(0, 1), chargingStation_0_1);
            chargingStation.addComponent(new Vector2(1, 1), chargingStation_1_1);
            this.AddItem("ChargingStation", chargingStation);


            MultiTiledObject grinder= new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Grinder", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Grinder"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Grinder", "Omegasis.Revitalize.Objects.Machines.Grinder", "Grinds up ores and rocks.", "Machine", Color.SteelBlue, -300, 0, false, 4000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Grinder"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Grinder"), new Animation(0, 0, 16, 16)), Color.White, false, new InventoryManager(18, 3, 6), null, new Energy.EnergyManager(1000, Enums.EnergyInteractionType.Consumes)));
            Grinder grinder_0_0 = new Grinder(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Grinder", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Grinder"), typeof(Grinder), Color.White, true), new BasicItemInformation("Grinder", "Omegasis.Revitalize.Objects.Machines.Grinder", "Grinds up ores and rocks.", "Machine", Color.SteelBlue, -300, 0, false, 4000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Grinder"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Grinder"), new Animation(0, 0, 16, 16)), Color.White, false, new InventoryManager(18, 3, 6), null, new Energy.EnergyManager(1000, Enums.EnergyInteractionType.Consumes)),null,ModCore.Configs.machinesConfig.grinderEnergyConsumption,ModCore.Configs.machinesConfig.grinderTimeToGrind,true,"");
            Grinder grinder_1_0 = new Grinder(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Grinder", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Grinder"), typeof(Grinder), Color.White, true), new BasicItemInformation("Grinder", "Omegasis.Revitalize.Objects.Machines.Grinder", "Grinds up ores and rocks.", "Machine", Color.SteelBlue, -300, 0, false, 4000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Grinder"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Grinder"), new Animation(16, 0, 16, 16)), Color.White, false, new InventoryManager(18, 3, 6), null, new Energy.EnergyManager(1000, Enums.EnergyInteractionType.Consumes)), null, ModCore.Configs.machinesConfig.grinderEnergyConsumption, ModCore.Configs.machinesConfig.grinderTimeToGrind, false, "");
            Grinder grinder_0_1 = new Grinder(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Grinder", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Grinder"), typeof(Grinder), Color.White, true), new BasicItemInformation("Grinder", "Omegasis.Revitalize.Objects.Machines.Grinder", "Grinds up ores and rocks.", "Machine", Color.SteelBlue, -300, 0, false, 4000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Grinder"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Grinder"), new Animation(0, 16, 16, 16)), Color.White, false, new InventoryManager(18, 3, 6), null, new Energy.EnergyManager(1000, Enums.EnergyInteractionType.Consumes)), null, ModCore.Configs.machinesConfig.grinderEnergyConsumption, ModCore.Configs.machinesConfig.grinderTimeToGrind, false, "");
            Grinder grinder_1_1 = new Grinder(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Grinder", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Grinder"), typeof(Grinder), Color.White, true), new BasicItemInformation("Grinder", "Omegasis.Revitalize.Objects.Machines.Grinder", "Grinds up ores and rocks.", "Machine", Color.SteelBlue, -300, 0, false, 4000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Grinder"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Grinder"), new Animation(16, 16, 16, 16)), Color.White, false, new InventoryManager(18, 3, 6), null, new Energy.EnergyManager(1000, Enums.EnergyInteractionType.Consumes)), null, ModCore.Configs.machinesConfig.grinderEnergyConsumption, ModCore.Configs.machinesConfig.grinderTimeToGrind, false, "");
            grinder.addComponent(new Vector2(0, 0), grinder_0_0);
            grinder.addComponent(new Vector2(1, 0), grinder_1_0);
            grinder.addComponent(new Vector2(0, 1), grinder_0_1);
            grinder.addComponent(new Vector2(1, 1), grinder_1_1);
            this.AddItem("Grinder", grinder);

            MultiTiledObject miningDrillMachine = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.MiningDrillV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Mining Drill", "Omegasis.Revitalize.Objects.Machines.MiningDrill", "Digs up rocks and ores. Requires energy to run.", "Machine", Color.SteelBlue, -300, 0, false, 4000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), new Animation(0, 0, 16, 16)), Color.White, false, new InventoryManager(18, 3, 6), null, new Energy.EnergyManager(1000, Enums.EnergyInteractionType.Consumes)));
            Machine miningDrillMachine_0_0 = new Machine(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.MiningDrillV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), typeof(Machine), Color.White, true), new BasicItemInformation("Mining Drill", "Omegasis.Revitalize.Objects.Machines.MiningDrill", "Digs up rocks and ores. Requires energy to run.", "Machine", Color.SteelBlue, -300, 0, false, 4000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), new Animation(0, 0, 16, 16), new Dictionary<string, List<Animation>>() {
                {"Default",new List<Animation>(){new Animation(0,0,16,16) } },
                { "Mining",new List<Animation>(){
                    new Animation(0,0,16,16,30),
                    new Animation(16,0,16,16,30),
                    new Animation(32,0,16,16,30),
                    new Animation(48,0,16,16,30),
                } }
            }, "Mining"), Color.White, false, new InventoryManager(18, 3, 6), null, new Energy.EnergyManager(1000, Enums.EnergyInteractionType.Consumes)), ModCore.ObjectManager.resources.miningDrillResources.Values.ToList(), ModCore.Configs.machinesConfig.miningDrillEnergyConsumption, ModCore.Configs.machinesConfig.miningDrillTimeToMine, true, "");

            Machine miningDrillMachine_0_1 = new Machine(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.MiningDrillV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), typeof(Machine), Color.White, true), new BasicItemInformation("Mining Drill", "Omegasis.Revitalize.Objects.Machines.MiningDrill", "Digs up rocks and ores. Requires energy to run.", "Machine", Color.SteelBlue, -300, 0, false, 4000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), new Animation(0, 16, 16, 16),new Dictionary<string, List<Animation>>() {
                {"Default",new List<Animation>(){new Animation(0,16,16,16) } },
                { "Mining",new List<Animation>(){
                    new Animation(0,16,16,16,30),
                    new Animation(16,16,16,16,30),
                    new Animation(32,16,16,16,30),
                    new Animation(48,16,16,16,30),
                } }
            }, "Mining"), Color.White, false, new InventoryManager(18, 3, 6), null, new Energy.EnergyManager(1000, Enums.EnergyInteractionType.Consumes)), ModCore.ObjectManager.resources.miningDrillResources.Values.ToList(), ModCore.Configs.machinesConfig.miningDrillEnergyConsumption, ModCore.Configs.machinesConfig.miningDrillTimeToMine, true, "");
            miningDrillMachine.addComponent(new Vector2(0, 0), miningDrillMachine_0_0);
            miningDrillMachine.addComponent(new Vector2(0, 1), miningDrillMachine_0_1);
            miningDrillMachine_0_0.animationManager.setAnimation("Mining");
            miningDrillMachine_0_0.animationManager.playAnimation("Mining");
            miningDrillMachine_0_1.animationManager.setAnimation("Mining");
            miningDrillMachine_0_1.animationManager.playAnimation("Mining");
            this.AddItem("MiningDrillMachineV1",miningDrillMachine);

            MultiTiledObject alloyFurnace = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.AlloyFurnace", TextureManager.GetTexture(ModCore.Manifest, "Machines", "AlloyFurnace"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Alloy Furnace", "Omegasis.Revitalize.Objects.Machines.AlloyFurnace", "Smelts bars into ingots. Works twice as fast as a traditional furnace.", "Machine", Color.SteelBlue, -300, 0, false, 250, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "AlloyFurnace"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "AlloyFurnace"), new Animation(0, 0, 16, 16)), Color.White, false, new InventoryManager(6, 3, 6), null,null));
            AlloyFurnace alloyFurnace_0_0 = new AlloyFurnace(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.AlloyFurnace", TextureManager.GetTexture(ModCore.Manifest, "Machines", "AlloyFurnace"), typeof(AlloyFurnace), Color.White, true), new BasicItemInformation("Alloy Furnace", "Omegasis.Revitalize.Objects.Machines.AlloyFurnace", "Smelts bars into ingots. Works twice as fast as a traditional furnace.", "Machine", Color.SteelBlue, -300, 0, false, 250, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "AlloyFurnace"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "AlloyFurnace"), new Animation(0, 0, 16, 16), new Dictionary<string, List<Animation>>()
            {
                {"Default",new List<Animation>()
                    {
                        new Animation(0,0,16,16)
                    }
                },
                {"Working",new List<Animation>()
                    {
                    new Animation(0,32,16,16,30),
                    new Animation(16,32,16,16,30)
                    }
                }

            },"Default"), Color.White, true, new InventoryManager(6, 3, 6), null, null), null, 0, 0, true, "AlloyFurnace");
            AlloyFurnace alloyFurnace_0_1 = new AlloyFurnace(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.AlloyFurnace", TextureManager.GetTexture(ModCore.Manifest, "Machines", "AlloyFurnace"), typeof(Machine), Color.White, true), new BasicItemInformation("Alloy Furnace", "Omegasis.Revitalize.Objects.Machines.AlloyFurnace", "Smelts bars into ingots. Works twice as fast as a traditional furnace.", "Machine", Color.SteelBlue, -300, 0, false, 250, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "AlloyFurnace"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "AlloyFurnace"), new Animation(0, 16, 16, 16), new Dictionary<string, List<Animation>>()
            {
                {"Default",new List<Animation>()
                    {
                        new Animation(0,16,16,16)
                    }
                },
                {"Working",new List<Animation>()
                    {
                    new Animation(0,48,16,16,30),
                    new Animation(16,48,16,16,30)
                    }
                }

            }, "Default"), Color.White, false, new InventoryManager(6, 3, 6), null, null), null, 0, 0, false, "AlloyFurnace");
            alloyFurnace.addComponent(new Vector2(0, 0), alloyFurnace_0_0);
            alloyFurnace.addComponent(new Vector2(0, 1), alloyFurnace_0_1);
            this.AddItem("AlloyFurnace", alloyFurnace);


            MultiTiledObject waterPumpV1 = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.WaterPump", TextureManager.GetTexture(ModCore.Manifest, "Machines", "WaterPump"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Water Pump", "Omegasis.Revitalize.Objects.Machines.WaterPump", "Pumps up water from a water source.", "Machine", Color.SteelBlue, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "WaterPump"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "WaterPump"), new Animation(0, 0, 16, 16)), Color.White, false, null, null, null, false, null, null, new Managers.FluidManagerV2(5000, true, Enums.FluidInteractionType.Machine, false)));
            WaterPump waterPumpV1_0_0 = new WaterPump(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.WaterPump", TextureManager.GetTexture(ModCore.Manifest, "Machines", "WaterPump"), typeof(WaterPump), Color.White, true), new BasicItemInformation("Water Pump", "Omegasis.Revitalize.Objects.Machines.WaterPump", "Pumps up water from a water source.", "Machine", Color.SteelBlue, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "WaterPump"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "WaterPump"), new Animation(0, 0, 16, 16)), Color.White, true, null, null, null, false, null, null, new Managers.FluidManagerV2(5000, true, Enums.FluidInteractionType.Machine, false)), null, 0, 0, false, "");
            WaterPump waterPumpV1_0_1= new WaterPump(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.WaterPump", TextureManager.GetTexture(ModCore.Manifest, "Machines", "WaterPump"), typeof(WaterPump), Color.White, true), new BasicItemInformation("Water Pump", "Omegasis.Revitalize.Objects.Machines.WaterPump", "Pumps up water from a water source.", "Machine", Color.SteelBlue, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "WaterPump"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "WaterPump"), new Animation(0, 16, 16, 16)), Color.White, false, null, null, null, false, null, null, new Managers.FluidManagerV2(5000, true, Enums.FluidInteractionType.Machine, false)), null, 0, 0, true, "");
            waterPumpV1.addComponent(new Vector2(0, 0), waterPumpV1_0_0);
            waterPumpV1.addComponent(new Vector2(0, 1), waterPumpV1_0_1);
            this.AddItem("WaterPumpV1", waterPumpV1);

            MultiTiledObject steamBoilerV1= new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SteamBoiler", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Steam Boiler", "Omegasis.Revitalize.Objects.Machines.SteamBoiler", "Burns coal and wood. Consumes water to produce steam which can be used in a steam generator.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new Animation(0, 0, 16, 16)), Color.White, false, new InventoryManager(9,3,3), null, null, false, null, null, new Managers.FluidManagerV2(4000, false, Enums.FluidInteractionType.Machine, false,false,1)));
            SteamBoiler steamBoilerV1_0_0 = new SteamBoiler(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SteamBoiler", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), typeof(SteamBoiler), Color.White, true), new BasicItemInformation("Steam Boiler", "Omegasis.Revitalize.Objects.Machines.SteamBoiler", "Burns coal and wood. Consumes water to produce steam which can be used in a steam generator.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new Animation(0, 0, 16, 16),new Dictionary<string, List<Animation>>()
            {
                {"Default",new List<Animation>(){
                    new Animation(0,0,16,16)
                } },
                {"Working",new List<Animation>(){
                    new Animation(32,0,16,16)
                } },

            },"Default"), Color.White, true, new InventoryManager(9, 3, 3), null, null, false, null, null, new Managers.FluidManagerV2(4000, false, Enums.FluidInteractionType.Machine, false, false, 1)), null, 0, 0, true, "");

            SteamBoiler steamBoilerV1_1_0 = new SteamBoiler(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SteamBoiler", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), typeof(SteamBoiler), Color.White, true), new BasicItemInformation("Steam Boiler", "Omegasis.Revitalize.Objects.Machines.SteamBoiler", "Burns coal and wood. Consumes water to produce steam which can be used in a steam generator.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new Animation(16, 0, 16, 16), new Dictionary<string, List<Animation>>()
            {
                {"Default",new List<Animation>(){
                    new Animation(16,0,16,16)
                } },
                {"Working",new List<Animation>(){
                    new Animation(48,0,16,16)
                } },

            }, "Default"), Color.White, true, new InventoryManager(9, 3, 3), null, null, false, null, null, new Managers.FluidManagerV2(4000, false, Enums.FluidInteractionType.Machine, false, false, 1)), null, 0, 0, false, "");

            SteamBoiler steamBoilerV1_0_1 = new SteamBoiler(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SteamBoiler", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), typeof(SteamBoiler), Color.White, true), new BasicItemInformation("Steam Boiler", "Omegasis.Revitalize.Objects.Machines.SteamBoiler", "Burns coal and wood. Consumes water to produce steam which can be used in a steam generator.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new Animation(0, 16, 16, 16), new Dictionary<string, List<Animation>>()
            {
                {"Default",new List<Animation>(){
                    new Animation(0,16,16,16)
                } },
                {"Working",new List<Animation>(){
                    new Animation(32,16,16,16)
                } },

            }, "Default"), Color.White, false, new InventoryManager(9, 3, 3), null, null, false, null, null, new Managers.FluidManagerV2(4000, false, Enums.FluidInteractionType.Machine, false, false, 1)), null, 0, 0, false, "");

            SteamBoiler steamBoilerV1_1_1 = new SteamBoiler(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SteamBoiler", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), typeof(SteamBoiler), Color.White, true), new BasicItemInformation("Steam Boiler", "Omegasis.Revitalize.Objects.Machines.SteamBoiler", "Burns coal and wood. Consumes water to produce steam which can be used in a steam generator.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new Animation(16, 16, 16, 16), new Dictionary<string, List<Animation>>()
            {
                {"Default",new List<Animation>(){
                    new Animation(16,16,16,16)
                } },
                {"Working",new List<Animation>(){
                    new Animation(48,16,16,16)
                } },

            }, "Default"), Color.White, false, new InventoryManager(9, 3, 3), null, null, false, null, null, new Managers.FluidManagerV2(4000, false, Enums.FluidInteractionType.Machine, false, false, 1)), null, 0, 0, false, "");

            SteamBoiler steamBoilerV1_0_2 = new SteamBoiler(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SteamBoiler", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), typeof(SteamBoiler), Color.White, true), new BasicItemInformation("Steam Boiler", "Omegasis.Revitalize.Objects.Machines.SteamBoiler", "Burns coal and wood. Consumes water to produce steam which can be used in a steam generator.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new Animation(0, 32, 16, 16), new Dictionary<string, List<Animation>>()
            {
                {"Default",new List<Animation>(){
                    new Animation(0,32,16,16)
                } },
                {"Working",new List<Animation>(){
                    new Animation(32,32,16,16)
                } },

            }, "Default"), Color.White, false, new InventoryManager(9, 3, 3), null, null, false, null, null, new Managers.FluidManagerV2(4000, false, Enums.FluidInteractionType.Machine, false, false, 1)), null, 0, 0, false, "");

            SteamBoiler steamBoilerV1_1_2 = new SteamBoiler(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SteamBoiler", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), typeof(SteamBoiler), Color.White, true), new BasicItemInformation("Steam Boiler", "Omegasis.Revitalize.Objects.Machines.SteamBoiler", "Burns coal and wood. Consumes water to produce steam which can be used in a steam generator.", "Machine", Color.SteelBlue, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SteamBoiler"), new Animation(16, 32, 16, 16), new Dictionary<string, List<Animation>>()
            {
                {"Default",new List<Animation>(){
                    new Animation(16,32,16,16)
                } },
                {"Working",new List<Animation>(){
                    new Animation(48,32,16,16)
                } },

            }, "Default"), Color.White, false, new InventoryManager(9, 3, 3), null, null, false, null, null, new Managers.FluidManagerV2(4000, false, Enums.FluidInteractionType.Machine, false, false, 1)), null, 0, 0, false, "");

            steamBoilerV1.addComponent(new Vector2(0, 0), steamBoilerV1_0_0);
            steamBoilerV1.addComponent(new Vector2(1, 0), steamBoilerV1_1_0);
            steamBoilerV1.addComponent(new Vector2(0, 1), steamBoilerV1_0_1);
            steamBoilerV1.addComponent(new Vector2(1, 1), steamBoilerV1_1_1);
            steamBoilerV1.addComponent(new Vector2(0, 2), steamBoilerV1_0_2);
            steamBoilerV1.addComponent(new Vector2(1, 2), steamBoilerV1_1_2);

            this.AddItem("SteamBoilerV1", steamBoilerV1);

            MultiTiledObject steamEngineV1 = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SteamEngineV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamEngine"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Steam Engine", "Omegasis.Revitalize.Objects.Machines.SteamEngine", "Consumes steam in order to produce power.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamEngine"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SteamEngine"), new Animation(0, 0, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Produces), false, null, null, new Managers.FluidManagerV2(2000, false, Enums.FluidInteractionType.Machine, false,true,1)));
            SteamEngine steamEngineV1_0_0 = new SteamEngine(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SteamEngineV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamEngine"), typeof(SteamEngine), Color.White, true), new BasicItemInformation("Steam Engine", "Omegasis.Revitalize.Objects.Machines.SteamEngine", "Consumes steam in order to produce power.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamEngine"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SteamEngine"), new Animation(0, 0, 16, 16)), Color.White, false, new InventoryManager(9, 3, 3), null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Produces), false, null, null, new Managers.FluidManagerV2(2000, false, Enums.FluidInteractionType.Machine, false, true, 1)), null, ModCore.Configs.machinesConfig.steamEngineV1_powerGeneratedPerOperation, 0, true, "", ModCore.ObjectManager.resources.getFluid("Steam"), ModCore.Configs.machinesConfig.steamEngineV1_requiredSteamPerOperation);
            SteamEngine steamEngineV1_1_0 = new SteamEngine(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.SteamEngineV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamEngine"), typeof(SteamEngine), Color.White, true), new BasicItemInformation("Steam Engine", "Omegasis.Revitalize.Objects.Machines.SteamEngine", "Consumes steam in order to produce power.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SteamEngine"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SteamEngine"), new Animation(16, 0, 16, 16)), Color.White, false, new InventoryManager(9, 3, 3), null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Produces), false, null, null, new Managers.FluidManagerV2(2000, false, Enums.FluidInteractionType.Machine, false, true, 1)), null, ModCore.Configs.machinesConfig.steamEngineV1_powerGeneratedPerOperation, 0, true, "", ModCore.ObjectManager.resources.getFluid("Steam"), ModCore.Configs.machinesConfig.steamEngineV1_requiredSteamPerOperation);
            steamEngineV1.addComponent(new Vector2(0, 0), steamEngineV1_0_0);
            steamEngineV1.addComponent(new Vector2(1, 0), steamEngineV1_1_0);
            this.AddItem("SteamEngineV1", steamEngineV1);


            MultiTiledObject windMillV1 = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.WindmillV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Windmill"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Windmill", "Omegasis.Revitalize.Objects.Machines.WindmillV1", "Generates power from the wind.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Windmill"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Windmill"), new Animation(0, 0, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Produces), false, null, null, null));
            Windmill windMillV1_0_0 = new Windmill(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.WindmillV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Windmill"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Windmill", "Omegasis.Revitalize.Objects.Machines.WindmillV1", "Generates power from the wind.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Windmill"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Windmill"), new Animation(0, 0, 16, 16),new Dictionary<string, List<Animation>>() {

                {"Default",new List<Animation>()
                {
                    new Animation(0,0,16,16)
                } },
                {"Working",new List<Animation>()
                {
                    new Animation(0,0,16,16,20),
                    new Animation(16,0,16,16,20)
                } }
            },"Working"), Color.White, false, null, null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Produces), false, null, null, null), null, ModCore.Configs.machinesConfig.windmillV1_basePowerProduction, 0, true, "", null, 0);
            Windmill windMillV1_0_1 = new Windmill(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.WindmillV1", TextureManager.GetTexture(ModCore.Manifest, "Machines", "Windmill"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Windmill", "Omegasis.Revitalize.Objects.Machines.WindmillV1", "Generates power from the wind.", "Machine", Color.SteelBlue, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Windmill"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Windmill"), new Animation(0, 0, 16, 16), new Dictionary<string, List<Animation>>() {

                {"Default",new List<Animation>()
                {
                    new Animation(0,16,16,16)
                } },
                {"Working",new List<Animation>()
                {
                    new Animation(0,16,16,16,20),
                    new Animation(16,16,16,16,20)
                } }
            },"Working"), Color.White, false, null, null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Produces), false, null, null, null), null, ModCore.Configs.machinesConfig.windmillV1_basePowerProduction, 0, false, "", null, 0);
            windMillV1.addComponent(new Vector2(0, 0), windMillV1_0_0);
            windMillV1.addComponent(new Vector2(0, 1), windMillV1_0_1);
            this.AddItem("WindmillV1", windMillV1);


            MultiTiledObject windMillV2 = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.WindmillV2", TextureManager.GetTexture(ModCore.Manifest, "Machines", "ClothWindmill"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("Improved Windmill", "Omegasis.Revitalize.Objects.Machines.WindmillV2", "Generates power from the wind.", "Machine", Color.SteelBlue, -300, 0, false, 700, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "ClothWindmill"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "ClothWindmill"), new Animation(0, 0, 16, 16)), Color.White, false, null, null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Produces), false, null, null, null));
            Windmill windMillV2_0_0 = new Windmill(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.WindmillV2", TextureManager.GetTexture(ModCore.Manifest, "Machines", "ClothWindmill"), typeof(Windmill), Color.White, true), new BasicItemInformation("Improved Windmill", "Omegasis.Revitalize.Objects.Machines.WindmillV2", "Generates power from the wind.", "Machine", Color.SteelBlue, -300, 0, false, 700, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "ClothWindmill"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "ClothWindmill"), new Animation(0, 0, 16, 16), new Dictionary<string, List<Animation>>() {

                {"Default",new List<Animation>()
                {
                    new Animation(0,0,16,16)
                } },
                {"Working",new List<Animation>()
                {
                    new Animation(0,0,16,16,20),
                    new Animation(16,0,16,16,20)
                } }
            },"Working"), Color.White, false, null, null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Produces), false, null, null, null), null, ModCore.Configs.machinesConfig.windmillV2_basePowerProduction, 0, true, "", null, 0);
            Windmill windMillV2_0_1 = new Windmill(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.WindmillV2", TextureManager.GetTexture(ModCore.Manifest, "Machines", "ClothWindmill"), typeof(Windmill), Color.White, true), new BasicItemInformation("Improved Windmill", "Omegasis.Revitalize.Objects.Machines.WindmillV2", "Generates power from the wind.", "Machine", Color.SteelBlue, -300, 0, false, 700, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "ClothWindmill"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "ClothWindmill"), new Animation(0, 0, 16, 16), new Dictionary<string, List<Animation>>() {

                {"Default",new List<Animation>()
                {
                    new Animation(0,16,16,16)
                } },
                {"Working",new List<Animation>()
                {
                    new Animation(0,16,16,16,20),
                    new Animation(16,16,16,16,20)
                } }
            },"Working"), Color.White, false, null, null, new Energy.EnergyManager(500, Enums.EnergyInteractionType.Produces), false, null, null, null), null, ModCore.Configs.machinesConfig.windmillV2_basePowerProduction, 0, false, "", null, 0);
            windMillV2.addComponent(new Vector2(0, 0), windMillV2_0_0);
            windMillV2.addComponent(new Vector2(0, 1), windMillV2_0_1);
            this.AddItem("WindmillV2", windMillV2);
        }

        private void loadInConnectionComponents()
        {
            WireMultiTiledObject copperWire = new WireMultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Wires.CopperWire", TextureManager.GetTexture(ModCore.Manifest, "Machines", "CopperWire"), typeof(Wire), Color.White, true), new BasicItemInformation("Copper Wire", "Omegasis.Revitalize.Objects.Machines.Wire.CopperWire", "Wire made from copper bars. Transfers energy between sources.", "Machine", Color.SteelBlue, -300, 0, false, 15, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "CopperWire"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "CopperWire"), new Animation(0, 0, 16, 16)), Color.White, true, null, null, new Energy.EnergyManager(100, Enums.EnergyInteractionType.Transfers), false));
            Wire copperWire_0_0 = new Wire(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Wires.CopperWire", TextureManager.GetTexture(ModCore.Manifest, "Machines", "CopperWire"), typeof(Wire), Color.White, true), new BasicItemInformation("Copper Wire", "Omegasis.Revitalize.Objects.Machines.Wire.CopperWire", "Wire made from copper bars. Transfers energy between sources.", "Machine", Color.SteelBlue, -300, 0, false, 15, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "CopperWire"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "CopperWire"), new Animation(0, 0, 16, 16)), Color.White, true, null, null, new Energy.EnergyManager(100, Enums.EnergyInteractionType.Transfers),false));
            copperWire.addComponent(new Vector2(0, 0), copperWire_0_0);
            this.AddItem("CopperWire", copperWire);

            PipeMultiTiledObject ironPipe = new PipeMultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Wires.IronPipe", TextureManager.GetTexture(ModCore.Manifest, "Machines", "IronPipe"), typeof(Pipe), Color.White, true), new BasicItemInformation("Iron Pipe", "Omegasis.Revitalize.Objects.Machines.Wire.Pipe", "Pipes made from iron. Transfers fluids between machines.", "Machine", Color.SteelBlue, -300, 0, false, 25, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "IronPipe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "IronPipe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null, null, false,null,null,new Managers.FluidManagerV2(0,false, Enums.FluidInteractionType.Transfers,false)));
            Pipe ironPipe_0_0 = new Pipe(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Objects.Machines.Wires.IronPipe", TextureManager.GetTexture(ModCore.Manifest, "Machines", "IronPipe"), typeof(Pipe), Color.White, true), new BasicItemInformation("Iron Pipe", "Omegasis.Revitalize.Objects.Machines.Wire.Pipe", "Pipes made from iron. Transfers fluids between machines.", "Machine", Color.SteelBlue, -300, 0, false, 25, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "IronPipe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "IronPipe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null, null, false, null, null, new Managers.FluidManagerV2(0, false, Enums.FluidInteractionType.Transfers, false)));
            ironPipe.addComponent(new Vector2(0, 0), ironPipe_0_0);
            this.AddItem("IronPipe", ironPipe);
        }

        private void loadInTools()
        {
            PickaxeExtended bronzePick = new PickaxeExtended(new BasicItemInformation("Bronze Pickaxe", "Omegasis.Revitalize.Items.Tools.BronzePickaxe", "A sturdy pickaxe made from bronze.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "BronzePickaxe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "BronzePickaxe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 2, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "BronzePickaxeWorking"));
            PickaxeExtended steelPick = new PickaxeExtended(new BasicItemInformation("Hardened Pickaxe", "Omegasis.Revitalize.Items.Tools.HardenedPickaxe", "A sturdy pickaxe made from hardened alloy.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "HardenedPickaxe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "HardenedPickaxe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 3, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "HardenedPickaxeWorking"));
            PickaxeExtended titaniumPick = new PickaxeExtended(new BasicItemInformation("Titanium Pickaxe", "Omegasis.Revitalize.Items.Tools.TitaniumPickaxe", "A sturdy pickaxe made from titanium.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "TitaniumPickaxe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "TitaniumPickaxe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 4, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "TitaniumPickaxeWorking"));

            AxeExtended bronzeAxe= new AxeExtended(new BasicItemInformation("Bronze Axe", "Omegasis.Revitalize.Items.Tools.BronzeAxe", "A sturdy axe made from bronze.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "BronzeAxe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "BronzeAxe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 2, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "BronzeAxeWorking"));
            AxeExtended steelAxe = new AxeExtended(new BasicItemInformation("Hardened Axe", "Omegasis.Revitalize.Items.Tools.HardenedAxe", "A sturdy axe made from hardened alloy.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "HardenedAxe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "HardenedAxe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null),3,TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "HardenedAxeWorking"));
            AxeExtended titaniumAxe = new AxeExtended(new BasicItemInformation("Titanium Axe", "Omegasis.Revitalize.Items.Tools.TitaniumAxe", "A sturdy axe made from Titanium.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "TitaniumAxe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "TitaniumAxe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 4, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "TitaniumAxeWorking"));

            HoeExtended bronzeHoe = new HoeExtended(new BasicItemInformation("Bronze Hoe", "Omegasis.Revitalize.Items.Tools.BronzeHoe", "A sturdy hoe made from bronze.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "BronzeHoe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "BronzeHoe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 2, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "BronzeHoeWorking"));
            HoeExtended steelHoe = new HoeExtended(new BasicItemInformation("Hardened Hoe", "Omegasis.Revitalize.Items.Tools.HardenedHoe", "A sturdy hoe made from hardened alloy.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "HardenedHoe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "HardenedHoe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 3, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "HardenedHoeWorking"));
            HoeExtended titaniumHoe = new HoeExtended(new BasicItemInformation("Titanium Hoe", "Omegasis.Revitalize.Items.Tools.TitaniumHoe", "A sturdy hoe made from titanium.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "TitaniumHoe"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "TitaniumHoe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 4, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "TitaniumHoeWorking"));

            WateringCanExtended bronzeCan = new WateringCanExtended(new BasicItemInformation("Bronze Watering Can", "Omegasis.Revitalize.Items.Tools.BronzeWateringCan", "A sturdy watering can made from bronze.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "BronzeWateringCan"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "BronzeWateringCan"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 1, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "BronzeWateringCanWorking"),70);
            WateringCanExtended steelCan = new WateringCanExtended(new BasicItemInformation("Hardened Watering Can", "Omegasis.Revitalize.Items.Tools.HardenedWateringCan", "A sturdy watering can made from hardened alloy.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "HardenedWateringCan"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "HardenedWateringCan"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 2, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "HardenedWateringCanWorking"), 100);
            WateringCanExtended titaniumCan = new WateringCanExtended(new BasicItemInformation("Titanium Watering Can", "Omegasis.Revitalize.Items.Tools.TitaniumWateringCan", "A sturdy watering can made from titanium.", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "TitaniumWateringCan"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "TitaniumWateringCan"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), 3, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "TitaniumWateringCanWorking"), 125);

            MiningDrill miningDrillV1 = new MiningDrill(new BasicItemInformation("Simple Mining Drill", "Omegasis.Revitalize.Items.Tools.MiningDrillV1", "A drill used in mining. Consumes energy instead of stamina.", "Tool", Color.SlateGray, 0, 0, false, 1000, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "MiningDrill"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "MiningDrill"), new Animation(0, 0, 16, 16)), Color.White, true, null, null,new Energy.EnergyManager(200, Enums.EnergyInteractionType.Consumes)), 2, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "MiningDrillWorking"));
            Chainsaw chainsawV1 = new Chainsaw(new BasicItemInformation("Simple Chainsaw", "Omegasis.Revitalize.Items.Tools.ChainsawV1", "A chainsaw used to fell trees and chop wood. Consumes energy instead of stamina.", "Tool", Color.SlateGray, 0, 0, false, 1000, false, false, TextureManager.GetTexture(ModCore.Manifest, "Tools", "Chainsaw"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "Chainsaw"), new Animation(0, 0, 16, 16)), Color.White, true, null, null, new Energy.EnergyManager(200, Enums.EnergyInteractionType.Consumes)), 2, TextureManager.GetExtendedTexture(ModCore.Manifest, "Tools", "ChainsawWorking"));

            this.Tools.Add("BronzePickaxe", bronzePick);
            this.Tools.Add("HardenedPickaxe", steelPick);
            this.Tools.Add("TitaniumPickaxe", titaniumPick);

            this.Tools.Add("BronzeAxe", bronzeAxe);
            this.Tools.Add("HardenedAxe", steelAxe);
            this.Tools.Add("TitaniumAxe", titaniumAxe);

            this.Tools.Add("BronzeHoe", bronzeHoe);
            this.Tools.Add("HardenedHoe", steelHoe);
            this.Tools.Add("TitaniumHoe", titaniumHoe);

            this.Tools.Add("BronzeWateringCan", bronzeCan);
            this.Tools.Add("HardenedWateringCan", steelCan);
            this.Tools.Add("TitaniumWateringCan", titaniumCan);

            this.Tools.Add("MiningDrillV1", miningDrillV1);
            this.Tools.Add("ChainsawV1", chainsawV1);
        }

        /// <summary>
        /// Gets a random object from the dictionary passed in.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public Item getRandomObject(Dictionary<string,CustomObject> dictionary)
        {
            if (dictionary.Count == 0) return null;
            List<CustomObject> objs = new List<CustomObject>();
            foreach(KeyValuePair<string,CustomObject> pair in dictionary)
            {
                objs.Add(pair.Value);
            }
            int rand = Game1.random.Next(0,objs.Count);
            return objs[rand].getOne();
        }

        
        /// <summary>
        /// Gets an object from the dictionary that is passed in.
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public Item getObject(string objectName, Dictionary<string,CustomObject> dictionary)
        {
            if (dictionary.ContainsKey(objectName))
            {
                return dictionary[objectName].getOne();
            }
            else
            {
                throw new Exception("Object pool doesn't contain said object.");
            }
        }
        

        public Item getObject(string objectName, Dictionary<string, MultiTiledObject> dictionary)
        {
            if (dictionary.ContainsKey(objectName))
            {
                return dictionary[objectName].getOne();
            }
            else
            {
                throw new Exception("Object pool doesn't contain said object.");
            }
        }

        /// <summary>
        /// Gets a chair from the object manager.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ChairMultiTiledObject getChair(string name)
        {
            if (this.chairs.ContainsKey(name))
            {
                return (ChairMultiTiledObject)this.chairs[name].getOne();
            }
            else
            {
                throw new Exception("Object pool doesn't contain said object.");
            }
        }
        /// <summary>
        /// Gets a table from the object manager.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TableMultiTiledObject getTable(string name)
        {
            if (this.tables.ContainsKey(name))
            {
                return (TableMultiTiledObject)this.tables[name].getOne();
            }
            else
            {
                throw new Exception("Object pool doesn't contain said object.");
            }
        }

        /// <summary>
        /// Gets a lamp from the object manager.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LampMultiTiledObject getLamp(string name)
        {
            if (this.lamps.ContainsKey(name))
            {
                return (LampMultiTiledObject)this.lamps[name].getOne();
            }
            else
            {
                throw new Exception("Object pool doesn't contain said object.");
            }
        }

        /// <summary>
        /// Gets storage furniture from the object manager.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StorageFurnitureOBJ getStorageFuriture(string name)
        {
            if (this.furnitureStorage.ContainsKey(name))
            {
                return (StorageFurnitureOBJ)this.furnitureStorage[name].getOne();
            }
            else
            {
                throw new Exception("Object pool doesn't contain said object.");
            }
        }

        /// <summary>
        /// Adds in an item to be tracked by the mod's object manager.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="I"></param>
        public void AddItem(string key, CustomObject I)
        {
            if (this.ItemsByName.ContainsKey(key))
            {
                throw new Exception("Item with the same key has already been added into the mod!");
            }
            else
            {
                this.ItemsByName.Add(key, I);
            }
        }

        /// <summary>
        /// Gets an item from the list of modded items.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Stack"></param>
        /// <returns></returns>
        public Item GetItem(string Key,int Stack=1)
        {
            if (this.ItemsByName.ContainsKey(Key))
            {
                Item I= this.ItemsByName[Key].getOne();
                I.Stack = Stack;
                return I;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a tool from the list of managed tools.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Item GetTool(string Name)
        {
            if (this.Tools.ContainsKey(Name)) return this.Tools[Name].getOne();
            else return null;
        }

        /// <summary>
        /// Adds a new object manager to the master pool of managers.
        /// </summary>
        /// <param name="Manifest"></param>
        public static void AddObjectManager(IManifest Manifest)
        {
            if (ObjectPools == null) ObjectPools = new Dictionary<string, ObjectManager>();
            ObjectPools.Add(Manifest.UniqueID, new ObjectManager(Manifest));
        }


        /// <summary>
        /// Cleans up all stored information.
        /// </summary>
        public void returnToTitleCleanUp()
        {

        }

        /// <summary>
        /// Scans all mod items to try to find a match.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public Item getItemByIDAndType(string ID,Type T)
        {

            foreach(var v in this.chairs)
            {
                if(v.Value.GetType()==T && v.Value.info.id == ID)
                {
                   Item I= v.Value.getOne();
                    return I;
                }
            }

            foreach(var v in this.furnitureStorage)
            {
                if (v.Value.GetType() == T && v.Value.info.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }

            foreach(var v in this.generic)
            {
                if (v.Value.GetType() == T && v.Value.info.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }

            foreach(var v in this.ItemsByName)
            {
                if (v.Value.GetType() == T && v.Value.info.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }
            foreach(var v in this.lamps)
            {
                if (v.Value.GetType() == T && v.Value.info.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }
            foreach(var v in this.miscellaneous)
            {
                if (v.Value.GetType() == T && v.Value.info.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }
            foreach(var v in this.rugs)
            {
                if (v.Value.GetType() == T && v.Value.info.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }
            foreach(var v in this.tables)
            {
                if (v.Value.GetType() == T && v.Value.info.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }

            foreach(var v in this.resources.ores)
            {
                if (v.Value.GetType() == T && v.Value.info.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }

            }
            foreach(var v in this.resources.oreVeins)
            {
                if (v.Value.GetType() == T && v.Value.info.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }
            foreach(var v in this.Tools)
            {
                if (v.Value.GetType() == T && (v.Value as IItemInfo).Info.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }

            return null;
        }

    }
}
