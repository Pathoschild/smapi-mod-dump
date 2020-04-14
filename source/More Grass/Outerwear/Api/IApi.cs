using System.Collections.Generic;

namespace Outerwear
{
    /// <summary>Provides basic outerwear apis.</summary>
    public interface IApi
    {
        /// <summary>Create an id for by the outerwear name, this is separate as it will also look in the file system for the static id map.</summary>
        /// <param name="outerwearName">The name of the outerwear to create the id of.</param>
        /// <returns>An id that is either from the static id map in the file system or an unused generated one.</returns>
        int CreateId(string outerwearName);

        /// <summary>Get metadata for all currently loaded outerwear.</summary>
        /// <returns>A list of outwear objects.</returns>
        List<Models.OuterwearData> GetAllOuterwearData();

        /// <summary>Get metadata for an outerwear by it's id.</summary>
        /// <param name="id">The id of the outerwear metadata to get.</param>
        /// <returns>The metadata for an outerwear with the respective id, or null if nothing was found.</returns>
        Models.OuterwearData GetOuterwearDataById(int id);

        /// <summary>Get metadata for an outerwear by it's name.</summary>
        /// <param name="name">The name of the outerwear metadata to get.</param>
        /// <returns>The metadata for an outerwear with the respective name, or null if nothing was found.</returns>
        Models.OuterwearData GetOuterwearDataByName(string name);
    }
}
