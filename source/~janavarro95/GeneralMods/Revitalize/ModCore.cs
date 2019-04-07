using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using PyTK.Extensions;
using PyTK.Types;
using Revitalize.Framework;
using Revitalize.Framework.Crafting;
using Revitalize.Framework.Environment;
using Revitalize.Framework.Graphics;
using Revitalize.Framework.Graphics.Animations;
using Revitalize.Framework.Illuminate;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Player;
using Revitalize.Framework.Utilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace Revitalize
{
    // TODO:
    //
    //  -Multiple Lights On Object
    //  -Illumination Colors
    //  Furniture:
    //      -rugs
    //      -tables
    //      -lamps
    //      -chairs (done)
    //      -benches
    //      -dressers/other storage containers
    //      -fun interactables
    //      -More crafting tables
    //  -Machines
    //      !=Energy
    //      -Furnace
    //      -Seed Maker
    //      -Stone Quarry
    //  -Materials
    //      -Tin/Bronze/Alluminum/Silver?Platinum/Etc
    //  -Crafting Menu
    //  -Item Grab Menu (Extendable)
    //  -Gift Boxes
    //  Magic!
    //      -Alchemy Bags
    //      -Transmutation
    //      -Effect Crystals
    //      -Spell books
    //      -Potions!
    //      -Magic Meter
    //      -Connected chests much like Project EE2 from MC
    //
    //
    //
    //  -Bigger chests
    //
    //  Festivals
    //      -Firework festival?
    //  Stargazing???
    //      -Moon Phases+DarkerNight
    //  Bigger/Better Museum?
    //  More Crops?
    //  More Food?
    // 
    //  Equippables!
    //      -accessories that provide buffs/regen/friendship
    //      -braclets/rings/broaches....more crafting for these???
    //      
    //  Music???
    //      -IDK maybe add in instruments???
    //      
    //  More buildings????
    //  
    //  More Animals???
    //  
    //  Readable Books?
    //  
    //  Custom NPCs for shops???
    //  
    //  Frisbee Minigame?
    //  
    //  HorseRace Minigame/Betting?
    //  
    //  Locations:
    //      -Small Island Home?
    //      
    //  More crops
    //
    //  More monsters
    //  -boss fights
    //
    //  More dungeons??


    public class ModCore : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;

        public static Dictionary<string, CustomObject> customObjects;

        public static Dictionary<string, MultiTiledObject>ObjectGroups;

        public static PlayerInfo playerInfo;

        public static Serializer Serializer;

        public static Dictionary<GameLocation,MultiTiledObject> ObjectsToDraw;

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = this.Monitor;

            this.createDirectories();
            this.initailizeComponents();

            ModHelper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            ModHelper.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
            ModHelper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            ModHelper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
            playerInfo = new PlayerInfo();

            Framework.Graphics.TextureManager.TextureManagers.Add("Furniture", new TextureManager());
            TextureManager.addTexture("Furniture","Oak Chair", new Texture2DExtended(this.Helper, this.ModManifest, Path.Combine("Content","Graphics","Furniture", "Chairs", "OakChair.png")));

            customObjects = new Dictionary<string, CustomObject>();
            ObjectGroups = new Dictionary<string, MultiTiledObject>();

            loadContent();
            Serializer = new Serializer();
            ObjectsToDraw = new Dictionary<GameLocation, MultiTiledObject>();
            
        }

        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            Serializer.returnToTitle();
        }

        private void loadContent()
        {
            MultiTiledComponent obj = new MultiTiledComponent(new BasicItemInformation("CoreObjectTest", "YAY FUN!", "Omegasis.Revitalize.MultiTiledComponent", Color.White, -300, 0, false, 100, Vector2.Zero, true, true, "Omegasis.TEST1", "2048/0/-300/Crafting -9/Play '2048 by Platonymous' at home!/true/true/0/2048", TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair").texture, Color.White, 0, true, typeof(MultiTiledComponent), null, new AnimationManager(TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair"), new Animation(new Rectangle(0, 0, 16, 16))), Color.Red, true, null, null));
            MultiTiledComponent obj2 = new MultiTiledComponent(new BasicItemInformation("CoreObjectTest2", "SomeFun", "Omegasis.Revitalize.MultiTiledComponent", Color.White, -300, 0, false, 100, Vector2.Zero, true, true, "Omegasis.TEST1", "2048/0/-300/Crafting -9/Play '2048 by Platonymous' at home!/true/true/0/2048", TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair").texture, Color.White, 0, true, typeof(MultiTiledComponent), null, new AnimationManager(TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair"), new Animation(new Rectangle(0, 16, 16, 16))), Color.Red, false, null, null));
            MultiTiledComponent obj3 = new MultiTiledComponent(new BasicItemInformation("CoreObjectTest3", "NoFun", "Omegasis.Revitalize.MultiTiledComponent", Color.White, -300, 0, false, 100, Vector2.Zero, true, true, "Omegasis.TEST1", "2048/0/-300/Crafting -9/Play '2048 by Platonymous' at home!/true/true/0/2048", TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair").texture, Color.White, 0, true, typeof(MultiTiledComponent), null, new AnimationManager(TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair"), new Animation(new Rectangle(0, 32, 16, 16))), Color.Red, false, null, null));


            obj.info.lightManager.addLight(new Vector2(Game1.tileSize), new LightSource(4, new Vector2(0, 0), 2.5f, Color.Orange.Invert()), obj);

            MultiTiledObject bigObject = new MultiTiledObject(new BasicItemInformation("MultiTest", "A really big object", "Omegasis.Revitalize.MultiTiledObject", Color.Blue, -300, 0, false, 100, Vector2.Zero, true, true, "Omegasis.BigTiledTest", "2048/0/-300/Crafting -9/Play '2048 by Platonymous' at home!/true/true/0/2048", TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair").texture, Color.White, 0, true, typeof(MultiTiledObject), null, new AnimationManager(), Color.White, false, null, null));
            bigObject.addComponent(new Vector2(0, 0), obj);
            bigObject.addComponent(new Vector2(1, 0), obj2);
            bigObject.addComponent(new Vector2(2, 0), obj3);

            Recipe pie = new Recipe(new Dictionary<Item, int>()
            {
                [bigObject] = 1
            }, new KeyValuePair<Item, int>(new Furniture(3, Vector2.Zero), 1), new StatCost(100, 50, 0, 0));

         


            Framework.Objects.Furniture.ChairTileComponent chairTop = new Framework.Objects.Furniture.ChairTileComponent(new BasicItemInformation("Oak Chair", "A basic wooden chair", "Chairs", Color.Brown, -300, 0, false, 100, Vector2.Zero, true, true, "Omegasis.Revitalize.Furniture.Basic.OakChair", "2048/0/-300/Crafting -9/Play '2048 by Platonymous' at home!/true/true/0/2048", Framework.Graphics.TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair").texture, Color.White, 0, false, typeof(Framework.Objects.Furniture.ChairTileComponent), null, new AnimationManager(TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair"), new Animation(new Rectangle(0, 0, 16, 16)), new Dictionary<string, List<Animation>>() {
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
            }, "Default_" + (int)Framework.Enums.Direction.Down), Color.White, true, new Framework.Utilities.InventoryManager(), new LightManager()), new Framework.Objects.InformationFiles.Furniture.ChairInformation(false));
            Framework.Objects.Furniture.ChairTileComponent chairBottom = new Framework.Objects.Furniture.ChairTileComponent(new BasicItemInformation("Oak Chair", "A basic wooden chair", "Chairs", Color.Brown, -300, 0, false, 100, Vector2.Zero, true, true, "Omegasis.Revitalize.Furniture.Basic.OakChair", "2048/0/-300/Crafting -9/Play '2048 by Platonymous' at home!/true/true/0/2048", Framework.Graphics.TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair").texture, Color.White, 0, false, typeof(Framework.Objects.Furniture.ChairTileComponent), null, new AnimationManager(TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair"), new Animation(new Rectangle(0, 16, 16, 16)), new Dictionary<string, List<Animation>>() {
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
            }, "Default_" + (int)Framework.Enums.Direction.Down), Color.White, false, new Framework.Utilities.InventoryManager(), new LightManager()), new Framework.Objects.InformationFiles.Furniture.ChairInformation(true));


            Framework.Objects.Furniture.ChairMultiTiledObject oakChair = new Framework.Objects.Furniture.ChairMultiTiledObject(new BasicItemInformation("Oak Chair", "A wood chair you can place anywhere.", "Chair", Color.White, -300, 0, true, 100, Vector2.Zero, true, true, "Omegasis.Revitalize.Furniture.OakChair", "2048/0/-300/Crafting -9/Play '2048 by Platonymous' at home!/true/true/0/2048", TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair").texture, Color.White, 0, true, typeof(Revitalize.Framework.Objects.Furniture.ChairMultiTiledObject), null, new AnimationManager(), Color.White, false, new Framework.Utilities.InventoryManager(), new LightManager()));
            oakChair.addComponent(new Vector2(0, -1), chairTop);
            oakChair.addComponent(new Vector2(0, 0), chairBottom);

            customObjects.Add("Omegasis.BigTiledTest", bigObject);
            customObjects.Add("Omegasis.Revitalize.Furniture.Chairs.OakChair",oakChair);

            Framework.Objects.Furniture.RugTileComponent rug1 = new Framework.Objects.Furniture.RugTileComponent(new BasicItemInformation("BasicRugTile", "A basic rug", "Rug", Color.Brown, -300, 0, false, 100, new Vector2(0, 0), true, true, "Omegasis.Revitalize.Furniture.Basic.Rugs.TestRug", generatePlaceholderString(), TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair").texture, Color.White, 0,true, typeof(Framework.Objects.Furniture.RugTileComponent), null, new AnimationManager(TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair"), new Animation(new Rectangle(0, 0, 16, 16))), Color.White, true, null, null));
            Framework.Objects.Furniture.RugMultiTiledObject rug = new Framework.Objects.Furniture.RugMultiTiledObject(new BasicItemInformation("BasicRugTile", "A basic rug", "Rug", Color.Brown, -300, 0, false, 100, new Vector2(0, 0), true, true, "Omegasis.Revitalize.Furniture.Basic.Rugs.TestRug", generatePlaceholderString(), TextureManager.TextureManagers["Furniture"].getTexture("Oak Chair").texture, Color.White, 0, true, typeof(Framework.Objects.Furniture.RugMultiTiledObject), null, new AnimationManager(), Color.White, true, null, null));
            rug.addComponent(new Vector2(0, 0), rug1);

            customObjects.Add("Omegasis.Revitalize.Furniture.Rugs.RugTest", rug);
        }

        private void createDirectories()
        {
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Configs"));

            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath,"Content" ,"Graphics"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics","Furniture"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics", "Furniture","Chairs"));
        }

        private void initailizeComponents()
        {
            DarkerNight.InitializeConfig();
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            DarkerNight.SetDarkerColor();
            playerInfo.update();
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            DarkerNight.CalculateDarkerNightColor();
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            Serializer.afterLoad();
            
            if (Game1.IsServer || Game1.IsMultiplayer || Game1.IsClient)
            {
                throw new Exception("Can't run Revitalize in multiplayer due to lack of current support!");
            }
            //Game1.player.addItemToInventory(customObjects["Omegasis.BigTiledTest"].getOne());
            Game1.player.addItemToInventory(getObjectFromPool("Omegasis.Revitalize.Furniture.Chairs.OakChair"));
            Game1.player.addItemToInventory(getObjectFromPool("Omegasis.Revitalize.Furniture.Rugs.RugTest"));

            /*
            StardewValley.Tools.Axe axe = new StardewValley.Tools.Axe();
            Serializer.Serialize(Path.Combine(this.Helper.DirectoryPath, "AXE.json"), axe);
            axe =(StardewValley.Tools.Axe)Serializer.Deserialize(Path.Combine(this.Helper.DirectoryPath, "AXE.json"),typeof(StardewValley.Tools.Axe));
            //Game1.player.addItemToInventory(axe);
            */

        }

        public Item getObjectFromPool(string objName)
        {
            if (customObjects.ContainsKey(objName))
            {
                return customObjects[objName];
            }
            else
            {
                throw new Exception("Object Key name not found: " + objName);
            }
        }
        

        public static void log(object message)
        {
            ModMonitor.Log(message.ToString());
        }

        public static string generatePlaceholderString()
        {
            return "2048/0/-300/Crafting -9/Play '2048 by Platonymous' at home!/true/true/0/2048";
        }
    }
}
