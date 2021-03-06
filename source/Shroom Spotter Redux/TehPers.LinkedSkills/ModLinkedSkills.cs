/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.Core.Multiplayer;
using TehPers.Core.Multiplayer.Synchronized;
using TehPers.Core.Multiplayer.Synchronized.Collections;
using TehPers.Core.Rewrite;

namespace TehPers.LinkedSkills {
    public class ModLinkedSkills : Mod {
        public override void Entry(IModHelper helper) {
            IMultiplayerApi multiplayerApi = this.GetCoreApi().GetMultiplayerApi();

            // Sync all the levels
            this.SyncLevel(multiplayerApi, "combatLevel", () => Game1.player.combatLevel.Value, n => Game1.player.combatLevel.Value = n);
            this.SyncLevel(multiplayerApi, "farmingLevel", () => Game1.player.farmingLevel.Value, n => Game1.player.farmingLevel.Value = n);
            this.SyncLevel(multiplayerApi, "fishingLevel", () => Game1.player.fishingLevel.Value, n => Game1.player.fishingLevel.Value = n);
            this.SyncLevel(multiplayerApi, "foragingLevel", () => Game1.player.foragingLevel.Value, n => Game1.player.foragingLevel.Value = n);
            this.SyncLevel(multiplayerApi, "miningLevel", () => Game1.player.miningLevel.Value, n => Game1.player.miningLevel.Value = n);
            this.SyncLevel(multiplayerApi, "luckLevel", () => Game1.player.luckLevel.Value, n => Game1.player.luckLevel.Value = n);

            // Sync the exp
            SynchronizedList<int> expList = new SynchronizedList<int>(n => n.MakeSynchronized());
            multiplayerApi.Synchronize("expList", expList);
            foreach (int exp in Game1.player.experiencePoints) {
                expList.Add(exp);
            }
            GameEvents.UpdateTick += (sender, args) => {
                for (int i = 0; i < Game1.player.experiencePoints.Count; i++) {
                    int cur = Game1.player.experiencePoints[i];
                    int synced = expList[i];

                    // Choose whichever is higher
                    if (cur < synced) {
                        Game1.player.experiencePoints[i] = synced;
                    } else if (synced < cur) {
                        expList[i] = cur;
                    }
                }
            };
        }

        private void SyncLevel(IMultiplayerApi multiplayerApi, string name, Func<int> getter, Action<int> setter) {
            ISynchronizedWrapper<int> synchronized = getter().MakeSynchronized();
            multiplayerApi.Synchronize(name, synchronized);
            GameEvents.UpdateTick += (sender, up) => {
                int cur = getter();

                // Choose whichever is higher
                if (cur < synchronized.Value) {
                    setter(synchronized.Value);
                } else if (synchronized.Value < cur) {
                    synchronized.Value = cur;
                }
            };
        }
    }
}
