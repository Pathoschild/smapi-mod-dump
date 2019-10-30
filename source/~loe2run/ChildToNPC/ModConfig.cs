using System.Collections.Generic;

namespace ChildToNPC
{
    /* The ModConfig currently covers ageForCP,
     * the age at which the CP mod will patch over the child.
     * The Default is currently 83.
     * (14 days as newborn, 14 days as baby, 28 days as crawler,
     *  then I'll say 28 days as a toddler
     *  on the assumption that CP mods will make them a child.)
     */ 
    class ModConfig
    {
        public int AgeWhenKidsAreModified { get; set; }
        public Dictionary<string, string> ChildParentPairs { get; set; }

        public ModConfig()
        {
            AgeWhenKidsAreModified = 83;
            ChildParentPairs = new Dictionary<string, string>();
        }
    }
}