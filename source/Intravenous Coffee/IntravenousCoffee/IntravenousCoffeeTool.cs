/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mpcomplete/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Netcode;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using PyTK.CustomElementHandler;
using StardewModdingAPI;

namespace IntravenousCoffee
{
  public class IntravenousCoffeeTool : Tool, ISaveElement, ICustomObject
  {
    internal static Texture2D texture;
    private static Texture2D attTexture;

    public Dictionary<string, string> getAdditionalSaveData() {
      Dictionary<string, string> savedata = new Dictionary<string, string>();
      savedata.Add("name", Name);
      return savedata;
    }

    public dynamic getReplacement() {
      Chest replacement = new Chest(true);
      if (attachments.Count() > 0) {
        if (attachments[0] != null)
          replacement.addItem(attachments[0]);
      }

      return replacement;
    }

    public void rebuild(Dictionary<string, string> additionalSaveData, object replacement) {
      build();
      Chest chest = (Chest)replacement;
      if (!chest.isEmpty()) {
        attachments[0] = (SObject)chest.items[0];
      }
    }

    public IntravenousCoffeeTool() : base() {
      build();
    }

    public override bool canBeTrashed() {
      return true;
    }

    //public override bool actionWhenPurchased() {
    //  return false;  // use default action
    //}

    public override Item getOne() {
      return new IntravenousCoffeeTool();
    }

    public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement) {
      return new IntravenousCoffeeTool();
    }

    internal static void loadTextures() {
      texture = IntravenousCoffeeMod._helper.Content.Load<Texture2D>(@"Assets/ivbag.png");
      attTexture = IntravenousCoffeeMod._helper.Content.Load<Texture2D>(@"Assets/attachment.png");
    }

    private void build() {
      if (texture == null)
        loadTextures();

      Name = "IV bag";
      description = "Fill it with coffee to constantly inject that sweet caffeine directly into your veins. Be careful, it's addictive!";

      numAttachmentSlots.Value = 1;
      attachments.SetCount(numAttachmentSlots);
      InitialParentTileIndex = 99;
      CurrentParentTileIndex = 99;
      IndexOfMenuItemView = 0;
      UpgradeLevel = 5;
      InstantUse = false;
    }

    public bool hasCoffee() {
      return this.attachments[0]?.Stack > 0;
    }

    public void consumeCoffee() {
      if (--this.attachments[0].Stack == 0)
        this.attachments[0] = null;
    }

    public override int attachmentSlots() {
      return numAttachmentSlots;
    }

    public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow) {
      spriteBatch.Draw(
        texture,
        location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)),
        new Rectangle(0, 0, 16, 16),
        Color.White * transparency, 0f,
        new Vector2((float)(Game1.tileSize / 4 / 2), (float)(Game1.tileSize / 4 / 2)),
        (float)Game1.pixelZoom * scaleSize, SpriteEffects.None, layerDepth);
    }

    public override void drawAttachments(SpriteBatch b, int x, int y) {
      b.Draw(attTexture, new Vector2(x, y), new Rectangle(0, 0, 64, 64), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);

      if (attachments.Count() > 0) {
        if (attachments[0] is SObject)
          attachments[0].drawInMenu(b, new Vector2(x, y), 1f);
      }
    }

    public override bool onRelease(GameLocation location, int x, int y, StardewValley.Farmer who) {
      return false;
    }

    public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who) {
      return false;
    }

    public override bool canThisBeAttached(SObject o) {
      return o == null || o.DisplayName == "Coffee";
    }

    public override SObject attach(SObject o) {
      SObject priorAttachment = null;

      if (attachments[0] != null)
        priorAttachment = new SObject(Vector2.Zero, attachments[0].ParentSheetIndex, attachments[0].Stack);

      if (o == null) {
        if (attachments[0] != null) {
          priorAttachment = new SObject(Vector2.Zero, attachments[0].ParentSheetIndex, attachments[0].Stack);
          attachments[0] = null;
        }

        Game1.playSound("dwop");
        return priorAttachment;
      }

      if (canThisBeAttached(o)) {
        if (attachments[0] != null) {
          attachments[0].Stack += o.Stack;
        } else {
          attachments[0] = o;
        }
        Game1.playSound("button1");
        return null;
      }

      return null;
    }

    public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who) {
      return;
    }

    public override string getDescription() {
      int width = Game1.tileSize * 4 + Game1.tileSize / 4;
      return Game1.parseText(description, Game1.smallFont, width);
    }

    protected override string loadDisplayName() {
      return Name;
    }

    protected override string loadDescription() {
      return getDescription();
    }
  }
}
