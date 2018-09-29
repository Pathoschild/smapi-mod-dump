using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;
using xTile.Tiles;

namespace Revitalize
{
    [XmlInclude(typeof(Farm)), XmlInclude(typeof(Beach)), XmlInclude(typeof(AnimalHouse)), XmlInclude(typeof(SlimeHutch)), XmlInclude(typeof(Shed)), XmlInclude(typeof(LibraryMuseum)), XmlInclude(typeof(AdventureGuild)), XmlInclude(typeof(Woods)), XmlInclude(typeof(Railroad)), XmlInclude(typeof(Summit)), XmlInclude(typeof(Forest)), XmlInclude(typeof(SeedShop)), XmlInclude(typeof(BathHousePool)), XmlInclude(typeof(FarmHouse)), XmlInclude(typeof(Club)), XmlInclude(typeof(BusStop)), XmlInclude(typeof(CommunityCenter)), XmlInclude(typeof(Desert)), XmlInclude(typeof(FarmCave)), XmlInclude(typeof(JojaMart)), XmlInclude(typeof(MineShaft)), XmlInclude(typeof(Mountain)), XmlInclude(typeof(Sewer)), XmlInclude(typeof(WizardHouse)), XmlInclude(typeof(Town)), XmlInclude(typeof(Cellar))]
    public class GameLoc : StardewValley.GameLocation
    {


        private float waterPosition;

        private Vector2 snowPos;

        public GameLoc()
        {
        }

        public GameLoc(Map map, string name)
        {
            this.map = map;
            this.name = name;
            if (name.Contains("Farm") || name.Contains("Coop") || name.Contains("Barn") || name.Equals("SlimeHutch"))
            {
                this.isFarm = true;
            }
            this.loadObjects();
            this.objects.CollectionChanged += new NotifyCollectionChangedEventHandler(this.objectCollectionChanged);
            this.terrainFeatures.CollectionChanged += new NotifyCollectionChangedEventHandler(this.terrainFeaturesCollectionChanged);
            if ((this.isOutdoors || this is Sewer) && !(this is Desert))
            {
                this.waterTiles = new bool[map.Layers[0].LayerWidth, map.Layers[0].LayerHeight];
                bool flag = false;
                for (int i = 0; i < map.Layers[0].LayerWidth; i++)
                {
                    for (int j = 0; j < map.Layers[0].LayerHeight; j++)
                    {
                        if (this.doesTileHaveProperty(i, j, "Water", "Back") != null)
                        {
                            flag = true;
                            this.waterTiles[i, j] = true;
                        }
                    }
                }
                if (!flag)
                {
                    this.waterTiles = null;
                }
            }
            if (this.isOutdoors)
            {
                this.critters = new List<Critter>();
            }
        }


        public override bool isCollidingPosition(Microsoft.Xna.Framework.Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {

            foreach (KeyValuePair<Vector2, StardewValley.Object> current in this.objects)
            {
               // Log.AsyncC("EHHHHHH");
                if (current.Value.getBoundingBox(current.Value.tileLocation).Intersects(position))
                {
                    return true;
                }
            }
            return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, false, false);


            if (position.Right < 0 || position.X > this.map.Layers[0].DisplayWidth || position.Bottom < 0 || position.Top > this.map.Layers[0].DisplayHeight)
            {
                return false;
            }
            Vector2 vector = new Vector2((float)(position.Right / Game1.tileSize), (float)(position.Top / Game1.tileSize));
            Vector2 vector2 = new Vector2((float)(position.Left / Game1.tileSize), (float)(position.Top / Game1.tileSize));
            Vector2 vector3 = new Vector2((float)(position.Right / Game1.tileSize), (float)(position.Bottom / Game1.tileSize));
            Vector2 vector4 = new Vector2((float)(position.Left / Game1.tileSize), (float)(position.Bottom / Game1.tileSize));
            bool flag = position.Width > Game1.tileSize;
            Vector2 vector5 = new Vector2((float)(position.Center.X / Game1.tileSize), (float)(position.Bottom / Game1.tileSize));
            Vector2 vector6 = new Vector2((float)(position.Center.X / Game1.tileSize), (float)(position.Top / Game1.tileSize));
            if (character == null && !ignoreCharacterRequirement)
            {
                return true;
            }
            if (!glider && (!Game1.eventUp || (character != null && character.GetType() != typeof(Character) && !isFarmer && (!pathfinding || !character.willDestroyObjectsUnderfoot))))
            {
                StardewValley.Object @object;
                this.objects.TryGetValue(vector, out @object);
                if (@object != null && !@object.IsHoeDirt && !@object.isPassable() && !Game1.player.temporaryImpassableTile.Intersects(@object.getBoundingBox(vector)) && @object.getBoundingBox(vector).Intersects(position) && character != null && (character.GetType() != typeof(FarmAnimal) || !@object.isAnimalProduct()) && character.collideWith(@object))
                {
                    return true;
                }
                this.objects.TryGetValue(vector3, out @object);
                if (@object != null && !@object.IsHoeDirt && !@object.isPassable() && !Game1.player.temporaryImpassableTile.Intersects(@object.getBoundingBox(vector3)) && @object.getBoundingBox(vector3).Intersects(position) && character != null && (character.GetType() != typeof(FarmAnimal) || !@object.isAnimalProduct()) && character.collideWith(@object))
                {
                    return true;
                }
                this.objects.TryGetValue(vector2, out @object);
                if (@object != null && !@object.IsHoeDirt && !@object.isPassable() && !Game1.player.temporaryImpassableTile.Intersects(@object.getBoundingBox(vector2)) && @object.getBoundingBox(vector2).Intersects(position) && character != null && (character.GetType() != typeof(FarmAnimal) || !@object.isAnimalProduct()) && character.collideWith(@object))
                {
                    return true;
                }
                this.objects.TryGetValue(vector4, out @object);
                if (@object != null && !@object.IsHoeDirt && !@object.isPassable() && !Game1.player.temporaryImpassableTile.Intersects(@object.getBoundingBox(vector4)) && @object.getBoundingBox(vector4).Intersects(position) && character != null && (character.GetType() != typeof(FarmAnimal) || !@object.isAnimalProduct()) && character.collideWith(@object))
                {
                    return true;
                }
                if (flag)
                {
                    this.objects.TryGetValue(vector5, out @object);
                    if (@object != null && !@object.IsHoeDirt && !@object.isPassable() && !Game1.player.temporaryImpassableTile.Intersects(@object.getBoundingBox(vector5)) && @object.getBoundingBox(vector5).Intersects(position) && (character.GetType() != typeof(FarmAnimal) || !@object.isAnimalProduct()) && character.collideWith(@object))
                    {
                        return true;
                    }
                    this.objects.TryGetValue(vector6, out @object);
                    if (@object != null && !@object.IsHoeDirt && !@object.isPassable() && !Game1.player.temporaryImpassableTile.Intersects(@object.getBoundingBox(vector6)) && @object.getBoundingBox(vector6).Intersects(position) && (character.GetType() != typeof(FarmAnimal) || !@object.isAnimalProduct()) && character.collideWith(@object))
                    {
                        return true;
                    }
                }
            }
            if (this.largeTerrainFeatures != null && !glider)
            {
                using (List<LargeTerrainFeature>.Enumerator enumerator = this.largeTerrainFeatures.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.getBoundingBox().Intersects(position))
                        {
                            return true;
                        }
                    }
                }
            }
            if (!Game1.eventUp && !glider)
            {
                if (this.terrainFeatures.ContainsKey(vector) && this.terrainFeatures[vector].getBoundingBox(vector).Intersects(position))
                {
                    if (!pathfinding)
                    {
                        this.terrainFeatures[vector].doCollisionAction(position, Game1.player.speed + Game1.player.addedSpeed, vector, character, this);
                    }
                    if (this.terrainFeatures.ContainsKey(vector) && !this.terrainFeatures[vector].isPassable(character))
                    {
                        return true;
                    }
                }
                if (this.terrainFeatures.ContainsKey(vector2) && this.terrainFeatures[vector2].getBoundingBox(vector2).Intersects(position))
                {
                    if (!pathfinding)
                    {
                        this.terrainFeatures[vector2].doCollisionAction(position, Game1.player.speed + Game1.player.addedSpeed, vector2, character, this);
                    }
                    if (this.terrainFeatures.ContainsKey(vector2) && !this.terrainFeatures[vector2].isPassable(character))
                    {
                        return true;
                    }
                }
                if (this.terrainFeatures.ContainsKey(vector3) && this.terrainFeatures[vector3].getBoundingBox(vector3).Intersects(position))
                {
                    if (!pathfinding)
                    {
                        this.terrainFeatures[vector3].doCollisionAction(position, Game1.player.speed + Game1.player.addedSpeed, vector3, character, this);
                    }
                    if (this.terrainFeatures.ContainsKey(vector3) && !this.terrainFeatures[vector3].isPassable(character))
                    {
                        return true;
                    }
                }
                if (this.terrainFeatures.ContainsKey(vector4) && this.terrainFeatures[vector4].getBoundingBox(vector4).Intersects(position))
                {
                    if (!pathfinding)
                    {
                        this.terrainFeatures[vector4].doCollisionAction(position, Game1.player.speed + Game1.player.addedSpeed, vector4, character, this);
                    }
                    if (this.terrainFeatures.ContainsKey(vector4) && !this.terrainFeatures[vector4].isPassable(character))
                    {
                        return true;
                    }
                }
                if (flag)
                {
                    if (this.terrainFeatures.ContainsKey(vector5) && this.terrainFeatures[vector5].getBoundingBox(vector5).Intersects(position))
                    {
                        if (!pathfinding)
                        {
                            this.terrainFeatures[vector5].doCollisionAction(position, Game1.player.speed + Game1.player.addedSpeed, vector5, character, this);
                        }
                        if (this.terrainFeatures.ContainsKey(vector5) && !this.terrainFeatures[vector5].isPassable(null))
                        {
                            return true;
                        }
                    }
                    if (this.terrainFeatures.ContainsKey(vector6) && this.terrainFeatures[vector6].getBoundingBox(vector6).Intersects(position))
                    {
                        if (!pathfinding)
                        {
                            this.terrainFeatures[vector6].doCollisionAction(position, Game1.player.speed + Game1.player.addedSpeed, vector6, character, this);
                        }
                        if (this.terrainFeatures.ContainsKey(vector6) && !this.terrainFeatures[vector6].isPassable(null))
                        {
                            return true;
                        }
                    }
                }
            }
            if (character != null && character.hasSpecialCollisionRules() && (character.isColliding(this, vector) || character.isColliding(this, vector2) || character.isColliding(this, vector3) || character.isColliding(this, vector4)))
            {
                return true;
            }
            if (isFarmer || (character != null && character.collidesWithOtherCharacters))
            {
                for (int i = this.characters.Count - 1; i >= 0; i--)
                {
                    if (this.characters[i] != null && (character == null || !character.Equals(this.characters[i])))
                    {
                        if (this.characters[i].GetBoundingBox().Intersects(position) && !Game1.player.temporarilyInvincible)
                        {
                            this.characters[i].behaviorOnFarmerPushing();
                        }
                        if (isFarmer && !Game1.eventUp && !this.characters[i].farmerPassesThrough && this.characters[i].GetBoundingBox().Intersects(position) && !Game1.player.temporarilyInvincible && Game1.player.temporaryImpassableTile.Equals(Microsoft.Xna.Framework.Rectangle.Empty) && (!this.characters[i].IsMonster || (!((Monster)this.characters[i]).isGlider && !Game1.player.GetBoundingBox().Intersects(this.characters[i].GetBoundingBox()))) && !this.characters[i].isInvisible)
                        {
                            return true;
                        }
                        if (!isFarmer && this.characters[i].GetBoundingBox().Intersects(position))
                        {
                            return true;
                        }
                    }
                }
            }
            if (isFarmer)
            {
                if (this.currentEvent != null && this.currentEvent.checkForCollision(position, (character != null) ? (character as StardewValley.Farmer) : Game1.player))
                {
                    return true;
                }
                if (Game1.player.currentUpgrade != null && Game1.player.currentUpgrade.daysLeftTillUpgradeDone <= 3 && this.name.Equals("Farm") && position.Intersects(new Microsoft.Xna.Framework.Rectangle((int)Game1.player.currentUpgrade.positionOfCarpenter.X, (int)Game1.player.currentUpgrade.positionOfCarpenter.Y + Game1.tileSize, Game1.tileSize, Game1.tileSize / 2)))
                {
                    return true;
                }
            }
            else
            {
                if (position.Intersects(Game1.player.GetBoundingBox()))
                {
                    if (damagesFarmer > 0)
                    {
                        if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon && ((MeleeWeapon)Game1.player.CurrentTool).isOnSpecial && ((MeleeWeapon)Game1.player.CurrentTool).type == 3)
                        {
                            Game1.farmerTakeDamage(damagesFarmer, false, character as Monster);
                            return true;
                        }
                        if (character != null)
                        {
                            character.collisionWithFarmerBehavior();
                        }
                        Game1.farmerTakeDamage(Math.Max(1, damagesFarmer + Game1.random.Next(-damagesFarmer / 4, damagesFarmer / 4)), false, character as Monster);
                        if (!glider && (character == null || !(character as Monster).passThroughCharacters()))
                        {
                            return true;
                        }
                    }
                    else if (!glider && (!pathfinding || !(character is Monster)))
                    {
                        return true;
                    }
                }
                if (damagesFarmer > 0 && !glider)
                {
                    for (int j = 0; j < this.characters.Count; j++)
                    {
                        if (!this.characters[j].Equals(character) && position.Intersects(this.characters[j].GetBoundingBox()) && !(character is GreenSlime) && !(character is DustSpirit))
                        {
                            return true;
                        }
                    }
                }
                if ((this.isFarm || this.name.Equals("UndergroundMine")) && character != null && !character.name.Contains("NPC") && !character.eventActor && !glider)
                {
                    PropertyValue propertyValue = null;
                    Tile tile = this.map.GetLayer("Back").PickTile(new Location(position.Right, position.Top), viewport.Size);
                    if (tile != null)
                    {
                        tile.Properties.TryGetValue("NPCBarrier", out propertyValue);
                    }
                    if (propertyValue != null)
                    {
                        return true;
                    }
                    tile = this.map.GetLayer("Back").PickTile(new Location(position.Right, position.Bottom), viewport.Size);
                    if (tile != null)
                    {
                        tile.Properties.TryGetValue("NPCBarrier", out propertyValue);
                    }
                    if (propertyValue != null)
                    {
                        return true;
                    }
                    tile = this.map.GetLayer("Back").PickTile(new Location(position.Left, position.Top), viewport.Size);
                    if (tile != null)
                    {
                        tile.Properties.TryGetValue("NPCBarrier", out propertyValue);
                    }
                    if (propertyValue != null)
                    {
                        return true;
                    }
                    tile = this.map.GetLayer("Back").PickTile(new Location(position.Left, position.Bottom), viewport.Size);
                    if (tile != null)
                    {
                        tile.Properties.TryGetValue("NPCBarrier", out propertyValue);
                    }
                    if (propertyValue != null)
                    {
                        return true;
                    }
                }
                if (glider && !projectile)
                {
                    return false;
                }
            }
            if (!isFarmer || !Game1.player.isRafting)
            {
                PropertyValue propertyValue2 = null;
                Tile tile2 = this.map.GetLayer("Back").PickTile(new Location(position.Right, position.Top), viewport.Size);
                if (tile2 != null)
                {
                    tile2.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                }
                if (propertyValue2 != null && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Right, position.Top)))
                {
                    return true;
                }
                tile2 = this.map.GetLayer("Back").PickTile(new Location(position.Right, position.Bottom), viewport.Size);
                if (tile2 != null)
                {
                    tile2.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                }
                if (propertyValue2 != null && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Right, position.Bottom)))
                {
                    return true;
                }
                tile2 = this.map.GetLayer("Back").PickTile(new Location(position.Left, position.Top), viewport.Size);
                if (tile2 != null)
                {
                    tile2.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                }
                if (propertyValue2 != null && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Left, position.Top)))
                {
                    return true;
                }
                tile2 = this.map.GetLayer("Back").PickTile(new Location(position.Left, position.Bottom), viewport.Size);
                if (tile2 != null)
                {
                    tile2.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                }
                if (propertyValue2 != null && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Left, position.Bottom)))
                {
                    return true;
                }
                if (flag)
                {
                    tile2 = this.map.GetLayer("Back").PickTile(new Location(position.Center.X, position.Bottom), viewport.Size);
                    if (tile2 != null)
                    {
                        tile2.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                    }
                    if (propertyValue2 != null && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Center.X, position.Bottom)))
                    {
                        return true;
                    }
                    tile2 = this.map.GetLayer("Back").PickTile(new Location(position.Center.X, position.Top), viewport.Size);
                    if (tile2 != null)
                    {
                        tile2.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                    }
                    if (propertyValue2 != null && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Center.X, position.Top)))
                    {
                        return true;
                    }
                }
                Tile tile3 = this.map.GetLayer("Buildings").PickTile(new Location(position.Right, position.Top), viewport.Size);
                if (tile3 != null)
                {
                    tile3.TileIndexProperties.TryGetValue("Shadow", out propertyValue2);
                    if (propertyValue2 == null)
                    {
                        tile3.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                    }
                    if (propertyValue2 == null && !isFarmer)
                    {
                        tile3.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue2);
                    }
                    if (propertyValue2 == null && !isFarmer && character != null && character.canPassThroughActionTiles())
                    {
                        tile3.Properties.TryGetValue("Action", out propertyValue2);
                    }
                    if ((propertyValue2 == null || propertyValue2.ToString().Length == 0) && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Right, position.Top)))
                    {
                        return character == null || character.shouldCollideWithBuildingLayer(this);
                    }
                }
                tile3 = this.map.GetLayer("Buildings").PickTile(new Location(position.Right, position.Bottom), viewport.Size);
                if (tile3 != null && (propertyValue2 == null | isFarmer))
                {
                    tile3.TileIndexProperties.TryGetValue("Shadow", out propertyValue2);
                    if (propertyValue2 == null)
                    {
                        tile3.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                    }
                    if (propertyValue2 == null && !isFarmer)
                    {
                        tile3.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue2);
                    }
                    if (propertyValue2 == null && !isFarmer && character != null && character.canPassThroughActionTiles())
                    {
                        tile3.Properties.TryGetValue("Action", out propertyValue2);
                    }
                    if ((propertyValue2 == null || propertyValue2.ToString().Length == 0) && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Right, position.Bottom)))
                    {
                        return character == null || character.shouldCollideWithBuildingLayer(this);
                    }
                }
                tile3 = this.map.GetLayer("Buildings").PickTile(new Location(position.Left, position.Top), viewport.Size);
                if (tile3 != null && (propertyValue2 == null | isFarmer))
                {
                    tile3.TileIndexProperties.TryGetValue("Shadow", out propertyValue2);
                    if (propertyValue2 == null)
                    {
                        tile3.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                    }
                    if (propertyValue2 == null && !isFarmer)
                    {
                        tile3.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue2);
                    }
                    if (propertyValue2 == null && !isFarmer && character != null && character.canPassThroughActionTiles())
                    {
                        tile3.Properties.TryGetValue("Action", out propertyValue2);
                    }
                    if ((propertyValue2 == null || propertyValue2.ToString().Length == 0) && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Left, position.Top)))
                    {
                        return character == null || character.shouldCollideWithBuildingLayer(this);
                    }
                }
                tile3 = this.map.GetLayer("Buildings").PickTile(new Location(position.Left, position.Bottom), viewport.Size);
                if (tile3 != null && (propertyValue2 == null | isFarmer))
                {
                    tile3.TileIndexProperties.TryGetValue("Shadow", out propertyValue2);
                    if (propertyValue2 == null)
                    {
                        tile3.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                    }
                    if (propertyValue2 == null && !isFarmer)
                    {
                        tile3.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue2);
                    }
                    if (propertyValue2 == null && !isFarmer && character != null && character.canPassThroughActionTiles())
                    {
                        tile3.Properties.TryGetValue("Action", out propertyValue2);
                    }
                    if ((propertyValue2 == null || propertyValue2.ToString().Length == 0) && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Left, position.Bottom)))
                    {
                        return character == null || character.shouldCollideWithBuildingLayer(this);
                    }
                }
                if (flag)
                {
                    tile3 = this.map.GetLayer("Buildings").PickTile(new Location(position.Center.X, position.Top), viewport.Size);
                    if (tile3 != null && (propertyValue2 == null | isFarmer))
                    {
                        tile3.TileIndexProperties.TryGetValue("Shadow", out propertyValue2);
                        if (propertyValue2 == null)
                        {
                            tile3.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                        }
                        if (propertyValue2 == null && !isFarmer)
                        {
                            tile3.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue2);
                        }
                        if (propertyValue2 == null && !isFarmer && character != null && character.canPassThroughActionTiles())
                        {
                            tile3.Properties.TryGetValue("Action", out propertyValue2);
                        }
                        if ((propertyValue2 == null || propertyValue2.ToString().Length == 0) && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Center.X, position.Top)))
                        {
                            return character == null || character.shouldCollideWithBuildingLayer(this);
                        }
                    }
                    tile3 = this.map.GetLayer("Buildings").PickTile(new Location(position.Center.X, position.Bottom), viewport.Size);
                    if (tile3 != null && (propertyValue2 == null | isFarmer))
                    {
                        tile3.TileIndexProperties.TryGetValue("Shadow", out propertyValue2);
                        if (propertyValue2 == null)
                        {
                            tile3.TileIndexProperties.TryGetValue("Passable", out propertyValue2);
                        }
                        if (propertyValue2 == null && !isFarmer)
                        {
                            tile3.TileIndexProperties.TryGetValue("NPCPassable", out propertyValue2);
                        }
                        if (propertyValue2 == null && !isFarmer && character != null && character.canPassThroughActionTiles())
                        {
                            tile3.Properties.TryGetValue("Action", out propertyValue2);
                        }
                        if ((propertyValue2 == null || propertyValue2.ToString().Length == 0) && (!isFarmer || !Game1.player.temporaryImpassableTile.Contains(position.Center.X, position.Bottom)))
                        {
                            return character == null || character.shouldCollideWithBuildingLayer(this);
                        }
                    }
                }
                if (!isFarmer && propertyValue2 != null)
                {
                    string a = propertyValue2.ToString().Split(new char[]
                    {
                        ' '
                    })[0];
                    if (a == "Door")
                    {
                        this.openDoor(new Location(position.Center.X / Game1.tileSize, position.Bottom / Game1.tileSize), false);
                        this.openDoor(new Location(position.Center.X / Game1.tileSize, position.Top / Game1.tileSize), Game1.currentLocation.Equals(this));
                    }
                }
                return false;
            }
            else
            {
                PropertyValue propertyValue3 = null;
                Tile tile4 = this.map.GetLayer("Back").PickTile(new Location(position.Right, position.Top), viewport.Size);
                if (tile4 != null)
                {
                    tile4.TileIndexProperties.TryGetValue("Water", out propertyValue3);
                }
                if (propertyValue3 == null)
                {
                    if (this.isTileLocationOpen(new Location(position.Right, position.Top)) && !this.isTileOccupiedForPlacement(new Vector2((float)(position.Right / Game1.tileSize), (float)(position.Top / Game1.tileSize)), null))
                    {
                        Game1.player.isRafting = false;
                        Game1.player.position = new Vector2((float)(position.Right / Game1.tileSize * Game1.tileSize), (float)(position.Top / Game1.tileSize * Game1.tileSize - Game1.tileSize / 2));
                        Game1.player.setTrajectory(0, 0);
                    }
                    return true;
                }
                tile4 = this.map.GetLayer("Back").PickTile(new Location(position.Right, position.Bottom), viewport.Size);
                if (tile4 != null)
                {
                    tile4.TileIndexProperties.TryGetValue("Water", out propertyValue3);
                }
                if (propertyValue3 == null)
                {
                    if (this.isTileLocationOpen(new Location(position.Right, position.Bottom)) && !this.isTileOccupiedForPlacement(new Vector2((float)(position.Right / Game1.tileSize), (float)(position.Bottom / Game1.tileSize)), null))
                    {
                        Game1.player.isRafting = false;
                        Game1.player.position = new Vector2((float)(position.Right / Game1.tileSize * Game1.tileSize), (float)(position.Bottom / Game1.tileSize * Game1.tileSize - Game1.tileSize / 2));
                        Game1.player.setTrajectory(0, 0);
                    }
                    return true;
                }
                tile4 = this.map.GetLayer("Back").PickTile(new Location(position.Left, position.Top), viewport.Size);
                if (tile4 != null)
                {
                    tile4.TileIndexProperties.TryGetValue("Water", out propertyValue3);
                }
                if (propertyValue3 == null)
                {
                    if (this.isTileLocationOpen(new Location(position.Left, position.Top)) && !this.isTileOccupiedForPlacement(new Vector2((float)(position.Left / Game1.tileSize), (float)(position.Top / Game1.tileSize)), null))
                    {
                        Game1.player.isRafting = false;
                        Game1.player.position = new Vector2((float)(position.Left / Game1.tileSize * Game1.tileSize), (float)(position.Top / Game1.tileSize * Game1.tileSize - Game1.tileSize / 2));
                        Game1.player.setTrajectory(0, 0);
                    }
                    return true;
                }
                tile4 = this.map.GetLayer("Back").PickTile(new Location(position.Left, position.Bottom), viewport.Size);
                if (tile4 != null)
                {
                    tile4.TileIndexProperties.TryGetValue("Water", out propertyValue3);
                }
                if (propertyValue3 == null)
                {
                    if (this.isTileLocationOpen(new Location(position.Left, position.Bottom)) && !this.isTileOccupiedForPlacement(new Vector2((float)(position.Left / Game1.tileSize), (float)(position.Bottom / Game1.tileSize)), null))
                    {
                        Game1.player.isRafting = false;
                        Game1.player.position = new Vector2((float)(position.Left / Game1.tileSize * Game1.tileSize), (float)(position.Bottom / Game1.tileSize * Game1.tileSize - Game1.tileSize / 2));
                        Game1.player.setTrajectory(0, 0);
                    }
                    return true;
                }
                return false;
            }
        }


        public new bool performAction(string action, StardewValley.Farmer who, Location tileLocation)
        {
            //Log.AsyncG("WHY???");

            //Log.AsyncC(action);
            //Log.AsyncM(who);
            //Log.AsyncO(tileLocation);


            if (action != null && who.IsMainPlayer)
            {
                string[] array = action.ToString().Split(new char[]
                {
                    ' '
                });
                string text = array[0];


                //Log.AsyncG(text);



                if (text == "Billboard")
                {
                   // Game1.activeClickableMenu = new Revitalize.Menus.Billboard(array[1].Equals("3"));
                }
                if (text == "Warp")
                {
                    who.faceGeneralDirection(new Vector2((float)tileLocation.X, (float)tileLocation.Y) * (float)Game1.tileSize, 0);
                    Rumble.rumble(0.15f, 200f);
                    Game1.warpFarmer(array[3], Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), false);
                    if (array.Length < 5)
                    {
                        Game1.playSound("doorClose");
                        return true;
                    }
                }


            }


                return true;
           // return base.performAction(action, who, tileLocation);
        }
        /*
        public bool performAction(string action, StardewValley.Farmer who, Location tileLocation)
        {
            if (action != null && who.IsMainPlayer)
            {
                string[] array = action.ToString().Split(new char[]
                {
                    ' '
                });
                string text = array[0];
                uint num = Hash.ComputeStringHash(text);
                if (num <= 2413466880u)
                {
                    if (num <= 1135412759u)
                    {
                        if (num <= 297990791u)
                        {
                            if (num <= 139067618u)
                            {
                                if (num <= 48641340u)
                                {
                                    if (num != 4774130u)
                                    {
                                        if (num != 48641340u)
                                        {
                                            return true;
                                        }
                                        if (!(text == "SpiritAltar"))
                                        {
                                            return true;
                                        }
                                        if (who.ActiveObject != null && Game1.dailyLuck != -0.12 && Game1.dailyLuck != 0.12)
                                        {
                                            if (who.ActiveObject.Price >= 60)
                                            {
                                                this.temporarySprites.Add(new TemporaryAnimatedSprite(352, 70f, 2, 2, new Vector2((float)(tileLocation.X * Game1.tileSize), (float)(tileLocation.Y * Game1.tileSize)), false, false));
                                                Game1.dailyLuck = 0.12;
                                                Game1.playSound("money");
                                            }
                                            else
                                            {
                                                this.temporarySprites.Add(new TemporaryAnimatedSprite(362, 50f, 6, 1, new Vector2((float)(tileLocation.X * Game1.tileSize), (float)(tileLocation.Y * Game1.tileSize)), false, false));
                                                Game1.dailyLuck = -0.12;
                                                Game1.playSound("thunder");
                                            }
                                            who.ActiveObject = null;
                                            who.showNotCarrying();
                                            return true;
                                        }
                                        return true;
                                    }
                                    else
                                    {
                                        if (!(text == "EvilShrineRight"))
                                        {
                                            return true;
                                        }
                                        if (Game1.spawnMonstersAtNight)
                                        {
                                            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineRightDeActivate", new object[0]), this.createYesNoResponses(), "evilShrineRightDeActivate");
                                            return true;
                                        }
                                        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineRightActivate", new object[0]), this.createYesNoResponses(), "evilShrineRightActivate");
                                        return true;
                                    }
                                }
                                else if (num != 49977355u)
                                {
                                    if (num != 135767117u)
                                    {
                                        if (num != 139067618u)
                                        {
                                            return true;
                                        }
                                        if (!(text == "IceCreamStand"))
                                        {
                                            return true;
                                        }
                                        if (this.isCharacterAtTile(new Vector2((float)tileLocation.X, (float)(tileLocation.Y - 2))) != null || this.isCharacterAtTile(new Vector2((float)tileLocation.X, (float)(tileLocation.Y - 1))) != null || this.isCharacterAtTile(new Vector2((float)tileLocation.X, (float)(tileLocation.Y - 3))) != null)
                                        {
                                            Game1.activeClickableMenu = new ShopMenu(new Dictionary<Item, int[]>
                                            {
                                                {
                                                    new Object(233, 1, false, -1, 0),
                                                    new int[]
                                                    {
                                                        250,
                                                        2147483647
                                                    }
                                                }
                                            }, 0, null);
                                            return true;
                                        }
                                        if (Game1.currentSeason.Equals("summer"))
                                        {
                                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:IceCreamStand_ComeBackLater", new object[0]));
                                            return true;
                                        }
                                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:IceCreamStand_NotSummer", new object[0]));
                                        return true;
                                    }
                                    else
                                    {
                                        if (!(text == "Jukebox"))
                                        {
                                            return true;
                                        }
                                        Game1.activeClickableMenu = new ChooseFromListMenu(Game1.player.songsHeard, new ChooseFromListMenu.actionOnChoosingListOption(ChooseFromListMenu.playSongAction), true);
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (!(text == "Crib"))
                                    {
                                        return true;
                                    }
                                    using (List<NPC>.Enumerator enumerator = this.characters.GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            NPC current = enumerator.Current;
                                            if (current is Child && (current as Child).age == 1)
                                            {
                                                (current as Child).toss(who);
                                            }
                                            else if (current is Child && (current as Child).age == 0)
                                            {
                                                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:FarmHouse_Crib_NewbornSleeping", new object[]
                                                {
                                                    current.name
                                                })));
                                            }
                                        }
                                        return true;
                                    }
                                }
                            }
                            else if (num <= 234320812u)
                            {
                                if (num != 183343509u)
                                {
                                    if (num != 234320812u)
                                    {
                                        return true;
                                    }
                                    if (!(text == "SandDragon"))
                                    {
                                        return true;
                                    }
                                    if (who.ActiveObject != null && who.ActiveObject.parentSheetIndex == 768 && !who.hasOrWillReceiveMail("TH_SandDragon") && who.hasOrWillReceiveMail("TH_MayorFridge"))
                                    {
                                        who.reduceActiveItemByOne();
                                        Game1.player.CanMove = false;
                                        Game1.playSound("eat");
                                        Game1.player.mailReceived.Add("TH_SandDragon");
                                        Game1.multipleDialogues(new string[]
                                        {
                                            Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_ConsumeEssence", new object[0]),
                                            Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_MrQiNote", new object[0])
                                        });
                                        Game1.player.removeQuest(4);
                                        Game1.player.addQuest(5);
                                        return true;
                                    }
                                    if (who.hasOrWillReceiveMail("TH_SandDragon"))
                                    {
                                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_MrQiNote", new object[0]));
                                        return true;
                                    }
                                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Desert_SandDragon_Initial", new object[0]));
                                    return true;
                                }
                                else
                                {
                                    if (!(text == "ColaMachine"))
                                    {
                                        return true;
                                    }
                                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_ColaMachine_Question", new object[0]), this.createYesNoResponses(), "buyJojaCola");
                                    return true;
                                }
                            }
                            else if (num != 267393898u)
                            {
                                if (num != 295776207u)
                                {
                                    if (num != 297990791u)
                                    {
                                        return true;
                                    }
                                    if (!(text == "NPCMessage"))
                                    {
                                        return true;
                                    }
                                    NPC characterFromName = Game1.getCharacterFromName(array[1]);
                                    if (characterFromName != null && characterFromName.withinPlayerThreshold(14))
                                    {
                                        try
                                        {
                                            characterFromName.setNewDialogue(Game1.content.LoadString(action.Substring(action.IndexOf('"') + 1).Split(new char[]
                                            {
                                                '/'
                                            })[0], new object[0]), true, false);
                                            Game1.drawDialogue(characterFromName);
                                            bool result = false;
                                            return result;
                                        }
                                        catch (Exception)
                                        {
                                            bool result = false;
                                            return result;
                                        }
                                    }
                                    try
                                    {
                                        Game1.drawDialogueNoTyping(Game1.content.LoadString(action.Substring(action.IndexOf('"')).Split(new char[]
                                        {
                                            '/'
                                        })[1].Replace("\"", ""), new object[0]));
                                        bool result = false;
                                        return result;
                                    }
                                    catch (Exception)
                                    {
                                        bool result = false;
                                        return result;
                                    }
                                    goto IL_1F09;
                                }
                                else
                                {
                                    if (!(text == "Warp"))
                                    {
                                        return true;
                                    }
                                    who.faceGeneralDirection(new Vector2((float)tileLocation.X, (float)tileLocation.Y) * (float)Game1.tileSize, 0);
                                    Rumble.rumble(0.15f, 200f);
                                    Game1.warpFarmer(array[3], Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), false);
                                    if (array.Length < 5)
                                    {
                                        Game1.playSound("doorClose");
                                        return true;
                                    }
                                    return true;
                                }
                            }
                            else
                            {
                                if (!(text == "Notes"))
                                {
                                    return true;
                                }
                                this.readNote(Convert.ToInt32(array[1]));
                                return true;
                            }
                        }
                        else if (num <= 837292325u)
                        {
                            if (num <= 414528787u)
                            {
                                if (num != 371676316u)
                                {
                                    if (num != 414528787u)
                                    {
                                        return true;
                                    }
                                    if (!(text == "QiCoins"))
                                    {
                                        return true;
                                    }
                                    if (who.clubCoins > 0)
                                    {
                                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Club_QiCoins", new object[]
                                        {
                                            who.clubCoins
                                        }));
                                        return true;
                                    }
                                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_QiCoins_BuyStarter", new object[0]), this.createYesNoResponses(), "BuyClubCoins");
                                    return true;
                                }
                                else
                                {
                                    if (!(text == "MineElevator"))
                                    {
                                        return true;
                                    }
                                    if (Game1.mine == null || Game1.mine.lowestLevelReached < 5)
                                    {
                                        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Mines_MineElevator_NotWorking", new object[0])));
                                        return true;
                                    }
                                    Game1.activeClickableMenu = new MineElevatorMenu();
                                    return true;
                                }
                            }
                            else if (num != 570160120u)
                            {
                                if (num != 634795166u)
                                {
                                    if (num != 837292325u)
                                    {
                                        return true;
                                    }
                                    if (!(text == "BuyBackpack"))
                                    {
                                        return true;
                                    }
                                    Response response = new Response("Purchase", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Response2000", new object[0]));
                                    Response response2 = new Response("Purchase", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Response10000", new object[0]));
                                    Response response3 = new Response("Not", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_ResponseNo", new object[0]));
                                    if (Game1.player.maxItems == 12)
                                    {
                                        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question24", new object[0]), new Response[]
                                        {
                                            response,
                                            response3
                                        }, "Backpack");
                                        return true;
                                    }
                                    if (Game1.player.maxItems < 36)
                                    {
                                        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_Question36", new object[0]), new Response[]
                                        {
                                            response2,
                                            response3
                                        }, "Backpack");
                                        return true;
                                    }
                                    return true;
                                }
                                else
                                {
                                    if (!(text == "ClubSlots"))
                                    {
                                        return true;
                                    }
                                    Game1.currentMinigame = new Slots(-1, false);
                                    return true;
                                }
                            }
                            else
                            {
                                if (!(text == "MagicInk"))
                                {
                                    return true;
                                }
                                if (!who.mailReceived.Contains("hasPickedUpMagicInk"))
                                {
                                    who.mailReceived.Add("hasPickedUpMagicInk");
                                    who.hasMagicInk = true;
                                    this.setMapTileIndex(4, 11, 113, "Buildings", 0);
                                    who.addItemByMenuIfNecessaryElseHoldUp(new SpecialItem(1, 7, ""), null);
                                    return true;
                                }
                                return true;
                            }
                        }
                        else if (num <= 895720287u)
                        {
                            if (num != 870733615u)
                            {
                                if (num != 895720287u)
                                {
                                    return true;
                                }
                                if (!(text == "HospitalShop"))
                                {
                                    return true;
                                }
                                if (this.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) != null || this.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) != null)
                                {
                                    Game1.activeClickableMenu = new ShopMenu(Utility.getHospitalStock(), 0, null);
                                    return true;
                                }
                                return true;
                            }
                            else
                            {
                                if (!(text == "kitchen"))
                                {
                                    return true;
                                }
                                Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, 0, 0);
                                Game1.activeClickableMenu = new CraftingPage((int)topLeftPositionForCenteringOnScreen.X, (int)topLeftPositionForCenteringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, true);
                                return true;
                            }
                        }
                        else if (num != 908820861u)
                        {
                            if (num != 1094091226u)
                            {
                                if (num != 1135412759u)
                                {
                                    return true;
                                }
                                if (!(text == "Carpenter"))
                                {
                                    return true;
                                }
                                if (who.getTileY() > tileLocation.Y)
                                {
                                    this.carpenters(tileLocation);
                                    return true;
                                }
                                return true;
                            }
                            else
                            {
                                if (!(text == "Arcade_Prairie"))
                                {
                                    return true;
                                }
                                Game1.currentMinigame = new AbigailGame(false);
                                return true;
                            }
                        }
                        else
                        {
                            if (!(text == "Letter"))
                            {
                                return true;
                            }
                            Game1.drawLetterMessage(this.actionParamsToString(array));
                            return true;
                        }
                    }
                    else if (num <= 1719994463u)
                    {
                        if (num <= 1555723527u)
                        {
                            if (num <= 1288111488u)
                            {
                                if (num != 1140116675u)
                                {
                                    if (num != 1288111488u)
                                    {
                                        return true;
                                    }
                                    if (!(text == "ShippingBin"))
                                    {
                                        return true;
                                    }
                                    Game1.shipHeldItem();
                                    return true;
                                }
                                else
                                {
                                    if (!(text == "JojaShop"))
                                    {
                                        return true;
                                    }
                                    goto IL_EEF;
                                }
                            }
                            else if (num != 1367472567u)
                            {
                                if (num != 1379459566u)
                                {
                                    if (num != 1555723527u)
                                    {
                                        return true;
                                    }
                                    if (!(text == "BlackJack"))
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (!(text == "WarpMensLocker"))
                                    {
                                        return true;
                                    }
                                    if (!who.isMale)
                                    {
                                        if (who.IsMainPlayer)
                                        {
                                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MensLocker_WrongGender", new object[0]));
                                        }
                                        return false;
                                    }
                                    who.faceGeneralDirection(new Vector2((float)tileLocation.X, (float)tileLocation.Y) * (float)Game1.tileSize, 0);
                                    Game1.warpFarmer(array[3], Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), false);
                                    if (array.Length < 5)
                                    {
                                        Game1.playSound("doorClose");
                                        return true;
                                    }
                                    return true;
                                }
                            }
                            else
                            {
                                if (!(text == "Blacksmith"))
                                {
                                    return true;
                                }
                                if (who.getTileY() > tileLocation.Y)
                                {
                                    this.blacksmith(tileLocation);
                                    return true;
                                }
                                return true;
                            }
                        }
                        else if (num <= 1673854597u)
                        {
                            if (num != 1573286044u)
                            {
                                if (num != 1673854597u)
                                {
                                    return true;
                                }
                                if (!(text == "WizardShrine"))
                                {
                                    return true;
                                }
                                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WizardTower_WizardShrine", new object[0]).Replace('\n', '^'), this.createYesNoResponses(), "WizardShrine");
                                return true;
                            }
                            else if (!(text == "ClubCards"))
                            {
                                return true;
                            }
                        }
                        else if (num != 1687261568u)
                        {
                            if (num != 1716769139u)
                            {
                                if (num != 1719994463u)
                                {
                                    return true;
                                }
                                if (!(text == "WizardBook"))
                                {
                                    return true;
                                }
                                if (who.mailReceived.Contains("hasPickedUpMagicInk") || who.hasMagicInk)
                                {
                                    Game1.activeClickableMenu = new CarpenterMenu(true);
                                    return true;
                                }
                                return true;
                            }
                            else
                            {
                                if (!(text == "Dialogue"))
                                {
                                    return true;
                                }
                                goto IL_1E36;
                            }
                        }
                        else
                        {
                            if (!(text == "ClubComputer"))
                            {
                                return true;
                            }
                            goto IL_25B2;
                        }
                        if (array.Length > 1 && array[1].Equals("1000"))
                        {
                            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_HS", new object[0]), new Response[]
                            {
                                new Response("Play", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Play", new object[0])),
                                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Leave", new object[0]))
                            }, "CalicoJackHS");
                            return true;
                        }
                        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_CalicoJack", new object[0]), new Response[]
                        {
                            new Response("Play", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Play", new object[0])),
                            new Response("Leave", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Leave", new object[0])),
                            new Response("Rules", Game1.content.LoadString("Strings\\Locations:Club_CalicoJack_Rules", new object[0]))
                        }, "CalicoJack");
                        return true;
                    }
                    else if (num <= 1959071256u)
                    {
                        if (num <= 1806134029u)
                        {
                            if (num != 1722787773u)
                            {
                                if (num != 1806134029u)
                                {
                                    return true;
                                }
                                if (!(text == "BuyQiCoins"))
                                {
                                    return true;
                                }
                                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_Buy100Coins", new object[0]), this.createYesNoResponses(), "BuyQiCoins");
                                return true;
                            }
                            else
                            {
                                if (!(text == "MessageOnce"))
                                {
                                    return true;
                                }
                                if (!who.eventsSeen.Contains(Convert.ToInt32(array[1])))
                                {
                                    who.eventsSeen.Add(Convert.ToInt32(array[1]));
                                    Game1.drawObjectDialogue(Game1.parseText(this.actionParamsToString(array).Substring(this.actionParamsToString(array).IndexOf(' '))));
                                    return true;
                                }
                                return true;
                            }
                        }
                        else if (num != 1852246243u)
                        {
                            if (num != 1856350152u)
                            {
                                if (num != 1959071256u)
                                {
                                    return true;
                                }
                                if (!(text == "WizardHatch"))
                                {
                                    return true;
                                }
                                if (who.friendships.ContainsKey("Wizard") && who.friendships["Wizard"][0] >= 1000)
                                {
                                    Game1.warpFarmer("WizardHouseBasement", 4, 4, true);
                                    Game1.playSound("doorClose");
                                    return true;
                                }
                                NPC nPC = this.characters[0];
                                nPC.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Data\\ExtraDialogue:Wizard_Hatch", new object[0]), nPC));
                                Game1.drawDialogue(nPC);
                                return true;
                            }
                            else
                            {
                                if (!(text == "Yoba"))
                                {
                                    return true;
                                }
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_Yoba", new object[0]));
                                return true;
                            }
                        }
                        else
                        {
                            if (!(text == "NextMineLevel"))
                            {
                                return true;
                            }
                            goto IL_21DE;
                        }
                    }
                    else if (num <= 2250926415u)
                    {
                        if (num != 2039622173u)
                        {
                            if (num != 2050472952u)
                            {
                                if (num != 2250926415u)
                                {
                                    return true;
                                }
                                if (!(text == "Tutorial"))
                                {
                                    return true;
                                }
                                Game1.activeClickableMenu = new TutorialMenu();
                                return true;
                            }
                            else
                            {
                                if (!(text == "DivorceBook"))
                                {
                                    return true;
                                }
                                if (Game1.player.divorceTonight)
                                {
                                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_CancelQuestion", new object[0]), this.createYesNoResponses(), "divorceCancel");
                                    return true;
                                }
                                if (Game1.player.isMarried())
                                {
                                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_Question", new object[0]), this.createYesNoResponses(), "divorce");
                                    return true;
                                }
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_DivorceBook_NoSpouse", new object[0]));
                                return true;
                            }
                        }
                        else
                        {
                            if (!(text == "LockedDoorWarp"))
                            {
                                return true;
                            }
                            who.faceGeneralDirection(new Vector2((float)tileLocation.X, (float)tileLocation.Y) * (float)Game1.tileSize, 0);
                            this.lockedDoorWarp(array);
                            return true;
                        }
                    }
                    else if (num != 2279498422u)
                    {
                        if (num != 2295680585u)
                        {
                            if (num != 2413466880u)
                            {
                                return true;
                            }
                            if (!(text == "RailroadBox"))
                            {
                                return true;
                            }
                            if (who.ActiveObject != null && who.ActiveObject.parentSheetIndex == 394 && !who.hasOrWillReceiveMail("TH_Railroad") && who.hasOrWillReceiveMail("TH_Tunnel"))
                            {
                                who.reduceActiveItemByOne();
                                Game1.player.CanMove = false;
                                Game1.playSound("Ship");
                                Game1.player.mailReceived.Add("TH_Railroad");
                                Game1.multipleDialogues(new string[]
                                {
                                    Game1.content.LoadString("Strings\\Locations:Railroad_Box_ConsumeShell", new object[0]),
                                    Game1.content.LoadString("Strings\\Locations:Railroad_Box_MrQiNote", new object[0])
                                });
                                Game1.player.removeQuest(2);
                                Game1.player.addQuest(3);
                                return true;
                            }
                            if (who.hasOrWillReceiveMail("TH_Railroad"))
                            {
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Railroad_Box_MrQiNote", new object[0]));
                                return true;
                            }
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Railroad_Box_Initial", new object[0]));
                            return true;
                        }
                        else
                        {
                            if (!(text == "Door"))
                            {
                                return true;
                            }
                            if (array.Length <= 1 || Game1.eventUp)
                            {
                                this.openDoor(tileLocation, true);
                                return false;
                            }
                            for (int i = 1; i < array.Length; i++)
                            {
                                if (who.getFriendshipHeartLevelForNPC(array[i]) >= 2 || Game1.player.mailReceived.Contains("doorUnlock" + array[i]))
                                {
                                    Rumble.rumble(0.1f, 100f);
                                    if (!Game1.player.mailReceived.Contains("doorUnlock" + array[i]))
                                    {
                                        Game1.player.mailReceived.Add("doorUnlock" + array[i]);
                                    }
                                    this.openDoor(tileLocation, true);
                                    return false;
                                }
                            }
                            if (array.Length == 2 && Game1.getCharacterFromName(array[1]) != null)
                            {
                                NPC characterFromName2 = Game1.getCharacterFromName(array[1]);
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_" + ((characterFromName2.gender == 0) ? "Male" : "Female"), new object[]
                                {
                                    characterFromName2.name
                                }));
                                return true;
                            }
                            if (Game1.getCharacterFromName(array[1]) != null && Game1.getCharacterFromName(array[2]) != null)
                            {
                                NPC characterFromName3 = Game1.getCharacterFromName(array[1]);
                                NPC characterFromName4 = Game1.getCharacterFromName(array[2]);
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:DoorUnlock_NotFriend_Couple", new object[]
                                {
                                    characterFromName3.name,
                                    characterFromName4.name
                                }));
                                return true;
                            }
                            return true;
                        }
                    }
                    else if (!(text == "WarpGreenhouse"))
                    {
                        return true;
                    }
                    if (Game1.player.mailReceived.Contains("ccPantry"))
                    {
                        who.faceGeneralDirection(new Vector2((float)tileLocation.X, (float)tileLocation.Y) * (float)Game1.tileSize, 0);
                        Game1.warpFarmer("Greenhouse", 10, 23, false);
                        Game1.playSound("doorClose");
                        return true;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Farm_GreenhouseRuins", new object[0]));
                    return true;
                }
                else if (num <= 3371180897u)
                {
                    if (num <= 2909376585u)
                    {
                        if (num <= 2764184545u)
                        {
                            if (num <= 2510785065u)
                            {
                                if (num != 2471112148u)
                                {
                                    if (num != 2510785065u)
                                    {
                                        return true;
                                    }
                                    if (!(text == "EvilShrineCenter"))
                                    {
                                        return true;
                                    }
                                    if (who.isDivorced())
                                    {
                                        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineCenter", new object[0]), this.createYesNoResponses(), "evilShrineCenter");
                                        return true;
                                    }
                                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineCenterInactive", new object[0]));
                                    return true;
                                }
                                else
                                {
                                    if (!(text == "FarmerFile"))
                                    {
                                        return true;
                                    }
                                    goto IL_25B2;
                                }
                            }
                            else if (num != 2528917107u)
                            {
                                if (num != 2738932126u)
                                {
                                    if (num != 2764184545u)
                                    {
                                        return true;
                                    }
                                    if (!(text == "MinecartTransport"))
                                    {
                                        return true;
                                    }
                                    if (Game1.player.mailReceived.Contains("ccBoilerRoom"))
                                    {
                                        Response[] answerChoices;
                                        if (Game1.player.mailReceived.Contains("ccCraftsRoom"))
                                        {
                                            answerChoices = new Response[]
                                            {
                                                new Response("Town", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town", new object[0])),
                                                new Response("Bus", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop", new object[0])),
                                                new Response("Quarry", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Quarry", new object[0])),
                                                new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel", new object[0]))
                                            };
                                        }
                                        else
                                        {
                                            answerChoices = new Response[]
                                            {
                                                new Response("Town", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town", new object[0])),
                                                new Response("Bus", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop", new object[0])),
                                                new Response("Cancel", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Cancel", new object[0]))
                                            };
                                        }
                                        this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_ChooseDestination", new object[0]), answerChoices, "Minecart");
                                        return true;
                                    }
                                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:MineCart_OutOfOrder", new object[0]));
                                    return true;
                                }
                                else
                                {
                                    if (!(text == "ClubSeller"))
                                    {
                                        return true;
                                    }
                                    this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Club_ClubSeller", new object[0]), new Response[]
                                    {
                                        new Response("I'll", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_Yes", new object[0])),
                                        new Response("No", Game1.content.LoadString("Strings\\Locations:Club_ClubSeller_No", new object[0]))
                                    }, "ClubSeller");
                                    return true;
                                }
                            }
                            else
                            {
                                if (!(text == "StorageBox"))
                                {
                                    return true;
                                }
                                this.openStorageBox(this.actionParamsToString(array));
                                return true;
                            }
                        }
                        else if (num <= 2815316590u)
                        {
                            if (num != 2802141700u)
                            {
                                if (num != 2815316590u)
                                {
                                    return true;
                                }
                                if (!(text == "MayorFridge"))
                                {
                                    return true;
                                }
                                int num2 = 0;
                                for (int j = 0; j < who.items.Count; j++)
                                {
                                    if (who.items[j] != null && who.items[j].parentSheetIndex == 284)
                                    {
                                        num2 += who.items[j].Stack;
                                    }
                                }
                                if (num2 >= 10 && !who.hasOrWillReceiveMail("TH_MayorFridge") && who.hasOrWillReceiveMail("TH_Railroad"))
                                {
                                    int k = 0;
                                    for (int l = 0; l < who.items.Count; l++)
                                    {
                                        if (who.items[l] != null && who.items[l].parentSheetIndex == 284)
                                        {
                                            while (k < 10)
                                            {
                                                Item expr_1581 = who.items[l];
                                                int stack = expr_1581.Stack;
                                                expr_1581.Stack = stack - 1;
                                                k++;
                                                if (who.items[l].Stack == 0)
                                                {
                                                    who.items[l] = null;
                                                    break;
                                                }
                                            }
                                            if (k >= 10)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    Game1.player.CanMove = false;
                                    Game1.playSound("coin");
                                    Game1.player.mailReceived.Add("TH_MayorFridge");
                                    Game1.multipleDialogues(new string[]
                                    {
                                        Game1.content.LoadString("Strings\\Locations:ManorHouse_MayorFridge_ConsumeBeets", new object[0]),
                                        Game1.content.LoadString("Strings\\Locations:ManorHouse_MayorFridge_MrQiNote", new object[0])
                                    });
                                    Game1.player.removeQuest(3);
                                    Game1.player.addQuest(4);
                                    return true;
                                }
                                if (who.hasOrWillReceiveMail("TH_MayorFridge"))
                                {
                                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_MayorFridge_MrQiNote", new object[0]));
                                    return true;
                                }
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ManorHouse_MayorFridge_Initial", new object[0]));
                                return true;
                            }
                            else
                            {
                                if (!(text == "Billboard"))
                                {
                                    return true;
                                }
                                Game1.activeClickableMenu = new Billboard(array[1].Equals("3"));
                                return true;
                            }
                        }
                        else if (num != 2817094304u)
                        {
                            if (num != 2832988535u)
                            {
                                if (num != 2909376585u)
                                {
                                    return true;
                                }
                                if (!(text == "Saloon"))
                                {
                                    return true;
                                }
                                if (who.getTileY() > tileLocation.Y)
                                {
                                    this.saloon(tileLocation);
                                    return true;
                                }
                                return true;
                            }
                            else
                            {
                                if (!(text == "AdventureShop"))
                                {
                                    return true;
                                }
                                this.adventureShop();
                                return true;
                            }
                        }
                        else
                        {
                            if (!(text == "Incubator"))
                            {
                                return true;
                            }
                            (this as AnimalHouse).incubator();
                            return true;
                        }
                    }
                    else if (num <= 3158608557u)
                    {
                        if (num <= 2959114096u)
                        {
                            if (num != 2920208772u)
                            {
                                if (num != 2959114096u)
                                {
                                    return true;
                                }
                                if (!(text == "GetLumber"))
                                {
                                    return true;
                                }
                                this.GetLumber();
                                return true;
                            }
                            else
                            {
                                if (!(text == "Message"))
                                {
                                    return true;
                                }
                                goto IL_1E36;
                            }
                        }
                        else if (num != 2987480683u)
                        {
                            if (num != 3155064199u)
                            {
                                if (num != 3158608557u)
                                {
                                    return true;
                                }
                                if (!(text == "Starpoint"))
                                {
                                    return true;
                                }
                                try
                                {
                                    this.doStarpoint(array[1]);
                                    return true;
                                }
                                catch (Exception)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (!(text == "farmstand"))
                                {
                                    return true;
                                }
                                Game1.shipHeldItem();
                                return true;
                            }
                        }
                        else
                        {
                            if (!(text == "Mailbox"))
                            {
                                return true;
                            }
                            this.mailbox();
                            return true;
                        }
                    }
                    else if (num <= 3244018497u)
                    {
                        if (num != 3162274371u)
                        {
                            if (num != 3211203767u)
                            {
                                if (num != 3244018497u)
                                {
                                    return true;
                                }
                                if (!(text == "WarpCommunityCenter"))
                                {
                                    return true;
                                }
                                if (who.mailReceived.Contains("ccDoorUnlock") || who.mailReceived.Contains("JojaMember"))
                                {
                                    Game1.warpFarmer("CommunityCenter", 32, 23, false);
                                    Game1.playSound("doorClose");
                                    return true;
                                }
                                Game1.drawObjectDialogue("It's locked.");
                                return true;
                            }
                            else
                            {
                                if (!(text == "HMTGF"))
                                {
                                    return true;
                                }
                                if (who.ActiveObject != null && who.ActiveObject != null && !who.ActiveObject.bigCraftable && who.ActiveObject.parentSheetIndex == 155)
                                {
                                    who.ActiveObject = new Object(Vector2.Zero, 155, false);
                                    Game1.playSound("discoverMineral");
                                    Game1.flashAlpha = 1f;
                                    return true;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            if (!(text == "MineSign"))
                            {
                                return true;
                            }
                            Game1.drawObjectDialogue(Game1.parseText(this.actionParamsToString(array)));
                            return true;
                        }
                    }
                    else if (num != 3327972754u)
                    {
                        if (num != 3329002772u)
                        {
                            if (num != 3371180897u)
                            {
                                return true;
                            }
                            if (!(text == "Craft"))
                            {
                                return true;
                            }
                            GameLocation.openCraftingMenu(this.actionParamsToString(array));
                            return true;
                        }
                        else
                        {
                            if (!(text == "Minecart"))
                            {
                                return true;
                            }
                            this.openChest(tileLocation, 4, Game1.random.Next(3, 7));
                            return true;
                        }
                    }
                    else
                    {
                        if (!(text == "TunnelSafe"))
                        {
                            return true;
                        }
                        if (who.ActiveObject != null && who.ActiveObject.parentSheetIndex == 787 && !who.hasOrWillReceiveMail("TH_Tunnel"))
                        {
                            who.reduceActiveItemByOne();
                            Game1.player.CanMove = false;
                            Game1.playSound("openBox");
                            DelayedAction.playSoundAfterDelay("doorCreakReverse", 500);
                            Game1.player.mailReceived.Add("TH_Tunnel");
                            Game1.multipleDialogues(new string[]
                            {
                                Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_ConsumeBattery", new object[0]),
                                Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_MrQiNote", new object[0])
                            });
                            Game1.player.addQuest(2);
                            return true;
                        }
                        if (who.hasOrWillReceiveMail("TH_Tunnel"))
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_MrQiNote", new object[0]));
                            return true;
                        }
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Tunnel_TunnelSafe_Initial", new object[0]));
                        return true;
                    }
                }
                else if (num <= 3912414904u)
                {
                    if (num <= 3424064554u)
                    {
                        if (num <= 3385614082u)
                        {
                            if (num != 3372476774u)
                            {
                                if (num != 3385614082u)
                                {
                                    return true;
                                }
                                if (!(text == "playSound"))
                                {
                                    return true;
                                }
                                goto IL_1F09;
                            }
                            else
                            {
                                if (!(text == "LumberPile"))
                                {
                                    return true;
                                }
                                if (!who.hasOrWillReceiveMail("TH_LumberPile") && who.hasOrWillReceiveMail("TH_SandDragon"))
                                {
                                    Game1.player.hasClubCard = true;
                                    Game1.player.CanMove = false;
                                    Game1.player.mailReceived.Add("TH_LumberPile");
                                    Game1.player.addItemByMenuIfNecessaryElseHoldUp(new SpecialItem(1, 2, ""), null);
                                    Game1.player.removeQuest(5);
                                    return true;
                                }
                                return true;
                            }
                        }
                        else if (num != 3403315211u)
                        {
                            if (num != 3419754368u)
                            {
                                if (num != 3424064554u)
                                {
                                    return true;
                                }
                                if (!(text == "Gunther"))
                                {
                                    return true;
                                }
                                this.gunther();
                                return true;
                            }
                            else
                            {
                                if (!(text == "Material"))
                                {
                                    return true;
                                }
                                Game1.drawObjectDialogue(string.Concat(new object[]
                                {
                                    "Material Stockpile: \n     ",
                                    who.WoodPieces,
                                    " pieces of lumber\n     ",
                                    who.StonePieces,
                                    " pieces of stone."
                                }));
                                return true;
                            }
                        }
                        else
                        {
                            if (!(text == "Buy"))
                            {
                                return true;
                            }
                            if (who.getTileY() >= tileLocation.Y)
                            {
                                this.openShopMenu(array[1]);
                                return true;
                            }
                            return true;
                        }
                    }
                    else if (num <= 3642125430u)
                    {
                        if (num != 3603749081u)
                        {
                            if (num != 3642125430u)
                            {
                                return true;
                            }
                            if (!(text == "ExitMine"))
                            {
                                return true;
                            }
                            Response[] answerChoices2 = new Response[]
                            {
                                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Mines_LeaveMine", new object[0])),
                                new Response("Go", Game1.content.LoadString("Strings\\Locations:Mines_GoUp", new object[0])),
                                new Response("Do", Game1.content.LoadString("Strings\\Locations:Mines_DoNothing", new object[0]))
                            };
                            this.createQuestionDialogue(" ", answerChoices2, "ExitMine");
                            return true;
                        }
                        else
                        {
                            if (!(text == "EnterSewer"))
                            {
                                return true;
                            }
                            if (who.hasRustyKey && !who.mailReceived.Contains("OpenedSewer"))
                            {
                                Game1.playSound("openBox");
                                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:Forest_OpenedSewer", new object[0])));
                                who.mailReceived.Add("OpenedSewer");
                                return true;
                            }
                            if (who.mailReceived.Contains("OpenedSewer"))
                            {
                                Game1.warpFarmer("Sewer", 16, 11, 2);
                                Game1.playSound("stairsdown");
                                return true;
                            }
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:LockedDoor", new object[0]));
                            return true;
                        }
                    }
                    else if (num != 3754457510u)
                    {
                        if (num != 3848897750u)
                        {
                            if (num != 3912414904u)
                            {
                                return true;
                            }
                            if (!(text == "DwarfGrave"))
                            {
                                return true;
                            }
                            if (who.canUnderstandDwarves)
                            {
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Town_DwarfGrave_Translated", new object[0]).Replace('\n', '^'));
                                return true;
                            }
                            Game1.drawObjectDialogue("Unop dunyuu doo pusutn snaus^Op hanp o toeday na doo smol^Vhu lonozol yenn huot olait tol");
                            return true;
                        }
                        else
                        {
                            if (!(text == "Mine"))
                            {
                                return true;
                            }
                            goto IL_21DE;
                        }
                    }
                    else
                    {
                        if (!(text == "SkullDoor"))
                        {
                            return true;
                        }
                        if (!who.hasSkullKey)
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SkullCave_SkullDoor_Locked", new object[0]));
                            return true;
                        }
                        if (!who.hasUnlockedSkullDoor)
                        {
                            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:SkullCave_SkullDoor_Unlock", new object[0])));
                            DelayedAction.playSoundAfterDelay("openBox", 500);
                            DelayedAction.playSoundAfterDelay("openBox", 700);
                            Game1.addMailForTomorrow("skullCave", false, false);
                            who.hasUnlockedSkullDoor = true;
                            who.completeQuest(19);
                            return true;
                        }
                        who.completelyStopAnimatingOrDoingAction();
                        Game1.playSound("doorClose");
                        DelayedAction.playSoundAfterDelay("stairsdown", 500);
                        Game1.enterMine(true, 121, null);
                        return true;
                    }
                }
                else if (num <= 4067857873u)
                {
                    if (num <= 3970342769u)
                    {
                        if (num != 3961338715u)
                        {
                            if (num != 3970342769u)
                            {
                                return true;
                            }
                            if (!(text == "ClubShop"))
                            {
                                return true;
                            }
                            Game1.activeClickableMenu = new ShopMenu(Utility.getQiShopStock(), 2, null);
                            return true;
                        }
                        else
                        {
                            if (!(text == "Lamp"))
                            {
                                return true;
                            }
                            if (this.lightLevel == 0f)
                            {
                                this.lightLevel = 0.6f;
                            }
                            else
                            {
                                this.lightLevel = 0f;
                            }
                            Game1.playSound("openBox");
                            return true;
                        }
                    }
                    else if (num != 3978811393u)
                    {
                        if (num != 4012092003u)
                        {
                            if (num != 4067857873u)
                            {
                                return true;
                            }
                            if (!(text == "EvilShrineLeft"))
                            {
                                return true;
                            }
                            if (who.getChildren().Count<Child>() == 0)
                            {
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineLeftInactive", new object[0]));
                                return true;
                            }
                            this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WitchHut_EvilShrineLeft", new object[0]), this.createYesNoResponses(), "evilShrineLeft");
                            return true;
                        }
                        else
                        {
                            if (!(text == "Arcade_Minecart"))
                            {
                                return true;
                            }
                            if (who.hasSkullKey)
                            {
                                Response[] answerChoices3 = new Response[]
                                {
                                    new Response("Progress", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_ProgressMode", new object[0])),
                                    new Response("Endless", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_EndlessMode", new object[0])),
                                    new Response("Exit", Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Exit", new object[0]))
                                };
                                this.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Menu", new object[0]), answerChoices3, "MinecartGame");
                                return true;
                            }
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Inactive", new object[0]));
                            return true;
                        }
                    }
                    else
                    {
                        if (!(text == "AnimalShop"))
                        {
                            return true;
                        }
                        if (who.getTileY() > tileLocation.Y)
                        {
                            this.animalShop(tileLocation);
                            return true;
                        }
                        return true;
                    }
                }
                else if (num <= 4097465949u)
                {
                    if (num != 4073653847u)
                    {
                        if (num != 4090469326u)
                        {
                            if (num != 4097465949u)
                            {
                                return true;
                            }
                            if (!(text == "EmilyRoomObject"))
                            {
                                return true;
                            }
                            if (!Game1.player.eventsSeen.Contains(463391) || (Game1.player.spouse != null && Game1.player.spouse.Equals("Emily")))
                            {
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:HaleyHouse_EmilyRoomObject", new object[0]));
                                return true;
                            }
                            TemporaryAnimatedSprite temporarySpriteByID = this.getTemporarySpriteByID(5858585);
                            if (temporarySpriteByID != null && temporarySpriteByID is EmilysParrot)
                            {
                                (temporarySpriteByID as EmilysParrot).doAction();
                                return true;
                            }
                            return true;
                        }
                        else
                        {
                            if (!(text == "RemoveChest"))
                            {
                                return true;
                            }
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:RemoveChest", new object[0]));
                            this.map.GetLayer("Buildings").Tiles[tileLocation.X, tileLocation.Y] = null;
                            return true;
                        }
                    }
                    else
                    {
                        if (!(text == "ItemChest"))
                        {
                            return true;
                        }
                        this.openItemChest(tileLocation, Convert.ToInt32(array[1]));
                        return true;
                    }
                }
                else if (num != 4104253281u)
                {
                    if (num != 4212892660u)
                    {
                        if (num != 4284593235u)
                        {
                            return true;
                        }
                        if (!(text == "MineWallDecor"))
                        {
                            return true;
                        }
                        this.getWallDecorItem(tileLocation);
                        return true;
                    }
                    else
                    {
                        if (!(text == "WarpWomensLocker"))
                        {
                            return true;
                        }
                        if (who.isMale)
                        {
                            if (who.IsMainPlayer)
                            {
                                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:WomensLocker_WrongGender", new object[0]));
                            }
                            return false;
                        }
                        who.faceGeneralDirection(new Vector2((float)tileLocation.X, (float)tileLocation.Y) * (float)Game1.tileSize, 0);
                        Game1.warpFarmer(array[3], Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), false);
                        if (array.Length < 5)
                        {
                            Game1.playSound("doorClose");
                            return true;
                        }
                        return true;
                    }
                }
                else
                {
                    if (!(text == "ElliottBook"))
                    {
                        return true;
                    }
                    if (who.eventsSeen.Contains(41))
                    {
                        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Locations:ElliottHouse_ElliottBook_Filled", new object[]
                        {
                            Game1.elliottBookName,
                            who.name
                        })));
                        return true;
                    }
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ElliottHouse_ElliottBook_Blank", new object[0]));
                    return true;
                }
                IL_EEF:
                Game1.activeClickableMenu = new ShopMenu(Utility.getJojaStock(), 0, null);
                return true;
                IL_1E36:
                Game1.drawDialogueNoTyping(this.actionParamsToString(array));
                return true;
                IL_1F09:
                Game1.playSound(array[1]);
                return true;
                IL_21DE:
                Game1.playSound("stairsdown");
                Game1.enterMine(this.name.Equals("Mine"), 1, null);
                return true;
                IL_25B2:
                this.farmerFile();
                return true;
            }
            if (action != null && !who.IsMainPlayer)
            {
                string text = action.ToString().Split(new char[]
                {
                    ' '
                })[0];
                if (!(text == "Minecart"))
                {
                    if (!(text == "RemoveChest"))
                    {
                        if (!(text == "Door"))
                        {
                            if (text == "TV")
                            {
                                Game1.tvStation = Game1.random.Next(2);
                            }
                        }
                        else
                        {
                            this.openDoor(tileLocation, true);
                        }
                    }
                    else
                    {
                        this.map.GetLayer("Buildings").Tiles[tileLocation.X, tileLocation.Y] = null;
                    }
                }
                else
                {
                    this.openChest(tileLocation, 4, Game1.random.Next(3, 7));
                }
            }
            return false;
        }
        */
        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, StardewValley.Farmer who)
        {
            //Log.AsyncG("BASLLS");

            if (this.currentEvent != null && this.currentEvent.isFestival)
            {
                return this.currentEvent.checkAction(tileLocation, viewport, who);
            }
            foreach (NPC current in this.characters)
            {
                if (current != null && !current.IsMonster && (!who.isRidingHorse() || !(current is Horse)) && current.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize)))
                {
                    bool result = current.checkAction(who, this);
                    return result;
                }
            }
            if (who.IsMainPlayer && who.currentUpgrade != null && this.name.Equals("Farm") && tileLocation.Equals(new Location((int)(who.currentUpgrade.positionOfCarpenter.X + (float)(Game1.tileSize / 2)) / Game1.tileSize, (int)(who.currentUpgrade.positionOfCarpenter.Y + (float)(Game1.tileSize / 2)) / Game1.tileSize)))
            {
                if (who.currentUpgrade.daysLeftTillUpgradeDone == 1)
                {
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin"), Game1.content.LoadString("Data\\ExtraDialogue:Farm_RobinWorking_ReadyTomorrow", new object[0]));
                }
                else
                {
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin"), Game1.content.LoadString("Data\\ExtraDialogue:Farm_RobinWorking" + (Game1.random.Next(2) + 1), new object[0]));
                }
            }
            Vector2 vector = new Vector2((float)tileLocation.X, (float)tileLocation.Y);
            if (this.objects.ContainsKey(vector) && this.objects[vector].Type != null)
            {
                if (who.isRidingHorse() && !(this.objects[vector] is Fence))
                {
                    return false;
                }
                if (vector.Equals(who.getTileLocation()) && !this.objects[vector].isPassable())
                {
                    Tool tool = new Pickaxe();
                    tool.DoFunction(Game1.currentLocation, -1, -1, 0, who);
                    if (this.objects[vector].performToolAction(tool))
                    {
                        this.objects[vector].performRemoveAction(this.objects[vector].tileLocation, Game1.currentLocation);
                        if ((this.objects[vector].type.Equals("Crafting") || this.objects[vector].Type.Equals("interactive")) && this.objects[vector].fragility != 2)
                        {
                            Game1.currentLocation.debris.Add(new Debris(this.objects[vector].bigCraftable ? (-this.objects[vector].ParentSheetIndex) : this.objects[vector].ParentSheetIndex, who.GetToolLocation(false), new Vector2((float)who.GetBoundingBox().Center.X, (float)who.GetBoundingBox().Center.Y)));
                        }
                        Game1.currentLocation.Objects.Remove(vector);
                        return true;
                    }
                    tool = new Axe();
                    tool.DoFunction(Game1.currentLocation, -1, -1, 0, who);
                    if (this.objects.ContainsKey(vector) && this.objects[vector].performToolAction(tool))
                    {
                        this.objects[vector].performRemoveAction(this.objects[vector].tileLocation, Game1.currentLocation);
                        if ((this.objects[vector].type.Equals("Crafting") || this.objects[vector].Type.Equals("interactive")) && this.objects[vector].fragility != 2)
                        {
                            Game1.currentLocation.debris.Add(new Debris(this.objects[vector].bigCraftable ? (-this.objects[vector].ParentSheetIndex) : this.objects[vector].ParentSheetIndex, who.GetToolLocation(false), new Vector2((float)who.GetBoundingBox().Center.X, (float)who.GetBoundingBox().Center.Y)));
                        }
                        Game1.currentLocation.Objects.Remove(vector);
                        return true;
                    }
                    if (!this.objects.ContainsKey(vector))
                    {
                        return true;
                    }
                }
                if (this.objects.ContainsKey(vector) && (this.objects[vector].Type.Equals("Crafting") || this.objects[vector].Type.Equals("interactive")))
                {
                    if (who.ActiveObject == null)
                    {
                        return this.objects[vector].checkForAction(who, false);
                    }
                    if (this.objects[vector].performObjectDropInAction(who.ActiveObject, false, who))
                    {
                        who.reduceActiveItemByOne();
                        return true;
                    }
                    return this.objects[vector].checkForAction(who, false);
                }
                else if (this.objects.ContainsKey(vector) && this.objects[vector].isSpawnedObject)
                {
                    int quality = this.objects[vector].quality;
                    if (who.professions.Contains(16))
                    {
                        this.objects[vector].quality = 2;
                    }
                    if (who.couldInventoryAcceptThisItem(this.objects[vector]))
                    {
                        this.objects[vector].quality = quality;
                        if (who.IsMainPlayer)
                        {
                            Game1.playSound("pickUpItem");
                            DelayedAction.playSoundAfterDelay("coin", 300);
                        }
                        who.animateOnce(279 + who.FacingDirection);
                        Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)vector.X + Game1.timeOfDay);
                        if (!this.isFarmBuildingInterior())
                        {
                            if (this.objects[vector].Category == -79 || this.objects[vector].Category == -81 || this.objects[vector].Category == -80 || this.objects[vector].Category == -75 || this is Beach)
                            {
                                who.gainExperience(2, 7);
                                if (who.professions.Contains(16))
                                {
                                    this.objects[vector].quality = 2;
                                }
                                else if (random.NextDouble() < (double)((float)who.ForagingLevel / 30f))
                                {
                                    this.objects[vector].quality = 2;
                                }
                                else if (random.NextDouble() < (double)((float)who.ForagingLevel / 15f))
                                {
                                    this.objects[vector].quality = 1;
                                }
                            }
                        }
                        else
                        {
                            who.gainExperience(0, 5);
                        }
                        who.addItemToInventoryBool(this.objects[vector].getOne(), false);
                        if (who.professions.Contains(13) && random.NextDouble() < 0.2 && !this.objects[vector].questItem && who.couldInventoryAcceptThisItem(this.objects[vector]) && !this.isFarmBuildingInterior())
                        {
                            who.addItemToInventoryBool(this.objects[vector].getOne(), false);
                            who.gainExperience(2, 7);
                        }
                        this.objects.Remove(vector);
                        return true;
                    }
                    this.objects[vector].quality = quality;
                }
            }
            if (who.isRidingHorse())
            {
                who.getMount().checkAction(who, this);
                return true;
            }
            if (this.terrainFeatures.ContainsKey(vector) && this.terrainFeatures[vector].GetType() == typeof(HoeDirt) && who.ActiveObject != null && (who.ActiveObject.Category == -74 || who.ActiveObject.Category == -19) && ((HoeDirt)this.terrainFeatures[vector]).canPlantThisSeedHere(who.ActiveObject.ParentSheetIndex, tileLocation.X, tileLocation.Y, who.ActiveObject.Category == -19))
            {
                if (((HoeDirt)this.terrainFeatures[vector]).plant(who.ActiveObject.ParentSheetIndex, tileLocation.X, tileLocation.Y, who, who.ActiveObject.Category == -19) && who.IsMainPlayer)
                {
                    who.reduceActiveItemByOne();
                }
                Game1.haltAfterCheck = false;
                return true;
            }
            Microsoft.Xna.Framework.Rectangle value = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
            foreach (KeyValuePair<Vector2, TerrainFeature> current2 in this.terrainFeatures)
            {
                if (current2.Value.getBoundingBox(current2.Key).Intersects(value))
                {
                    Game1.haltAfterCheck = false;
                    bool result = current2.Value.performUseAction(current2.Key);
                    return result;
                }
            }
            if (this.largeTerrainFeatures != null)
            {
                foreach (LargeTerrainFeature current3 in this.largeTerrainFeatures)
                {
                    if (current3.getBoundingBox().Intersects(value))
                    {
                        Game1.haltAfterCheck = false;
                        bool result = current3.performUseAction(current3.tilePosition);
                        return result;
                    }
                }
            }
            PropertyValue propertyValue = null;
            Tile tile = this.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize), viewport.Size);
            if (tile != null)
            {
                tile.Properties.TryGetValue("Action", out propertyValue);
            }
            return propertyValue != null && (this.currentEvent != null || this.isCharacterAtTile(vector + new Vector2(0f, 1f)) == null) && this.performAction(propertyValue, who, tileLocation);
        }



        public override void resetForPlayerEntry()
        {
            Utility.killAllStaticLoopingSoundCues();
            if (Game1.CurrentEvent == null && !this.Name.ToLower().Contains("bath"))
            {
                Game1.player.canOnlyWalk = false;
            }
            if (!(this is Farm))
            {
                this.temporarySprites.Clear();
            }
            if (Game1.options != null)
            {
                if (Game1.isOneOfTheseKeysDown(Keyboard.GetState(), Game1.options.runButton))
                {
                    Game1.player.setRunning(!Game1.options.autoRun, true);
                }
                else
                {
                    Game1.player.setRunning(Game1.options.autoRun, true);
                }
            }
            Game1.UpdateViewPort(false, new Point(Game1.player.getStandingX(), Game1.player.getStandingY()));
            Game1.previousViewportPosition = new Vector2((float)Game1.viewport.X, (float)Game1.viewport.Y);
            using (List<IClickableMenu>.Enumerator enumerator = Game1.onScreenMenus.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.gameWindowSizeChanged(new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height), new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
                }
            }
            if (Game1.player.rightRing != null)
            {
                Game1.player.rightRing.onNewLocation(Game1.player, this);
            }
            if (Game1.player.leftRing != null)
            {
                Game1.player.leftRing.onNewLocation(Game1.player, this);
            }
            this.forceViewportPlayerFollow = this.map.Properties.ContainsKey("ViewportFollowPlayer");
            this.lastTouchActionLocation = Game1.player.getTileLocation();
            for (int i = Game1.player.questLog.Count - 1; i >= 0; i--)
            {
                Game1.player.questLog[i].adjustGameLocation(this);
            }
            for (int j = this.characters.Count - 1; j >= 0; j--)
            {
                this.characters[j].behaviorOnFarmerLocationEntry(this, Game1.player);
            }
            if (!this.isOutdoors)
            {
                Game1.player.FarmerSprite.currentStep = "thudStep";
                if (this.doorSprites != null)
                {
                    foreach (KeyValuePair<Point, TemporaryAnimatedSprite> current in this.doorSprites)
                    {
                        current.Value.reset();
                        current.Value.paused = true;
                    }
                }
            }
            if (!this.isOutdoors || this.ignoreOutdoorLighting)
            {
                PropertyValue propertyValue;
                this.map.Properties.TryGetValue("AmbientLight", out propertyValue);
                if (propertyValue != null)
                {
                    string[] array = propertyValue.ToString().Split(new char[]
                    {
                        ' '
                    });
                    Game1.ambientLight = new Color(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]));
                }
                else if (Game1.isDarkOut() || this.lightLevel > 0f)
                {
                    Game1.ambientLight = new Color(180, 180, 0);
                }
                else
                {
                    Game1.ambientLight = Color.White;
                }
                if (Game1.bloom != null)
                {
                    Game1.bloom.Visible = false;
                }
                if (Game1.currentSong != null && Game1.currentSong.Name.Contains("ambient"))
                {
                    Game1.changeMusicTrack("none");
                }
            }
            else if (!(this is Desert))
            {
                Game1.ambientLight = (Game1.isRaining ? new Color(255, 200, 80) : Color.White);
                if (Game1.bloom != null)
                {
                    Game1.bloom.Visible = Game1.bloomDay;
                }
            }
            this.setUpLocationSpecificFlair();
            PropertyValue propertyValue2;
            this.map.Properties.TryGetValue("UniqueSprite", out propertyValue2);
            if (propertyValue2 != null)
            {
                string[] array2 = propertyValue2.ToString().Split(new char[]
                {
                    ' '
                });
                for (int k = 0; k < array2.Length; k++)
                {
                    NPC characterFromName = Game1.getCharacterFromName(array2[k]);
                    if (this.characters.Contains(characterFromName))
                    {
                        try
                        {
                            characterFromName.sprite.Texture = Game1.content.Load<Texture2D>("Characters\\" + characterFromName.name + "_" + this.name);
                            characterFromName.uniqueSpriteActive = true;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            PropertyValue propertyValue3;
            this.map.Properties.TryGetValue("UniquePortrait", out propertyValue3);
            if (propertyValue3 != null)
            {
                string[] array2 = propertyValue3.ToString().Split(new char[]
                {
                    ' '
                });
                for (int k = 0; k < array2.Length; k++)
                {
                    NPC characterFromName2 = Game1.getCharacterFromName(array2[k]);
                    if (this.characters.Contains(characterFromName2))
                    {
                        try
                        {
                            characterFromName2.Portrait = Game1.content.Load<Texture2D>("Portraits\\" + characterFromName2.name + "_" + this.name);
                            characterFromName2.uniquePortraitActive = true;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            PropertyValue propertyValue4;
            this.map.Properties.TryGetValue("Light", out propertyValue4);
            if (propertyValue4 != null && !this.ignoreLights)
            {
                string[] array3 = propertyValue4.ToString().Split(new char[]
                {
                    ' '
                });
                for (int l = 0; l < array3.Length; l += 3)
                {
                    Game1.currentLightSources.Add(new LightSource(Convert.ToInt32(array3[l + 2]), new Vector2((float)(Convert.ToInt32(array3[l]) * Game1.tileSize + Game1.tileSize / 2), (float)(Convert.ToInt32(array3[l + 1]) * Game1.tileSize + Game1.tileSize / 2)), 1f));
                }
            }
            if (this.isOutdoors)
            {
                PropertyValue propertyValue5;
                this.map.Properties.TryGetValue("BrookSounds", out propertyValue5);
                if (propertyValue5 != null)
                {
                    string[] array4 = propertyValue5.ToString().Split(new char[]
                    {
                        ' '
                    });
                    for (int m = 0; m < array4.Length; m += 3)
                    {
                        AmbientLocationSounds.addSound(new Vector2((float)Convert.ToInt32(array4[m]), (float)Convert.ToInt32(array4[m + 1])), Convert.ToInt32(array4[m + 2]));
                    }
                }
                Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
            }
            foreach (KeyValuePair<Vector2, TerrainFeature> current2 in this.terrainFeatures)
            {
                current2.Value.performPlayerEntryAction(current2.Key);
            }
            if (this.largeTerrainFeatures != null)
            {
                foreach (LargeTerrainFeature expr_6A2 in this.largeTerrainFeatures)
                {
                    expr_6A2.performPlayerEntryAction(expr_6A2.tilePosition);
                }
            }
            foreach (KeyValuePair<Vector2, StardewValley.Object> current3 in this.objects)
            {
                current3.Value.actionOnPlayerEntry();
            }
            if (this.isOutdoors && !Game1.eventUp && Game1.options.musicVolumeLevel > 0.025f && Game1.timeOfDay < 1200 && (Game1.currentSong == null || Game1.currentSong.Name.ToLower().Contains("ambient")))
            {
                Game1.playMorningSong();
            }
            if (!(this is MineShaft))
            {
                string a = Game1.currentSeason.ToLower();
                if (!(a == "spring"))
                {
                    if (!(a == "summer"))
                    {
                        if (!(a == "fall"))
                        {
                            if (a == "winter")
                            {
                                this.waterColor = new Color(130, 80, 255) * 0.5f;
                            }
                        }
                        else
                        {
                            this.waterColor = new Color(255, 130, 200) * 0.5f;
                        }
                    }
                    else
                    {
                        this.waterColor = new Color(60, 240, 255) * 0.5f;
                    }
                }
                else
                {
                    this.waterColor = new Color(120, 200, 255) * 0.5f;
                }
            }
            PropertyValue propertyValue6 = null;
            this.map.Properties.TryGetValue("Music", out propertyValue6);
            if (propertyValue6 != null)
            {
                string[] array5 = propertyValue6.ToString().Split(new char[]
                {
                    ' '
                });
                if (array5.Length > 1)
                {
                    if (Game1.timeOfDay >= Convert.ToInt32(array5[0]) && Game1.timeOfDay < Convert.ToInt32(array5[1]) && !array5[2].Equals(Game1.currentSong.Name))
                    {
                        Game1.changeMusicTrack(array5[2]);
                    }
                }
                else if (Game1.currentSong == null || Game1.currentSong.IsStopped || !array5[0].Equals(Game1.currentSong.Name))
                {
                    Game1.changeMusicTrack(array5[0]);
                }
            }
            if (this.isOutdoors)
            {
                ((FarmerSprite)Game1.player.Sprite).currentStep = "sandyStep";
                this.tryToAddCritters(false);
            }
            PropertyValue propertyValue7 = null;
            this.map.Properties.TryGetValue("Doors", out propertyValue7);
            if (propertyValue7 != null)
            {
                string[] array6 = propertyValue7.ToString().Split(new char[]
                {
                    ' '
                });
                for (int n = 0; n < array6.Length; n += 4)
                {
                    int x = Convert.ToInt32(array6[n]);
                    int y = Convert.ToInt32(array6[n + 1]);
                    Location location = new Location(x, y);
                    if (this.map.GetLayer("Buildings").Tiles[location] == null)
                    {
                        Tile tile = new StaticTile(this.map.GetLayer("Buildings"), this.map.GetTileSheet(array6[n + 2]), BlendMode.Alpha, Convert.ToInt32(array6[n + 3]));
                        tile.Properties.Add("Action", new PropertyValue("Door" + ((this.doorSprites[new Point(x, y)].endSound != null) ? (" " + this.doorSprites[new Point(x, y)].endSound) : "")));
                        this.map.GetLayer("Buildings").Tiles[location] = tile;
                        location.Y--;
                        this.map.GetLayer("Front").Tiles[location] = new StaticTile(this.map.GetLayer("Front"), this.map.GetTileSheet(array6[n + 2]), BlendMode.Alpha, Convert.ToInt32(array6[n + 3]) - this.map.GetTileSheet(array6[n + 2]).SheetWidth);
                        location.Y--;
                        this.map.GetLayer("Front").Tiles[location] = new StaticTile(this.map.GetLayer("Front"), this.map.GetTileSheet(array6[n + 2]), BlendMode.Alpha, Convert.ToInt32(array6[n + 3]) - this.map.GetTileSheet(array6[n + 2]).SheetWidth * 2);
                    }
                }
            }
            if (Game1.timeOfDay < 1900 && (!Game1.isRaining || this.name.Equals("SandyHouse")))
            {
                PropertyValue propertyValue8;
                this.map.Properties.TryGetValue("DayTiles", out propertyValue8);
                if (propertyValue8 != null)
                {
                    string[] array7 = propertyValue8.ToString().Trim().Split(new char[]
                    {
                        ' '
                    });
                    for (int num = 0; num < array7.Length; num += 4)
                    {
                        if (this.map.GetLayer(array7[num]).Tiles[Convert.ToInt32(array7[num + 1]), Convert.ToInt32(array7[num + 2])] != null)
                        {
                            this.map.GetLayer(array7[num]).Tiles[Convert.ToInt32(array7[num + 1]), Convert.ToInt32(array7[num + 2])].TileIndex = Convert.ToInt32(array7[num + 3]);
                        }
                    }
                }
            }
            else if (Game1.timeOfDay >= 1900 || (Game1.isRaining && !this.name.Equals("SandyHouse")))
            {
                if (!(this is MineShaft))
                {
                    this.lightGlows.Clear();
                }
                PropertyValue propertyValue9;
                this.map.Properties.TryGetValue("NightTiles", out propertyValue9);
                if (propertyValue9 != null)
                {
                    string[] array8 = propertyValue9.ToString().Split(new char[]
                    {
                        ' '
                    });
                    for (int num2 = 0; num2 < array8.Length; num2 += 4)
                    {
                        if (this.map.GetLayer(array8[num2]).Tiles[Convert.ToInt32(array8[num2 + 1]), Convert.ToInt32(array8[num2 + 2])] != null)
                        {
                            this.map.GetLayer(array8[num2]).Tiles[Convert.ToInt32(array8[num2 + 1]), Convert.ToInt32(array8[num2 + 2])].TileIndex = Convert.ToInt32(array8[num2 + 3]);
                        }
                    }
                }
            }
            if (this.name.Equals("Coop"))
            {
                using (List<CoopDweller>.Enumerator enumerator6 = Game1.player.coopDwellers.GetEnumerator())
                {
                    while (enumerator6.MoveNext())
                    {
                        enumerator6.Current.setRandomPosition();
                    }
                }
                string[] array9 = this.getMapProperty("Feed").Split(new char[]
                {
                    ' '
                });
                if (Game1.player.Feed <= 0)
                {
                    this.map.GetLayer("Buildings").Tiles[Convert.ToInt32(array9[0]), Convert.ToInt32(array9[1])].TileIndex = 35;
                }
                else
                {
                    this.map.GetLayer("Buildings").Tiles[Convert.ToInt32(array9[0]), Convert.ToInt32(array9[1])].TileIndex = 31;
                }
            }
            else if (this.name.Equals("Barn"))
            {
                using (List<BarnDweller>.Enumerator enumerator7 = Game1.player.barnDwellers.GetEnumerator())
                {
                    while (enumerator7.MoveNext())
                    {
                        enumerator7.Current.setRandomPosition();
                    }
                }
                string[] array10 = this.getMapProperty("Feed").Split(new char[]
                {
                    ' '
                });
                if (Game1.player.Feed <= 0)
                {
                    this.map.GetLayer("Buildings").Tiles[Convert.ToInt32(array10[0]), Convert.ToInt32(array10[1])].TileIndex = 35;
                }
                else
                {
                    this.map.GetLayer("Buildings").Tiles[Convert.ToInt32(array10[0]), Convert.ToInt32(array10[1])].TileIndex = 31;
                }
            }
            if (this.name.Equals("Mountain") && (Game1.timeOfDay < 2000 || !Game1.currentSeason.Equals("summer") || Game1.random.NextDouble() >= 0.3) && Game1.isRaining && !Game1.currentSeason.Equals("winter"))
            {
                Game1.random.NextDouble();
            }
            if (this.name.Equals("Club"))
            {
                Game1.changeMusicTrack("clubloop");
            }
            else if (Game1.currentSong != null && Game1.currentSong.Name.Equals("clubloop") && (Game1.nextMusicTrack == null || Game1.nextMusicTrack.Count<char>() == 0))
            {
                Game1.changeMusicTrack("none");
            }
            if (Game1.activeClickableMenu == null)
            {
                this.checkForEvents();
            }
        }

        public new void checkForEvents()
        {
            if (Game1.killScreen && !Game1.eventUp)
            {
                if (this.name.Equals("Mine"))
                {
                    string text;
                    string text2;
                    switch (Game1.random.Next(7))
                    {
                        case 0:
                            text = "Robin";
                            text2 = "Data\\ExtraDialogue:Mines_PlayerKilled_Robin";
                            goto IL_B8;
                        case 1:
                            text = "Clint";
                            text2 = "Data\\ExtraDialogue:Mines_PlayerKilled_Clint";
                            goto IL_B8;
                        case 2:
                            text = "Maru";
                            text2 = ((Game1.player.spouse != null && Game1.player.spouse.Equals("Maru")) ? "Data\\ExtraDialogue:Mines_PlayerKilled_Maru_Spouse" : "Data\\ExtraDialogue:Mines_PlayerKilled_Maru_NotSpouse");
                            goto IL_B8;
                    }
                    text = "Linus";
                    text2 = "Data\\ExtraDialogue:Mines_PlayerKilled_Linus";
                    IL_B8:
                    if (Game1.random.NextDouble() < 0.1 && Game1.player.spouse != null && !Game1.player.spouse.Contains("engaged") && Game1.player.spouse.Length > 1)
                    {
                        text = Game1.player.spouse;
                        text2 = (Game1.player.isMale ? "Data\\ExtraDialogue:Mines_PlayerKilled_Spouse_PlayerMale" : "Data\\ExtraDialogue:Mines_PlayerKilled_Spouse_PlayerFemale");
                    }
                    this.currentEvent = new Event(Game1.content.LoadString("Data\\Events\\Mine:PlayerKilled", new object[]
                    {
                        text,
                        text2,
                        Game1.player.name
                    }), -1);
                }
                else if (this.name.Equals("Hospital"))
                {
                    this.currentEvent = new Event(Game1.content.LoadString("Data\\Events\\Hospital:PlayerKilled", new object[]
                    {
                        Game1.player.name
                    }), -1);
                }
                Game1.eventUp = true;
                Game1.killScreen = false;
                Game1.player.health = 10;
                return;
            }
            if (!Game1.eventUp && Game1.farmEvent == null)
            {
                string festival = Game1.currentSeason + Game1.dayOfMonth;
                try
                {
                    Event @event = new Event();
                    if (@event.tryToLoadFestival(festival))
                    {
                        this.currentEvent = @event;
                    }
                }
                catch (Exception)
                {
                }
                if (!Game1.eventUp && this.currentEvent == null)
                {
                    Dictionary<string, string> dictionary = null;
                    try
                    {
                        dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + this.name);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    if (dictionary != null)
                    {
                        string[] array = dictionary.Keys.ToArray<string>();
                        for (int i = 0; i < array.Length; i++)
                        {
                            int num = this.checkEventPrecondition(array[i]);
                            if (num != -1)
                            {
                                this.currentEvent = new Event(dictionary[array[i]], num);
                                break;
                            }
                        }
                        if (this.currentEvent == null && this.name.Equals("Farm") && !Game1.player.mailReceived.Contains("rejectedPet") && Game1.stats.DaysPlayed >= 20u && !Game1.player.hasPet())
                        {
                            for (int j = 0; j < array.Length; j++)
                            {
                                if ((array[j].Contains("dog") && !Game1.player.catPerson) || (array[j].Contains("cat") && Game1.player.catPerson))
                                {
                                    this.currentEvent = new Event(dictionary[array[j]], -1);
                                    Game1.player.eventsSeen.Add(Convert.ToInt32(array[j].Split(new char[]
                                    {
                                        '/'
                                    })[0]));
                                    break;
                                }
                            }
                        }
                    }
                }
                if (this.currentEvent != null)
                {
                    if (Game1.player.getMount() != null)
                    {
                        this.currentEvent.playerWasMounted = true;
                        Game1.player.getMount().dismount();
                    }
                    using (List<NPC>.Enumerator enumerator = this.characters.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.clearTextAboveHead();
                        }
                    }
                    Game1.eventUp = true;
                    Game1.displayHUD = false;
                    Game1.player.CanMove = false;
                    Game1.player.showNotCarrying();
                    if (this.critters != null)
                    {
                        this.critters.Clear();
                    }
                }
            }
        }
        private int checkEventPrecondition(string precondition)
        {
            string[] array = precondition.Split(new char[]
            {
                '/'
            });
            try
            {
                if (Game1.player.eventsSeen.Contains(Convert.ToInt32(array[0])))
                {
                    int result = -1;
                    return result;
                }
            }
            catch (Exception)
            {
                int result = -1;
                return result;
            }
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i][0] == 'e')
                {
                    if (this.checkEventsSeenPreconditions(array[i].Split(new char[]
                    {
                        ' '
                    })))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'h')
                {
                    if (Game1.player.hasPet())
                    {
                        return -1;
                    }
                    if ((Game1.player.catPerson && !array[i].Split(new char[]
                    {
                        ' '
                    })[1].ToString().ToLower().Equals("cat")) || (!Game1.player.catPerson && !array[i].Split(new char[]
                    {
                        ' '
                    })[1].ToString().ToLower().Equals("dog")))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'm')
                {
                    string[] array2 = array[i].Split(new char[]
                    {
                        ' '
                    });
                    if ((ulong)Game1.player.totalMoneyEarned < (ulong)((long)Convert.ToInt32(array2[1])))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'M')
                {
                    string[] array3 = array[i].Split(new char[]
                    {
                        ' '
                    });
                    if (Game1.player.money < Convert.ToInt32(array3[1]))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'c')
                {
                    if (Game1.player.freeSpotsInInventory() < Convert.ToInt32(array[i].Split(new char[]
                    {
                        ' '
                    })[1]))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'C')
                {
                    if (!Game1.player.eventsSeen.Contains(191393) && !Game1.player.eventsSeen.Contains(502261) && !Game1.player.hasCompletedCommunityCenter())
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'j')
                {
                    if ((ulong)Game1.stats.DaysPlayed <= (ulong)((long)Convert.ToInt32(array[i].Split(new char[]
                    {
                        ' '
                    })[1])))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'f')
                {
                    if (!this.checkFriendshipPrecondition(array[i].Split(new char[]
                    {
                        ' '
                    })))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'r')
                {
                    string[] array4 = array[i].Split(new char[]
                    {
                        ' '
                    });
                    if (Game1.random.NextDouble() > Convert.ToDouble(array4[1]))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 's')
                {
                    if (!this.checkItemsPrecondition(array[i].Split(new char[]
                    {
                        ' '
                    })))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'q')
                {
                    if (!this.checkDialoguePrecondition(array[i].Split(new char[]
                    {
                        ' '
                    })))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'n')
                {
                    if (!Game1.player.mailReceived.Contains(array[i].Split(new char[]
                    {
                        ' '
                    })[1]))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'l')
                {
                    if (Game1.player.mailReceived.Contains(array[i].Split(new char[]
                    {
                        ' '
                    })[1]))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 't')
                {
                    string[] array5 = array[i].Split(new char[]
                    {
                        ' '
                    });
                    if (Game1.timeOfDay < Convert.ToInt32(array5[1]) || Game1.timeOfDay > Convert.ToInt32(array5[2]))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'w')
                {
                    string[] array6 = array[i].Split(new char[]
                    {
                        ' '
                    });
                    if ((array6[1].Equals("rainy") && !Game1.isRaining) || (array6[1].Equals("sunny") && Game1.isRaining))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'd')
                {
                    if (array[i].Split(new char[]
                    {
                        ' '
                    }).Contains(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'o')
                {
                    if (Game1.player.spouse != null && Game1.player.spouse.Equals(array[i].Split(new char[]
                    {
                        ' '
                    })[1]))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'v')
                {
                    if (Game1.getCharacterFromName(array[i].Split(new char[]
                    {
                        ' '
                    })[1]).isInvisible)
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'p')
                {
                    string[] array7 = array[i].Split(new char[]
                    {
                        ' '
                    });
                    if (!this.isCharacterHere(array7[1]))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'z')
                {
                    string[] array8 = array[i].Split(new char[]
                    {
                        ' '
                    });
                    if (Game1.currentSeason.Equals(array8[1]))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'b')
                {
                    string[] array9 = array[i].Split(new char[]
                    {
                        ' '
                    });
                    if (Game1.player.timesReachedMineBottom < Convert.ToInt32(array9[1]))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'y')
                {
                    if (Game1.year < Convert.ToInt32(array[i].Split(new char[]
                    {
                        ' '
                    })[1]) || (Convert.ToInt32(array[i].Split(new char[]
                    {
                        ' '
                    })[1]) == 1 && Game1.year != 1))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'g')
                {
                    if (!(Game1.player.isMale ? "male" : "female").Equals(array[i].Split(new char[]
                    {
                        ' '
                    })[1].ToLower()))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'i')
                {
                    if (!Game1.player.hasItemInInventory(Convert.ToInt32(array[i].Split(new char[]
                    {
                        ' '
                    })[1]), 1, 0) && (Game1.player.ActiveObject == null || Game1.player.ActiveObject.ParentSheetIndex != Convert.ToInt32(array[i].Split(new char[]
                    {
                        ' '
                    })[1])))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'k')
                {
                    if (!this.checkEventsSeenPreconditions(array[i].Split(new char[]
                    {
                        ' '
                    })))
                    {
                        return -1;
                    }
                }
                else if (array[i][0] == 'a')
                {
                    if (Game1.player.getTileLocation().X != (float)Convert.ToInt32(array[i].Split(new char[]
                    {
                        ' '
                    })[1]) || Game1.player.getTileLocation().Y != (float)Convert.ToInt32(array[i].Split(new char[]
                    {
                        ' '
                    })[2]))
                    {
                        return -1;
                    }
                }
                else
                {
                    if (array[i][0] == 'x')
                    {
                        Game1.addMailForTomorrow(array[i].Split(new char[]
                        {
                            ' '
                        })[1], false, false);
                        Game1.player.eventsSeen.Add(Convert.ToInt32(array[0]));
                        return -1;
                    }
                    if (array[i][0] != 'u')
                    {
                        return -1;
                    }
                    bool flag = false;
                    string[] array10 = array[i].Split(new char[]
                    {
                        ' '
                    });
                    for (int j = 1; j < array10.Length; j++)
                    {
                        if (Game1.dayOfMonth == Convert.ToInt32(array10[j]))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        return -1;
                    }
                }
            }
            return Convert.ToInt32(array[0]);
        }
        private bool checkEventsSeenPreconditions(string[] eventIDs)
        {
            for (int i = 1; i < eventIDs.Length; i++)
            {
                int num;
                if (int.TryParse(eventIDs[i], out num) && Game1.player.eventsSeen.Contains(Convert.ToInt32(eventIDs[i])))
                {
                    return false;
                }
            }
            return true;
        }
        private bool checkFriendshipPrecondition(string[] friendshipString)
        {
            for (int i = 1; i < friendshipString.Length; i += 2)
            {
                if (!Game1.player.friendships.ContainsKey(friendshipString[i]) || Game1.player.friendships[friendshipString[i]][0] < Convert.ToInt32(friendshipString[i + 1]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool checkItemsPrecondition(string[] itemString)
        {
            for (int i = 1; i < itemString.Length; i += 2)
            {
                if (!Game1.player.basicShipped.ContainsKey(Convert.ToInt32(itemString[i])) || Game1.player.basicShipped[Convert.ToInt32(itemString[i])] < Convert.ToInt32(itemString[i + 1]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool checkDialoguePrecondition(string[] dialogueString)
        {
            for (int i = 1; i < dialogueString.Length; i += 2)
            {
                if (!Game1.player.DialogueQuestionsAnswered.Contains(Convert.ToInt32(dialogueString[i])))
                {
                    return false;
                }
            }
            return true;
        }

        private bool isCharacterHere(string name)
        {
            using (List<NPC>.Enumerator enumerator = this.characters.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.name.Equals(name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    
}
