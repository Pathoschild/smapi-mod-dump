/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

namespace LuckSkill.Framework
{
    /// <summary>A luck skill profession.</summary>
    internal class Profession : IProfession
    {
        /*********
        ** Accessors
        *********/
        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public string DefaultName { get; }

        /// <inheritdoc />
        public string DefaultDescription { get; }

        /// <inheritdoc />
        public string Name => this.DefaultName;

        /// <inheritdoc />
        public string Description => this.DefaultDescription;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="id"><inheritdoc cref="IProfession.Id" path="/summary"/></param>
        /// <param name="name"><inheritdoc cref="IProfession.Name" path="/summary"/></param>
        /// <param name="description"><inheritdoc cref="IProfession.Description" path="/summary"/></param>
        public Profession(int id, string name, string description)
        {
            this.Id = id;
            this.DefaultName = name;
            this.DefaultDescription = description;
        }
    }
}
