using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Revitalize.Framework.Illuminate
{
    public class LightManager
    {
        public Dictionary<Vector2, LightSource> lights;
        public bool lightsOn;

        public LightManager()
        {
            this.lights = new Dictionary<Vector2, LightSource>();
            this.lightsOn = false;
        }

        /// <summary>Add a light to the list of tracked lights.</summary>
        public bool addLight(Vector2 IdKey, LightSource light, StardewValley.Object gameObject)
        {
            Vector2 initialPosition = gameObject.TileLocation * Game1.tileSize;
            initialPosition += IdKey;

            if (this.lights.ContainsKey(IdKey))
                return false;

            light.position.Value = initialPosition;
            this.lights.Add(IdKey, light);
            return true;
        }

        /// <summary>Turn off a single light.</summary>
        public bool turnOffLight(Vector2 IdKey, GameLocation location)
        {
            if (!this.lights.ContainsKey(IdKey))
                return false;

            this.lights.TryGetValue(IdKey, out LightSource light);
            Game1.currentLightSources.Remove(light);
            location.sharedLights.Remove((int)IdKey.X * 1000000 + (int)IdKey.Y);
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

            Game1.showRedMessage("TURN ON!");
            Game1.currentLightSources.Add(light);
            location.sharedLights.Add((int)IdKey.X*10000+(int)IdKey.Y,light);
            this.repositionLight(light, IdKey, gameObject);
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

        public void repositionLights(StardewValley.Object gameObject)
        {
            foreach (KeyValuePair<Vector2, LightSource> pair in this.lights)
                this.repositionLight(pair.Value, pair.Key, gameObject);
        }

        public void repositionLight(LightSource light, Vector2 offset, StardewValley.Object gameObject)
        {
            Vector2 initialPosition = gameObject.TileLocation * Game1.tileSize;
            light.position.Value = initialPosition + offset;
        }

        public virtual void toggleLights(GameLocation location, StardewValley.Object gameObject)
        {
            if (!this.lightsOn)
            {
                this.turnOnLights(location, gameObject);
                this.lightsOn = true;
            }
            else if (this.lightsOn)
            {
                this.turnOffLights(Game1.player.currentLocation);
                this.lightsOn = false;
            }
        }
    }
}
