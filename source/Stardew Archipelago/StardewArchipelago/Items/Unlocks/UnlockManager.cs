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
using System.Linq;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Constants;

namespace StardewArchipelago.Items.Unlocks
{
    public class UnlockManager
    {
        private List<IUnlockManager> _specificUnlockManagers;

        public UnlockManager(ArchipelagoClient archipelago)
        {
            _specificUnlockManagers = new List<IUnlockManager>();
            _specificUnlockManagers.Add(new VanillaUnlockManager());
            if (archipelago.SlotData.Mods.HasModdedSkill())
            {
                _specificUnlockManagers.Add(new ModSkillUnlockManager());
                
            }
            if (archipelago.SlotData.Mods.HasMod(ModNames.MAGIC))
            {
                _specificUnlockManagers.Add(new MagicUnlockManager());
            }
            if (archipelago.SlotData.Mods.HasMod(ModNames.SVE))
            {
                _specificUnlockManagers.Add(new SVEUnlockManager());
            }
        }

        public bool IsUnlock(string unlockName)
        {
            return _specificUnlockManagers.Any(specificUnlockManager => specificUnlockManager.IsUnlock(unlockName));
        }

        public LetterAttachment PerformUnlockAsLetter(ReceivedItem unlock)
        {
            foreach (var specificUnlockManager in _specificUnlockManagers)
            {
                if (specificUnlockManager.IsUnlock(unlock.ItemName))
                {
                    return specificUnlockManager.PerformUnlockAsLetter(unlock);
                }
            }

            throw new ArgumentException($"Could not perform unlock '{unlock.ItemName}'");
        }
    }
}
