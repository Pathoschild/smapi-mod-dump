using MegaStorage.Framework.Models;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaStorage.Framework.Persistence
{
    internal class StateManager
    {
        /*********
        ** Fields
        *********/
        public static event EventHandler PlayerAdded;
        public static event EventHandler PlayerRemoved;

        internal static readonly IDictionary<Tuple<GameLocation, Vector2>, CustomChest> PlacedChests =
            new Dictionary<Tuple<GameLocation, Vector2>, CustomChest>();
        internal static CustomChest MainChest
        {
            get
            {
                if (_mainChest is null || _mainChest.items.Count == 0)
                {
                    _mainChest = PlacedChests
                        .Where(c => c.Value is SuperMagicChest)
                        .OrderByDescending(c => c.Value.items.Count)
                        .First()
                        .Value;
                }

                return _mainChest;
            }
            set
            {
                if (!Context.IsMainPlayer || !Context.IsWorldReady)
                    return;
                _mainChest = value;
            }
        }

        private static CustomChest _mainChest;
        private static int _prevLength;

        /*********
        ** Public methods
        *********/
        public static void Start()
        {
            MegaStorageMod.ModHelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /*********
        ** Private methods
        *********/
        private static void OnUpdateTicked(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            var currentLength = MegaStorageMod.ModHelper.Multiplayer.GetConnectedPlayers().Count();
            if (currentLength > _prevLength)
            {
                PlayerAdded?.Invoke(null, null);
            }
            else if (currentLength < _prevLength)
            {
                PlayerRemoved?.Invoke(null, null);
            }
            _prevLength = currentLength;
        }
    }
}
