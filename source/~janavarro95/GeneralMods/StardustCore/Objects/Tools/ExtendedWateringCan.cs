using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardustCore.Interfaces;
using StardustCore.Objects.Tools.SerializationInformation;
using StardustCore.UIUtilities;

namespace StardustCore.Objects.Tools
{
    public class ExtendedWateringCan : StardewValley.Tools.WateringCan, IItemSerializeable, IToolSerializer
    {
        public Texture2DExtended texture;

        public override string DisplayName { get => this.displayName; set => this.displayName = value; }
        public override string Name { get => this.displayName; set => this.displayName = value; }

        /// <summary>
        /// Generates a default axe. Doens't really do much.
        /// </summary>
        public ExtendedWateringCan() : base()
        {
            this.texture = new Texture2DExtended(StardustCore.ModCore.ModHelper, ModCore.Manifest,Path.Combine("Content", "Graphics", "Tools", "CustomAxe.png"));
            this.waterCanMax = 30;
            this.WaterLeft = 0;
        }

        public ExtendedWateringCan(BasicToolInfo info, Texture2DExtended texture, int waterMax, int waterCurrent)
        {
            this.texture = texture;
            this.displayName = info.name;
            this.description = info.description;
            this.UpgradeLevel = info.level;
            this.waterCanMax = waterMax;
            this.WaterLeft = waterCurrent;
        }

        public ExtendedWateringCan(SerializedObjectBase dataBase) : base()
        {
            StardustCore.ModCore.ModMonitor.Log((dataBase as Serialization_ExtendedWateringCan).Name);
            this.displayName = "Hello";
            this.description = (dataBase as Serialization_ExtendedWateringCan).Description;
            this.texture = StardustCore.ModCore.TextureManager.getTexture((dataBase as Serialization_ExtendedWateringCan).TextureInformation.Name);
            this.UpgradeLevel = (dataBase as Serialization_ExtendedWateringCan).UpgradeLevel;
            this.waterCanMax= (dataBase as Serialization_ExtendedWateringCan).MaxCapacity;
            this.WaterLeft= (dataBase as Serialization_ExtendedWateringCan).WaterLeft;
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color color, bool drawShadow)
        {
            spriteBatch.Draw(texture.getTexture(), location + new Vector2(32f, 32f), new Rectangle(0, 0, 16, 16), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
            if (!drawStackNumber || Game1.player.hasWateringCanEnchantment)
                return;
            spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(4f, 44f), new Rectangle?(new Rectangle(297, 420, 14, 5)), Color.White * transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 0.0001f);
            spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)location.X + 8, (int)location.Y + 64 - 16, (int)((double)this.WaterLeft / (double)this.waterCanMax * 48.0), 8), Color.DodgerBlue * 0.7f * transparency);
        }

        public override bool canBeDropped()
        {
            return true;
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        public Type getCustomType()
        {
            return this.GetType();
        }

        public string GetSerializationName()
        {
            return this.GetType().ToString();
        }

        public override int maximumStackSize()
        {
            return 1;
        }

        public override void setNewTileIndexForUpgradeLevel()
        {
            //Do nothing.
        }

        public void upgradeWateringCapacity(int amount)
        {
            this.waterCanMax += amount;
        }

        /// <summary>
        /// Serializes the said item properly.
        /// </summary>
        /// <param name="I"></param>
        public static void Serialize(Item I)
        {
            SerializationInformation.Serialization_ExtendedWateringCan tool = new SerializationInformation.Serialization_ExtendedWateringCan((I as ExtendedWateringCan));
            String savePath = ModCore.SerializationManager.playerInventoryPath;
            String fileName = I.Name + ".json";
            String resultPath = Path.Combine(savePath, fileName);
            int count = 0;
            while (File.Exists(resultPath))
            {
                resultPath = Serialization.SerializationManager.getValidSavePathIfDuplicatesExist(I, savePath, count);
                count++;
            }
            StardustCore.ModCore.ModHelper.WriteJsonFile<SerializedObjectBase>(resultPath, tool);
        }

        /// <summary>
        /// Serializes the said item to a chest.
        /// </summary>
        /// <param name="I"></param>
        /// <param name="s"></param>
        public static void SerializeToContainer(Item I, string s)
        {
            SerializationInformation.Serialization_ExtendedWateringCan tool = new SerializationInformation.Serialization_ExtendedWateringCan((I as ExtendedWateringCan));
            String savePath = s;
            String fileName = I.Name + ".json";
            String resultPath = Path.Combine(savePath, fileName);
            int count = 0;
            while (File.Exists(resultPath))
            {
                resultPath = Serialization.SerializationManager.getValidSavePathIfDuplicatesExist(I, savePath, count);
                count++;
            }
            StardustCore.ModCore.ModHelper.WriteJsonFile<SerializedObjectBase>(resultPath, tool);
        }

        /// <summary>
        /// Deserializes the object from a .json.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static ExtendedWateringCan Deserialize(string data)
        {
            SerializationInformation.Serialization_ExtendedWateringCan toolData = ModCore.ModHelper.ReadJsonFile< SerializationInformation.Serialization_ExtendedWateringCan>(data);
            return new ExtendedWateringCan(toolData);
        }


    }
}
