/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using NpcAdventure.StateMachine;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using static NpcAdventure.StateMachine.CompanionStateMachine;

namespace NpcAdventure
{
    /// <summary>
    /// NPC Adventures mod API
    /// </summary>
    public interface INpcAdventureModApi
    {
        /// <summary>
        /// Checks if farmer is able to recruit any companion
        /// </summary>
        /// <returns></returns>
        bool CanRecruitCompanions();

        /// <summary>
        /// Returns list of companion NPCs
        /// </summary>
        /// <returns></returns>
        IEnumerable<NPC> GetPossibleCompanions();
        
        /// <summary>
        /// Is this NPC a possible companion?
        /// </summary>
        /// <param name="npcName">NPC name</param>
        /// <returns></returns>
        bool IsPossibleCompanion(string npcName);

        /// <summary>
        /// Is this NPC a possible companion?
        /// </summary>
        /// <param name="npc">NPC instance</param>
        /// <returns></returns>
        bool IsPossibleCompanion(NPC npc);

        /// <summary>
        /// Can farmer ask this NPC to follow?
        /// </summary>
        /// <param name="npc">NPC instance</param>
        /// <returns></returns>
        bool CanAskToFollow(NPC npc);

        /// <summary>
        /// Can farmer recruit this NPC?
        /// </summary>
        /// <param name="farmer">Farmer instance</param>
        /// <param name="npc">NPC instance</param>
        /// <returns></returns>
        bool CanRecruit(Farmer farmer, NPC npc);

        /// <summary>
        /// Is this NPC recruited right now?
        /// </summary>
        /// <param name="npc">NPC instance</param>
        /// <returns></returns>
        bool IsRecruited(NPC npc);

        /// <summary>
        /// Is this NPC available for recruit?
        /// </summary>
        /// <param name="npc">NPC instance</param>
        /// <returns></returns>
        bool IsAvailable(NPC npc);

        /// <summary>
        /// Get NPC companion CSM state (as string)
        /// </summary>
        /// <param name="npc">NPC instance</param>
        /// <returns></returns>
        string GetNpcState(NPC npc);

        /// <summary>
        /// Recruit this companion to a farmer
        /// </summary>
        /// <param name="farmer">Farmer instance</param>
        /// <param name="npc">NPC instance</param>
        /// <returns></returns>
        bool RecruitCompanion(Farmer farmer, NPC npc);

        /// <summary>
        /// Load one string from strings dictionary content data asset
        /// </summary>
        /// <param name="path">Path to string in asset with whole asset name (like `Strings/Strings:companionRecruited.yes`</param>
        /// <returns>A loaded string from asset dictionary</returns>
        string LoadString(string path);

        /// <summary>
        /// Load one string from strings dictionary asset with substituions.
        /// Placeholders `{%number%}` in string wil be replaced with substitution.
        /// </summary>
        /// <param name="path">Path to string in asset with whole asset name (like `Strings/Strings:companionRecruited.yes`)</param>
        /// <param name="substitutions">A substitution for replace placeholder in string</param>
        /// <returns>A loaded string from asset dictionary</returns>
        string LoadString(string path, params object[] substitutions);

        /// <summary>
        /// Loads NPC Adventures mod content data.
        /// </summary>
        /// <typeparam name="TKey">Type of asset data keys</typeparam>
        /// <typeparam name="TValue">Type of asset data values</typeparam>
        /// <param name="path">Name of asset, like `Strings/Strings` or `Dialogue/Abigail` and etc</param>
        /// <returns></returns>
        Dictionary<TKey, TValue> LoadData<TKey, TValue>(string path);
    }

    public class NpcAdventureModApi : INpcAdventureModApi
    {
        private readonly NpcAdventureMod npcAdventureMod;

        internal NpcAdventureModApi(NpcAdventureMod npcAdventureMod)
        {
            this.npcAdventureMod = npcAdventureMod;
        }

        public bool CanRecruitCompanions()
        {
            return this.npcAdventureMod.CompanionManager.CanRecruit();
        }

        public IEnumerable<NPC> GetPossibleCompanions()
        {
            return this.npcAdventureMod.CompanionManager.PossibleCompanions.Select(s => s.Value.Companion);
        }

        public bool IsPossibleCompanion(string npcName)
        {
            return this.npcAdventureMod.CompanionManager.PossibleCompanions.ContainsKey(npcName);
        }

        public bool IsPossibleCompanion(NPC npc)
        {
            return this.IsPossibleCompanion(npc.Name);
        }

        public bool CanAskToFollow(NPC npc)
        {
            if (!this.IsPossibleCompanion(npc))
                return false;

            var csm = this.npcAdventureMod.CompanionManager.PossibleCompanions[npc.Name];

            return csm != null && csm.Name == npc.Name && csm.CurrentStateFlag == StateFlag.AVAILABLE && csm.CanPerformAction();
        }

        public bool CanRecruit(Farmer farmer, NPC npc)
        {
            if (!this.IsPossibleCompanion(npc))
                return false;

            var csm = this.npcAdventureMod.CompanionManager.PossibleCompanions[npc.Name];

            return farmer.getFriendshipHeartLevelForNPC(npc.Name) >= csm.CompanionManager.Config.HeartThreshold && Game1.timeOfDay < 2200;
        }

        public bool IsRecruited(NPC npc)
        {
            if (!this.IsPossibleCompanion(npc))
                return false;
            var csm = this.npcAdventureMod.CompanionManager.PossibleCompanions[npc.Name];
            return csm != null && csm.Name == npc.Name && csm.CurrentStateFlag == StateFlag.RECRUITED;
        }

        public bool IsAvailable(NPC npc)
        {
            if (!this.IsPossibleCompanion(npc))
                return false;

            var csm = this.npcAdventureMod.CompanionManager.PossibleCompanions[npc.Name];

            return csm != null 
                && csm.Name == npc.Name 
                && csm.CurrentStateFlag == StateFlag.AVAILABLE;
        }

        public string GetNpcState(NPC npc)
        {
            if (!this.IsPossibleCompanion(npc))
                return null;

            var csm = this.npcAdventureMod.CompanionManager.PossibleCompanions[npc.Name];

            if (csm != null && csm.Name == npc.Name)
                return Enum.GetName(typeof(StateFlag), csm.CurrentStateFlag);

            return null;
        }

        public bool RecruitCompanion(Farmer farmer, NPC npc)
        {
            var csm = this.npcAdventureMod.CompanionManager.PossibleCompanions[npc.Name];

            if (this.CanRecruitCompanions() && this.CanAskToFollow(npc) && this.CanRecruit(farmer, npc))
            {
                csm.Recruit();

                return true;
            }

            return false;
        }

        public string LoadString(string path)
        {
            return this.npcAdventureMod.ContentLoader.LoadString(path);
        }

        public string LoadString(string path, params object[] substitutions)
        {
            return this.npcAdventureMod.ContentLoader.LoadString(path, substitutions);
        }

        public Dictionary<TKey, TValue> LoadData<TKey, TValue>(string path)
        {
            return this.npcAdventureMod.ContentLoader.LoadData<TKey, TValue>(path);
        }
    }
}