namespace CompostPestsCultivation
{
    public class Config
    {
        /// <summary>
        /// Chance of infestation for each crop each day (in %)
        /// </summary>
        public double pest_infestation_chance { set; get; } = 10; //was 0.1
        public double adjacent_infestation_chance { set; get; } = 25;

        public int processed_crops_for_cultivation_level { set; get; } = 3; //was 100

        public int minimal_speed_ungrow_chance { set; get; } = 20;
        public int speed_i_trait_grow_chance { set; get; } = 10;
        public int speed_ii_trait_grow_chance { set; get; } = 25;

        public int pest_resistance_i_chance { set; get; } = 50;
        public int pest_resistance_ii_chance { set; get; } = 100;
    }
}
