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
using Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCEnemies;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCEntities
{
    public class SSCEntityManager
    {
        public SSCProjectiles.SSCProjectileManager projectiles;
        public SSCEnemies.EnemyManager enemies;
        public List<KillZone> killZones;

        public SSCEntityManager()
        {
            this.projectiles = new SSCProjectiles.SSCProjectileManager();
            this.enemies = new EnemyManager();
            this.killZones = new List<KillZone>();
        }

        public void addProjectile(SSCProjectiles.SSCProjectile projectile)
        {
            this.projectiles.addProjectile(projectile);
        }

        public void addEnemy(SSCEnemies.SSCEnemy enemy)
        {
            this.enemies.addEnemy(enemy);
        }

        public void addSpawner(ISpawner spawner)
        {
            this.enemies.addSpawner(spawner);
        }

        public void addKillZone(KillZone zone)
        {
            this.killZones.Add(zone);
        }

        public void update(GameTime time)
        {
            this.projectiles.update(time);
            this.enemies.update(time);            
        }

        public void draw(SpriteBatch b)
        {
            this.projectiles.draw(b);
            this.enemies.draw(b);
        }
        
    }
}
