namespace TwilightShards.Common
{
    /// <summary>
    /// generic pair class
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
    	   this.LowerBound = 0;
           this.HigherBound = 0;
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

            if (this.LowerBound > this.HigherBound && EnforceHigherOverLower)
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
                double temp = this.LowerBound;
                this.LowerBound = this.HigherBound;
                this.HigherBound = temp;
            }
        }
		
		///<summary>
		/// This function updates the range pair. Used to avoid creating more objects.
		/// </summary>
		/// <param name="lower">The lower of the new range pair</param>
		/// <param name="hhigher">The higher of the new range pair</param>
		public void UpdateRangePair(double lower, double higher)
		{
			this.LowerBound = lower;
			this.HigherBound = higher;
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

        /// <summary>
        /// This function returns the midpoint of the range.
        /// </summary>
        /// <returns>The mid point of the range</returns>
        public double GetMidPoint()
        {
            return ((LowerBound + HigherBound) / 2.0);
        }

        public bool IsDefault()
        {
            if (LowerBound == HigherBound)
                if (LowerBound == 0)
                {
                    return true;
                }
            return false;
        }
    }
}
