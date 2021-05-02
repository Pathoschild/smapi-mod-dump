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

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCEnemies
{
    public class EnemyManager
    {
        public List<SSCEnemy> enemies;
        private List<SSCEnemy> garbageCollection;

        private List<ISpawner> spawnerGarbageCollection;
        public List<ISpawner> spawners;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EnemyManager()
        {
            this.enemies = new List<SSCEnemy>();
            this.garbageCollection = new List<SSCEnemy>();

            this.spawners = new List<ISpawner>();
            this.spawnerGarbageCollection = new List<ISpawner>();
        }

        /// <summary>
        /// Adds an enemy to the game.
        /// </summary>
        /// <param name="enemy"></param>
        public void addEnemy(SSCEnemy enemy)
        {
            this.enemies.Add(enemy);
        }
        /// <summary>
        /// Removes an enemy from the game.
        /// </summary>
        /// <param name="enemy"></param>
        public void removeEnemy(SSCEnemy enemy)
        {
            this.garbageCollection.Add(enemy);
        }

        public void addSpawner(ISpawner spawner)
        {
            this.spawners.Add(spawner);
        }
        public void removeSpawner(ISpawner spawner)
        {
            this.spawnerGarbageCollection.Add(spawner);
        }

        /// <summary>
        /// Update all enemies.
        /// </summary>
        /// <param name="time"></param>
        public void update(GameTime time)
        {

            foreach(ISpawner spawner in this.spawnerGarbageCollection)
            {
                this.spawners.Remove(spawner);
            }
            foreach(ISpawner spawner in this.spawners)
            {
                spawner.update(time);
            }

            foreach(SSCEnemy enemy in this.garbageCollection)
            {
                this.enemies.Remove(enemy);
            }
            foreach(SSCEnemy enemy in this.enemies)
            {
                enemy.update(time);
                if (enemy.shouldDie) this.removeEnemy(enemy);
                //Delete enemies that are too far off screen.
                Vector2 mapSize = SeasideScramble.self.currentMap.getPixelSize();
                if (enemy.Position.X>mapSize.X*2 || enemy.Position.X<-mapSize.X || enemy.Position.Y>mapSize.Y*2|| enemy.Position.Y < -mapSize.Y)
                {
                    enemy.shouldDie = true; //Silently remove the enemy from the map.
                    this.removeEnemy(enemy);
                }
            }
            
        }

        /// <summary>
        /// Draw all enemies to the screen.
        /// </summary>
        /// <param name="b"></param>
        public void draw(SpriteBatch b)
        {
            foreach (SSCEnemy enemy in this.enemies)
            {
                enemy.draw(b);
            }
            foreach(ISpawner spawner in this.spawners)
            {
                spawner.draw(b);
            }
        }

    }
}
