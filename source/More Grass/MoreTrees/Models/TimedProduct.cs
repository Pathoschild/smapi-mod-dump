namespace MoreTrees.Models
{
    /// <summary>Represents a product that a tree drops with a number of days between each production.</summary>
    public class TimedProduct
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The number of days inbetween the product dropping.</summary>
        public int DaysBetweenProduce { get; set; }

        /// <summary>The product that will drop.</summary>
        public string Product { get; set; }
    }
}
