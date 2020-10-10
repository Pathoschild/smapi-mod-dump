/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley.Monsters;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A class containing all necessary information about an in-game object.</summary>
        public class SavedObject
        {
            /// <summary>The name of the in-game location where this object exists.</summary>
            public string MapName { get; set; } = null;
            /// <summary>The name of this object. Used for ID generation and log messages.</summary>
            public string Name { get; set; } = null;
            /// <summary>A tile indicating where this object exists.</summary>
            public Vector2 Tile { get; set; } = default(Vector2);
            /// <summary>The enumerated spawn type of the object.</summary>
            public ObjectType Type { get; set; } = default(ObjectType);
            /// <summary>The ID of this object. Also known as index or parentSheetIndex.</summary>
            public int? ID { get; set; } = null;
            /// <summary>The remaining number of days before this object should be removed from the game.</summary>
            public int? DaysUntilExpire { get; set; } = null;
            /// <summary>The specific in-game time at which this object will spawn.</summary>
            public StardewTime SpawnTime { get; set; } = 600; //default to 6:00AM
            /// <summary>The list of definitions for this saved object's contents. Null if this type does not use the ConfigItem format.</summary>
            public ConfigItem ConfigItem { get; set; } = null;
            /// <summary>The monster type of this saved object. Null if this type is not a monster.</summary>
            public MonsterType MonType { get; set; } = null;

            /// <summary>A cache of monster classes' tile sizes. Used to improve the speed of the Size property.</summary>
            [JsonIgnore]
            private static Dictionary<string, Point> MonsterSizeCache = new Dictionary<string, Point>();

            /// <summary>A point representing this object's size in tiles.</summary>
            [JsonIgnore]
            public Point Size
            {
                get
                {
                    switch (Type)
                    {
                        case ObjectType.Object:
                        case ObjectType.Item:
                        case ObjectType.Ore:
                            return new Point(1, 1);
                        case ObjectType.LargeObject:
                            if (ID == 190 || ID == 254 || ID == 276) //if this seems to be a giant crop
                                return new Point(3, 3);
                            else //if this seems to be a resource clump
                                return new Point(2, 2);
                        case ObjectType.Monster:
                            if (!MonsterSizeCache.ContainsKey(MonType.MonsterName)) //if this monster type's size has not been cached yet
                            {
                                if (MonType.MonsterName.Contains(".")) //if this is an external monster class
                                {
                                    Type externalType = Utility.GetTypeFromName(MonType.MonsterName, typeof(Monster)); //find a monster subclass with a matching name
                                    Monster monster = (Monster)Activator.CreateInstance(externalType, Vector2.Zero); //create a monster with the Vector2 constructor (the tile should be irrelevant)
                                    int width = Convert.ToInt32(Math.Ceiling(((double)monster.Sprite.SpriteWidth) / 16)); //get the monster's sprite width in tiles, rounded up
                                    if (width <= 0)
                                        width = 1; //enforce minimum 1x1 size
                                    MonsterSizeCache.Add(MonType.MonsterName, new Point(width, width)); //use the width for both dimensions (to avoid problems with "tall" 1x1 monster sprites)
                                }
                                else if //if this is a known type of 2x2 monster
                                (
                                    MonType.MonsterName.StartsWith("big", StringComparison.OrdinalIgnoreCase) ||
                                    MonType.MonsterName.StartsWith("serpent", StringComparison.OrdinalIgnoreCase) ||
                                    MonType.MonsterName.StartsWith("rex", StringComparison.OrdinalIgnoreCase) ||
                                    MonType.MonsterName.StartsWith("pepper", StringComparison.OrdinalIgnoreCase) ||
                                    MonType.MonsterName.StartsWith("dino", StringComparison.OrdinalIgnoreCase)
                                )
                                    MonsterSizeCache.Add(MonType.MonsterName, new Point(2, 2));
                                else //if this is a known type of 1x1 monster
                                    MonsterSizeCache.Add(MonType.MonsterName, new Point(1, 1));
                            }

                            return MonsterSizeCache[MonType.MonsterName];
                        default:
                            return new Point(1, 1); //use default size for unknown enum values
                    }
                }
            }

            /// <summary>Enumerated list of general object types managed by this mod. Used to process saved object information.</summary>
            public enum ObjectType
            {
                Object, Forage = Object,
                ResourceClump, LargeObject = ResourceClump,
                Ore,
                Monster,
                Item,
                Container
            }

            public SavedObject()
            {
            }
        }
    }
}