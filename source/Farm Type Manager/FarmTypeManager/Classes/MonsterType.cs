/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A container for a monster's name and a set of optional customization settings.</summary>
        public class MonsterType
        {
            public string MonsterName { get; set; } = "";

            private Dictionary<string, object> settings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            public Dictionary<string, object> Settings
            {
                get
                {
                    return settings;
                }
                set
                {
                    if (value == null) //if the provided dictionary doesn't exist
                    {
                        //create a new dictionary with a case-insensitive comparer
                        settings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    }
                    else if (value.Comparer != StringComparer.OrdinalIgnoreCase) //if the provided dictionary exists, but isn't using a case-insensitive comparer
                    {
                        //copy the provided dictionary and use a case-insensitive comparer
                        settings = new Dictionary<string, object>(value, StringComparer.OrdinalIgnoreCase);
                    }
                    else //if the provided dictionary exists is using the correct comparer
                    {
                        settings = value;
                    }
                }
            }

            /// <summary>Initalizes a monster type with a blank name and default customization settings.</summary>
            public MonsterType()
            {

            }

            /// <summary>Initalizes a monster type with the provided name and default customization settings.</summary>
            /// <param name="monsterName">A name respresenting a known monster type.</param>
            public MonsterType(string monsterName)
            {
                MonsterName = monsterName;
            }

            /// <summary>Initalizes a monster type with the provided name and dictionary of optional customization settings.</summary>
            /// <param name="monsterName">A name respresenting a known monster type.</param>
            /// <param name="settings">A dictionary of optional setting names and values. The type of each value varies between settings.</param>
            public MonsterType(string monsterName, Dictionary<string, object> settings)
            {
                MonsterName = monsterName;
                Settings = settings;
            }
        }
    }
}