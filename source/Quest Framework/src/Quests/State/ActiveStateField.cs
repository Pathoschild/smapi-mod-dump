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
using System;

namespace QuestFramework.Quests.State
{
    public abstract class ActiveStateField
    {
        private string _name;

        public string Name { 
            get => this._name;
            set
            {
                if (this._name != null)
                    throw new InvalidOperationException("This active state field is already named.");

                this._name = value;
            }
        }

        public Action<ActiveStateField> OnChange { get; internal set; }
        public abstract JToken ToJToken();
        public abstract void FromJToken(JToken token);
        public abstract void Reset();
    }

    public sealed class ActiveStateField<T> : ActiveStateField
    {
        private readonly T _defaultValue;
        private T _value;

        public ActiveStateField()
        {
            this._defaultValue = default;
            this._value = default;
        }

        public ActiveStateField(T initialValue)
        {
            this._defaultValue = initialValue;
            this._value = initialValue;
        }

        public ActiveStateField(T initialValue, string name) : this(initialValue)
        {
            this.Name = name;
        }

        public T Value
        {
            get => this._value;
            set
            {
                this._value = value;
                this.OnChange?.Invoke(this);
            }
        }

        public override JToken ToJToken()
        {
            return JToken.FromObject(this._value);
        }

        public override void Reset()
        {
            this._value = this._defaultValue;
            this.OnChange?.Invoke(this);
        }

        public override void FromJToken(JToken token)
        {
            this._value = token.ToObject<T>();
        }
    }
}
