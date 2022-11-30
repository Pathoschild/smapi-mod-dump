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
using System.Linq;
using System.Security.Cryptography;
using InputTools;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;


namespace InputTools
{
    public class ControlStack
    {
        private InputToolsAPI inputTools;

        internal Dictionary<object, InputLayer> layerDict = new Dictionary<object, InputLayer>();
        internal List<object> layers = new List<object>();

        public ControlStack(InputToolsAPI inputTools)
        {
            this.inputTools = inputTools;
        }

        public InputLayer Create(object layerKey, bool startActive = true, IInputToolsAPI.BlockBehavior blockBehaviour = IInputToolsAPI.BlockBehavior.Block)
        {
            if (layerKey == null)
            {
                this.inputTools.Monitor.Log($"Layer key required to create an input layer", LogLevel.Warn);
                return null;
            }
            if (this.layers.Contains(layerKey))
                this.inputTools.Monitor.Log($"Layer {layerKey} is being created more than once - remove it first if it's intentional", LogLevel.Warn);
            this.layerDict[layerKey] = new InputLayer(this.inputTools, layerKey) { _isActive = startActive, _block = blockBehaviour };
            this.layers.Add(layerKey);
            return this.layerDict[layerKey];
        }

        public InputLayer Pop()
        {
            if (this.layers.Count == 0)
                return null;
            object layerKey = this.layers[this.layers.Count - 1];
            this.layers.Remove(layerKey);
            if (this.layerDict.ContainsKey(layerKey))
            {
                InputLayer poppedLayer = this.layerDict[layerKey];
                this.layerDict.Remove(layerKey);
                return poppedLayer;
            }
            return null;
        }

        public void Remove(object layerKey)
        {
            if (layerKey == null)
                return;
            if (this.layers.Contains(layerKey))
                this.layers.Remove(layerKey);
            else
                this.inputTools.Monitor.Log($"Tried to remove layer {layerKey} that hasn't been created", LogLevel.Warn);
            if (this.layerDict.ContainsKey(layerKey))
                this.layerDict.Remove(layerKey);
        }

        public IInputToolsAPI.IInputLayer Peek()
        {
            if (this.layers.Count == 0)
                return null;
            object layerKey = this.layers[this.layers.Count - 1];
            return this.Get(layerKey);
        }

        public IInputToolsAPI.IInputLayer Get(object layerKey)
        {
            if (layerKey == null)
                return this.inputTools.Global as InputLayer;
            if (this.layerDict.ContainsKey(layerKey))
                return this.layerDict[layerKey];
            return null;
        }

        public void MoveToTop(object layerKey)
        {
            if (layerKey == null)
                return;
            if (!this.layers.Contains(layerKey))
                return;
            this.layers.Remove(layerKey);
            this.layers.Add(layerKey);
        }

        public bool IsLayerReachableByInput(object layerKey)
        {
            InputLayer layer = this.Get(layerKey) as InputLayer;
            if (layer == null || !layer._isActive)
                return false;
            if (layerKey == null)
                return layer._isActive;
            if (layerKey != null && this.inputTools._Global._block == IInputToolsAPI.BlockBehavior.Block)
                return false;

            for (int i=this.layers.Count-1; i>=0; i--)
            {
                if (layerKey == this.layers[i])
                    return true;
                InputLayer layerI = this.Get(this.layers[i]) as InputLayer;
                if (layerI._block == IInputToolsAPI.BlockBehavior.Block)
                    break;
            }
            return false;
        }
    }
}
