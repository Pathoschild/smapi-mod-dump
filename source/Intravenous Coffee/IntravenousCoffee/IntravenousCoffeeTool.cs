using System.Collections.Generic;
using System.Linq;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using PyTK.CustomElementHandler;
using StardewModdingAPI;

namespace IntravenousCoffee
{
    public class IntravenousCoffeeTool : Tool, ISaveElement
    {
        internal static Texture2D texture;
        private static Texture2D attTexture;

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("name", name);
            return savedata;
        }

        public dynamic getReplacement()
        {
            Chest replacement = new Chest(true);
            if (attachments.Count() > 0) {
                if (attachments[0] != null)
                    replacement.addItem(attachments[0]);
            }

            return replacement;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            build();
            Chest chest = (Chest)replacement;
            if (!chest.isEmpty()) {
                attachments[0] = (SObject)chest.items[0];
            }
        }

        public IntravenousCoffeeTool() : base()
        {
            build();
        }

        public override string Name
        {
            get => name;
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        public override bool actionWhenPurchased()
        {
            return true;
        }

        public override Item getOne()
        {
            return new IntravenousCoffeeTool();
        }

        internal static void loadTextures()
        {
            texture = IntravenousCoffeeMod._helper.Content.Load<Texture2D>(@"Assets/ivbag.png");
            attTexture = IntravenousCoffeeMod._helper.Content.Load<Texture2D>(@"Assets/attachment.png");
        }

        private void build()
        {
            if (texture == null)
                loadTextures();

            name = "IV bag";
            description = "Fill it with coffee to constantly inject that sweet caffeine directly into your veins. Be careful, it's addictive!";

            numAttachmentSlots = 1;
            attachments = new SObject[numAttachmentSlots];
            initialParentTileIndex = 99;
            currentParentTileIndex = 99;
            indexOfMenuItemView = 0;
            upgradeLevel = 5;
            instantUse = false;
        }

        public bool hasCoffee()
        {
            return this.attachments[0]?.stack > 0;
        }

        public void consumeCoffee()
        {
            if (--this.attachments[0].stack == 0)
                this.attachments[0] = null;
        }

        public override int attachmentSlots()
        {
            return numAttachmentSlots;
        }
        
        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            spriteBatch.Draw(texture, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(Game1.getSquareSourceRectForNonStandardTileSheet(texture, Game1.tileSize / 4, Game1.tileSize / 4, this.indexOfMenuItemView)), Color.White * transparency, 0f, new Vector2((float)(Game1.tileSize / 4 / 2), (float)(Game1.tileSize / 4 / 2)), (float)Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {
            Rectangle attachementSourceRectangle = new Rectangle(0, 0, 64, 64);
            b.Draw(attTexture, new Vector2(x, y), new Rectangle?(attachementSourceRectangle), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);

            if (attachments.Count() > 0)
            {
                if (attachments[0] is SObject)
                    attachments[0].drawInMenu(b, new Vector2(x, y), 1f);
            }
        }

        public override bool onRelease(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            return false;
        }

        public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            return false;
        }

        public override bool canThisBeAttached(SObject o)
        {
            return o == null || o.DisplayName == "Coffee";
        }

        public override SObject attach(SObject o)
        {
            SObject priorAttachment = null;

            if (attachments[0] != null)
                priorAttachment = new SObject(Vector2.Zero, attachments[0].parentSheetIndex, attachments[0].stack);

            if (o == null) {
                if (attachments[0] != null) {
                    priorAttachment = new SObject(Vector2.Zero, attachments[0].parentSheetIndex, attachments[0].stack);
                    attachments[0] = null;
                }

                Game1.playSound("dwop");
                return priorAttachment;
            }

            if (canThisBeAttached(o)) {
                if (attachments[0] != null) {
                    attachments[0].stack += o.stack;
                } else {
                    attachments[0] = o;
                }
                Game1.playSound("button1");
                return null;
            }
 
            return null;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            return;
        }
       
        public override string getDescription()
        {
            int width = Game1.tileSize * 4 + Game1.tileSize / 4;
            return Game1.parseText(description, Game1.smallFont, width);
        }

        protected override string loadDisplayName()
        {
            return name;
        }

        protected override string loadDescription()
        {
            return getDescription();
        }
    }
}
