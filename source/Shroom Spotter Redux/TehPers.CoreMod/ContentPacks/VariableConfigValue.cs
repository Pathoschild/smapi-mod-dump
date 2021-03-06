/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

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
