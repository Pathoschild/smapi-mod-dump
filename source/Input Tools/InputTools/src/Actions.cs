/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sagittaeri/StardewValleyMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using InputTools;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;


namespace InputTools
{
    public class Actions
    {
        private InputToolsAPI inputTools;
        private Dictionary<string, List<Tuple<SButton, SButton>>> allActions = new Dictionary<string, List<Tuple<SButton, SButton>>>();
        private Dictionary<Tuple<SButton, SButton>, List<string>> allActionsByKeys = new Dictionary<Tuple<SButton, SButton>, List<string>>();

        public Actions(InputToolsAPI inputTools)
        {
            this.inputTools = inputTools;
        }

        public void RegisterAction(string actionID, params SButton[] keyTriggers)
        {
            if (string.IsNullOrWhiteSpace(actionID) || keyTriggers == null || keyTriggers.Length == 0)
                return;
            Tuple<SButton, SButton>[] keyTriggerTuples = new Tuple<SButton, SButton>[keyTriggers.Length];
            for (int i=0; i< keyTriggers.Length; i++)
                keyTriggerTuples[i] = new Tuple<SButton, SButton>(keyTriggers[i], SButton.None);
            this.RegisterAction(actionID, keyTriggerTuples);
        }

        public void RegisterAction(string actionID, params Tuple<SButton, SButton>[] keyTriggers)
        {
            if (string.IsNullOrWhiteSpace(actionID) || keyTriggers == null || keyTriggers.Length == 0)
                return;
            if (!this.allActions.ContainsKey(actionID))
                this.allActions[actionID] = new List<Tuple<SButton, SButton>>();
            foreach (Tuple<SButton, SButton> keyTrigger in keyTriggers)
            {
                if (keyTrigger.Item1 != SButton.None)
                {
                    if (!this.allActions[actionID].Contains(keyTrigger))
                        this.allActions[actionID].Add(keyTrigger);
                    if (!this.allActionsByKeys.ContainsKey(keyTrigger))
                        this.allActionsByKeys[keyTrigger] = new List<string>();
                    if (!this.allActionsByKeys[keyTrigger].Contains(actionID))
                        this.allActionsByKeys[keyTrigger].Add(actionID);
                }
            }
            if (this.allActions[actionID].Count == 0)
                this.allActions.Remove(actionID);
        }

        public void UnregisterAction(string actionID)
        {
            if (string.IsNullOrWhiteSpace(actionID))
                return;
            if (this.allActions.ContainsKey(actionID))
                this.allActions.Remove(actionID);
            foreach (Tuple<SButton, SButton> key in new List<Tuple<SButton, SButton>>(this.allActionsByKeys.Keys))
            {
                if (this.allActionsByKeys[key].Contains(actionID))
                    this.allActionsByKeys[key].Remove(actionID);
                if (this.allActionsByKeys[key].Count == 0)
                    this.allActionsByKeys.Remove(key);
            }
        }

        public List<string> GetActionsFromKey(SButton key)
        {
            Tuple<SButton, SButton> keyPair = new Tuple<SButton, SButton>(key, SButton.None);
            if (this.allActionsByKeys.ContainsKey(keyPair))
                return new List<string>(this.allActionsByKeys[keyPair]);
            return new List<string>();
        }

        public List<string> GetActionsFromKeyPair(Tuple<SButton, SButton> keyPair)
        {
            if (keyPair != null && this.allActionsByKeys.ContainsKey(keyPair))
                return new List<string>(this.allActionsByKeys[keyPair]);
            return new List<string>();
        }

        public List<Tuple<SButton, SButton>> GetKeyPairsFromActions(string actionID)
        {
            if (!string.IsNullOrWhiteSpace(actionID) && this.allActions.ContainsKey(actionID))
                return this.allActions[actionID];
            return new List<Tuple<SButton, SButton>>();
        }
    }
}
