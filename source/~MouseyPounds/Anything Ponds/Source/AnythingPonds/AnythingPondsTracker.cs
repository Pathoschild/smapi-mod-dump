namespace AnythingPonds
{
    // This is a dumb little object made necessary because I am stubborn and wanted to be able to keep the name
    //  of the parent location for my fishponds as well as their empty day count.
    // And yes, having four different constructors plus an increment method is complete overkill.
    class AnythingPondsTracker
    {
        public string locationName { get; set; }
        public int days { get; set; }

        public AnythingPondsTracker()
        {
            this.locationName = "Farm";
            this.days = 0;
        }

        public AnythingPondsTracker(string LocationName) : this()
        {
            this.locationName = LocationName;
        }

        public AnythingPondsTracker(int Days) : this()
        {
            this.days = Days;
        }

        public AnythingPondsTracker(string LocationName, int Days) : this()
        {
            this.locationName = LocationName;
            this.days = Days;
        }

        public void Increment()
        {
            this.days++;
        }
    }
}
