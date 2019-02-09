using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.FishingOverhaul.Api.Overridables {
    public class Overridable2<T> {
        private T _overriddenValue = default(T);
        private bool _overridden = false;

        public virtual T Get(Func<T> defaultFactory) {
            return this._overridden ? this._overriddenValue : defaultFactory();
        }

        public virtual void Set(T overrideValue) {
            this._overridden = true;
            this._overriddenValue = overrideValue;
        }

        public virtual bool Reset() {
            if (this._overridden) {
                // Dispose of the overridden value if it needs to be disposed
                if (this._overriddenValue is IDisposable disposable) {
                    disposable.Dispose();
                }

                // Release any references (assuming it's a reference type, otherwise just set it to the default value)
                this._overridden = false;
                this._overriddenValue = default(T);
                return true;
            }

            return false;
        }
    }
}
