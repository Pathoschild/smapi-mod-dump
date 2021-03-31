/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestFramework.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QuestFramework.Quests.State
{
    public sealed class ActiveState : IDisposable, IPersistentState, IReactiveState
    {
        private JObject _state;
        private Dictionary<string, ActiveStateField> _activeFields;

        [JsonIgnore]
        public bool WasChanged { get; private set; }

        public event Action<JObject> OnChange;
        public event Action<Dictionary<string, ActiveStateField>> OnUpdateFields;

        public ActiveState()
        {
            this._state = new JObject();
            this._activeFields = new Dictionary<string, ActiveStateField>();
        }

        public ActiveState WatchFields(params ActiveStateField[] fields)
        {
            foreach (var field in fields)
            {
                if (field == null)
                    throw new ActiveStateException($"Watched Field can't be null!");

                if (field.Name == null)
                    throw new ActiveStateException("Watched field has no given name. Do you forget add JsonProperty attribute for your active state field declaration?");

                this._activeFields.Add(field.Name, field);

                field.OnChange = (af) => this.UpdateState(field.Name, af.ToJToken());
            }

            return this;
        }

        private void UpdateState(string fieldName, JToken value)
        {
            this._state[fieldName] = value;
            this.WasChanged = true;
            this.OnChange?.Invoke(this._state);
        }

        private void UpdateFields()
        {
            foreach (var field in this._activeFields.Values)
            {
                if (this._state.ContainsKey(field.Name))
                {
                    field.FromJToken(this._state[field.Name]);
                }
            }

            this.OnUpdateFields?.Invoke(this._activeFields);
        }

        public JObject GetState()
        {
            return this._state;
        }

        public void SetState(JObject state)
        {
            this._state = state;
            this.UpdateFields();
        }

        public void Dispose()
        {
            foreach (var field in this._activeFields.Values)
            {
                if (field is IDisposable disposableField)
                    disposableField.Dispose();
            }

            this.OnChange = null;
            this._activeFields = null;
            this._state = null;
        }

        public void Reset()
        {
            foreach (var field in this._activeFields.Values)
                field.Reset();
        }

        public void StateSynced()
        {
            this.WasChanged = false;
        }

        public void Initialize(CustomQuest customQuest)
        {
            this.WatchFields(ActiveStateHelper.GatherActiveStateProperties(customQuest).ToArray());
            this.WatchFields(ActiveStateHelper.GatherActiveStateFields(customQuest).ToArray());
        }
    }
}
