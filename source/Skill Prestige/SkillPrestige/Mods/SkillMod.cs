using System.Collections.Generic;

namespace SkillPrestige.Mods
{
    /// <summary>
    /// Base class for mods that provides IsFound functionality out of the box. 
    /// </summary>
    public abstract class SkillMod : ISkillMod
    {
        public abstract string DisplayName { get; }

        /// <summary>
        /// Unique Id of the mod, used to determine if the mod is loaded through SMAPI.
        /// </summary>
        protected abstract string UniqueId { get; }

        /// <summary>
        /// The additional skills added by this mod.
        /// </summary>
        public abstract IEnumerable<Skill> AdditionalSkills { get; }

        /// <summary>
        /// The additional prestige options added by this mod.
        /// </summary>
        public abstract IEnumerable<Prestige> AdditonalPrestiges { get; }

        /// <summary>
        /// Whether or not the mod is found in SMAPI. By default it attempts to get the type by the namespace and class name.
        /// When implementing this for a mod external to this program, you can also choose to return true, 
        /// but you should only do this if your mod is loaded in the same assembly as this registration.
        /// </summary>
        // ReSharper disable once VirtualMemberNeverOverridden.Global - meant to be potentially overridden in externally inherited class.
        public virtual bool IsFound => SkillPrestigeMod.ModRegistry.IsLoaded(UniqueId);
    }
}
