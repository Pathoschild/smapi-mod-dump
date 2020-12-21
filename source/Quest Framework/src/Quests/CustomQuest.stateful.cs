/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Newtonsoft.Json.Linq;
using QuestFramework.Framework.Helpers;
using QuestFramework.Framework.Store;
using QuestFramework.Quests.State;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QuestFramework.Quests
{
    /// <summary>
    /// Custom quest definition with custom local state
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public class CustomQuest<TState> : CustomQuest, IStatefull<TState>, IStateRestorable where TState : class, new()
    {
        /// <summary>
        /// A state data
        /// </summary>
        public TState State { get; private set; }

        /// <summary>
        /// Create custom quest with state (Statefull quest)
        /// </summary>
        public CustomQuest() : base()
        {
            this.State = this.PrepareState();
        }

        /// <summary>
        /// Create custom quest with state (Statefull quest)
        /// </summary>
        /// <param name="name">Name of the quest</param>
        public CustomQuest(string name) : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Sync quest state with the store (singleplayer or mainplayer)
        /// or with host (server) via network in multiplayer game.
        /// 
        /// CALL THIS METHOD EVER WHEN YOU CHANGED QUEST STATE!
        /// </summary>
        public void Sync()
        {
            var payload = new StatePayload(
                questName: this.GetFullName(),
                farmerId: Game1.player.UniqueMultiplayerID,
                stateData: this.State is IPersistentState persistentState
                    ? persistentState.GetState()
                    : JObject.FromObject(this.State)
             );

            if (!Context.IsMainPlayer)
            {
                Helper.Multiplayer.SendMessage(
                    payload, "SyncState", new[] { QuestFrameworkMod.Instance.ModManifest.UniqueID });
                Monitor.Log($"Payload `{payload.QuestName}/{payload.FarmerId}` type `{payload.StateData.Type}` sent to sync to host.");
            }

            QuestFrameworkMod.Instance.QuestStateStore.Commit(payload);
            (this.State as IReactiveState)?.StateSynced();
        }

        void IStateRestorable.RestoreState(StatePayload payload)
        {
            if (payload.StateData == null)
            {
                this.ClearState();
                this.State = this.PrepareState();
                return;
            }

            if (this.State == null)
            {
                this.State = this.PrepareState();
            }

            if (this.State is IPersistentState persistentState)
            {
                persistentState.SetState(payload.StateData);
            }
            else
            {
                this.State = payload.StateData.ToObject<TState>();
            }
        }

        private void ClearState()
        {
            if (this.State is IDisposable disposableState)
            {
                disposableState.Dispose();
            }

            this.State = null;
        }

        bool IStateRestorable.VerifyState(StatePayload payload)
        {
            var localStateData = this.State is IPersistentState persistentState
                ? persistentState.GetState()
                : JObject.FromObject(this.State);

            return JToken.DeepEquals(payload.StateData, localStateData);
        }

        internal override void Update()
        {
            base.Update();

            if (this.State is IReactiveState activeState && activeState.WasChanged)
            {
                this.Sync();
            }
        }

        /// <summary>
        /// Creates and prepares new quest state.
        /// </summary>
        /// <returns></returns>
        protected virtual TState PrepareState()
        {
            var state = new TState();

            if (state is IReactiveState reactiveState)
            {
                reactiveState.Initialize(this);
                reactiveState.OnChange += this.StateChanged;
            }

            return state;
        }

        private void StateChanged(JObject state)
        {
            if (this.State is IReactiveState reactiveState)
            {
                this.NeedsUpdate = reactiveState.WasChanged;
            }
        }

        /// <inheritdoc cref="CustomQuest.Reset"/>
        public override void Reset()
        {
            base.Reset();

            if (this.State is IPersistentState resetableState)
            {
                resetableState.Reset();
                this.Sync();
                return;
            }

            if (this.State is IDisposable disposableState)
            {
                disposableState.Dispose();
            }

            this.State = this.PrepareState();
            this.Sync();
        }

        /// <summary>
        /// Reset quest state to default state data
        /// </summary>
        [Obsolete("Deprecated. Use method CustomQuest.Reset instead", true)]
        public void ResetState()
        {
            this.Reset();
        }
    }
}
