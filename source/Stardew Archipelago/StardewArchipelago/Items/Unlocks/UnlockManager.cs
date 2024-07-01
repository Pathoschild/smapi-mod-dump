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
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Items.Unlocks.Modded;
using StardewArchipelago.Items.Unlocks.Vanilla;
using StardewArchipelago.Locations;

namespace StardewArchipelago.Items.Unlocks
{
    public class UnlockManager
    {
        private List<IUnlockManager> _specificUnlockManagers;
        private Dictionary<string, Func<ReceivedItem, LetterAttachment>> _unlockables;

        public UnlockManager(ArchipelagoClient archipelago, LocationChecker locationChecker)
        {
            _unlockables = new Dictionary<string, Func<ReceivedItem, LetterAttachment>>();
            _specificUnlockManagers = new List<IUnlockManager>();
            _specificUnlockManagers.Add(new VanillaUnlockManager(archipelago, locationChecker));

            if (archipelago.SlotData.Mods.IsModded)
            {
                _specificUnlockManagers.Add(new ModUnlockManager(archipelago));
            }

            RegisterUnlocks();
        }

        private void RegisterUnlocks()
        {
            foreach (var specificUnlockManager in _specificUnlockManagers)
            {
                specificUnlockManager.RegisterUnlocks(_unlockables);
            }
        }

        public bool IsUnlock(string unlockName)
        {
            return _unlockables.ContainsKey(unlockName);
        }

        public LetterAttachment PerformUnlockAsLetter(ReceivedItem unlock)
        {
            if (IsUnlock(unlock.ItemName))
            {
                return _unlockables[unlock.ItemName](unlock);
            }

            throw new ArgumentException($"Could not perform unlock '{unlock.ItemName}'");
        }
    }
}
