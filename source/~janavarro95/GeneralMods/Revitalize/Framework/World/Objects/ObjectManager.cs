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
using Revitalize.Framework.Objects;
using Revitalize.Framework.Objects.InformationFiles;
using Revitalize.Framework.World.Objects;
using Revitalize.Framework.World.Objects.CraftingTables;
using Revitalize.Framework.World.Objects.InformationFiles;
using Revitalize.Framework.World.Objects.Interfaces;
using Revitalize.Framework.World.Objects.Machines;
using Revitalize.Framework.World.Objects.Machines.EnergyGeneration;
using Revitalize.Framework.Objects.Items.Tools;
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
            this.loadInAestheticsObjects();
        }

        private void loadInAestheticsObjects()
        {
            /*
            LampMultiTiledObject lighthouse = new LampMultiTiledObject(PyTKHelper.CreateOBJData("Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(MultiTiledObject), Color.White, true), new BasicItemInformation("LightHouse", "Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(), Color.White, false, null, null));
            LampTileComponent lighthouse_0_0 = new LampTileComponent(PyTKHelper.CreateOBJData("Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(0, 0, 16, 16)), Color.White, true, null, new Illuminate.LightManager(),null,true));
            LampTileComponent lighthouse_1_0 = new LampTileComponent(PyTKHelper.CreateOBJData("Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(16, 0, 16, 16)), Color.White, true, null, new Illuminate.LightManager(), null, true));
            LampTileComponent lighthouse_0_1 = new LampTileComponent(PyTKHelper.CreateOBJData("Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(0, 16, 16, 16)), Color.White, true, null, new Illuminate.LightManager(), null, true));
            LampTileComponent lighthouse_1_1 = new LampTileComponent(PyTKHelper.CreateOBJData("Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(16, 16, 16, 16)), Color.White, true, null, new Illuminate.LightManager(), null, true));
            LampTileComponent lighthouse_0_2 = new LampTileComponent(PyTKHelper.CreateOBJData("Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(0, 32, 16, 16)), Color.White, true, null, new Illuminate.LightManager(), null, false));
            LampTileComponent lighthouse_1_2 = new LampTileComponent(PyTKHelper.CreateOBJData("Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(16, 32, 16, 16)), Color.White, true, null, new Illuminate.LightManager(), null, false));
            LampTileComponent lighthouse_0_3 = new LampTileComponent(PyTKHelper.CreateOBJData("Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(0, 48, 16, 16)), Color.White, false, null, new Illuminate.LightManager()));
            LampTileComponent lighthouse_1_3 = new LampTileComponent(PyTKHelper.CreateOBJData("Revitalize.Objects.Furniture.Misc.Lighthouse", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), typeof(LampTileComponent), Color.White, true), new BasicItemInformation("LightHouse", "Revitalize.Objects.Furniture.Misc.Lighthouse", "A minuture lighthouse that provides a decent amount of light.", "Furniture", Color.Brown, -300, 0, false, 2500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Lighthouse"), new Animation(16, 48, 16, 16)), Color.White, false, null, new Illuminate.LightManager()));
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
            */
        }

        private void loadInCraftingTables()
        {
            CraftingTable WorkbenchObj = new CraftingTable(new BasicItemInformation("Workbench", "Revitalize.Objects.Crafting.Workbench", "A workbench that can be used for crafting different objects.", "Crafting", Color.Brown, -300, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Objects.Crafting", "Workbench"), new Animation(0, 0, 32, 32)), Color.White, false, new Vector2(2, 2), null, null), "Workbench");
            CraftingTable AnvilObj = new CraftingTable(new BasicItemInformation("Anvil", "Revitalize.Objects.Crafting.Anvil", "An anvil that can be used for crafting different machines and other metalic objects.", "Crafting", Color.Brown, -300, -300, 0, false, 2000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Objects.Crafting", "Anvil"), new Animation(new Rectangle(0, 0, 32, 32))), Color.White, false, new Vector2(2, 2), null, null), "Anvil");

            this.AddItem("Workbench", WorkbenchObj);
            this.AddItem("Anvil", AnvilObj);
        }

        private void loadInMachines()
        {

            AdvancedSolarPanel solarP1 = new AdvancedSolarPanel(new BasicItemInformation("Solar Panel", "Revitalize.Objects.Machines.SolarPanelV1", "Generates energy while the sun is up.", "Machine", Color.SteelBlue, -300, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarPanelTier1"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SolarPanelTier1"), new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), null, null), 2, 0);
            AdvancedSolarPanel solarA1V1 = new AdvancedSolarPanel(new BasicItemInformation("Solar Array", "Revitalize.Objects.Machines.SolarArrayV1", "A collection of solar panels that generates even more energy while the sun is up.", "Machine", Color.SteelBlue, -300, -300, 0, false, 1000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "SolarArrayTier1"), new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), null, null), 8, 0);

            this.AddItem("SolarPanelTier1", solarP1);
            this.AddItem("SolarArrayTier1", solarA1V1);


            Machine miningDrillMachine_0_0 = new Machine(new BasicItemInformation("Mining Drill", "Revitalize.Objects.Machines.MiningDrill", "Digs up rocks and ores. Requires energy to run.", "Machine", Color.SteelBlue, -300, -300, 0, false, 4000, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "MiningDrillMachine"), new Dictionary<string, Animation>() {
                {"Default",new Animation(new AnimationFrame(0,0,16,16))  },
                { "Mining",new Animation(new List<AnimationFrame>(){
                    new AnimationFrame(0,0,16,32,30),
                    new AnimationFrame(16,0,16,32,30),
                    new AnimationFrame(32,0,16,32,30),
                    new AnimationFrame(48,0,16,32,30)},
                    true) }
            }, "Default", "Mining"), Color.White, false, new Vector2(1, 2), new InventoryManager(18, 3, 6), null), ModCore.ObjectManager.resources.miningDrillResources.Values.ToList(), ModCore.Configs.machinesConfig.miningDrillEnergyConsumption, ModCore.Configs.machinesConfig.miningDrillTimeToMine, "");

            this.AddItem("MiningDrillMachineV1", miningDrillMachine_0_0);


            Windmill windMillV1_0_0 = new Windmill(new BasicItemInformation("Windmill", "Revitalize.Objects.Machines.WindmillV1", "Generates power from the wind.", "Machine", Color.SteelBlue, -300, -300, 0, false, 500, true, true, TextureManager.GetTexture(ModCore.Manifest, "Machines", "Windmill"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Machines", "Windmill"), new Dictionary<string, Animation>() {

                {"Default",new Animation( new AnimationFrame(0,0,16,32)) },
                {"Working",new Animation(new List<AnimationFrame>(){
                    new AnimationFrame(0,0,16,32,20),
                    new AnimationFrame(16,0,16,32,20) },true)
                }
            }, "Default", "Working"), Color.White, false, new Vector2(1, 2), null, null, false, null, null), Vector2.Zero);

            this.AddItem("WindmillV1", windMillV1_0_0);

        }

        /// <summary>
        /// Gets a random object from the dictionary passed in.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public Item getRandomObject(Dictionary<string, CustomObject> dictionary)
        {
            if (dictionary.Count == 0) return null;
            List<CustomObject> objs = new List<CustomObject>();
            foreach (KeyValuePair<string, CustomObject> pair in dictionary)
            {
                objs.Add(pair.Value);
            }
            int rand = Game1.random.Next(0, objs.Count);
            return objs[rand].getOne();
        }


        /// <summary>
        /// Gets an object from the dictionary that is passed in.
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public Item getObject(string objectName, Dictionary<string, CustomObject> dictionary)
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
        public Item GetItem(string Key, int Stack = 1)
        {
            if (this.ItemsByName.ContainsKey(Key))
            {
                Item I = this.ItemsByName[Key].getOne();
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
        public Item getItemByIDAndType(string ID, Type T)
        {

            foreach (var v in this.ItemsByName)
            {
                if (v.Value.GetType() == T && v.Value.basicItemInfo.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }

            foreach (var v in this.resources.ores)
            {
                if (v.Value.GetType() == T && v.Value.basicItemInfo.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }

            }
            foreach (var v in this.resources.oreVeins)
            {
                if (v.Value.GetType() == T && v.Value.basicItemInfo.id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }
            foreach (var v in this.Tools)
            {
                if (v.Value.GetType() == T && (v.Value as IBasicItemInfoProvider).getItemInformation().id == ID)
                {
                    Item I = v.Value.getOne();
                    return I;
                }
            }

            return null;
        }

    }
}
