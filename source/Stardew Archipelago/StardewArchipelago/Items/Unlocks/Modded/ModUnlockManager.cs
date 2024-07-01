/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Items.Mail;

namespace StardewArchipelago.Items.Unlocks.Modded
{
    public class ModUnlockManager : IUnlockManager
    {
        private List<IUnlockManager> _childUnlockManagers;

        public ModUnlockManager(ArchipelagoClient archipelago)
        {
            _childUnlockManagers = new List<IUnlockManager>();

            if (archipelago.SlotData.Mods.HasModdedSkill())
            {
                _childUnlockManagers.Add(new ModSkillUnlockManager());
            }
            if (archipelago.SlotData.Mods.HasMod(ModNames.MAGIC))
            {
                _childUnlockManagers.Add(new MagicUnlockManager());
            }
            if (archipelago.SlotData.Mods.HasMod(ModNames.SVE))
            {
                _childUnlockManagers.Add(new SVEUnlockManager());
            }
        }

        public void RegisterUnlocks(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            foreach (var childUnlockManager in _childUnlockManagers)
            {
                childUnlockManager.RegisterUnlocks(unlocks);
            }
        }
    }
}
