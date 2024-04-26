/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Event;
using StardewDruid.Monster.Boss;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Constants;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace StardewDruid.Cast.Weald
{
    public class WispEvent : EventHandle
    {

        public Dictionary<Vector2, WispHandle> wisps = new();

        new public string eventId = "wisps";

        new public int expireIn = 30;

        public WispEvent()
            : base()
        {

        }

        public static Vector2 WispVector(Vector2 target)
        {

            return new((int)(target.X / 15), (int)(target.Y / 15));

        }

        public override bool EventActive()
        {

            if (expireEarly)
            {

                return false;

            }

            if (Game1.player.currentLocation.Name != location.Name)
            {

                return false;

            }

            if (expireTime > Game1.currentGameTime.TotalGameTime.TotalSeconds)
            {

                return true;

            }

            if (wisps.Count > 0)
            {

                expireTime += 30;

            }

            return false;

        }

        public void UpdateWisp(Vector2 castVector, int level)
        {

            Vector2 vector = new((int)(castVector.X / 15), (int)(castVector.Y / 15));

            if (wisps.ContainsKey(vector))
            {

                if (wisps[vector].source < level)
                {

                    Vector2 tile = wisps[vector].tile;

                    wisps[vector].shutdown();

                    wisps.Remove(vector);

                    wisps[vector] = new(location, tile, level, 20);

                }

            }

        }

        public override void EventInterval()
        {

            activeCounter++;

            if (activeCounter % 3 != 0)
            {

                return;

            }

            for (int i = wisps.Count - 1; i >= 0; i--)
            {

                KeyValuePair<Vector2, WispHandle> moment = wisps.ElementAt(i);

                if(moment.Value.activation > 0)
                {

                    moment.Value.activation--;

                }

                if (activeCounter % 6 == 0)
                {
                    if (!moment.Value.reset())
                    {

                        wisps.Remove(moment.Key);

                    }

                }

            }

            float threshold = 300;

            float damage = Mod.instance.CombatDamage();

            foreach (NPC nonPlayableCharacter in location.characters)
            {

                if (nonPlayableCharacter is StardewValley.Monsters.Monster monster)
                {

                    if (monster.IsInvisible || monster.Health <= 0)
                    {

                        continue;

                    }

                    if (monster.isInvincible())
                    {
                        continue;
                    }

                    Vector2 tileVector = new((int)(monster.Position.X / 64), (int)(monster.Position.Y / 64));

                    Vector2 terrainVector = WispVector(tileVector);

                    if (!wisps.ContainsKey(terrainVector))
                    {

                        continue;

                    };

                    if (wisps[terrainVector].timer == 0)
                    {

                        continue;

                    }

                    float monsterThreshold = threshold;

                    if (monster.Sprite.SpriteWidth > 16)
                    {
                        monsterThreshold += 32f;
                    }

                    if (monster.Sprite.SpriteWidth > 32)
                    {

                        monsterThreshold += 32f;

                    }

                    if (Vector2.Distance(monster.Position, wisps[terrainVector].position) < monsterThreshold)
                    {

                        switch (wisps[terrainVector].source)
                        {

                            case 1:

                                if (monster.stunTime.Value < 1500)
                                {

                                    monster.stunTime.Set(1500);

                                    Microsoft.Xna.Framework.Rectangle boundingBox = monster.GetBoundingBox();

                                    location.debris.Add(new Debris("stun", 1, new Vector2(boundingBox.Center.X + 16, boundingBox.Center.Y), Microsoft.Xna.Framework.Color.Green,1,0));

                                    if (wisps[terrainVector].activation <= 0)
                                    {

                                        Mod.instance.iconData.DecorativeIndicator(location, wisps[terrainVector].position, Data.IconData.decorations.weald, 0.5f);

                                        wisps[terrainVector].activation = 1;

                                    }

                                }

                                break;

                            /*case 2:

                                new Mists.Smite(monster.Tile, monster, damage / 2).CastEffect();

                                if (wisps[terrainVector].activation <= 0)
                                {

                                    ModUtility.AnimateDecoration(location, wisps[terrainVector].position, "mists", 0.5f);

                                    wisps[terrainVector].activation = 1;

                                }

                                break;

                            case 3:

                                Mod.instance.rite.CastComet(wisps[terrainVector].location, wisps[terrainVector].tile);

                                wisps[terrainVector].timer = 0;

                                break;*/

                        }

                    }

                }

            }

        }

    }

}
