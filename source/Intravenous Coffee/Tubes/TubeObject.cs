using System;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using System.Collections.Generic;
using StardewValley.Objects;
using PyTK.Extensions;

namespace Tubes
{
    // Static info for our Tube object / terrain feature.
    public class TubeInfo
    {
        public const string fullid = "Pneumatic Tube";
        public const string name = "Pneumatic Tube";
        public const string category = "Crafting";
        public const int price = 100;
        public const string description = "Connects machines together with the magic of vacuums.";
        public const string crafting = "337 1";
        public const int spriteSize = 48;

        internal static Texture2D icon;
        internal static Texture2D terrainSprites;
        internal static CustomObjectData objectData;
        internal static CustomObjectData junkObjectData;

        internal static void init()
        {
            icon = TubesMod._helper.Content.Load<Texture2D>(@"Assets/icon.png");
            terrainSprites = TubesMod._helper.Content.Load<Texture2D>(@"Assets/terrain.png");
            objectData = new CustomObjectData(
                TubeInfo.fullid,
                $"{TubeInfo.name}/{TubeInfo.price}/-300/{TubeInfo.category} -24/{TubeInfo.name}/{TubeInfo.description}",
                TubeInfo.icon,
                Color.White,
                0,
                false,
                typeof(TubeObject),
                new CraftingData(TubeInfo.fullid, TubeInfo.crafting));

            junkObjectData = new CustomObjectData(
                $"{TubeInfo.fullid} junk",
                $"{TubeInfo.name} junk/{TubeInfo.price}/-300/{TubeInfo.category} -24/{TubeInfo.name}/{TubeInfo.description}",
                TubeInfo.icon,
                Color.White,
                0,
                false,
                null, // typeof(TubeObject),
                null);
        }
    }

    // The Tube object type. This is used whenever the object is not placed on the ground (it's not a terrain feature).
    public class TubeObject : StardewValley.Object, ICustomObject, ISaveElement, IDrawFromCustomObjectData
    {
        public CustomObjectData data { get => TubeInfo.objectData; }

        public TubeObject()
        {
        }

        public TubeObject(CustomObjectData data)
            : base(data.sdvId, 1)
        {
        }

        public TubeObject(CustomObjectData data, Vector2 tileLocation)
            : base(tileLocation, data.sdvId)
        {
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            return new Dictionary<string, string>() { { "name", name }, { "price", price.ToString() }, { "stack", stack.ToString() } };
        }

        public object getReplacement()
        {
            return new Chest(true) { playerChoiceColor = Color.Magenta };
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            name = additionalSaveData["name"];
            price = additionalSaveData["price"].toInt();
            stack = additionalSaveData["stack"].toInt();
        }

        public Item getOne()
        {
            return new TubeObject(TubeInfo.objectData) { tileLocation = Vector2.Zero, name = name, price = price };
        }

        public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            return new TubeObject(TubeInfo.objectData);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            spriteBatch.Draw(Game1.shadowTexture, location + new Vector2((Game1.tileSize / 2), (Game1.tileSize * 3 / 4)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * 0.5f, 0.0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
            spriteBatch.Draw(TubeInfo.icon, location + new Vector2((Game1.tileSize / 2), (Game1.tileSize / 2)), new Rectangle?(TubeInfo.objectData.sourceRectangle), Color.White * transparency, 0.0f, new Vector2(16 / 2, 16 / 2), Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);

            if (drawStackNumber && maximumStackSize() > 1 && (scaleSize > 0.3 && Stack != int.MaxValue) && Stack > 1)
                Utility.drawTinyDigits(stack, spriteBatch, location + new Vector2((Game1.tileSize - Utility.getWidthOfTinyDigitString(stack, 3f * scaleSize)) + 3f * scaleSize, (float)(Game1.tileSize - 18.0 * scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, StardewValley.Farmer f)
        {
            spriteBatch.Draw(TubeInfo.icon, objectPosition, TubeInfo.objectData.sourceRectangle, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0.0f, (f.getStandingY() + 2) / 10000f));
        }
    }
}
