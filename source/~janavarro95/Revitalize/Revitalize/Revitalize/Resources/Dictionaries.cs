using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Revitalize.Resources.DataNodes;
using Revitalize.Objects;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Revitalize.Resources
{

    class Dictionaries
    {

        public delegate CoreObject par(string data);
        public delegate void ser(Item item);
        public delegate void world(CoreObject obj);

        public delegate void interactFunction();

        public static Dictionary<string, SerializerDataNode> acceptedTypes;
        public static Dictionary<string, interactFunction> interactionTypes;
        public static Dictionary<string, QuarryDataNode> quarryList;
        public static Dictionary<string, SeedDataNode> seedList;
        public static Dictionary<int, Spell> spellList;
        public static Dictionary<string, TextureDataNode> spriteFontList;

        public static Dictionary<string, WeatherDebris> weatherDebrisDictionary;

        public static void initializeDictionaries()
        {
            acceptedTypes = new Dictionary<string, SerializerDataNode>();
            quarryList = new Dictionary<string, QuarryDataNode>();
            interactionTypes = new Dictionary<string, interactFunction>();
            seedList = new Dictionary<string, SeedDataNode>();
            spellList = new Dictionary<int, Spell>();
            weatherDebrisDictionary = new Dictionary<string, WeatherDebris>();
            spriteFontList = new Dictionary<string, TextureDataNode>();
            fillAllDictionaries();
       }

        public static void fillAllDictionaries()
        {
            addAllAcceptedTypes();
            addAllInteractionTypes();
            fillQuaryList();
            fillSeedList();
            fillSpellList();
            fillWeatherDebrisList();
            fillSpriteFontList();
        }
     

        public static void addAllAcceptedTypes()
        {
            acceptedTypes.Add("Revitalize.Objects.Decoration", new SerializerDataNode(new ser(Serialize.serializeDecoration) ,new par(Serialize.parseDecoration),new world(Serialize.serializeDecorationFromWorld)));
            acceptedTypes.Add("Revitalize.Objects.Light", new SerializerDataNode(new ser(Serialize.serializeLight), new par(Serialize.parseLight),new world(Serialize.serializeLightFromWorld)));
            acceptedTypes.Add("Revitalize.Objects.SpriteFontObject", new SerializerDataNode(new ser(Serialize.serializeSpriteFontObject), new par(Serialize.parseSpriteFontObject), new world(Serialize.serializeSpriteFontObjectFromWorld)));
            acceptedTypes.Add("Revitalize.Objects.shopObject", new SerializerDataNode(new ser(Serialize.serializeShopObject), new par(Serialize.parseShopObject), new world(Serialize.serializeShopObjectFromWorld)));
            acceptedTypes.Add("Revitalize.Objects.Machines.Quarry", new SerializerDataNode(new ser(Serialize.serializeQuarry), new par(Serialize.parseQuarry),new world(Serialize.serializeQuarryFromWorld)));
            acceptedTypes.Add("Revitalize.Objects.Machines.Spawner", new SerializerDataNode(new ser(Serialize.serializeSpawner), new par(Serialize.parseSpawner), new world(Serialize.serializeSpawnerFromWorld)));
            acceptedTypes.Add("Revitalize.Objects.GiftPackage", new SerializerDataNode(new ser(Serialize.serializeGiftPackage), new par(Serialize.parseGiftPackage),null));
            acceptedTypes.Add("Revitalize.Objects.ExtraSeeds", new SerializerDataNode(new ser(Serialize.serializeExtraSeeds), new par(Serialize.parseExtraSeeds),null));
            acceptedTypes.Add("Revitalize.Objects.Spell", new SerializerDataNode(new ser(Serialize.serializeSpell), new par(Serialize.parseSpell), null));
            acceptedTypes.Add("Revitalize.Magic.Alchemy.Objects.BagofHolding", new SerializerDataNode(new ser(Serialize.serializeBagOfHolding), new par(Serialize.parseBagOfHolding), null));
            acceptedTypes.Add("Revitalize.Objects.Machines.TestMachine", new SerializerDataNode(new ser(Revitalize.Objects.Machines.TestMachine.SerializeObject), new par(Objects.Machines.TestMachine.ParseObject), new world(Objects.Machines.TestMachine.SerializeObjectFromWorld)));
        }

        public static void addAllInteractionTypes()
        {
            interactionTypes.Add("Seed", Util.plantCropHere); //for generic stardew seeds
            interactionTypes.Add("Seeds", Util.plantExtraCropHere); //for modded stardew seeds
            interactionTypes.Add("Gift Package", Util.getGiftPackageContents);
            interactionTypes.Add("Spell", Magic.MagicMonitor.castMagic);
            interactionTypes.Add("Bag of Holding", Revitalize.Magic.Alchemy.Objects.BagofHolding.OpenBag);
          
        }


       

        public static void fillQuaryList()
        {
            quarryList.Add("clay", new QuarryDataNode("clay", new StardewValley.Object(330, 1, false), 60));
            quarryList.Add("stone", new QuarryDataNode("stone", new StardewValley.Object(390, 1, false), 60));
            quarryList.Add("coal", new QuarryDataNode("coal", new StardewValley.Object(382, 1, false), 240));
            quarryList.Add("copper", new QuarryDataNode("copper",new StardewValley.Object(378,1,false),120));
            quarryList.Add("iron", new QuarryDataNode("iron", new StardewValley.Object(380, 1, false), 480));
            quarryList.Add("gold", new QuarryDataNode("gold", new StardewValley.Object(384, 1, false), 1440));
            quarryList.Add("irridium", new QuarryDataNode("irridium", new StardewValley.Object(386, 1, false), 4320));

        }

        public static void fillSeedList()
        {
            //crop row number is actually counts row 0 on upper left and row right on upper right.
            //parentsheetindex for seeds image, actualCropNumber from crops.xnb
            seedList.Add("Pink Turnip Seeds", new SeedDataNode(1,1)); 
            seedList.Add("Blue Charm Seeds", new SeedDataNode(2, 2));
        }

        public static void fillSpellList()
        {            
            Spell book;
            //add in a single spell book to my system

            //testing
            book = new Spell(0, Vector2.Zero,new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.TestingSpells.showRedMessage),1),Color.Aqua,0);
            spellList.Add(0, book);

            //crops
            book = new Spell(1, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.CropSpells.cropGrowthSpell), 1), Color.ForestGreen,0);
            spellList.Add(1, book);
            book = new Spell(2, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.CropSpells.waterCropSpell), 1), Color.Aquamarine,0);
            spellList.Add(2, book);

            //Utility Spells
            book = new Spell(3, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.UtilitySpells.warpHome), 1), Color.Gray, 0);
            spellList.Add(3, book);

            //weather
            book = new Spell(4, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.UtilitySpells.sunnyWeather), 1), Color.LightYellow, 0);
            spellList.Add(4, book);
            book = new Spell(5, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.UtilitySpells.rainyWeather), 1), Color.Blue, 0);
            spellList.Add(5, book);
            book = new Spell(6, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.UtilitySpells.stormyWeather), 1), Color.DarkBlue, 0);
            spellList.Add(6, book);

            //health restoring
            book = new Spell(7, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.PlayerSpecificSpells.firstAide), 1), Util.invertColor(LightColors.LightCoral), 0);
            spellList.Add(7, book);
            book = new Spell(8, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.PlayerSpecificSpells.heal), 1), Color.LightPink, 0);
            spellList.Add(8, book);
            book = new Spell(9, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.PlayerSpecificSpells.cure), 1), Color.PaleVioletRed, 0);
            spellList.Add(9, book);
            book = new Spell(10, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.PlayerSpecificSpells.mend), 1), Color.DarkRed, 0);
            spellList.Add(10, book);

            //stamina restoring
            book = new Spell(11, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.PlayerSpecificSpells.deepBreaths), 1), Color.LightCyan, 0);
            spellList.Add(11, book);
            book = new Spell(12, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.PlayerSpecificSpells.refresh), 1), Color.LightGreen, 0);
            spellList.Add(12, book);
            book = new Spell(13, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.PlayerSpecificSpells.replenish), 1), Util.invertColor(LightColors.LightSeaGreen), 0);
            spellList.Add(13, book);
            book = new Spell(14, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.PlayerSpecificSpells.rejuvinate), 1), Util.invertColor(LightColors.LightSkyBlue), 0);
            spellList.Add(14, book);
            book = new Spell(15, Vector2.Zero, new SpellFunctionDataNode(new Spell.spellFunction(Magic.MagicFunctions.PlayerSpecificSpells.revitalize), 1), Util.invertColor(LightColors.LightSteelBlue), 0);
            spellList.Add(15, book);


        }

        public static void fillWeatherDebrisList()
        {
            WeatherDebris w = new WeatherDebris(new Vector2((float)Game1.random.Next(0, Game1.graphics.GraphicsDevice.Viewport.Width), (float)Game1.random.Next(0, Game1.graphics.GraphicsDevice.Viewport.Height)), 0, (float)Game1.random.Next(15) / 500f, (float)Game1.random.Next(-10, 0) / 50f, (float)Game1.random.Next(10) / 50f);
            weatherDebrisDictionary.Add("Pink Flower Petal", w);
        }

        public static void fillSpriteFontList()
        {
            spriteFontList.Add("0", new TextureDataNode(Game1.content.Load<Texture2D>(Path.Combine("Revitalize", "Fonts", "colorlessSpriteFont", "vanilla", "0")), Path.Combine("Revitalize", "Fonts", "colorlessSpriteFont", "vanilla", "0")));

            spriteFontList.Add("leftArrow", new TextureDataNode(Game1.content.Load<Texture2D>(Path.Combine("Revitalize", "Fonts", "colorlessSpriteFont", "vanilla", "leftArrow")), Path.Combine("Revitalize", "Fonts", "colorlessSpriteFont", "vanilla", "leftArrow")));
            spriteFontList.Add("rightArrow", new TextureDataNode(Game1.content.Load<Texture2D>(Path.Combine("Revitalize", "Fonts", "colorlessSpriteFont", "vanilla", "rightArrow")), Path.Combine("Revitalize", "Fonts", "colorlessSpriteFont", "vanilla", "rightArrow")));
        }

    }
}
