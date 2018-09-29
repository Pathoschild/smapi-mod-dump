namespace SolarEclipseEvent
{
    public class EclipseConfig
    {
        public double EclipseChance { get; set; }
        public bool SpawnMonsters { get; set; }
        public bool SpawnMonstersAllFarms { get; set; }

        public EclipseConfig()
        {
            EclipseChance = .01;
            SpawnMonsters = true;
            SpawnMonstersAllFarms = false;
        }
    }
}
