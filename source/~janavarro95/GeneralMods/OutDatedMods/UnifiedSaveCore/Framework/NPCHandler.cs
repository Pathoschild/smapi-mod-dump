/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedSaveCore.Framework
{
    public class NPCHandler:IInformationHandler
    {
        /// <summary>
        /// Deals with characters at specific locations.
        /// </summary>
        Dictionary<GameLocation, List<NPC>> characterLocations;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NPCHandler()
        {
            characterLocations = new Dictionary<GameLocation, List<NPC>>();
        }

        /// <summary>
        /// Removes all custom characters before saving.
        /// </summary>
        public void beforeSave()
        {
            foreach (var loc in Game1.locations)
            {
                List<NPC> characters = new List<NPC>();
                foreach (var character in loc.characters)
                {
                    foreach (Type t in UnifiedSaveCore.modTypes)
                    {
                        if (character.GetType().ToString() == t.ToString())
                        {
                            characters.Add(character);
                        }
                    }

                }
                foreach (var character in characters)
                {
                    loc.characters.Remove(character);
                }
                characterLocations.Add(loc, characters);
            }
        }

        /// <summary>
        /// Restores all custom characters after saving.
        /// </summary>
        public void afterSave()
        {
            foreach(KeyValuePair<GameLocation,List<NPC>> pair in characterLocations)
            {
                foreach(var npc in pair.Value)
                {
                    pair.Key.characters.Add(npc);
                }
            }
            characterLocations.Clear();
        }

        public void afterLoad()
        {
            throw new NotImplementedException();
        }
    }
}
