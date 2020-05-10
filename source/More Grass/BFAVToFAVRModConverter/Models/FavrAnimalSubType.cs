namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents an animal's sub type for FAVR.</summary>
    public class FavrAnimalSubType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the subtype.</summary>
        public string Name { get; set; }

        /// <summary>The produce for the sub type.</summary>
        public FavrAnimalProduce Produce { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the subtype.</param>
        /// <param name="produce">The produce for the sub type.</param>
        public FavrAnimalSubType(string name, FavrAnimalProduce produce)
        {
            Name = name;
            Produce = produce;
        }
    }
}
