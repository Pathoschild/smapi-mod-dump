/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnimalsNeedWater.Types;
using StardewModdingAPI;

namespace AnimalsNeedWater
{
    public class TroughPlacementProfiles
    {
        public static TroughPlacementProfile DefaultProfile;
        public static List<TroughPlacementProfile> LoadedProfiles;

        public static void LoadProfiles(IModHelper helper)
        {
            var profiles = new List<TroughPlacementProfile>();
            var availableFiles = Directory.GetFiles(Path.Combine(helper.DirectoryPath, "assets/TroughPlacementProfiles")).Where(filename => filename.Contains(".json"));
            foreach (var file in availableFiles)
            {
                var profile = helper.Data.ReadJsonFile<TroughPlacementProfile>("assets/TroughPlacementProfiles/" + Path.GetFileName(file));
                
                if (Path.GetFileNameWithoutExtension(file).Contains("default"))
                {
                    DefaultProfile = profile;
                }
                else
                {
                    profiles.Add(profile);
                }
            }

            LoadedProfiles = profiles;
        }
        
        public static TroughPlacementProfile GetProfileByUniqueId(string id)
        {
            TroughPlacementProfile result = null;
            foreach (var profile in LoadedProfiles)
            {
                if (profile.modUniqueId.Contains(id, StringComparison.CurrentCultureIgnoreCase))
                {
                    result = profile;
                }
            }

            return result;
        }
    }
}