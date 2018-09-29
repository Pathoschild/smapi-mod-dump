namespace TwilightShards.Common
{
    /// <summary>
    /// generic pair struct
    /// </summary>
    public class RangePair
    {
        /// <summary>
        /// Lower bound
        /// </summary>
        public double LowerBound;

        /// <summary>
        /// Higher bound
        /// </summary>
        public double HigherBound;

        /// <summary>
        /// Default constructor
        /// </summary>
        public RangePair()
        {

        }

        /// <summary>
        /// Constructor with option to enforce higher over lower
        /// </summary>
        /// <param name="l">Lower bound</param>
        /// <param name="h">Higher bound</param>
        public RangePair(double l, double h, bool EnforceHigherOverLower = false)
        {
            this.LowerBound = l;
            this.HigherBound = h;

            if (this.LowerBound > this.HigherBound)
            {
                double temp = this.LowerBound;
                this.LowerBound = this.HigherBound;
                this.HigherBound = temp;                
            }
        }

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="r">The value for this pair</param>
        public RangePair(double r)
        {
            this.LowerBound = this.HigherBound = r;
        }

        /// <summary>
        /// Copy constructor with option to enforce higher over lower.
        /// </summary>
        /// <param name="c">The object being copied</param>
        public RangePair(RangePair c, bool EnforceHigherOverLower = false)
        {
            this.LowerBound = c.LowerBound;
            this.HigherBound = c.HigherBound;

            if (EnforceHigherOverLower == true && this.LowerBound > this.HigherBound)
            {
                double temp = 0;
                temp = this.LowerBound;
                this.LowerBound = this.HigherBound;
                this.HigherBound = temp;
            }
        }

        /// <summary>
        /// Returns an item in range between Lower and Higher Bound
        /// </summary>
        /// <param name="d">The random object</param>
        /// <returns>Number in range</returns>
        public double RollInRange(MersenneTwister d)
        {
            if (HigherBound == LowerBound)
                return LowerBound;

            return (d.NextDoublePositive() * (HigherBound - LowerBound) + LowerBound);
        }
    }
}
