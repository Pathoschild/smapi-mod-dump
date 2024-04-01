/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;


namespace LittleNPCs.Framework {
    internal record LittleNPCInfo {
        public enum LoadState { None, LittleNPC, Child, SaveGame }

        private IMonitor monitor_;

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public Gender Gender { get; private set; }

        public SDate Birthday { get; private set; }

        public LoadState LoadedFrom { get; private set; }

        public LittleNPCInfo(int childIndex, IMonitor monitor) {
            monitor_ = monitor;
            if (Context.IsWorldReady) {
                var littleNPC = ModEntry.GetLittleNPC(childIndex);
                if (littleNPC is not null) {
                    // Internal asset name of a LittleNPC already has a prefix.
                    Name = littleNPC.Name;
                    DisplayName = littleNPC.displayName;
                    Gender = littleNPC.Gender;
                    Birthday = littleNPC.GetBirthday();
                    LoadedFrom = LoadState.LittleNPC;
                    monitor_.VerboseLog($"GetLittleNPC({childIndex}) returns {Name}");
                }
                else {
                    // No LittleNPC, try to get Child object.
                    var children = GetChildrenFromFarmHouse(false, out FarmHouse farmHouse);
                    Child child = children.FirstOrDefault(c => c.daysOld.Value >= ModEntry.config_.AgeWhenKidsAreModified && c.GetChildIndex() == childIndex);
                    if (child is not null) {
                        Name = CreateInternalAssetName(childIndex, child.Name);
                        DisplayName = child.Name;
                        Gender = child.Gender;
                        Birthday = LittleNPC.GetBirthday(child);
                        LoadedFrom = LoadState.Child;
                        monitor_.VerboseLog($"Query for convertible child with index {childIndex} returns {Name}");
                    }
                    else {
                        monitor_.VerboseLog($"Query for convertible child with index {childIndex} returns null");
                    }
                }
            }
            else {
                // World not ready, load from save.
                var children = GetChildrenFromFarmHouse(true, out FarmHouse farmHouse);
                Child child = children.FirstOrDefault(c => c.daysOld.Value >= ModEntry.config_.AgeWhenKidsAreModified && c.GetChildIndex() == childIndex);
                if (child is not null) {
                    Name = CreateInternalAssetName(childIndex, child.Name);
                    DisplayName = child.Name;
                    Gender = child.Gender;
                    Birthday = LittleNPC.GetBirthday(child);
                    LoadedFrom = LoadState.SaveGame;
                    monitor_.VerboseLog($"Query for convertible child with index {childIndex} returns {Name}");
                }
                else {
                    monitor_.VerboseLog($"Query for convertible child with index {childIndex} returns null");
                }
            }
        }

        private static IEnumerable<Child> GetChildrenFromFarmHouse(bool loadFromSave, out FarmHouse farmHouse) {
            farmHouse = loadFromSave ? SaveGame.loaded?.locations.OfType<FarmHouse>().FirstOrDefault(l => l.Name == "FarmHouse")
                                     : Utility.getHomeOfFarmer(Game1.player);
            
            return farmHouse is not null ? farmHouse.getChildren()
                                         : Enumerable.Empty<Child>();
        }

        public static string CreateInternalAssetName(int childIndex, string childName) {
            // Internal asset name to create a LittleNPC from needs a prefix.
            string prefix = childIndex == 0 ? "FirstLittleNPC" : "SecondLittleNPC";
            
            // Remove spaces. This could to equal names but there's still the prefix to distinguish them. 
            string sanitizedChildName = childName.Replace(' ', '_');
            
            return $"{prefix}{sanitizedChildName}{Game1.player.UniqueMultiplayerID}";
        }
    }
}
