/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JustCylon/stardew-brewery
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Buildings;
using xTile;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Locations;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;
using Entoarox.Framework;

namespace StardewBrewery
{
    public class BreweryBuilding : Building, ICustomItem
    {
        public override bool doAction(Vector2 tileLocation, StardewValley.Farmer who)
        {
            if (who.IsMainPlayer && (double)tileLocation.X >= (double)this.tileX && ((double)tileLocation.X < (double)(this.tileX + this.tilesWide) && (double)tileLocation.Y >= (double)this.tileY) && ((double)tileLocation.Y < (double)(this.tileY + this.tilesHigh) && this.daysOfConstructionLeft > 0))
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:UnderConstruction"));
            }
            else
            {
                if (who.IsMainPlayer && (double)tileLocation.X == (double)(this.humanDoor.X + this.tileX) && ((double)tileLocation.Y == (double)(this.humanDoor.Y + this.tileY) && this.indoors != null))
                {
                    if (who.getMount() != null)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:DismountBeforeEntering"));
                        return false;
                    }
                    this.indoors.isStructure = true;
                    this.indoors.uniqueName = this.baseNameOfIndoors + (object)(this.tileX * 2000 + this.tileY);
                    Game1.warpFarmer(this.indoors, this.indoors.warps[0].X, this.indoors.warps[0].Y - 1, Game1.player.facingDirection, true);
                    Game1.playSound("doorClose");
                    return true;
                }
            }
            return false;
        }

        protected override GameLocation getIndoors()
        {
            if (this.indoors != null)
                this.nameOfIndoorsWithoutUnique = this.indoors.name;
            GameLocation gameLocation = (GameLocation)new BreweryLocation();
            gameLocation.IsFarm = true;
            gameLocation.isStructure = true;
            gameLocation.isOutdoors = false;
            gameLocation.warps[0].TargetX = this.humanDoor.X + this.tileX;
            gameLocation.warps[0].TargetY = this.humanDoor.Y + this.tileY + 1;
            gameLocation.warps.Add(new Warp(18, 21, this.nameOfIndoors, 18, 44, false));
            gameLocation.warps.Add(new Warp(19, 21, this.nameOfIndoors, 19, 44, false));
            gameLocation.warps.Add(new Warp(18, 43, this.nameOfIndoors, 18, 19, false));
            gameLocation.warps.Add(new Warp(19, 43, this.nameOfIndoors, 19, 19, false));

            return gameLocation;
        }

        public override void load()
        {
            this.texture = Game1.content.Load<Texture2D>(@"Buildings\Brewery");
            GameLocation indoors1 = this.getIndoors();
            if (indoors1 == null)
                return;
            indoors1.characters = this.indoors.characters;
            indoors1.objects = this.indoors.objects;
            indoors1.terrainFeatures = this.indoors.terrainFeatures;
            indoors1.IsFarm = true;
            indoors1.IsOutdoors = false;
            indoors1.isStructure = true;
            indoors1.uniqueName = indoors1.name + (object)(this.tileX * 2000 + this.tileY);
            indoors1.numberOfSpawnedObjectsOnMap = this.indoors.numberOfSpawnedObjectsOnMap;

            if (indoors1 is DecoratableLocation && this.indoors is DecoratableLocation)
            {
                ((DecoratableLocation)indoors1).furniture = ((DecoratableLocation)this.indoors).furniture;
                foreach (Furniture furniture in ((DecoratableLocation)indoors1).furniture)
                    furniture.updateDrawPosition();
                ((DecoratableLocation)indoors1).wallPaper = ((DecoratableLocation)this.indoors).wallPaper;
                ((DecoratableLocation)indoors1).floor = ((DecoratableLocation)this.indoors).floor;
            }

            this.indoors = indoors1;

            if (this.indoors.IsFarm && this.indoors.terrainFeatures == null)
                this.indoors.terrainFeatures = new SerializableDictionary<Vector2, TerrainFeature>();
            foreach (NPC character in this.indoors.characters)
                character.reloadSprite();
            foreach (TerrainFeature terrainFeature in this.indoors.terrainFeatures.Values)
                terrainFeature.loadSprite();
            foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in (Dictionary<Vector2, StardewValley.Object>)this.indoors.objects)
            {
                keyValuePair.Value.initializeLightSource(keyValuePair.Key);
                keyValuePair.Value.reloadSprite();
            }
        }
    }
}
