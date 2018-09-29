
using CustomNPCFramework.Framework.Enums;
using CustomNPCFramework.Framework.ModularNPCS;
using CustomNPCFramework.Framework.ModularNPCS.CharacterAnimationBases;
using CustomNPCFramework.Framework.ModularNPCS.ColorCollections;
using CustomNPCFramework.Framework.ModularNPCS.ModularRenderers;
using CustomNPCFramework.Framework.NPCS;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.Graphics
{

    /// <summary>
    /// Used to hold a collection of strings.
    /// </summary>
    public class NamePairings
    {
        public string leftString;
        public string rightString;
        public string upString;
        public string downString;
        public NamePairings(string LeftString,string RightString, string UpString, string DownString)
        {
            this.leftString = LeftString;
            this.rightString = RightString;
            this.upString = UpString;
            this.downString = DownString;
        }
    }

    /// <summary>
    /// Used to contain all of the asset managers.
    /// </summary>
    public class AssetPool
    {

        /// <summary>
        /// A dictionary holding all of the asset managers. (Name, AssetManager)
        /// </summary>
        public Dictionary<string, AssetManager> assetPool;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AssetPool()
        {
            this.assetPool = new Dictionary<string, AssetManager>();
        }

        /// <summary>
        /// Adds an asset manager to the asset pool.
        /// </summary>
        /// <param name="pair">A key value pair with the convention being (Manager Name, Asset Manager)</param>
        public void addAssetManager(KeyValuePair<string, AssetManager> pair)
        {
            this.assetPool.Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Adds an asset manager to the asset pool.
        /// </summary>
        /// <param name="assetManagerName">The name of the asset manager to be added.</param>
        /// <param name="assetManager">The asset manager object to be added to the asset pool.</param>
        public void addAssetManager(string assetManagerName, AssetManager assetManager)
        {
            this.assetPool.Add(assetManagerName, assetManager);
        }

        /// <summary>
        /// Get an asset manager from the asset pool from a name.
        /// </summary>
        /// <param name="name">The name of the asset manager to return.</param>
        /// <returns></returns>
        public AssetManager getAssetManager(string name)
        {
            assetPool.TryGetValue(name, out AssetManager asset);
            return asset;
        }

        /// <summary>
        /// Remove an asset manager from the asset pool.
        /// </summary>
        /// <param name="key">The name of the asset manager to remove.</param>
        public void removeAssetManager(string key)
        {
            assetPool.Remove(key);
        }

        /// <summary>
        /// Go through all of the asset managers and load assets according to their respective paths.
        /// </summary>
        public void loadAllAssets()
        {
            foreach (KeyValuePair<string, AssetManager> assetManager in this.assetPool)
            {
                assetManager.Value.loadAssets();
            }
        }

        /// <summary>
        /// Creates an extended animated sprite object given the asset name in the asset manager.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AnimatedSpriteExtended getAnimatedSpriteFromAsset(string name)
        {
            assetPool.TryGetValue(name, out AssetManager asset);
            var assetSheet = asset.getAssetByName(name);
            return new AnimatedSpriteExtended(assetSheet.path.Clone().ToString(),assetSheet.index, (int)assetSheet.assetInfo.assetSize.X, (int)assetSheet.assetInfo.assetSize.Y);
        }


        /// <summary>
        /// Generates a new AnimatedSpriteCollection object from the data held in an asset sheet.
        /// </summary>
        /// <param name="assetSheet">An asset sheet that holds the data for textures.</param>
        /// <param name="type">The type of asset to get from the sheet. Hair, eyes, shoes, etc.</param>
        /// <returns></returns>
        public AnimatedSpriteCollection getSpriteCollectionFromSheet(AssetSheet assetSheet, AnimationType type)
        {    
                var left = new AnimatedSpriteExtended(assetSheet.clone().getTexture(Direction.left, type),assetSheet);
                var right = new AnimatedSpriteExtended(assetSheet.clone().getTexture(Direction.right, type), assetSheet);
                var up = new AnimatedSpriteExtended(assetSheet.clone().getTexture(Direction.up, type), assetSheet);
                var down = new AnimatedSpriteExtended(assetSheet.clone().getTexture(Direction.down, type), assetSheet);
                return new AnimatedSpriteCollection(left, right, up, down, Direction.down); 
        }
        

        /// <summary>
        /// Gets an animated sprite collection (ie a hair style facing all four directions) from a list of asset names.
        /// </summary>
        /// <param name="left">The name of the asset for the left facing sprite.</param>
        /// <param name="right">The name of the asset for the right facing sprite.</param>
        /// <param name="up">The name of the asset for the up facing sprite.</param>
        /// <param name="down">The name of the asset for the down facing sprite.</param>
        /// <param name="startingDirection"></param>
        /// <returns></returns>
        public AnimatedSpriteCollection getAnimatedSpriteCollectionFromAssets(string left, string right, string up, string down, Direction startingDirection = Direction.down)
        {
            var Left = getAnimatedSpriteFromAsset(left);
            var Right = getAnimatedSpriteFromAsset(right);
            var Up = getAnimatedSpriteFromAsset(up);
            var Down = getAnimatedSpriteFromAsset(down);
            return new AnimatedSpriteCollection(Left, Right, Up, Down, startingDirection);

        }

        /// <summary>
        /// Get an AnimatedSpriteCollection from a name pairing.
        /// </summary>
        /// <param name="pair">A collection of strings that hold information on directional textures.</param>
        /// <param name="startingDirection">The direction in which the sprite should face.</param>
        /// <returns></returns>
        public AnimatedSpriteCollection getAnimatedSpriteCollectionFromAssets(NamePairings pair, Direction startingDirection = Direction.down)
        {
            return getAnimatedSpriteCollectionFromAssets(pair.leftString, pair.rightString, pair.upString, pair.downString);
        }

        /// <summary>
        /// Get a collection of sprites to generate a collective animated sprite.
        /// </summary>
        /// <param name="BodySprites">The collection of sprites to be used for the boyd of the npc.</param>
        /// <param name="EyeSprites">The collection of sprites to be used for the eye of the npc.</param>
        /// <param name="HairSprites">The collection of sprites to be used for the hair of the npc.</param>
        /// <param name="ShirtsSprites">The collection of sprites to be used for the shirts of the npc.</param>
        /// <param name="PantsSprites">The collection of sprites to be used for the pants of the npc.</param>
        /// <param name="ShoesSprites">The collection of sprites to be used for the shoes of the npc.</param>
        /// <param name="AccessoriesSprites">The collection of sprites to be used for the accessories of the npc.</param>
        /// <param name="DrawColors">The collection of collors to be used for chaing the color of an individual asset.</param>
        /// <returns></returns>
        public StandardCharacterAnimation GetStandardCharacterAnimation(NamePairings BodySprites, NamePairings EyeSprites, NamePairings HairSprites, NamePairings ShirtsSprites, NamePairings PantsSprites, NamePairings ShoesSprites,List<NamePairings> AccessoriesSprites,StandardColorCollection DrawColors=null)
        {
            var body = getAnimatedSpriteCollectionFromAssets(BodySprites);
            var eyes = getAnimatedSpriteCollectionFromAssets(EyeSprites);
            var hair = getAnimatedSpriteCollectionFromAssets(HairSprites);
            var shirts = getAnimatedSpriteCollectionFromAssets(ShirtsSprites);
            var pants = getAnimatedSpriteCollectionFromAssets(PantsSprites);
            var shoes = getAnimatedSpriteCollectionFromAssets(ShoesSprites);
            List<AnimatedSpriteCollection> accessories = new List<AnimatedSpriteCollection>();
            foreach(var v in AccessoriesSprites)
            {
                accessories.Add(getAnimatedSpriteCollectionFromAssets(v));
            }
            if (DrawColors == null) DrawColors = new StandardColorCollection();
            return new StandardCharacterAnimation(body,eyes,hair,shirts,pants,shoes,accessories,DrawColors);
        }

        /// <summary>
        /// Get a list of parts that can apply for this criteria.
        /// </summary>
        /// <param name="assetManagerName">The name of the asset manager.</param>
        /// <param name="gender">The gender critera.</param>
        /// <param name="season">The season critera.</param>
        /// <param name="type">The part type critera.</param>
        /// <returns></returns>
        public List<AssetSheet> getListOfApplicableBodyParts(string assetManagerName,Genders gender, Seasons season, PartType type)
        {
            var parts = this.getAssetManager(assetManagerName).getListOfAssetsThatMatchThisCriteria(gender, season, type);
            return parts;
        }


        /// <summary>
        /// Generate a basic npc based off of all all of the NPC data here.
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="minNumOfAccessories"></param>
        /// <param name="maxNumOfAccessories"></param>
        public ExtendedNPC generateNPC(Genders gender, int minNumOfAccessories, int maxNumOfAccessories ,StandardColorCollection DrawColors=null)
        {
            Seasons myseason=Seasons.spring;

            if (Game1.currentSeason == "spring") myseason = Seasons.spring;
            if (Game1.currentSeason == "summer") myseason = Seasons.summer;
            if (Game1.currentSeason == "fall") myseason = Seasons.fall;
            if (Game1.currentSeason == "winter") myseason = Seasons.winter;

            List<AssetSheet> bodyList = new List<AssetSheet>();
            List<AssetSheet> eyesList = new List<AssetSheet>();
            List<AssetSheet> hairList = new List<AssetSheet>();
            List<AssetSheet> shirtList = new List<AssetSheet>();
            List<AssetSheet> shoesList = new List<AssetSheet>();
            List<AssetSheet> pantsList = new List<AssetSheet>();
            List<AssetSheet> accessoryList = new List<AssetSheet>();

            //Get all applicable parts from this current asset manager
            foreach (var assetManager in this.assetPool)
            {
                var body = getListOfApplicableBodyParts(assetManager.Key, gender, myseason, PartType.body);
                foreach (var piece in body) bodyList.Add(piece);

                var eyes = getListOfApplicableBodyParts(assetManager.Key, gender, myseason, PartType.eyes);
                foreach (var piece in eyes) eyesList.Add(piece);

                var hair = getListOfApplicableBodyParts(assetManager.Key, gender, myseason, PartType.hair);
                foreach (var piece in hair) hairList.Add(piece);

                var shirt = getListOfApplicableBodyParts(assetManager.Key, gender, myseason, PartType.shirt);
                foreach (var piece in shirt) shirtList.Add(piece);

                var pants = getListOfApplicableBodyParts(assetManager.Key, gender, myseason, PartType.pants);
                foreach (var piece in pants) pantsList.Add(piece);

                var shoes = getListOfApplicableBodyParts(assetManager.Key, gender, myseason, PartType.shoes);
                foreach (var piece in shoes) shoesList.Add(piece);

                var accessory = getListOfApplicableBodyParts(assetManager.Key, gender, myseason, PartType.accessory);
                foreach (var piece in accessory) accessoryList.Add(piece);
            }

            
            Random r = new Random(System.DateTime.Now.Millisecond);
            int amount = 0;
            
            amount = r.Next(minNumOfAccessories,maxNumOfAccessories + 1); //Necessary since r.next returns a num between min and (max-1)

            int bodyIndex = 0;
            int eyesIndex = 0;
            int hairIndex = 0;
            int shirtIndex = 0;
            int pantsIndex = 0;
            int shoesIndex = 0;

            if (bodyList.Count != 0) {
                bodyIndex = r.Next(0, bodyList.Count - 1);
            }
            else
            {
                Class1.ModMonitor.Log("Error: Not enough body templates to generate an npc. Aborting", StardewModdingAPI.LogLevel.Error);
                return null;
            }

            if (eyesList.Count != 0) {
                eyesIndex = r.Next(0, eyesList.Count - 1);
            }
            else
            {
                Class1.ModMonitor.Log("Error: Not enough eyes templates to generate an npc. Aborting", StardewModdingAPI.LogLevel.Error);
                return null;
            }

            if (hairList.Count != 0) {
                hairIndex = r.Next(0, hairList.Count - 1);
            }
            else
            {
                Class1.ModMonitor.Log("Error: Not enough hair templates to generate an npc. Aborting", StardewModdingAPI.LogLevel.Error);
                return null;
            }

            if (shirtList.Count != 0) {
                shirtIndex = r.Next(0, shirtList.Count - 1);
            }
            else
            {
                Class1.ModMonitor.Log("Error: Not enough shirt templates to generate an npc. Aborting", StardewModdingAPI.LogLevel.Error);
                return null;
            }

            if (pantsList.Count != 0) {
                pantsIndex = r.Next(0, pantsList.Count - 1);
            }
            else
            {
                Class1.ModMonitor.Log("Error: Not enough pants templates to generate an npc. Aborting", StardewModdingAPI.LogLevel.Error);
                return null;
            }

            if (shoesList.Count != 0) {
                shoesIndex = r.Next(0, shoesList.Count - 1);

            }
            else
            {
                Class1.ModMonitor.Log("Error: Not enough shoes templates to generate an npc. Aborting", StardewModdingAPI.LogLevel.Error);
                return null;
            }
            List<int> accIntList = new List<int>();
            if (accessoryList.Count != 0)
            {
                for (int i = 0; i < amount; i++)
                {
                    int acc = r.Next(0, accessoryList.Count - 1);
                    accIntList.Add(acc);
                }
            }

            //Get a single sheet to pull from.
            AssetSheet bodySheet;
            AssetSheet eyesSheet;
            AssetSheet hairSheet;
            AssetSheet shirtSheet;
            AssetSheet shoesSheet;
            AssetSheet pantsSheet;

            bodySheet = bodyList.ElementAt(bodyIndex);
            eyesSheet = eyesList.ElementAt(eyesIndex);
            hairSheet = hairList.ElementAt(hairIndex);
            shirtSheet = shirtList.ElementAt(shirtIndex);
            pantsSheet = pantsList.ElementAt(pantsIndex);
            shoesSheet = shoesList.ElementAt(shoesIndex);


            List<AssetSheet> accessorySheet = new List<AssetSheet>();

            foreach (var v in accIntList)
            {
                accessorySheet.Add(accessoryList.ElementAt(v));
            }
            if (DrawColors == null) DrawColors = new StandardColorCollection();
            var render = generateBasicRenderer(bodySheet, eyesSheet, hairSheet, shirtSheet, pantsSheet, shoesSheet, accessorySheet,DrawColors);
            ExtendedNPC npc = new ExtendedNPC(new Sprite(getDefaultSpriteImage(bodySheet)), render, new Microsoft.Xna.Framework.Vector2(0,0) * Game1.tileSize, 2, NPCNames.getRandomNPCName(gender));
            return npc;
    }

        /// <summary>
        /// Creates a character renderer (a collection of textures) from a bunch of different asset sheets.
        /// </summary>
        /// <param name="bodySheet">The textures for the npc's body.</param>
        /// <param name="eyesSheet">The textures for the npc's eyes.</param>
        /// <param name="hairSheet">The textures for the npc's hair.</param>
        /// <param name="shirtSheet">The textures for the npc's shirt.</param>
        /// <param name="pantsSheet">The textures for the npc's pants.</param>
        /// <param name="shoesSheet">The textures for the npc's shoes.</param>
        /// <param name="accessorySheet">The textures for the npc's accessories.</param>
        /// <param name="DrawColors">The colors for the npc's different assets.</param>
        /// <returns></returns>
        public virtual BasicRenderer generateBasicRenderer(AssetSheet bodySheet, AssetSheet eyesSheet, AssetSheet hairSheet, AssetSheet shirtSheet, AssetSheet pantsSheet, AssetSheet shoesSheet, List<AssetSheet> accessorySheet, StandardColorCollection DrawColors=null)
        {
            if (DrawColors == null) DrawColors = new StandardColorCollection();
            //Get all of the appropriate animations.
            AnimationType type = AnimationType.standing;
            var standingAnimation = generateCharacterAnimation(bodySheet, eyesSheet, hairSheet, shirtSheet, pantsSheet, shoesSheet, accessorySheet, type,DrawColors);
            type = AnimationType.walking;
            var movingAnimation = generateCharacterAnimation(bodySheet, eyesSheet, hairSheet, shirtSheet, pantsSheet, shoesSheet, accessorySheet, type,DrawColors);
            type = AnimationType.swimming;
            var swimmingAnimation = generateCharacterAnimation(bodySheet, eyesSheet, hairSheet, shirtSheet, pantsSheet, shoesSheet, accessorySheet, type,DrawColors);

            BasicRenderer render = new BasicRenderer(standingAnimation, movingAnimation, swimmingAnimation);
            return render;
        }

        /// <summary>
        /// Generate a Standard Character Animation from some asset sheets.
        /// (collection of textures to animations) 
        /// </summary>
        /// <param name="body">The textures for the npc's body.</param>
        /// <param name="eyes">The textures for the npc's eyes.</param>
        /// <param name="hair">The textures for the npc's hair.</param>
        /// <param name="shirt">The textures for the npc's shirt.</param>
        /// <param name="pants">The textures for the npc's pants.</param>
        /// <param name="shoes">The textures for the npc's shoes.</param>
        /// <param name="accessoryType">The textures for the npc's accessories.</param>
        /// <param name="DrawColors">The colors for the npc's different assets.</param>
        /// <returns></returns>
        public virtual StandardCharacterAnimation generateCharacterAnimation(AssetSheet body, AssetSheet eyes, AssetSheet hair, AssetSheet shirt, AssetSheet pants, AssetSheet shoes,List<AssetSheet> accessories, AnimationType animationType, StandardColorCollection DrawColors=null)
        {
            var bodySprite = getSpriteCollectionFromSheet(body, animationType);
            var eyesSprite = getSpriteCollectionFromSheet(eyes, animationType);
            var hairSprite = getSpriteCollectionFromSheet(hair, animationType);
            var shirtSprite = getSpriteCollectionFromSheet(shirt, animationType);
            var pantsSprite = getSpriteCollectionFromSheet(pants, animationType);
            var shoesSprite = getSpriteCollectionFromSheet(shoes, animationType);
            List<AnimatedSpriteCollection> accessoryCollection = new List<AnimatedSpriteCollection>();
            foreach (var v in accessories)
            {
                AnimatedSpriteCollection acc = getSpriteCollectionFromSheet(v, AnimationType.standing);
                accessoryCollection.Add(acc);
            }
            if (DrawColors == null) DrawColors = new StandardColorCollection();
            StandardCharacterAnimation standingAnimation = new StandardCharacterAnimation(bodySprite, eyesSprite, hairSprite, shirtSprite, pantsSprite, shoesSprite, accessoryCollection,DrawColors);
            return standingAnimation;
        }

        /// <summary>
        /// Get the string for the standard character sprite to be used from this asset sheet.
        /// </summary>
        /// <param name="imageGraphics">The standard asset sheet to be used.</param>
        /// <returns></returns>
        public virtual string getDefaultSpriteImage(AssetSheet imageGraphics)
        {
            return Class1.getRelativeDirectory(Path.Combine(imageGraphics.path, imageGraphics.assetInfo.standingAssetPaths.downString));
        }
    }
}
