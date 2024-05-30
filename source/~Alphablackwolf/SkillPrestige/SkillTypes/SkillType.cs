/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Linq;
using SkillPrestige.Framework.SkillTypes;
using SkillPrestige.Logging;

namespace SkillPrestige.SkillTypes
{
    /// <summary>Represents a skill type in Stardew Valley (e.g. Farming, Fishing, Foraging).</summary>
    [Serializable]
    public partial class SkillType
    {
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local - setter used by deserializer.
        public string Name { get; set; }

        /// <summary>The ordinal and lookup used to get the skill type from Stardew Valley.</summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local - setter used by deserializer.
        public int Ordinal { get; set; }


        //spacecore skill identifier
        public string SpaceCoreSkillId { get; set; }

        public SkillType() { }

        // ReSharper disable once MemberCanBeProtected.Global - this time resharper is just out of it's gourd. this is used publically.
        public SkillType(string name, int ordinal)
        {
            this.Name = name;
            this.Ordinal = ordinal;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == this.GetType() && this.Equals((SkillType)obj);
        }

        public bool Equals(SkillType other)
        {
            return string.Equals(this.Name, other.Name) && this.Ordinal == other.Ordinal;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode -- used by deserializer only
                return ((this.Name?.GetHashCode() ?? 0) * 397) ^ this.Ordinal;
            }
        }

        public static bool operator ==(SkillType left, SkillType right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if ((object)left == null || (object)right == null)
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(SkillType left, SkillType right)
        {
            return !(left == right);
        }

        static SkillType()
        {
            Logger.LogInformation("Registering skill types...");
            var concreteSkillTypeRegistrations = AppDomain.CurrentDomain
                .GetNonSystemAssemblies()
                .SelectMany(x => x.GetTypesSafely())
                .Where(x => typeof(ISkillTypeRegistration).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
                .ToList();
            Logger.LogVerbose($"concrete skill type registration count: {concreteSkillTypeRegistrations.Count}");
            foreach (var registration in concreteSkillTypeRegistrations)
            {
                Logger.LogVerbose($"Creating instance of type {registration.FullName}...");
                ((ISkillTypeRegistration)Activator.CreateInstance(registration))?.RegisterSkillTypes();
            }
            Logger.LogInformation("Skill types registered.");
        }
    }
}

