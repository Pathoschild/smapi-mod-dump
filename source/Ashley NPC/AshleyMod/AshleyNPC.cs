using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AshleyMod
{
  public class AshleyNPC : NPC
  {
    public AshleyNPC()
      :base()
    {
      this.initializeAshley();
    }

    public AshleyNPC(AnimatedSprite sprite, Vector2 position, string defaultMap, int facingDir, string name, bool dateable, Dictionary<int, int[]> schedule, Texture2D portrait)
      : base(sprite, position, defaultMap, facingDir, name, dateable, schedule, portrait)
    {
      this.initializeAshley();
    }

    public AshleyNPC(AnimatedSprite sprite, Vector2 position, int facingDir, string name, ContentManager manager = null)
      :base(sprite, position, facingDir, name, manager)
    {
      this.initializeAshley();
    }

    private void initializeAshley()
    {
      this.age = 2; //child
      this.manners = 2; //rude
      this.socialAnxiety = 1; //shy
      this.optimism = 1; //negative
      this.gender = 1; //female
      this.datable = false; //not-datable
      this.homeRegion = 0; //Other
      this.birthday_Season = "fall"; //fall 8
      this.birthday_Day = 8;
    }

    public override void reloadSprite()
    {
      base.reloadSprite();
      this.sprite.spriteWidth = 24;
      this.sprite.spriteHeight = 32;
      this.faceDirection(this.defaultFacingDirection);
      this.sprite.standAndFaceDirection(this.defaultFacingDirection);
    }

    public override Rectangle GetBoundingBox()
    {
      if (this.sprite == null)
        return Rectangle.Empty;
      return new Rectangle((int)this.position.X + Game1.tileSize / 8, (int)this.position.Y + Game1.tileSize / 4, 16 * Game1.pixelZoom * 3 / 4, Game1.tileSize / 2);
    }

    public override Rectangle getMugShotSourceRect()
    {
      return new Rectangle(1, 4, 22, 24);
    }

    //WEIRD SHOP THING
    /*
    public override bool checkAction(Farmer who, GameLocation l)
    {
      if (base.checkAction(who, l))
        return true;
      if (l.currentEvent != null)
        return false;
      if (CurrentDialogue.Count == 0 && who.getFriendshipHeartLevelForNPC(this.name) >= 10)
      {
        var ashleyStock = new Dictionary<Item, int[]>();
        ashleyStock.Add(new ItemWario(), new int[2] { 450, int.MaxValue });
        var shopMenu = new ShopMenu(ashleyStock, 0, "Ashley");
        shopMenu.portraitPerson = this;
        shopMenu.potraitPersonDialogue = Game1.parseText("Have a look at my WarioWares.", Game1.dialogueFont, Game1.tileSize * 5 - Game1.pixelZoom * 4);
        Game1.activeClickableMenu = shopMenu;
        return true;
      }
      return false;
    }
    */
  }
}
