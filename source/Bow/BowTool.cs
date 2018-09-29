using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bow
{
    class BowTool : Tool, ISaveElement
    {
        public static Texture2D Texture { get; set; }

        private static Dictionary<Farmer, bool> usingBow = new Dictionary<Farmer, bool>();

        [XmlIgnore]
        public readonly NetPoint AimingAt = new NetPoint().Interpolated(true, true);

        [XmlIgnore]
        public readonly NetInt UseTime = new NetInt();

        public BowTool()
        {
            BaseName = "Bow";
            numAttachmentSlots.Value = 1;
            this.attachments.SetCount(1);
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddField(AimingAt);
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        public override Item getOne()
        {
            return new BowTool();
        }

        protected override string loadDisplayName()
        {
            return "Bow";
        }
        protected override string loadDescription()
        {
            return "It shoots arrows.";
        }

        public override bool doesShowTileLocationMarker()
        {
            return false;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
        }

        public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
        {
            usingBow[who] = true;
            who.currentLocation.playSound("slingshot");
            UseTime.Value = 0;
            return true;
        }

        public override bool onRelease(GameLocation location, int x, int y, Farmer who)
        {
            usingBow[who] = false;

            if (this.attachments[0] != null)
            {
                StardewValley.Object one = (StardewValley.Object)this.attachments[0].getOne();
                --this.attachments[0].Stack;
                if (this.attachments[0].Stack <= 0)
                    this.attachments[0] = (StardewValley.Object)null;
                int x1 = this.AimingAt.X;
                int y1 = this.AimingAt.Y;
                //int num1 = Math.Min(20, (int)Vector2.Distance(new Vector2((float)who.getStandingX(), (float)(who.getStandingY() - 64)), new Vector2((float)x1, (float)y1)) / 20);
                Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(new Point(who.getStandingX(), who.getStandingY() + 64), new Vector2((float)x1, (float)(y1 + 64)), (float)(15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));
                //int num2 = 4;
                int heldLen = UseTime.Value;
                velocityTowardPoint *= heldLen / 100f;

                //if (num1 > num2 && !this.canPlaySound)
                {
                    int num3 = 1;
                    BasicProjectile.onCollisionBehavior collisionBehavior = (BasicProjectile.onCollisionBehavior)null;
                    string collisionSound = "hammer";
                    float num4 = 2;
                    switch (one.ParentSheetIndex)
                    {
                        case 378:
                            num3 = 10;
                            ++one.ParentSheetIndex;
                            break;
                        case 380:
                            num3 = 20;
                            ++one.ParentSheetIndex;
                            break;
                        case 382:
                            num3 = 15;
                            ++one.ParentSheetIndex;
                            break;
                        case 384:
                            num3 = 30;
                            ++one.ParentSheetIndex;
                            break;
                        case 386:
                            num3 = 50;
                            ++one.ParentSheetIndex;
                            break;
                        case 388:
                            num3 = 2;
                            ++one.ParentSheetIndex;
                            break;
                        case 390:
                            num3 = 5;
                            ++one.ParentSheetIndex;
                            break;
                        case 441:
                            num3 = 20;
                            collisionBehavior = new BasicProjectile.onCollisionBehavior(BasicProjectile.explodeOnImpact);
                            collisionSound = "explosion";
                            break;
                    }
                    if (one.Category == -5)
                        collisionSound = "slimedead";
                    NetCollection<Projectile> projectiles = location.projectiles;
                    BasicProjectile basicProjectile = new BasicProjectile((int)((double)num4 * (double)(num3 + Game1.random.Next(-(num3 / 2), num3 + 2)) * (1.0 + (double)who.attackIncreaseModifier)), one.ParentSheetIndex, 0, 0, (float)(Math.PI / (64.0 + (double)Game1.random.Next(-63, 64))), velocityTowardPoint.X, velocityTowardPoint.Y, new Vector2((float)(who.getStandingX() - 16), (float)(who.getStandingY() - 64 - 8)), collisionSound, "", false, true, location, (Character)who, true, collisionBehavior);
                    basicProjectile.IgnoreLocationCollision = Game1.currentLocation.currentEvent != null;
                    projectiles.Add((Projectile)basicProjectile);
                }
            }
            else
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
            return true;
        }

        public override void tickUpdate(GameTime time, Farmer who)
        {
            lastUser = who;
            if (!usingBow.ContainsKey(who) || !usingBow[who])
                return;

            if (who.IsLocalPlayer)
            {
                ++UseTime.Value;
                AimingAt.X = Game1.getMousePosition().X + Game1.viewport.X;
                AimingAt.Y = Game1.getMousePosition().Y + Game1.viewport.Y;
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.mouseCursors, new Vector2( AimingAt.X - Game1.viewport.X, AimingAt.Y - Game1.viewport.Y ), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 43, -1, -1)), Color.White, 0, new Vector2( 32, 32 ), 1, SpriteEffects.None, 1);
        }

        public override void drawInMenu(SpriteBatch b, Vector2 loc, float scale, float transparency, float depth, bool drawStackNumber, Color color, bool shadow)
        {
            b.Draw(Texture, loc + new Vector2(32, 29), null, Color.White * transparency, 0, new Vector2(8, 8), scale * 4, SpriteEffects.None, depth);
            if (!drawStackNumber || this.attachments == null || this.attachments[0] == null)
                return;
            Utility.drawTinyDigits(this.attachments[0].Stack, b, loc + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(this.attachments[0].Stack, 3f * scale)) + 3f * scale, (float)(64.0 - 18.0 * (double)scale + 2.0)), 3f * scale, 1f, Color.White);
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {
            if (this.attachments[0] == null)
            {
                b.Draw(Game1.menuTexture, new Vector2((float)x, (float)y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 43, -1, -1)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
            }
            else
            {
                b.Draw(Game1.menuTexture, new Vector2((float)x, (float)y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
                this.attachments[0].drawInMenu(b, new Vector2((float)x, (float)y), 1f);
            }
        }

        public object getReplacement()
        {
            var slingshot = new Slingshot();
            slingshot.attachments[0] = this.attachments[0];
            return slingshot;
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            return new Dictionary<string, string>();
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            this.attachments[ 0 ] = (replacement as Slingshot).attachments[0];
        }
    }
}
