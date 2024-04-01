/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/PolyamorySweetLove
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;

namespace PolyamorySweetLove
{
    public partial class ModEntry
    {
       
        public static IPlannedParenthoodAPI plannedParenthoodAPI;
        public static IContentPatcherAPI contentPatcherAPI;

        public static void LoadModApis()
        {
           
            plannedParenthoodAPI = SHelper.ModRegistry.GetApi<IPlannedParenthoodAPI>("aedenthorn.PlannedParenthood");

            if (plannedParenthoodAPI != null)
            {
                SMonitor.Log("PlannedParenthood API loaded");
            }
            contentPatcherAPI = SHelper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if(contentPatcherAPI is not null)
            {
                contentPatcherAPI.RegisterToken(context.ModManifest, "PlayerSpouses", () =>
                {
                    Farmer player;

                    if (Context.IsWorldReady)
                        player = Game1.player;
                    else if (SaveGame.loaded?.player != null)
                        player = SaveGame.loaded.player;
                    else
                        return null;

                    var spouses = GetSpouses(player, true).Keys.ToList();
                    spouses.Sort(delegate (string a, string b) {
                        player.friendshipData.TryGetValue(a, out Friendship af);
                        player.friendshipData.TryGetValue(b, out Friendship bf);
                        if (af == null && bf == null)
                            return 0;
                        if (af == null)
                            return -1;
                        if (bf == null)
                            return 1;
                        if (af.WeddingDate == bf.WeddingDate)
                            return 0;
                        return af.WeddingDate > bf.WeddingDate ? -1 : 1;
                    });
                    return spouses.ToArray();
                });
            }
        }
    }
}