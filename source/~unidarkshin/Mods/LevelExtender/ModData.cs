namespace LevelExtender {
    public class ModData
    {
        public int FXP { get; set; }
        public int FLV { get; set; }
        public int FaXP { get; set; }
        public int FaLV { get; set; }
        public int MXP { get; set; }
        public int MLV { get; set; }
        public int CXP { get; set; }
        public int CLV { get; set; }
        public int FoXP { get; set; }
        public int FoLV { get; set; }
        public bool WorldMonsters { get; set; }
        public double Xp_modifier { get; set; }
        public ModData()
        {
            this.FXP = 0;
            this.FLV = 10;
            this.FaXP = 0;
            this.FaLV = 10;
            this.MXP = 0;
            this.MLV = 10;
            this.CXP = 0;
            this.CLV = 10;
            this.FoXP = 0;
            this.FoLV = 10;
            this.WorldMonsters = false;
            this.Xp_modifier = 1.0;
        }
    }
}