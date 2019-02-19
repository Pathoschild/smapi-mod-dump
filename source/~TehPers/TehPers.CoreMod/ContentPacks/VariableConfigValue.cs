using TehPers.CoreMod.Api.Environment;

namespace TehPers.CoreMod.ContentPacks {
    internal abstract class VariableConfigValue<T> {
        public abstract T Value { get; }

    }

    internal class ConstantConfigValue<T> : VariableConfigValue<T> {
        public override T Value { get; }

        public ConstantConfigValue(T value) {
            this.Value = value;
        }

        public void RegisterUpdates() {
             
        }
    }

    // internal class SeasonConfigValue : VariableConfigValue<Season> {
    // 
    //     public SeasonConfigValue(ModCore coreMod) {
    // 
    //     }
    // }
}
