using System;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.FishingOverhaul.Api.Overridables {
    public class Overridable<T> {
        private readonly Func<T> _defaultFactory;
        private T _overrideValue;
        private bool _overridden;

        public T Value {
            get => this.Get();
            set => this.Set(value);
        }

        public virtual bool Overridden => this._overridden;

        public Overridable(Func<T> defaultFactory) {
            this._defaultFactory = defaultFactory;
        }

        public virtual void Set(T value) {
            this._overrideValue = value;
            this._overridden = true;
        }

        public virtual T Get() {
            return this.Overridden ? this._overrideValue : this._defaultFactory();
        }

        public virtual void Reset() {
            this._overridden = false;
        }
    }
}
