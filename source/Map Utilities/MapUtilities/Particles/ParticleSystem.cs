using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace MapUtilities.Particles
{
    public class ParticleSystem
    {
        public const int Out = 0;
        public const int North = 1;
        public const int East = 2;
        public const int Up = 3;
        public const int Right = 4;

        //public Texture2D particleSheet;
        //public Rectangle particleRect;

        public List<Tuple<float, Texture2D, Rectangle>> possibleSprites;
        public float totalWeight;

        public List<Tuple<float, Color, float>> colors;

        public Vector2 tileLocation;

        public int spawnStaggering;

        private int spawnCooldown;

        public float scale;

        public float range;

        public float killRange;

        public int count;

        public int longetivity;

        public List<Particle> particles;

        public Dictionary<int, float> velocities;

        public Dictionary<int, float> accelerations;

        public float minRotation;
        public float maxRotation;

        public float rotationSpeed;

        public float rotationAcceleration;

        public ParticleSystem(Texture2D spriteSheet, Rectangle sprite, Vector2 location, float scale = 4f, float range = 2f, float killRange = 8f, int count = 4, int longetivity = 1200, float minRotation = 0f, float maxRotation = 0f, float rotationSpeed = 0f, float rotationAcceleration = 0f, Dictionary<int, float> velocities = null, Dictionary<int, float> accelerations = null)
        {
            possibleSprites = new List<Tuple<float, Texture2D, Rectangle>>();
            possibleSprites.Add(new Tuple<float, Texture2D, Rectangle>(1, spriteSheet, sprite));
            colors = new List<Tuple<float, Color, float>>();
            totalWeight = 1f;

            this.tileLocation = location;
            this.scale = scale;
            this.range = range;
            this.killRange = killRange;
            this.count = count;
            this.longetivity = longetivity;
            this.spawnCooldown = 0;
            this.spawnStaggering = 48;
            this.minRotation = minRotation;
            this.maxRotation = maxRotation;
            this.rotationSpeed = rotationSpeed;
            this.rotationAcceleration = rotationAcceleration;
            if (velocities == null)
            {
                this.velocities = new Dictionary<int, float>();
                this.velocities[Out] = 0;
                this.velocities[North] = 0;
                this.velocities[East] = 0;
                this.velocities[Up] = 0;
                this.velocities[Right] = 0;
            }
            else
            {
                this.velocities = velocities;
                if (!this.velocities.ContainsKey(Out))
                    this.velocities[Out] = 0;
                if (!this.velocities.ContainsKey(North))
                    this.velocities[North] = 0;
                if (!this.velocities.ContainsKey(East))
                    this.velocities[East] = 0;
                if (!this.velocities.ContainsKey(Up))
                    this.velocities[Up] = 0;
                if (!this.velocities.ContainsKey(Right))
                    this.velocities[Right] = 0;
            }

            if (accelerations == null)
                this.accelerations = new Dictionary<int, float>();
            else
                this.accelerations = accelerations;

            if (!this.accelerations.ContainsKey(Out))
                this.accelerations[Out] = 0;
            if (!this.accelerations.ContainsKey(North))
                this.accelerations[North] = 0;
            if (!this.accelerations.ContainsKey(East))
                this.accelerations[East] = 0;
            if (!this.accelerations.ContainsKey(Up))
                this.accelerations[Up] = 0;
            if (!this.accelerations.ContainsKey(Right))
                this.accelerations[Right] = 0;
            this.particles = new List<Particle>();
        }

        public ParticleSystem(string particleFilePath) : this((JObject)Loader.load<JObject>(particleFilePath.Split('.')[0] + ".json"))
        {

        }

        public ParticleSystem(JObject particleJSON)
        {
            if(!particleJSON.ContainsKey("Format") || (!particleJSON.ContainsKey("Image") && !particleJSON.ContainsKey("Sprites")))
            {
                Logger.log("Particle system did not contain necessary field(s)!  Missing" + (!particleJSON.ContainsKey("Format") ? " Format" : "") + (!particleJSON.ContainsKey("Image") ? " Image" : ""), StardewModdingAPI.LogLevel.Error);
                return;
            }
            float formatVersion;
            try
            {
                formatVersion = Convert.ToSingle(particleJSON["Format"]);
            }
            catch (FormatException)
            {
                Logger.log("Format version not given an acceptable value!  Format should be written as a version number, but was instead " + particleJSON["Format"], StardewModdingAPI.LogLevel.Error);
                return;
            }
            possibleSprites = new List<Tuple<float, Texture2D, Rectangle>>();
            colors = new List<Tuple<float, Color, float>>();
            if (particleJSON.ContainsKey("Image"))
            {
                //Particle defined with a single image, not multiple
                string imagePath = particleJSON["Image"].ToString().Split('.')[0];
                Texture2D particleSheet = (Texture2D)Loader.load<Texture2D>(imagePath, imagePath + ".png");
                Rectangle particleRect;
                if (particleJSON.ContainsKey("Sprite"))
                {
                    int spriteX = (((JObject)particleJSON["Sprite"]).ContainsKey("X") ? Convert.ToInt32(particleJSON["Sprite"]["X"]) : 0);
                    int spriteY = (((JObject)particleJSON["Sprite"]).ContainsKey("Y") ? Convert.ToInt32(particleJSON["Sprite"]["Y"]) : 0);
                    int spriteWidth = (((JObject)particleJSON["Sprite"]).ContainsKey("Width") ? Convert.ToInt32(particleJSON["Sprite"]["Width"]) : particleSheet.Width - spriteX);
                    int spriteHeight = (((JObject)particleJSON["Sprite"]).ContainsKey("Height") ? Convert.ToInt32(particleJSON["Sprite"]["Height"]) : particleSheet.Height - spriteY);
                    particleRect = new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight);
                }
                else
                {
                    particleRect = new Rectangle(0, 0, particleSheet.Width, particleSheet.Height);
                }
                possibleSprites.Add(new Tuple<float, Texture2D, Rectangle>(1f, particleSheet, particleRect));
                totalWeight = 1f;
            }
            else
            {
                //Particle defined with multiple, weighted images
                foreach(JObject spriteObject in ((JArray)(particleJSON["Sprites"])))
                {
                    float weight = 1f;
                    string imagePath = spriteObject["Image"].ToString().Split('.')[0];
                    Texture2D spriteImage = (Texture2D)Loader.load<Texture2D>(imagePath, "Content/" + imagePath + ".png");
                    Rectangle spriteBounds;
                    if (spriteObject.ContainsKey("Weight"))
                        weight = Convert.ToSingle(spriteObject["Weight"]);
                    if (spriteObject.ContainsKey("Sprite"))
                    {
                        int spriteX = (((JObject)spriteObject["Sprite"]).ContainsKey("X") ? Convert.ToInt32(spriteObject["Sprite"]["X"]) : 0);
                        int spriteY = (((JObject)spriteObject["Sprite"]).ContainsKey("Y") ? Convert.ToInt32(spriteObject["Sprite"]["Y"]) : 0);
                        int spriteWidth = (((JObject)spriteObject["Sprite"]).ContainsKey("Width") ? Convert.ToInt32(spriteObject["Sprite"]["Width"]) : spriteImage.Width - spriteX);
                        int spriteHeight = (((JObject)spriteObject["Sprite"]).ContainsKey("Height") ? Convert.ToInt32(spriteObject["Sprite"]["Height"]) : spriteImage.Height - spriteY);
                        spriteBounds = new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight);
                    }
                    else
                    {
                        spriteBounds = new Rectangle(0, 0, spriteImage.Width, spriteImage.Height);
                    }
                    possibleSprites.Add(new Tuple<float, Texture2D, Rectangle>(weight, spriteImage, spriteBounds));
                    totalWeight += weight;
                }
            }
            scale = 4f;
            range = 2f;
            killRange = 8f;
            count = 4;
            longetivity = 1200;
            minRotation = 0f;
            maxRotation = (float)(Math.PI * 2);
            rotationSpeed = 0f;
            rotationAcceleration = 0f;
            spawnStaggering = 48;
            spawnCooldown = 0;

            

            if (particleJSON.ContainsKey("Scale"))
                scale = Convert.ToInt32(particleJSON["Scale"]);
            if (particleJSON.ContainsKey("SpawnRadius"))
                range = Convert.ToSingle(particleJSON["SpawnRadius"]);
            if (particleJSON.ContainsKey("KillRadius"))
                killRange = Convert.ToSingle(particleJSON["KillRadius"]);
            if (particleJSON.ContainsKey("Count"))
                count = Convert.ToInt32(particleJSON["Count"]);
            if (particleJSON.ContainsKey("Lifetime"))
                longetivity = Convert.ToInt32(particleJSON["Lifetime"]);
            if (particleJSON.ContainsKey("SpawnStagger"))
                spawnStaggering = Convert.ToInt32(particleJSON["SpawnStagger"]);
            if (particleJSON.ContainsKey("SpawnDelay"))
                spawnCooldown = Convert.ToInt32(particleJSON["SpawnDelay"]);

            velocities = new Dictionary<int, float>();
            velocities[Out] = 0;
            velocities[North] = 0;
            velocities[East] = 0;
            velocities[Up] = 0;
            velocities[Right] = 0;

            accelerations = new Dictionary<int, float>();
            accelerations[Out] = 0;
            accelerations[North] = 0;
            accelerations[East] = 0;
            accelerations[Up] = 0;
            accelerations[Right] = 0;

            if (particleJSON.ContainsKey("Rotation"))
            {
                if (((JObject)particleJSON["Rotation"]).ContainsKey("Min"))
                    minRotation = radiansFromDegreeString(particleJSON["Rotation"]["Min"].ToString());
                if (((JObject)particleJSON["Rotation"]).ContainsKey("Max"))
                    maxRotation = radiansFromDegreeString(particleJSON["Rotation"]["Max"].ToString());
                if (((JObject)particleJSON["Rotation"]).ContainsKey("Speed"))
                    rotationSpeed = radiansFromDegreeString(particleJSON["Rotation"]["Speed"].ToString());
                if (((JObject)particleJSON["Rotation"]).ContainsKey("Acceleration"))
                    rotationAcceleration = radiansFromDegreeString(particleJSON["Rotation"]["Acceleration"].ToString());
            }
            if (particleJSON.ContainsKey("Velocity"))
            {
                foreach(JObject velocityObject in particleJSON["Velocity"])
                {
                    int direction = directionFromString(velocityObject["Direction"].ToString().Substring(0,1).ToLower());
                    if (direction == -1)
                        continue;
                    bool invertDirection = false;
                    if (direction > 4)
                    {
                        invertDirection = true;
                        direction = direction % 5;
                    }
                    velocities[direction] += Convert.ToSingle(velocityObject["Speed"]) * (invertDirection ? -1 : 1);
                }
            }
            if (particleJSON.ContainsKey("Acceleration"))
            {
                foreach (JObject accelerationObject in particleJSON["Acceleration"])
                {
                    int direction = directionFromString(accelerationObject["Direction"].ToString().Substring(0, 1).ToLower());
                    if (direction == -1)
                        continue;
                    bool invertDirection = false;
                    if (direction > 4)
                    {
                        invertDirection = true;
                        direction = direction % 5;
                    }
                    accelerations[direction] = Convert.ToSingle(accelerationObject["Acceleration"]) * (invertDirection ? -1 : 1);
                }
            }
            if (particleJSON.ContainsKey("Color"))
            {
                foreach(JObject colorObject in particleJSON["Color"])
                {
                    float colorAge = 0;
                    Color color = (colors.Count > 0 ? colors[colors.Count - 1].Item2 : Color.White);
                    float alpha = 1f;
                    if(colorObject.ContainsKey("Age"))
                        colorAge = Convert.ToSingle(colorObject["Age"]);
                    if (colorObject.ContainsKey("Color"))
                    {
                        if(colorObject["Color"] is JObject)
                        {
                            JObject rgbObject = (JObject)(colorObject["Color"]);
                            Logger.log("Getting color info from JObject...");
                            int r = 0;
                            int g = 0;
                            int b = 0;
                            int a = 255;
                            if (rgbObject.ContainsKey("R"))
                                r = Convert.ToInt32(rgbObject["R"]);
                            if (rgbObject.ContainsKey("G"))
                                g = Convert.ToInt32(rgbObject["G"]);
                            if (rgbObject.ContainsKey("B"))
                                b = Convert.ToInt32(rgbObject["B"]);
                            if (rgbObject.ContainsKey("A"))
                                a = Convert.ToInt32(rgbObject["A"]);
                            color = new Color(r, g, b);
                            alpha = (a / 255f);
                        }
                        else if(colorObject["Color"] is JValue && colorObject["Color"].ToString().StartsWith("#"))
                        {
                            Logger.log("Getting color info from hex string...");
                            System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(colorObject["Color"].ToString());
                            color = new Color(c.R, c.G, c.B);
                        }
                        Logger.log("Color parsed as " + color.ToString());
                    }
                    if (colorObject.ContainsKey("Alpha"))
                    {
                        alpha = Convert.ToSingle(colorObject["Alpha"].ToString());
                    }
                    colors.Add(new Tuple<float, Color, float>(colorAge, color, alpha));
                }
            }
            particles = new List<Particle>();
        }

        public float radiansFromDegreeString(string degreeString)
        {
            float degrees = Convert.ToSingle(degreeString);
            return (degrees * (float)(Math.PI / 180f));
        }

        public int directionFromString(string direction)
        {
            switch (direction)
            {
                case "o":
                    return Out;
                case "n":
                    return North;
                case "e":
                    return East;
                case "u":
                    return Up;
                case "r":
                    return Right;
                case "i":
                    return Out + 5;
                case "s":
                    return North + 5;
                case "w":
                    return East + 5;
                case "d":
                    return Up + 5;
                case "l":
                    return Right + 5;
            }
            return -1;
        }

        public void update(GameTime time, GameLocation location)
        {
            if (particles.Count < count && spawnCooldown <= 0)
            {
                //Logger.log("Adding particle.");
                spawnParticle(time);
                spawnCooldown += spawnStaggering;
            }
            else
            {
                spawnCooldown -= time.ElapsedGameTime.Milliseconds;
            }
            List<int> particlesToDespawn = new List<int>();
            for(int i = 0; i < particles.Count; i++)
            {
                if(Math.Sqrt(Math.Pow(particles[i].position.X,2) + Math.Pow(particles[i].position.Y,2)) / 64f >= killRange)
                {
                    particlesToDespawn.Add(i);
                    continue;
                }
                if (particles[i].lifetime >= particles[i].lifeDuration)
                {
                    //Logger.log("Killing particle.");
                    particlesToDespawn.Add(i);
                }
                else
                {
                    particles[i].lifetime += time.ElapsedGameTime.Milliseconds;
                    //Logger.log("Particle lifetime: " + particles[i].lifetime + ", longetivity: " + longetivity + ", game time: " + time.ElapsedGameTime.Milliseconds + ", difference: " + (time.ElapsedGameTime.Milliseconds - (particles[i].lifetime + longetivity)));
                }
            }
            particlesToDespawn.Reverse();
            foreach(int index in particlesToDespawn)
            {
                particles.RemoveAt(index);
            }
            updateParticleRotations();
            updateParticlePositions();
        }

        public void updateParticleRotations()
        {
            foreach (Particle particle in particles)
            {
                particle.rotation = (particle.rotation + rotationSpeed + ((particle.lifetime / 1000f) * rotationAcceleration)) % ((float)Math.PI * 2);
            }
        }

        public void updateParticlePositions()
        {
            foreach (Particle particle in particles)
            {
                float deltaX = 0f;
                float deltaY = 0f;

                if (velocities.ContainsKey(Out))
                {
                    float unitMultiplier = 1f / (float)(Math.Sqrt(Math.Pow(particle.position.X, 2) + Math.Pow(particle.position.Y, 2)));
                    float xMult = particle.position.X * unitMultiplier;
                    float yMult = particle.position.Y * unitMultiplier;
                    deltaX += xMult * velocities[Out];
                    deltaY += yMult * velocities[Out];
                }
                if (velocities.ContainsKey(North))
                {
                    deltaY -= velocities[North];
                }
                if (velocities.ContainsKey(East))
                {
                    deltaX += velocities[East];
                }
                if (velocities.ContainsKey(Up))
                {
                    deltaX -= (float)(velocities[Up] * Math.Sin(particle.rotation) * -1);
                    deltaY -= (float)(velocities[Up] * Math.Cos(particle.rotation));
                }
                if (velocities.ContainsKey(Right))
                {
                    deltaX += (float)(velocities[Right] * Math.Cos(particle.rotation));
                    deltaY += (float)(velocities[Right] * Math.Sin(particle.rotation));
                }


                if (accelerations.ContainsKey(Out))
                {
                    float unitMultiplier = 1f / (float)(Math.Sqrt(Math.Pow(particle.position.X, 2) + Math.Pow(particle.position.Y, 2)));
                    float xMult = particle.position.X * unitMultiplier;
                    float yMult = particle.position.Y * unitMultiplier;
                    deltaX += accelerations[Out] * (particle.lifetime / 1000f) * xMult;
                    deltaY += accelerations[Out] * (particle.lifetime / 1000f) * yMult;
                }
                if (accelerations.ContainsKey(North))
                {
                    deltaY -= accelerations[North] * (particle.lifetime / 1000f);
                }
                if (accelerations.ContainsKey(East))
                {
                    deltaX += accelerations[East] * (particle.lifetime / 1000f);
                }
                if (accelerations.ContainsKey(Up))
                {
                    deltaX -= (float)(accelerations[Up] * Math.Sin(particle.rotation) * -1 * (particle.lifetime / 1000f));
                    deltaY -= (float)(accelerations[Up] * Math.Cos(particle.rotation) * (particle.lifetime / 1000f));
                }
                if (accelerations.ContainsKey(Right))
                {
                    deltaX += (float)(accelerations[Right] * Math.Cos(particle.rotation) * (particle.lifetime / 1000f));
                    deltaY += (float)(accelerations[Right] * Math.Sin(particle.rotation) * (particle.lifetime / 1000f));
                }

                particle.position.X += deltaX;
                particle.position.Y += deltaY;
            }
        }

        public void draw(SpriteBatch b)
        {
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64, tileLocation.Y * 64));
            foreach (Particle particle in particles)
            {
                particle.draw(b, possibleSprites[particle.spriteIndex].Item2, possibleSprites[particle.spriteIndex].Item3, (tileLocation.Y * 64f) / 10000f, local, getColorForAge(particle.lifetime));
            }
        }

        public void drawAsChild(SpriteBatch b, Vector2 offset, float depth)
        {
            Vector2 local = new Vector2(tileLocation.X + offset.X, tileLocation.Y + offset.Y);
            foreach (Particle particle in particles)
            {
                particle.draw(b, possibleSprites[particle.spriteIndex].Item2, possibleSprites[particle.spriteIndex].Item3, depth, local, getColorForAge(particle.lifetime));
            }
        }

        public virtual void spawnParticle(GameTime time)
        {
            float particleRotation = ((float)Game1.random.NextDouble() * (maxRotation - minRotation) + minRotation);
            Particle newParticle = new Particle(particleSpawnPoint(), scale, particleRotation, 0, longetivity);
            if (possibleSprites.Count > 0)
            {
                newParticle.spriteIndex = getRandomSpriteIndex();
            }
            particles.Add(newParticle);
        }

        public virtual Vector2 particleSpawnPoint()
        {
            double distance = Game1.random.NextDouble() * range * 64 * 2f - range;
            double rotation = Game1.random.NextDouble() * Math.PI * 2;

            float x = (float)(distance * Math.Sin(rotation) * -1);
            float y = (float)(distance * Math.Cos(rotation));

            return new Vector2(x, y);
        }

        public virtual int getRandomSpriteIndex()
        {
            float randomWeight = (float)Game1.random.NextDouble() * totalWeight;
            float currentWeight = 0;
            foreach(Tuple<float, Texture2D, Rectangle> possibleSprite in possibleSprites)
            {
                currentWeight += possibleSprite.Item1;
                if(randomWeight <= currentWeight)
                {
                    return possibleSprites.IndexOf(possibleSprite);
                }
            }
            return 0;
        }

        public virtual Color getColorForAge(float age)
        {
            float alphaA = 1f;
            float alphaB = 1f;
            float ageA = 0f;
            float ageB = 0f;
            Color colorA = Color.White;
            Color colorB = Color.White;
            bool hadColorB = false;

            foreach (Tuple<float, Color, float> colorSet in colors)
            {
                if(age >= colorSet.Item1)
                {
                    alphaA = colorSet.Item3;
                    colorA = colorSet.Item2;
                    ageA = colorSet.Item1;
                }
                else
                {
                    alphaB = colorSet.Item3;
                    colorB = colorSet.Item2;
                    ageB = colorSet.Item1;
                    hadColorB = true;
                    break;
                }
            }
            if(!hadColorB)
            {
                return colorA * alphaA;
            }
            else
            {
                float progress = (age - ageA) / (ageB - ageA);
                //Logger.log("Ramping between " + colorA.ToString() + " and " + colorB.ToString() + ", progress " + progress * 100 + "%");
                int R = (int)((colorB.R - colorA.R) * progress + colorA.R);
                int G = (int)((colorB.G - colorA.G) * progress + colorA.G);
                int B = (int)((colorB.B - colorA.B) * progress + colorA.B);
                float alpha = (alphaB - alphaA) * progress + alphaA;
                Color outColor = new Color(R, G, B);
                //Logger.log("Ramped color: " + outColor.ToString());
                return outColor * alpha;
            }
        }

    }
}
