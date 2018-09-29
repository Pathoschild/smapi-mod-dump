using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley.Menus;
using CustomElementHandler;
using StardewValley.Locations;
using System.Collections.Generic;

namespace Stardewponics
{
    public class Aquaponics : Building, ISaveElement
    {
        /*********
        ** Properties
        *********/
        private GameLocation location;
        static int DaysToBuild = 4;
        private Vector2 ghLocation;


        /*********
		** Public methods
		*********/
        public Aquaponics() : base()
        {

        }

        public Aquaponics(Vector2 coords, BuildableGameLocation location) : base()
        {
            build(coords, location, DaysToBuild);

            if (coords != Vector2.Zero)
            {
                ghLocation = new Vector2(45, 45);
                //Routine's Code:
                //AquaponicsLocation apl = new AquaponicsLocation(AquaponicsMod.helper.Content.Load<Map>(@"assets\greenhouseMap.xnb", ContentSource.ModFolder), nameOfIndoors, (BuildableGameLocation)location);
                //indoors = apl;
            }
        }

        private void build(Vector2 coords, BuildableGameLocation loc, int days)
        {
            this.location = loc;
            tileX = (int)coords.X;
            tileY = (int)coords.Y;
            tilesWide = 14;
            tilesHigh = 7;
            humanDoor = new Point(-1, -1);
            animalDoor = new Point(-1, -1);
            texture = StardewponicsMod.helper.Content.Load<Texture2D>(@"assets\greenhouse.xnb", ContentSource.ModFolder);
            buildingType = "Aquaponics";
            baseNameOfIndoors = "Greenhouse";
            nameOfIndoorsWithoutUnique = baseNameOfIndoors;
            nameOfIndoors = baseNameOfIndoors;
            maxOccupants = -1;
            magical = false;
            daysOfConstructionLeft = days;
            owner = Game1.player.uniqueMultiplayerID;
        }

        public override bool intersects(Rectangle boundingBox)
        {
            if (!base.intersects(boundingBox))
                return false;
            if (boundingBox.X >= (this.tileX + 4) * Game1.tileSize && boundingBox.Right < (this.tileX + 7) * Game1.tileSize)
                return boundingBox.Y <= (this.tileY + 1) * Game1.tileSize;
            return true;
        }


        public override void draw(SpriteBatch b)
        {
            if (this.daysOfConstructionLeft > 0)
            {
                this.drawInConstruction(b);
            }
            else
            {
                this.drawShadow(b, -1, -1);
                b.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.tileX * Game1.tileSize), (float)(this.tileY * Game1.tileSize + this.tilesHigh * Game1.tileSize))), new Rectangle?(this.texture.Bounds), this.color * this.alpha, 0.0f, new Vector2(0.0f, (float)this.texture.Bounds.Height), 4f, SpriteEffects.None, (float)((this.tileY + this.tilesHigh - 2) * Game1.tileSize) / 10000f);
            }
        }

        public override void drawInMenu(SpriteBatch b, int x, int y)
        {
            CarpenterMenu menu = Game1.activeClickableMenu as CarpenterMenu;
            float texScale = 2;
            int num1 = (menu.maxWidthOfBuildingViewer - (int)(texture.Width * texScale)) / 2;
            num1 -= (int)(texture.Width / 3.5);
            int num2 = (menu.maxHeightOfBuildingViewer - (int)(texture.Height * texScale)) / 2;
            this.drawShadow(b, num1, num2);
            b.Draw(this.texture, new Rectangle(Game1.activeClickableMenu.xPositionOnScreen + num1, Game1.activeClickableMenu.yPositionOnScreen + num2, (int)(texture.Width * texScale), (int)(texture.Height * texScale)), Color.White);
        }

        public override void drawShadow(SpriteBatch b, int localX = -1, int localY = -1)
        {
            Vector2 position = localX == -1 ? Game1.GlobalToLocal(new Vector2((float)(this.tileX * Game1.tileSize), (float)((this.tileY + this.tilesHigh) * Game1.tileSize))) : new Vector2((float)localX, (float)(localY + this.getSourceRectForMenu().Height * Game1.pixelZoom));
            b.Draw(Game1.mouseCursors, position, new Rectangle?(Building.leftShadow), Color.White * (localX == -1 ? this.alpha : 1f), 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1E-05f);
            for (int index = 1; index < this.tilesWide - 1; ++index)
                b.Draw(Game1.mouseCursors, position + new Vector2((float)(index * Game1.tileSize), 0.0f), new Rectangle?(Building.middleShadow), Color.White * (localX == -1 ? this.alpha : 1f), 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1E-05f);
            b.Draw(Game1.mouseCursors, position + new Vector2((float)((this.tilesWide - 1) * Game1.tileSize), 0.0f), new Rectangle?(Building.rightShadow), Color.White * (localX == -1 ? this.alpha : 1f), 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 1E-05f);
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("name", buildingType);
            savedata.Add("location", location.name);
            savedata.Add("ghlocationx", ghLocation.X.ToString());
            savedata.Add("ghlocationy", ghLocation.Y.ToString());
            return savedata;
        }

        public object getReplacement()
        {
            Building building = new Building(new BluePrint("Shed"), new Vector2(tileX, tileY));
            building.daysOfConstructionLeft = daysOfConstructionLeft;
            building.tilesHigh = tilesHigh;
            building.tilesWide = tilesWide;
            building.indoors = indoors;
            building.tileX = tileX;
            building.tileY = tileY;
            return building;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Building building = (Building)replacement;
            indoors = building.indoors;
            Vector2 p = new Vector2(building.tileX, building.tileY);
            BuildableGameLocation l = (BuildableGameLocation)Game1.getLocationFromName(additionalSaveData["location"]);
            build(p, l, building.daysOfConstructionLeft);
        }


        //public override Rectangle getSourceRectForMenu()
        //{
        //    return new Rectangle(0, 0, this.texture.Bounds.Width - 1, this.texture.Bounds.Height);
        //}
    }
}