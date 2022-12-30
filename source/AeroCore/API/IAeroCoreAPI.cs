/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Particles;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace AeroCore.API
{
    public interface IAeroCoreAPI
    {
        public delegate void ActionHandler(Farmer who, string what, Point tile, GameLocation where);
        public delegate bool ParseDialogueDelegate(string[] tag_split, ref string substitution, Random r);

        public event Action<ILightingEventArgs> LightingEvent;
        public event Action<IUseItemEventArgs> UseItemEvent;
        public event Action<IUseObjectEventArgs> UseObjectEvent;
        public event Action<IHeldItemEventArgs> ItemHeldEvent;
        public event Action<IHeldItemEventArgs> StopItemHeldEvent;
        public event Action<GameLocation> LocationCleanup;
        public event Action<int> AfterFadeOut;
        public event Action<int> AfterFadeIn;

        /// <summary>Registers a custom action</summary>
        /// <param name="name">The name of the action</param>
        /// <param name="action">The code to run when the action is activated</param>
        /// <param name="cursor">Which cursor to use when hovering over this action.</param>
        public void RegisterAction(string name, ActionHandler action, int cursor = 0);

        /// <summary>Unregisters a registered action</summary>
        public void UnregisterAction(string name);

        /// <summary>Change the hover cursor of a given action</summary>
        /// <param name="name">The action to change the cursor of</param>
        /// <param name="which">The cursor index to change it to. check LooseSprites/Cursors for reference.</param>
        public void ChangeActionCursor(string name, int which);

        /// <summary>Register a custom touchaction</summary>
        /// <param name="name">The name of the touchaction</param>
        /// <param name="action">The code to run when the touchaction is activated</param>
        public void RegisterTouchAction(string name, ActionHandler action);

        /// <summary>Unregister a registered touch action</summary>
        public void UnregisterTouchAction(string name);

        /// <summary>Initializes all <see cref="ModInitAttribute"/> marked classes in your mod</summary>
        /// <param name="ModClass">Any type from your mod</param>
        public void InitAll(params object[] args);

        /// <summary>Builds and registers a config with GMCM if it is installed. Can be enhanced with attributes.</summary>
        /// <typeparam name="T">The config type</typeparam>
        /// <param name="config">The config instance</param>
        /// <param name="ConfigChanged">An action that is executed whenever settings are changed for this config</param>
        /// <param name="TitleScreenOnly">Whether or not it should only be available on the title screen</param>
        public void RegisterGMCMConfig<T>(IManifest who, IModHelper helper, T config, Action ConfigChanged = null, bool TitleScreenOnly = false) where T : class, new();

        /// <summary>Registers a custom behavior type which can be used by particle systems</summary>
        public void RegisterParticleBehavior(string name, Func<IParticleBehavior> factory);

        /// <summary>Registers a custom skin type which can be used by particle systems</summary>
        public void RegisterParticleSkin(string name, Func<IParticleSkin> factory);

        /// <summary>Creates a new particle system</summary>
        public IParticleManager CreateParticleSystem(string behavior, object behaviorArgs, string skin, object skinArgs, IParticleEmitter emitter, int count);

        /// <summary>Creates a new particle system from a game data asset</summary>
        public IParticleManager CreateParticleSystem(IGameContentHelper helper, string path, IParticleEmitter emitter, int count);

        /// <summary>Creates a new particle system from a mod data asset</summary>
        public IParticleManager CreateParticleSystem(IModContentHelper helper, string path, IParticleEmitter emitter, int count);

        /// <summary>Creates a new particle system from a json object</summary>
        public IParticleManager CreateParticleSystem(JObject json, IParticleEmitter emitter, int count);

        /// <summary>Checks a game query string.</summary>
        public bool CheckConditions(string condition_query, Random seeded_random = null, Farmer target_farmer = null, 
            Item target_item = null, GameLocation target_location = null);

        /// <summary>Registers a custom query type.</summary>
        public void RegisterQueryType(string query_name, Func<string[], bool> query_delegate);

        /// <summary>Parses tokenized text.</summary>
        public string ParseTokenText(string format, Random r = null, ParseDialogueDelegate handle_additional_tags = null, Farmer target_farmer = null);
        
        /// <summary>Wraps an <see cref="Item"/> in an <see cref="StardewValley.Object"/>. Save safe.
        /// Automatically unwrapped by most inventory methods. Will not wrap an Item if it is already an Object.</summary>
        /// <param name="forceSubtype">Whether to wrap subtypes (big craftables, furniture, etc.) even though they are Objects.</param>
        /// <returns>The wrapper containing the item.</returns>
        public StardewValley.Object WrapItem(Item what, bool forceSubtype = false);

        /// <summary>If the Object is an item wrapper, unwraps it. Otherwise, return it as-is.</summary>
        public Item UnwrapItem(StardewValley.Object what);

        /// <summary>Unwraps any wrapped items in a list.</summary>
        /// <returns>The number of items unwrapped.</returns>
        public int UnwrapItems(List<Item> items);

        /// <summary>Gets if an object is an item wrapper.</summary>
        public bool IsWrappedItem(StardewValley.Object what);

        /// <summary>Adds a one-time call after the current screen's fade out is done.</summary>
        public void AfterThisFadeOut(Action action);

        /// <summary>Adds a one-time call after the current screen's fade in is done.</summary>
        public void AfterThisFadeIn(Action action);

        /// <summary>Forces a spawned object to not be removed on sundays.</summary>
        public void PersistSpawnedObject(StardewValley.Object what, bool persist = true);
    }
}
