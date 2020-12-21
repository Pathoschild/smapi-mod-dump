/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gathouria/Adopt-Skin
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoptSkin
{
    class ModConfig
    {
        /// <summary>Whether or not to allow horses being ridden to fit through any area that the player can normally walk through</summary>
        public bool OneTileHorse { get; set; } = true;
        public bool PetAndHorseNameTags { get; set; } = true;
        public string HorseWhistleKey { get; set; } = "R";
        public string CorralKey { get; set; } = "G";


        public int StrayAdoptionPrice { get; set; } = 1000;
        /// <summary>Whether or not the player can walk through pets on the farm</summary>
        public bool WalkThroughPets { get; set; } = false;
        /// <summary>Whether or not pets will be teleported around the farmhouse at the beginning of the day (true) or will surround the water dish as normal (false)</summary>
        public bool DisperseCuddlePuddle { get; set; } = true;
        /// <summary>The radius of the space through which pets can be dispersed from their original spawn location at the water dish. Default 5.</summary>
        public int CuddleExplosionRadius { get; set; } = 5;



        /// <summary>Determines whether wild adoptable horses can spawn in the map after the player obtains a stable</summary>
        public bool WildHorseSpawn { get; set; } = true;
        /// <summary>Determines whether stray pets will appear at Marnie's after the player obtains a pet</summary>
        public bool StraySpawn { get; set; } = true;



        /// <summary>Sets the locations that wild horses may spawn at</summary>
        public List<string> WildHorseSpawnLocations { get; set; } = new List<string> { "Forest", "BusStop", "Mountain", "Town", "Railroad", "Beach" };



        /// <summary>The percentage chance that a WildHorse will spawn. 25% by default.</summary>
        public int WildHorseChancePercentage { get; set; } = 25;
        /// <summary>The percentage chance that a Stray will spawn. 60% by default.</summary>
        public int StrayChancePercentage { get; set; } = 60;
        /// <summary>Whether or not the Stray and Wild Horse spawn chances are affected by daily luck values (the effect is up to 10% difference)</summary>
        public bool ChanceAffectedByLuck { get; set; } = true;




        /// <summary>Whether or not to tell the player IF a WildHorse has spawned on the map. Will be FALSE by default</summary>
        public bool NotifyHorseSpawn { get; set; } = false;
        /// <summary>Whether or not to tell the player WHERE a WildHorse has spawned on the map. Will be FALSE by default</summary>
        public bool NotifyHorseSpawnLocation { get; set; } = false;
        /// <summary>Whether or not to tell the player when a Stray has spawned at Marnie's. Will be FALSE by default</summary>
        public bool NotifyStraySpawn { get; set; } = false;




        /// <summary>Whether or not to allow debugging commands for Adopt & Skin. Will be FALSE by default</summary>
        public bool DebuggingMode { get; set; } = false;
    }
}
