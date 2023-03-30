/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Models;
using AeroCore.Particles;
using AeroCore.Utils;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AeroCore.API
{
    public class API : IAeroCoreAPI
    {
        #region interface_accessible
        public event Action<ILightingEventArgs> LightingEvent;
        public event Action<IUseItemEventArgs> UseItemEvent;
        public event Action<IUseObjectEventArgs> UseObjectEvent;
        public event Action<IHeldItemEventArgs> ItemHeldEvent;
        public event Action<IHeldItemEventArgs> StopItemHeldEvent;
        public event Action<GameLocation> LocationCleanup;
        public event Action<int> AfterFadeOut;
        public event Action<int> AfterFadeIn;
        public event Action<SpriteBatch> OnDrawingWorld;

        public void RegisterAction(string name, IAeroCoreAPI.ActionHandler action, int cursor = 0)
        {
            Patches.Action.Actions[name] = action;
            ChangeActionCursor(name, cursor);
        }
        public void UnregisterAction(string name)
        {
            Patches.Action.Actions.Remove(name);
            Patches.Action.ActionCursors.Remove(name);
        }
        public void ChangeActionCursor(string name, int cursor)
        {
            cursor = Math.Clamp(cursor, 0, 7);
            if (cursor != 0)
                Patches.Action.ActionCursors[name] = cursor;
            else
                Patches.Action.ActionCursors.Remove(name);
        }
        public void RegisterTouchAction(string name, IAeroCoreAPI.ActionHandler action)
            => Patches.TouchAction.Actions.Add(name, action);
        public void UnregisterTouchAction(string name)
            => Patches.TouchAction.Actions.Remove(name);
        public void InitAll(params object[] args)
        {
            List<MethodInfo> calls = new();
            var ass = Assembly.GetCallingAssembly();
            if (ass is null || ass.FullName == "StardewModdingAPI")
                throw new Exception("Could not find caller! Do you need to use NoInling?");
            foreach (var type in ass.DefinedTypes)
            {
                var init = type.GetCustomAttribute<ModInitAttribute>();
                if (init is not null)
                {
                    string method = init.Method ?? "Init";
                    if (init.WhenHasMod is not null && !ModEntry.helper.ModRegistry.IsLoaded(init.WhenHasMod))
                        continue;
                    var m = AccessTools.DeclaredMethod(type, method);
                    if (m is not null && m.IsStatic)
                        calls.Add(m);
                }
            }
            ModEntry.monitor.Log($"Init scan complete, found {calls.Count} methods in {ass.FullName}.");
            for (int i = 0; i < calls.Count; i++)
            {
                var call = calls[i];
                try { call.Invoke(null, args); }
                catch (Exception e)
                {
                    ModEntry.monitor.Log($"Exception calling init '{call.DeclaringType.FullName}.{call.Name}':\n{e}", LogLevel.Error);
                }
            }
        }
        public void RegisterParticleBehavior(string name, Func<IParticleBehavior> factory)
            => knownPartBehaviors.Add(name, factory);

        public void RegisterParticleSkin(string name, Func<IParticleSkin> factory)
            => knownPartSkins.Add(name, factory);

        public IParticleManager CreateParticleSystem(string behavior, object behaviorArgs, string skin, object skinArgs, IParticleEmitter emitter, int count)
            => ParticleDefinition.Create(behavior, skin, behaviorArgs, skinArgs, emitter, count);

        public IParticleManager CreateParticleSystem(IGameContentHelper helper, string path, IParticleEmitter emitter, int count)
            => helper.Load<ParticleDefinition>(path).Create(emitter, count);

        public IParticleManager CreateParticleSystem(IModContentHelper helper, string path, IParticleEmitter emitter, int count)
            => helper.Load<ParticleDefinition>(path).Create(emitter, count);

        public IParticleManager CreateParticleSystem(JObject json, IParticleEmitter emitter, int count)
            => json.ToObject<ParticleDefinition>().Create(emitter, count);

        public bool CheckConditions(string condition_query, Random seeded_random = null, Farmer target_farmer = null,
            Item target_item = null, GameLocation target_location = null)
            => Backport.GameStateQuery.CheckConditions(condition_query, seeded_random, target_farmer, target_item, target_location);

        public void RegisterQueryType(string query_name, Func<string[], bool> query_delegate)
            => Backport.GameStateQuery.RegisterQueryType(query_name, query_delegate is null ? null : new(query_delegate));

        public string ParseTokenText(string format, Random r = null,
            IAeroCoreAPI.ParseDialogueDelegate handle_additional_tags = null, Farmer target_farmer = null)
            => Backport.TextParser.ParseText(format, r, 
                handle_additional_tags is null ? null : new(handle_additional_tags), 
                target_farmer);

        public StardewValley.Object WrapItem(Item what, bool forceSubtype = false)
            => Patches.ItemWrapper.WrapItem(what, forceSubtype);

        public Item UnwrapItem(StardewValley.Object what)
            => Patches.ItemWrapper.UnwrapItem(what);

        public int UnwrapItems(List<Item> items)
            => Patches.ItemWrapper.UnwrapItemsInList(items);

        public bool IsWrappedItem(StardewValley.Object what)
            => what.modData.ContainsKey(Patches.ItemWrapper.WrapFlag);

        public void AfterThisFadeOut(Action action)
            => afterFadeOut.Value.Add(action);

        public void AfterThisFadeIn(Action action)
            => afterFadeIn.Value.Add(action);

        public void PersistSpawnedObject(StardewValley.Object what, bool persist = true)
            => Patches.PersistSpawned.SetPersist(what, persist);

        public void RegisterGMCMConfig<T>(IManifest who, IModHelper helper, T config, Action ConfigChanged = null, bool TitleScreenOnly = false) where T : class, new()
            => Framework.ConfigFactory.BuildConfig<T>(who, helper, config, ConfigChanged, TitleScreenOnly);

        #endregion interface_accessible
        #region internals

        internal static readonly Dictionary<string, Func<IParticleBehavior>> knownPartBehaviors = new(StringComparer.OrdinalIgnoreCase);
        internal static readonly Dictionary<string, Func<IParticleSkin>> knownPartSkins = new(StringComparer.OrdinalIgnoreCase);
        internal static readonly PerScreen<List<Action>> afterFadeOut = new(() => new());
        internal static readonly PerScreen<List<Action>> afterFadeIn = new(() => new());
        internal API() {
            Patches.Lighting.LightingEvent += (e) => LightingEvent?.Invoke(e);
            Patches.UseItem.OnUseItem += (e) => UseItemEvent?.Invoke(e);
            Patches.UseObject.OnUseObject += (e) => UseObjectEvent?.Invoke(e);
            Patches.UseItem.OnItemHeld += (e) => ItemHeldEvent?.Invoke(e);
            Patches.UseItem.OnStopItemHeld += (e) => StopItemHeldEvent?.Invoke(e);
            Patches.LocationCleanup.Cleanup += (e) => LocationCleanup?.Invoke(e);
            Patches.FadeHooks.AfterFadeIn += (e) =>
            {
                AfterFadeIn?.Invoke(e);
                var once = afterFadeOut.GetValueForScreen(e);
                for (int i = once.Count - 1; i >= 0; i--)
                {
                    once[i]?.Invoke();
                    once.RemoveAt(i);
                }
            };
            Patches.FadeHooks.AfterFadeOut += (e) =>
            {
                AfterFadeOut?.Invoke(e);
                var once = afterFadeIn.GetValueForScreen(e);
                for (int i = once.Count - 1; i >= 0; i--)
                {
                    once[i]?.Invoke();
                    once.RemoveAt(i);
                }
            };

            knownPartBehaviors.Add("simple", () => new SimpleBehavior());
            knownPartSkins.Add("simple", () => new SimpleSkin());
        }
        internal void EmitWorldDraw(SpriteBatch batch) => OnDrawingWorld?.Invoke(batch);
        #endregion internals
    }
}
