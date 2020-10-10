/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.LookupAnything.Framework.Data
{
    /// <summary>Provides override metadata about a game NPC.</summary>
    internal class CharacterData
    {
        /*********
        ** Accessors
        *********/
        /****
        ** Identify object
        ****/
        /// <summary>The NPC identifier, like "Horse" (any NPCs of type Horse) or "Villager::Gunther" (any NPCs of type Villager with the name "Gunther").</summary>
        public string ID { get; set; }


        /****
        ** Overrides
        ****/
        /// <summary>The translation key which should override the NPC description (if any).</summary>
        public string DescriptionKey { get; set; }
    }
}
