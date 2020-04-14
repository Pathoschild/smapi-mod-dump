namespace Outerwear.Models
{
    /// <summary>Metadata about save persistant id map.</summary>
    public class IdMap
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The id of the item.</summary>
        public int Id { get; set; }

        /// <summary>The name of the item.</summary>
        public string Name { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="id">The id of the item.</param>
        /// <param name="name">The name of the item.</param>
        public IdMap(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
