/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/ParticleFramework
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleFramework.Framework.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace ParticleFramework.Framework.Managers
{
    internal class ParticleEffectManager
    {
        public static readonly List<string> dictPaths = new List<string>
        {
            "Mods/Espy.ParticleFramework/dict",
            "Mods/aedenthorn.ParticleEffects/dict"
        };

        public static Dictionary<string, ParticleEffectData> effectDict = [];

        public static Dictionary<string, EntityParticleData> objectEffectDict = [];
        public static Dictionary<string, EntityParticleData> furnitureEffectDict = [];
        public static Dictionary<long, EntityParticleData> farmerEffectDict = [];
        public static Dictionary<string, EntityParticleData> npcEffectDict = [];
        public static Dictionary<string, EntityParticleData> locationEffectDict = [];
        public static Dictionary<string, EntityParticleData> screenEffectDict = [];


        public static void ShowFarmerParticleEffect(SpriteBatch spriteBatch, Farmer instance, ParticleEffectData ped)
        {
            ShowParticleEffect(spriteBatch, farmerEffectDict, instance.UniqueMultiplayerID, ped, i => instance.GetBoundingBox(), 1, false, instance);
        }

        public static void ShowNPCParticleEffect(SpriteBatch spriteBatch, NPC instance, ParticleEffectData ped)
        {
            ShowParticleEffect(spriteBatch, npcEffectDict, instance.Name, ped, i => instance.GetBoundingBox(), 1, false, instance);
        }

        public static void ShowObjectParticleEffect(SpriteBatch spriteBatch, SObject instance, int x, int y, ParticleEffectData ped)
        {
            ShowParticleEffect(spriteBatch, objectEffectDict, instance.QualifiedItemId, ped, i => instance.GetBoundingBoxAt(x, y), Math.Max(0f, ((y + 1) * 64 - 24) / 10000f) + x * 1E-05f);
        }

        public static void ShowFurnitureParticleEffect(SpriteBatch spriteBatch, Furniture instance, int x, int y, ParticleEffectData ped)
        {
            ShowParticleEffect(spriteBatch, furnitureEffectDict, instance.QualifiedItemId, ped, i => instance.GetBoundingBoxAt(x, y), ((int)instance.furniture_type.Value == 12) ? (2E-09f + (instance.boundingBox.Y / 64) / 100000f) : ((float)(instance.boundingBox.Value.Bottom - (((int)instance.furniture_type.Value == 6 || (int)instance.furniture_type.Value == 17 || (int)instance.furniture_type.Value == 13) ? 48 : 8)) / 10000f));
        }

        public static void ShowLocationParticleEffect(SpriteBatch spriteBatch, GameLocation instance, ParticleEffectData ped)
        {
            ShowParticleEffect(spriteBatch, locationEffectDict, instance.Name, ped, i => new Rectangle(0, 0, 0, 0));
        }

        public static void ShowScreenParticleEffect(SpriteBatch spriteBatch, ParticleEffectData ped)
        {
            ShowParticleEffect(spriteBatch, screenEffectDict, "screen", ped, i => new Rectangle(0, 0, 0, 0), 1f, true);
        }


        public static void ShowParticleEffect<T>(SpriteBatch spriteBatch, Dictionary<T, EntityParticleData> dict, T key, ParticleEffectData ped, Func<T, Rectangle> getBoundingBox, float drawDepth = 1f, bool isScreenSpace = false, object instance = null)
        {
            if (!dict.TryGetValue(key, out EntityParticleData entityParticleData))
            {
                entityParticleData = new EntityParticleData();
                dict[key] = entityParticleData;
            }

            if (!entityParticleData.particleDict.TryGetValue(ped.key, out var particleList))
            {
                particleList = new List<ParticleData>();
                entityParticleData.particleDict[ped.key] = particleList;
            }

            var center = getBoundingBox(key).Center.ToVector2() + new Vector2(ped.fieldOffsetX, ped.fieldOffsetY);

            foreach (var particle in particleList.ToArray())
            {
                UpdateParticle(particleList, particle, ped, center, instance);

                if (IsOutOfBounds(particle, ped, center))
                {
                    particleList.Remove(particle);
                    continue;
                }

                DrawParticle(spriteBatch, particle, ped, drawDepth, isScreenSpace);
            }

            AddNewParticles(particleList, ped, center);
        }


        // TODO: FIX FOLLOW FOR ALL MOVEMENT TYPES
        private static void UpdateParticle(List<ParticleData> particles, ParticleData particle, ParticleEffectData effectData, Vector2 center, object instance = null)
        {
            particle.age++;

            if (particle.age > particle.lifespan)
            {
                particles.Remove(particle);
                return;
            }

            if (particle.direction == Vector2.Zero)
            {
                SetParticleDirection(particle, effectData, center);
            }

            if (particle.originalDirection == Vector2.Zero)
            {
                particle.originalDirection = particle.direction;
            }

            // Check if the particle should follow the player or npc
            if (instance is Farmer farmer && effectData.follow && farmer.movedDuringLastTick())
            {
                Vector2 playerMovementDirection = farmer.getMostRecentMovementVector();
                playerMovementDirection.Normalize();

                Vector2 directionToCenter = center - particle.position;
                Vector2 awayDirection = directionToCenter * new Vector2(-1, -1);
                awayDirection.Normalize();

                // Combine the away direction and the player's movement direction
                Vector2 combinedDirection = awayDirection + playerMovementDirection * (farmer.speed + farmer.addedSpeed);

                // Normalize the combined direction vector
                combinedDirection.Normalize();

                // Update the particle direction
                particle.direction = combinedDirection;
            }
            else if (instance is NPC npc && effectData.follow && !npc.Position.Equals(npc.lastPosition))
            {
                Vector2 npcMovementDirection = new Vector2(npc.Position.X - npc.lastPosition.X, npc.Position.Y - npc.lastPosition.Y);
                npcMovementDirection.Normalize();

                particle.direction = particle.originalDirection + npcMovementDirection * (npc.speed + npc.addedSpeed);
            }
            else if (particle.originalDirection != Vector2.Zero)
            {
                // Restore the original direction if the player is not moving
                particle.direction = particle.originalDirection;
                particle.originalDirection = Vector2.Zero;
            }

            // Update particle position
            particle.position += particle.direction * (effectData.movementSpeed + effectData.acceleration * particle.age);
            particle.rotation += particle.rotationRate;
        }

        private static void SetParticleDirection(ParticleData particle, ParticleEffectData effectData, Vector2 center)
        {
            Vector2 direction = Vector2.Zero;

            switch (effectData.movementType)
            {
                case "away":
                    direction += (center - particle.position) * new Vector2(-1, -1);
                    break;
                case "towards":
                    direction += center - particle.position;
                    break;
                case "up":
                    direction += -Vector2.UnitY;
                    break;
                case "down":
                    direction += Vector2.UnitY;
                    break;
                case "left":
                    direction += -Vector2.UnitX;
                    break;
                case "right":
                    direction += Vector2.UnitX;
                    break;
                case "random":
                    direction += new Vector2((float)Game1.random.NextDouble() - 0.5f, (float)Game1.random.NextDouble() - 0.5f);
                    break;
            }

            direction.Normalize();
            particle.direction = direction;
        }

        private static void DrawParticle(SpriteBatch spriteBatch, ParticleData particle, ParticleEffectData effectData, float drawDepth, bool isScreenSpace)
        {
            try
            {
                // Avoid divide by zero error
                if (effectData.particleWidth <= 0 || effectData.particleHeight <= 0)
                {
                    ModEntry.monitor.Log($"Particle '{effectData.key}' width or height is zero or negative. Unable to draw particle.", LogLevel.Error);
                    UnloadEffect(effectData.key);
                    return;
                }

                float frameSpeed = (effectData.frameSpeed != 0) ? effectData.frameSpeed : 1; // Ensure frameSpeed is not zero
                int totalFrames = effectData.spriteSheet.Width / effectData.particleWidth;
                // Avoid divide by zero error
                if (totalFrames <= 0)
                {
                    ModEntry.monitor.Log($"Particle '{effectData.key}' total frames calculated as zero. Make sure particleWidth is valid. Unable to draw particle.", LogLevel.Error);
                    UnloadEffect(effectData.key);
                    return;
                }
                int frame = (int)Math.Round(particle.age * frameSpeed) % totalFrames;

                float depthOffset = GetDepthOffset(effectData);

                spriteBatch.Draw(
                    effectData.spriteSheet,
                    new Rectangle(
                        Utility.Vector2ToPoint(isScreenSpace ? particle.position : Game1.GlobalToLocal(particle.position)),
                        new Point((int)(effectData.particleWidth * particle.scale), (int)(effectData.particleHeight * particle.scale))
                    ),
                    new Rectangle(
                        frame * effectData.particleWidth,
                        particle.option * effectData.particleHeight,
                        effectData.particleWidth,
                        effectData.particleHeight
                    ),
                    Color.White * particle.alpha,
                    particle.rotation,
                    new Vector2(effectData.particleWidth / 2, effectData.particleHeight / 2),
                    SpriteEffects.None,
                    drawDepth + depthOffset
                );
            } 
            catch (Exception e)
            {
                ModEntry.monitor.Log($"Error drawing particle for effect '{effectData.key}': {e.Message}", LogLevel.Error);
                UnloadEffect(effectData.key);
            }
        }

        private static float GetDepthOffset(ParticleEffectData effectData)
        {
            float depthOffset = effectData.belowOffset >= 0 ? (effectData.aboveOffset >= 0 ? (Game1.random.NextDouble() < 0.5 ? effectData.aboveOffset : effectData.belowOffset) : effectData.belowOffset) : effectData.aboveOffset;
            return depthOffset;
        }

        private static void AddNewParticles(List<ParticleData> particles, ParticleEffectData effectData, Vector2 center)
        {
            if (particles.Count < effectData.maxParticles && Game1.random.NextDouble() < effectData.particleChance)
            {
                var newParticle = GenerateParticle(effectData, center);
                particles.Add(newParticle);
            }
        }

        private static ParticleData GenerateParticle(ParticleEffectData effectData, Vector2 center)
        {
            try
            {
                if (effectData.spriteSheet == null)
                {
                    ModEntry.monitor.Log($"Error generating particle: Sprite sheet is null for particle '{effectData.key}'", LogLevel.Error);
                    UnloadEffect(effectData.key);
                    return null;
                }
                var newParticle = new ParticleData();
                newParticle.lifespan = Game1.random.Next(effectData.minLifespan, effectData.maxLifespan + 1);
                newParticle.scale = effectData.minParticleScale + (float)Game1.random.NextDouble() * (effectData.maxParticleScale - effectData.minParticleScale);
                newParticle.alpha = effectData.minAlpha + (float)Game1.random.NextDouble() * (effectData.maxAlpha - effectData.minAlpha);
                newParticle.rotationRate = effectData.minRotationRate + (float)Game1.random.NextDouble() * (effectData.maxRotationRate - effectData.minRotationRate);
                newParticle.option = Game1.random.Next(effectData.spriteSheet.Height / effectData.particleHeight);

                if (effectData.fieldOuterRadius <= 0)
                {
                    newParticle.position = center - new Vector2(effectData.fieldOuterWidth, effectData.fieldOuterHeight) / 2 + GetRandomOffset(effectData);
                }
                else
                {
                    newParticle.position = center + GetCirclePos(effectData);
                }

                return newParticle;
            }
            catch (Exception e)
            {
                ModEntry.monitor.Log($"Error generating particle for effect '{effectData.key}': {e}", LogLevel.Error);
                UnloadEffect(effectData.key);
                return null;
            }
        }

        private static Vector2 GetRandomOffset(ParticleEffectData effectData)
        {
            double x;
            double y;

            if (effectData.fieldInnerHeight > 0)
            {
                var innerTop = (effectData.fieldOuterHeight - effectData.fieldInnerHeight) / 2;
                var innerBottom = effectData.fieldOuterHeight - innerTop;
                var innerLeft = (effectData.fieldOuterWidth - effectData.fieldInnerWidth) / 2;
                var innerRight = effectData.fieldOuterWidth - innerLeft;

                // Calculate a random point inside the inner field
                int pixel = (int)((effectData.fieldOuterWidth * innerTop * 2 + effectData.fieldInnerHeight * innerLeft * 2) * Game1.random.NextDouble());

                if (pixel >= effectData.fieldOuterWidth * innerTop + effectData.fieldInnerHeight * innerLeft * 2) // bottom
                {
                    pixel = pixel - effectData.fieldOuterWidth * innerTop - effectData.fieldInnerHeight * innerLeft * 2;
                    x = pixel % effectData.fieldOuterWidth;
                    y = innerBottom + pixel / effectData.fieldOuterWidth;
                }
                else if (pixel >= effectData.fieldOuterWidth * innerTop + effectData.fieldInnerHeight * innerLeft) // right
                {
                    pixel = pixel - effectData.fieldOuterWidth * innerTop - effectData.fieldInnerHeight * innerLeft;
                    x = innerRight + pixel % innerLeft;
                    y = innerTop + pixel / innerLeft;
                }
                else if (pixel >= effectData.fieldOuterWidth * innerTop) // left
                {
                    pixel = pixel - effectData.fieldOuterWidth * innerTop;
                    x = pixel % innerLeft;
                    y = innerTop + pixel / innerLeft;
                }
                else // top
                {
                    x = pixel % effectData.fieldOuterWidth;
                    y = pixel / effectData.fieldOuterWidth;
                }
            }
            else
            {
                x = effectData.fieldOuterWidth * Game1.random.NextDouble();
                y = effectData.fieldOuterHeight * Game1.random.NextDouble();
            }

            return new Vector2((float)x, (float)y);
        }

        private static Vector2 GetCirclePos(ParticleEffectData ped)
        {
            var angle = (float)Game1.random.NextDouble() * 2 * Math.PI;
            var distance = (float)Math.Sqrt(ped.fieldInnerRadius / ped.fieldOuterRadius + (float)Game1.random.NextDouble() * (1 - ped.fieldInnerRadius / ped.fieldOuterRadius)) * ped.fieldOuterRadius;
            return new Vector2(distance * (float)Math.Cos(angle), distance * (float)Math.Sin(angle));
        }

        private static bool IsOutOfBounds(ParticleData particle, ParticleEffectData ped, Vector2 center)
        {
            if (!ped.restrictOuter && !ped.restrictInner)
                return false;
            if (ped.fieldOuterRadius > 0)
            {
                return (ped.restrictOuter && Vector2.Distance(center, particle.position) > ped.fieldOuterRadius) || (ped.restrictInner && Vector2.Distance(center, particle.position) <= ped.fieldInnerRadius);
            }
            else
            {
                return (ped.restrictOuter && Math.Abs(particle.position.X - center.X) > ped.fieldOuterWidth / 2 || Math.Abs(particle.position.Y - center.Y) > ped.fieldOuterHeight / 2) || (ped.restrictInner && Math.Abs(particle.position.X - center.X) <= ped.fieldInnerWidth / 2 && Math.Abs(particle.position.Y - center.Y) <= ped.fieldInnerHeight / 2);
            }
        }

        public static void LoadEffects()
        {
            try
            {
                foreach (var path in dictPaths)
                {
                    var dict = Game1.content.Load<Dictionary<string, ParticleEffectData>>(path);
                    foreach (var kvp in dict)
                    {
                        try
                        {
                            kvp.Value.key = kvp.Key;
                            kvp.Value.spriteSheet = Game1.content.Load<Texture2D>(kvp.Value.spriteSheetPath);
                        }
                        catch (Exception e)
                        {
                            ModEntry.monitor.Log($"Error loading particle effect with key '{kvp.Key}': {e}", LogLevel.Error);
                            UnloadEffect(kvp.Key);
                        }
                    }
                    // Merge the loaded dictionary with the existing one
                    foreach (var kvp in dict)
                    {
                        effectDict[kvp.Key] = kvp.Value;
                    }
                }
            }
            catch (Exception e)
            {
                ModEntry.monitor.Log($"Error loading particle effects: {e}", LogLevel.Error);
            }
        }

        public static void UnloadEffect(string key)
        {
            try
            {
                if (effectDict.ContainsKey(key))
                {
                    effectDict.Remove(key);
                    ModEntry.monitor.Log($"Successfully unloaded particle effect '{key}'.", LogLevel.Warn);
                }
                else
                {
                    ModEntry.monitor.Log($"Error unloading particle effect '{key}':  Not found in the dictionary.", LogLevel.Warn);
                }
            }
            catch (Exception e)
            {
                ModEntry.monitor.Log($"Error unloading particle effect '{key}': {e}", LogLevel.Error);
            }
        }

        public static List<ParticleData> GetAllParticleData(object effectKey)
        {
            var allParticleData = new List<ParticleData>();

            // Dictionaries with string keys
            Dictionary<string, EntityParticleData>[] stringDicts = { npcEffectDict, objectEffectDict, furnitureEffectDict, locationEffectDict, screenEffectDict };

            // Dictionary with long keys
            Dictionary<long, EntityParticleData>[] longDicts = { farmerEffectDict };

            // Check if the effectKey is a string or a long
            if (effectKey is string)
            {
                string keyString = (string)effectKey;
                // Iterate through dictionaries with string keys
                foreach (var dict in stringDicts)
                {
                    if (dict.TryGetValue(keyString, out EntityParticleData entityParticleData))
                    {
                        if (entityParticleData.particleDict.TryGetValue(keyString, out List<ParticleData> particleList))
                        {
                            allParticleData.AddRange(particleList);
                        }
                    }
                }
            }
            else if (effectKey is long)
            {
                long keyLong = (long)effectKey;
                // Iterate through dictionary with long keys
                foreach (var dict in longDicts)
                {
                    if (dict.TryGetValue(keyLong, out EntityParticleData entityParticleData))
                    {
                        if (entityParticleData.particleDict.TryGetValue(keyLong.ToString(), out List<ParticleData> particleList))
                        {
                            allParticleData.AddRange(particleList);
                        }
                    }
                }
            }
            else
            {
                // Invalid type for effectKey
                ModEntry.monitor.Log($"Error: Invalid type for effect key '{effectKey}'. Expected string or long.", LogLevel.Error);
            }

            return allParticleData;
        }

    }
}


/*public static ParticleEffectData CloneParticleEffect(string key, string type, string name, int x, int y, ParticleEffectData template)
{
    return new ParticleEffectData()
    {
        key = key,
        type = type,
        name = name,
        movementType = template.movementType,
        movementSpeed = template.movementSpeed,
        frameSpeed = template.frameSpeed,
        acceleration = template.acceleration,
        restrictOuter = template.restrictOuter,
        restrictInner = template.restrictInner,
        minRotationRate = template.minRotationRate,
        maxRotationRate = template.maxRotationRate,
        particleWidth = template.particleWidth,
        particleHeight = template.particleHeight,
        fieldInnerWidth = template.fieldInnerWidth,
        fieldInnerHeight = template.fieldInnerHeight,
        fieldOuterWidth = template.fieldOuterWidth,
        fieldOuterHeight = template.fieldOuterHeight,
        minParticleScale = template.minParticleScale,
        maxParticleScale = template.maxParticleScale,
        maxParticles = template.maxParticles,
        minLifespan = template.minLifespan,
        maxLifespan = template.maxLifespan,
        spriteSheetPath = template.spriteSheetPath,
        spriteSheet = template.spriteSheet,
        fieldOffsetX = x,
        fieldOffsetY = y
    };
}*/