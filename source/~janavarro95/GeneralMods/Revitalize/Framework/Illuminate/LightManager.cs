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
    /// Note that since lighting in Stardew Valley seems to work as a subtraction value, that's the reason many methods here do color inversions.
    /// <see cref="addLightToTileLocation(Vector2, GameLocation, LightManager.LightIdentifier, Color, Vector2, float)"/> for a version that takes the actual color passed in.
    /// </summary>
    public class LightManager : NetObject
    {
        /// <summary>
        /// The light souces for this LightManager. The key is the positional pixel offset for the light source.
        /// </summary>
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
        /// Different light shapes supported by the base game.
        /// </summary>
        public enum LightIdentifier
        {

            Lantern = 1,

            WindowLight = 2,

            SconceLight = 4,

            CauldronLight = 5,

            IndoorWindowLight = 6,
        }

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

        /// <summary>
        /// Adds in a light at the given tile location in the world.
        /// </summary>
        /// <param name="IdKey"></param>
        /// <param name="light"></param>
        /// <param name="tileLocation"></param>
        /// <returns></returns>
        public bool addLightToTileLocation(Vector2 IdKey, GameLocation gameLocation ,LightSource light, Vector2 tileLocation)
        {
            if (this.fakeLights.ContainsKey(IdKey))
            {
                this.turnOnLight(IdKey, gameLocation, tileLocation);
                return true;
            }

            if (tileLocation.X < 0) tileLocation = new Vector2(tileLocation.X * -1, tileLocation.Y);
            if (tileLocation.Y < 0) tileLocation = new Vector2(tileLocation.X, tileLocation.Y * -1);


            Vector2 initialPosition = tileLocation * Game1.tileSize;
            initialPosition += IdKey;

            if (this.lights.ContainsKey(IdKey))
                return false;

            light.position.Value = initialPosition;
            this.lights.Add(IdKey, light);
            if (!this.fakeLights.ContainsKey(IdKey))
            {
                this.fakeLights.Add(IdKey, new FakeLightSource(light.Identifier, light.position.Value, light.color.Value.Invert(), light.radius.Value));
            }
            this.turnOnLight(IdKey, gameLocation, tileLocation);
            return true;
        }

        /// <summary>
        /// Adds in a light source to the world.
        /// </summary>
        /// <param name="IdKey">The identifier key to find for the light</param>
        /// <param name="lightShape"></param>
        /// <param name="DesiredColor"></param>
        /// <param name="TileLocation"></param>
        /// <param name="LightRadius"></param>
        /// <returns></returns>
        public bool addLightToTileLocation(Vector2 IdKey, GameLocation gameLocation ,LightIdentifier lightShape, Color DesiredColor, Vector2 TileLocation, float LightRadius)
        {
            return this.addLightToTileLocation(IdKey,gameLocation ,new LightSource((int)lightShape, TileLocation, LightRadius, DesiredColor.Invert()), TileLocation);
        }

        /// <summary>
        /// Removes a light 
        /// </summary>
        /// <param name="IdKey"></param>
        /// <returns></returns>
        public bool removeLight(Vector2 IdKey, GameLocation location)
        {
            this.turnOffLight(IdKey, location);
            this.fakeLights.Remove(IdKey);
            return this.lights.Remove(IdKey);
        }

        /// <summary>
        /// Removes all of the lights associated with this light manager.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool removeLights(GameLocation location)
        {
            List<Vector2> lightIds = new List<Vector2>(this.fakeLights.Keys);
            bool anyRemoved = false;
            foreach (Vector2 lightId in lightIds)
            {
                bool removed = this.removeLight(lightId, location);
                if (anyRemoved == false && removed == true)
                {
                    anyRemoved = true;
                }
            }
            return anyRemoved;
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
        public bool turnOnLight(Vector2 IdKey, GameLocation location, Vector2 TilePosition)
        {

            if (location == null)
            {
                return false;
            }

            if (!this.lights.ContainsKey(IdKey))
                return false;


            this.lights.TryGetValue(IdKey, out var light);

            Game1.currentLightSources.Add(light);

            if (light.lightTexture == null)
                light.lightTexture = this.loadTextureFromConstantValue(light.Identifier);
            if (!Game1.currentLightSources.Contains(light))
            {
                Game1.currentLightSources.Add(light);
            }
            //Light is already displayed at the shared location.
            if (!location.sharedLights.ContainsKey((int)IdKey.X * lightBigNumber + (int)IdKey.Y))
            {
                location.sharedLights.Add((int)IdKey.X * lightBigNumber + (int)IdKey.Y, light);
            }
            this.repositionLight(light, IdKey, TilePosition);
            return true;
        }

        /*
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
        */

        /// <summary>
        /// Repositions all lights for this object.
        /// </summary>
        /// <param name="gameObject"></param>
        public void repositionLights(Vector2 newTilePosition, GameLocation gameLocation)
        {
            if (this.lights.Count() < this.fakeLights.Count())
                this.regenerateRealLightsFromFakeLights(gameLocation);
            foreach (KeyValuePair<Vector2, LightSource> pair in this.lights.Pairs)
                this.repositionLight(pair.Value, pair.Key, newTilePosition);
        }

        /// <summary>
        /// Reposition a light for this object.
        /// </summary>
        /// <param name="light"></param>
        /// <param name="offset"></param>
        /// <param name="gameObject"></param>
        public void repositionLight(LightSource light, Vector2 offset, Vector2 NewTilePosition)
        {
            Vector2 initialPosition = NewTilePosition * Game1.tileSize;
            light.position.Value = initialPosition + offset;
        }

        /*
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
        */

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
                case 2:
                    return Game1.windowLight;
                case 4:
                    return Game1.sconceLight;
                case 5:
                    return Game1.cauldronLight;
                case 6:
                    return Game1.indoorWindowLight;
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
                    copy.addLightToTileLocation(light.Key,null ,new LightSource(light.Value.id, new Vector2(0, 0), light.Value.radius, light.Value.color.Invert()), position);
                }
            }
            return copy;
        }


        public virtual void regenerateRealLightsFromFakeLights(GameLocation gameLocation)
        {
            //ModCore.log("Info for file"+Path.GetFileNameWithoutExtension(file)+" has this many lights: " + info.info.lightManager.fakeLights.Count);
            this.lights.Clear();
            foreach (KeyValuePair<Vector2, FakeLightSource> light in this.fakeLights.Pairs)
            {
                Vector2 position = light.Value.positionOffset;
                position -= light.Key;
                position /= Game1.tileSize;
                position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));
                this.addLightToTileLocation(light.Key, gameLocation, new LightSource(light.Value.id, new Vector2(0, 0), light.Value.radius, light.Value.color.Invert()), position);
            }
        }

        public static LightSource CreateLightSource(float Radius, Color ActualColor, int Texture = 4)
        {
            return new LightSource(Texture, new Vector2(0, 0), Radius, ActualColor.Invert());
        }

    }
}
