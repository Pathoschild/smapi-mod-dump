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
using Microsoft.Xna.Framework;
using Revitalize.Framework.Factories.Objects.Furniture;
using Revitalize.Framework.Illuminate;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Objects.Furniture;
using Revitalize.Framework.Objects.InformationFiles.Furniture;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardustCore.UIUtilities;
using StardustCore.Animations;

namespace Revitalize.Framework.Factories.Objects
{
    //TODO: Add Rugs
    //Add Benches
    //Add dressers for storage/appearance change (create this)
    //Create portable beds???
    public class FurnitureFactory
    {
        /// <summary>
        /// The path to the chairs data on disk.
        /// </summary>
        public static string ChairFolder = Path.Combine("Data","Objects" ,"Furniture", "Chairs");
        /// <summary>
        /// The path to the tables data on disk.
        /// </summary>
        public static string TablesFolder = Path.Combine("Data","Objects","Furniture", "Tables");
        /// <summary>
        /// The path to the lamps data on disk.
        /// </summary>
        public static string LampsFolder = Path.Combine("Data","Objects","Furniture", "Lamps");
        /// <summary>
        /// The path to the storage data on disk.
        /// </summary>
        public static string StorageFolder = Path.Combine("Data","Objects" ,"Furniture", "Storage");

        /// <summary>
        /// Loads all furniture files.
        /// </summary>
        public static void LoadFurnitureFiles()
        {
            LoadChairFiles();
            LoadTableFiles();
            LoadLampFiles();
            LoadFurnitureStorageFiles();
        }

        /// <summary>
        /// Loads all chair files.
        /// </summary>
        private static void LoadChairFiles()
        {
            SerializeChairs();
            DeserializeChairs();
        }

        /// <summary>
        /// Loads all table files.
        /// </summary>
        private static void LoadTableFiles()
        {
            SerializeTableFiles();
            DeserializeTableFiles();
        }

        /// <summary>
        /// Loads all lamp files.
        /// </summary>
        private static void LoadLampFiles()
        {
            SerializeLamps();
            DeserializeLamps();
        }

        /// <summary>
        /// Loads all furniture storage files.
        /// </summary>
        private static void LoadFurnitureStorageFiles()
        {
            SerializeFurnitureStorageFiles();
            DeserializeFurnitureStorageFiles();
        }

        /// <summary>
        /// Serialize a lamp to a .json file for easier creation of like objects.
        /// </summary>
        private static void SerializeLamps()
        {
            LampTileComponent lampTop = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Lamps.OakLamp", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), typeof(LampTileComponent), Color.White), new BasicItemInformation("Oak Lamp", "Omegasis.Revitalize.Furniture.Lamps.OakLamp", "A basic wooden light.", "Lamps", Color.Brown, -300, 0, true, 100, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), new Animation(new Rectangle(0, 0, 16, 16))), Color.White,true, new InventoryManager(), new LightManager()));

            LampTileComponent lampMiddle = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Lamps.OakLamp", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), typeof(LampTileComponent), Color.White), new BasicItemInformation("Oak Lamp", "Omegasis.Revitalize.Furniture.Lamps.OakLamp", "A basic wooden light.", "Lamps", Color.Brown, -300, 0, true, 100, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), new Animation(new Rectangle(0, 16, 16, 16))), Color.White, true, new InventoryManager(), new LightManager()));
            LampTileComponent lampBottom = new LampTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Lamps.OakLamp", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), typeof(LampTileComponent), Color.White), new BasicItemInformation("Oak Lamp", "Omegasis.Revitalize.Furniture.Lamps.OakLamp", "A basic wooden light.", "Lamps", Color.Brown, -300, 0, true, 100 , true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), new Animation(new Rectangle(0, 32, 16, 16))), Color.White, false, new InventoryManager(), new LightManager()));

            lampMiddle.lightManager.addLight(new Vector2(Game1.tileSize), new LightSource(4, new Vector2(0, 0), 2.5f, Color.Orange.Invert()), lampMiddle);

            LampMultiTiledObject lamp = new LampMultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Lamps.OakLamp", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), typeof(LampMultiTiledObject), Color.White), new BasicItemInformation("Oak Lamp", "Omegasis.Revitalize.Furniture.Lamps.OakLamp", "A basic wooden light", "Lamps", Color.Brown, -300, 0, true, 300, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Lamp"), new AnimationManager(), Color.White, false, new InventoryManager(), new LightManager()));

            lamp.addComponent(new Vector2(0, -2), lampTop);
            lamp.addComponent(new Vector2(0, -1), lampMiddle);
            lamp.addComponent(new Vector2(0, 0), lampBottom);

            FactoryInfo lT = new FactoryInfo(lampTop);
            FactoryInfo lM = new FactoryInfo(lampMiddle);
            FactoryInfo lB = new FactoryInfo(lampBottom);

            FactoryInfo lO = new FactoryInfo(lamp);

            ModCore.Serializer.SerializeContentFile("OakLamp_0_-2", lT,Path.Combine(LampsFolder,"OakLamp"));
            ModCore.Serializer.SerializeContentFile("OakLamp_0_-1", lM, Path.Combine(LampsFolder, "OakLamp"));
            ModCore.Serializer.SerializeContentFile("OakLamp_0_0", lB, Path.Combine(LampsFolder, "OakLamp"));
            ModCore.Serializer.SerializeContentFile("OakLamp", lO, Path.Combine(LampsFolder, "OakLamp"));

            //ModCore.customObjects.Add(lamp.info.id, lamp);
        }

        /// <summary>
        /// Deserializes all lamp files for the mod.
        /// </summary>
        private static void DeserializeLamps()
        {
            if (!Directory.Exists(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", LampsFolder))) Directory.CreateDirectory(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", LampsFolder));
            string[] directories = Directory.GetDirectories(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", LampsFolder));

            foreach(string directory in directories)
            {
                string[] files = Directory.GetFiles(directory);

                Dictionary<string, LampMultiTiledObject> objs = new Dictionary<string, LampMultiTiledObject>();

                //Deserialize container.
                foreach (string file in files)
                {
                    if ((Path.GetFileName(file)).Contains("_") == true) continue;
                    else
                    {
                        FactoryInfo factoryInfo = ModCore.Serializer.DeserializeContentFile<FactoryInfo>(file);
                        objs.Add(Path.GetFileNameWithoutExtension(file), new LampMultiTiledObject(factoryInfo.PyTkData,factoryInfo.info));
                    }
                }
                //Deseralize components
                foreach (string file in files)
                {
                    if ((Path.GetFileName(file)).Contains("_") == false) continue;
                    else
                    {

                        string[] splits = Path.GetFileNameWithoutExtension(file).Split('_');
                        string name = splits[0];
                        Vector2 offset = new Vector2(Convert.ToInt32(splits[1]), Convert.ToInt32(splits[2]));
                        FactoryInfo info = ModCore.Serializer.DeserializeContentFile<FactoryInfo>(file);

                        LampTileComponent lampPiece = new LampTileComponent(info.PyTkData,info.info);
                        //Recreate the lights info.
                        if (lampPiece.lightManager != null)
                        {
                            //ModCore.log("Info for file"+Path.GetFileNameWithoutExtension(file)+" has this many lights: " + info.info.lightManager.fakeLights.Count);
                            lampPiece.lightManager.lights.Clear();
                            foreach (KeyValuePair<Vector2, FakeLightSource> light in info.info.lightManager.fakeLights)
                            {
                                lampPiece.lightManager.addLight(new Vector2(Game1.tileSize), new LightSource(light.Value.id, new Vector2(0, 0), light.Value.radius, light.Value.color.Invert()), lampPiece);
                            }
                        }


                        objs[name].addComponent(offset, lampPiece);
                    }
                }
                foreach (var v in objs)
                {
                    ModCore.ObjectManager.lamps.Add(v.Value.info.id, v.Value);
                }
            }


        }

        /// <summary>
        /// Serialize all chair basic information to a file to have as a reference for making other like objects.
        /// </summary>
        private static void SerializeChairs()
        {
            Framework.Objects.Furniture.ChairTileComponent chairTop = new ChairTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Chairs.OakChair", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Chair"), typeof(ChairTileComponent), Color.White), new BasicItemInformation("Oak Chair", "Omegasis.Revitalize.Furniture.Chairs.OakChair", "A basic wooden chair made out of oak.", "Chairs", Color.Brown, -300, 0, false, 250, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Chair"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest,"Furniture","Oak Chair"), new Animation(new Rectangle(0, 0, 16, 16)), new Dictionary<string, List<Animation>>() {
                { "Default_" + (int)Framework.Enums.Direction.Down , new List<Animation>()
                    {
                        new Animation(new Rectangle(0,0,16,16))
                    }
                },
                { "Sitting_" + (int)Framework.Enums.Direction.Down , new List<Animation>()
                    {
                        new Animation(new Rectangle(0,0,16,16))
                    }
                },
                { "Default_" + (int)Framework.Enums.Direction.Right , new List<Animation>()
                    {
                        new Animation(new Rectangle(16,0,16,16))
                    }
                },
                { "Sitting_" + (int)Framework.Enums.Direction.Right , new List<Animation>()
                    {
                        new Animation(new Rectangle(16,0,16,16))
                    }
                },
                { "Default_" + (int)Framework.Enums.Direction.Up , new List<Animation>()
                    {
                        new Animation(new Rectangle(32,0,16,16))
                    }
                },
                { "Sitting_" + (int)Framework.Enums.Direction.Up , new List<Animation>()
                    {
                        new Animation(new Rectangle(32,32,16,32))
                    }
                },
                { "Default_" + (int)Framework.Enums.Direction.Left , new List<Animation>()
                    {
                        new Animation(new Rectangle(48,0,16,16))
                    }
                },
                { "Sitting_" + (int)Framework.Enums.Direction.Left , new List<Animation>()
                    {
                        new Animation(new Rectangle(48,0,16,16))
                    }
                }
            }, "Default_" + (int)Framework.Enums.Direction.Down), Color.White, true, null, null),new ChairInformation(false));


            Framework.Objects.Furniture.ChairTileComponent chairBottom = new ChairTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Chairs.OakChair", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Chair"), typeof(ChairTileComponent), Color.White), new BasicItemInformation("Oak Chair", "Omegasis.Revitalize.Furniture.Chairs.OakChair", "A basic wooden chair.", "Chairs", Color.Brown, -300, 0, false, 250, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Chair"), new AnimationManager(TextureManager.TextureManagers[ModCore.Manifest.UniqueID]["Furniture"].getTexture("Oak Chair"), new Animation(new Rectangle(0, 16, 16, 16)), new Dictionary<string, List<Animation>>() {
                { "Default_" + (int)Framework.Enums.Direction.Down , new List<Animation>()
                    {
                        new Animation(new Rectangle(0,16,16,16))
                    }
                },
                { "Sitting_" + (int)Framework.Enums.Direction.Down , new List<Animation>()
                    {
                        new Animation(new Rectangle(0,16,16,16))
                    }
                },
                { "Default_" + (int)Framework.Enums.Direction.Right , new List<Animation>()
                    {
                        new Animation(new Rectangle(16,16,16,16))
                    }
                },
                { "Sitting_" + (int)Framework.Enums.Direction.Right , new List<Animation>()
                    {
                        new Animation(new Rectangle(16,16,16,16))
                    }
                },
                { "Default_" + (int)Framework.Enums.Direction.Up , new List<Animation>()
                    {
                        new Animation(new Rectangle(32,16,16,16))
                    }
                },
                { "Sitting_" + (int)Framework.Enums.Direction.Up , new List<Animation>()
                    {
                        new Animation(new Rectangle(48,32,16,32))
                    }
                },
                { "Default_" + (int)Framework.Enums.Direction.Left , new List<Animation>()
                    {
                        new Animation(new Rectangle(48,16,16,16))
                    }
                },
                { "Sitting" + (int)Framework.Enums.Direction.Left , new List<Animation>()
                    {
                        new Animation(new Rectangle(48,16,16,16))
                    }
                }
            }, "Default_" + (int)Framework.Enums.Direction.Down), Color.White, false, null, null), new ChairInformation(true));

            Framework.Objects.Furniture.ChairMultiTiledObject oakChair = new ChairMultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Chairs.OakChair", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Chair"), typeof(ChairMultiTiledObject), Color.White), new BasicItemInformation("Oak Chair", "Omegasis.Revitalize.Furniture.Chairs.OakChair", "A basic wooden chair.", "Chairs", Color.White, -300, 0, false, 250, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Chair"), new AnimationManager(), Color.White, false, null, null));


            ChairFactoryInfo top = new ChairFactoryInfo(chairTop);
            ChairFactoryInfo bottom = new ChairFactoryInfo(chairBottom);
            ChairFactoryInfo obj = new ChairFactoryInfo(oakChair);


            ModCore.Serializer.SerializeContentFile("OakChair_0_-1", top, Path.Combine(ChairFolder, "OakChair"));
            ModCore.Serializer.SerializeContentFile("OakChair_0_0", bottom, Path.Combine(ChairFolder, "OakChair"));
            ModCore.Serializer.SerializeContentFile("OakChair", obj, Path.Combine(ChairFolder, "OakChair"));
        }
        /// <summary>
        /// Deserializes all chair files for the mod.
        /// </summary>
        private static void DeserializeChairs()
        {
            if (!Directory.Exists(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", ChairFolder))) Directory.CreateDirectory(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", ChairFolder));
            string[] directories = Directory.GetDirectories(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", ChairFolder));

            foreach (string directory in directories)
            {
                string[] files = Directory.GetFiles(directory);

                Dictionary<string, ChairMultiTiledObject> chairObjects = new Dictionary<string, ChairMultiTiledObject>();

                //Deserialize container.
                foreach (string file in files)
                {
                    if ((Path.GetFileName(file)).Contains("_") == true) continue;
                    else
                    {
                        ChairFactoryInfo factoryInfo = ModCore.Serializer.DeserializeContentFile<ChairFactoryInfo>(file);
                        chairObjects.Add(Path.GetFileNameWithoutExtension(file), new ChairMultiTiledObject(factoryInfo.PyTkData,factoryInfo.info));
                    }
                }
                //Deseralize components
                foreach (string file in files)
                {
                    if ((Path.GetFileName(file)).Contains("_") == false) continue;
                    else
                    {

                        string[] splits = Path.GetFileNameWithoutExtension(file).Split('_');
                        string name = splits[0];
                        Vector2 offset = new Vector2(Convert.ToInt32(splits[1]), Convert.ToInt32(splits[2]));
                        ChairFactoryInfo info = ModCore.Serializer.DeserializeContentFile<ChairFactoryInfo>(file);
                        chairObjects[name].addComponent(offset, new ChairTileComponent(info.PyTkData,info.info, info.chairInfo));
                    }
                }
                foreach (var v in chairObjects)
                {
                    ModCore.ObjectManager.chairs.Add(v.Value.info.id, v.Value);
                    ModCore.ObjectManager.AddItem(v.Value.info.name, v.Value);
                }
            }
        }

        /// <summary>
        /// Creates an example table file for the mod.
        /// </summary>
        private static void SerializeTableFiles()
        {
            TableTileComponent upperLeft = new TableTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Tables.OakTable", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), typeof(TableTileComponent), Color.White), new BasicItemInformation("Oak Table", "Omegasis.Revitalize.Furniture.Tables.OakTable", "A simple wooden table to place objects on.", "Tables", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Table"), new Animation(0, 0, 16, 16)), Color.White, true, null, null), new TableInformation(true));
            TableTileComponent upperRight = new TableTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Tables.OakTable", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), typeof(TableTileComponent), Color.White), new BasicItemInformation("Oak Table", "Omegasis.Revitalize.Furniture.Tables.OakTable", "A simple wooden table to place objects on.", "Tables", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Table"), new Animation(16, 0, 16, 16)), Color.White, true, null, null), new TableInformation(true));
            TableTileComponent centerLeft = new TableTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Tables.OakTable", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), typeof(TableTileComponent), Color.White), new BasicItemInformation("Oak Table", "Omegasis.Revitalize.Furniture.Tables.OakTable", "A simple wooden table to place objects on.", "Tables", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Table"), new Animation(0, 16, 16, 16)), Color.White, false, null, null), new TableInformation(true));
            TableTileComponent centerRight = new TableTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Tables.OakTable", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), typeof(TableTileComponent), Color.White), new BasicItemInformation("Oak Table", "Omegasis.Revitalize.Furniture.Tables.OakTable", "A simple wooden table to place objects on.", "Tables", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Table"), new Animation(16, 16, 16, 16)), Color.White, false, null, null), new TableInformation(true));
            TableTileComponent bottomLeft = new TableTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Tables.OakTable", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), typeof(TableTileComponent), Color.White), new BasicItemInformation("Oak Table", "Omegasis.Revitalize.Furniture.Tables.OakTable", "A simple wooden table to place objects on.", "Tables", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Table"), new Animation(0, 32, 16, 16)), Color.White, false, null, null), new TableInformation(true));
            TableTileComponent bottomRight = new TableTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Tables.OakTable", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), typeof(TableTileComponent), Color.White), new BasicItemInformation("Oak Table", "Omegasis.Revitalize.Furniture.Tables.OakTable", "A simple wooden table to place objects on.", "Tables", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Table"), new Animation(16, 32, 16, 16)), Color.White, false, null, null), new TableInformation(true));

            TableMultiTiledObject obj = new TableMultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Tables.OakTable", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), typeof(TableMultiTiledObject), Color.White), new BasicItemInformation("Oak Table", "Omegasis.Revitalize.Furniture.Tables.OakTable", "A simple oak table to place things on.", "Tables", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), new AnimationManager(), Color.White, false, null, null));

            TableFactoryInfo uL = new TableFactoryInfo(upperLeft);
            TableFactoryInfo uR = new TableFactoryInfo(upperRight);
            TableFactoryInfo cL = new TableFactoryInfo(centerLeft);
            TableFactoryInfo cR = new TableFactoryInfo(centerRight);
            TableFactoryInfo bL = new TableFactoryInfo(bottomLeft);
            TableFactoryInfo bR = new TableFactoryInfo(bottomRight);

            TableFactoryInfo table = new TableFactoryInfo(obj);


            ModCore.Serializer.SerializeContentFile("OakTable_0_0", uL, Path.Combine(TablesFolder, "OakTable"));
            ModCore.Serializer.SerializeContentFile("OakTable_1_0", uR, Path.Combine(TablesFolder, "OakTable"));
            ModCore.Serializer.SerializeContentFile("OakTable_0_1", cL, Path.Combine(TablesFolder, "OakTable"));
            ModCore.Serializer.SerializeContentFile("OakTable_1_1", cR, Path.Combine(TablesFolder, "OakTable"));
            ModCore.Serializer.SerializeContentFile("OakTable_0_2", bL, Path.Combine(TablesFolder, "OakTable"));
            ModCore.Serializer.SerializeContentFile("OakTable_1_2", bR, Path.Combine(TablesFolder, "OakTable"));

            ModCore.Serializer.SerializeContentFile("OakTable", table, Path.Combine(TablesFolder, "OakTable"));

        }

        /// <summary>
        /// Deserailzes all table files for the mod.
        /// </summary>
        private static void DeserializeTableFiles()
        {
            if (!Directory.Exists(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", TablesFolder))) Directory.CreateDirectory(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", TablesFolder));

            string[] directories = Directory.GetDirectories(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", TablesFolder));
            foreach (string directory in directories)
            {

                string[] files = Directory.GetFiles(directory);

                Dictionary<string, TableMultiTiledObject> chairObjects = new Dictionary<string, TableMultiTiledObject>();

                //Deserialize container.
                foreach (string file in files)
                {
                    if ((Path.GetFileName(file)).Contains("_") == true) continue;
                    else
                    {
                        TableFactoryInfo factoryInfo = ModCore.Serializer.DeserializeContentFile<TableFactoryInfo>(file);
                        chairObjects.Add(Path.GetFileNameWithoutExtension(file), new TableMultiTiledObject(factoryInfo.PyTkData,factoryInfo.info));
                    }
                }
                //Deseralize components
                foreach (string file in files)
                {
                    if ((Path.GetFileName(file)).Contains("_") == false) continue;
                    else
                    {

                        string[] splits = Path.GetFileNameWithoutExtension(file).Split('_');
                        string name = splits[0];
                        Vector2 offset = new Vector2(Convert.ToInt32(splits[1]), Convert.ToInt32(splits[2]));
                        TableFactoryInfo info = ModCore.Serializer.DeserializeContentFile<TableFactoryInfo>(file);
                        chairObjects[name].addComponent(offset, new TableTileComponent(info.PyTkData,info.info, info.tableInfo));
                    }
                }
                foreach (var v in chairObjects)
                {
                    ModCore.ObjectManager.tables.Add(v.Value.info.id, v.Value);
                    ModCore.ObjectManager.AddItem(v.Value.info.name, v.Value);
                }
            }
        }

        /// <summary>
        /// Creates an example storage file for the mod.
        /// </summary>
        private static void SerializeFurnitureStorageFiles()
        {
            StorageFurnitureTile upperLeft = new StorageFurnitureTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Storage.OakCabinet", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), typeof(StorageFurnitureTile), Color.White), new BasicItemInformation("Oak Cabinet", "Omegasis.Revitalize.Furniture.Storage.OakCabinet", "A beautiful oak cabinet to place objects inside of.", "Storage", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), new Animation(0, 0, 16, 16)), Color.White, false, null, null));
            StorageFurnitureTile upperRight = new StorageFurnitureTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Storage.OakCabinet", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), typeof(StorageFurnitureTile), Color.White), new BasicItemInformation("Oak Cabinet", "Omegasis.Revitalize.Furniture.Storage.OakCabinet", "A beautiful oak cabinet to place objects inside of.", "Storage", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), new Animation(16, 0, 16, 16)), Color.White, false, null, null));
            StorageFurnitureTile bottomLeft = new StorageFurnitureTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Storage.OakCabinet", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), typeof(StorageFurnitureTile), Color.White), new BasicItemInformation("Oak Cabinet", "Omegasis.Revitalize.Furniture.Storage.OakCabinet", "A beautiful oak cabinet to place objects inside of.", "Storage", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), new Animation(0, 16, 16, 16)), Color.White, false, null, null));
            StorageFurnitureTile bottomRight = new StorageFurnitureTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Storage.OakCabinet", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), typeof(StorageFurnitureTile), Color.White), new BasicItemInformation("Oak Cabinet", "Omegasis.Revitalize.Furniture.Storage.OakCabinet", "A beautiful oak cabinet to place objects inside of.", "Storage", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), new Animation(16, 16, 16, 16)), Color.White, false, null, null));


            StorageFurnitureOBJ obj = new StorageFurnitureOBJ(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Storage.OakCabinet", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), typeof(StorageFurnitureTile), Color.White), new BasicItemInformation("Oak Cabinet", "Omegasis.Revitalize.Furniture.Storage.OakCabinet", "A beautiful oak cabinet to place objects inside of.", "Storage", Color.Brown, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Furniture", "Oak Cabinet"), new Animation(16, 16, 16, 16)), Color.White, false, new InventoryManager(9), null));

            FactoryInfo uL = new FactoryInfo(upperLeft);
            FactoryInfo uR = new FactoryInfo(upperRight);
            FactoryInfo bL = new FactoryInfo(bottomLeft);
            FactoryInfo bR = new FactoryInfo(bottomRight);
            FactoryInfo cabinet = new FactoryInfo(obj);
            //TableMultiTiledObject obj = new TableMultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Tables.OakTable", TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), typeof(TableMultiTiledObject), Color.White), new BasicItemInformation("Oak Table", "Omegasis.Revitalize.Furniture.Tables.OakTable", "A simple oak table to place things on.", "Tables", Color.Brown, -300, 0, false, 350, Vector2.Zero, true, true, TextureManager.GetTexture(ModCore.Manifest, "Furniture", "Oak Table"), new AnimationManager(), Color.White, false, null, null));

            //TableFactoryInfo uL = new TableFactoryInfo(upperLeft);
            //TableFactoryInfo uR = new TableFactoryInfo(upperRight);
            //TableFactoryInfo cL = new TableFactoryInfo(centerLeft);
            //TableFactoryInfo cR = new TableFactoryInfo(centerRight);
            //TableFactoryInfo bR = new TableFactoryInfo(bottomRight);



            ModCore.Serializer.SerializeContentFile("OakCabinet_0_0", uL, Path.Combine(StorageFolder, "OakCabinet"));
            ModCore.Serializer.SerializeContentFile("OakCabinet_1_0", uR, Path.Combine(StorageFolder, "OakCabinet"));
            ModCore.Serializer.SerializeContentFile("OakCabinet_0_1", bL, Path.Combine(StorageFolder, "OakCabinet"));
            ModCore.Serializer.SerializeContentFile("OakCabinet_1_1", bR, Path.Combine(StorageFolder, "OakCabinet"));
            ModCore.Serializer.SerializeContentFile("OakCabinet", cabinet, Path.Combine(StorageFolder, "OakCabinet"));

        }

        /// <summary>
        /// Deserializes the furniure files for the mod.
        /// </summary>
        private static void DeserializeFurnitureStorageFiles()
        {
            if (!Directory.Exists(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", StorageFolder))) Directory.CreateDirectory(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", StorageFolder));

            string[] directories = Directory.GetDirectories(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", StorageFolder));
            foreach (string directory in directories)
            {

                string[] files = Directory.GetFiles(directory);

                Dictionary<string, StorageFurnitureOBJ> chairObjects = new Dictionary<string, StorageFurnitureOBJ>();

                //Deserialize container.
                foreach (string file in files)
                {
                    if ((Path.GetFileName(file)).Contains("_") == true) continue;
                    else
                    {
                        FactoryInfo factoryInfo = ModCore.Serializer.DeserializeContentFile<FactoryInfo>(file);
                        chairObjects.Add(Path.GetFileNameWithoutExtension(file), new StorageFurnitureOBJ(factoryInfo.PyTkData, factoryInfo.info));
                    }
                }
                //Deseralize components
                foreach (string file in files)
                {
                    if ((Path.GetFileName(file)).Contains("_") == false) continue;
                    else
                    {

                        string[] splits = Path.GetFileNameWithoutExtension(file).Split('_');
                        string name = splits[0];
                        Vector2 offset = new Vector2(Convert.ToInt32(splits[1]), Convert.ToInt32(splits[2]));
                        FactoryInfo info = ModCore.Serializer.DeserializeContentFile<FactoryInfo>(file);
                        chairObjects[name].addComponent(offset, new StorageFurnitureTile(info.PyTkData, info.info));
                    }
                }
                foreach (var v in chairObjects)
                {
                    ModCore.ObjectManager.furnitureStorage.Add(v.Value.info.id, v.Value);
                    ModCore.ObjectManager.AddItem(v.Value.info.name, v.Value);
                }
            }
        }

        /// <summary>
        /// Gets a chair from the object manager.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ChairMultiTiledObject GetChair(string name)
        {
            return (ChairMultiTiledObject)ModCore.ObjectManager.getChair(name);
        }
        /// <summary>
        /// Gets a table from the object manager.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TableMultiTiledObject GetTable(string name)
        {
            return (TableMultiTiledObject)ModCore.ObjectManager.getTable(name);
        }

        /// <summary>
        /// Gets a lamp from the object manager.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static LampMultiTiledObject GetLamp(string name)
        {
            return (LampMultiTiledObject)ModCore.ObjectManager.getLamp(name);
        }

        /// <summary>
        /// Gets a furniture storage file for the mod.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static StorageFurnitureOBJ GetFurnitureStorage(string name)
        {
            return (StorageFurnitureOBJ)ModCore.ObjectManager.getStorageFuriture(name);
        }

    }
}
