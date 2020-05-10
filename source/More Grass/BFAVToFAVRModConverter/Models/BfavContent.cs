using System.Collections.Generic;

namespace BFAVToFAVRModConverter.Models
{
    /// <summary>Represents BFAV's 'content.json' file for deserialization.</summary>
    public class BfavContent
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The content wrapper for each animal.</summary>
        public List<BfavCategory> Categories { get; set; }
    }
}
