namespace Revitalize.Framework.Environment
{
    public class DarkerNightConfig
    {
        public bool Enabled;
        public float DarknessIntensity;
        public DarkerNightConfig()
        {
            this.Enabled = true;
            this.DarknessIntensity = .9f;
        }
    }
}
