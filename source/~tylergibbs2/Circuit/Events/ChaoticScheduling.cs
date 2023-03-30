/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Circuit.Extensions;
using Circuit.VirtualProperties;
using StardewValley;

namespace Circuit.Events
{
    internal class ChaoticScheduling : EventBase
    {
        private List<string> BlacklistedNpcs { get; } = new()
        {
            "Mister Qi"
        };

        private Dictionary<string, string> NPCNameMap { get; } = new();

        public override bool ContinueUntilSleep => true;

        public ChaoticScheduling(EventType eventType) : base(eventType) { }

        public override string GetDisplayName()
        {
            return "Chaotic Scheduling";
        }

        public override string GetChatWarningMessage()
        {
            return "A strange feeling washes over the town...";
        }

        public override string GetChatStartMessage()
        {
            return "It seems like everyone has forgotten what they're doing!";
        }

        public override string GetDescription()
        {
            return "All villager schedules are randomized.";
        }

        private void MapNPCsToNewNames()
        {
            List<string> AvailableNames = new();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc.isVillager() && !BlacklistedNpcs.Contains(npc.Name))
                    AvailableNames.Add(npc.Name);
            }

            AvailableNames = AvailableNames.Distinct().ToList();

            List<string> ShuffledNames = new(AvailableNames);
            ShuffledNames.Shuffle();

            for (int i = 0; i < AvailableNames.Count; i++)
                NPCNameMap.Add(AvailableNames[i], ShuffledNames[i]);
        }

        public string GetNpcOriginalName(string swappedName)
        {
            return NPCNameMap.First(i => i.Value == swappedName).Key;
        }

        public string GetNpcSwappedName(string originalName)
        {
            return NPCNameMap[originalName];
        }

        public void SwapNpc(NPC npc)
        {
            if (npc.get_NPCIsSwapped() || BlacklistedNpcs.Contains(npc.Name))
                return;

            npc.Name = GetNpcSwappedName(npc.Name);
            npc.InvalidateMasterSchedule();

            npc.set_NPCIsSwapped(true);
        }

        public void UnswapNpc(NPC npc)
        {
            if (!npc.get_NPCIsSwapped())
                return;

            npc.Name = GetNpcOriginalName(npc.Name);

            npc.InvalidateMasterSchedule();
            npc.reloadSprite();
            npc.resetPortrait();

            npc.set_NPCIsSwapped(false);
        }

        public override void StartEvent()
        {
            MapNPCsToNewNames();
        }
    }
}
