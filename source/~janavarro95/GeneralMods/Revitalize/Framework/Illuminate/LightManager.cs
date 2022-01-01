/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;

namespace Revitalize.Framework.Illuminate
{
    /// <summary>
    /// Deals with handling lights on custom objects.
    /// </summary>
    public class LightManager
    {
        /// <summary>
        /// The lights held by this object.
        /// </summary>
        public Dictionary<Vector2, LightSource> lights;
        /// <summary>
        /// Used to recreate lights at run time.
        /// </summary>
        public Dictionary<Vector2, FakeLightSource> fakeLights;
        /// <summary>
        /// Are the lights on this object on?
        /// </summary>
        public bool lightsOn;

        /// <summary>
        /// Magic number for positioning.
        /// </summary>
        public const int lightBigNumber = 1000000;

        [JsonIgnore]
        public bool requiresUpdate;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LightManager()
        {
            this.lights = new Dictionary<Vector2, LightSource>();
            this.fakeLights = new Dictionary<Vector2, FakeLightSource>();
            this.lightsOn = false;
        }

        /// <summary>Add a light to the list of tracked lights.</summary>
        public bool addLight(Vector2 IdKey, LightSource light, StardewValley.Object gameObject)
        {
            if (gameObject.TileLocation.X < 0) gameObject.TileLocation = new Vector2(gameObject.TileLocation.X * -1, gameObject.TileLocation.Y);
            if (gameObject.TileLocation.Y < 0) gameObject.TileLocation = new Vector2(gameObject.TileLocation.X, gameObject.TileLocation.Y * -1);

            Vector2 initialPosition = gameObject.TileLocation * Game1.tileSize;
            initialPosition += IdKey;

            if (this.lights.ContainsKey(IdKey))
                return false;

            light.position.Value = initialPosition;
            this.lights.Add(IdKey, light);
            if (this.fakeLights.ContainsKey(IdKey)) return true;
            this.fakeLights.Add(IdKey, new FakeLightSource(light.Identifier, light.position.Value, light.color.Value.Invert(), light.radius.Value));
            this.requiresUpdate = true;
            return true;
        }

        /// <summary>
        /// Adds in a light at the given tile location in the world.
        /// </summary>
        /// <param name="IdKey"></param>
        /// <param name="light"></param>
        /// <param name="gameObjectTileLocation"></param>
        /// <returns></returns>
        public bool addLight(Vector2 IdKey, LightSource light, Vector2 gameObjectTileLocation)
        {
            if (gameObjectTileLocation.X < 0) gameObjectTileLocation = new Vector2(gameObjectTileLocation.X * -1, gameObjectTileLocation.Y);
            if (gameObjectTileLocation.Y < 0) gameObjectTileLocation = new Vector2(gameObjectTileLocation.X, gameObjectTileLocation.Y * -1);

            Vector2 initialPosition = gameObjectTileLocation * Game1.tileSize;
            initialPosition += IdKey;

            if (this.lights.ContainsKey(IdKey))
                return false;

            light.position.Value = initialPosition;
            this.lights.Add(IdKey, light);
            if (this.fakeLights.ContainsKey(IdKey)) return true;
            this.fakeLights.Add(IdKey, new FakeLightSource(light.Identifier, light.position.Value, light.color.Value.Invert(), light.radius.Value));
            this.requiresUpdate = true;
            return true;
        }

        /// <summary>Turn off a single light.</summary>
        public bool turnOffLight(Vector2 IdKey, GameLocation location)
        {
            if (!this.lights.ContainsKey(IdKey))
                return false;

            this.lights.TryGetValue(IdKey, out LightSource light);
            Game1.currentLightSources.Remove(light);
            location.sharedLights.Remove((int)IdKey.X * lightBigNumber + (int)IdKey.Y);
            return true;
        }

        /// <summary>Turn on a single light.</summary>
        public bool turnOnLight(Vector2 IdKey, GameLocation location, StardewValley.Object gameObject)
        {
            if (!this.lights.ContainsKey(IdKey))
                return false;

            this.lights.TryGetValue(IdKey, out var light);
            if (light == null)
                throw new Exception("Light is null????");

            Game1.currentLightSources.Add(light);
            if (location == null)
                throw new Exception("WHY IS LOC NULL???");

            if (location.sharedLights == null)
                throw new Exception("Locational lights is null!");


            if (light.lightTexture == null)
                light.lightTexture = this.loadTextureFromConstantValue(light.Identifier);

            Game1.currentLightSources.Add(light);
            location.sharedLights.Add((int)IdKey.X * lightBigNumber + (int)IdKey.Y, light);
            this.repositionLight(light, IdKey, gameObject);
            this.requiresUpdate = true;
            return true;
        }

        /// <summary>Add a light source to this location.</summary>
        /// <param name="environment">The game location to add the light source in.</param>
        public virtual void turnOnLights(GameLocation environment, StardewValley.Object gameObject)
        {
            foreach (KeyValuePair<Vector2, LightSource> pair in this.lights)
                this.turnOnLight(pair.Key, environment, gameObject);
            this.repositionLights(gameObject);
        }

        /// <summary>Removes a lightsource from the game location.</summary>
        /// <param name="environment">The game location to remove the light source from.</param>
        public void turnOffLights(GameLocation environment)
        {
            foreach (KeyValuePair<Vector2, LightSource> pair in this.lights)
                this.turnOffLight(pair.Key, environment);
        }

        /// <summary>
        /// Repositions all lights for this object.
        /// </summary>
        /// <param name="gameObject"></param>
        public void repositionLights(StardewValley.Object gameObject)
        {
            foreach (KeyValuePair<Vector2, LightSource> pair in this.lights)
                this.repositionLight(pair.Value, pair.Key, gameObject);
        }

        /// <summary>
        /// Reposition a light for this object.
        /// </summary>
        /// <param name="light"></param>
        /// <param name="offset"></param>
        /// <param name="gameObject"></param>
        public void repositionLight(LightSource light, Vector2 offset, StardewValley.Object gameObject)
        {
            Vector2 initialPosition = gameObject.TileLocation * Game1.tileSize;
            light.position.Value = initialPosition + offset;
            this.requiresUpdate = true;
        }

        /// <summary>
        /// Toggles the lights for this object.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="gameObject"></param>
        public virtual void toggleLights(GameLocation location, StardewValley.Object gameObject)
        {
            if (!this.lightsOn)
            {
                this.turnOnLights(location, gameObject);
                this.lightsOn = true;
                return;
            }
            else if (this.lightsOn)
            {
                this.turnOffLights(Game1.player.currentLocation);
                this.lightsOn = false;
                return;
            }
        }

        /// <summary>
        /// Removes the lights from the world when this object needs to be cleaned up.
        /// </summary>
        /// <param name="loc"></param>
        public virtual void removeForCleanUp(GameLocation loc)
        {
            this.turnOffLights(loc);
        }

        /// <summary>
        /// Loads in the appropriate texture from sdv depending on the int value used.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Texture2D loadTextureFromConstantValue(int value)
        {
            switch (value)
            {
                case 1:
                    return Game1.lantern;
                    break;
                case 2:
                    return Game1.windowLight;
                    break;
                case 4:
                    return Game1.sconceLight;
                    break;
                case 5:
                    return Game1.cauldronLight;
                    break;
                case 6:
                    return Game1.indoorWindowLight;
                    break;
            }
            return Game1.sconceLight;
        }

        /// <summary>
        /// Gets a copy of all of the 
        /// </summary>
        /// <returns></returns>
        public LightManager Copy()
        {
            LightManager copy = new LightManager();
            if (this.lights != null)
            {
                //ModCore.log("Info for file"+Path.GetFileNameWithoutExtension(file)+" has this many lights: " + info.info.lightManager.fakeLights.Count);
                copy.lights.Clear();
                foreach (KeyValuePair<Vector2, FakeLightSource> light in this.fakeLights)
                {
                    Vector2 position = light.Value.positionOffset;
                    position -= light.Key;
                    position /= Game1.tileSize;
                    position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));
                    copy.addLight(light.Key, new LightSource(light.Value.id, new Vector2(0, 0), light.Value.radius, light.Value.color.Invert()), position);
                }
            }
            return copy;
        }

        public static LightSource CreateLightSource(float Radius, Color ActualColor, int Texture = 4)
        {
            return new LightSource(Texture, new Vector2(0, 0), Radius, ActualColor.Invert());
        }

    }
}
