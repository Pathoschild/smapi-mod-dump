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
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using Omegasis.StardustCore.Networking;
using StardewValley;
using StardewValley.Network;

namespace Omegasis.Revitalize.Framework.Illuminate
{
    /// <summary>
    /// Deals with handling lights on custom objects.
    /// </summary>
    public class LightManager : NetObject
    {
        [XmlIgnore]

        public readonly NetVector2Dictionary<LightSource, NetRef<LightSource>> lights = new NetVector2Dictionary<LightSource, NetRef<LightSource>>();

        /// <summary>
        /// Used to recreate lights at run time.
        /// </summary>
        public readonly NetVector2Dictionary<FakeLightSource, NetFakeLightSource> fakeLights;
        /// <summary>
        /// Are the lights on this object on?
        /// </summary>
        public readonly NetBool lightsOn = new NetBool();

        /// <summary>
        /// Magic number for positioning.
        /// </summary>
        public const int lightBigNumber = 1000000;


        /// <summary>
        /// Constructor.
        /// </summary>
        public LightManager()
        {
            this.lights = new NetVector2Dictionary<LightSource, NetRef<LightSource>>();
            this.fakeLights = new NetVector2Dictionary<FakeLightSource, NetFakeLightSource>();
            this.lightsOn.Value = false;
            this.initializeNetFields();
        }

        protected override void initializeNetFields()
        {
            this.NetFields.AddFields(this.lights,
                this.fakeLights,
                this.lightsOn);

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
            return true;
        }

        /// <summary>Add a light source to this location.</summary>
        /// <param name="environment">The game location to add the light source in.</param>
        public virtual void turnOnLights(GameLocation environment, StardewValley.Object gameObject)
        {
            if (this.lights.Count() < this.fakeLights.Count())
                this.regenerateRealLightsFromFakeLights();
            foreach (KeyValuePair<Vector2, LightSource> pair in this.lights.Pairs)
                this.turnOnLight(pair.Key, environment, gameObject);
            this.repositionLights(gameObject);
        }

        /// <summary>Removes a lightsource from the game location.</summary>
        /// <param name="environment">The game location to remove the light source from.</param>
        public void turnOffLights(GameLocation environment)
        {
            if (this.lights.Count() < this.fakeLights.Count())
                this.regenerateRealLightsFromFakeLights();
            foreach (KeyValuePair<Vector2, LightSource> pair in this.lights.Pairs)
                this.turnOffLight(pair.Key, environment);
        }

        /// <summary>
        /// Repositions all lights for this object.
        /// </summary>
        /// <param name="gameObject"></param>
        public void repositionLights(StardewValley.Object gameObject)
        {
            if (this.lights.Count() < this.fakeLights.Count())
                this.regenerateRealLightsFromFakeLights();
            foreach (KeyValuePair<Vector2, LightSource> pair in this.lights.Pairs)
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
        }

        /// <summary>
        /// Toggles the lights for this object.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="gameObject"></param>
        public virtual void toggleLights(GameLocation location, StardewValley.Object gameObject)
        {
            if (this.lights.Count() < this.fakeLights.Count())
                this.regenerateRealLightsFromFakeLights();
            if (!this.lightsOn)
            {
                this.turnOnLights(location, gameObject);
                this.lightsOn.Value = true;
                return;
            }
            else if (this.lightsOn)
            {
                this.turnOffLights(Game1.player.currentLocation);
                this.lightsOn.Value = false;
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
                foreach (KeyValuePair<Vector2, FakeLightSource> light in this.fakeLights.Pairs)
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


        public virtual void regenerateRealLightsFromFakeLights()
        {
            //ModCore.log("Info for file"+Path.GetFileNameWithoutExtension(file)+" has this many lights: " + info.info.lightManager.fakeLights.Count);
            this.lights.Clear();
            foreach (KeyValuePair<Vector2, FakeLightSource> light in this.fakeLights.Pairs)
            {
                Vector2 position = light.Value.positionOffset;
                position -= light.Key;
                position /= Game1.tileSize;
                position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));
                this.addLight(light.Key, new LightSource(light.Value.id, new Vector2(0, 0), light.Value.radius, light.Value.color.Invert()), position);
            }
        }

        public static LightSource CreateLightSource(float Radius, Color ActualColor, int Texture = 4)
        {
            return new LightSource(Texture, new Vector2(0, 0), Radius, ActualColor.Invert());
        }

    }
}
