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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.Interfaces;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCEnemies.Spawners
{
    public class Target_Spawner:ISpawner
    {

        public Vector2 position;
        /// <summary>
        /// The max time in milliseconds between spawns.
        /// </summary>
        public double maxFrequency;
        /// <summary>
        /// The min time in milliseconds between spawns.
        /// </summary>
        public double minFrequency;

        /// <summary>
        /// The time until the next spawn.
        /// </summary>
        public double timeUntilNextSpawn;

        /// <summary>
        /// Randomizes the spawn time between spawns between 0 and this.frequency.
        /// </summary>
        public bool randomizeSpawnTimes;

        public Vector2 spawnDirection;

        public Color spawnColor;
        public bool randomizeColor;
        /// <summary>
        /// The max speed a target can travel at.
        /// </summary>
        public float maxSpeed;
        public float minSpeed;

        public bool enabled { get; set; }

        public Target_Spawner()
        {

        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Position">The position of the spawner.</param>
        /// <param name="SpawnDirection">The direction to spawn targets.</param>
        /// <param name="Color">The default color of targets that are spawned.</param>
        /// <param name="RandomizeColor">Should the color be randomized every time a target is spawned?</param>
        /// <param name="MaxFrequency">The max amount of time between spawns.</param>
        /// <param name="MinFrequency">The min amount of time between spawns.</param>
        /// <param name="RandomizeSpawnTime">Should the frequency between spawns be randomized?</param>
        /// <param name="MinSpeed"></param>
        /// <param name="MaxSpeed"></param>
        public Target_Spawner(Vector2 Position,Vector2 SpawnDirection ,Color Color,bool RandomizeColor,double MinFrequency,double MaxFrequency,bool RandomizeSpawnTime,float MinSpeed,float MaxSpeed,bool Enabled)
        {
            this.position = Position;
            this.spawnDirection = SpawnDirection;

            this.spawnColor = Color;
            this.randomizeColor = RandomizeColor;

            this.minFrequency = MinFrequency;
            this.maxFrequency = MaxFrequency;
            this.randomizeSpawnTimes = RandomizeSpawnTime;
            this.setNextSpawnTime();
    
            this.minSpeed = MinSpeed;
            this.maxSpeed = MaxSpeed;
            this.enabled = Enabled;
        }

        /// <summary>
        /// Sets the time until the next spawn.
        /// </summary>
        private void setNextSpawnTime()
        {
            if (this.randomizeSpawnTimes)
            {
                this.timeUntilNextSpawn = SeasideScramble.self.random.NextDouble() * this.maxFrequency;
                if (this.timeUntilNextSpawn < this.minFrequency) this.timeUntilNextSpawn = this.minFrequency;
            }
            else
            {
                this.timeUntilNextSpawn = this.maxFrequency;
            }
        }

        /// <summary>
        /// Update the logic of the spawner.
        /// </summary>
        /// <param name="time"></param>
        public void update(GameTime time)
        {
            if (this.enabled == false) return;
            this.timeUntilNextSpawn -= time.ElapsedGameTime.Milliseconds;
            if (this.timeUntilNextSpawn <= 0)
            {
                this.spawn(this.position, this.spawnDirection, this.randomizeColor ? this.getRandomColor() : this.spawnColor, (float)this.getRandomSpeed());
                this.setNextSpawnTime();
            }
        }
        public void draw(SpriteBatch b)
        {
            //Do I really want to draw a spawner???
        }


        public void turnOn()
        {
            this.enabled = true;
        }

        public void turnOff()
        {
            this.enabled = false;
        }

        /// <summary>
        /// Get a random color for the spawned target.
        /// </summary>
        /// <returns></returns>
        private Color getRandomColor()
        {
            int red = (int)(SeasideScramble.self.random.NextDouble() * 255);
            int green = (int)(SeasideScramble.self.random.NextDouble() * 255);
            int blue = (int)(SeasideScramble.self.random.NextDouble() * 255);
            Color c = new Color(red, green, blue);
            return c;
        }

        /// <summary>
        /// Get a random speed for the newly spawned target.
        /// </summary>
        /// <returns></returns>
        private double getRandomSpeed()
        {
            double speed = this.maxSpeed * SeasideScramble.self.random.NextDouble();
            if (speed < this.minSpeed) speed = this.minSpeed;
            return speed;
        }

        /// <summary>
        /// Spawns a target at this position and sends it a specific direction.
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="color"></param>
        /// <param name="Speed"></param>
        public void spawn(SSCEnums.FacingDirection Direction,Color color, float Speed)
        {
            if(Direction== SSCEnums.FacingDirection.Down)
            {
                this.spawnDown(this.position, color, Speed);
            }
            if (Direction == SSCEnums.FacingDirection.Up)
            {
                this.spawnUp(this.position, color, Speed);
            }
            if (Direction == SSCEnums.FacingDirection.Right)
            {
                this.spawnRight(this.position, color, Speed);
            }
            if (Direction == SSCEnums.FacingDirection.Left)
            {
                this.spawnLeft(this.position, color, Speed);
            }
        }

        public void spawn(Vector2 Position,Vector2 Direction,Color Color, float Speed)
        {
            SSCEnemies.SSCE_Target.Spawn_SSCE_Target(Position, Color, Direction, Speed);
        }

        public void spawnStationary(Vector2 Position, Color Color)
        {
            SSCEnemies.SSCE_Target.Spawn_SSCE_Target(Position, Color);
        }

        public void spawnLeft(Vector2 Position, Color Color,float Speed)
        {
            SSCEnemies.SSCE_Target.Spawn_SSCE_Target(Position, Color,new Vector2(-1,0),Speed);
        }
        public void spawnRight(Vector2 Position, Color Color, float Speed)
        {
            SSCEnemies.SSCE_Target.Spawn_SSCE_Target(Position, Color, new Vector2(1, 0), Speed);
        }
        public void spawnUp(Vector2 Position, Color Color, float Speed)
        {
            SSCEnemies.SSCE_Target.Spawn_SSCE_Target(Position, Color, new Vector2(0, -1), Speed);
        }
        public void spawnDown(Vector2 Position, Color Color, float Speed)
        {
            SSCEnemies.SSCE_Target.Spawn_SSCE_Target(Position, Color, new Vector2(0, 1), Speed);
        }

    }
}
